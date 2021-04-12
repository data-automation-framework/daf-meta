// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.ComponentModel;
using Daf.Meta.Layers;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Daf.Meta.Editor.ViewModels
{
	public class TenantViewModel : ObservableValidator
	{
		public TenantViewModel(Tenant tenant)
		{
			Tenant = tenant;
		}

		[Browsable(false)]
		public Tenant Tenant { get; }

		[Category("General")]
		public string Name
		{
			get => Tenant.Name;
			set
			{
				SetProperty(Tenant.Name, value, Tenant, (tenant, name) => tenant.Name = name, true);

			}
		}

		[Category("General")]
		public string ShortName
		{
			get => Tenant.ShortName;
			set
			{
				SetProperty(Tenant.ShortName, value, Tenant, (tenant, shortName) => tenant.ShortName = shortName, true);
			}
		}

		// Preventing the inherited HasErrors property from showing up in the PropertyGrid.
		[Browsable(false)]
		public new bool HasErrors => base.HasErrors;
	}
}
