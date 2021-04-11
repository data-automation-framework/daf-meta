// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Daf.Meta.Layers;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Daf.Meta.Editor.ViewModels
{
	public class LinksViewModel : ObservableObject
	{
		public LinksViewModel(IMessageBoxService mbService, IWindowService windowService)
		{
			_mbService = mbService;
			_windowService = windowService;

			AddLinkCommand = new RelayCommand<Type?>(OpenAddLinkDialog);
			DeleteLinkCommand = new RelayCommand(OpenDeleteLinkDialog, CanDeleteLink);
			AddLinkColumnCommand = new RelayCommand(OpenAddLinkColumnDialog);
			DeleteLinkColumnCommand = new RelayCommand(DeleteLinkColumn, CanDeleteLinkColumn);
			RenameLinkCommand = new RelayCommand<Type?>(RenameLink);
			_mbService = mbService;
			_windowService = windowService;
		}

		private readonly IMessageBoxService _mbService;
		private readonly IWindowService _windowService;

		public RelayCommand<Type?> AddLinkCommand { get; }
		public RelayCommand DeleteLinkCommand { get; }
		public RelayCommand<Type?> RenameLinkCommand { get; }
		public RelayCommand AddLinkColumnCommand { get; }
		public RelayCommand DeleteLinkColumnCommand { get; }

		private ObservableCollection<LinkViewModel> _links = new();

		public ObservableCollection<LinkViewModel> Links
		{
			get
			{
				return _links;
			}
			set
			{
				SetProperty(ref _links, value);

				if (SelectedLink == null)
					SelectedLink = Links.FirstOrDefault();
			}
		}

		private LinkViewModel? _selectedLink;

		public LinkViewModel? SelectedLink
		{
			get { return _selectedLink; }
			set
			{
				SetProperty(ref _selectedLink, value);

				DeleteLinkCommand.NotifyCanExecuteChanged();

				if (_selectedLink == null)
					LinkSelected = false;
				else
					LinkSelected = true;
			}
		}

		private bool _linkSelected;

		public bool LinkSelected
		{
			get { return _linkSelected; }
			set
			{
				SetProperty(ref _linkSelected, value);
			}
		}

		private BusinessKeyViewModel? _selectedLinkColumn;

		public BusinessKeyViewModel? SelectedLinkColumn
		{
			get { return _selectedLinkColumn; }
			set
			{
				SetProperty(ref _selectedLinkColumn, value);

				DeleteLinkColumnCommand.NotifyCanExecuteChanged();
			}
		}

		private List<BusinessKeyViewModel>? _selectedLinkColumns;

		public List<BusinessKeyViewModel>? SelectedLinkColumns
		{
			get { return _selectedLinkColumns; }
			set
			{
				SetProperty(ref _selectedLinkColumns, value);

				DeleteLinkColumnCommand.NotifyCanExecuteChanged();
			}
		}

		private void OpenAddLinkDialog(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			AddOrEditViewModel viewModel = new()
			{
				Title = "Add New Link"
			};

			bool dialogResult = _windowService.ShowDialog(windowType, viewModel);

			if (dialogResult)
			{
				AddLink(viewModel.Name);
			}
		}

		internal void AddLink(string name)
		{
			Link link = new(name);

			// Add to view model
			Links.Insert(0, new LinkViewModel(link));

			// Tell MainViewModel to add a new Link to the collection.
			WeakReferenceMessenger.Default.Send(new AddLinkToModel(link));

			SelectedLink = Links[0];
		}

		private bool CanDeleteLink()
		{
			if (SelectedLink == null)
				return false;
			else
				return true;
		}

		private void OpenDeleteLinkDialog()
		{
			// TODO: We need to ensure that the link isn't being referenced by any link or satellite.
			if (SelectedLink != null)
			{
				//WeakReferenceMessenger.Default.Send<DeleteLink>();
				string msg = $"Are you sure you wish to delete {SelectedLink.Name}?";
				MessageBoxResult result = _mbService.Show(msg, "Delete link?", MessageBoxButton.YesNo, MessageBoxImage.Warning);

				if (result == MessageBoxResult.Yes)
				{
					DeleteLink(SelectedLink);
					SelectedLink = Links.FirstOrDefault();
				}
			}
		}

		internal void DeleteLink(LinkViewModel SelectedLink)
		{
			// Remove Link in Model.
			WeakReferenceMessenger.Default.Send(new DeleteLink(SelectedLink.Link));
			// To be safe, need to confirm that Link has actually been deleted.
			// Also, Link needs to be deleted before the HubViewModel can be deleted.

			// Remove LinkViewModel
			Links.Remove(SelectedLink);
		}

		private void RenameLink(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			AddOrEditViewModel viewModel = new()
			{
				Title = "Rename Link",
				Name = SelectedLink!.Name
			};

			bool dialogResult = _windowService.ShowDialog(windowType, viewModel);

			if (dialogResult)
			{
				if (SelectedLink == null)
					throw new InvalidOperationException("SelectedLink was null");

				SelectedLink.Name = viewModel.Name;
			}
		}

		private void OpenAddLinkColumnDialog()
		{
			if (SelectedLink == null)
			{
				string msg = "Please select or create a link before adding a column!";
				_mbService.Show(msg, "Adding column to non-existing Link", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			else
			{
				AddLinkColumn();
			}
		}

		internal void AddLinkColumn()
		{
			if (SelectedLink == null)
			{
				throw new ArgumentNullException();
			}
			else
			{
				StagingColumn businessKey = new("New Column");

				// Send message to MainViewModel that the new BusinessKey needs to be added to its corresponding Hub in the Model.
				WeakReferenceMessenger.Default.Send(new AddBusinessKeyColumnToLink(SelectedLink.Link, businessKey));

				// Add new StagingColumnViewModel to HubsViewModel.Hubs and pass the new StagingColumn to its constructor.
				SelectedLink.BusinessKeys.Add(new BusinessKeyViewModel(businessKey));
			}
		}

		internal void DeleteLinkColumn()
		{
			if (SelectedLink == null || SelectedLinkColumn == null)
				throw new InvalidOperationException("Either SelectedLink or SelectedLinkColumn was null.");

			// Send message to MainViewModel that the BusinessKeyColumn needs to be deleted.
			WeakReferenceMessenger.Default.Send(new RemoveBusinessKeyColumnFromLink(SelectedLink.Link, SelectedLinkColumn.StagingColumn));

			// Delete hub column from view model.
			SelectedLink.BusinessKeys.Remove(SelectedLinkColumn);
		}

		private bool CanDeleteLinkColumn()
		{
			if (SelectedLinkColumn == null)
				return false;
			else
				return true;
		}
	}
}
