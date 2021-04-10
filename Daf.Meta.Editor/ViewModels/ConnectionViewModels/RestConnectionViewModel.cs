// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using Daf.Meta.Layers;
using Daf.Meta.Layers.Connections;
using PropertyTools.DataAnnotations;

namespace Daf.Meta.Editor.ViewModels
{
	public class RestConnectionViewModel : ConnectionViewModel
	{
		private readonly RestConnection _restConnection;

		public RestConnectionViewModel(Connection connection) : base(connection)
		{
			_restConnection = (RestConnection)connection;
		}

		public override Connection Connection
		{
			get { return _restConnection; }
		}

		[Category("REST")]
		[SortIndex(100)]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "<Pending>")]
		public string? BaseUrl
		{
			get { return _restConnection.BaseUrl; }
			set
			{
				SetProperty(_restConnection.BaseUrl, value, _restConnection, (connection, baseUrl) => _restConnection.BaseUrl = baseUrl, true);
			}
		}

		[Category("REST")]
		[SortIndex(100)]
		public string? EncryptedCredential
		{
			get { return _restConnection.EncryptedCredential; }
			set
			{
				SetProperty(_restConnection.EncryptedCredential, value, _restConnection, (connection, encryptedCredential) => _restConnection.EncryptedCredential = encryptedCredential, true);

			}
		}

		[Category("REST")]
		[SelectorStyle(SelectorStyle.ComboBox)]
		[SortIndex(100)]
		public HttpAuthorization Authorization
		{
			get { return _restConnection.Authorization; }
			set
			{
				SetProperty(_restConnection.Authorization, value, _restConnection, (connection, authorization) => _restConnection.Authorization = authorization, true);
			}
		}

		[Category("REST")]
		[IndentationLevel(1)]
		[VisibleBy(nameof(TokenOrOauth2Selected))]
		[SortIndex(100)]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "<Pending>")]
		public string? TokenAbsoluteUrl
		{
			get { return _restConnection.TokenAbsoluteUrl; }
			set
			{
				SetProperty(_restConnection.TokenAbsoluteUrl, value, _restConnection, (connection, tokenAbsoluteUrl) => _restConnection.TokenAbsoluteUrl = tokenAbsoluteUrl, true);
			}
		}

		[Category("REST")]
		[IndentationLevel(1)]
		[VisibleBy(nameof(TokenOrOauth2Selected))]
		[SortIndex(100)]
		public List<KeyValue> TokenBody
		{
			get { return _restConnection.TokenBody; }
		}

		[Category("REST")]
		[IndentationLevel(1)]
		[VisibleBy(nameof(TokenOrOauth2Selected))]
		[SortIndex(100)]
		public List<KeyValue> TokenParameters
		{
			get { return _restConnection.TokenParameters; }
		}

		[Category("REST")]
		[IndentationLevel(1)]
		[VisibleBy(nameof(TokenOrOauth2Selected))]
		[SortIndex(100)]
		public string? TokenJsonIdentifier
		{
			get { return _restConnection.TokenJsonIdentifier; }
			set
			{
				SetProperty(_restConnection.TokenJsonIdentifier, value, _restConnection, (connection, tokenJsonIdentifier) => _restConnection.TokenJsonIdentifier = tokenJsonIdentifier, true);
			}
		}

		[Category("REST")]
		[IndentationLevel(1)]
		[VisibleBy(nameof(Authorization), HttpAuthorization.OAuth2)]
		[SortIndex(100)]
		public string? ClientID
		{
			get { return _restConnection.ClientID; }
			set
			{
				SetProperty(_restConnection.EncryptedCredential, value, _restConnection, (connection, clientID) => _restConnection.ClientID = clientID, true);
			}
		}

		[Category("REST")]
		[IndentationLevel(1)]
		[VisibleBy(nameof(Authorization), HttpAuthorization.OAuth2)]
		[SortIndex(100)]
		public string? ClientSecret
		{
			get { return _restConnection.ClientSecret; }
			set
			{
				SetProperty(_restConnection.ClientSecret, value, _restConnection, (connection, clientSecret) => _restConnection.ClientSecret = clientSecret, true);
			}
		}

		[Category("REST")]
		[IndentationLevel(1)]
		[VisibleBy(nameof(Authorization), HttpAuthorization.OAuth2)]
		[SelectorStyle(SelectorStyle.ComboBox)]
		[SortIndex(100)]
		public GrantType? GrantType
		{
			get { return _restConnection.GrantType; }
			set
			{
				SetProperty(_restConnection.GrantType, value, _restConnection, (connection, grantType) => _restConnection.GrantType = grantType, true);
			}
		}

		[Category("REST")]
		[IndentationLevel(1)]
		[VisibleBy(nameof(BasicOrOauth2Selected))]
		[SortIndex(100)]
		public string? RestUser
		{
			get { return _restConnection.RestUser; }
			set
			{
				SetProperty(_restConnection.RestUser, value, _restConnection, (connection, restUser) => _restConnection.RestUser = restUser, true);
			}
		}

		[Category("REST")]
		[IndentationLevel(1)]
		[VisibleBy(nameof(BasicOrOauth2Selected))]
		[SortIndex(100)]
		public string? Password
		{
			get { return _restConnection.Password; }
			set
			{
				SetProperty(_restConnection.Password, value, _restConnection, (connection, password) => _restConnection.Password = password, true);
			}
		}

		[Browsable(false)]
		public bool BasicOrOauth2Selected
		{
			get { return _restConnection.BasicOrOauth2Selected; }
		}

		[Browsable(false)]
		public bool TokenOrOauth2Selected
		{
			get { return _restConnection.TokenOrOauth2Selected; }
		}

		// Preventing the inherited HasErrors property from showing up in the PropertyGrid.
		[System.ComponentModel.Browsable(false)]
		public new bool HasErrors
		{
			get
			{
				return base.HasErrors;
			}
		}
	}
}
