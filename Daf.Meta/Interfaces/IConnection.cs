// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

namespace Daf.Meta.Interfaces
{
	public interface IConnection
	{
		public uint ConnectionRetryAttempts { get; set; }
		public uint ConnectionRetryMinutes { get; set; }
	}
}
