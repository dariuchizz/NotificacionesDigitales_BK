namespace Common.Functions
{
    public static class FunctionsText
    {
        public static string CutText(string textValue, int length)
        {
            if (textValue?.Length > length)
                return textValue.Substring(0, length);
            else
                return textValue;
        }
    }
}
