using System;
using System.Collections.Generic;
using System.Text;

namespace RE4_MDT_EDIT_CHOICE
{
    internal class JsonLoadException : Exception
    {
        public JsonLoadException(string mensage) : base(mensage) { }
        public JsonLoadException(string mensage, Exception ex) : base(mensage, ex) { }
    }
}
