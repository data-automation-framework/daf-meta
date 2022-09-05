// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Daf.Meta.Layers;

namespace Daf.Meta.Editor.ViewModels
{
	public class LinkViewModel : ObservableValidator
	{
		public LinkViewModel(Link link)
		{
			Link = link;

			if (link != null)
			{
				foreach (StagingColumn stagingColumn in link.BusinessKeys)
				{
					BusinessKeys.Add(new BusinessKeyViewModel(stagingColumn));
				}
			}
		}

		internal Link Link { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "A value is required.")]
		public string Name
		{
			get { return Link.Name; }
			set
			{
				SetProperty(Link.Name, value, Link, (link, name) => link.Name = name, true);
			}
		}

		private ObservableCollection<BusinessKeyViewModel> _businessKeys = new();
		public ObservableCollection<BusinessKeyViewModel> BusinessKeys
		{
			get
			{
				return _businessKeys;
			}
			set
			{
				SetProperty(ref _businessKeys, value);

			}
		}
	}
}
