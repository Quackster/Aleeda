using System;
using System.Collections.Generic;
using System.Text;
using Aleeda.Storage;

namespace Aleeda.Core
{
    class mBoolean
    {
        public static bool CheckRoomBanned(string user, int rID)
        {
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("user", user);
                dbClient.AddParamWithValue("id", rID);
                return dbClient.ReadBoolean("SELECT * FROM room_bans WHERE user = @user AND roomid = @id;");
            }
        }
        public static bool CheckRoomRights(string user, int rID)
        {
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("user", user);
                dbClient.AddParamWithValue("id", rID);
                return dbClient.ReadBoolean("SELECT * FROM room_rights WHERE user = @user AND roomid = @id;");
            }
        }
        public static bool FindExistingFriend(uint uID, string uUser)
        {
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("id", uID);
                dbClient.AddParamWithValue("user", AleedaEnvironment.GetHabboHotel().GetHabbos().GetHabbo(uUser).ID);
                return dbClient.ReadBoolean("SELECT * FROM messenger_buddylist WHERE buddyid = @id AND userid = @user;");
            }
        }
        public static bool CheckRoomOwner(int userID, int rID)
        {
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("user", userID);
                dbClient.AddParamWithValue("id", rID);
                return dbClient.ReadBoolean("SELECT * FROM private_rooms WHERE ownerid = @user AND id = @id;");
            }
        }
    }
}
