// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.ObjectModel;
using Daf.Meta.Layers;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Daf.Meta.Editor.ViewModels
{
	public class HubRelationshipsViewModel : ObservableObject
	{
		private readonly IMessageBoxService _mbService;
		private readonly IWindowService _windowService;

		public RelayCommand<Type?> AddHubRelationshipCommand { get; }
		public RelayCommand DeleteHubRelationshipCommand { get; }

		public HubRelationshipsViewModel()
		{
			_mbService = Ioc.Default.GetService<IMessageBoxService>()!;
			_windowService = Ioc.Default.GetService<IWindowService>()!;

			AddHubRelationshipCommand = new RelayCommand<Type?>(OpenAddHubRelationshipDialog);
			DeleteHubRelationshipCommand = new RelayCommand(OpenDeleteHubRelationshipDialog, CanDeleteHubRelationship);
		}

		private ObservableCollection<HubRelationshipViewModel>? _hubRelationships;

		public ObservableCollection<HubRelationshipViewModel>? HubRelationships
		{
			get => _hubRelationships;
			set
			{
				SetProperty(ref _hubRelationships, value);
			}
		}

		private HubRelationshipViewModel? _selectedHubRelationship;
		public HubRelationshipViewModel? SelectedHubRelationship
		{
			get => _selectedHubRelationship;
			set
			{
				SetProperty(ref _selectedHubRelationship, value);

				DeleteHubRelationshipCommand.NotifyCanExecuteChanged();
			}
		}

		private DataSource? _selectedDataSource;

		public DataSource? SelectedDataSource
		{
			get => _selectedDataSource;
			set
			{
				SetProperty(ref _selectedDataSource, value);
			}
		}

		public ObservableCollection<StagingColumn>? StagingColumns
		{
			get { return new ObservableCollection<StagingColumn>(SelectedDataSource?.StagingTable?.Columns!); }
		}

		private void OpenAddHubRelationshipDialog(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			if (SelectedDataSource == null)
				throw new InvalidOperationException();

			bool dialogResult = _windowService.ShowDialog(windowType, out object dataContext);

			if (dialogResult)
			{
				Hub hub = ((AddHubRelationshipViewModel)dataContext!).SelectedHub!;

				AddHubRelationship(hub, SelectedDataSource);
			}
		}

		internal static void AddHubRelationship(Hub hub, DataSource dataSource) // TODO: Fix app crashing when no hub is selected in AddHub-window.
		{
			WeakReferenceMessenger.Default.Send(new AddHubRelationship(hub, dataSource));

			//HubRelationships.Add(new HubRelationshipViewModel())
			// MainViewModel should automatically regenerate the list of HubRelationships at this point?

			//StagingColumn stagingColumn = inputDialog.StagingColumn;


			//foreach (var x in selectedDataSource.HubRelationships)
			//{
			//	foreach (var y in x.Mappings)
			//	{
			//		if (y.StagingColumn == stagingColumn)
			//		{
			//			string msg = $"Staging column {stagingColumn.Name} already has a hub assigned! No change was made.";
			//			MessageBoxResult result =
			//			  MessageBox.Show(
			//				msg,
			//				"Column already has a hub assigned",
			//				MessageBoxButton.OK,
			//				MessageBoxImage.Exclamation);

			//			return;
			//		}
			//	}
			//}

			//foreach (StagingColumn column in selectedDataSource.StagingTable.Columns)
			//{
			//	if (column == stagingColumn)
			//	{

			// keeping below for reference.
			//HubRelationship hubRelationship = new(hub);

			//foreach (StagingColumn bk in hub.BusinessKeys)
			//{
			//	HubMapping hubMapping = new(bk);

			//	hubMapping.PropertyChanged += (s, e) =>
			//	{
			//		hubRelationship.NotifyPropertyChanged("HubMapping");
			//	};

			//	hubRelationship.Mappings.Add(hubMapping);
			//}

			//if (SelectedDataSource == null)
			//	throw new InvalidOperationException("SelectedDataSource was null!");


			//hubRelationship.PropertyChanged += (s, e) =>
			//{
			//	SelectedDataSource.NotifyPropertyChanged("HubRelationship");
			//};

			//SelectedDataSource.HubRelationships.Add(hubRelationship);

			// TODO: businessKeyComboBox is in Satellite, we need to send it a message to run the equivalent command.
			//businessKeyComboBox.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();

			//break;
			//}
			//}
		}

		private bool CanDeleteHubRelationship()
		{
			if (SelectedHubRelationship == null)
				return false;
			else
				return true;
		}

		private void OpenDeleteHubRelationshipDialog()
		{
			if (SelectedHubRelationship == null || SelectedDataSource == null)
				throw new InvalidOperationException("SelectedHubRelationship or SelectedDataSource was null!");

			DeleteHubRelationship(SelectedHubRelationship, SelectedDataSource);
		}

		private static void DeleteHubRelationship(HubRelationshipViewModel hubRelationshipViewModel, DataSource dataSource)
		{
			WeakReferenceMessenger.Default.Send(new RemoveHubRelationship(hubRelationshipViewModel.HubRelationship, dataSource));
		}
	}
}
