// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors


namespace Daf.Meta.Editor.ViewModels
{
	public class ScriptDataSourceViewModel : DataSourceViewModel
	{
		public ScriptDataSourceViewModel(string name, SourceSystem sourceSystem, Tenant tenant) : base(name, sourceSystem, tenant)
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
