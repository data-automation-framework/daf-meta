// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Dahomey.Json.Attributes;

namespace Daf.Meta.Layers.Connections
{
	[JsonDiscriminator("OleDB")]
	public class OleDBConnection : Connection
	{
		public OleDBConnection(string name, string connectionString)
		{
			Name = name;
			_connectionString = connectionString;
		}

		private string _connectionString;

		public string ConnectionString
		{
			get { return _connectionString; }
			set
			{
				if (_connectionString != value)
				{
					_connectionString = value;

					NotifyPropertyChanged("ConnectionString");
				}
			}
		}
	}
}
