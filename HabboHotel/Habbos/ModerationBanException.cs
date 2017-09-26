using System;

namespace Aleeda.HabboHotel.Habbos
{
    public class ModerationBanException : Exception
    {
        public ModerationBanException(string sReason) : base(sReason) { }
    }
}
