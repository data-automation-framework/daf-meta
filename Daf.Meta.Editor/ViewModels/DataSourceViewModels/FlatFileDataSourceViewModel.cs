// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Meta.Layers;
using Daf.Meta.Layers.DataSources;
using PropertyTools.DataAnnotations;

namespace Daf.Meta.Editor.ViewModels
{
	public class FlatFileDataSourceViewModel : DataSourceViewModel
	{
		public FlatFileDataSourceViewModel(DataSource dataSource) : base(dataSource)
		{
			_flatFileDataSource = (FlatFileDataSource)dataSource;
		}

		private readonly FlatFileDataSource _flatFileDataSource;

		public override DataSource DataSource { get => _flatFileDataSource; }

		[Category("FlatFile")]
		[SortIndex(100)]
		public uint? CodePage
		{
			get => _flatFileDataSource.CodePage;
			set
			{
				SetProperty(_flatFileDataSource.CodePage, value, _flatFileDataSource, (dataSource, codePage) => _flatFileDataSource.CodePage = codePage, true);
			}
		}

		[Category("FlatFile")]
		[SortIndex(100)]
		public string? Format
		{
			get => _flatFileDataSource.Format;
			set
			{
				SetProperty(_flatFileDataSource.Format, value, _flatFileDataSource, (dataSource, format) => _flatFileDataSource.Format = format, true);
			}
		}

		[Category("FlatFile")]
		[SortIndex(100)]
		public string? RowDelimiter
		{
			get => _flatFileDataSource.RowDelimiter;
			set
			{
				SetProperty(_flatFileDataSource.RowDelimiter, value, _flatFileDataSource, (dataSource, rowDelimiter) => _flatFileDataSource.RowDelimiter = rowDelimiter, true);
			}
		}

		[Category("FlatFile")]
		[SortIndex(100)]
		public string? ColumnDelimiter
		{
			get => _flatFileDataSource.ColumnDelimiter;
			set
			{
				SetProperty(_flatFileDataSource.ColumnDelimiter, value, _flatFileDataSource, (dataSource, columnDelimiter) => _flatFileDataSource.ColumnDelimiter = columnDelimiter, true);
			}
		}

		[Category("FlatFile")]
		[SortIndex(100)]
		public bool? TextQualified
		{
			get => _flatFileDataSource.TextQualified;
			set
			{
				SetProperty(_flatFileDataSource.TextQualified, value, _flatFileDataSource, (dataSource, textQualified) => _flatFileDataSource.TextQualified = textQualified, true);
			}
		}

		[Category("FlatFile")]
		[SortIndex(100)]
		public string? TextQualifier
		{
			get => _flatFileDataSource.TextQualifier;
			set
			{
				SetProperty(_flatFileDataSource.TextQualifier, value, _flatFileDataSource, (dataSource, textQualifier) => _flatFileDataSource.TextQualifier = textQualifier, true);
			}
		}

		[Category("FlatFile")]
		[SortIndex(100)]
		public bool? HeadersInFirstRow
		{
			get => _flatFileDataSource.HeadersInFirstRow;
			set
			{
				SetProperty(_flatFileDataSource.HeadersInFirstRow, value, _flatFileDataSource, (dataSource, headersInFirstRow) => _flatFileDataSource.HeadersInFirstRow = headersInFirstRow, true);
			}
		}

		[Category("FlatFile")]
		[SortIndex(100)]
		[Spinnable(1, 10)]
		[Width(60)]
		public uint BusinessDateOffset
		{
			get => _flatFileDataSource.BusinessDateOffset;
			set
			{
				SetProperty(_flatFileDataSource.BusinessDateOffset, value, _flatFileDataSource, (dataSource, businessDateOffset) => _flatFileDataSource.BusinessDateOffset = businessDateOffset, true);
			}
		}

		[Category("FlatFile")]
		[SortIndex(100)]
		public string? FileDateRegex
		{
			get => _flatFileDataSource.FileDateRegex;
			set
			{
				SetProperty(_flatFileDataSource.FileDateRegex, value, _flatFileDataSource, (dataSource, fileDateRegex) => _flatFileDataSource.FileDateRegex = fileDateRegex, true);
			}
		}
	}
}
