using System;
using System.Collections.Generic;
using System.Text;

namespace RE4_MDT_EDIT_MULTI
{
    internal class MonoLangParsed 
    {
        public uint Magic;
        public string[] Lines;
        public string MdtEncodingInfoTitle;

        public MonoLangParsed(uint Magic, string[] Lines, string MdtEncodingInfoTitle) 
        {
            this.Magic = Magic;
            this.Lines = Lines;
            this.MdtEncodingInfoTitle = MdtEncodingInfoTitle;
        }

    }
}
