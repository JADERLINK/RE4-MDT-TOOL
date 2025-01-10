using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_MDT_CHOICE_NS_MULTI
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.WriteLine("RE4_MDT_CHOICE_NS_MULTI");
            Console.WriteLine("by: JADERLINK");
            Console.WriteLine("youtube.com/@JADERLINK");
            Console.WriteLine("github.com/JADERLINK");
            Console.WriteLine("Version 1.1 (2025-01-10)");
            Console.WriteLine("");

            RE4_MDT_CHOICE.MainAction.Continue(args, true, false, SimpleEndianBinaryIO.Endianness.LittleEndian);
        }
        
    }
}
