// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Daf.Meta.Layers;
using Daf.Meta.Layers.Connections;
using Daf.Meta.Layers.DataSources;
using Microsoft.Data.SqlClient;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;

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
		public HubRelationshipsViewModel HubRelationshipsVM { get; }
		public LinkRelationshipsViewModel LinkRelationshipsVM { get; }
		public SatellitesViewModel SatellitesVM { get; }
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
			WeakReferenceMessenger.Default.Register<MainViewModel, AddHubToModel>(this, (r, m) => AddHubToModel(m.Name));

			WeakReferenceMessenger.Default.Register<MainViewModel, DeleteLink>(this, (r, m) => DeleteLinkFromModel(m.Link));
			WeakReferenceMessenger.Default.Register<MainViewModel, AddLinkToModel>(this, (r, m) => AddLinkToModel(m.Name));

			WeakReferenceMessenger.Default.Register<MainViewModel, RemoveConnection>(this, (r, m) => RemoveConnectionFromModel(m.Connection));
			WeakReferenceMessenger.Default.Register<MainViewModel, AddConnection>(this, (r, m) => AddConnectionToModel(m.Connection));

			WeakReferenceMessenger.Default.Register<MainViewModel, RemoveTenant>(this, (r, m) => RemoveTenantFromModel(m.Tenant));
			WeakReferenceMessenger.Default.Register<MainViewModel, AddTenant>(this, (r, m) => AddTenantToModel(m.Tenant));

			WeakReferenceMessenger.Default.Register<MainViewModel, RemoveSourceSystem>(this, (r, m) => RemoveSourceSystemFromModel(m.SourceSystem));
			WeakReferenceMessenger.Default.Register<MainViewModel, AddSourceSystem>(this, (r, m) => AddSourceSystemToModel(m.SourceSystem));

			WeakReferenceMessenger.Default.Register<MainViewModel, AddHubRelationship>(this, (r, m) => AddHubRelationshipToModel(m.Hub, m.DataSource));
			WeakReferenceMessenger.Default.Register<MainViewModel, RemoveHubRelationship>(this, (r, m) => RemoveHubRelationshipFromModel(m.HubRelationship, m.DataSource));

			WeakReferenceMessenger.Default.Register<MainViewModel, AddLinkRelationship>(this, (r, m) => AddLinkRelationshipToModel(m.Link, m.DataSource));
			WeakReferenceMessenger.Default.Register<MainViewModel, RemoveLinkRelationship>(this, (r, m) => RemoveLinkRelationshipFromModel(m.LinkRelationship, m.DataSource));

			LoadMetadata();

			_model = Model.Instance;

			_model.PropertyChanged += (s, e) =>
			{
				IsDirty = true;
			};

			_dataSources = GetDataSources(Model.DataSources);

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
			HubRelationshipsVM = new HubRelationshipsViewModel();
			LinkRelationshipsVM = new LinkRelationshipsViewModel();
			SatellitesVM = new SatellitesViewModel();

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


		private ObservableCollection<DataSourceViewModel> _dataSources;

		public ObservableCollection<DataSourceViewModel> DataSources
		{
			get { return _dataSources; }
			set { SetProperty(ref _dataSources, value); }
		}

		private List<DataSourceViewModel>? _selectedDataSources;

		public List<DataSourceViewModel>? SelectedDataSources
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

		private DataSourceViewModel? _selectedDataSourceSingle;

		public DataSourceViewModel? SelectedDataSourceSingle
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

				HubRelationshipsVM.SelectedDataSource = value;

				// TODO: I think this ought to be set from inside HubRelationshipsVM instead.
				if (value != null)
					HubRelationshipsVM.HubRelationships = new(value.DataSource.HubRelationships.Select(hubRelationship => new HubRelationshipViewModel(hubRelationship)));

				LinkRelationshipsVM.SelectedDataSource = value;

				if (value != null)
					LinkRelationshipsVM.LinkRelationships = new(value.DataSource.LinkRelationships.Select(linkRelationship => new LinkRelationshipViewModel(linkRelationship)));

				SatellitesVM.SelectedDataSource = value;
				if (value != null)
					SatellitesVM.Satellites = new(value.DataSource.Satellites.Select(satellite => new SatelliteViewModel(satellite)));
			}
		}

		private static void LoadMetadata()
		{
			if (MetadataPath != null)
			{
				if (File.Exists(Path.Combine(MetadataPath, "Model.json")))
					Model.Deserialize(MetadataPath);
				else
				{
					MetadataPath = null;
					MessageBox.Show("Stored Metadata path is invalid. Initializing an empty project instead.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
					Model.Initialize(); // Create an empty Model.
				}
			}
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

			DataSources = GetDataSources(Model.DataSources);

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

			ObservableCollection<ConnectionViewModel> viewModels = GetConnections(Model.Connections);

			ConnectionsViewModel connectionsViewModel = new(viewModels);

			_windowService.ShowDialog(windowType, connectionsViewModel);
		}

		internal static ObservableCollection<ConnectionViewModel> GetConnections(ObservableCollection<Connection> connections)
		{
			ObservableCollection<ConnectionViewModel> viewModels = new();

			foreach (Connection connection in connections)
			{
				switch (connection)
				{
					case RestConnection:
						viewModels.Add(new RestConnectionViewModel(connection));
						break;
					case OleDBConnection:
						viewModels.Add(new OleDBConnectionViewModel(connection));
						break;
					case OdbcConnection:
						viewModels.Add(new OdbcConnectionViewModel(connection));
						break;
					case MySqlConnection:
						viewModels.Add(new MySqlConnectionViewModel(connection));
						break;
					case GraphQlConnection:
						viewModels.Add(new GraphQlConnectionViewModel(connection));
						break;
					default:
						throw new InvalidOperationException("Invalid connection type");
				}
			}

			return viewModels;
		}

		internal static ObservableCollection<DataSourceViewModel> GetDataSources(ObservableCollection<DataSource> dataSources)
		{
			ObservableCollection<DataSourceViewModel> viewModels = new();

			foreach (DataSource dataSource in dataSources)
			{
				switch (dataSource)
				{
					case FlatFileDataSource:
						viewModels.Add(new FlatFileDataSourceViewModel(dataSource));
						break;
					case GraphQlDataSource:
						viewModels.Add(new GraphQlDataSourceViewModel(dataSource));
						break;
					//case JsonFileDataSource:
					//	viewModels.Add(new JsonFileDataSourceViewModel(dataSource));
					//	break;
					case RestDataSource:
						viewModels.Add(new RestDataSourceViewModel(dataSource));
						break;
					case ScriptDataSource:
						viewModels.Add(new ScriptDataSourceViewModel(dataSource));
						break;
					case SqlDataSource:
						viewModels.Add(new SqlDataSourceViewModel(dataSource));
						break;
					default:
						throw new InvalidOperationException("Invalid Data Source type");
				}
			}

			return viewModels;
		}

		private void OpenSourceSystemsWindow(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			ObservableCollection<SourceSystemViewModel> sourceSystems = new(Model.SourceSystems.Select(sourceSystem => new SourceSystemViewModel(sourceSystem)));

			SourceSystemsViewModel sourceSystemsViewModel = new(sourceSystems);

			_windowService.ShowDialog(windowType, sourceSystemsViewModel);
		}

		private void OpenTenantsWindow(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			ObservableCollection<TenantViewModel> tenants = new(Model.Tenants.Select(tenant => new TenantViewModel(tenant)));

			TenantsViewModel tenantsViewModel = new(tenants);

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

				AddDataSourceViewModel(source);
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
				DataSourceViewModel selectedDataSource = SelectedDataSources!.FirstOrDefault()!;

				if (MetadataPath == null)
					throw new ArgumentException("MetadataPath is null in RemoveDataSource_Click");

				// Todo: We should save the data sources that have been deleted to a list
				// and postpone removing them from disk until the user saves the data model.
				string msg = $"Are you sure you wish to delete {selectedDataSource.Name}? This will also erase its file from disk.";
				MessageBoxResult result = _mbService.Show(msg, "Delete data source?", MessageBoxButton.YesNo, MessageBoxImage.Warning);

				if (result == MessageBoxResult.Yes)
				{
					string dataSourcePath = Path.Combine(MetadataPath, "DataSources", selectedDataSource.SourceSystem.ShortName, selectedDataSource.Tenant.ShortName, $"{selectedDataSource.Name}.json");

					if (File.Exists(dataSourcePath))
					{
						Model.RemoveDataSource(selectedDataSource.DataSource);

						DataSources.Remove(selectedDataSource);

						File.Delete(dataSourcePath);
					}
					else
					{
						MessageBox.Show($"The specified file path does not exist. Data Source could not be deleted.\n\nFile path: {dataSourcePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
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

			if (SelectedDataSourceSingle.DataSource.StagingTable?.Columns.Count > 0 || SelectedDataSourceSingle.DataSource.LoadTable?.Columns.Count > 0)
			{
				if (_windowService.ShowDialog(windowType, out _))
				{
					SelectedDataSourceSingle.DataSource.LoadTable?.Columns.Clear();
					SelectedDataSourceSingle.DataSource.StagingTable?.Columns.Clear();
				}
				else
					return; // If the user didn't click OK, return early.
			}

			try
			{
				SelectedDataSourceSingle.DataSource.GetMetadata();

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

			if (SelectedDataSourceSingle.DataSource is RestDataSource rest)
			{
				connection = rest.Connection;
			}
			else if (SelectedDataSourceSingle.DataSource is GraphQlDataSource graphQl)
			{
				connection = graphQl.Connection;
			}
			else if (SelectedDataSourceSingle.DataSource is SqlDataSource sql)
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
				// Attempt to clone the Data Source. Catch the error and inform the user if they attempt to clone a Data Source type which does not have the Clone-method implemented.
				try
				{
					DataSource clonedDataSource = Model.CopyDataSource(SelectedDataSourceSingle.DataSource, vm.Name, vm.SourceSystem, vm.Tenant, vm.Connection!);

					AddDataSourceViewModel(clonedDataSource);
				}
				catch (NotImplementedException)
				{
					MessageBox.Show($"Clone method not implemented for Data Source Type: {SelectedDataSourceSingle.DataSource.DataSourceType}", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void AddDataSourceViewModel(DataSource dataSource)
		{
			switch (dataSource)
			{
				case FlatFileDataSource:
					DataSources.Add(new FlatFileDataSourceViewModel(dataSource));
					break;
				case GraphQlDataSource:
					DataSources.Add(new GraphQlDataSourceViewModel(dataSource));
					break;
				//case JsonFileDataSource:
				//	DataSources.Add(new JsonFileDataSourceViewModel(dataSource));
				//	break;
				case RestDataSource:
					DataSources.Add(new RestDataSourceViewModel(dataSource));
					break;
				case ScriptDataSource:
					DataSources.Add(new ScriptDataSourceViewModel(dataSource));
					break;
				case SqlDataSource:
					DataSources.Add(new SqlDataSourceViewModel(dataSource));
					break;
				default:
					throw new InvalidOperationException("Invalid connection type");
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
		/// Removes a specified Hub from the Model.
		/// </summary>
		/// <param name="hub">The Hub object that will be removed from the Model.</param>
		private void DeleteHubFromModel(Hub hub)
		{
			Model.RemoveHub(hub);
		}

		/// <summary>
		/// Adds a new Hub to the Model.
		/// </summary>
		/// <param name="name">The name of the Hub object that will be added to the Model.</param>
		private void AddHubToModel(string name)
		{
			Hub hub;

			if (string.IsNullOrEmpty(name))
				hub = Model.AddHub();
			else
				hub = Model.AddHub(name);

			HubsVM.Hubs.Insert(0, new HubViewModel(hub));
		}

		/// <summary>
		/// Removes a specified Link from the Model.
		/// </summary>
		/// <param name="link">The Link object that will be removed from the Model.</param>
		private void DeleteLinkFromModel(Link link)
		{
			Model.RemoveLink(link);
		}

		/// <summary>
		/// Adds a new Link to the Model.
		/// </summary>
		/// <param name="name">The name of the Link object that will be added to the Model.</param>
		private void AddLinkToModel(string name)
		{
			Link link;

			if (string.IsNullOrEmpty(name))
				link = Model.AddLink();
			else
				link = Model.AddLink(name);

			LinksVM.Links.Insert(0, new LinkViewModel(link));
		}

		/// <summary>
		/// Removes the Connection that is wrapped by ConnectionViewModel.
		/// </summary>
		/// <param name="connection">The Connection object that will be removed from the Model.</param>
		private void RemoveConnectionFromModel(Connection connection)
		{
			Model.RemoveConnection(connection);
		}

		/// <summary>
		/// Adds a new Connection to the Model.
		/// </summary>
		/// <param name="connection">The Connection object that will be added to the Model.</param>
		private void AddConnectionToModel(Connection connection)
		{
			Model.AddConnection(connection);
		}

		/// <summary>
		/// Removes the Tenant that is wrapped by TenantViewModel.
		/// </summary>
		/// <param name="tenant">The Tenant object that will be removed from the Model.</param>
		private void RemoveTenantFromModel(Tenant tenant)
		{
			Model.RemoveTenant(tenant);
		}

		/// <summary>
		/// Adds a new Tenant to the Model.
		/// </summary>
		/// <param name="tenant">The Tenant object that will be added to the Model.</param>
		private void AddTenantToModel(Tenant tenant)
		{
			Model.AddTenant(tenant);
		}

		/// <summary>
		/// Removes the SourceSystem that is wrapped by SourceSystemViewModel.
		/// </summary>
		/// <param name="sourceSystem">The SourceSystem object that will be removed from the Model.</param>
		private void RemoveSourceSystemFromModel(SourceSystem sourceSystem)
		{
			Model.RemoveSourceSystem(sourceSystem);
		}

		/// <summary>
		/// Adds a new SourceSystem to the Model.
		/// </summary>
		/// <param name="sourceSystem">The SourceSystem object that will be added to the Model.</param>
		private void AddSourceSystemToModel(SourceSystem sourceSystem)
		{
			Model.AddSourceSystem(sourceSystem);
		}

		/// <summary>
		/// Adds a new HubRelationship to the Model.
		/// </summary>
		/// <param name="hub">The Hub object associated with the new HubRelationship.</param>
		/// <param name="dataSource">The DataSource to which the HubRelationship will be added.</param>
		private void AddHubRelationshipToModel(Hub hub, DataSource dataSource)
		{
			HubRelationship hubRelationship = Model.AddHubRelationship(hub, dataSource);

			// Add View Model.
			if (HubRelationshipsVM.HubRelationships == null)
				throw new InvalidOperationException("HubRelationships was null!");

			// MainViewModel knows about HubRelationShipVM AND has access to the new hubRelationship.
			// That's why it makes sense to do this here.
			HubRelationshipsVM.HubRelationships.Add(new HubRelationshipViewModel(hubRelationship));
		}

		/// <summary>
		/// Adds a new HubRelationship to the Model.
		/// </summary>
		/// <param name="hub">The Hub object associated with the new HubRelationship.</param>
		/// <param name="dataSource">The DataSource to which the HubRelationship will be added.</param>
		private static void RemoveHubRelationshipFromModel(HubRelationship hub, DataSource dataSource)
		{
			Model.RemoveHubRelationship(hub, dataSource);
		}

		/// <summary>
		/// Adds a new LinkRelationship to the Model.
		/// </summary>
		/// <param name="link">The Link object associated with the new LinkRelationship.</param>
		/// <param name="dataSource">The DataSource to which the LinkRelationship will be added.</param>
		private void AddLinkRelationshipToModel(Link link, DataSource dataSource)
		{
			LinkRelationship linkRelationship = Model.AddLinkRelationship(link, dataSource);

			// Add View Model.
			if (LinkRelationshipsVM.LinkRelationships == null)
				throw new InvalidOperationException("LinkRelationships was null!");

			// MainViewModel knows about LinkRelationShipVM AND has access to the new linkRelationship.
			// That's why it makes sense to do this here.
			LinkRelationshipsVM.LinkRelationships.Add(new LinkRelationshipViewModel(linkRelationship));
		}

		/// <summary>
		/// Adds a new LinkRelationship to the Model.
		/// </summary>
		/// <param name="link">The Link object associated with the new LinkRelationship.</param>
		/// <param name="dataSource">The DataSource to which the LinkRelationship will be added.</param>
		private static void RemoveLinkRelationshipFromModel(LinkRelationship link, DataSource dataSource)
		{
			Model.RemoveLinkRelationship(link, dataSource);
		}
	}
}
