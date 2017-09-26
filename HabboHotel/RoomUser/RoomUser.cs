using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Aleeda.HabboHotel.Client;
using Aleeda.Storage;
using Aleeda.Net.Messages;
using Aleeda.Core;

namespace Aleeda.HabboHotel
{
    public class RoomUser
    {
        public static ClientMessageHandler mRoom;
        public List<UserInventory> GetUserInventory(uint userid, bool b, int id)
        {
            List<UserInventory> rDetails = new List<UserInventory>();
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                if (b == true)
                {
                    dbClient.AddParamWithValue("@userid", userid);
                    foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM user_inventory WHERE userid = @userid").Rows)
                    {
                        UserInventory details = UserInventory.Parse(row, false);
                        if (details != null)
                        {
                            rDetails.Add(details);
                        }
                    }
                }
                else if (b == false)
                {
                    dbClient.AddParamWithValue("@id", id);
                    foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM user_inventory WHERE id = @id").Rows)
                    {
                        UserInventory details = UserInventory.Parse(row, false);
                        if (details != null)
                        {
                            rDetails.Add(details);
                        }
                    }
                }
            }
            return rDetails;
        }
        public List<UserInventory> GetUserPetInventory(uint userid)
        {
            List<UserInventory> rDetails = new List<UserInventory>();
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("@userid", userid);
                foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM user_pet_inventory WHERE userid = @userid").Rows)
                {
                    UserInventory details = UserInventory.Parse(row, true);
                    if (details != null)
                    {
                        rDetails.Add(details);
                    }
                }
            }
            return rDetails;
        }
        public List<UserInventory> GetItem(uint id)
        {
            List<UserInventory> rDetails = new List<UserInventory>();
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("@id", id);
                foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM user_inventory WHERE id = @id").Rows)
                {
                    UserInventory details = UserInventory.Parse(row, false);
                    if (details != null)
                    {
                        rDetails.Add(details);
                    }
                }
            }
            return rDetails;
        }
        public static ServerMessage PlaceFurni(GameClient Session, int id, int x, int y, int rot, DatabaseClient dbClient)
        {
            List<UserInventory> mInventoryItem = AleedaEnvironment.GetHabboHotel().GetRoomUser().GetUserInventory(Session.GetHabbo().ID, false, Session.GetHabbo().ItemUsingID);

            foreach (UserInventory mItem in mInventoryItem)
            {
                dbClient.AddParamWithValue("id", Session.GetHabbo().RoomId);
                dbClient.AddParamWithValue("x", x);
                dbClient.AddParamWithValue("y", y);
                dbClient.AddParamWithValue("rotation", rot);
                dbClient.AddParamWithValue("sID", mItem.SpriteID);
                dbClient.ExecuteQuery("INSERT INTO room_items (`id`, `mID`, `x_axis`, `y_axis`, `rotation`, `sprite_id`, `trigger`, `isWallItem`) VALUES ('" + id + "', @id, @x, @y, @rotation, @sID, 1, 0);");

                ServerMessage Message = new ServerMessage(93);
                Message.AppendInt32(id);
                Message.AppendInt32(mItem.SpriteID);
                Message.AppendInt32(x);
                Message.AppendInt32(y);
                Message.AppendInt32(rot);
                Message.AppendString("0.0");
                Message.AppendInt32(1);
                Message.AppendString("" + 1);
                Message.AppendInt32(-1);
                return Message;
            }
            return null;
        }
        public static bool CalculateRights(int RoomID, uint usernameid)
        {
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                bool CheckRights = dbClient.ReadBoolean("SELECT * FROM room_rights WHERE user = '" + usernameid + "' AND roomid = '" + RoomID + "'");
                bool CheckOwner = dbClient.ReadBoolean("SELECT * FROM private_rooms WHERE ownerid = '" + usernameid + "' AND id = '" + RoomID + "'");

                if (CheckRights && !CheckOwner)
                    return true;
                else if (!CheckRights && CheckOwner)
                    return true;
                else if (CheckRights && CheckOwner)
                    return true;
                else if (!CheckRights && !CheckOwner)
                    return false;
                else
                    return false;
            }
        }
    }
}
