// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;

namespace Daf.Meta.Layers
{
	public enum BusinessKeyEventType
	{
		Add,
		Remove
	}

	public class BusinessKeyEventArgs : EventArgs
	{
		public StagingColumn? BusinessKey { get; set; } // Do I want this to be nullable?
		public BusinessKeyEventType Action { get; set; }
	}
}
