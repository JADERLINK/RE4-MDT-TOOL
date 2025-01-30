using System;
using System.Collections.Generic;
using System.Text;

namespace RE4_MDT_EDIT
{
    internal class MdtEncoding
    {
        public string InfoTitle { get; set; }
        public string InfoAuthor { get; set; }
        public string InfoDescription { get; set; }
        public char CmdStartChar { get; set; }
        public char CmdEndChar { get; set; }
        public Dictionary<ushort, string> ColorList { get; set; }
        public Dictionary<ushort, string> CharsetList { get; set; }
        public Dictionary<ushort, string> ExtraCharset { get; set; }
        public Dictionary<string, ushort> AkaCharset { get; set; }

        public Dictionary<string, ushort> InvCharsetListCMD { get; set; }
        public Dictionary<char, ushort> InvCharsetListChars { get; set; }

        public (string inText, string outText)[] Replace { get; set; }
    }
}
