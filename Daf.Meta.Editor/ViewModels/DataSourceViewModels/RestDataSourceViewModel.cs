// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Daf.Meta.Layers;
using Daf.Meta.Layers.Connections;
using Daf.Meta.Layers.DataSources;
using PropertyTools.DataAnnotations;

namespace Daf.Meta.Editor.ViewModels
{
	public class RestDataSourceViewModel : DataSourceViewModel
	{
		private readonly RestDataSource _restDataSource;

		public RestDataSourceViewModel(DataSource dataSource) : base(dataSource)
		{
			_restDataSource = (RestDataSource)dataSource;
		}

		public override DataSource DataSource => _restDataSource;

		// ItemsSource for Connection.
		[Browsable(false)]
		public ObservableCollection<Connection> Connections => _restDataSource.Connections;

		[Category("REST")]
		[SelectorStyle(SelectorStyle.ComboBox)]
		[ItemsSourceProperty("Connections")]
		[DisplayMemberPath("Name")]
		[SortIndex(100)]
		public RestConnection Connection
		{
			get => _restDataSource.Connection;
			set
			{
				SetProperty(_restDataSource.Connection, value, _restDataSource, (dataSource, connection) => dataSource.Connection = connection, true);
			}
		}

		[Category("REST")]
		[SortIndex(100)]
		[Spinnable(1, 10)]
		[Width(60)]
		public uint ConnectionRetryAttempts
		{
			get => _restDataSource.ConnectionRetryAttempts;
			set
			{
				SetProperty(_restDataSource.ConnectionRetryAttempts, value, _restDataSource, (dataSource, connectionRetryAttempts) => dataSource.ConnectionRetryAttempts = connectionRetryAttempts, true);
			}
		}

		[Category("REST")]
		[SortIndex(100)]
		[Spinnable(1, 10)]
		[Width(60)]
		public uint ConnectionRetryMinutes
		{
			get => _restDataSource.ConnectionRetryMinutes;
			set
			{
				SetProperty(_restDataSource.ConnectionRetryMinutes, value, _restDataSource, (dataSource, connectionRetryMinutes) => dataSource.ConnectionRetryMinutes = connectionRetryMinutes, true);
			}
		}

		[Category("REST")]
		[SortIndex(100)]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "<Pending>")]
		public string? RelativeUrl
		{
			get => _restDataSource.RelativeUrl;
			set
			{
				SetProperty(_restDataSource.RelativeUrl, value, _restDataSource, (dataSource, relativeUrl) => dataSource.RelativeUrl = relativeUrl, true);
			}
		}

		[Category("REST")]
		[SortIndex(100)]
		public bool SaveCookie
		{
			get => _restDataSource.SaveCookie;
			set
			{
				SetProperty(_restDataSource.SaveCookie, value, _restDataSource, (dataSource, saveCookie) => dataSource.SaveCookie = saveCookie, true);
			}
		}

		[Category("REST")]
		[SortIndex(100)]
		public string? CustomScriptPath
		{
			get => _restDataSource.CustomScriptPath;
			set
			{
				SetProperty(_restDataSource.CustomScriptPath, value, _restDataSource, (dataSource, customScriptPath) => dataSource.CustomScriptPath = customScriptPath, true);
			}
		}

		[Category("REST")]
		[VisibleBy("Authorization", HttpAuthorization.Token)]
		[SortIndex(100)]
		public List<KeyValue> Parameters
		{
			get => _restDataSource.Parameters;
			//set
			//{
			//	SetProperty(_restDataSource.Parameters, value, _restDataSource, (dataSource, parameters) => dataSource.Parameters = parameters, true);
			//}
		}

		[Category("General")]
		[VisibleBy(nameof(DestinationType), DestinationType.Blob)]
		public string? DestinationEncoding
		{
			get => _restDataSource.DestinationEncoding;
			set
			{
				SetProperty(_restDataSource.DestinationEncoding, value, _restDataSource, (dataSource, destinationEncoding) => dataSource.DestinationEncoding = destinationEncoding, true);
			}
		}

		[Category("General")]
		[VisibleBy(nameof(DestinationType), DestinationType.Blob)]
		public bool MergeToBlob
		{
			get => _restDataSource.MergeToBlob;
			set
			{
				SetProperty(_restDataSource.MergeToBlob, value, _restDataSource, (dataSource, mergeToBlob) => dataSource.MergeToBlob = mergeToBlob, true);
			}
		}

		[Category("REST")]
		[Description(
			"Azure Data Factory expression that should evaluate to HTTP parameters relevant to incremental filtering." +
			"The text '{incremental}' will be replaced with the output column name of the IncrementalQuery SQL statement." +
			"The text '{load}' will be replaced with the load column corresponding to IncrementalStagingColumn.")]
		public string? IncrementalExpression
		{
			get => _restDataSource.IncrementalExpression;
			set
			{
				SetProperty(_restDataSource.IncrementalExpression, value, _restDataSource, (dataSource, incrementalExpression) => dataSource.IncrementalExpression = incrementalExpression, true);
			}
		}

		[Category("REST")]
		[SortIndex(100)]
		public string? CollectionReference
		{
			get => _restDataSource.CollectionReference;
			set
			{
				SetProperty(_restDataSource.CollectionReference, value, _restDataSource, (dataSource, collectionReference) => dataSource.CollectionReference = collectionReference, true);
			}
		}

		[Category("REST")]
		[SortIndex(100)]
		public string? PaginationNextLink
		{
			get => _restDataSource.PaginationNextLink;
			set
			{
				SetProperty(_restDataSource.PaginationNextLink, value, _restDataSource, (dataSource, paginationNextLink) => dataSource.PaginationNextLink = paginationNextLink, true);
			}
		}

		[Category("REST")]
		[SortIndex(100)]
		public bool PaginationLinkIsRelative
		{
			get => _restDataSource.PaginationLinkIsRelative;
			set
			{
				SetProperty(_restDataSource.PaginationLinkIsRelative, value, _restDataSource, (dataSource, paginationLinkIsRelative) => dataSource.PaginationLinkIsRelative = paginationLinkIsRelative, true);
			}
		}
	}
}
