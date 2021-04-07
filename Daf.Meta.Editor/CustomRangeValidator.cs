// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.ComponentModel.DataAnnotations;

namespace Daf.Meta.Editor
{
	public sealed class CustomRangeValidator : ValidationAttribute
	{
		public int Min { get; }
		public int Max { get; }
		public int? Special { get; }

		public CustomRangeValidator(int min, int max)
		{
			Min = min;
			Max = max;
		}

		public CustomRangeValidator(int min, int max, int special) : this(min, max)
		{
			Special = special;
		}

		public override bool IsValid(object? value)
		{
			if (int.TryParse(value as string, out int result))
			{
				if (result >= Min && result <= Max)
					return true;
				else if (result == Special)
					return true;
				else
					return false;
			}
			else
				return false;
		}
	}
}
