using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Aleeda.Storage;

namespace Aleeda.HabboHotel.Messenger
{
    public class MessengerManager
    {
        public List<MessengerBuddy> SearchHabbos(string criteria)
        {
            List<MessengerBuddy> matches = new List<MessengerBuddy>();
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("@criteria", criteria + "%");
                foreach (DataRow row in dbClient.ReadDataTable("SELECT id,username,figure,motto FROM users WHERE username LIKE @criteria;").Rows)
                {
                    MessengerBuddy match = MessengerBuddy.Parse(row);
                    if (match != null)
                    {
                        matches.Add(match);
                    }
                }
            }
            return matches; 
        }
    }
}
