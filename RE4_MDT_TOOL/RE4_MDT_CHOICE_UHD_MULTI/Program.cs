using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_MDT_CHOICE_UHD_MULTI
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.WriteLine("RE4_MDT_CHOICE_UHD_MULTI");
            Console.WriteLine("by: JADERLINK");
            Console.WriteLine("youtube.com/@JADERLINK");
            Console.WriteLine("github.com/JADERLINK");
            Console.WriteLine(RE4_MDT_CHOICE.MainAction.Version);
            Console.WriteLine("");

            RE4_MDT_CHOICE.MainAction.Continue(args, false, false, SimpleEndianBinaryIO.Endianness.LittleEndian);
        }
    }
}
