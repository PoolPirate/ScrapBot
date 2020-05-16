using System.Text;

namespace ScrapTDWrapper.Extensions
{
    public static class StringExtensions
    {
        public static byte[] GetBytes(this string text) => Encoding.ASCII.GetBytes(text);
    }
}
