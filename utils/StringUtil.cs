using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiden
{
    public class StringUtil
    {

        public static string ordinal_suffix_of(int i)
        {
            var j = i % 10;
            var k = i % 100;
            if (j == 1 && k != 11)
            {
                return i + "st";
            }
            if (j == 2 && k != 12)
            {
                return i + "nd";
            }
            if (j == 3 && k != 13)
            {
                return i + "rd";
            }
            return i + "th";
        }


    }
}
