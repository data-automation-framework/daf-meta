// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Daf.Meta.Editor.ViewModels;

namespace Daf.Meta.Editor.Windows
{
	public partial class MainWindow : Window
	{
		private readonly Jot.Tracker tracker;

		public MainWindow()
		{
			// Restore settings before initializing component, we have view models that depend on it.
			tracker = Settings.Tracker;
			tracker.Track(this);

			InitializeComponent();

			if (MainViewModel.MetadataPath == null)
				Title = "Nucleus Data Vault 2.0 Metadata Editor - No save path selected";
			else
				Title = $"Nucleus Data Vault 2.0 Metadata Editor - {MainViewModel.MetadataPath}";
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void MainWindow_Closing(object sender, CancelEventArgs e)
		{
			MainViewModel viewModel = (MainViewModel)DataContext;

			// If data is dirty, notify user and ask for a response.
			if (viewModel.IsDirty)
			{
				string msg = "You have unsaved changes. Close without saving?";
				MessageBoxResult result =
				  MessageBox.Show(
					msg,
					"Warning - Unsaved changes!",
					MessageBoxButton.YesNo,
					MessageBoxImage.Warning);

				if (result == MessageBoxResult.No)
				{
					// If user doesn't want to close, cancel.
					e.Cancel = true;
				}
			}
		}

		private void DataSourceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			MainViewModel viewModel = (MainViewModel)DataContext;

			List<DataSourceViewModel>? dataSources = dataSourceList.SelectedItems.Cast<DataSourceViewModel>().ToList();

			viewModel.SelectedDataSources = dataSources;
			viewModel.SelectedDataSourceSingle = dataSources.FirstOrDefault();
		}

		//private void FilteredItems_OnFilter(object sender, FilterEventArgs e)
		//{
		//	StagingColumn column = sender as StagingColumn;

		//	e.Accepted = column.Nullable;
		//}
	}
}
