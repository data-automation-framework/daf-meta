// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Daf.Meta.Layers;

namespace Daf.Meta.Editor.ViewModels
{
	public class TenantsViewModel : ObservableObject
	{
		private readonly IMessageBoxService _mbService;

		public RelayCommand AddTenantCommand { get; }
		public RelayCommand DeleteTenantCommand { get; }

		public TenantsViewModel()
		{
			Tenants = null!;

			_mbService = Ioc.Default.GetService<IMessageBoxService>()!;

			AddTenantCommand = new RelayCommand(AddTenant);
			DeleteTenantCommand = new RelayCommand(DeleteTenant);
		}

		public TenantsViewModel(ObservableCollection<Tenant> tenants)
		{
			Tenants = tenants;

			_mbService = Ioc.Default.GetService<IMessageBoxService>()!;

			AddTenantCommand = new RelayCommand(AddTenant);
			DeleteTenantCommand = new RelayCommand(DeleteTenant);

			SelectedTenant = Tenants.FirstOrDefault();
		}

		public ObservableCollection<Tenant> Tenants { get; }

		private Tenant? _selectedTenant;
		public Tenant? SelectedTenant
		{
			get { return _selectedTenant; }
			set
			{
				SetProperty(ref _selectedTenant, value);
			}
		}

		private void AddTenant()
		{
			Tenant tenant = new(name: "NewTenant", shortName: "XYZ");

			//Model.Instance.AddTenant(source);
			Tenants.AddSorted(tenant);
		}

		private void DeleteTenant()
		{
			if (SelectedTenant != null)
			{
				foreach (DataSource dataSource in Model.Instance.DataSources)
				{
					if (dataSource.Tenant == SelectedTenant)
					{
						string msg = "At least one data source is using the selected tenant. Remove all data sources using the tenant and try again.";
						_mbService.Show(msg, "Tenant in use", MessageBoxButton.OK, MessageBoxImage.Information);

						return;
					}
				}

				//MainWindow.Model.RemoveTenant(selectedTenant);
				Tenants.Remove(SelectedTenant);
			}
		}
	}
}
