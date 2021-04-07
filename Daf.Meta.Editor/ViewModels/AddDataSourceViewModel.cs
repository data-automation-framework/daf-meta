// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Daf.Meta.Layers;

namespace Daf.Meta.Editor.ViewModels
{
	public class AddDataSourceViewModel : ObservableObject
	{
		public AddDataSourceViewModel()
		{
			SelectedSourceSystem = SourceSystems.FirstOrDefault();
			SelectedTenant = Tenants.FirstOrDefault();
			SelectedDataSourceType = DataSourceType.FlatFile;
			SelectedConnection = Connections.FirstOrDefault();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Static doesn't work right with WPF.")]
		public ObservableCollection<SourceSystem> SourceSystems
		{
			get { return Model.Instance.SourceSystems; }
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Static doesn't work right with WPF.")]
		public ObservableCollection<Tenant> Tenants
		{
			get { return Model.Instance.Tenants; }
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Static doesn't work right with WPF.")]
		public ObservableCollection<Connection> Connections
		{
			get { return Model.Instance.Connections; }
		}

		private string _name = string.Empty;
		public string Name
		{
			get { return _name; }
			set
			{
				SetProperty(ref _name, value);
			}
		}

		private SourceSystem? _selectedSourceSystem;
		public SourceSystem? SelectedSourceSystem
		{
			get { return _selectedSourceSystem; }
			set
			{
				SetProperty(ref _selectedSourceSystem, value);
			}
		}

		private Tenant? _selectedTenant;
		public Tenant? SelectedTenant
		{
			get { return _selectedTenant; }
			set
			{
				SetProperty(ref _selectedTenant, value);
			}
		}

		private DataSourceType _selectedDataSourceType;
		public DataSourceType SelectedDataSourceType
		{
			get { return _selectedDataSourceType; }
			set
			{
				SetProperty(ref _selectedDataSourceType, value);
			}
		}

		private Connection? _selectedConnection;
		public Connection? SelectedConnection
		{
			get { return _selectedConnection; }
			set
			{
				SetProperty(ref _selectedConnection, value);
			}
		}
	}
}
