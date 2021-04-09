﻿// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Data.SqlClient;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Daf.Meta.Layers;
using Daf.Meta.Layers.Connections;
using Daf.Meta.Layers.DataSources;

namespace Daf.Meta.Editor.ViewModels
{
	public class MainViewModel : ObservableObject
	{
		private bool _isDirty;
		public bool IsDirty
		{
			get { return _isDirty; }
			set
			{
				SetProperty(ref _isDirty, value);

				SaveCommand.NotifyCanExecuteChanged();
			}
		}

		private Model _model;

		public Model Model
		{
			get
			{
				return _model;
			}
			set
			{
				SetProperty(ref _model, value);
			}
		}

		private readonly IMessageBoxService _mbService;
		private readonly IWindowService _windowService;

		public static string? MetadataPath { get; set; }

		public HubsViewModel HubsVM { get; }

		public LinksViewModel LinksVM { get; }

		public GeneralViewModel GeneralVM { get; }
		public LoadViewModel LoadVM { get; }
		public StagingViewModel StagingVM { get; }
		public HubRelationshipViewModel HubRelationshipVM { get; }
		public LinkRelationshipViewModel LinkRelationshipVM { get; }
		public SatelliteViewModel SatelliteVM { get; }

		public RelayCommand NewFileCommand { get; }
		public RelayCommand OpenFileCommand { get; }
		public RelayCommand SaveCommand { get; }
		public RelayCommand<Type?> DataWarehouseCommand { get; }
		public RelayCommand<Type?> ConnectionsCommand { get; }
		public RelayCommand<Type?> SourceSystemsCommand { get; }
		public RelayCommand<Type?> TenantsCommand { get; }
		public RelayCommand<Type?> AddDataSourceCommand { get; }
		public RelayCommand DeleteDataSourceCommand { get; }
		public RelayCommand<Type?> GetMetadataCommand { get; }
		public RelayCommand<Type?> CopyDataSourceCommand { get; }

		public MainViewModel()
		{
			_mbService = Ioc.Default.GetService<IMessageBoxService>()!;
			_windowService = Ioc.Default.GetService<IWindowService>()!;

			WeakReferenceMessenger.Default.Register<MainViewModel, DeleteHub>(this, (r, m) => DeleteHubFromModel(m.Hub));
			WeakReferenceMessenger.Default.Register<MainViewModel, AddHubToModel>(this, (r, m) => AddHubToModel(m.Hub));
			WeakReferenceMessenger.Default.Register<MainViewModel, RemoveBusinessKeyColumnFromHubs>(this, (r, m) => DeleteBusinessKeyFromHub(m.Hub, m.BusinessKey));
			WeakReferenceMessenger.Default.Register<MainViewModel, AddBusinessKeyColumnToHub>(this, (r, m) => AddBusinessKeyToHub(m.Hub, m.BusinessKey));

			WeakReferenceMessenger.Default.Register<MainViewModel, DeleteLink>(this, (r, m) => DeleteLinkFromModel(m.Link));
			WeakReferenceMessenger.Default.Register<MainViewModel, AddLinkToModel>(this, (r, m) => AddLinkToModel(m.Link));
			WeakReferenceMessenger.Default.Register<MainViewModel, RemoveBusinessKeyColumnFromLink>(this, (r, m) => DeleteBusinessKeyFromLink(m.Link, m.BusinessKey));
			WeakReferenceMessenger.Default.Register<MainViewModel, AddBusinessKeyColumnToLink>(this, (r, m) => AddBusinessKeyToLink(m.Link, m.BusinessKey));

			LoadMetadata();

			_model = Model.Instance;

			_model.PropertyChanged += (s, e) =>
			{
				IsDirty = true;
			};

			_dataSources = Model.DataSources;

			HubsVM = new HubsViewModel
			{
				Hubs = new(Model.Hubs.Select(hub => new HubViewModel(hub))),
			};

			LinksVM = new LinksViewModel
			{
				Links = new(Model.Links.Select(link => new LinkViewModel(link))),
			};

			GeneralVM = new GeneralViewModel();
			LoadVM = new LoadViewModel();
			StagingVM = new StagingViewModel();
			HubRelationshipVM = new HubRelationshipViewModel();
			LinkRelationshipVM = new LinkRelationshipViewModel();
			SatelliteVM = new SatelliteViewModel();

			NewFileCommand = new RelayCommand(NewFile);
			OpenFileCommand = new RelayCommand(OpenFile);
			SaveCommand = new RelayCommand(Save, CanSave);
			DataWarehouseCommand = new RelayCommand<Type?>(OpenDataWarehouseWindow);
			ConnectionsCommand = new RelayCommand<Type?>(OpenConnectionsWindow);
			SourceSystemsCommand = new RelayCommand<Type?>(OpenSourceSystemsWindow);
			TenantsCommand = new RelayCommand<Type?>(OpenTenantsWindow);
			AddDataSourceCommand = new RelayCommand<Type?>(AddDataSource);
			DeleteDataSourceCommand = new RelayCommand(DeleteDataSource, CanDeleteDataSource);
			GetMetadataCommand = new RelayCommand<Type?>(GetMetadata, CanGetMetadata);
			CopyDataSourceCommand = new RelayCommand<Type?>(CopyDataSource);
		}


		private ObservableCollection<DataSource> _dataSources;

		public ObservableCollection<DataSource> DataSources
		{
			get { return _dataSources; }
			set { SetProperty(ref _dataSources, value); }
		}

		private List<DataSource>? _selectedDataSources;

		public List<DataSource>? SelectedDataSources
		{
			get { return _selectedDataSources; }
			set
			{
				SetProperty(ref _selectedDataSources, value);

				GeneralVM.SelectedDataSources = value;
				LoadVM.SelectedDataSources = value;
				StagingVM.SelectedDataSources = value;

				if (_selectedDataSources == null)
				{
					DataSourceSelected = false;
				}
				else
					DataSourceSelected = true;
			}
		}

		private bool _dataSourceSelected;
		public bool DataSourceSelected
		{
			get { return _dataSourceSelected; }
			set
			{
				SetProperty(ref _dataSourceSelected, value);
			}
		}

		private DataSource? _selectedDataSourceSingle;

		public DataSource? SelectedDataSourceSingle
		{
			get
			{
				return _selectedDataSourceSingle;
			}
			set
			{
				SetProperty(ref _selectedDataSourceSingle, value);

				DeleteDataSourceCommand.NotifyCanExecuteChanged();
				GetMetadataCommand.NotifyCanExecuteChanged();

				LoadVM.SelectedDataSource = value;

				StagingVM.SelectedDataSource = value;

				HubRelationshipVM.SelectedDataSource = value;
				HubRelationshipVM.HubRelationships = value?.HubRelationships;

				LinkRelationshipVM.SelectedDataSource = value;
				LinkRelationshipVM.LinkRelationships = value?.LinkRelationships;

				SatelliteVM.SelectedDataSource = value;
				SatelliteVM.Satellites = value?.Satellites;
			}
		}

		private static void LoadMetadata()
		{
			if (MetadataPath != null)
				Model.Deserialize(MetadataPath);
			else
				Model.Initialize(); // Create an empty Model.
		}

		private void ResetViewModel()
		{
			Model = Model.Instance;
			IsDirty = false;

			Model.PropertyChanged += (s, e) =>
			{
				IsDirty = true;
			};

			DataSources = Model.DataSources;

			HubsVM.Hubs = new(Model.Hubs.Select(hub => new HubViewModel(hub)));
			LinksVM.Links = new(Model.Links.Select(link => new LinkViewModel(link)));

		}

		private void OpenDataWarehouseWindow(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			DataWarehouseViewModel dwViewModel = new(Model.DataWarehouse);

			_windowService.ShowDialog(windowType, dwViewModel);
		}

		private void OpenConnectionsWindow(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			ConnectionsViewModel connectionsViewModel = new(Model.Connections);

			_windowService.ShowDialog(windowType, connectionsViewModel);
		}

		private void OpenSourceSystemsWindow(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			SourceSystemsViewModel sourceSystemsViewModel = new(Model.SourceSystems);

			_windowService.ShowDialog(windowType, sourceSystemsViewModel);
		}

		private void OpenTenantsWindow(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			TenantsViewModel tenantsViewModel = new(Model.Tenants);

			_windowService.ShowDialog(windowType, tenantsViewModel);
		}

		private void AddDataSource(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			if (_windowService.ShowDialog(windowType, out object viewModel))
			{
				AddDataSourceViewModel dataSourceVM = (AddDataSourceViewModel)viewModel!;

				DataSource source;

				switch (dataSourceVM.SelectedDataSourceType)
				{
					case DataSourceType.Rest:
						source = new RestDataSource(dataSourceVM.Name, (RestConnection)dataSourceVM.SelectedConnection!, dataSourceVM.SelectedSourceSystem!, dataSourceVM.SelectedTenant!) { DataSourceType = dataSourceVM.SelectedDataSourceType };
						break;
					case DataSourceType.FlatFile:
						source = new FlatFileDataSource(dataSourceVM.Name, dataSourceVM.SelectedSourceSystem!, dataSourceVM.SelectedTenant!) { DataSourceType = dataSourceVM.SelectedDataSourceType };
						break;
					case DataSourceType.Script:
						source = new ScriptDataSource(dataSourceVM.Name, dataSourceVM.SelectedSourceSystem!, dataSourceVM.SelectedTenant!) { DataSourceType = dataSourceVM.SelectedDataSourceType };
						break;
					case DataSourceType.Sql:
						source = new SqlDataSource(dataSourceVM.Name, dataSourceVM.SelectedConnection!, dataSourceVM.SelectedSourceSystem!, dataSourceVM.SelectedTenant!) { DataSourceType = dataSourceVM.SelectedDataSourceType };
						break;
					case DataSourceType.GraphQl:
						source = new GraphQlDataSource(dataSourceVM.Name, (GraphQlConnection)dataSourceVM.SelectedConnection!, dataSourceVM.SelectedSourceSystem!, dataSourceVM.SelectedTenant!) { DataSourceType = dataSourceVM.SelectedDataSourceType };
						break;
					default:
						throw new NotImplementedException($"Data source type invalid for {dataSourceVM.Name}.");
				}

				Model.AddDataSource(source);
			}
		}

		private bool CanDeleteDataSource()
		{
			if (SelectedDataSourceSingle == null)
				return false;
			else
				return true;
		}

		private void DeleteDataSource()
		{
			if (SelectedDataSources!.FirstOrDefault() != null)
			{
				DataSource selectedDataSource = SelectedDataSources!.FirstOrDefault()!;

				if (MetadataPath == null)
					throw new ArgumentException("MetadataPath is null in RemoveDataSource_Click");

				// Todo: We should save the data sources that have been deleted to a list
				// and postpone removing them from disk until the user saves the data model.
				string msg = $"Are you sure you wish to delete {selectedDataSource.Name}? This will also erase its file from disk.";
				MessageBoxResult result = _mbService.Show(msg, "Delete data source?", MessageBoxButton.YesNo, MessageBoxImage.Warning);

				if (result == MessageBoxResult.Yes)
				{
					string dataSourcePath = Path.Combine(MetadataPath, "DataSources", $"{selectedDataSource.Name}.json");

					Model.RemoveDataSource(selectedDataSource);

					File.Delete(dataSourcePath);
				}
			}
		}

		private bool CanGetMetadata(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			if (SelectedDataSourceSingle == null)
				return false;
			else
				return true;
		}

		private void GetMetadata(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			if (SelectedDataSourceSingle == null)
				throw new InvalidOperationException();

			if (SelectedDataSourceSingle.StagingTable?.Columns.Count > 0 || SelectedDataSourceSingle.LoadTable?.Columns.Count > 0)
			{
				if (_windowService.ShowDialog(windowType, out _))
				{
					SelectedDataSourceSingle.LoadTable?.Columns.Clear();
					SelectedDataSourceSingle.StagingTable?.Columns.Clear();
				}
				else
					return; // If the user didn't click OK, return early.
			}

			try
			{
				SelectedDataSourceSingle.GetMetadata();

				WeakReferenceMessenger.Default.Send(new RefreshedMetadata());
			}
			catch (Exception ex) when (ex is SqlException or InvalidOperationException)
			{
				_mbService.Show($"Caught exception of type {ex.GetType()}: {ex.Message}", "Exception caught!", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void CopyDataSource(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			if (SelectedDataSourceSingle == null)
				throw new InvalidOperationException();

			Connection? connection = null;

			if (SelectedDataSourceSingle is RestDataSource rest)
			{
				connection = rest.Connection;
			}
			else if (SelectedDataSourceSingle is GraphQlDataSource graphQl)
			{
				connection = graphQl.Connection;
			}
			else if (SelectedDataSourceSingle is SqlDataSource sql)
			{
				connection = sql.Connection;
			}

			CopyDataSourceViewModel vm = new()
			{
				Name = SelectedDataSourceSingle.Name,
				SourceSystem = SelectedDataSourceSingle.SourceSystem,
				Tenant = SelectedDataSourceSingle.Tenant,
				Connection = connection
			};

			if (_windowService.ShowDialog(windowType, vm))
			{
				DataSource clonedDataSource = SelectedDataSourceSingle.Clone();

				clonedDataSource.Name = vm.Name;
				clonedDataSource.SourceSystem = vm.SourceSystem;
				clonedDataSource.Tenant = vm.Tenant;

				// Replace names with new ones
				clonedDataSource.FileName = $"{clonedDataSource.Tenant.ShortName}_{clonedDataSource.Name}";
				clonedDataSource.QualifiedName = $"{clonedDataSource.SourceSystem.ShortName}_{clonedDataSource.FileName}";

				foreach (Satellite cloneSatellite in clonedDataSource.Satellites)
				{
					cloneSatellite.Name = cloneSatellite.Name.Replace(SelectedDataSourceSingle.SourceSystem.ShortName, clonedDataSource.SourceSystem.ShortName);
					cloneSatellite.Name = cloneSatellite.Name.Replace(SelectedDataSourceSingle.FileName!, clonedDataSource.FileName);
				}

				if (clonedDataSource is RestDataSource restSource)
				{
					restSource.Connection = (RestConnection)vm.Connection!;
				}
				else if (clonedDataSource is GraphQlDataSource graphQlSource)
				{
					graphQlSource.Connection = (GraphQlConnection)vm.Connection!;
				}
				else if (clonedDataSource is SqlDataSource sqlSource)
				{
					sqlSource.Connection = (OleDBConnection)vm.Connection!;
				}

				Model.AddDataSource(clonedDataSource);
			}
		}

		private void NewFile()
		{
			// If data is dirty, notify user and ask for a response.
			if (IsDirty)
			{
				string msg = "You have unsaved changes. Open a new metadata file without saving?";
				MessageBoxResult saveResult = _mbService.Show(msg, "Warning - Unsaved changes!", MessageBoxButton.YesNo, MessageBoxImage.Warning);

				if (saveResult == MessageBoxResult.No)
				{
					// If user doesn't want to close, cancel.
					return;
				}
			}

			MetadataPath = null;
			_windowService.SetMainWindowTitle("Nucleus Data Vault 2.0 Metadata Editor - No save path selected");
			IsDirty = false;

			LoadMetadata();
			ResetViewModel();
		}

		private void OpenFile()
		{
			// If data is dirty, notify user and ask for a response.
			if (IsDirty)
			{
				string msg = "You have unsaved changes. Open a new metadata file without saving?";
				MessageBoxResult openFileResult = _mbService.Show(msg, "Warning - Unsaved changes!", MessageBoxButton.YesNo, MessageBoxImage.Warning);

				if (openFileResult == MessageBoxResult.No)
				{
					// If user doesn't want to close, cancel.
					return;
				}
			}

			System.Windows.Forms.DialogResult result = _windowService.OpenFile(out string path);

			if (result == System.Windows.Forms.DialogResult.OK)
			{
				// Check if directory contains Model.json, return if not.
				if (!File.Exists(Path.Combine(path, "Model.json")))
				{
					_mbService.Show("Folder does not contain Model.json. Please select a valid path.", "Error loading metadata", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				MetadataPath = path;
				_windowService.SetMainWindowTitle($"Nucleus Data Vault 2.0 Metadata Editor - {MetadataPath}");

				LoadMetadata();
				ResetViewModel();
			}
		}

		private bool CanSave()
		{
			return IsDirty;
		}

		private void Save()
		{
			foreach (DataSource dataSource in Model.DataSources)
			{
				if (!dataSource.IsValid(out string message))
				{
					_mbService.Show($"Failed to save data model due to errors in data source {dataSource.SourceSystem.ShortName}_{dataSource.Tenant.ShortName}_{dataSource.Name}: {message}", "Invalid data model", MessageBoxButton.OK);

					return;
				}
			}

			if (MetadataPath == null)
			{
				System.Windows.Forms.DialogResult result = _windowService.OpenFile(out string path);

				if (result == System.Windows.Forms.DialogResult.OK)
				{
					MetadataPath = path;
					_windowService.SetMainWindowTitle($"Nucleus Data Vault 2.0 Metadata Editor - {MetadataPath}");
				}
			}

			if (MetadataPath != null)
			{
				Model.Serialize(MetadataPath);

				IsDirty = false;
			}
		}

		/// <summary>
		/// Removes the Hub that is wrapped by HubViewModel.
		/// </summary>
		/// <param name="hub">The Hub object that will be removed from the Model.</param>
		private void DeleteHubFromModel(Hub hub)
		{
			if (Model.Hubs.Contains(hub))
				Model.Hubs.Remove(hub);
			else
				throw new InvalidOperationException("Attempted to delete hub which does not exist in Model.Hubs!");
		}

		/// <summary>
		/// Removes the Hub that is wrapped by HubViewModel.
		/// </summary>
		/// <param name="hub">The Hub object that will be added to the Model.</param>
		private void AddHubToModel(Hub hub)
		{
			Model.Hubs.Insert(0, hub);
		}

		/// <summary>
		/// Removes the StagingColumn that is wrapped by BusinessKeyViewModel.
		/// </summary>
		/// <param name="businessKey">The StagingColumn that will be deleted.</param>
		private void DeleteBusinessKeyFromHub(Hub hub, StagingColumn businessKey)
		{
			if (Model.Hubs.Contains(hub))
			{
				if (hub.BusinessKeys.Contains(businessKey))
				{
					hub.BusinessKeys.Remove(businessKey);
				}
				else
				{
					throw new InvalidOperationException("The specified Hub does not contain the specified BusinessKey in its list of BusinessKeys.");
				}
			}
			else
			{
				throw new InvalidOperationException("The specified Hub does not exist in Model.Hubs.");
			}
		}

		/// <summary>
		/// Adds a new StagingColumn to a specified Hub in the Model.
		/// </summary>
		/// <param name="hub">The hub that the StagingColumn will be added to.</param>
		/// <param name="businessKey">The StagingColumn that will be added.</param>
		private void AddBusinessKeyToHub(Hub hub, StagingColumn businessKey)
		{
			if (Model.Hubs.Contains(hub))
			{
				hub.BusinessKeys.Add(businessKey);
			}
			else
			{
				throw new InvalidOperationException("The specified Hub does not exist in Model.Hubs. Could not add StagingColumn.");
			}
		}

		/// <summary>
		/// Removes the Link that is wrapped by LinkViewModel.
		/// </summary>
		/// <param name="link">The Link object that will be removed from the Model.</param>
		private void DeleteLinkFromModel(Link link)
		{
			if (Model.Links.Contains(link))
				Model.Links.Remove(link);
			else
				throw new InvalidOperationException("Attempted to delete hub which does not exist in Model.Hubs!");
		}

		/// <summary>
		/// Removes the Link that is wrapped by LinkViewModel.
		/// </summary>
		/// <param name="hub">The Link object that will be added to the Model.</param>
		private void AddLinkToModel(Link link)
		{
			Model.Links.Insert(0, link);
		}

		/// <summary>
		/// Removes the StagingColumn that is wrapped by BusinessKeyViewModel.
		/// </summary>
		/// <param name="businessKey">The StagingColumn that will be deleted.</param>
		private void DeleteBusinessKeyFromLink(Link link, StagingColumn businessKey)
		{
			if (Model.Links.Contains(link))
			{
				if (link.BusinessKeys.Contains(businessKey))
				{
					link.BusinessKeys.Remove(businessKey);
				}
				else
				{
					throw new InvalidOperationException("The specified Link does not contain the specified BusinessKey in its list of BusinessKeys.");
				}
			}
			else
			{
				throw new InvalidOperationException("The specified Link does not exist in Model.Links.");
			}
		}

		/// <summary>
		/// Adds a new StagingColumn to a specified Link in the Model.
		/// </summary>
		/// <param name="link">The link that the StagingColumn will be added to.</param>
		/// <param name="businessKey">The StagingColumn that will be added.</param>
		private void AddBusinessKeyToLink(Link link, StagingColumn businessKey)
		{
			if (Model.Links.Contains(link))
			{
				link.BusinessKeys.Add(businessKey);
			}
			else
			{
				throw new InvalidOperationException("The specified Link does not exist in Model.Links. Could not add StagingColumn.");
			}
		}
	}
}
