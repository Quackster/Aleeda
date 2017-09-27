using System;
using System.Data;
using System.Text;
using Aleeda.Core;
using Aleeda.HabboHotel.Cache;
using Aleeda.HabboHotel.Habbos;
using Aleeda.HabboHotel.Rooms;
using Aleeda.HabboHotel.Rooms.User;
using Aleeda.Storage;

namespace Aleeda.HabboHotel.Client
{
    public partial class ClientMessageHandler
    {
        private void RequestRoom()
        {
            if (mRoomList.Contains(Client))
            {
                Client.GetHabbo().Functions().DepartFromRoom(false);
            }

            PrivateRooms mRoom = AleedaEnvironment.GetCache().GetPrivateRooms().getRoom(Request.PopWiredInt32());
            {
                if (mRoom == null)
                {
                    return;
                }
                if (mBoolean.CheckRoomBanned(Client.GetHabbo().Username, mRoom.ID) == true)
                {
                    Response.Initialize(224); // "C`"
                    SendResponse();

                    Response.Initialize(18); // "@R"
                    SendResponse();
                }
                else
                {
                    //Add the room id it's trying to enter!
                    Client.GetHabbo().RoomId = mRoom.ID;

                    if (mRoom.Status == 0)
                    {
                        AllowRoomEntry();
                    }
                    else if ((uint)mRoom.OwnerID == Client.GetHabbo().ID)
                    {
                        AllowRoomEntry();
                    }
                    else if (mRoom.Status == 1)
                    {
                        Response.Initialize(91);
                        Response.AppendString("");
                        SendResponse();

                        Response.Initialize(91);
                        Response.AppendString(Client.GetHabbo().Username);
                        SendToUsersWithRights();
                    }
                    if (AleedaEnvironment.GetHabboHotel().GetHabbos().GetHabbo((uint)mRoom.OwnerID).Username == Client.GetHabbo().Username)
                    {
                        Response.Initialize(42); // "@j"
                        SendResponse();

                        Response.Initialize(47); // "@o"
                        SendResponse();
                    }
                }
                if (mBoolean.CheckRoomRights(Client.GetHabbo().Username, mRoom.ID))
                {
                    Response.Initialize(42); // "@j"
                    SendResponse();

                    Response.Initialize(47); // "@o"
                    SendResponse();

                }
            }
        }
        private void AllowRoomEntry()
        {
            //Says the user has entered the room! woo!
            Client.GetHabbo().Functions().ArriveToRoom();

            PrivateRooms mRoom = AleedaEnvironment.GetCache().GetPrivateRooms().getRoom(Client.GetHabbo().RoomId);
            {
                if (mRoom == null)
                {
                    return;
                }

                Response.Initialize(19); // "@S"
                SendResponse();

                Response.Initialize(166); // "Bf"
                Response.Append("/client/private/");
                Response.Append(mRoom.ID);
                Response.Append("/id");
                SendResponse();

                Response.Initialize(69); // "AE"
                Response.AppendString(mRoom.Model);
                Response.AppendInt32(mRoom.ID);
                SendResponse();

                //Response.Initialize(345); // "EY"
                Response.AppendInt32(mRoom.Rating);
                SendResponse();

                //Sets the room model the user is currently in.
                Client.GetHabbo().FlatModel = mRoom.Model;

                //Sets the type public/private
                Client.GetHabbo().FlatType = "private";

                //Make a random unit id
                Client.GetHabbo().UnitId = AleedaEnvironment.GenerateRandomNum(100, 1000);

                //Save the user
                Client.SaveUserObject();

                if (mRoom.Wall != 0)
                {
                    Response.Initialize(46); // @n
                    Response.AppendString("wallpaper");
                    Response.Append(mRoom.Wall);
                    SendResponse();
                }
                if (mRoom.Floor != 0)
                {
                    Response.Initialize(46); // @n
                    Response.AppendString("floor");
                    Response.Append(mRoom.Floor);
                    SendResponse();
                }

                Response.Initialize(46); // @n
                Response.AppendString("landscape");
                Response.Append(mRoom.LandScape);
                SendResponse();
            }
        }
        private void AnswerDoorbell()
        {
            string Name = Request.PopFixedString();
            byte[] Result = Request.ReadBytes(1);

            GameClient sClient = AleedaEnvironment.GetHabboHotel().GetClients().GetClientOfHabbo(Name);

            if (Client == null)
            {
                return;
            }

            Boolean Accepted = (Result[0] == Convert.ToByte(65));
            Console.WriteLine(Accepted);

            if (Accepted)
            {
                sClient.GetMessageHandler().GetResponse().Initialize(41);
                sClient.GetMessageHandler().SendResponse();
            }
            else
            {
                sClient.GetMessageHandler().GetResponse().Initialize(131);
                sClient.GetMessageHandler().SendResponse();
            }
        }
        /// <summary>
        /// 390 - "FF"
        /// </summary>
        private void GetRoomEntryDataMessageComposer()
        {
            try
            {
                Client.GetHabbo().X = Client.GetHabbo().Functions().GetModelX();
                Client.GetHabbo().Y = Client.GetHabbo().Functions().GetModelY();
            }
            catch
            {
                Client.GetHabbo().X = 0;
                Client.GetHabbo().Y = 0;
            }

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                 string map;
                 try
                 {
                     map = dbClient.ReadString("SELECT heightmap FROM room_models WHERE id = '" + Client.GetHabbo().FlatModel + "'");
                 }
                 catch
                 {
                     map = "";
                 }
                Response.Initialize(31); // "@_"
                Response.AppendString(map.Replace("/", "\r"));
                SendResponse();

                Response.Initialize(470); // "GV"
                Response.AppendString(map.Replace("/", "\r"));
                SendResponse();

                CatalogIndexMessageEvent();
            }

        }

        #region Nothing packets
        private void ProccessPacketCf()
        {
            Response.Initialize(309); // "Du"
            Response.AppendInt32(0);
            SendResponse();
        }
        private void ProccessPacketCW()
        {
            Response.Initialize(297); // "Di"
            Response.AppendInt32(0);
            SendResponse();
        }
        #endregion
        private void CatalogIndexMessageEvent()
        {
            if (Client.GetHabbo().FlatType == "private")
            {
                PrivateRooms mRoom = AleedaEnvironment.GetCache().GetPrivateRooms().getRoom(Client.GetHabbo().RoomId);

                if (mRoom == null)
                {
                    return;
                }

                #region Bots
                if (AleedaEnvironment.GetCache().GetRoomBots().RoomBotCounts(Client.GetHabbo().RoomId) != 0)
                {
                    AleedaEnvironment.GetCache().GetRoomBots().LoadAnyBotChats(Client.GetHabbo().Username, Client.GetHabbo().RoomId);
                }
                #endregion

                Response.Initialize(45);
                GetWallItems().SerializeWallDisplay(GetResponse(), Client.GetHabbo().RoomId);
                SendResponse();

                Response.Initialize(32);
                GetFloorItems().Serialize(GetResponse(), Client.GetHabbo().RoomId);
                SendResponse();

                Response.Initialize(471); // "GW"
                Serialize.RoomEntryInfo(Client, AleedaEnvironment.GetHabboHotel().GetHabbos().GetHabbo((UInt32)mRoom.OwnerID).Username, Response);
                SendResponse();

                Response.Initialize(28); // "@\"
                Serialize.SerializeUsers(true, Client.GetHabbo().RoomId, Response);
                SendRoom();

                Response.Initialize(34); // "@b"
                Serialize.SerializeStatus(true, Client.GetHabbo().RoomId, Response);
                SendRoom();
            }
            else if (Client.GetHabbo().FlatType == "public")
            {
                Response.Initialize(471); // "GW"
                Serialize.RoomEntryInfo(Client, "", Response);
                SendResponse();

                Response.Initialize(30); // "@^"
                Response.Append(Select.GetPublicRoomItems(Client.GetHabbo().FlatModel));
                SendResponse();

                try
                {
                    Response.Initialize(28); // "@\"
                    Serialize.SerializeUsers(false, Client.GetHabbo().pRoomId, GetResponse());
                    SendRoom();

                    Response.Initialize(34); // "@b"
                    Serialize.SerializeStatus(false, Client.GetHabbo().RoomId, GetResponse());
                    SendRoom();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
        }
        private void GetRoomSettingsMessageComposer()
        {
            PrivateRooms mRoom = AleedaEnvironment.GetCache().GetPrivateRooms().getRoom(Client.GetHabbo().RoomId);

            if (mRoom == null)
            {
                return;
            }

            Response.Initialize(465); // "GQ"
            Response.AppendInt32(mRoom.ID);
            Response.AppendString(mRoom.Name);
            Response.AppendString(mRoom.Description);
            Response.AppendInt32(mRoom.Status);
            Response.AppendInt32(mRoom.Category);
            Response.AppendInt32(mRoom.MaxVisitors);
            Response.AppendInt32(25);
            Response.AppendInt32(AleedaEnvironment.GetCache().GetPrivateRooms().GetTagCount(mRoom.ID)); //TAG count
            AleedaEnvironment.GetCache().GetPrivateRooms().AppendTags(Response, mRoom.ID);
            AleedaEnvironment.GetCache().GetPrivateRooms().SerializeUsersWithRights(Response, mRoom.ID);
            Response.AppendInt32(AleedaEnvironment.GetCache().GetPrivateRooms().RoomWithRightsCount(mRoom.ID));
            Response.AppendInt32(mRoom.PetsAllowed); // allows pets in room - pet system lacking, so always off
            Response.AppendBoolean(mRoom.PetsEatOtherFood); // allows pets to eat your food - pet system lacking, so always off
            Response.AppendBoolean(true);
            Response.AppendBoolean(false); //hidewalls
            Response.AppendInt32(0);
            Response.AppendInt32(0);
            SendResponse();

        }
        private void GetGuestRoomMessageComposer()
        {
            PrivateRooms mRoom = AleedaEnvironment.GetCache().GetPrivateRooms().getRoom(Client.GetHabbo().RoomId);

            if (mRoom == null)
            {
                return;
            }

            Response.Initialize(454); // "GF"
            Response.AppendInt32(0);
            Response.AppendInt32(mRoom.ID);
            Response.AppendBoolean(true);
            Response.AppendString(mRoom.Name);
            Response.AppendString(AleedaEnvironment.GetHabboHotel().GetHabbos().GetHabbo((uint)mRoom.OwnerID).Username);
            Response.AppendInt32(mRoom.Status);
            Response.AppendInt32(AleedaEnvironment.GetCache().GetPrivateRooms().UsersInRoomCount(mRoom.ID));
            Response.AppendInt32(mRoom.MaxVisitors);
            Response.AppendString(mRoom.Description);
            Response.AppendBoolean(true);
            Response.AppendBoolean(true); //Trading set on by default.
            Response.AppendInt32(mRoom.Rating);
            Response.AppendInt32(mRoom.Category);
            Response.AppendString("");
            Response.AppendInt32(AleedaEnvironment.GetCache().GetPrivateRooms().GetTagCount(mRoom.ID)); //TAG count
            AleedaEnvironment.GetCache().GetPrivateRooms().AppendTags(Response, mRoom.ID);
            //Response.Append(mRoom.Thumbnail);
            SendResponse();

        }
        private void QuitMessageComposer()
        {
            //Make sure the user offically leaves the room
            Client.GetHabbo().Functions().DepartFromRoom(true);
        }
        private void ChangeMotto()
        {
            string newMotto = Request.PopFixedString();

            Response.Initialize(266); // "DJ"
            Response.AppendInt32(Client.GetHabbo().UnitId); //virtual id (unit id)
            Response.AppendString(Client.GetHabbo().Figure);
            Response.AppendString(Client.GetHabbo().Gender.ToString().ToLower());
            Response.AppendString(newMotto);
            Response.AppendInt32(0); //Achievement score
            SendRoom();

            Client.GetHabbo().Motto = newMotto;
            Client.SaveUserObject();

        }
        private void TriggerItem()
        {
            if (RoomUser.CalculateRights(Client.GetHabbo().RoomId, Client.GetHabbo().ID))
            {
                int furniID = Request.PopWiredInt32();

                FloorItems mFloor = AleedaEnvironment.GetCache().GetFloorItems().getItem(furniID);
                WallItems mWall = AleedaEnvironment.GetCache().GetWallItems().getItem(furniID);

                if (mFloor == null)
                {
                    Response.Initialize(85); // "AU"
                    Response.AppendString(mWall.ID + "");
                    Response.AppendInt32(mWall.SpriteID);
                    Response.AppendString(mWall.Wall);
                    if (mWall.Trigger == 0)
                    {
                        Response.Append(1);
                        Update.UpdateFurniTrigger(furniID, 1);

                        //Set the trigger
                        AleedaEnvironment.GetCache().GetWallItems().getItem(mWall.ID).Trigger = 1;
                    }
                    else
                    {
                        Response.Append(0);
                        Update.UpdateFurniTrigger(furniID, 0);

                        //Set the trigger
                        AleedaEnvironment.GetCache().GetWallItems().getItem(mWall.ID).Trigger = 0;
                    }
                    SendRoom();
                }
                else if (mWall == null)
                {
                    Response.Initialize(88); // "AX"
                    Response.Append(furniID);
                    Response.AppendChar(2);
                    if (mFloor.Trigger == 0)
                    {
                        Response.Append(1);
                        Update.UpdateFurniTrigger(furniID, 1);

                        //Set the trigger
                        AleedaEnvironment.GetCache().GetFloorItems().getItem(mFloor.ID).Trigger = 1;
                    }
                    else
                    {
                        Response.Append(0);
                        Update.UpdateFurniTrigger(furniID, 0);

                        //Set the trigger
                        AleedaEnvironment.GetCache().GetFloorItems().getItem(mFloor.ID).Trigger = 0;
                    }
                    Response.AppendChar(2);
                    SendRoom();

                }
            }
        }
        private void DeleteRoom()
        {
            int Id = Request.PopWiredInt32();

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("DELETE FROM room_items WHERE mID = '" + Id + "'");
                dbClient.ExecuteQuery("DELETE FROM room_bans WHERE roomid = '" + Id + "'");
                dbClient.ExecuteQuery("DELETE FROM room_rights WHERE roomid = '" + Id + "'");
                dbClient.ExecuteQuery("DELETE FROM private_rooms WHERE id = '" + Id + "'");

                //Update Users ID when pressing the x button on navigator
                Client.GetHabbo().RoomId = 0;
                Client.SaveUserObject();

                //Make sure the user offically leaves the room
                Leave.LeaveRoom(Client, true);

                //Delete room from cache
                AleedaEnvironment.GetCache().GetPrivateRooms().deleteRoom(Id);

                this.GetOwnGuestRoom();
            }
        }
        private void SaveRoomIcon()
        {
            //Remove crap
            Request.PopWiredInt32();

            string Background = Request.PopEncodeInt32();
            string TopLayer = Request.PopEncodeInt32();

            //Save the thumbnail
            AleedaEnvironment.GetCache().GetPrivateRooms().getRoom(Client.GetHabbo().RoomId).Thumbnail = Background + TopLayer + "HH";

            GetResponse().Initialize(457);
            GetResponse().AppendInt32(Client.GetHabbo().RoomId);
            GetResponse().AppendBoolean(true);
            SendResponse();

            GetResponse().Initialize(456);
            GetResponse().AppendInt32(Client.GetHabbo().RoomId);
            SendResponse();

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("UPDATE private_rooms SET thumbnail = '" + Background + TopLayer + "HH' WHERE id = '" + Client.GetHabbo().RoomId + "'");
            }
        }
        private void NewHomeRoom()
        {
            Client.GetHabbo().HomeRoom = Request.PopWiredInt32();

            Response.Initialize(455);
            Response.AppendInt32(Client.GetHabbo().HomeRoom);
            SendResponse();

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("UPDATE users SET homeRoom = '" + Client.GetHabbo().HomeRoom + "' WHERE id = '" + Client.GetHabbo().ID + "'");
            }
        }
        private void KickUser()
        {
            int VirtualId = Request.PopWiredInt32();

            Response.Initialize(29); // "@]"
            Response.AppendRawInt32(VirtualId);
            SendRoom();

            if (AleedaEnvironment.GetCache().GetRoomBots().getBotWithVirtualID(VirtualId) != null)
            {
                AleedaEnvironment.GetCache().GetRoomBots().getBotWithVirtualID(VirtualId).IsKicked = true;
            }
            else
            {
                foreach (GameClient mClient in mRoomList)
                {
                    if (mClient.GetHabbo().RoomId == Client.GetHabbo().RoomId && mClient.GetHabbo().UnitId == VirtualId)
                    {
                        Response.Initialize(18);
                        mClient.GetMessageHandler().SendResponse();
                    }
                }
            }
        }
        public FloorItems GetFloorItems()
        {
            return AleedaEnvironment.GetCache().GetFloorItems();
        }
        public WallItems GetWallItems()
        {
            return AleedaEnvironment.GetCache().GetWallItems();
        }
        public PrivateRooms GetPrivateRooms()
        {
            return AleedaEnvironment.GetCache().GetPrivateRooms();
        }
        public RoomBots GetRoomBots()
        {
            return AleedaEnvironment.GetCache().GetRoomBots();
        }
        /// <summary>
        /// Registers request handlers that process room Navigator queries etc.
        /// </summary>

        public void RegisterRoom()
        {
            mRequestHandlers[386] = new RequestHandler(SaveRoomIcon);
            mRequestHandlers[391] = new RequestHandler(RequestRoom);
            mRequestHandlers[230] = new RequestHandler(ProccessPacketCf);
            mRequestHandlers[215] = new RequestHandler(ProccessPacketCW);
            mRequestHandlers[126] = new RequestHandler(CatalogIndexMessageEvent);
            mRequestHandlers[400] = new RequestHandler(GetRoomSettingsMessageComposer);
            mRequestHandlers[390] = new RequestHandler(GetRoomEntryDataMessageComposer);
            mRequestHandlers[385] = new RequestHandler(GetGuestRoomMessageComposer);
            mRequestHandlers[53] = new RequestHandler(QuitMessageComposer);
            mRequestHandlers[484] = new RequestHandler(ChangeMotto);
            mRequestHandlers[393] = new RequestHandler(TriggerItem);
            mRequestHandlers[392] = new RequestHandler(TriggerItem);
            mRequestHandlers[23] = new RequestHandler(DeleteRoom);
            mRequestHandlers[384] = new RequestHandler(NewHomeRoom);
            mRequestHandlers[98] = new RequestHandler(AnswerDoorbell);
            mRequestHandlers[441] = new RequestHandler(KickUser);
        }
    }
}
