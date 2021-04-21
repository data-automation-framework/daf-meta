// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Daf.Meta.Layers;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Daf.Meta.Editor.ViewModels
{
	public class HubViewModel : ObservableValidator
	{
		public HubViewModel(Hub hub)
		{
			Hub = hub;

			foreach (StagingColumn stagingColumn in Hub.BusinessKeys)
			{
				BusinessKeys.Add(new BusinessKeyViewModel(stagingColumn));
			}
		}

		public ObservableCollection<BusinessKeyViewModel> BusinessKeys { get; } = new();

		internal Hub Hub { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "A value is required.")]
		public string Name
		{
			get { return Hub.Name; }
			set
			{
				SetProperty(Hub.Name, value, Hub, (hub, name) => hub.Name = name, true);
			}
		}
	}
}
