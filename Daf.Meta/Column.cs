// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Daf.Meta
{
	public class Column : PropertyChangedBaseClass
	{
		public Column(string name)
		{
			_name = name;
		}

		private string _name;

		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;

					NotifyPropertyChanged("Name");
				}
			}
		}

		private SqlServerDataType _dataType;

		public SqlServerDataType DataType
		{
			get { return _dataType; }
			set
			{
				if (_dataType != value)
				{
					_dataType = value;

					switch (_dataType)
					{
						case SqlServerDataType.VarChar:
						case SqlServerDataType.NVarChar:
						case SqlServerDataType.Binary:
							Precision = null;
							Scale = null;

							break;
						case SqlServerDataType.Decimal:
							Length = null;

							break;
						case SqlServerDataType.DateTime2:
						case SqlServerDataType.DateTimeOffset:
							Length = null;
							Precision = null;

							break;
						case SqlServerDataType.Int:
						case SqlServerDataType.BigInt:
						case SqlServerDataType.Date:
						case SqlServerDataType.Bit:
						case SqlServerDataType.DateTime:
						case SqlServerDataType.TinyInt:
						case SqlServerDataType.Float:
						case SqlServerDataType.UniqueIdentifier:
						case SqlServerDataType.SmallInt:
							Length = null;
							Precision = null;
							Scale = null;

							break;
					}

					NotifyPropertyChanged("DataType");
					NotifyPropertyChanged("GuiLengthVisible");
					NotifyPropertyChanged("GuiPrecisionVisible");
					NotifyPropertyChanged("GuiScaleVisible");
					NotifyPropertyChanged("DataTypeString");
				}
			}
		}

		// This JsonIgnore shouldn't be necessary, but the Dahomey.Json library used for polymorphic serialization
		// appears to change it so that internal variables are serialized by default.
		[JsonIgnore]
		internal int DataTypeStringLength
		{
			get
			{
				switch (DataType)
				{
					case SqlServerDataType.VarChar:
						return _length ?? throw new InvalidOperationException($"Column {_name} of type {_dataType} has NULL length.");
					case SqlServerDataType.NVarChar:
						return _length ?? throw new InvalidOperationException($"Column {_name} of type {_dataType} has NULL length.");
					case SqlServerDataType.Int:
						return 11;
					case SqlServerDataType.BigInt:
						return 20;
					case SqlServerDataType.Date:
						return 10;
					case SqlServerDataType.Bit:
						return 1;
					case SqlServerDataType.DateTime:
						return 19;
					case SqlServerDataType.DateTime2:
						if (_scale.HasValue)
						{
							if (_scale.Value == 0)
							{
								return 19;
							}
							else
							{
								return 20 + _scale.Value;
							}
						}

						throw new InvalidOperationException($"Column {_name} of type {_dataType} has NULL scale.");
					case SqlServerDataType.DateTimeOffset:
						if (_scale.HasValue)
						{
							if (_scale.Value == 0)
							{
								return 26;
							}
							else
							{
								return 27 + _scale.Value;
							}
						}

						throw new InvalidOperationException($"Column {_name} of type {_dataType} has NULL scale.");
					case SqlServerDataType.Decimal:
						if (_scale.HasValue && _precision.HasValue)
						{
							int minusSignLength = 1;

							int beforeDecimalLength;
							int decimalLength = default;
							int afterDecimalLength = default;

							if (_precision.Value - _scale.Value == 0)
							{
								// For example number -0.01 with precision 2 and scale 2, still 1 number before decimal point.
								beforeDecimalLength = 1;
							}
							else
							{
								beforeDecimalLength = _precision.Value - _scale.Value;
							}

							if (_scale > 0)
							{
								afterDecimalLength = _scale.Value;
								decimalLength = 1;
							}

							return minusSignLength + beforeDecimalLength + decimalLength + afterDecimalLength;
						}
						throw new InvalidOperationException($"Column {_name} of type {_dataType} cannot have NULL precision (has {_precision}) or scale (has {_scale}).");
					case SqlServerDataType.TinyInt:
						return 3;
					case SqlServerDataType.Float:
						if (_length.HasValue)
						{
							throw new NotImplementedException($"Column {_name} of type {_dataType} does not yet have support for mantissa bit specification.");
						}
						return 24;
					case SqlServerDataType.UniqueIdentifier:
						return 36;
					case SqlServerDataType.SmallInt:
						return 6;
					case SqlServerDataType.Binary:
						if (_length.HasValue)
						{
							return 2 + (2 * _length.Value);
						}
						return 4;
					default:
						throw new NotSupportedException($"Datatype {_dataType} in column {_name} not supported.");
				}
			}
		}

		private bool _nullable;

		public bool Nullable
		{
			get { return _nullable; }
			set
			{
				if (_nullable != value)
				{
					_nullable = value;

					NotifyPropertyChanged("Nullable");
					NotifyPropertyChanged("DataTypeString");
				}
			}
		}

		private int? _length;

		public int? Length
		{
			get { return _length; }
			set
			{
				if (_length != value)
				{
					_length = value;

					NotifyPropertyChanged("Length");
					NotifyPropertyChanged("GuiLengthVisible");
					NotifyPropertyChanged("DataTypeString");
				}
			}
		}

		private int? _precision;

		public int? Precision
		{
			get { return _precision; }
			set
			{
				if (_precision != value)
				{
					_precision = value;

					NotifyPropertyChanged("Precision");
					NotifyPropertyChanged("GuiPrecisionVisible");
					NotifyPropertyChanged("DataTypeString");
				}
			}
		}

		private int? _scale;

		public int? Scale
		{
			get { return _scale; }
			set
			{
				if (_scale != value)
				{
					_scale = value;

					NotifyPropertyChanged("Scale");
					NotifyPropertyChanged("GuiScaleVisible");
					NotifyPropertyChanged("DataTypeString");
				}
			}
		}

		// This is only used for load table columns.
		// Should load column be its own class, derived from column? Maybe it'd be better to make column abstract.
		private string? _addedOnBusinessDate;

		public string? AddedOnBusinessDate
		{
			get { return _addedOnBusinessDate; }
			set
			{
				if (_addedOnBusinessDate != value)
				{
					_addedOnBusinessDate = value;

					if (string.IsNullOrEmpty(_addedOnBusinessDate))
						_addedOnBusinessDate = null;

					NotifyPropertyChanged("AddedOnBusinessDate");
				}
			}
		}

		public string GetIonColumn()
		{
			string column;

			if (Length == null && Precision == null && Scale == null)
			{
				column = "";
			}
			else if (Length == null && Precision != null && Scale != null)
			{
				column = $"Precision=\"{Precision.Value.ToString(CultureInfo.InvariantCulture)}\" Scale=\"{Scale.Value.ToString(CultureInfo.InvariantCulture)}\"";
			}
			else if (Length != null && Precision == null && Scale == null)
			{
				column = $"Length=\"{Length.Value.ToString(CultureInfo.InvariantCulture)}\"";
			}
			else if (Length == null && Precision == null && Scale != null)
			{
				column = $"Scale=\"{Scale.Value.ToString(CultureInfo.InvariantCulture)}\"";
			}
			else
			{
				throw new InvalidOperationException($"Case not handled by GetIonColumn method for column {Name}. Length: {Length?.ToString(CultureInfo.InvariantCulture)}, Precision: {Precision?.ToString(CultureInfo.InvariantCulture)}, Scale: {Scale?.ToString(CultureInfo.InvariantCulture)}");
			}

			return column;
		}

		public virtual bool IsValid(out string message)
		{
			message = string.Empty;

			if (string.IsNullOrWhiteSpace(Name))
			{
				message = "Column does not have a name!";

				return false;
			}

			return true;
		}
	}
}
