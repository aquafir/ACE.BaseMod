namespace Ironman;

public static class RandomHelper
{
    static Random random = new Random();

    // Return a random item from an array.
    public static T Random<T>(this T[] items)
    {
        // Return a random item.
        return items[random.Next(0, items.Length)];
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
}