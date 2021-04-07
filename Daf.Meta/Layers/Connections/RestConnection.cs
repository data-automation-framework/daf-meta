// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Dahomey.Json.Attributes;
using PropertyTools.DataAnnotations;

namespace Daf.Meta.Layers.Connections
{
	[JsonDiscriminator("Rest")]
	public class RestConnection : Connection
	{
		public RestConnection(string name)
		{
			Name = name;
		}

		private string? _baseUrl;

		[Category("REST")]
		[SortIndex(100)]
		public string? BaseUrl
		{
			get { return _baseUrl; }
			set
			{
				if (_baseUrl != value)
				{
					_baseUrl = value;

					NotifyPropertyChanged("BaseUrl");
				}
			}
		}

		private string? _encryptedCredential;

		[Category("REST")]
		[SortIndex(100)]
		public string? EncryptedCredential
		{
			get { return _encryptedCredential; }
			set
			{
				if (_encryptedCredential != value)
				{
					_encryptedCredential = value;

					NotifyPropertyChanged("EncryptedCredential");
				}
			}
		}

		private HttpAuthorization _authorization;

		[Category("REST")]
		[SelectorStyle(SelectorStyle.ComboBox)]
		[SortIndex(100)]
		public HttpAuthorization Authorization
		{
			get { return _authorization; }
			set
			{
				if (_authorization != value)
				{
					_authorization = value;

					NotifyPropertyChanged("Authorization");
				}
			}
		}

		private string? _tokenAbsoluteUrl;

		[Category("REST")]
		[IndentationLevel(1)]
		[VisibleBy(nameof(TokenOrOauth2Selected))]
		[SortIndex(100)]
		public string? TokenAbsoluteUrl
		{
			get { return _tokenAbsoluteUrl; }
			set
			{
				if (_tokenAbsoluteUrl != value)
				{
					_tokenAbsoluteUrl = value;

					NotifyPropertyChanged("TokenAbsoluteUrl");
				}
			}
		}

		[Category("REST")]
		[IndentationLevel(1)]
		[VisibleBy(nameof(TokenOrOauth2Selected))]
		[SortIndex(100)]
		public List<KeyValue> TokenBody { get; } = new();

		[Category("REST")]
		[IndentationLevel(1)]
		[VisibleBy(nameof(TokenOrOauth2Selected))]
		[SortIndex(100)]
		public List<KeyValue> TokenParameters { get; } = new();

		private string? _tokenJsonIdentifier;

		[Category("REST")]
		[IndentationLevel(1)]
		[VisibleBy(nameof(TokenOrOauth2Selected))]
		[SortIndex(100)]
		public string? TokenJsonIdentifier
		{
			get { return _tokenJsonIdentifier; }
			set
			{
				if (_tokenJsonIdentifier != value)
				{
					_tokenJsonIdentifier = value;

					NotifyPropertyChanged("TokenJsonIdentifier");
				}
			}
		}

		private string? _clientID;

		[Category("REST")]
		[IndentationLevel(1)]
		[VisibleBy(nameof(Authorization), HttpAuthorization.OAuth2)]
		[SortIndex(100)]
		public string? ClientID
		{
			get { return _clientID; }
			set
			{
				if (_clientID != value)
				{
					_clientID = value;

					NotifyPropertyChanged("ClientID");
				}
			}
		}

		private string? _clientSecret;

		[Category("REST")]
		[IndentationLevel(1)]
		[VisibleBy(nameof(Authorization), HttpAuthorization.OAuth2)]
		[SortIndex(100)]
		public string? ClientSecret
		{
			get { return _clientSecret; }
			set
			{
				if (_clientSecret != value)
				{
					_clientSecret = value;

					NotifyPropertyChanged("ClientSecret");
				}
			}
		}

		private GrantType? _grantType;

		[Category("REST")]
		[IndentationLevel(1)]
		[VisibleBy(nameof(Authorization), HttpAuthorization.OAuth2)]
		[SelectorStyle(SelectorStyle.ComboBox)]
		[SortIndex(100)]
		public GrantType? GrantType
		{
			get { return _grantType; }
			set
			{
				if (_grantType != value)
				{
					_grantType = value;

					NotifyPropertyChanged("GrantType");
				}
			}
		}

		private string? _restUser;

		[Category("REST")]
		[IndentationLevel(1)]
		[VisibleBy(nameof(BasicOrOauth2Selected))]
		[SortIndex(100)]
		public string? RestUser
		{
			get { return _restUser; }
			set
			{
				if (_restUser != value)
				{
					_restUser = value;

					NotifyPropertyChanged("RestUser");
				}
			}
		}

		private string? _password;

		[Category("REST")]
		[IndentationLevel(1)]
		[VisibleBy(nameof(BasicOrOauth2Selected))]
		[SortIndex(100)]
		public string? Password
		{
			get { return _password; }
			set
			{
				if (_password != value)
				{
					_password = value;

					NotifyPropertyChanged("Password");
				}
			}
		}

		[Browsable(false)]
		[JsonIgnore]
		public bool BasicOrOauth2Selected => Authorization is HttpAuthorization.Basic or HttpAuthorization.OAuth2;

		[Browsable(false)]
		[JsonIgnore]
		public bool TokenOrOauth2Selected => Authorization is HttpAuthorization.Token or HttpAuthorization.OAuth2;
	}
}
