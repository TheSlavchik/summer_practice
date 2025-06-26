namespace task01
{
    public static class StringExtensions
    {
        public static bool IsPalindrome(this string input)
        {
            input = input.ToLower();

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            string newString = string.Empty;

            newString = new string(input.Where(c => !(char.IsPunctuation(c) || char.IsWhiteSpace(c))).ToArray());

            char[] charArray = newString.ToCharArray();
            Array.Reverse(charArray);
            string reversedString = new string(charArray);

            return newString == reversedString;
        }
    }
}
