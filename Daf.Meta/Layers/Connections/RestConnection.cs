// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Dahomey.Json.Attributes;

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

		public List<KeyValue> TokenBody { get; } = new();

		public List<KeyValue> TokenParameters { get; } = new();

		private string? _tokenJsonIdentifier;

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

		[JsonIgnore]
		public bool BasicOrOauth2Selected => Authorization is HttpAuthorization.Basic or HttpAuthorization.OAuth2;

		[JsonIgnore]
		public bool TokenOrOauth2Selected => Authorization is HttpAuthorization.Token or HttpAuthorization.OAuth2;
	}
}
