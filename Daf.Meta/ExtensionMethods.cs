// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;

namespace Daf.Meta
{
	public static class ExtensionMethods
	{
		public static void AddSorted<T>(this IList<T> list, T item, IComparer<T>? comparer = null)
		{
			if (list == null)
				throw new ArgumentNullException($"Can't sort a {nameof(IList<T>)} that is null.");

			if (comparer == null)
				comparer = Comparer<T>.Default;

			int i = 0;

			while (i < list.Count && comparer.Compare(list[i], item) < 0)
				i++;

			list.Insert(i, item);
		}
	}
}
