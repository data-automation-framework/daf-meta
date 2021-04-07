// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;

namespace Daf.Meta.Layers
{
	public class LoadTable
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "We need at least an init setter in order to support deserialization.")]
		public List<Column> Columns { get; init; } = new List<Column>();
	}
}
