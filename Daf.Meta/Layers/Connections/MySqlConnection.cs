// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Dahomey.Json.Attributes;
using PropertyTools.DataAnnotations;

namespace Daf.Meta.Layers.Connections
{
	[JsonDiscriminator("MySql")]
	public class MySqlConnection : Connection
	{
		public MySqlConnection(string name, string connectionString)
		{
			Name = name;
			_connectionString = connectionString;
		}

		private string _connectionString;

		[Category("Database")]
		[SortIndex(100)]
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
