namespace InfiniteVoid.SpamFramework.Core.Utils
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhitespace(this string str) => string.IsNullOrWhiteSpace(str);

        public static bool IsNotNullOrWhitespace(this string str) => !IsNullOrWhitespace(str);
    }
}