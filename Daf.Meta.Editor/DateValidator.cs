// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Daf.Meta.Editor
{
	public sealed class DateValidator : ValidationAttribute
	{
		public override bool IsValid(object? value)
		{
			string? input = (string?)value;

			if (string.IsNullOrWhiteSpace(input))
				return true;
			else if (DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.None, out _))
				return true;
			else
				return false;
		}
	}
}
