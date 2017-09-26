using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aleeda.Storage;

namespace Aleeda.Core
{
    public class Update
    {
        public static void UpdateRoomID(int id, string username)
        {
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("user", username);
                dbClient.AddParamWithValue("id", Convert.ToUInt64(id));
                dbClient.ExecuteQuery("UPDATE users SET flat = @id WHERE username = @user;");
            }
        }
        public static void UpdateMotto(string motto, string username)
        {
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("user", username);
                dbClient.AddParamWithValue("motto", motto);
                dbClient.ExecuteQuery("UPDATE `users` SET `motto` = @motto WHERE `username` = @user;");
            }
        }
        public static void UpdateFurniTrigger(int mFurniID, int trigger)
        {
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("trigger", trigger);
                dbClient.AddParamWithValue("id", mFurniID);
                dbClient.ExecuteQuery("UPDATE `room_items` SET `trigger` = @trigger WHERE `id` = @id");
            }
        }
        public static int CalculateVirtualID(bool HasVirtualID)
        {
            if (HasVirtualID == false)
            {
                //Generate random number
                int randomNum = AleedaEnvironment.GenerateRandomNum(1, 1000);

                //Sets the virtual id
                return randomNum;
            }
            return 0;
        }
    }
}
