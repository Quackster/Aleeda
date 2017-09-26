using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Aleeda.Storage;
using Aleeda.HabboHotel.Client;

namespace Aleeda.HabboHotel.Cache
{
    class Privilege
    {
        public static Dictionary<string, bool> Privileges;

        public static void BootUp()
        {
            Privileges = new Dictionary<string, bool>();

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                DataTable BootTable = null;
                
                BootTable = dbClient.ReadDataTable("SELECT field, enabled FROM server_config");

                if (BootTable != null)
                {
                    foreach (DataRow BootRows in BootTable.Rows)
                    {
                        if ((int)BootRows["enabled"] == 0)
                            Privilege.Privileges.Add((string)BootRows["field"], true);
                        else
                            Privilege.Privileges.Add((string)BootRows["field"], false);
                    }
                }
            }

            //Console.WriteLine("Initializing System Hotel Privileges.");
            //Console.WriteLine("Successfully cached [ " + Privileges.Count + " ] privileges.\n");

        }
        public static void Reload()
        {
            Privilege.Privileges.Clear();

            BootUp();
        }
        public static bool GetEnabled(GameClient Session, string field)
        {
            bool PrivilegeBool;

            if (Privilege.Privileges.ContainsKey(field))
            {
                PrivilegeBool = Privilege.Privileges[field];

                if (PrivilegeBool == true)
                    Session.GetConnection().Send_Data("BKThe function '" + field + "' is disabled.");

                return PrivilegeBool;
            }
            return false;
        }
    }
}
