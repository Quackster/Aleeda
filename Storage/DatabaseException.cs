using System;

namespace Aleeda.Storage
{
    public class DatabaseException : Exception
    {
        public DatabaseException(string sMessage) : base(sMessage) { }
    }
}
