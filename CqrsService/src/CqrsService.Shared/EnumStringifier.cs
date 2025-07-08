namespace CqrsService.Shared;

public static class EnumStringifier
{
    /// <summary>
    /// Convert an enum to a string value
    /// </summary>
    /// <typeparam name="T">Type T to stringify</typeparam>
    /// <param name="item">Enum whose string we require</param>
    /// <returns></returns>
    public static string Stringify<T>(T item) where T : struct, System.Enum
    {
        return ((T)item).ToString();
    }

    /// <summary>
    /// Convert a string to the relevant value of the supplied Enum type
    /// </summary>
    /// <typeparam name="T">Enum of type T</typeparam>
    /// <param name="enumString">string to convert</param>
    /// <returns>Success bool and Enum of supplied type T (or default in the case of failure)</returns>
    public static (bool, T) ToEnum<T>(string enumString) where T : struct, System.Enum
    {
        if (Enum.TryParse(enumString, true, out T test))
        {
            var output = (T)Enum.Parse(typeof(T), enumString, true);
            return (true, output);
        }
        return (false, default(T));
    }
}