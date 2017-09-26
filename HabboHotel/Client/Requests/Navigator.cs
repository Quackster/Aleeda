using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Aleeda.HabboHotel.Rooms;
using Aleeda.Storage;
using Aleeda.Core;


namespace Aleeda.HabboHotel.Client
{
    public partial class ClientMessageHandler
    {
        /// <summary>
        /// 380 - "E|"
        /// </summary>
        private void SetFrontPageListening()
        {
            Response.Initialize(450); // "GB"
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                DataTable DataQuery = dbClient.ReadDataTable("SELECT * FROM public_rooms ORDER BY style ASC");

                Response.AppendInt32(DataQuery.Rows.Count);
                foreach (System.Data.DataRow Row in DataQuery.Rows)
                {
                    Response.AppendInt32((int)Row["id"]);
                    Response.AppendString((string)Row["frontpagetext"]);
                    Response.AppendString((string)Row["desc"]);
                    Response.AppendInt32((int)Row["style"]);
                    Response.AppendString((string)Row["frontpagetext"]);
                    Response.AppendString(string.Empty);
                    Response.AppendInt32((int)Row["hidden"]);
                    Response.AppendInt32((int)Row["in_room"]);
                    Response.AppendInt32((int)Row["type"]);
                    Response.AppendString((string)Row["model"]);
                    Response.AppendInt32((int)Row["id"]);
                    Response.AppendInt32(0);
                    Response.AppendString((string)Row["ccts"]);
                    Response.AppendInt32((int)Row["max_in"]);
                    Response.AppendInt32((int)Row["id"]);
                }
                SendResponse();
            }

        }
        private void ShowRoomOMatic()
        {
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                if (dbClient.ReadDataTable("SELECT * FROM private_rooms WHERE ownerid = '" + Client.GetHabbo().ID + "'").Rows.Count > 25) //If user has more then 25 already
                {
                    Response.Initialize(139);
                    Response.Append("You have exceded your room limit, the room limit is 25 rooms per user");
                    SendResponse();
                }
                else
                {
                    Response.Initialize(512);
                    Response.AppendInt32(0);
                    Response.AppendInt32(25);
                    SendResponse();
                }
            }
        }
        private void RoomInsertQuery()
        {
            string Name = Request.PopFixedString();
            string Model = Request.PopFixedString();
            int ID = 0;

            #region Insert
            Dictionary<string, object> KeyWithValue = new Dictionary<string, object>();

            KeyWithValue.Add("name", Name);
            KeyWithValue.Add("rating", 0);
            KeyWithValue.Add("description", "New room.");
            KeyWithValue.Add("ownerid", Client.GetHabbo().ID);
            KeyWithValue.Add("status", 0);
            KeyWithValue.Add("tags", "");
            KeyWithValue.Add("thumbnail", "HHHH");
            KeyWithValue.Add("petsAllowed", 1);
            KeyWithValue.Add("category", 0);
            KeyWithValue.Add("model", Model);
            KeyWithValue.Add("wallpaper", 000);
            KeyWithValue.Add("floorpaper", 000);
            KeyWithValue.Add("landscape", 0.0);

            doQuery().doInsertQuery("private_rooms", KeyWithValue); 
            #endregion

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                ID = dbClient.ReadInt32("SELECT id FROM private_rooms WHERE ownerid = '" + Client.GetHabbo().ID + "' AND name = '" + Name + "' AND model = '" + Model + "'");
            }

            Response.Initialize(59); // "@{"
            Response.AppendInt32(ID);
            Response.Append(Name);
            SendResponse();

            //Add new room to cache
            AleedaEnvironment.GetCache().GetPrivateRooms().newRoom(ID);
        }
        private void GetOwnGuestRoom()
        {
            Client.GetConnection().SendMessage(AleedaEnvironment.GetCache().GetPrivateRooms().SerializeOwnRooms(Client));
        }
        private void HighestInRoomCurrent()
        {
            Client.GetConnection().SendMessage(AleedaEnvironment.GetCache().GetPrivateRooms().SerializeUsersInRooms());
        }
        private void HighestInRoomRating()
        {
            Client.GetConnection().SendMessage(AleedaEnvironment.GetCache().GetPrivateRooms().SerializeHighestRoomRating());
        }
        private void AddFav()
        {
            int RoomID = Request.PopWiredInt32();

            Response.Initialize(459);
            Response.AppendInt32(RoomID);
            Response.AppendBoolean(true);
            SendResponse();

            AleedaEnvironment.GetCache().GetRoomFavs().NewEntry(RoomID, Client.GetHabbo().ID);
        }
        private void RemoveFav()
        {
            int RoomID = Request.PopWiredInt32();

            Response.Initialize(459);
            Response.AppendInt32(RoomID);
            Response.AppendBoolean(false);
            SendResponse();

            AleedaEnvironment.GetCache().GetRoomFavs().DeleteEntry(RoomID, Client.GetHabbo().ID);
        }
        /// <summary>
        /// Registers request handlers that process room Navigator queries etc.
        /// </summary>
        public void RegisterNavigator()
        {
            mRequestHandlers[380] = new RequestHandler(SetFrontPageListening);
            mRequestHandlers[434] = new RequestHandler(GetOwnGuestRoom);
            mRequestHandlers[387] = new RequestHandler(ShowRoomOMatic);
            mRequestHandlers[29] = new RequestHandler(RoomInsertQuery);
            mRequestHandlers[430] = new RequestHandler(HighestInRoomCurrent);
            mRequestHandlers[431] = new RequestHandler(HighestInRoomRating);
            mRequestHandlers[20] = new RequestHandler(RemoveFav);
        }
    }
}
