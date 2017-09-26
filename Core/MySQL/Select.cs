using System.Data;
using Aleeda.HabboHotel.Client;
using Aleeda.Net.Messages;
using Aleeda.Storage;
using Aleeda.HabboHotel.Cache;

namespace Aleeda.Core
{
    class Select
    {
        public static bool IfOwnerIsInRoom(int id, string owner)
        {
            bool IfOwnerIsInRoom = false;

            foreach (GameClient mClient in ClientMessageHandler.mRoomList)
            {
                if (mClient.GetHabbo().Username == owner && mClient.GetHabbo().RoomId == (uint)id)
                {
                    IfOwnerIsInRoom = true;
                }
                else
                {
                    IfOwnerIsInRoom = false;
                }
            }
            return IfOwnerIsInRoom;
        }
        public static string GetStartingPosition(string model)
        {
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("model", model);
                return dbClient.ReadString("SELECT starting_pos FROM room_models WHERE id = @model");
            }

        }
        public static string GetPublicRoomItems(string model)
        {
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("model", model);
                try
                {
                    return dbClient.ReadString("SELECT public_items FROM room_models WHERE id = @model");
                }
                catch
                {
                    return wireEncoding.encodeVL64(0); //item count
                }
            }
        }
        public static int OnlineCount()
        {
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                return dbClient.ReadDataTable("SELECT * FROM users WHERE online = '1'").Rows.Count;
            }
        }
    }
    public class Server
    {
        public static void SendRoom(int RoomId, ServerMessage Message)
        {
            foreach (GameClient mClient in HabboHotel.Client.ClientMessageHandler.mRoomList)
            {
                if (mClient.GetHabbo().RoomId == RoomId)
                {
                    mClient.GetConnection().SendMessage(Message);
                }
            }
        }
        public static void SendHotelMsg(ServerMessage Message)
        {
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM users WHERE online = '1';").Rows)
                {
                    uint mId = (uint)row["id"];
                    GameClient mClient = AleedaEnvironment.GetHabboHotel().GetClients().GetClientOfHabbo(mId);
                    mClient.GetConnection().SendMessage(Message);
                }
            }
        }
    }
}
