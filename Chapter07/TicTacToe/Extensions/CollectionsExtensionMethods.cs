using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.Extensions
{
    public static class CollectionsExtensionMethods
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> genericEnumerable)
        {
            return (genericEnumerable == null) || (!genericEnumerable.Any());
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> genericCollection)
        {
            if (genericCollection == null)
            {
                return true;
            }
            return genericCollection.Count < 1;
        }
    }
}
