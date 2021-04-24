// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Meta.Layers;

namespace Daf.Meta.Editor.ViewModels
{
	public class JsonFileDataSourceViewModel : DataSourceViewModel
	{
		public JsonFileDataSourceViewModel(DataSource dataSource)
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
