// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Daf.Meta.Layers;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Daf.Meta.Editor.ViewModels
{
	public class HubsViewModel : ObservableObject
	{
		public HubsViewModel()
		{
			_mbService = Ioc.Default.GetService<IMessageBoxService>()!;
			_windowService = Ioc.Default.GetService<IWindowService>()!;

			AddHubCommand = new RelayCommand<Type?>(OpenAddHubDialog);
			DeleteHubCommand = new RelayCommand(OpenDeleteHubDialog, CanDeleteHub);
			AddHubColumnCommand = new RelayCommand(OpenAddHubColumnDialog);
			DeleteHubColumnCommand = new RelayCommand(DeleteHubColumn, CanDeleteHubColumn);
			RenameHubCommand = new RelayCommand<Type?>(RenameHub);
		}

		private readonly IMessageBoxService _mbService;
		private readonly IWindowService _windowService;

		public RelayCommand<Type?> AddHubCommand { get; }
		public RelayCommand DeleteHubCommand { get; }
		public RelayCommand<Type?> RenameHubCommand { get; }
		public RelayCommand AddHubColumnCommand { get; }
		public RelayCommand DeleteHubColumnCommand { get; }


		private ObservableCollection<HubViewModel> _hubs = new();

		public ObservableCollection<HubViewModel> Hubs
		{
			get
			{
				return _hubs;
			}
			set
			{
				SetProperty(ref _hubs, value);

				if (SelectedHub == null)
					SelectedHub = Hubs.FirstOrDefault();
			}
		}

		private HubViewModel? _selectedHub;

		public HubViewModel? SelectedHub
		{
			get { return _selectedHub; }
			set
			{
				SetProperty(ref _selectedHub, value);

				DeleteHubCommand.NotifyCanExecuteChanged();

				if (_selectedHub == null)
					HubSelected = false;
				else
					HubSelected = true;
			}
		}

		private bool _hubSelected;

		public bool HubSelected
		{
			get { return _hubSelected; }
			set
			{
				SetProperty(ref _hubSelected, value);
			}
		}

		private BusinessKeyViewModel? _selectedHubColumn;

		public BusinessKeyViewModel? SelectedHubColumn
		{
			get { return _selectedHubColumn; }
			set
			{
				SetProperty(ref _selectedHubColumn, value);

				DeleteHubColumnCommand.NotifyCanExecuteChanged();
			}
		}

		private List<BusinessKeyViewModel>? _selectedHubColumns;
		public List<BusinessKeyViewModel>? SelectedHubColumns
		{
			get { return _selectedHubColumns; }
			set
			{
				SetProperty(ref _selectedHubColumns, value);

				DeleteHubColumnCommand.NotifyCanExecuteChanged();
			}
		}

		private void OpenAddHubDialog(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			AddOrEditViewModel viewModel = new()
			{
				Title = "Add New Hub"
			};

			bool dialogResult = _windowService.ShowDialog(windowType, viewModel);

			if (dialogResult)
			{
				AddHub(viewModel.Name);
			}
		}

		internal void AddHub(string name)
		{
			Hub hub = new(name);

			// Inserting alphabetically is much more convoluted but there may be reasons to do it, uncertain.
			// Create new ViewModel.
			Hubs.Insert(0, new HubViewModel(hub));

			// Tell MainViewModel to add a new Hub to the collection.
			WeakReferenceMessenger.Default.Send(new AddHubToModel(hub));

			SelectedHub = Hubs[0];
		}

		private bool CanDeleteHub()
		{
			if (SelectedHub == null)
				return false;
			else
				return true;
		}

		private void OpenDeleteHubDialog()
		{
			// TODO: We need to ensure that the hub isn't being referenced by any link or satellite.
			if (SelectedHub != null)
			{
				//WeakReferenceMessenger.Default.Send<DeleteHub>();
				string msg = $"Are you sure you wish to delete {SelectedHub.Name}?";
				MessageBoxResult result = _mbService.Show(msg, "Delete hub?", MessageBoxButton.YesNo, MessageBoxImage.Warning);

				if (result == MessageBoxResult.Yes)
				{
					DeleteHub(SelectedHub);
					SelectedHub = Hubs.FirstOrDefault();
				}
			}
		}

		internal void DeleteHub(HubViewModel SelectedHub)
		{
			// TODO: Communicate the removal to other lists? Gotta keep 'em in sync!
			// Remove Hub in Model.
			WeakReferenceMessenger.Default.Send(new DeleteHub(SelectedHub.Hub));
			// To be safe, need to confirm that Hub has actually been deleted.
			// Also, Hub needs to be deleted before the HubViewModel can be deleted.

			// Remove HubViewModel
			Hubs.Remove(SelectedHub);
		}

		private void RenameHub(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			AddOrEditViewModel viewModel = new()
			{
				Title = "Rename Hub",
				Name = SelectedHub!.Name
			};

			bool dialogResult = _windowService.ShowDialog(windowType, viewModel);

			if (dialogResult)
			{
				if (SelectedHub == null)
					throw new InvalidOperationException("SelectedHub was null");

				SelectedHub.Name = viewModel.Name;
			}
		}

		private void OpenAddHubColumnDialog()
		{
			if (SelectedHub == null)
			{
				string msg = "Please select or create a hub before adding a column!";
				_mbService.Show(msg, "Adding column to non-existing Hub", MessageBoxButton.OK, MessageBoxImage.Information);

				//WeakReferenceMessenger.Default.Send<AddHubColumnFailed>();
			}
			else
				AddHubColumn();
		}

		internal void AddHubColumn()
		{
			if (SelectedHub == null)
				throw new ArgumentNullException();
			else
			{
				StagingColumn stagingColumn = SelectedHub.Hub.AddBusinessKeyColumn();
				SelectedHub.BusinessKeys.Add(new BusinessKeyViewModel(stagingColumn));
			}
		}

		internal void DeleteHubColumn()
		{
			if (SelectedHub == null || SelectedHubColumn == null)
				throw new InvalidOperationException("Either SelectedHub or SelectedHubColumn was null.");
			else
			{
				SelectedHub.Hub.RemoveBusinessKeyColumn(SelectedHubColumn.StagingColumn);
				SelectedHub.BusinessKeys.Remove(SelectedHubColumn);
			}
		}

		private bool CanDeleteHubColumn()
		{
			if (SelectedHubColumn == null)
				return false;
			else
				return true;
		}
	}
}
