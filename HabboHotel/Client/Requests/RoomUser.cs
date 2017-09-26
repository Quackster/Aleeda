using System;
using System.Collections.Generic;
using Aleeda.Core;
using Aleeda.HabboHotel.Cache;
using Aleeda.HabboHotel.Rooms;
using Aleeda.HabboHotel.Rooms.User;
using Aleeda.Net.Messages;
using System.Diagnostics;
using Aleeda.Storage;
using System.Threading;

namespace Aleeda.HabboHotel.Client
{
    public partial class ClientMessageHandler
    {
        private void ChatHandler()
        {
            string chat = Request.PopFixedString();
            if (chat.StartsWith(":"))
            {
                chat = chat.TrimStart(':');

                new Aleeda.HabboHotel.Cache.Rooms.Misc.Commands().InitCommand(Client, chat.Split(' '), chat);
            }
            else
            {
                Response.Initialize(24); // "@X"
                Response.AppendInt32(Client.GetHabbo().UnitId);
                Response.AppendString(chat);
                Response.AppendInt32(Request.PopWiredInt32());
                SendRoom(Response);
            }
        }
        private void ShoutHandler()
        {
            Response.Initialize(26); // "@Z"
            Response.AppendInt32(Client.GetHabbo().UnitId);
            Response.AppendString(Request.PopFixedString());
            Response.AppendInt32(Request.PopWiredInt32());
            SendRoom();

            //Client.GetHabbo().SendChat(1, "cake", 1);
        }
        private void WisperHandler()
        {
            string mMessage = Request.PopFixedString();
            string TargetUser = mMessage.Split(' ')[0];
            mMessage = mMessage.Substring(TargetUser.Length + 1);

            try
            {
                uint SessionId = AleedaEnvironment.GetHabboHotel().GetHabbos().GetHabbo(TargetUser).ID;
                GameClient mUser = AleedaEnvironment.GetHabboHotel().GetClients().GetClientOfHabbo(SessionId);

                ServerMessage Message = new ServerMessage(25); // "@Y"
                Message.AppendInt32(Client.GetHabbo().UnitId);
                Message.AppendString(mMessage);
                Message.AppendBoolean(true);
                mUser.GetConnection().SendMessage(Message);


                Response.Initialize(25); // "@Y"
                Response.AppendInt32(Client.GetHabbo().UnitId);
                Response.AppendString(mMessage);
                Response.AppendBoolean(true);
                SendResponse();
            }
            catch
            {
                Response.Initialize(25); // "@Y"
                Response.AppendInt32(Client.GetHabbo().UnitId);
                Response.AppendString(mMessage);
                Response.AppendBoolean(true);
                SendRoom();
            }

        }
        private void WaveHandler()
        {
            Response.Initialize(481); // "Ga"
            Response.AppendInt32(Client.GetHabbo().UnitId);
            SendRoom();
        }
        private void DanceHandler()
        {
            int DanceId = Request.PopWiredInt32();
            Client.GetHabbo().DanceId = DanceId;

            if (DanceId != 0)
                Client.GetHabbo().IsDancing = true;
            else
                Client.GetHabbo().IsDancing = false;


            Response.Initialize(480); // "G`"
            Response.AppendInt32(Client.GetHabbo().UnitId);
            Response.AppendInt32(Client.GetHabbo().DanceId);
            SendRoom();
        }
        private void PathHandler()
        {
            int X = Request.PopWiredInt32();
            int Y = Request.PopWiredInt32();

            Client.GetHabbo().ReqX = X;
            Client.GetHabbo().ReqY = Y;

            if (Client.GetHabbo().FlatType == "private")
             {
                 if (Funcs.RequestToMove(Client, Client.GetHabbo().ReqX, Client.GetHabbo().ReqY))
                 {

                     if (GetFloorItems().xyCoordIsValid(Client.GetHabbo().RoomId, X, Y) == false)
                     {
                         Client.GetHabbo().IsOnItem = true;
                         Client.GetHabbo().ItemUsingID = GetFloorItems().currentId(Client.GetHabbo().RoomId, X, Y);
                         Client.GetHabbo().UserRotation = GetFloorItems().currentRot(Client.GetHabbo().RoomId, X, Y);
                     }
                     else
                     {
                         Client.GetHabbo().IsOnItem = false;
                         Client.GetHabbo().UserRotation = 2;
                         Client.GetHabbo().ItemUsingID = 0;
                     }
                    
                     if (GetRoomBots().RequestToMove(Client, X, Y))
                     {
                         MoveUser(true);
                     }
                     else if (GetRoomBots().RoomBotCounts(Client.GetHabbo().RoomId) == 0)
                     {
                         Client.GetHabbo().X = X;
                         Client.GetHabbo().Y = Y;
                         MoveUser(true);
                     }
                     else
                     {
                         Client.GetHabbo().X = X;
                         Client.GetHabbo().Y = Y;
                         MoveUser(true);
                     }
                 }
             }
             else
             {
                 Client.GetHabbo().X = X;
                 Client.GetHabbo().Y = Y;
                 Client.GetHabbo().IsOnItem = false;
                 MoveUser(false);
             }
         }
        private void MoveUser(bool isPrivate)
        {
            //AleedaEnvironment.GetCache().GetPrivateRooms().LoopPath(Client.GetHabbo().RoomId);
            Response.Initialize(34); // "@b"
            Response.AppendInt32(1);
            Response.AppendInt32(Client.GetHabbo().UnitId);
            Response.AppendInt32(Client.GetHabbo().X);
            Response.AppendInt32(Client.GetHabbo().Y);
            Response.AppendString("0.0");
            Response.AppendInt32(Client.GetHabbo().UserRotation);
            Response.AppendInt32(Client.GetHabbo().UserRotation);
            if (Client.GetHabbo().IsOnItem)
                Response.AppendString("/flatctrl useradmin/sit 1.3/");
            else
                Response.AppendString("/flatctrl useradmin//");
            if (isPrivate)
                SendRoom();
            else
                SendPublicRoom();
        }
        private string MoveUserString(string customStatus)
        {
            ServerMessage r = new ServerMessage();
            r.AppendInt32(Client.GetHabbo().UnitId);
            r.AppendInt32(Client.GetHabbo().X);
            r.AppendInt32(Client.GetHabbo().Y);
            r.AppendString("0.0");
            r.AppendInt32(Client.GetHabbo().UserRotation);
            r.AppendInt32(Client.GetHabbo().UserRotation);

            if (customStatus != "")
                r.AppendString(customStatus);
            else if (Client.GetHabbo().IsOnItem)
                r.AppendString("/flatctrl useradmin/sit 1.3/");
            else
                r.AppendString("/flatctrl useradmin//");

            return r.ToString();
        }

        private void InventoryHandler()
        {
            List<UserInventory> mInventory = AleedaEnvironment.GetHabboHotel().GetRoomUser().GetUserInventory(Client.GetHabbo().ID, true, 0);

            Response.Initialize(140); // "BL"
            Response.AppendString("S");
            Response.AppendInt32(1);
            Response.AppendInt32(1);
            Response.AppendInt32(mInventory.Count);
            foreach (UserInventory mUser in mInventory)
            {
                string CCT = AleedaEnvironment.GetCache().GetCatalogueItems().ReturnCCTName(mUser.SpriteID);

                Response.AppendInt32(mUser.ID);
                Response.AppendString(mUser.Type.ToUpper());
                Response.AppendInt32(mUser.ID);
                Response.AppendInt32(mUser.SpriteID);
                if (mUser.Name.Contains("wallpaper"))
                    Response.AppendInt32(2);
                else if (mUser.Name.Contains("landscape"))
                    Response.AppendInt32(4);
                else if (mUser.Name.Contains("floor"))
                    Response.AppendInt32(3);
                else
                    Response.AppendInt32(0);
                Response.AppendChar(2);
                Response.AppendInt32(1); //Can Recycle
                Response.AppendInt32(1); //Can Trade
                Response.AppendInt32(1); //Can Sell
                Response.AppendInt32(0);
                Response.AppendInt32(-1);
                if (mUser.Type.ToLower() == "s")
                {
                    Response.AppendChar(2);
                    Response.AppendInt32(-1);
                }
            }
            SendResponse();

        }
        private void GetPetsInventory()
        {
            //IXIhByN\lol{{2}}RAHFFFFFF{{2}}{{1}}

            Response.Initialize(600); // "IX"
            Response.AppendInt32(AleedaEnvironment.GetHabboHotel().GetRoomUser().GetUserPetInventory(Client.GetHabbo().ID).Count);
            foreach (UserInventory mUser in AleedaEnvironment.GetHabboHotel().GetRoomUser().GetUserPetInventory(Client.GetHabbo().ID))
            {
                Response.AppendInt32(mUser.ID);
                Response.AppendString(mUser.Petname);
                Response.AppendInt32(mUser.Race);
                Response.AppendInt32(0);
                Response.AppendString(mUser.Colour);
                //Response.AppendBoolean(false);
            }
            SendResponse();
        }
        private void PlaceItem()
        {
            string WallPlacementMap = "";
            int ID;

            if (RoomUser.CalculateRights(Client.GetHabbo().RoomId, Client.GetHabbo().ID))
            {
                WallPlacementMap = Request.PopFixedString(); //Gets the map (placement) string
                if (WallPlacementMap.Contains(":"))
                {
                    foreach (UserInventory mItem in AleedaEnvironment.GetHabboHotel().GetRoomUser().GetUserInventory(Client.GetHabbo().ID, false, int.Parse(WallPlacementMap.Split(' ')[0])))
                    {
                        #region Field
                        string WallPlace = ":" + WallPlacementMap.Split(':')[1];
                        #endregion

                        Dictionary<string, object> KeyWithValue = new Dictionary<string, object>();

                        KeyWithValue.Add("mID", Client.GetHabbo().RoomId);
                        KeyWithValue.Add("wall_item", WallPlace);
                        KeyWithValue.Add("sprite_id", mItem.SpriteID);
                        KeyWithValue.Add("trigger", 0);
                        KeyWithValue.Add("isWallItem", Convert.ToInt32(true));

                        doQuery().doInsertQuery("room_items", KeyWithValue);

                        #region SQL
                        using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
                        {
                            dbClient.AddParamWithValue("id", Client.GetHabbo().RoomId);
                            dbClient.AddParamWithValue("wallitem", WallPlace);
                            ID = dbClient.ReadInt32("SELECT id FROM room_items WHERE mID  = @id AND wall_item = @wallitem;");
                        }
                        #endregion

                        Response.Initialize(83);
                        Response.AppendRawInt32WithBreak(ID, 2);
                        Response.AppendInt32(mItem.SpriteID);
                        Response.AppendString(WallPlace);
                        Response.Append(0);
                        Response.AppendChar(2);
                        Response.AppendChar(2);
                        SendRoom();

                        //Add the new item into the wall items cache.
                        GetWallItems().newItem(ID);

                    }

                }
                else
                {
                    int mX = int.Parse(WallPlacementMap.Split(' ')[1]);
                    int mY = int.Parse(WallPlacementMap.Split(' ')[2]);
                    int mRotation = int.Parse(WallPlacementMap.Split(' ')[3]);

                    foreach (UserInventory mItem in AleedaEnvironment.GetHabboHotel().GetRoomUser().GetUserInventory(Client.GetHabbo().ID, false, int.Parse(WallPlacementMap.Split(' ')[0])))
                    {
                        Dictionary<string, object> KeyWithValue = new Dictionary<string, object>();

                        KeyWithValue.Add("mID", Client.GetHabbo().RoomId);
                        KeyWithValue.Add("x_axis", mX);
                        KeyWithValue.Add("y_axis", mY);
                        KeyWithValue.Add("rotation", mRotation);
                        KeyWithValue.Add("sprite_id", mItem.SpriteID);
                        KeyWithValue.Add("trigger", 0);
                        KeyWithValue.Add("isWallItem", Convert.ToInt32(false));

                        doQuery().doInsertQuery("room_items", KeyWithValue);

                        using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
                        {
                            ID = dbClient.ReadInt32("SELECT id FROM room_items WHERE mID = '" + Client.GetHabbo().RoomId + "' AND x_axis = '" + mX + "' AND y_axis = '" + mY + "'");
                        }

                        Response.Initialize(93);
                        Response.AppendInt32(ID);
                        Response.AppendInt32(mItem.SpriteID);
                        Response.AppendInt32(mX);
                        Response.AppendInt32(mY);
                        Response.AppendInt32(mRotation);
                        Response.AppendString("0.0");
                        Response.AppendInt32(1);
                        Response.AppendString("" + 1);
                        Response.AppendInt32(-1);
                        SendRoom();

                        //Add the new item into the wall items cache.
                        GetFloorItems().newItem(ID);

                    }
                }
                //Remove the item from the inventory.
                Client.GetHabbo().RemoveItem(int.Parse(WallPlacementMap.Split(' ')[0]));
            }
        }
        private void PlacePet()
        {
            PrivateRooms Room = AleedaEnvironment.GetCache().GetPrivateRooms().getRoom(Client.GetHabbo().RoomId);

            if (Room == null || !Client.GetHabbo().HasFuse("fuse_admin") || !Client.GetHabbo().HasFuse("fuse_sysadmin") || !Client.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }
            else if (!Room.petsAllowed)
            {
                return;
            }
            else if (!RoomUser.CalculateRights(Client.GetHabbo().RoomId, Client.GetHabbo().ID))
            {
                return;
            }

            int PetId = Request.PopWiredInt32();
            int X = Request.PopWiredInt32();
            int Y = Request.PopWiredInt32();

            //do shit
        }
        private void StartTyping()
        {
            Response.Initialize(361);
            Response.AppendInt32(Client.GetHabbo().UnitId);
            Response.AppendBoolean(true);
            SendRoom();
        }
        private void StopTyping()
        {
            Response.Initialize(361);
            Response.AppendInt32(Client.GetHabbo().UnitId);
            Response.AppendBoolean(false);
            SendRoom();
        }
        private void RotateItem()
        {
            if (RoomUser.CalculateRights(Client.GetHabbo().RoomId, Client.GetHabbo().ID))
            {
                int FurnitureId = Request.PopWiredInt32();
                int FurniX = Request.PopWiredInt32();
                int FurniY = Request.PopWiredInt32();
                int FurniRot = Request.PopWiredInt32();

                //Update furni coords.
                GetFloorItems().getItem(FurnitureId).X = FurniX;
                GetFloorItems().getItem(FurnitureId).Y = FurniY;
                GetFloorItems().getItem(FurnitureId).Rotation = FurniRot;

                //Do the mysql queries
                using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
                {
                    dbClient.ExecuteQuery("UPDATE room_items SET x_axis = '" + FurniX + "' WHERE id = '" + FurnitureId + "'");
                    dbClient.ExecuteQuery("UPDATE room_items SET y_axis = '" + FurniY + "'WHERE id = '" + FurnitureId + "'");
                    dbClient.ExecuteQuery("UPDATE room_items SET rotation = '" + FurniRot + "' WHERE id = '" + FurnitureId + "'");
                }

                if (Client.GetHabbo().ItemUsingID == FurnitureId && Client.GetHabbo().IsOnItem)
                {
                    Client.GetHabbo().UserRotation = GetFloorItems().getItem(FurnitureId).Rotation;
                    MoveUser(true);
                }
                else
                {
                    Client.GetHabbo().IsOnItem = false;
                    MoveUser(true);
                }

                //Send the updated move/rotate
                FloorItems mItem = AleedaEnvironment.GetCache().GetFloorItems().getItem(FurnitureId);
                {
                    Response.Initialize(95);
                    Response.AppendInt32(mItem.ID);
                    Response.AppendInt32(mItem.SpriteID);
                    Response.AppendInt32(mItem.X);
                    Response.AppendInt32(mItem.Y);
                    Response.AppendInt32(mItem.Rotation);
                    Response.AppendString("0.0");
                    Response.AppendInt32(0);
                    Response.AppendRawInt32WithBreak(mItem.Trigger, 2);
                    Response.AppendInt32(-1);
                    Response.AppendInt32(0);
                    SendRoom();
                }
            }
        }
        private void ChangeLook()
        {
            ServerMessage fuseMessage = null;

            string Gender = Request.PopFixedString().ToUpper();
            string Look = Request.PopFixedString();

            Client.GetHabbo().Figure = Look;
            Client.GetHabbo().Gender = Convert.ToChar(Gender);

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("figure", Look);
                dbClient.AddParamWithValue("gender", Gender);
                dbClient.ExecuteQuery("UPDATE users SET figure=@figure,gender=@gender WHERE id = '" + Client.GetHabbo().ID + "'");
            }

            Response.Initialize(266);
            Response.AppendInt32(-1);
            Response.AppendString(Client.GetHabbo().Figure);
            Response.AppendString(Client.GetHabbo().Gender.ToString().ToLower());
            Response.AppendString(Client.GetHabbo().Motto);
            Response.AppendInt32(0); //To do: Achievement score
            SendResponse();

            if (Client.GetHabbo().RoomId != 0)
            {
                fuseMessage = new ServerMessage(266); // "DJ"
                fuseMessage.AppendInt32(Client.GetHabbo().UnitId);
                fuseMessage.AppendString(Client.GetHabbo().Figure);
                fuseMessage.AppendString(Client.GetHabbo().Gender.ToString().ToLower());
                fuseMessage.AppendString(Client.GetHabbo().Motto);
                fuseMessage.AppendInt32(0); //To do: Achievement score
                SendRoom(fuseMessage);
            }
        }
        private void PickUpItem()
        {
            //Remove garbage
            Request.PopWiredInt32();

            int furniID = Request.PopWiredInt32();

            FloorItems mFloor = GetFloorItems().getItem(furniID);
            WallItems mWall = GetWallItems().getItem(furniID);

            uint ServerMessageID = 0;
            string FurnitureType = "";

            if (mFloor == null)
            {
                ServerMessageID = 84;
                FurnitureType = "i";

                //Add item to inventory.
                Client.GetHabbo().AddItem(furniID, mWall.SpriteID, FurnitureType);

                //Get rid of item from cache.
                AleedaEnvironment.GetCache().GetWallItems().deleteItem(furniID);
            }
            else if (mWall == null)
            {
                ServerMessageID = 94;
                FurnitureType = "s";

                //Add item to inventory.
                Client.GetHabbo().AddItem(furniID, mFloor.SpriteID, FurnitureType);

                //Get rid of item from cache.
                AleedaEnvironment.GetCache().GetFloorItems().deleteItem(furniID);
            }

            if (Client.GetHabbo().ItemUsingID != 0)
            {
                Client.GetHabbo().IsOnItem = false;
                MoveUser(true);
            }

            Response.Initialize(ServerMessageID);
            Response.AppendRawInt32(furniID);
            Response.AppendString("");
            Response.AppendBoolean(false);
            SendRoom();
        }
        private void MoveWallItem()
        {
        }
        private void LookAtUser()
        {
            uint UserId = Request.PopWiredUInt32();


        }
        private void ApplyDecoration()
        {
            if (RoomUser.CalculateRights(Client.GetHabbo().RoomId, Client.GetHabbo().ID))
            {
                uint Id = Request.PopWiredUInt32();

                PrivateRooms mRoom = GetPrivateRooms().getRoom(Client.GetHabbo().RoomId);

                foreach (UserInventory mUser in AleedaEnvironment.GetHabboHotel().GetRoomUser().GetItem(Id))
                {
                    if (mUser.Name.Contains("wall"))
                    {
                        int value = int.Parse(mUser.Name.Split('_')[2]);

                        mRoom.Wall = value;

                        Response.Initialize(46); // @n
                        Response.AppendString("wallpaper");
                        Response.Append(value);
                        SendRoom();

                        using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
                        {
                            dbClient.ExecuteQuery("UPDATE private_rooms SET wallpaper = '" + mRoom.Wall + "' WHERE id = '" + Client.GetHabbo().RoomId + "'");
                        }
                    }
                    else if (mUser.Name.Contains("floor"))
                    {
                        int value = int.Parse(mUser.Name.Split('_')[2]);

                        mRoom.Floor = value;

                        Response.Initialize(46); // @n
                        Response.AppendString("floor");
                        Response.Append(value);
                        SendRoom();

                        using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
                        {
                            dbClient.ExecuteQuery("UPDATE private_rooms SET floorpaper = '" + mRoom.Floor + "' WHERE id = '" + Client.GetHabbo().RoomId + "'");
                        }
                    }
                    else if (mUser.Name.Contains("landscape"))
                    {
                        string value = mUser.Name.Split('_')[2];

                        mRoom.LandScape = value;

                        Response.Initialize(46); // @n
                        Response.AppendString("landscape");
                        Response.Append(value);
                        SendRoom();

                        using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
                        {
                            dbClient.ExecuteQuery("UPDATE private_rooms SET landscape = '" + mRoom.LandScape + "' WHERE id = '" + Client.GetHabbo().RoomId + "'");
                        }
                    }

                }
                Client.GetHabbo().RemoveItem((int)Id);
            }
        }
        /// <summary>
        /// Registers request handlers that process room Navigator queries etc.
        /// </summary>
        public void RegisterRoomUser()
        {
            mRequestHandlers[52] = new RequestHandler(ChatHandler);
            mRequestHandlers[55] = new RequestHandler(ShoutHandler);
            mRequestHandlers[94] = new RequestHandler(WaveHandler);
            mRequestHandlers[93] = new RequestHandler(DanceHandler);
            mRequestHandlers[75] = new RequestHandler(PathHandler);
            mRequestHandlers[404] = new RequestHandler(InventoryHandler);
            mRequestHandlers[90] = new RequestHandler(PlaceItem);
            mRequestHandlers[56] = new RequestHandler(WisperHandler);
            mRequestHandlers[73] = new RequestHandler(RotateItem);
            mRequestHandlers[317] = new RequestHandler(StartTyping);
            mRequestHandlers[318] = new RequestHandler(StopTyping);
            mRequestHandlers[44] = new RequestHandler(ChangeLook);
            mRequestHandlers[67] = new RequestHandler(PickUpItem);
            mRequestHandlers[3000] = new RequestHandler(GetPetsInventory);
            mRequestHandlers[3002] = new RequestHandler(PlacePet);
            mRequestHandlers[159] = new RequestHandler(LookAtUser);
            mRequestHandlers[66] = new RequestHandler(ApplyDecoration);
        }
    }
}
