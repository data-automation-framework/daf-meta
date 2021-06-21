// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Meta.Layers;
using Daf.Meta.Layers.DataSources;

namespace Daf.Meta.Editor.ViewModels
{
	public class ScriptDataSourceViewModel : DataSourceViewModel
	{
		private readonly ScriptDataSource _scriptDataSource;

		public ScriptDataSourceViewModel(DataSource dataSource) : base(dataSource)
		{
			_scriptDataSource = (ScriptDataSource)dataSource;
		}

		public override DataSource DataSource => _scriptDataSource;
	}
}
