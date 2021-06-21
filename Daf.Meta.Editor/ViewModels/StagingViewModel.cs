// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace Daf.Meta.Editor.ViewModels
{
	public class StagingViewModel : ObservableObject
	{
		public RelayCommand AddStagingColumnCommand { get; }
		public RelayCommand DeleteStagingColumnCommand { get; }

		public StagingViewModel()
		{
			AddStagingColumnCommand = new RelayCommand(AddStagingColumn);
			DeleteStagingColumnCommand = new RelayCommand(DeleteStagingColumn, CanDeleteStagingColumn);
			WeakReferenceMessenger.Default.Register<StagingViewModel, RefreshedMetadata>(this, (r, m) => { RefreshedMetadata(); });
		}

		private void RefreshedMetadata()
		{
			StagingColumns.Clear();

			if (_selectedDataSource != null)
			{
				foreach (StagingColumn column in SelectedDataSource!.DataSource.StagingTable!.Columns)
				{
					StagingColumnViewModel columnViewModel = new(column);
					StagingColumns.Add(columnViewModel);
				}
			}
		}

		public ObservableCollection<StagingColumnViewModel> StagingColumns { get; } = new();

		private List<DataSourceViewModel>? _selectedDataSources;

		public List<DataSourceViewModel>? SelectedDataSources
		{
			get { return _selectedDataSources; }
			set
			{
				SetProperty(ref _selectedDataSources, value);
			}
		}

		private DataSourceViewModel? _selectedDataSource;

		public DataSourceViewModel? SelectedDataSource
		{
			get
			{
				return _selectedDataSource;
			}
			set
			{
				StagingColumns.Clear();

				SetProperty(ref _selectedDataSource, value);

				if (_selectedDataSource != null)
				{
					foreach (StagingColumn column in SelectedDataSource!.DataSource.StagingTable!.Columns)
					{
						StagingColumnViewModel columnViewModel = new(column);
						StagingColumns.Add(columnViewModel);
					}
				}
			}
		}

		private List<StagingColumnViewModel>? _selectedColumns;
		public List<StagingColumnViewModel>? SelectedColumns
		{
			get { return _selectedColumns; }
			set
			{
				SetProperty(ref _selectedColumns, value);

				DeleteStagingColumnCommand.NotifyCanExecuteChanged();
			}
		}

		private StagingColumnViewModel? _selectedColumn;
		public StagingColumnViewModel? SelectedColumn
		{
			get { return _selectedColumn; }
			set
			{
				SetProperty(ref _selectedColumn, value);

				DeleteStagingColumnCommand.NotifyCanExecuteChanged();
			}
		}

		private void AddStagingColumn()
		{
			if (SelectedDataSource == null)
				throw new InvalidOperationException();

			StagingColumn stagingColumn = SelectedDataSource.DataSource.AddStagingColumn();

			// Add new StagingColumnViewModel passing the new StagingColumn as an argument.
			StagingColumns.Add(new StagingColumnViewModel(stagingColumn));

			WeakReferenceMessenger.Default.Send(new StagingColumnAddedRemoved());
		}

		private bool CanDeleteStagingColumn()
		{
			if (SelectedColumn == null)
				return false;
			else
				return true;
		}

		private void DeleteStagingColumn()
		{
			if (SelectedDataSource == null || SelectedColumn == null)
				throw new InvalidOperationException();

			SelectedDataSource.DataSource.RemoveStagingColumn(SelectedColumn.StagingColumn);

			// Remove the view model column from the list.
			StagingColumns.Remove(SelectedColumn);

			// Inform subscribers that the StagingColumn is being removed.
			WeakReferenceMessenger.Default.Send(new StagingColumnAddedRemoved());
		}
	}
}
