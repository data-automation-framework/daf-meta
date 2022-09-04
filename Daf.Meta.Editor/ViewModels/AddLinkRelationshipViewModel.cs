// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Daf.Meta.Layers;

namespace Daf.Meta.Editor.ViewModels
{
	public class AddLinkRelationshipViewModel : ObservableObject
	{
		private Link? _selectedLink;
		public Link? SelectedLink
		{
			get { return _selectedLink; }
			set
			{
				SetProperty(ref _selectedLink, value);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Static doesn't work right with WPF.")]
		public ObservableCollection<Link> Links
		{
			get { return Model.Instance.Links; }
		}
	}
}
