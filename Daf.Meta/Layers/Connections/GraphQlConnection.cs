// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using Dahomey.Json.Attributes;

namespace Daf.Meta.Layers.Connections
{
	[JsonDiscriminator("GraphQl")]
	public class GraphQlConnection : Connection
	{
		public GraphQlConnection(string name)
		{
			Name = name;
		}

		private string? _authorizationToken;

		public string? AuthorizationToken
		{
			get { return _authorizationToken; }
			set
			{
				if (_authorizationToken != value)
				{
					_authorizationToken = value;

					NotifyPropertyChanged("AuthorizationToken");
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

		private string? _baseUrl;

		public string? BaseUrl
		{
			get
			{
				return _baseUrl;
			}
			set
			{
				if (_baseUrl != value)
				{
					_baseUrl = value;
					NotifyPropertyChanged("BaseUrl");
				}
			}
		}
	}
}
