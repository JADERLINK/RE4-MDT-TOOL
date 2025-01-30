using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_MDT_EDIT_MONO_BIG
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("RE4_MDT_EDIT_MONO_BIG");
            Console.WriteLine("by: JADERLINK");
            Console.WriteLine("youtube.com/@JADERLINK");
            Console.WriteLine("github.com/JADERLINK");
            Console.WriteLine(RE4_MDT_EDIT_SHARED.ProgVersion.Version);
            Console.WriteLine("");

            RE4_MDT_EDIT_MONO.MainAction.Continue(args, false, SimpleEndianBinaryIO.Endianness.BigEndian, false);
        }
    }
}
