using System;

namespace BetterEditor.Core.Exceptions
{
    class BECommandRequiredKeyNotFoundException : Exception
    {
        public BECommandRequiredKeyNotFoundException(string message) : base(message)
        {
            
        }
    }
}
