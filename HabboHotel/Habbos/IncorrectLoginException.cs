using System;

namespace Aleeda.HabboHotel.Habbos
{
    public class IncorrectLoginException : Exception
    {
        public IncorrectLoginException(string sMessage) : base(sMessage) { }
    }
}
