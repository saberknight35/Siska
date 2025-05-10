namespace Siska.Admin.Utility
{
    public static class CollectionUtilities
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable != null)
            {
                return !enumerable.Any();
            }

            return true;
        }
    }
}
