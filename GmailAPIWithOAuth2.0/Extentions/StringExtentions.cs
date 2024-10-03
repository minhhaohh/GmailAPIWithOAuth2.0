namespace GmailAPIWithOAuth2.Extentions
{
    public static class StringExtentions
    {
		public static bool IsEmpty(this string value)
		{
			return string.IsNullOrWhiteSpace(value);
		}

		public static bool HasValue(this string value)
		{
			return !string.IsNullOrWhiteSpace(value);
		}
	}
}
