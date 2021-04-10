// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using PropertyTools.DataAnnotations;
using Daf.Meta.Layers.Connections;
using Daf.Meta.Layers;

namespace Daf.Meta.Editor.ViewModels
{
	public class OdbcConnectionViewModel : ConnectionViewModel
	{
		private readonly OdbcConnection _odbcConnectionViewModel;

		public OdbcConnectionViewModel(Connection connection) : base(connection)
		{
			_odbcConnectionViewModel = (OdbcConnection)connection;
		}

		public override Connection Connection
		{
			get { return _odbcConnectionViewModel; }
		}

		[Category("Database")]
		[SortIndex(100)]
		public string ConnectionString
		{
			get { return _odbcConnectionViewModel.ConnectionString; }
			set
			{
				SetProperty(_odbcConnectionViewModel.ConnectionString, value, _odbcConnectionViewModel, (connection, connectionString) => _odbcConnectionViewModel.ConnectionString = connectionString, true);
			}
		}
	}
}
