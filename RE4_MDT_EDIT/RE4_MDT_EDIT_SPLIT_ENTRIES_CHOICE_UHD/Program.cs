﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_MDT_EDIT_SPLIT_ENTRIES_CHOICE_UHD
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("RE4_MDT_EDIT_SPLIT_ENTRIES_CHOICE_UHD");
            Console.WriteLine("by: JADERLINK");
            Console.WriteLine("youtube.com/@JADERLINK");
            Console.WriteLine("github.com/JADERLINK");
            Console.WriteLine(RE4_MDT_EDIT_SHARED.ProgVersion.Version);
            Console.WriteLine("");

            RE4_MDT_EDIT_CHOICE.MainAction.Continue(args, RE4_MDT_EDIT_CHOICE.FileVersion.CHOICE_UHD, true);
        }
    }
}
