// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Meta.Layers
{
	public class Hub : BusinessKey, IComparable<Hub>
	{
		public Hub(string name)
		{
			Name = name;
		}

		public int CompareTo(Hub? other)
		{
			if (other == null)
				return -1;

			if (Name == other.Name)
				return 0;

			if (string.Compare(Name, other.Name, StringComparison.InvariantCulture) < 0)
				return -1;

			return 1;
		}
	}
}
