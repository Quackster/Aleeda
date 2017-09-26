using System;

using Aleeda.HabboHotel.Client;

namespace Aleeda.HabboHotel.Habbos
{
    /// <summary>
    /// Manages service users ('Habbo's') and provides methods for updating and retrieving accounts.
    /// </summary>
    public class HabboManager
    {
        #region Constructors

        #endregion

        #region Methods
        public Habbo GetHabbo(uint ID)
        {
            // Prefer active client over Database
            GameClient client = AleedaEnvironment.GetHabboHotel().GetClients().GetClientOfHabbo(ID);
            if (client != null)
            {
                return client.GetHabbo();
            }
            else
            {
                Habbo habbo = new Habbo();
                if (habbo.LoadByID(AleedaEnvironment.GetDatabase(), ID))
                {
                    return habbo;
                }
            }

            return null;
        }
        public Habbo GetHabbo(string sUsername)
        {
            // TODO: some sort of cache?

            Habbo habbo = new Habbo();
            if (habbo.LoadByUsername(AleedaEnvironment.GetDatabase(), sUsername))
            {
                return habbo;
            }

            return null;
        }

        public bool UpdateHabbo(Habbo habbo)
        {
            return AleedaEnvironment.GetDatabase().UPDATE(habbo);
        }
        #endregion
    }
}
