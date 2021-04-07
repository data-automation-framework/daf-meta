// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Daf.Meta.Editor.ViewModels
{
	public class AddOrEditViewModel : ObservableObject
	{
		public string Title { get; set; } = "No title";
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
