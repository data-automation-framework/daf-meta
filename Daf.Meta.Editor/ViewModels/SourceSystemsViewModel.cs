// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Daf.Meta.Layers;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace Daf.Meta.Editor.ViewModels
{
	public class SourceSystemsViewModel : ObservableObject
	{
		private readonly IMessageBoxService _mbService;

		public RelayCommand AddSourceSystemCommand { get; }
		public RelayCommand DeleteSourceSystemCommand { get; }

		public SourceSystemsViewModel(IMessageBoxService mbService)
		{
			SourceSystems = null!;

			_mbService = mbService;

			AddSourceSystemCommand = new RelayCommand(AddSourceSystem);
			DeleteSourceSystemCommand = new RelayCommand(DeleteSourceSystem);
			_mbService = mbService;
		}

		public SourceSystemsViewModel(ObservableCollection<SourceSystem> sourceSystems, IMessageBoxService mbService) // TODO: Improve constructors.
		{
			SourceSystems = sourceSystems;

			_mbService = mbService;

			AddSourceSystemCommand = new RelayCommand(AddSourceSystem);
			DeleteSourceSystemCommand = new RelayCommand(DeleteSourceSystem);

			SelectedSourceSystem = SourceSystems.FirstOrDefault();
		}

		public ObservableCollection<SourceSystem> SourceSystems { get; }

		private SourceSystem? _selectedSourceSystem;
		public SourceSystem? SelectedSourceSystem
		{
			get { return _selectedSourceSystem; }
			set
			{
				SetProperty(ref _selectedSourceSystem, value);
			}
		}

		private void AddSourceSystem()
		{
			SourceSystem sourceSystem = new(name: "NewSourceSystem", shortName: "XYZ");

			//Model.Instance.AddSourceSystems(source);
			SourceSystems.AddSorted(sourceSystem);
		}

		private void DeleteSourceSystem()
		{
			if (SelectedSourceSystem != null)
			{
				foreach (DataSource dataSource in Model.Instance.DataSources)
				{
					if (dataSource.SourceSystem == SelectedSourceSystem)
					{
						string msg = "At least one data source is using the selected source system. Remove all data sources using the source system and try again.";
						_mbService.Show(msg, "Source system in use", MessageBoxButton.OK, MessageBoxImage.Information);

						return;
					}
				}

				//MainWindow.Model.RemoveSourceSystem(selectedSourceSystem);
				SourceSystems.Remove(SelectedSourceSystem);
			}
		}
	}
}
