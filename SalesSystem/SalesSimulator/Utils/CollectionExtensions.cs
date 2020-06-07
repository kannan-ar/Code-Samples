namespace SalesSimulator.Utils
{
    using System.Collections.Generic;
    using System.Linq;
    public static class CollectionExtensions
    {
        public static bool ContainsAny(this IEnumerable<string> collection, IEnumerable<string> searchCollection)
        {
            foreach (var item in searchCollection)
            {
                if (collection.Contains(item))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
