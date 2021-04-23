// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Meta.Layers;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Daf.Meta.Editor.ViewModels
{
	public class SatelliteViewModel : ObservableValidator
	{
		public SatelliteViewModel(Satellite satellite)
		{
			Satellite = satellite;
		}

		internal Satellite Satellite { get; }

		public string Name
		{
			get => Satellite.Name;
			set
			{
				SetProperty(Satellite.Name, value, Satellite, (satellite, name) => satellite.Name = name);
			}
		}

		public SatelliteType Type
		{
			get => Satellite.Type;
			set
			{
				SetProperty(Satellite.Type, value, Satellite, (satellite, type) => satellite.Type = type);
			}
		}
	}
}
