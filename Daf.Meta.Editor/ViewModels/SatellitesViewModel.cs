// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.ObjectModel;
using Daf.Meta.Layers;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;

namespace Daf.Meta.Editor.ViewModels
{
	public class SatellitesViewModel : ObservableObject
	{
		private readonly IWindowService _windowService;

		public RelayCommand<SatelliteViewModel?> AssignAllCommand { get; }
		public RelayCommand<Type?> AddSatelliteCommand { get; }
		public RelayCommand DeleteSatelliteCommand { get; }

		public SatellitesViewModel()
		{
			_windowService = Ioc.Default.GetService<IWindowService>()!;

			AssignAllCommand = new RelayCommand<SatelliteViewModel?>(AssignAll);
			AddSatelliteCommand = new RelayCommand<Type?>(OpenAddSatelliteDialog);
			DeleteSatelliteCommand = new RelayCommand(DeleteSatellite, CanDeleteSatellite);

			//WeakReferenceMessenger.Default.Register<SatelliteViewModel, StagingColumnAddedRemoved>(this, (r, m) => { ModifiedRelationships(); });
		}

		private ObservableCollection<SatelliteViewModel> _satellites = new();

		public ObservableCollection<SatelliteViewModel> Satellites
		{
			get => _satellites;
			set
			{
				SetProperty(ref _satellites, value);
			}
		}

		private SatelliteViewModel? _selectedSatellite;
		public SatelliteViewModel? SelectedSatellite
		{
			get => _selectedSatellite;
			set
			{
				SetProperty(ref _selectedSatellite, value);

				DeleteSatelliteCommand.NotifyCanExecuteChanged();
			}
		}

		private DataSourceViewModel? _selectedDataSource;

		public DataSourceViewModel? SelectedDataSource
		{
			get => _selectedDataSource;
			set
			{
				SetProperty(ref _selectedDataSource, value);
			}
		}

		private void AssignAll(SatelliteViewModel? satelliteViewModel)
		{
			if (satelliteViewModel == null)
				throw new ArgumentNullException(nameof(satelliteViewModel));

			if (SelectedDataSource != null)
			{
				foreach (StagingColumn stagingColumn in SelectedDataSource.ColumnsNotInHubsOrLinks)
				{
					stagingColumn.Satellite = satelliteViewModel.Satellite;
				}
			}
		}

		private void OpenAddSatelliteDialog(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			if (SelectedDataSource == null)
				throw new InvalidOperationException();

			if (_windowService.ShowDialog(windowType, out object dataContext))
			{
				AddSatelliteViewModel vm = (AddSatelliteViewModel)dataContext;

				AddSatellite(vm.Name, SelectedDataSource.DataSource);
			}
		}

		private void AddSatellite(string name, DataSource dataSource)
		{
			Satellite satellite;
			// Request that the SelectedDataSource creates a new Satellite.
			if (string.IsNullOrEmpty(name))
				satellite = dataSource.AddSatellite();
			else
				satellite = dataSource.AddSatellite(name);

			// Use the new Satellite to create a new SatelliteViewModel.
			Satellites.Add(new SatelliteViewModel(satellite));
		}

		private bool CanDeleteSatellite()
		{
			if (SelectedSatellite == null)
				return false;
			else
				return true;
		}

		private void DeleteSatellite()
		{
			if (SelectedDataSource == null || SelectedSatellite == null)
				throw new InvalidOperationException();

			// Call on SelectedDataSource to remove the Satellite.
			SelectedDataSource.DataSource.RemoveSatellite(SelectedSatellite.Satellite);

			// Remove the SatelliteViewModel in Satellites.
			Satellites.Remove(SelectedSatellite);
		}
	}
}
