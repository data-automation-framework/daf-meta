// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Daf.Meta.Layers;
using Daf.Meta.Layers.Connections;
using Daf.Meta.Layers.DataSources;

namespace Daf.Meta
{
	public class DataTypeAnalyzer
	{
		private static readonly int[] PRECISION_BOUNDS = { 9, 19, 28, 38 };

		private const int UNKNOWN_VARCHAR_DEFAULT_LENGTH = 30;
		private DataSource DataSource { get; set; }
		private WebClient Client { get; set; } = new WebClient();
		private string? Url { get; set; }

		/// <summary>
		/// A dictionary containing suggested column names and whether or not we've set their suggested data types before.
		/// </summary>
		public Dictionary<string, bool> ColumnTypeDecided { get; } = new Dictionary<string, bool>();

		public DataTypeAnalyzer(DataSource dataSource)
		{
			DataSource = dataSource;
		}

		private string TryGetJson(string nextPageUrl = "")
		{
			RestDataSource restDataSource = (RestDataSource)DataSource;
			RestConnection connection = restDataSource.Connection;

			if (string.IsNullOrEmpty(Url))
			{
				throw new InvalidOperationException($"URL was null / empty in TryGetJsonDoc for data source {DataSource.Name}");
			}

			string? rawJson;

			try
			{
				if (!string.IsNullOrEmpty(nextPageUrl))
				{
					rawJson = Client.DownloadString(nextPageUrl);
				}
				else
				{
					if (restDataSource.Connection.Authorization == HttpAuthorization.OAuth2)
					{
						Dictionary<string, string> tokenPostDict = new();

						foreach (KeyValue keyVal in connection.TokenBody!)
						{
							tokenPostDict[keyVal.Key!] = keyVal.Value!;
						}

						string tokenPostBody = JsonSerializer.Serialize(tokenPostDict);

						string fullTokenUrl = Helper.GetUrlWithParameters(connection.TokenAbsoluteUrl!, connection.TokenParameters);

						if (connection.TokenParameters != null && connection.TokenParameters.Count > 0)
						{
							fullTokenUrl += $"&grant_type={connection.GrantType}".ToLower(CultureInfo.InvariantCulture);
						}
						else
						{
							fullTokenUrl += $"grant_type={connection.GrantType}".ToLower(CultureInfo.InvariantCulture);
						}

						fullTokenUrl += $"&client_id={connection.ClientID}";
						fullTokenUrl += $"&client_secret={connection.ClientSecret}";
						fullTokenUrl += $"&username={connection.RestUser}";
						fullTokenUrl += $"&password={connection.Password}";

						string tokenJson = Client.UploadString(fullTokenUrl, tokenPostBody);

						JsonDocument jsonDoc = JsonDocument.Parse(tokenJson);

						string? token;

						try
						{
							token = jsonDoc.RootElement.GetProperty(connection.TokenJsonIdentifier!).GetString();
						}
						catch (KeyNotFoundException e)
						{
							throw new KeyNotFoundException($"Key \"{connection.TokenJsonIdentifier!}\" was not found in JSON body: {tokenJson}", e);
						}

						string urlWithParameters = Helper.GetUrlWithParameters(Url, restDataSource.Parameters);

						Client.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {token}");

						rawJson = Client.DownloadString(urlWithParameters);
					}
					else if (restDataSource.Connection.Authorization == HttpAuthorization.Token)
					{
						Dictionary<string, string> tokenPostDict = new();

						foreach (KeyValue keyVal in connection.TokenBody!)
						{
							tokenPostDict[keyVal.Key!] = keyVal.Value!;
						}

						string tokenPostBody = JsonSerializer.Serialize(tokenPostDict);

						string fullTokenUrl = Helper.GetUrlWithParameters(connection.TokenAbsoluteUrl!, connection.TokenParameters);

						string tokenJson = Client.UploadString(fullTokenUrl, tokenPostBody);

						JsonDocument jsonDoc = JsonDocument.Parse(tokenJson);

						string? token;

						try
						{
							token = jsonDoc.RootElement.GetProperty(connection.TokenJsonIdentifier!).GetString();
						}
						catch (KeyNotFoundException e)
						{
							throw new KeyNotFoundException($"Key \"{connection.TokenJsonIdentifier!}\" was not found in JSON body: {tokenJson}", e);
						}

						string urlWithParameters = Helper.GetUrlWithParameters(Url, restDataSource.Parameters).Replace("{token}", token);

						rawJson = Client.DownloadString(urlWithParameters);
					}
					else // if (restDataSource.Connection.Authorization == HttpAuthorization.Basic || restDataSource.Connection.Authorization == HttpAuthorization.None)
					{
						rawJson = Client.DownloadString(Helper.GetUrlWithParameters(Url, restDataSource.Parameters));
					}
				}
			}
			catch (WebException)
			{
				return "";
			}

			return rawJson;
		}

		private void InitializeWebClient(RestDataSource dataSource)
		{
			RestConnection restConnection = dataSource.Connection;

			switch (dataSource.Connection.Authorization)
			{
				case HttpAuthorization.Basic:
					string credentials = $"{restConnection.RestUser}:{restConnection.Password}";
					string basicToken = $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials))}";
					Client.Headers.Add(HttpRequestHeader.Authorization, basicToken);
					break;
				case HttpAuthorization.OAuth2:
				case HttpAuthorization.Token:
					if (restConnection.TokenJsonIdentifier == null || restConnection.TokenAbsoluteUrl == null)
					{
						string error = restConnection.TokenJsonIdentifier == null ? nameof(restConnection.TokenJsonIdentifier) + " " : "";
						error += restConnection.TokenAbsoluteUrl == null ? nameof(restConnection.TokenAbsoluteUrl) + " " : "";

						throw new InvalidOperationException(error + " is null!");
					}

					Client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
					break;
				case HttpAuthorization.None:
					// TODO
					break;
			}

			Url = $"{dataSource.Connection.BaseUrl}{dataSource.RelativeUrl}";
		}

		private void InitializeWebClient(GraphQlDataSource dataSource)
		{
			GraphQlConnection graphQlConnection = dataSource.Connection;

			Url = $"{dataSource.Connection.BaseUrl}{dataSource.RelativeUrl}";

			Client.Headers.Add(HttpRequestHeader.Authorization, "token " + graphQlConnection.AuthorizationToken);
		}

		public Dictionary<string, Column> AnalyzeDataTypes()
		{
			Dictionary<string, Column> result;

			switch (DataSource)
			{
				case RestDataSource restDataSource:
					InitializeWebClient(restDataSource);
					result = AnalyzeRestDataTypes();
					break;
				case GraphQlDataSource graphQlDataSource:
					InitializeWebClient(graphQlDataSource);
					result = AnalyzeGraphQlDataTypes();
					break;
				//case JsonFileDataSource _:
				//	break;
				//case SqlDataSource _:
				//	break;
				default:
					throw new NotImplementedException($"Support for source type {typeof(DataSource)} is not implemented!");
			}

			return result;
		}

		private static List<string> GetCollectionReferenceParts(string collectionReference)
		{
			Regex regex = new(@"[^$\['\]]+");

			return regex.Matches(collectionReference).Select(match => match.Value).ToList();
		}

		private Dictionary<string, Column> AnalyzeGraphQlDataTypes(string path = "")
		{
			GraphQlDataSource graphQlDataSource = (GraphQlDataSource)DataSource;

			Dictionary<string, Column> suggestedColumnDict = new();

			List<string> collectionReferenceParts = GetCollectionReferenceParts(((GraphQlDataSource)DataSource).CollectionReference!);

			bool firstIteration = true;

			if (!string.IsNullOrEmpty(path))
			{
				IEnumerable<string> rawPages = File.ReadLines(path);

				foreach (string rawPage in rawPages)
				{
					AnalyzeJsonDocument(rawPage, collectionReferenceParts[^1], ref suggestedColumnDict, ref firstIteration, useGraphQlQuery: true, parent: graphQlDataSource.Parent ?? "");
				}
			}
			else
			{
				bool hasNextPage;

				string jsonPage = Client.UploadString(Url!, graphQlDataSource.GraphQlQuery.Replace("{first}", $"first: {graphQlDataSource.NumberToFetch}"));

				JsonDocument jsonDocument = JsonDocument.Parse(jsonPage);
				JsonElement collectionElements;

				try
				{
					collectionElements = jsonDocument.RootElement.GetProperty(collectionReferenceParts[0]);
				}
				catch (KeyNotFoundException e)
				{
					throw new KeyNotFoundException($"Key \"{collectionReferenceParts[0]}\" was not found in JSON body: {jsonPage}", e);
				}

				for (int i = 1; i < collectionReferenceParts.Count; ++i)
				{
					collectionElements = collectionElements.GetProperty(collectionReferenceParts[i]);
				}

				do
				{
					// Analyze the data
					AnalyzeJsonDocument(jsonPage, collectionReferenceParts[^1], ref suggestedColumnDict, ref firstIteration, useGraphQlQuery: true, parent: graphQlDataSource.Parent ?? "");

					// See if has next page
					try
					{
						hasNextPage = jsonDocument.RootElement
							.GetProperty(collectionReferenceParts[0])
							.GetProperty(collectionReferenceParts[1])
							.GetProperty("pageInfo")
							.GetProperty("hasNextPage")
							.GetBoolean();
					}
					catch (KeyNotFoundException e)
					{
						throw new KeyNotFoundException($"One of keys \"{collectionReferenceParts[0]}\", \"{collectionReferenceParts[1]}\", \"pageInfo\" or \"hasNextPage\" was not found in JSON body: {jsonPage}", e);
					}

					if (hasNextPage)
					{
						// Get max cursor
						string maxCursor = "";
						int maxCursorValue = int.MinValue;
						for (int i = 0; i < collectionElements.GetArrayLength(); ++i)
						{
							JsonElement collectionElement = collectionElements[i];

							string? currentCursor;

							try
							{
								currentCursor = collectionElement.GetProperty("cursor").GetString()!;
							}
							catch (KeyNotFoundException e)
							{
								throw new KeyNotFoundException($"Key \"cursor\" was not found in JSON body: {collectionElement}", e);
							}

							string[] neighbours = currentCursor.Split(".");

							int largestNeighbour;

							largestNeighbour = int.Parse(neighbours[^1], CultureInfo.InvariantCulture);

							if (largestNeighbour > maxCursorValue)
							{
								maxCursor = currentCursor;
								maxCursorValue = largestNeighbour;
							}
						}

						// Get next page
						try
						{
							jsonPage = Client.UploadString(Url!, graphQlDataSource.GraphQlQuery.Replace("{first}", $"first: {graphQlDataSource.NumberToFetch}, after: \\\"{maxCursor}\\\""));
							jsonDocument = JsonDocument.Parse(jsonPage);
							collectionElements = jsonDocument.RootElement.GetProperty(collectionReferenceParts[0]);
						}
						catch (KeyNotFoundException e)
						{
							throw new KeyNotFoundException($"Key \"{collectionReferenceParts[0]}\" was not found in JSON body: {jsonPage}", e);
						}

						for (int i = 1; i < collectionReferenceParts.Count; ++i)
						{
							collectionElements = collectionElements.GetProperty(collectionReferenceParts[i]);
						}
					}
				} while (hasNextPage);
			}

			return suggestedColumnDict;
		}

		private Dictionary<string, Column> AnalyzeRestDataTypes(string path = "")
		{
			RestDataSource restDataSource = (RestDataSource)DataSource;

			string collectionReference = restDataSource.CollectionReference!;

			bool firstIteration = true;

			if (!string.IsNullOrEmpty(path))
			{
				Dictionary<string, Column> suggestedColumnDict = new();

				IEnumerable<string> rawPages = File.ReadLines(path);

				foreach (string rawPage in rawPages)
				{
					AnalyzeJsonDocument(rawPage, collectionReference, ref suggestedColumnDict, ref firstIteration);
				}

				return suggestedColumnDict;
			}
			else
			{
				string restJson = TryGetJson();

				JsonDocument restJsonPage = JsonDocument.Parse(restJson);

				Dictionary<string, Column> suggestedColumnDict = new();

				string? currentUrl = Url;

				bool paginate = restDataSource.PaginationNextLink != null;

				while (currentUrl != null)
				{
					if (paginate)
					{
						try
						{
							string? nextUrl;

							try
							{
								nextUrl = restJsonPage.RootElement.GetProperty(restDataSource.PaginationNextLink!).GetString();
							}
							catch (KeyNotFoundException e)
							{
								throw new KeyNotFoundException($"Key \"{restDataSource.PaginationNextLink!}\" was not found in JSON body: {restJson}", e);
							}

							if (restDataSource.PaginationLinkIsRelative)
							{
								nextUrl = restDataSource.Connection.BaseUrl + nextUrl;
							}

							if (nextUrl != currentUrl)
							{
								currentUrl = nextUrl;
							}
							else
							{
								currentUrl = null;
							}
						}
						catch (KeyNotFoundException)
						{
							currentUrl = null;
						}
					}
					else
					{
						currentUrl = null;
					}

					AnalyzeJsonDocument(restJson, collectionReference, ref suggestedColumnDict, ref firstIteration);

					if (currentUrl != null)
					{
						restJson = TryGetJson(currentUrl);
						restJsonPage = JsonDocument.Parse(restJson);
					}
				}

				restJsonPage.Dispose();

				return suggestedColumnDict;
			}
		}

		private static string RemoveAllButCollectionJson(string json, string collectionReference)
		{
			string[] splitAtRef = json.Split($"\"{collectionReference}\"");

			int lastArrayEndIndex = splitAtRef[1].LastIndexOf("}]", StringComparison.Ordinal);

			string arrayContents = splitAtRef[1].Substring(0, lastArrayEndIndex);

			return $"{{\"{collectionReference}\"{arrayContents}}}]}}";
		}

		private static List<KeyValuePair<string, JsonElement>> GetObjectLeaves(string? path, JsonProperty jsonProperty)
		{
			path = (path == null) ? jsonProperty.Name : path + "." + jsonProperty.Name;

			List<KeyValuePair<string, JsonElement>> pairList = new();

			if (jsonProperty.Value.ValueKind is not JsonValueKind.Object and not JsonValueKind.Array)
			{
				pairList.Add(new KeyValuePair<string, JsonElement>(path, jsonProperty.Value));
			}
			else if (jsonProperty.Value.ValueKind == JsonValueKind.Object)
			{
				foreach (JsonProperty child in jsonProperty.Value.EnumerateObject())
				{
					pairList.AddRange(GetObjectLeaves(path, child));
				}
			}
			else // if (jsonProperty.Value.ValueKind == JsonValueKind.Array) 
			{
				foreach (JsonElement child in jsonProperty.Value.EnumerateArray())
				{
					pairList.AddRange(GetArrayLeaves(path, child));
				}
			}

			return pairList;
		}

		private static List<KeyValuePair<string, JsonElement>> GetArrayLeaves(string? path, JsonElement jsonElement)
		{
			List<KeyValuePair<string, JsonElement>> pairList = new();

			if (jsonElement.ValueKind == JsonValueKind.Object)
			{
				int numNonArrayChildren = 0;

				foreach (JsonProperty child in jsonElement.EnumerateObject().Where(jsonProp => jsonProp.Value.ValueKind != JsonValueKind.Array))
				{
					pairList.AddRange(GetObjectLeaves(path, child));
					numNonArrayChildren++;
				}

				foreach (JsonProperty child in jsonElement.EnumerateObject().Where(jsonProp => jsonProp.Value.ValueKind == JsonValueKind.Array))
				{
					List<KeyValuePair<string, JsonElement>> innerPairList = GetObjectLeaves(path, child);

					// Firstly, reform innerPairList to lists with each array object at each index
					List<string> distinctColumnNames = innerPairList.Select(kvp => kvp.Key).Distinct().ToList();

					List<List<KeyValuePair<string, JsonElement>>> collectedPairList = new();

					// Create a new list for every array object
					// Based on the idea that first distinct column name should appear as many times as there are array objects
					for (int i = 0; i < innerPairList.Count(innerPair => innerPair.Key == distinctColumnNames[0]); i++)
					{
						collectedPairList.Add(new List<KeyValuePair<string, JsonElement>>());
					}

					int currentListIndex = 0;

					for (int i = 0; i < innerPairList.Count; i++)
					{
						if (i % distinctColumnNames.Count == 0 && i > 0)
						{
							currentListIndex++;
						}

						KeyValuePair<string, JsonElement> innerPair = innerPairList[i];

						collectedPairList[currentListIndex].Add(innerPair);
					}

					// Secondly, for each such list, create one new row
					List<KeyValuePair<string, JsonElement>> result = collectedPairList.Select(collectedInnerPair => pairList.Concat(collectedInnerPair).ToList()).SelectMany(x => x).ToList();

					pairList.AddRange(result.Skip(numNonArrayChildren));
				}
			}
			else if (jsonElement.ValueKind == JsonValueKind.Array)
			{
				foreach (JsonElement child in jsonElement.EnumerateArray())
				{
					pairList.AddRange(GetArrayLeaves(path, child));
				}
			}
			else // Array element
			{
				path += ".";

				pairList.Add(new KeyValuePair<string, JsonElement>(path!, jsonElement));
			}

			return pairList;
		}

		private static List<Dictionary<string, JsonElement>> GetFlatCollection(string json)
		{
			JsonDocument jsonDoc = JsonDocument.Parse(json);

			JsonElement root = jsonDoc.RootElement;

			List<Dictionary<string, JsonElement>> dictList = new();

			if (root.ValueKind == JsonValueKind.Object)
			{
				foreach (JsonProperty jsonProperty in root.EnumerateObject())
				{
					Dictionary<string, JsonElement> dict = new();

					List<KeyValuePair<string, JsonElement>> pairList = GetObjectLeaves(path: null, jsonProperty);

					foreach (KeyValuePair<string, JsonElement> pair in pairList)
					{
						if (!dict.ContainsKey(pair.Key))
						{
							dict[pair.Key] = pair.Value;
						}
					}

					dictList.Add(dict);
				}
			}
			else if (root.ValueKind == JsonValueKind.Array)
			{
				foreach (JsonElement jsonElement in root.EnumerateArray())
				{
					List<KeyValuePair<string, JsonElement>> pairList = GetArrayLeaves(path: null, jsonElement);

					List<string> distinctColumnNames = pairList.Select(kvp => kvp.Key).Distinct().ToList();

					// create new dict to dictList for each i % distinctColumnCount == 0.
					Dictionary<string, JsonElement> dict = new();

					for (int i = 0; i < pairList.Count; i++)
					{
						if (i == pairList.Count - 1)
						{
							dictList.Add(dict);
						}

						if (i % distinctColumnNames.Count == 0 && i > 0)
						{
							dictList.Add(dict);

							dict = new Dictionary<string, JsonElement>();
						}

						KeyValuePair<string, JsonElement> pair = pairList[i];

						if (!dict.ContainsKey(pair.Key))
						{
							dict[pair.Key] = pair.Value;
						}
					}
				}
			}

			return dictList;
		}

		private static Dictionary<string, JsonElement> GetFlatSingle(string json)
		{
			JsonDocument jsonDoc = JsonDocument.Parse(json);

			JsonElement root = jsonDoc.RootElement;

			Dictionary<string, JsonElement> dict = new();

			if (root.ValueKind == JsonValueKind.Object)
			{
				foreach (JsonProperty jsonProperty in root.EnumerateObject())
				{
					List<KeyValuePair<string, JsonElement>> pairList = GetObjectLeaves(path: null, jsonProperty);

					foreach (KeyValuePair<string, JsonElement> pair in pairList)
					{
						if (!dict.ContainsKey(pair.Key))
						{
							dict[pair.Key] = pair.Value;
						}
					}
				}
			}
			else if (root.ValueKind == JsonValueKind.Array)
			{
				foreach (JsonElement jsonElement in root.EnumerateArray())
				{
					List<KeyValuePair<string, JsonElement>> pairList = GetArrayLeaves(path: null, jsonElement);

					foreach (KeyValuePair<string, JsonElement> pair in pairList)
					{
						if (!dict.ContainsKey(pair.Key))
						{
							dict[pair.Key] = pair.Value;
						}
					}
				}
			}

			return dict;
		}

		private void AnalyzeJsonDocument(string json, string collectionReference, ref Dictionary<string, Column> suggestedColumnDict, ref bool firstIteration, bool useGraphQlQuery = false, string parent = "")
		{
			JsonElement collectionElements;

			JsonDocument jsonDoc;

			if (collectionReference != null)
			{
				jsonDoc = JsonDocument.Parse(RemoveAllButCollectionJson(json, collectionReference));

				try
				{
					collectionElements = jsonDoc!.RootElement.GetProperty(collectionReference);
				}
				catch (KeyNotFoundException e)
				{
					throw new ArgumentException($"Couldn't find collection reference {collectionReference} in {DataSource.Name}.", e);
				}
			}
			else
			{
				jsonDoc = JsonDocument.Parse(json);

				collectionElements = jsonDoc.RootElement;
			}

			if (firstIteration && !useGraphQlQuery)
			{
				string rawCollectionJson = collectionElements.ToString()!;

				List<string>? suggestedColumnNames = GetFlatCollection(rawCollectionJson).OrderByDescending(x => x.Count).First().Select(x => x.Key).ToList();

				foreach (string suggestedName in suggestedColumnNames)
				{
					if (!ColumnTypeDecided.ContainsKey(suggestedName))
					{
						ColumnTypeDecided.Add(suggestedName, false);
					}
				}

				suggestedColumnNames = null;

				firstIteration = false;
			}
			else if (firstIteration && useGraphQlQuery)
			{
				string quotedQuery = ((GraphQlDataSource)DataSource).GraphQlQuery.Split("query\":")[1];
				string query = quotedQuery.Substring(quotedQuery.IndexOf("\"", StringComparison.Ordinal) + 1, quotedQuery.LastIndexOf("\"", StringComparison.Ordinal) - quotedQuery.IndexOf("\"", StringComparison.Ordinal) - 1);

				string[] queryParts = query.Split(((GraphQlDataSource)DataSource).Parent)[1].Split(" ");

				List<string> suggestedNames = new();

				List<string> prefixes = new();

				string potentialPrefix = "";

				foreach (string queryPart in queryParts)
				{
#pragma warning disable CA1508 // Avoid dead conditional code. Something is wonky with the analyzer, it doesn't detect that we're in a loop.
					if (queryPart == "{" && !string.IsNullOrEmpty(potentialPrefix))
#pragma warning restore CA1508 // Avoid dead conditional code
					{
						prefixes.Add(potentialPrefix);
					}
					else if (queryPart == "}" && prefixes.Count > 0)
					{
						prefixes.RemoveAt(prefixes.Count - 1);
					}
					else if (queryPart != "{" && queryPart != "}" && !string.IsNullOrEmpty(queryPart))
					{
						string suggestedName = "";

						if (prefixes.Count > 0)
						{
							foreach (string prefix in prefixes)
							{
								suggestedName += prefix + ".";
							}

							if (suggestedNames[^1] == prefixes.Aggregate((prefix, otherPrefix) => prefix + "." + otherPrefix))
							{
								suggestedNames.RemoveAt(suggestedNames.Count - 1);
							}
						}

						suggestedName += queryPart;

						if (suggestedName != "pageInfo.hasNextPage")
						{
							suggestedNames.Add(suggestedName);
						}

						potentialPrefix = queryPart;
					}
				}

				foreach (string suggestedName in suggestedNames)
				{
					if (!ColumnTypeDecided.ContainsKey(suggestedName))
					{
						ColumnTypeDecided.Add(suggestedName, false);
					}
				}

				firstIteration = false;
			}

			// Dictionary mapping column name to column object with strictest possible definition

			List<JsonElement> enumerator;

			if (collectionElements.ValueKind == JsonValueKind.Array)
			{
				enumerator = collectionElements.EnumerateArray().ToList();
			}
			else
			{
				enumerator = new List<JsonElement>() { collectionElements };
			}

			foreach (JsonElement collectionElement in enumerator)
			{
				JsonElement flattenedElement;

				if (!string.IsNullOrEmpty(parent))
				{
					try
					{
						flattenedElement = JsonDocument.Parse(JsonSerializer.Serialize(GetFlatSingle(collectionElement.GetProperty(parent).ToString()!))).RootElement;
					}
					catch (KeyNotFoundException e)
					{
						throw new KeyNotFoundException($"Key \"{parent}\" was not found in JSON body: {collectionElement}", e);
					}
				}
				else
				{
					flattenedElement = JsonDocument.Parse(JsonSerializer.Serialize(GetFlatSingle(collectionElement.ToString()!))).RootElement;
				}

				foreach (string suggestedColumnName in ColumnTypeDecided.Keys.ToList())
				{
					Column suggestedColumn;

					// If name not present in suggestedColumns, create new column object
					if (!suggestedColumnDict.ContainsKey(suggestedColumnName))
					{
						suggestedColumn = new Column(name: suggestedColumnName)
						{
							Nullable = false
						};
					}
					else
					{
						suggestedColumn = suggestedColumnDict[suggestedColumnName];

						if (suggestedColumn == null)
						{
							throw new InvalidOperationException($"Suggested column name {suggestedColumnName} in rest source for {DataSource.Name} was not found! This should really never happen.");
						}
					}

					AnalyzeJsonColumn(suggestedColumnName, flattenedElement, ref suggestedColumn);

					suggestedColumnDict[suggestedColumnName] = suggestedColumn;
				}
			}
		}

		private void AnalyzeJsonColumn(string suggestedColumnName, JsonElement collectionElement, ref Column suggestedColumn)
		{
			string possibleArrayName = "";

			int lastIndexOfSeparator = suggestedColumnName.LastIndexOf('.');

			if (lastIndexOfSeparator != -1)
			{
				possibleArrayName = suggestedColumnName.Substring(0, lastIndexOfSeparator);
			}

			// Check and update datatype if current row demands it
			if (collectionElement.ValueKind != JsonValueKind.Array && collectionElement.TryGetProperty(suggestedColumnName, out JsonElement valueElement))
			{
				switch (valueElement.ValueKind)
				{
					case JsonValueKind.True:
					case JsonValueKind.False:
						suggestedColumn.DataType = SqlServerDataType.Bit;
						break;
					case JsonValueKind.Number:
						AnalyzeJsonNumberDataType(valueElement, ref suggestedColumn);
						break;
					case JsonValueKind.Null:
						if (!ColumnTypeDecided[suggestedColumn.Name!])
						{
							suggestedColumn.DataType = SqlServerDataType.Bit;
							suggestedColumn.Nullable = true;
						}
						else
						{
							suggestedColumn.Nullable = true;
						}
						break;
					case JsonValueKind.String:
						AnalyzeJsonStringDataType(valueElement, ref suggestedColumn);
						break;
					default:
						throw new ArgumentException($"Cannot match column {suggestedColumnName} in {DataSource.Name} against any supported data type.");
				}
			}
			else if (collectionElement.ValueKind == JsonValueKind.Array)
			{
				// Reaching this code block means that we need to loop through a JSON array to determine column types, recursively
				if (string.IsNullOrEmpty(possibleArrayName))
				{
					foreach (JsonElement arrayMember in collectionElement.EnumerateArray())
					{
						AnalyzeJsonColumn(suggestedColumnName, arrayMember, ref suggestedColumn);
					}
				}
				else if (collectionElement.TryGetProperty(possibleArrayName, out JsonElement arrayElement) && arrayElement.ValueKind != JsonValueKind.Null)
				{
					for (int i = 0; i < arrayElement.GetArrayLength(); i++)
					{
						JsonElement arrayMember = arrayElement[i];

						string arrayColumnName = suggestedColumnName.Substring(lastIndexOfSeparator + 1);

						AnalyzeJsonColumn(arrayColumnName, arrayMember, ref suggestedColumn);
					}
				}
			}
			else
			{
				// If not present it's probably because JSON object was suddenly null i.e. all of its member columns should be nullable.
				if (!ColumnTypeDecided[suggestedColumn.Name!])
				{
					suggestedColumn.DataType = SqlServerDataType.Bit;
				}

				suggestedColumn.Nullable = true;
			}
		}

		private bool AnalyzeJsonNumberDataType(JsonElement valueJson, ref Column suggestedColumn)
		{
			string valueElement = valueJson.ToString()!;

			if (Byte.TryParse(valueElement, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
			{
				if (!ColumnTypeDecided[suggestedColumn.Name!] || (suggestedColumn.DataType != SqlServerDataType.SmallInt && suggestedColumn.DataType != SqlServerDataType.Int && suggestedColumn.DataType != SqlServerDataType.BigInt && suggestedColumn.DataType != SqlServerDataType.Decimal && suggestedColumn.DataType != SqlServerDataType.Float))
				{
					suggestedColumn.DataType = SqlServerDataType.TinyInt;
				}
			}
			else if (Int16.TryParse(valueElement, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
			{
				if (!ColumnTypeDecided[suggestedColumn.Name!] || (suggestedColumn.DataType != SqlServerDataType.Int && suggestedColumn.DataType != SqlServerDataType.BigInt && suggestedColumn.DataType != SqlServerDataType.Decimal && suggestedColumn.DataType != SqlServerDataType.Float))
				{
					suggestedColumn.DataType = SqlServerDataType.SmallInt;
				}
			}
			else if (Int32.TryParse(valueElement, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
			{
				if (!ColumnTypeDecided[suggestedColumn.Name!] || (suggestedColumn.DataType != SqlServerDataType.BigInt && suggestedColumn.DataType != SqlServerDataType.Decimal && suggestedColumn.DataType != SqlServerDataType.Float))
				{
					suggestedColumn.DataType = SqlServerDataType.Int;
				}
			}
			else if (Int64.TryParse(valueElement, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
			{
				if (!ColumnTypeDecided[suggestedColumn.Name!] || (suggestedColumn.DataType != SqlServerDataType.Decimal && suggestedColumn.DataType != SqlServerDataType.Float))
				{
					suggestedColumn.DataType = SqlServerDataType.BigInt;
				}
			}
			else if (Decimal.TryParse(valueElement, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal suggestedDecimal))
			{
				if (!ColumnTypeDecided[suggestedColumn.Name!] || suggestedColumn.DataType != SqlServerDataType.Float)
				{
					suggestedColumn.DataType = SqlServerDataType.Decimal;

					string value = suggestedDecimal.ToString(CultureInfo.InvariantCulture);

					int suggestedScale = 0;

					int suggestedPrecision;

					if (value.Contains("."))
					{
						string[] decimalParts = value.Split(".");

						suggestedScale = decimalParts[1].Length;
						suggestedPrecision = decimalParts[0].Length + suggestedScale;
					}
					else
					{
						suggestedPrecision = value.Length;
					}

					if (suggestedColumn.Precision == null || suggestedColumn.Precision < suggestedPrecision || suggestedColumn.Scale < suggestedScale)
					{
						if (suggestedPrecision > PRECISION_BOUNDS[^1])
						{
							throw new InvalidCastException($"{suggestedColumn.Name} looks like a decimal, but does not fit into a {SqlServerDataType.Decimal}!" +
								$"\nSuggested precision: {suggestedPrecision}" +
								$"\nMax precision for {SqlServerDataType.Decimal}: {PRECISION_BOUNDS[^1]}"
							);
						}

						foreach (int bound in PRECISION_BOUNDS)
						{
							if (suggestedPrecision < bound)
							{
								suggestedPrecision = bound;
								break;
							}
						}

						suggestedColumn.Scale = suggestedScale;
						suggestedColumn.Precision = suggestedPrecision;
					}
				}
			}
			else if (Double.TryParse(valueElement, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
			{
				suggestedColumn.DataType = SqlServerDataType.Float;
			}
			else
			{
				return false;
			}

			ColumnTypeDecided[suggestedColumn.Name!] = true;

			return true;
		}

		private void AnalyzeJsonStringDataType(JsonElement valueJson, ref Column suggestedColumn)
		{
			string valueElement = valueJson.ToString()!;

			if (DateTime.TryParse(valueElement, out DateTime date))
			{
				if (date.Year == 1900 && date.Month == 1 && date.Day == 1 && valueElement.Substring(0, 4) != "1900" && suggestedColumn.DataType != SqlServerDataType.Date && suggestedColumn.DataType != SqlServerDataType.DateTimeOffset && suggestedColumn.DataType != SqlServerDataType.DateTime2 && suggestedColumn.DataType != SqlServerDataType.UniqueIdentifier && (suggestedColumn.DataType != SqlServerDataType.VarChar || !ColumnTypeDecided[suggestedColumn.Name!]))
				{
					try
					{
						suggestedColumn.DataType = SqlServerDataType.Time;

						string timeDecimals = "";

						if (valueElement.Contains("."))
						{
							timeDecimals = valueElement.Split(".")[1];
						}

						int suggestedScale = timeDecimals.Length;

						string scaleString = suggestedScale > 0 ? "." + string.Concat(Enumerable.Repeat("f", suggestedScale)) : "";

						_ = date.ToString($"HH:mm:ss{scaleString}", CultureInfo.InvariantCulture);

						if (suggestedColumn.Scale == null || suggestedColumn.Scale < suggestedScale)
						{
							suggestedColumn.Scale = suggestedScale;
						}

						ColumnTypeDecided[suggestedColumn.Name!] = true;
					}
					catch (FormatException)
					{
						// Do nothing
					}
				}
				else if (date.Hour == 0 && date.Minute == 0 && date.Second == 0 && date.Millisecond == 0 && suggestedColumn.DataType != SqlServerDataType.DateTimeOffset && suggestedColumn.DataType != SqlServerDataType.DateTime2 && suggestedColumn.DataType != SqlServerDataType.UniqueIdentifier && (suggestedColumn.DataType != SqlServerDataType.VarChar || !ColumnTypeDecided[suggestedColumn.Name!]))
				{
					try
					{
						_ = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

						suggestedColumn.DataType = SqlServerDataType.Date;

						ColumnTypeDecided[suggestedColumn.Name!] = true;
					}
					catch (FormatException)
					{
						// Do nothing
					}
				}
				else if ((valueElement.ToString()!.Contains("Z") || valueElement.ToString()!.Contains("+") || valueElement.ToString()!.Contains(" -")) && suggestedColumn.DataType != SqlServerDataType.DateTime2 && suggestedColumn.DataType != SqlServerDataType.UniqueIdentifier && (suggestedColumn.DataType != SqlServerDataType.VarChar || !ColumnTypeDecided[suggestedColumn.Name!]))
				{
					suggestedColumn.DataType = SqlServerDataType.DateTimeOffset;

					ColumnTypeDecided[suggestedColumn.Name!] = true;
				}
				else if (suggestedColumn.DataType != SqlServerDataType.UniqueIdentifier && suggestedColumn.DataType != SqlServerDataType.VarChar && !ColumnTypeDecided[suggestedColumn.Name!])
				{
					suggestedColumn.DataType = SqlServerDataType.DateTime2;

					string dateTimeDecimals = "";

					if (valueElement.Contains("."))
					{
						dateTimeDecimals = valueElement.Split(".")[1];
					}

					int suggestedScale = dateTimeDecimals.Length;

					if (suggestedColumn.Scale == null || suggestedColumn.Scale < suggestedScale)
					{
						suggestedColumn.Scale = suggestedScale;
					}

					ColumnTypeDecided[suggestedColumn.Name!] = true;
				}
			}
			else if (Guid.TryParse(valueElement, out Guid _) && (suggestedColumn.DataType != SqlServerDataType.VarChar || !ColumnTypeDecided[suggestedColumn.Name!]))
			{
				suggestedColumn.DataType = SqlServerDataType.UniqueIdentifier;

				ColumnTypeDecided[suggestedColumn.Name!] = true;
			}
			else
			{
				string? value = valueElement.ToString();

				suggestedColumn.DataType = SqlServerDataType.VarChar;

				if (string.IsNullOrEmpty(value))
				{
					suggestedColumn.Nullable = true;
				}

				if (suggestedColumn.Length == null || (suggestedColumn.Length < value?.Length && suggestedColumn.DataType == SqlServerDataType.VarChar))
				{
					if (value?.Length == 0)
					{
						suggestedColumn.Length = UNKNOWN_VARCHAR_DEFAULT_LENGTH;
					}
					else
					{
						suggestedColumn.Length = value?.Length;
					}
				}

				ColumnTypeDecided[suggestedColumn.Name!] = true;
			}
		}
	}
}
