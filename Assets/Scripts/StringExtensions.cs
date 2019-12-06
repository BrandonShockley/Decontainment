namespace Extensions
{
    public static class StringExtensions
    {
        public static int IndexOfToEnd(this string str, char value, int startIndex)
        {
            int i = str.IndexOf(value, startIndex);
            return i == -1 ? str.Length : i;
        }
        public static int IndexOfAnyToEnd(this string str, char[] anyOf, int startIndex)
        {
            int i = str.IndexOfAny(anyOf, startIndex);
            return i == -1 ? str.Length : i;
        }
    }
}