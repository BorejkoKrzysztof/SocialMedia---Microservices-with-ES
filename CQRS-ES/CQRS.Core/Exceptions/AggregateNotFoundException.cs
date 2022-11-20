using System;

namespace CQRS.Core.Exceptions
{
    public class AggregateNotFoundException : Exception
    {
        public AggregateException(string message) : base(message)
        {

        }
    }
}
