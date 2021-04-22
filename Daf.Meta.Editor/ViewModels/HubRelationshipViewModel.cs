// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Meta.Layers;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Daf.Meta.Editor.ViewModels
{
	public class HubRelationshipViewModel : ObservableValidator
	{
		public HubRelationshipViewModel(HubRelationship hubRelationship)
		{
			HubRelationship = hubRelationship;
		}

		public HubRelationship HubRelationship { get; }
	}
}
