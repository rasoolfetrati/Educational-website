using System;
using System.Collections.Generic;
using System.Text;

namespace TopLearn.Core.Convertors
{
    public static class FixedText
    {
        public static string FixEmail(this string value)
        {
            return value.Trim().ToLower();
        }
    }
}
