﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_MDT_EDIT_MULTI8_UHD
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("RE4_MDT_EDIT_MULTI8_UHD");
            Console.WriteLine("by: JADERLINK");
            Console.WriteLine("youtube.com/@JADERLINK");
            Console.WriteLine("github.com/JADERLINK");
            Console.WriteLine(RE4_MDT_EDIT_MULTI.MainAction.Version);
            Console.WriteLine("");

            RE4_MDT_EDIT_MULTI.MainAction.Continue(args, RE4_MDT_EDIT_MULTI.FileVersion.MULTI8_UHD, false);
        }
    }
}