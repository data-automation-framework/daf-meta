// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Dahomey.Json.Attributes;

namespace Daf.Meta.Layers.DataSources
{
	[JsonDiscriminator("Script")]
	public class ScriptDataSource : DataSource
	{
		public ScriptDataSource(string name, SourceSystem sourceSystem, Tenant tenant) : base(name, sourceSystem, tenant)
		{
		}

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
