using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType
{

    [global::System.Serializable]
    public class TTFReadException : ApplicationException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public TTFReadException() { }
        public TTFReadException(string message) : base(message) { }
        public TTFReadException(string message, Exception inner) : base(message, inner) { }
        protected TTFReadException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
