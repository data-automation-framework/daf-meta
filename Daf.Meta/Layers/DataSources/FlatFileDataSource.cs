// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Dahomey.Json.Attributes;

namespace Daf.Meta.Layers.DataSources
{
	[JsonDiscriminator("FlatFile")]
	public class FlatFileDataSource : DataSource
	{
		public FlatFileDataSource(string name, SourceSystem sourceSystem, Tenant tenant) : base(name, sourceSystem, tenant)
		{
		}

		private uint? _codePage { get; set; }

		public uint? CodePage
		{
			get { return _codePage; }
			set
			{
				if (_codePage != value)
				{
					_codePage = value;

					NotifyPropertyChanged("CodePage");
				}
			}
		}

		private string? _format;

		public string? Format
		{
			get { return _format; }
			set
			{
				if (_format != value)
				{
					_format = value;

					NotifyPropertyChanged("Format");
				}
			}
		}

		private string? _rowDelimiter;

		public string? RowDelimiter
		{
			get { return _rowDelimiter; }
			set
			{
				if (_rowDelimiter != value)
				{
					_rowDelimiter = value;

					NotifyPropertyChanged("RowDelimiter");
				}
			}
		}

		private string? _columnDelimiter;

		public string? ColumnDelimiter
		{
			get { return _columnDelimiter; }
			set
			{
				if (_columnDelimiter != value)
				{
					_columnDelimiter = value;

					NotifyPropertyChanged("ColumnDelimiter");
				}
			}
		}

		private bool? _textQualified;

		public bool? TextQualified
		{
			get { return _textQualified; }
			set
			{
				if (_textQualified != value)
				{
					_textQualified = value;

					NotifyPropertyChanged("TextQualified");
				}
			}
		}

		private string? _textQualifier;

		public string? TextQualifier
		{
			get { return _textQualifier; }
			set
			{
				if (_textQualifier != value)
				{
					_textQualifier = value;

					NotifyPropertyChanged("TextQualifier");
				}
			}
		}

		private bool? _headersInFirstRow;

		public bool? HeadersInFirstRow
		{
			get { return _headersInFirstRow; }
			set
			{
				if (_headersInFirstRow != value)
				{
					_headersInFirstRow = value;

					NotifyPropertyChanged("HeadersInFirstRow");
				}
			}
		}

		private uint _businessDateOffset;

		public uint BusinessDateOffset
		{
			get { return _businessDateOffset; }
			set
			{
				if (_businessDateOffset != value)
				{
					_businessDateOffset = value;

					NotifyPropertyChanged("BusinessDateOffset");
				}
			}
		}

		private string? _fileDateRegex;

		public string? FileDateRegex
		{
			get { return _fileDateRegex; }
			set
			{
				if (_fileDateRegex != value)
				{
					_fileDateRegex = value;

					NotifyPropertyChanged("FileDateRegex");
				}
			}
		}

		public override DataSource Clone()
		{
			throw new System.NotImplementedException();
		}

		public override void GetMetadata()
		{
			throw new System.NotImplementedException();
		}
	}
}
