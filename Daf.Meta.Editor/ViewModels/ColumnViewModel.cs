// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using PropertyTools.DataAnnotations;

namespace Daf.Meta.Editor.ViewModels
{
	public class ColumnViewModel : ObservableValidator
	{
		public ColumnViewModel(Column column)
		{
			Column = column;
		}

		internal Column Column { get; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "A value is required.")]
		[System.ComponentModel.Browsable(false)]
		public string Name
		{
			get { return Column.Name; }
			set
			{
				SetProperty(Column.Name, value, Column, (column, name) => column.Name = name, true);
			}
		}

		[SortIndex(1)]
		[System.ComponentModel.Category("Data type")]
		public SqlServerDataType DataType
		{
			get { return Column.DataType; }
			set
			{
				SetProperty(Column.DataType, value, Column, (column, dataType) => column.DataType = dataType);

				OnPropertyChanged(nameof(GuiLengthVisible));
				OnPropertyChanged(nameof(GuiPrecisionVisible));
				OnPropertyChanged(nameof(GuiScaleVisible));
				OnPropertyChanged(nameof(DataTypeString));
			}
		}

		[SortIndex(2)]
		[System.ComponentModel.Category("Data type")]
		public bool Nullable
		{
			get { return Column.Nullable; }
			set
			{
				SetProperty(Column.Nullable, value, Column, (column, nullable) => column.Nullable = nullable);

				OnPropertyChanged(nameof(DataTypeString));
			}
		}

		private string? _length;

		[SortIndex(3)]
		[VisibleBy(nameof(GuiLengthVisible), true)]
		[AutoUpdateText]
		[System.ComponentModel.Category("Data type")]
		[CustomRangeValidator(1, 8000, -1, ErrorMessage = "Range invalid. Must be 1-8000 or -1.")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Cannot be empty.")]
		public string? Length
		{
			get { return _length ?? Column.Length.ToString(); }
			set
			{
				if (SetProperty(ref _length, value, true) && !HasErrors)
				{
					if (int.TryParse(value, out int result))
						Column.Length = result;
				}
				OnPropertyChanged(nameof(GuiLengthVisible));
				OnPropertyChanged(nameof(DataTypeString));
			}
		}

		[SortIndex(4)]
		[VisibleBy(nameof(GuiPrecisionVisible), true)]
		[AutoUpdateText]
		[System.ComponentModel.Category("Data type")]
		public int? Precision
		{
			get { return Column.Precision; }
			set
			{
				SetProperty(Column.Precision, value, Column, (column, precision) => column.Precision = precision);

				OnPropertyChanged(nameof(GuiPrecisionVisible));
				OnPropertyChanged(nameof(DataTypeString));
			}
		}

		[SortIndex(5)]
		[VisibleBy(nameof(GuiScaleVisible), true)]
		[AutoUpdateText]
		[System.ComponentModel.Category("Data type")]
		public int? Scale
		{
			get { return Column.Scale; }
			set
			{
				SetProperty(Column.Scale, value, Column, (column, scale) => column.Scale = scale);

				OnPropertyChanged(nameof(GuiScaleVisible));
				OnPropertyChanged(nameof(DataTypeString));
			}
		}

		[SortIndex(8)]
		[System.ComponentModel.Category("General")]
		[AutoUpdateText]
		[DateValidator(ErrorMessage = "Must follow format yyyy-mm-dd.")]
		public string? AddedOnBusinessDate
		{
			get { return Column.AddedOnBusinessDate; }
			set
			{
				SetProperty(Column.AddedOnBusinessDate, value, Column, (column, addedOnBusinessDate) => column.AddedOnBusinessDate = addedOnBusinessDate, true);
			}
		}

		[System.ComponentModel.Browsable(false)]
		public bool GuiLengthVisible
		{
			get
			{
				switch (DataType)
				{
					case SqlServerDataType.Char:
					case SqlServerDataType.NChar:
					case SqlServerDataType.VarChar:
					case SqlServerDataType.NVarChar:
					case SqlServerDataType.Binary:
					case SqlServerDataType.VarBinary:
						return true;
					default:
						return false;
				}
			}
		}

		[System.ComponentModel.Browsable(false)]
		public bool GuiPrecisionVisible
		{
			get
			{
				switch (DataType)
				{
					case SqlServerDataType.Decimal:
						return true;
					default:
						return false;
				}
			}
		}

		[System.ComponentModel.Browsable(false)]
		public bool GuiScaleVisible
		{
			get
			{
				switch (DataType)
				{
					case SqlServerDataType.DateTime2:
					case SqlServerDataType.DateTimeOffset:
					case SqlServerDataType.Decimal:
					case SqlServerDataType.Time:
						return true;
					default:
						return false;
				}
			}
		}

		[System.ComponentModel.Browsable(false)]
		public string DataTypeString
		{
			get
			{
				string dataType = $"{DataType}";

				switch (DataType)
				{
					case SqlServerDataType.VarChar:
					case SqlServerDataType.NVarChar:
					case SqlServerDataType.Char:
					case SqlServerDataType.NChar:
					case SqlServerDataType.Binary:
					case SqlServerDataType.VarBinary:
						dataType += $" ({Column.Length + ")"}";

						break;
					case SqlServerDataType.Decimal:
						dataType += $" ({Precision + ", " + Scale + ")"}";

						break;
					case SqlServerDataType.Time:
					case SqlServerDataType.DateTime2:
					case SqlServerDataType.DateTimeOffset:
						dataType += $" ({Scale + ")"}";

						break;
					case SqlServerDataType.Bit:
					case SqlServerDataType.Money:
					case SqlServerDataType.Date:
					case SqlServerDataType.DateTime:
					case SqlServerDataType.Float:
					case SqlServerDataType.TinyInt:
					case SqlServerDataType.SmallDateTime:
					case SqlServerDataType.SmallInt:
					case SqlServerDataType.Int:
					case SqlServerDataType.BigInt:
					case SqlServerDataType.UniqueIdentifier:
						break;
					default:
						throw new InvalidOperationException($"Data type {DataType} is not supported by Daf.Meta.Editor");
				}

				return dataType.PadRight(18) + $"{(Nullable ? " NULL" : " NOT NULL"),9}";
			}
		}

		// Preventing the inherited HasErrors property from showing up in the PropertyGrid.
		[Browsable(false)]
		public new bool HasErrors => base.HasErrors;
	}
}
