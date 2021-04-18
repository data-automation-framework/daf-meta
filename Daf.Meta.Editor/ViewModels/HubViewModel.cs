// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Daf.Meta.Layers;
using System;

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

			Hub.ChangedBusinessKeyColumn += Hub_ChangedBusinessKeyColumn;
		}

		/// <summary>
		/// Subscribes to events raised when the wrapped hub adds or removes BusinessKeys from its collection
		/// and creates or deletes the corresponding BusinessKeyViewModels.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Hub_ChangedBusinessKeyColumn(object? sender, BusinessKeyEventArgs e)
		{
			if (e.BusinessKey == null)
				throw new InvalidOperationException("BusinessKey was null!");

			if (e.Action == BusinessKeyEventType.Add)
			{
				BusinessKeys.Add(new BusinessKeyViewModel(e.BusinessKey));
			}
			else if (e.Action == BusinessKeyEventType.Remove)
			{
				foreach (BusinessKeyViewModel businessKeyViewModel in BusinessKeys)
				{
					if (businessKeyViewModel.StagingColumn == e.BusinessKey)
					{
						BusinessKeys.Remove(businessKeyViewModel);
						break;
					}
				}
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
