// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using PropertyTools.DataAnnotations;

namespace Daf.Meta.Editor.ViewModels
{
	public class BusinessKeyViewModel : ObservableObject
	{
		public BusinessKeyViewModel(StagingColumn stagingColumn)
		{
			StagingColumn = stagingColumn;
		}

		internal StagingColumn StagingColumn { get; }

		[System.ComponentModel.Browsable(false)]
		public string Name
		{
			get { return StagingColumn.Name; }
			set
			{
				SetProperty(StagingColumn.Name, value, StagingColumn, (stagingColumn, name) => stagingColumn.Name = name);
			}
		}

		[SortIndex(1)]
		[AutoUpdateText]
		[System.ComponentModel.Category("General")]
		public bool? Driving
		{
			get { return StagingColumn.Driving; }
			set
			{
				SetProperty(StagingColumn.Driving, value, StagingColumn, (stagingColumn, driving) => stagingColumn.Driving = driving);

				OnPropertyChanged(nameof(DataTypeString));
			}
		}

		[SortIndex(2)]
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

		[SortIndex(3)]
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

		[SortIndex(4)]
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

		[SortIndex(5)]
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

		[SortIndex(6)]
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
					case SqlServerDataType.SmallInt:
					case SqlServerDataType.Int:
					case SqlServerDataType.BigInt:
					case SqlServerDataType.UniqueIdentifier:
						break;
					default:
						throw new InvalidOperationException($"Data type {DataType} is not supported by Daf.Meta.Editor");
				}

				return dataType.PadRight(18) + $"{(Nullable ? " NULL" : " NOT NULL"),9}{(Driving == true ? " Driving" : string.Empty),12}";
			}
		}
	}
}
