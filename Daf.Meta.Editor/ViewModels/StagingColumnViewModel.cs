// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using PropertyTools.DataAnnotations;

namespace Daf.Meta.Editor.ViewModels
{
	public class StagingColumnViewModel : ObservableValidator
	{
		public StagingColumnViewModel(StagingColumn stagingColumn)
		{
			StagingColumn = stagingColumn;
		}

		internal StagingColumn StagingColumn { get; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "A value is required.")]
		[System.ComponentModel.Browsable(false)]
		public string Name
		{
			get { return StagingColumn.Name; }
			set
			{
				SetProperty(StagingColumn.Name, value, StagingColumn, (stagingColumn, name) => stagingColumn.Name = name, true);
			}
		}

		[SortIndex(1)]
		[System.ComponentModel.Category("Data type")]
		public SqlServerDataType DataType
		{
			get { return StagingColumn.DataType; }
			set
			{
				SetProperty(StagingColumn.DataType, value, StagingColumn, (stagingColumn, dataType) => stagingColumn.DataType = dataType);

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
			get { return StagingColumn.Nullable; }
			set
			{
				SetProperty(StagingColumn.Nullable, value, StagingColumn, (stagingColumn, nullable) => stagingColumn.Nullable = nullable);

				OnPropertyChanged(nameof(DataTypeString));
			}
		}

		[SortIndex(3)]
		[VisibleBy(nameof(GuiLengthVisible), true)]
		[AutoUpdateText]
		[System.ComponentModel.Category("Data type")]
		public int? Length
		{
			get { return StagingColumn.Length; }
			set
			{
				SetProperty(StagingColumn.Length, value, StagingColumn, (stagingColumn, length) => stagingColumn.Length = length);

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
			get { return StagingColumn.Precision; }
			set
			{
				SetProperty(StagingColumn.Precision, value, StagingColumn, (stagingColumn, precision) => stagingColumn.Precision = precision);

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
			get { return StagingColumn.Scale; }
			set
			{
				SetProperty(StagingColumn.Scale, value, StagingColumn, (stagingColumn, scale) => stagingColumn.Scale = scale);

				OnPropertyChanged(nameof(GuiScaleVisible));
				OnPropertyChanged(nameof(DataTypeString));
			}
		}

		[SortIndex(7)]
		[System.ComponentModel.Category("General")]
		public string? Logic
		{
			get { return StagingColumn.Logic; }
			set
			{
				SetProperty(StagingColumn.Logic, value, StagingColumn, (stagingColumn, logic) => stagingColumn.Logic = logic);
			}
		}

		[System.ComponentModel.Browsable(false)]
		public Column? LoadColumn
		{
			get { return StagingColumn.LoadColumn; }
			set
			{
				SetProperty(StagingColumn.LoadColumn, value, StagingColumn, (stagingColumn, loadColumn) => stagingColumn.LoadColumn = loadColumn);
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
						dataType += $" ({Length + ")"}";

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
		[System.ComponentModel.Browsable(false)]
		public new bool HasErrors => base.HasErrors;
	}
}
