// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Daf.Meta.Layers
{
	public class StagingTable
	{
		// Deserializing doesn't actually set this, it only adds new columns.
		// Without a setter the application crashes when loading new data due to the way metadata is refreshed.
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
		public List<StagingColumn> Columns { get; set; } = new();

		[JsonIgnore]
		public List<string> ColumnNames
		{
			get
			{
				List<string> names = new();

				foreach (StagingColumn? column in Columns)
				{
					names.Add(column.Name);
				}

				return names;
			}
		}
	}
}
