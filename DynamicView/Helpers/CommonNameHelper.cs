namespace DynamicView.Helpers
{
    public class CommonNameHelper
    {
        public static string GetAlphabeticalSequence(int num)
        {
            string result = "";
            while (num > 0)
            {
                num--;
                result = (char)('A' + (num % 26)) + result;
                num /= 26;
            }
            return result;
        }

        public static List<string> GenerateColumnNames(int numberOfColumns)
        {
            List<string> columnNames = new List<string>();

            for (int i = 1; i <= numberOfColumns; i++)
            {
                columnNames.Add(GetAlphabeticalSequence(i));
            }

            return columnNames;
        }
    }
}
