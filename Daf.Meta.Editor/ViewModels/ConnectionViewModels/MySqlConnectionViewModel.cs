// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using PropertyTools.DataAnnotations;
using Daf.Meta.Layers.Connections;
using Daf.Meta.Layers;

namespace Daf.Meta.Editor.ViewModels
{
	public class MySqlConnectionViewModel : ConnectionViewModel
	{
		private readonly MySqlConnection _mySqlConnection;

		public MySqlConnectionViewModel(Connection connection) : base(connection)
		{
			_mySqlConnection = (MySqlConnection)connection;
		}

		public override Connection Connection
		{
			get { return _mySqlConnection; }
		}

		[Category("Database")]
		[SortIndex(100)]
		public string ConnectionString
		{
			get { return _mySqlConnection.ConnectionString; }
			set
			{
				SetProperty(_mySqlConnection.ConnectionString, value, _mySqlConnection, (connection, connectionString) => _mySqlConnection.ConnectionString = connectionString, true);
			}
		}
	}
}
