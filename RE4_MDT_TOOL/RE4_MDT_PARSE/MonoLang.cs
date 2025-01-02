using System;
using System.Collections.Generic;
using System.Text;

namespace RE4_MDT_PARSE
{
    internal class MonoLang
    {
        public uint Magic;
        public ushort[] CharArr;
        public uint[] Offset;

        public MonoLang(uint Magic, ushort[] CharArr, uint[] Offset)
        {
            this.Magic = Magic;
            this.CharArr = CharArr;
            this.Offset = Offset;
        }
    }
}
