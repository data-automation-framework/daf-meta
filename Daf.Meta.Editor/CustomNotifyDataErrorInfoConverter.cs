// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Daf.Meta.Editor
{
	public sealed class CustomNotifyDataErrorInfoConverter : IValueConverter
	{
		/// <summary>
		/// The instance
		/// </summary>
		private readonly INotifyDataErrorInfo instance;

		/// <summary>
		/// The column name
		/// </summary>
		private readonly string columnName;

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomNotifyDataErrorInfoConverter" /> class.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="columnName">Name of the column.</param>
		public CustomNotifyDataErrorInfoConverter(INotifyDataErrorInfo instance, string columnName)
		{
			this.instance = instance;
			// columnName is confusing, this is the PROPERTY. Kept as colunmnName to be consistent with converter that's in PropertyTools source.
			this.columnName = columnName;
		}

		/// <summary>
		/// Converts a value.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns <c>null</c>, the valid <c>null</c> value is used.
		/// </returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch (targetType)
			{
				case object when targetType == typeof(bool):
					return instance.HasErrors;
				case object when targetType == typeof(Visibility):
					IEnumerable<ValidationResult> errors = (IEnumerable<ValidationResult>)instance.GetErrors(columnName);

					if (errors.Any())
						return Visibility.Visible;
					else
						return Visibility.Collapsed;
				default:
					// Null-forgiving because null is a valid return value.
					return instance.GetErrors(columnName).Cast<object>().FirstOrDefault()!;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
