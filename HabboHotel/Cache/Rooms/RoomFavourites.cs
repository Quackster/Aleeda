using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Aleeda.Storage;
using Aleeda.Net.Messages;

namespace Aleeda.HabboHotel.Cache
{
    public class RoomFavourites
    {
        #region Fields
        public int favID;
        public uint userID;
        public List<RoomFavourites> roomFav;
        #endregion

        #region Constructers
        public RoomFavourites()
        {
            //Inialize the roomFav list :D
            roomFav = new List<RoomFavourites>();

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM room_favourites;").Rows)
                {
                    roomFav.Add(new RoomFavourites((int)row["roomid"], (uint)row["userid"]));
                }
            }
        }
        public RoomFavourites(int favId, uint userId)
        {
            this.favID = favId;
            this.userID = userId;
        }
        #endregion

        #region Methods
        public int GetUserCount(uint userId)
        {
            int i = 0;
            foreach (RoomFavourites fav in roomFav)
            {
                if (fav.userID == userId)
                {
                    ++i;
                }
            }
            return i;
        }
        public void AppendRoomIds(ServerMessage Message, uint userId)
        {
            foreach (RoomFavourites fav in roomFav)
            {
                if (fav.userID == userId)
                {
                    Message.AppendInt32(fav.favID);
                }
            }
        }
        public void NewEntry(int roomId, uint userId)
        {
            roomFav.Add(new RoomFavourites(roomId, userId));

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("INSERT INTO room_favourites (roomid, userid) VALUES ('" + roomId + "','" + userId + "')");
            }
        }
        public void DeleteEntry(int roomId, uint userId)
        {
            foreach (RoomFavourites fav in roomFav.ToArray())
            {
                if (fav.userID == userId && fav.favID == roomId)
                {
                    roomFav.Remove(fav);
                }
            }
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("DELETE FROM room_favourites WHERE roomid='" + roomId + "' AND userid='" + userId + "'");
            }
        }
        #endregion
    }
}
