namespace ACE.Shared.Helpers;

public static class RandomExtensions
{
    static Random random = new Random();

    // Return a random item from an array.
    public static T Random<T>(this T[] items)
    {
        // Return a random item.
        return items[random.Next(0, items.Length)];
    }

    public static bool TryGetRandom<T>(this T[] array, out T value)
    {
        value = default;

        if (array == null || array.Length == 0)
            return false;

        value = array[random.Next(array.Length)];
        return true;
    }

    public static T Random<T>(this List<T> collection)
    {
        if (collection == null || collection.Count == 0)
        {
            throw new ArgumentException("Collection cannot be null or empty");
        }

        int randomIndex = random.Next(collection.Count);
        return collection[randomIndex];
    }

    public static T GetRandom<T>(this IEnumerable<T> list) => list.GetRandomElements<T>(1).FirstOrDefault();
    public static List<T> GetRandomElements<T>(this IEnumerable<T> list, int elementsCount) =>
        list.OrderBy(arg => Guid.NewGuid()).Take(elementsCount).ToList();

    //public static int RollRemainder(this float amount) {
    //    var truncated = (int)amount;
    //    var remainder = amount - truncated;

    //    //Roll a chance for the remainder of a decimal
    //    if(random.NextSingle() )

    //    return truncated + ((random.NextSingle() < remainder) ? 1 : 0);

    //    return 0;            
    //        }
}