using System;
using System.Collections.Generic;
using System.Text;

namespace Mod.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Return a randomly selected item from the list.
        /// </summary>
        /// <param name="list">The list to select from.</param>
        /// <returns>A randomly selected item from the list or throws if empty/null.</returns>
        public static T GetRandom<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new InvalidOperationException("Cannot select random element from a null or empty list.");
            }

            return list[UnityEngine.Random.Range(0, maxExclusive: list.Count)];
        }
    }
}
