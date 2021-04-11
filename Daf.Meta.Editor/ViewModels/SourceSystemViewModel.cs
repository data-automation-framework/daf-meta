// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Microsoft.Toolkit.Mvvm.ComponentModel;
using PropertyTools.DataAnnotations;

namespace Daf.Meta.Layers
{
	public class SourceSystemViewModel : ObservableValidator
	{
		private readonly SourceSystem SourceSystem;

		public SourceSystemViewModel(SourceSystem sourceSystem)
		{
			SourceSystem = sourceSystem;
		}

		public string Name
		{
			get => SourceSystem.Name;
			set
			{
				SetProperty(SourceSystem.Name, value, SourceSystem, (sourceSystem, name) => sourceSystem.Name = name, true);
			}
		}

		public string ShortName
		{
			get => SourceSystem.ShortName;
			set
			{
				SetProperty(SourceSystem.Name, value, SourceSystem, (sourceSystem, shortName) => sourceSystem.ShortName = shortName, true);
			}
		}

		// Preventing the inherited HasErrors property from showing up in the PropertyGrid.
		[Browsable(false)]
		public new bool HasErrors => base.HasErrors;
	}
}
