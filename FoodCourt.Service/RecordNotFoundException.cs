using System;

namespace FoodCourt.Service
{
    public class RecordNotFoundException : Exception
    {
        public RecordNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public RecordNotFoundException(string message)
            : base(message)
        {
        }
    }
}