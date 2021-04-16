// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Daf.Meta.Layers;

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

			AddLinkRelationshipCommand = new RelayCommand<Type?>(AddLinkRelationship);
			DeleteLinkRelationshipCommand = new RelayCommand(DeleteLinkRelationship, CanDeleteLinkRelationship);
		}

		private ObservableCollection<LinkRelationship>? _linkRelationships;

		public ObservableCollection<LinkRelationship>? LinkRelationships
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

		private LinkRelationship? _selectedLinkRelationship;
		public LinkRelationship? SelectedLinkRelationship
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

		private void AddLinkRelationship(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			if (SelectedDataSource == null)
				throw new InvalidOperationException();

			bool dialogResult = _windowService.ShowDialog(windowType, out object dataContext);

			if (dialogResult)
			{
				//string stagingColumnName = inputDialog.StagingColumnName;
				//int order = inputDialog.Order;
				//bool isDriving = (bool)inputDialog.IsDriving;
				Link link = ((AddLinkRelationshipViewModel)dataContext!).SelectedLink!;

				//foreach (var column in selectedDataSource.StagingTable.Columns)
				//{
				//	if (column.Name == stagingColumnName)
				//	{
				//		if (selectedDataSource.LinkRelationships == null)
				//		{
				//			selectedDataSource.LinkRelationships = new ObservableCollection<LinkRelationship>();
				//			selectedDataSource.LinkRelationships.Add(new LinkRelationship { });
				//		}
				//		else
				//		{
				//			string msg = $"Staging column {column.Name} already has a link assigned! Multiple links aren't supported yet. No change was made.";
				//			MessageBoxResult result =
				//			  MessageBox.Show(
				//				msg,
				//				"Column already has a link assigned",
				//				MessageBoxButton.OK,
				//				MessageBoxImage.Exclamation);
				//		}

				//		break;
				//	}
				//}

				LinkRelationship linkRelationship = new(link);

				foreach (StagingColumn bk in link.BusinessKeys)
				{
					LinkMapping linkMapping = new(bk);

					linkMapping.PropertyChanged += (s, e) =>
					{
						linkRelationship.NotifyPropertyChanged("LinkMapping");
					};

					linkRelationship.Mappings.Add(linkMapping);
				}

				linkRelationship.PropertyChanged += (s, e) =>
				{
					SelectedDataSource.NotifyPropertyChanged("LinkRelationship");
				};

				SelectedDataSource.LinkRelationships.Add(linkRelationship);

				//WeakReferenceMessenger.Default.Send(new ModifiedRelationships());
			}
		}

		private bool CanDeleteLinkRelationship()
		{
			if (SelectedLinkRelationship == null)
				return false;
			else
				return true;
		}

		private void DeleteLinkRelationship()
		{
			if (SelectedDataSource != null && SelectedLinkRelationship != null)
			{
				foreach (LinkMapping linkMapping in SelectedLinkRelationship.Mappings)
				{
					linkMapping.ClearSubscribers();
				}

				SelectedLinkRelationship.ClearSubscribers();

				SelectedDataSource.LinkRelationships.Remove(SelectedLinkRelationship);

				// TODO: businessKeyComboBox is in Satellite, we need to send it a message to run the equivalent command.
				//businessKeyComboBox.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
			}
		}
	}
}
