using System;
using System.Collections.Generic;
using System.Text;

namespace RE4_MDT_EDIT
{
    internal static class MdtEncodingPrint
    {
        public static void Print(MdtEncoding mdtEncoding) 
        {
            Console.WriteLine("Encoding:");
            Console.WriteLine("Info:");
            Console.WriteLine("Title: " + mdtEncoding.InfoTitle);
            Console.WriteLine("Author: " + mdtEncoding.InfoAuthor);
            Console.WriteLine("Description: " + mdtEncoding.InfoDescription);
            Console.WriteLine("Config:");
            Console.WriteLine("CmdStartChar: " + mdtEncoding.CmdStartChar);
            Console.WriteLine("CmdEndChar: " + mdtEncoding.CmdEndChar);

            if (mdtEncoding.CharsetList.Count != 0)
            {
                Console.WriteLine("CharsetList:");
                foreach (var item in mdtEncoding.CharsetList)
                {
                    Console.WriteLine($"{item.Key:X4}={item.Value}");
                }
            }

            if (mdtEncoding.ExtraCharset.Count != 0)
            {
                Console.WriteLine("ExtraCharset:");
                foreach (var item in mdtEncoding.ExtraCharset)
                {
                    Console.WriteLine($"{item.Key:X4}={item.Value}");
                }
            }

            if (mdtEncoding.ColorList.Count !=0)
            {
                Console.WriteLine("ColorList:");
                foreach (var item in mdtEncoding.ColorList)
                {
                    Console.WriteLine($"{item.Key:X4}={item.Value}");
                }
            }

            if (mdtEncoding.AkaCharset.Count != 0)
            {
                Console.WriteLine("AkaCharset:");
                foreach (var item in mdtEncoding.AkaCharset)
                {
                    Console.WriteLine($"{item.Key}={item.Value:X4}");
                }
            }

            if (mdtEncoding.InvCharsetListCMD.Count != 0)
            {
                Console.WriteLine("InvCharsetListCMD:");
                foreach (var item in mdtEncoding.InvCharsetListCMD)
                {
                    Console.WriteLine($"{item.Key}={item.Value:X4}");
                }
            }

            if (mdtEncoding.InvCharsetListChars.Count != 0)
            {
                Console.WriteLine("InvCharsetListChars:");
                foreach (var item in mdtEncoding.InvCharsetListChars)
                {
                    Console.WriteLine($"{item.Key}={item.Value:X4}");
                }
            }

            if (mdtEncoding.Replace.Length != 0)
            {
                Console.WriteLine("Replace:");
                foreach (var item in mdtEncoding.Replace)
                {
                    Console.WriteLine($"{item.inText}={item.outText}");
                }
            }
        }

    }
}
