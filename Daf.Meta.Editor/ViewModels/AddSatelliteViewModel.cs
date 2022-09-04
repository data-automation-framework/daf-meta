// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using CommunityToolkit.Mvvm.ComponentModel;

namespace Daf.Meta.Editor.ViewModels
{
	public class AddSatelliteViewModel : ObservableObject
	{
		private string _name = string.Empty;
		public string Name
		{
			get { return _name; }
			set
			{
				SetProperty(ref _name, value);
			}
		}
	}
}
