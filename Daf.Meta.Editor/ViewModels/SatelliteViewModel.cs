// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.ObjectModel;
using Daf.Meta.Layers;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace Daf.Meta.Editor.ViewModels
{
	public class SatelliteViewModel : ObservableObject
	{
		private readonly IWindowService _windowService;

		public RelayCommand<Satellite?> AssignAllCommand { get; }
		public RelayCommand<Type?> AddSatelliteCommand { get; }
		public RelayCommand DeleteSatelliteCommand { get; }

		public SatelliteViewModel(IWindowService windowService)
		{
			_windowService = windowService;

			AssignAllCommand = new RelayCommand<Satellite?>(AssignAll);
			AddSatelliteCommand = new RelayCommand<Type?>(AddSatellite);
			DeleteSatelliteCommand = new RelayCommand(DeleteSatellite, CanDeleteSatellite);
			_windowService = windowService;

			//WeakReferenceMessenger.Default.Register<SatelliteViewModel, ModifiedRelationships>(this, (r, m) => { ModifiedRelationships(); });
		}

		private ObservableCollection<Satellite>? _satellites;

		public ObservableCollection<Satellite>? Satellites
		{
			get
			{
				return _satellites;
			}
			set
			{
				SetProperty(ref _satellites, value);
			}
		}

		private Satellite? _selectedSatellite;
		public Satellite? SelectedSatellite
		{
			get { return _selectedSatellite; }
			set
			{
				SetProperty(ref _selectedSatellite, value);

				DeleteSatelliteCommand.NotifyCanExecuteChanged();
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

		private void AssignAll(Satellite? satellite)
		{
			if (satellite == null)
				throw new ArgumentNullException(nameof(satellite));

			if (SelectedDataSource != null)
			{
				foreach (StagingColumn stagingColumn in SelectedDataSource.ColumnsNotInHubsOrLinks)
				{
					stagingColumn.Satellite = satellite;
				}
			}
		}

		private void AddSatellite(Type? windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException(nameof(windowType));

			if (SelectedDataSource == null)
				throw new InvalidOperationException();

			if (_windowService.ShowDialog(windowType, out object dataContext))
			{
				AddSatelliteViewModel vm = (AddSatelliteViewModel)dataContext;

				Satellite satellite = new(vm.Name) { Type = SatelliteType.HashDiff };

				satellite.PropertyChanged += (s, e) =>
				{
					SelectedDataSource.NotifyPropertyChanged("Satellite");
				};

				SelectedDataSource.Satellites.Add(satellite);
			}
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

			SelectedSatellite.ClearSubscribers();

			SelectedDataSource.Satellites.Remove(SelectedSatellite);
		}
	}
}
