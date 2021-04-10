// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Daf.Meta.Layers;
using PropertyTools.DataAnnotations;
using Daf.Meta.Layers.Connections;

namespace Daf.Meta.Editor.ViewModels
{
	public class GraphQlConnectionViewModel : ConnectionViewModel
	{
		private readonly GraphQlConnection _graphQlConnection;

		public GraphQlConnectionViewModel(Connection connection) : base(connection)
		{
			_graphQlConnection = (GraphQlConnection)connection;
		}

		public override Connection Connection
		{
			get { return _graphQlConnection; }
		}

		[Category("GRAPHQL")]
		[SortIndex(100)]
		public string? AuthorizationToken
		{
			get { return _graphQlConnection.AuthorizationToken; }
			set
			{
				SetProperty(_graphQlConnection.AuthorizationToken, value, _graphQlConnection, (connection, baseUrl) => _graphQlConnection.AuthorizationToken = baseUrl, true);
			}
		}

		[Category("GRAPHQL")]
		[SortIndex(100)]
		public string? EncryptedCredential
		{
			get { return _graphQlConnection.EncryptedCredential; }
			set
			{
				SetProperty(_graphQlConnection.EncryptedCredential, value, _graphQlConnection, (connection, encryptedCredential) => _graphQlConnection.EncryptedCredential = encryptedCredential, true);
			}
		}

		[Category("GRAPHQL")]
		[SortIndex(100)]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "<Pending>")]
		public string? BaseUrl
		{
			get { return _graphQlConnection.BaseUrl; }
			set
			{
				SetProperty(_graphQlConnection.BaseUrl, value, _graphQlConnection, (connection, baseUrl) => _graphQlConnection.BaseUrl = baseUrl, true);
			}
		}
	}
}
