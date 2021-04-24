// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Meta.Layers.Connections;

namespace Daf.Meta.Layers.DataSources
{
	//[JsonDiscriminator("JsonFile")]
	public class JsonFileDataSource : DataSource
	{
		public JsonFileDataSource(string name, RestConnection connection, SourceSystem sourceSystem, Tenant tenant) : base(name, sourceSystem, tenant)
		{
			_connection = connection;
		}

		private readonly RestConnection _connection;

		public override DataSource Clone()
		{
			throw new System.NotImplementedException();
		}

		public override void GetMetadata()
		{
			throw new System.NotImplementedException();
		}
	}
}
