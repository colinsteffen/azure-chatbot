using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FAQBot.Helper
{
    public static class TextFormatHelper
    {
        public static string RemoveWhitespaceBeforeAfterHyphen(string text)
        {
            string newText = text.Replace(" - ", "-");

            return newText;
        }
    }
}
