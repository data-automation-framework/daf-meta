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
	public class LoadViewModel : ObservableObject
	{
		public RelayCommand AddLoadColumnCommand { get; }
		public RelayCommand DeleteLoadColumnCommand { get; }

		public LoadViewModel()
		{
			AddLoadColumnCommand = new RelayCommand(AddLoadColumn);
			DeleteLoadColumnCommand = new RelayCommand(DeleteLoadColumn, CanDeleteLoadColumn);
			WeakReferenceMessenger.Default.Register<LoadViewModel, RefreshedMetadata>(this, (r, m) => { RefreshedMetadata(); });
		}

		private void RefreshedMetadata()
		{
			Columns.Clear();

			if (_selectedDataSource != null)
			{
				foreach (Column column in SelectedDataSource!.LoadTable!.Columns)
				{
					ColumnViewModel columnViewModel = new(column);
					Columns.Add(columnViewModel);
				}
			}
		}

		public ObservableCollection<ColumnViewModel> Columns { get; } = new();

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
				Columns.Clear();

				SetProperty(ref _selectedDataSource, value);

				if (_selectedDataSource != null)
				{
					foreach (Column column in SelectedDataSource!.DataSource.LoadTable!.Columns)
					{
						ColumnViewModel columnViewModel = new(column);
						Columns.Add(columnViewModel);
					}
				}
			}
		}

		private List<ColumnViewModel>? _selectedColumns;
		public List<ColumnViewModel>? SelectedColumns
		{
			get { return _selectedColumns; }
			set
			{
				SetProperty(ref _selectedColumns, value);

				DeleteLoadColumnCommand.NotifyCanExecuteChanged();
			}
		}

		private ColumnViewModel? _selectedColumn;
		public ColumnViewModel? SelectedColumn
		{
			get { return _selectedColumn; }
			set
			{
				SetProperty(ref _selectedColumn, value);

				DeleteLoadColumnCommand.NotifyCanExecuteChanged();
			}
		}

		private void AddLoadColumn()
		{
			if (SelectedDataSource == null)
				throw new InvalidOperationException();

			Column column = SelectedDataSource.DataSource.AddLoadColumn();

			// Create a new view model column and add it to the list.
			ColumnViewModel columnViewModel = new(column);
			Columns.Add(columnViewModel);
		}

		private bool CanDeleteLoadColumn()
		{
			if (SelectedColumn == null)
				return false;
			else
				return true;
		}

		private void DeleteLoadColumn()
		{
			if (SelectedDataSource == null || SelectedColumn == null)
				throw new InvalidOperationException();

			SelectedDataSource.DataSource.RemoveLoadColumn(SelectedColumn.Column);

			// Remove the view model column from the list.
			Columns.Remove(SelectedColumn);
		}
	}
}
