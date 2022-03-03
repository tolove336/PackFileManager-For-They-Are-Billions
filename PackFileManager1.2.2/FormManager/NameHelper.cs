using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackFileManager.FormManager
{
    public class NameHelper
    {
        public static string[] AllChars = new string[26] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        public static string[] AllNumbers = new string[10] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        public static string GetNewControl(int CharLength = 5, int NumberLength = 3)
        {
            string StringFormat = "{0}{1}";

            string CharContent = "";

            string NumberContent = "";

            for (int i = 0; i < CharLength; i++)
                CharContent += AllChars[new Random(Guid.NewGuid().GetHashCode()).Next(0, AllChars.Length)];

            for (int i = 0; i < NumberLength; i++)
                NumberContent += AllNumbers[new Random(Guid.NewGuid().GetHashCode()).Next(0, AllNumbers.Length)];

            return string.Format(StringFormat, CharContent, NumberContent);
        }
    }
}
