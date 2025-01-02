using System;
using System.Collections.Generic;
using System.Text;

namespace RE4_MDT_EDIT
{
    internal class MdtJsonLoadException : Exception
    {
        public MdtJsonLoadException(string mensage) : base(mensage) { }
        public MdtJsonLoadException(string mensage, Exception ex) : base(mensage, ex) { }
    }
}
