namespace Raw.Streaming.Common.Extensions;

public static class IEnumerableExtensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> values)
    {
        return values is null || !values.Any();
    }
}
