namespace TaxCash.LoanOrigination
{
    public static class KeyConverter
    {
        public static string Convert(long id)
        {
            return id.ToString();
        }

        public static long Convert(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return 0;
            }
            return long.Parse(id);
        }
    }
}
