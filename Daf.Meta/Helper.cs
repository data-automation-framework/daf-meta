// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;
using System.Globalization;
using Daf.Meta.Layers.Connections;

namespace Daf.Meta
{
	public static class Helper
	{
		public static string GetFormattedText(IEnumerable<string> lines, int numTabs)
		{
			if (lines == null)
				throw new ArgumentNullException($"Parameter '{nameof(lines)}' cannot be null!");

			return string.Join($",{Environment.NewLine}{new string('\t', numTabs)}", lines);
		}

		public static string GetUrlWithParameters(string url, List<KeyValue>? parameters)
		{
			string result = url;

			if (parameters != null)
			{
				result += "?";

				for (int i = 0; i < parameters.Count; i++)
				{
					KeyValue keyValue = parameters[i];

					if (keyValue.Value?.ToLower(CultureInfo.InvariantCulture) != "{incremental}")
					{
						result += $"{keyValue.Key}={keyValue.Value}{(i < parameters.Count - 1 ? "&" : "")}";
					}
				}
			}

			return result;
		}

		public static string GetBody(List<KeyValue>? bodyKeyValues)
		{
			string result = "";

			if (bodyKeyValues != null)
			{
				result += "{";

				for (int i = 0; i < bodyKeyValues.Count; i++)
				{
					KeyValue keyValue = bodyKeyValues[i];

					result += $"\t\"{keyValue.Key}\": \"{keyValue.Value}\"{(i != bodyKeyValues.Count - 1 ? "," : "")}\n";
				}

				result += "}";
			}

			return result;
		}

		public static string GetTrimSql(string columnSQL)
		{
			/**
			 * Previously here we benchmarked TRIM(@test FROM columnSQL) where @test was defined as:
			 *
			 * DECLARE @test NVARCHAR(100);
			 *
			 * SET @test = CHAR(9) + -- Character tabulation
			 * CHAR(10) + -- Line feed
			 * CHAR(11) + -- Line tabulation
			 * CHAR(12) + -- Form feed
			 * CHAR(13) + -- Carriage return
			 * CHAR(32) + -- Space
			 * CHAR(133) + -- Next line
			 * CHAR(160); -- No-break space
			 *
			 * The benchmark was carried out by running:
			 * ---------------------------------------------
			 * SET QUERY_GOVERNOR_COST_LIMIT 0;
			 *
			 * DROP TABLE IF EXISTS #tmp
			 *
			 * DECLARE @chars NVARCHAR(10) = CHAR(9) + CHAR(10) + CHAR(11) + CHAR(12) + CHAR(13) + CHAR(32) + CHAR(133) + CHAR(160);
			 *
			 * SELECT	TRIM(@chars FROM ssod.SalesOrderID) AS SalesOrderID,
			 *		TRIM(@chars FROM ssod.SalesOrderDetailID) AS SalesOrderDetailID,
			 *		TRIM(@chars FROM ssod.LocalCurrency) AS LocalCurrency,
			 *		TRIM(@chars FROM ssod.ProductID) AS ProductID,
			 *		TRIM(@chars FROM ssod.SpecialOfferID) AS SpecialOfferID,
			 *		TRIM(@chars FROM ssod.UnitPrice) AS UnitPrice,
			 *		TRIM(@chars FROM ssod.OrderQty) AS OrderQty,
			 *		TRIM(@chars FROM ssod.UnitPriceDiscount) AS UnitPriceDiscount,
			 *		TRIM(@chars FROM ssod.CarrierTrackingNumber) AS CarrierTrackingNumber,
			 *		TRIM(@chars FROM ssod.DueDate) AS DueDate,
			 *		TRIM(@chars FROM ssod.ShipDate) AS ShipDate
			 * INTO #tmp
			 * FROM Stg_SalesOrderDetail ssod
			 * ---------------------------------------------
			 * Four times on Stg_SalesOrderDetail with ~10 million rows all of type NVARCHAR(150).
			 * Avg CPU time = 161512 ms, Avg elapsed time = 40939 ms.
			 *
			 * The same test was carried out with LTRIM RTRIM on the same columns.
			 * Avg CPU time = 18637 ms, Avg elapsed time = 6426 ms.
			 *
			 * The same test was carried out with TRIM, without the @chars parameter i.e. TRIM(col) instead of TRIM(@chars FROM col).
			 * Avg CPU time = 15511 ms, Avg elapsed time = 5247 ms.
			 */
			return Model.Instance.DataWarehouse.TargetPlatformVersion switch
			{
				"2017" => "TRIM(" + columnSQL + ")",
				_ => "LTRIM(RTRIM(" + columnSQL + "))",
			};
		}
	}
}
