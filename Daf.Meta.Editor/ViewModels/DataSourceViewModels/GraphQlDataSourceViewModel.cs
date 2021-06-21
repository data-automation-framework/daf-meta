// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Daf.Meta.Layers;
using Daf.Meta.Layers.Connections;
using Daf.Meta.Layers.DataSources;
using PropertyTools.DataAnnotations;

namespace Daf.Meta.Editor.ViewModels
{
	public class GraphQlDataSourceViewModel : DataSourceViewModel
	{
		public GraphQlDataSourceViewModel(DataSource dataSource) : base(dataSource)
		{
			_graphQlDataSource = (GraphQlDataSource)dataSource;
		}

		private readonly GraphQlDataSource _graphQlDataSource;

		public override DataSource DataSource { get => _graphQlDataSource; }

		[Browsable(false)]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Static collections don't appear to work when binding to WPF.")]
		public ObservableCollection<Connection> Connections { get { return Model.Instance.Connections; } }

		[Category("GraphQL")]
		[SelectorStyle(SelectorStyle.ComboBox)]
		[ItemsSourceProperty("Connections")]
		[DisplayMemberPath("Name")]
		[SortIndex(100)]
		public GraphQlConnection Connection
		{
			get => _graphQlDataSource.Connection;
			set
			{
				SetProperty(_graphQlDataSource.Connection, value, _graphQlDataSource, (dataSource, connection) => _graphQlDataSource.Connection = connection, true);
			}
		}

		[Category("GraphQL")]
		[SortIndex(100)]
		[Spinnable(1, 10)]
		[Width(60)]
		public uint ConnectionRetryAttempts
		{
			get => _graphQlDataSource.ConnectionRetryAttempts;
			set
			{
				SetProperty(_graphQlDataSource.ConnectionRetryAttempts, value, _graphQlDataSource, (dataSource, connectionRetryAttempts) => _graphQlDataSource.ConnectionRetryAttempts = connectionRetryAttempts, true);
			}
		}

		[Category("GraphQL")]
		[SortIndex(100)]
		[Spinnable(1, 10)]
		[Width(60)]
		public uint ConnectionRetryMinutes
		{
			get => _graphQlDataSource.ConnectionRetryMinutes;
			set
			{
				SetProperty(_graphQlDataSource.ConnectionRetryMinutes, value, _graphQlDataSource, (dataSource, connectionRetryMinutes) => _graphQlDataSource.ConnectionRetryMinutes = connectionRetryMinutes, true);
			}
		}

		[Category("GraphQL")]
		[SortIndex(100)]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "<Pending>")]
		public string? RelativeUrl
		{
			get => _graphQlDataSource.RelativeUrl;
			set
			{
				SetProperty(_graphQlDataSource.RelativeUrl, value, _graphQlDataSource, (dataSource, relativeUrl) => _graphQlDataSource.RelativeUrl = relativeUrl, true);
			}
		}

		[Category("GraphQL")]
		[SortIndex(100)]
		public int NumberToFetch
		{
			get => _graphQlDataSource.NumberToFetch;
			set
			{
				SetProperty(_graphQlDataSource.NumberToFetch, value, _graphQlDataSource, (dataSource, numberToFetch) => _graphQlDataSource.NumberToFetch = numberToFetch, true);
			}
		}

		[Category("GraphQL")]
		[DataType(DataType.MultilineText)]
		[SortIndex(100)]
		public string GraphQlQuery
		{
			get => _graphQlDataSource.GraphQlQuery;
			set
			{
				SetProperty(_graphQlDataSource.GraphQlQuery, value, _graphQlDataSource, (dataSource, graphQlQuery) => _graphQlDataSource.GraphQlQuery = graphQlQuery, true);
			}
		}

		[Category("GraphQL")]
		[SortIndex(100)]
		public string? Parent
		{
			get => _graphQlDataSource.Parent;
			set
			{
				SetProperty(_graphQlDataSource.Parent, value, _graphQlDataSource, (dataSource, parent) => _graphQlDataSource.Parent = parent, true);
			}
		}

		[Category("GraphQL")]
		[SortIndex(100)]
		public string? CollectionReference
		{
			get => _graphQlDataSource.CollectionReference;
			set
			{
				SetProperty(_graphQlDataSource.CollectionReference, value, _graphQlDataSource, (dataSource, collectionReference) => _graphQlDataSource.CollectionReference = collectionReference, true);
			}
		}

		[Category("General")]
		[VisibleBy(nameof(DestinationType), DestinationType.Blob)]
		public string? DestinationEncoding
		{
			get => _graphQlDataSource.DestinationEncoding;
			set
			{
				SetProperty(_graphQlDataSource.DestinationEncoding, value, _graphQlDataSource, (dataSource, destinationEncoding) => _graphQlDataSource.DestinationEncoding = destinationEncoding, true);
			}
		}

		[Category("General")]
		[VisibleBy(nameof(DestinationType), DestinationType.Blob)]
		public bool MergeToBlob
		{
			get => _graphQlDataSource.MergeToBlob;
			set
			{
				SetProperty(_graphQlDataSource.MergeToBlob, value, _graphQlDataSource, (dataSource, mergeToBlob) => _graphQlDataSource.MergeToBlob = mergeToBlob, true);
			}
		}
	}
}
