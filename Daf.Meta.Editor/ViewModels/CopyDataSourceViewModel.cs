// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Daf.Meta.Layers;

namespace Daf.Meta.Editor.ViewModels
{
	public class CopyDataSourceViewModel : ObservableObject
	{
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

		private SourceSystem? _sourceSystem;
		public SourceSystem? SourceSystem
		{
			get { return _sourceSystem; }
			set
			{
				SetProperty(ref _sourceSystem, value);
			}
		}
		private Tenant? _tenant;
		public Tenant? Tenant
		{
			get { return _tenant; }
			set
			{
				SetProperty(ref _tenant, value);
			}
		}

		private Connection? _connection;
		public Connection? Connection
		{
			get { return _connection; }
			set
			{
				SetProperty(ref _connection, value);
			}
		}
	}
}
