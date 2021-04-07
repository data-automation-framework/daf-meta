// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Daf.Meta.Layers;

namespace Daf.Meta.JsonConverters
{
	public class ConnectionConverter : JsonConverter<Connection>
	{
		public override Connection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			throw new NotImplementedException("Not implemented!");
		}

		public override void Write(Utf8JsonWriter writer, Connection value, JsonSerializerOptions options)
		{
			if (writer == null || value == null)
				throw new ArgumentNullException($"Can't serialize the value if {nameof(Utf8JsonWriter)} or {nameof(Connection)} is null.");

			writer.WriteStringValue(value.Name);
		}
	}
}
