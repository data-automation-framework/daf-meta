// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.ObjectModel;
using Daf.Meta.Layers;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace Daf.Meta.Editor.ViewModels
{
	public class HubRelationshipViewModel : ObservableObject
	{
		private readonly IMessageBoxService _mbService;
		private readonly IWindowService _windowService;

		public RelayCommand<Type?> AddHubRelationshipCommand { get; }
		public RelayCommand DeleteHubRelationshipCommand { get; }

		public HubRelationshipViewModel(IMessageBoxService mbService, IWindowService windowService)
		{
			_mbService = mbService;
			_windowService = windowService;

			AddHubRelationshipCommand = new RelayCommand<Type?>(AddHubRelationship);
			DeleteHubRelationshipCommand = new RelayCommand(DeleteHubRelationship, CanDeleteHubRelationship);
			_mbService = mbService;
			_windowService = windowService;
		}

		private ObservableCollection<HubRelationship>? _hubRelationships;

		public ObservableCollection<HubRelationship>? HubRelationships
		{
			get
			{
				return _hubRelationships;
			}
			set
			{
				SetProperty(ref _hubRelationships, value);
			}
		}

		private HubRelationship? _selectedHubRelationship;
		public HubRelationship? SelectedHubRelationship
		{
			get
			{
				return _selectedHubRelationship;
			}
			set
			{
				SetProperty(ref _selectedHubRelationship, value);

				DeleteHubRelationshipCommand.NotifyCanExecuteChanged();
			}
		}

		private DataSource? _selectedDataSource;

		public DataSource? SelectedDataSource
		{
			get
			{
				return _selectedDataSource;
			}
			set
			{
				SetProperty(ref _selectedDataSource, value);
			}
		}

		public ObservableCollection<StagingColumn>? StagingColumns
		{
			get { return new ObservableCollection<StagingColumn>(SelectedDataSource?.StagingTable?.Columns!); }
		}

		private void AddHubRelationship(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			if (SelectedDataSource == null)
				throw new InvalidOperationException();

			bool dialogResult = _windowService.ShowDialog(windowType, out object dataContext);

			if (dialogResult)
			{
				//StagingColumn stagingColumn = inputDialog.StagingColumn;

				Hub hub = ((AddHubRelationshipViewModel)dataContext!).SelectedHub!;

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
				HubRelationship hubRelationship = new(hub);

				foreach (StagingColumn bk in hub.BusinessKeys)
				{
					HubMapping hubMapping = new(bk);

					hubMapping.PropertyChanged += (s, e) =>
					{
						hubRelationship.NotifyPropertyChanged("HubMapping");
					};

					hubRelationship.Mappings.Add(hubMapping);
				}

				hubRelationship.PropertyChanged += (s, e) =>
				{
					SelectedDataSource.NotifyPropertyChanged("HubRelationship");
				};

				SelectedDataSource.HubRelationships.Add(hubRelationship);

				// TODO: businessKeyComboBox is in Satellite, we need to send it a message to run the equivalent command.
				//businessKeyComboBox.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();

				//break;
				//}
				//}
			}
		}

		private bool CanDeleteHubRelationship()
		{
			if (SelectedHubRelationship == null)
				return false;
			else
				return true;
		}

		private void DeleteHubRelationship()
		{
			if (SelectedDataSource != null && SelectedHubRelationship != null)
			{
				foreach (HubMapping hubMapping in SelectedHubRelationship.Mappings)
				{
					hubMapping.ClearSubscribers();
				}

				SelectedHubRelationship.ClearSubscribers();

				SelectedDataSource.HubRelationships.Remove(SelectedHubRelationship);

				// TODO: businessKeyComboBox is in Satellite, we need to send it a message to run the equivalent command.
				//businessKeyComboBox.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
			}
		}
	}
}
