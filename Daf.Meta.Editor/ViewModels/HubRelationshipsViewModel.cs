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

		private ObservableCollection<HubRelationshipViewModel> _hubRelationships = new();

		public ObservableCollection<HubRelationshipViewModel> HubRelationships
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
			// This needs to be fixed so it refers to ColumnViewModels instead, but have to make View Model for DataSource first.
			get { return new ObservableCollection<StagingColumn>(SelectedDataSource?.StagingTable?.Columns!); }
		}

		private void OpenAddHubRelationshipDialog(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			if (SelectedDataSource == null)
				throw new InvalidOperationException("SelectedDataSource was null!");

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

		private void DeleteHubRelationship(HubRelationshipViewModel hubRelationshipViewModel, DataSource dataSource)
		{
			if (HubRelationships == null)
				throw new InvalidOperationException("HubRelationships was null!");

			HubRelationships.Remove(hubRelationshipViewModel);

			WeakReferenceMessenger.Default.Send(new RemoveHubRelationship(hubRelationshipViewModel.HubRelationship, dataSource));
		}
	}
}
