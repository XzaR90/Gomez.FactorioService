using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gomez.Core.Exceptions
{
    [Serializable]
    public class FactorioServiceException : Exception
    {
        public FactorioServiceException()
        {
        }

        public FactorioServiceException(string? message)
            : base(message)
        {
        }

        public FactorioServiceException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        protected FactorioServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
