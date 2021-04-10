// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using PropertyTools.DataAnnotations;
using Daf.Meta.Layers.Connections;
using Daf.Meta.Layers;

namespace Daf.Meta.Editor.ViewModels
{
	public class OleDBConnectionViewModel : ConnectionViewModel
	{
		private readonly OleDBConnection _oleDBConnection;

		public OleDBConnectionViewModel(Connection connection) : base(connection)
		{
			_oleDBConnection = (OleDBConnection)connection;
		}

		public override Connection Connection
		{
			get { return _oleDBConnection; }
		}

		[Category("Database")]
		[SortIndex(100)]
		public string ConnectionString
		{
			get { return _oleDBConnection.ConnectionString; }
			set
			{
				SetProperty(_oleDBConnection.ConnectionString, value, _oleDBConnection, (connection, connectionString) => _oleDBConnection.ConnectionString = connectionString, true);
			}
		}
	}
}
