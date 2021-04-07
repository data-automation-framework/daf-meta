// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Daf.Meta
{
	// Output Json in a normalized, sorted order.
	// Written by Mike S
	// Grabbed from: https://stackoverflow.com/a/63625361
	internal static class JsonNormalizer
	{
		public static string Normalize(string jsonStr)
		{
			using (JsonDocument doc = JsonDocument.Parse(jsonStr)) // Free up array pool rent.
			{
				return Normalize(doc.RootElement);
			}
		}

		public static string Normalize(JsonElement je)
		{
			var ms = new MemoryStream();

			JsonWriterOptions opts = new()
			{
				Indented = true
			};

			using (var writer = new Utf8JsonWriter(ms, opts))
			{
				Write(je, writer);
			}

			byte[] bytes = ms.ToArray();
			string str = Encoding.UTF8.GetString(bytes);

			return str;
		}

		private static void Write(JsonElement je, Utf8JsonWriter writer)
		{
			switch (je.ValueKind)
			{
				case JsonValueKind.Object:
					writer.WriteStartObject();

					// !!! This is where we can order the properties. 
					foreach (JsonProperty x in je.EnumerateObject().OrderBy(prop => prop.Name))
					{
						writer.WritePropertyName(x.Name);
						Write(x.Value, writer);
					}

					writer.WriteEndObject();
					break;
				// When normalizing... Original msapp arrays can be in any order...
				case JsonValueKind.Array:
					writer.WriteStartArray();
					foreach (JsonElement x in je.EnumerateArray())
					{
						Write(x, writer);
					}
					writer.WriteEndArray();
					break;
				case JsonValueKind.Number:
					writer.WriteNumberValue(je.GetDouble());
					break;
				case JsonValueKind.String:
					// Escape the string 
					writer.WriteStringValue(je.GetString());
					break;
				case JsonValueKind.Null:
					writer.WriteNullValue();
					break;
				case JsonValueKind.True:
					writer.WriteBooleanValue(true);
					break;
				case JsonValueKind.False:
					writer.WriteBooleanValue(false);
					break;
				default:
					throw new NotImplementedException($"Kind: {je.ValueKind}");
			}
		}
	}
}
