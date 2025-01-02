using System;
using System.Collections.Generic;
using System.Text;

namespace RE4_MDT_EDIT
{
    internal class MdtEncodingException : Exception
    {
        public MdtEncodingException(string mensage) : base(mensage) { }
        public MdtEncodingException(string mensage, Exception ex) : base(mensage, ex) { }
    }
}
