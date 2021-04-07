// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Daf.Meta.Layers;

namespace Daf.Meta.JsonConverters
{
	public class HubConverter : JsonConverter<Hub>
	{
		public override Hub Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			throw new NotImplementedException("Not implemented!");
		}

		public override void Write(Utf8JsonWriter writer, Hub value, JsonSerializerOptions options)
		{
			if (writer == null || value == null)
				throw new ArgumentNullException($"Can't serialize the value if {nameof(Utf8JsonWriter)} or {nameof(Hub)} is null.");

			writer.WriteStringValue(value.Name);
		}
	}
}
