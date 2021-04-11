// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Daf.Meta.Layers;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;

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

		public TenantsViewModel(ObservableCollection<TenantViewModel> tenants)
		{
			Tenants = tenants;

			_mbService = Ioc.Default.GetService<IMessageBoxService>()!;

			AddTenantCommand = new RelayCommand(AddTenant);
			DeleteTenantCommand = new RelayCommand(DeleteTenant);

			SelectedTenant = Tenants.FirstOrDefault();
		}

		public ObservableCollection<TenantViewModel> Tenants { get; }

		private TenantViewModel? _selectedTenant;

		public TenantViewModel? SelectedTenant
		{
			get => _selectedTenant;
			set
			{
				SetProperty(ref _selectedTenant, value);
			}
		}

		private void AddTenant()
		{
			Tenant tenant = new(name: "NewTenant", shortName: "XYZ");

			WeakReferenceMessenger.Default.Send(new AddTenant(tenant));

			// AddSorted demands IComparable be implemented.
			Tenants.Add(new TenantViewModel(tenant));
		}

		private void DeleteTenant()
		{
			if (SelectedTenant != null)
			{
				foreach (DataSource dataSource in Model.Instance.DataSources)
				{
					if (dataSource.Tenant == SelectedTenant.Tenant)
					{
						string msg = "At least one data source is using the selected tenant. Remove all data sources using the tenant and try again.";
						_mbService.Show(msg, "Tenant in use", MessageBoxButton.OK, MessageBoxImage.Information);

						return;
					}
				}

				WeakReferenceMessenger.Default.Send(new RemoveTenant(SelectedTenant.Tenant));

				Tenants.Remove(SelectedTenant);
			}
		}
	}
}
