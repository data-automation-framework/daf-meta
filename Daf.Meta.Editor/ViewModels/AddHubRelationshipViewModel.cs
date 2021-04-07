// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Daf.Meta.Layers;

namespace Daf.Meta.Editor.ViewModels
{
	public class AddHubRelationshipViewModel : ObservableObject
	{
		private Hub? _selectedHub;
		public Hub? SelectedHub
		{
			get { return _selectedHub; }
			set
			{
				SetProperty(ref _selectedHub, value);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Static doesn't work right with WPF.")]
		public ObservableCollection<Hub> Hubs
		{
			get { return Model.Instance.Hubs; }
		}
	}
}
