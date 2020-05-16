using System.Text;

namespace ScrapTDWrapper
{
    internal static class Formatter
    {
        internal const string SpecialChars = " .?()!,;:*&/_-@%+=#'|\"[]{}<>^äöüÄÖÜß$€~§\\“´`абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯÇáéíóúÓąćĘęŁłŃńŚśŹźżĆÁÍÚÑÀÈÌÒÙñàè";

        public static string FromFormatted(string formattedText)
        {
            var plainBuilder = new StringBuilder();
            var numberBuilder = new StringBuilder();

            bool isSepcialChar = false;

            foreach (char character in formattedText)
            {
                if (!isSepcialChar)
                {
                    if (character == '<')
                    {
                        isSepcialChar = true;
                    }
                    else
                    {
                        plainBuilder.Append(character);
                    }
                }
                else
                {
                    if (character == '>')
                    {
                        int index = int.Parse(numberBuilder.ToString());
                        numberBuilder.Clear();
                        isSepcialChar = false;

                        if (index < SpecialChars.Length)
                        {
                            plainBuilder.Append(SpecialChars[index]);
                        }
                        else
                        {
                            plainBuilder.Append("?");
                        }
                    }
                    else
                    {
                        numberBuilder.Append(character);
                    }
                }
            }

            return plainBuilder.ToString();
        }

        public static string ToFormatted(string plainText)
        {
            var formattedBuilder = new StringBuilder();

            for (int i = 0; i < plainText.Length; i++)
            {
                char character = plainText[i];

                int index = SpecialChars.IndexOf(character);

                if (index != -1)
                {
                    formattedBuilder.Append($"<{index}>");
                }
                else
                {
                    formattedBuilder.Append(character);
                }
            }

            return formattedBuilder.ToString();
        }
    }
}
