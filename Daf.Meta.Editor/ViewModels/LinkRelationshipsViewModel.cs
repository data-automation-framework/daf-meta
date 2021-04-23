// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Daf.Meta.Layers;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Daf.Meta.Editor.ViewModels
{
	public class LinkRelationshipsViewModel : ObservableObject
	{
		private readonly IMessageBoxService _mbService;
		private readonly IWindowService _windowService;

		public RelayCommand<Type?> AddLinkRelationshipCommand { get; }
		public RelayCommand DeleteLinkRelationshipCommand { get; }

		public LinkRelationshipsViewModel()
		{
			_mbService = Ioc.Default.GetService<IMessageBoxService>()!;
			_windowService = Ioc.Default.GetService<IWindowService>()!;

			AddLinkRelationshipCommand = new RelayCommand<Type?>(OpenAddLinkRelationshipDialog);
			DeleteLinkRelationshipCommand = new RelayCommand(OpenDeleteLinkRelationshipDialog, CanDeleteLinkRelationship);
		}

		private ObservableCollection<LinkRelationshipViewModel>? _linkRelationships;

		public ObservableCollection<LinkRelationshipViewModel>? LinkRelationships
		{
			get
			{
				return _linkRelationships;
			}
			set
			{
				SetProperty(ref _linkRelationships, value);
			}
		}

		private LinkRelationshipViewModel? _selectedLinkRelationship;
		public LinkRelationshipViewModel? SelectedLinkRelationship
		{
			get
			{
				return _selectedLinkRelationship;
			}
			set
			{
				SetProperty(ref _selectedLinkRelationship, value);

				DeleteLinkRelationshipCommand.NotifyCanExecuteChanged();
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

		private void OpenAddLinkRelationshipDialog(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			if (SelectedDataSource == null)
				throw new InvalidOperationException("SelectedDataSource was null!");

			bool dialogResult = _windowService.ShowDialog(windowType, out object dataContext);

			if (dialogResult)
			{
				Link link = ((AddLinkRelationshipViewModel)dataContext!).SelectedLink!;

				AddLinkRelationship(link, SelectedDataSource);
			}
		}

		internal static void AddLinkRelationship(Link link, DataSource dataSource) // TODO: Fix app crashing when no link is selected in AddLink-window. Same for Hubs.
		{
			WeakReferenceMessenger.Default.Send(new AddLinkRelationship(link, dataSource));
		}

		private bool CanDeleteLinkRelationship()
		{
			if (SelectedLinkRelationship == null)
				return false;
			else
				return true;
		}

		private void OpenDeleteLinkRelationshipDialog()
		{
			if (SelectedLinkRelationship == null || SelectedDataSource == null)
				throw new InvalidOperationException("SelectedLinkRelationship or SelectedDataSource was null!");

			DeleteLinkRelationship(SelectedLinkRelationship, SelectedDataSource);
		}

		private void DeleteLinkRelationship(LinkRelationshipViewModel linkRelationshipViewModel, DataSource dataSource)
		{
			if (LinkRelationships == null)
				throw new InvalidOperationException("LinkRelationships was null!");

			LinkRelationships.Remove(linkRelationshipViewModel);

			WeakReferenceMessenger.Default.Send(new RemoveLinkRelationship(linkRelationshipViewModel.LinkRelationship, dataSource));
		}
	}
}
