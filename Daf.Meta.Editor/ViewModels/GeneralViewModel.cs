// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Daf.Meta.Editor.ViewModels
{
	public class GeneralViewModel : ObservableObject
	{
		private List<DataSourceViewModel>? _selectedDataSources;

		public List<DataSourceViewModel>? SelectedDataSources
		{
			get => _selectedDataSources;
			set
			{
				SetProperty(ref _selectedDataSources, value);
			}
		}
	}
}
