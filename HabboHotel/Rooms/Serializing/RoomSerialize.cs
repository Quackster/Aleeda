using System;
using System.Collections.Generic;
using System.Data;
using Aleeda.Core;
using Aleeda.HabboHotel.Cache;
using Aleeda.HabboHotel.Rooms.User;
using Aleeda.HabboHotel.Client;
using Aleeda.Net.Messages;
using Aleeda.Storage;

namespace Aleeda.HabboHotel.Rooms
{
    public class RoomSerialize
    {
        public int RoomCount(int roomid)
        {
            int i = 0;
            foreach (GameClient Session in ClientMessageHandler.mRoomList)
            {
                if (Session.GetHabbo().RoomId == roomid)
                {
                    i++;
                }
            }
            return i;
        }
        public void SerializeUsers(bool IsPrivate, int mRoomId, ServerMessage fuseMessage)
        {
            if (IsPrivate)
            {
                int UsersInRoom = 0;

                if (GetRoomBots().RoomBotCounts(mRoomId) != 0)
                {
                    UsersInRoom = RoomCount(mRoomId);
                    int BotCount = GetRoomBots().RoomBotCounts(mRoomId);

                    //Add up bot and users count
                    UsersInRoom = BotCount += UsersInRoom;
                }
                else
                {
                    UsersInRoom = RoomCount(mRoomId);
                }
                fuseMessage.AppendInt32(UsersInRoom);
                foreach (GameClient Session in ClientMessageHandler.mRoomList)
                {
                    if (Session.GetHabbo().RoomId == mRoomId)
                    {
                        fuseMessage.AppendUInt32(Session.GetHabbo().ID);
                        fuseMessage.AppendString(Session.GetHabbo().Username);
                        fuseMessage.AppendString(Session.GetHabbo().Motto);
                        fuseMessage.AppendString(Session.GetHabbo().Figure);
                        fuseMessage.AppendInt32(Session.GetHabbo().UnitId);
                        fuseMessage.AppendInt32(Session.GetHabbo().X);
                        fuseMessage.AppendInt32(Session.GetHabbo().Y);
                        fuseMessage.AppendString("0.0");
                        fuseMessage.AppendInt32(2);
                        fuseMessage.AppendInt32(1);
                        fuseMessage.AppendString(Session.GetHabbo().Gender.ToString().ToLower());
                        fuseMessage.AppendInt32(-1);
                        fuseMessage.AppendInt32(-1);
                        fuseMessage.AppendInt32(-1);
                        fuseMessage.AppendString(string.Empty);
                        fuseMessage.AppendInt32(0);
                    }
                } //Do room bots
                if (GetRoomBots().RoomBotCounts(mRoomId) != 0)
                {
                    AleedaEnvironment.GetCache().GetRoomBots().LoadBots(mRoomId, fuseMessage);
                }
            }
            else
            {
                int i = 0;
                foreach (GameClient Session in ClientMessageHandler.mRoomList)
                {
                    if (Session.GetHabbo().pRoomId == mRoomId)
                    {
                        i++;
                    }
                }
                fuseMessage.AppendInt32(i);
                foreach (GameClient Session in ClientMessageHandler.mRoomList)
                {
                    if (Session.GetHabbo().pRoomId == mRoomId)
                    {
                        fuseMessage.AppendUInt32(Session.GetHabbo().ID);
                        fuseMessage.AppendString(Session.GetHabbo().Username);
                        fuseMessage.AppendString(Session.GetHabbo().Motto);
                        fuseMessage.AppendString(Session.GetHabbo().Figure);
                        fuseMessage.AppendInt32(Session.GetHabbo().UnitId);
                        fuseMessage.AppendInt32(Session.GetHabbo().X);
                        fuseMessage.AppendInt32(Session.GetHabbo().Y);
                        fuseMessage.AppendString("0.0");
                        fuseMessage.AppendInt32(2);
                        fuseMessage.AppendInt32(1);
                        fuseMessage.AppendString(Session.GetHabbo().Gender.ToString().ToLower());
                        fuseMessage.AppendInt32(-1);
                        fuseMessage.AppendInt32(-1);
                        fuseMessage.AppendInt32(-1);
                        fuseMessage.AppendString(string.Empty);
                        fuseMessage.AppendInt32(0);
                    }
                } //Do room bots
            }
        }
        public void SerializeStatus(bool IsPrivate, int mRoomId, ServerMessage fuseMessage)
        {
            if (IsPrivate)
            {
                int UsersInRoom = 0;

                UsersInRoom = GetPrivateRooms().UsersInRoomCount(mRoomId);
                int BotCount = GetRoomBots().RoomBotCounts(mRoomId);

                UsersInRoom = BotCount += UsersInRoom;

                fuseMessage.AppendInt32(BotCount += UsersInRoom);
                foreach (GameClient Session in ClientMessageHandler.mRoomList)
                {
                    if (Session.GetHabbo().RoomId == mRoomId)
                    {
                        fuseMessage.AppendInt32(Session.GetHabbo().UnitId);
                        fuseMessage.AppendInt32(Session.GetHabbo().X);
                        fuseMessage.AppendInt32(Session.GetHabbo().Y);
                        fuseMessage.AppendString("0.0");
                        fuseMessage.AppendInt32(Session.GetHabbo().UserRotation);
                        fuseMessage.AppendInt32(Session.GetHabbo().UserRotation);
                        fuseMessage.AppendString("/flatctrl useradmin//");
                    }

                }

                if (GetRoomBots().RoomBotCounts(mRoomId) != 0)
                {
                    //Do room bots
                    AleedaEnvironment.GetCache().GetRoomBots().LoadStatus(mRoomId, fuseMessage);
                }
            }
            else
            {
                int i = 0;
                foreach (GameClient Session in ClientMessageHandler.mRoomList)
                {
                    if (Session.GetHabbo().pRoomId == mRoomId)
                    {
                        ++i;
                    }
                }
                fuseMessage.AppendInt32(i);
                foreach (GameClient Session in ClientMessageHandler.mRoomList)
                {
                    if (Session.GetHabbo().RoomId == mRoomId)
                    {
                        fuseMessage.AppendInt32(Session.GetHabbo().UnitId);
                        fuseMessage.AppendInt32(Session.GetHabbo().X);
                        fuseMessage.AppendInt32(Session.GetHabbo().Y);
                        fuseMessage.AppendString("0.0");
                        fuseMessage.AppendInt32(Session.GetHabbo().UserRotation);
                        fuseMessage.AppendInt32(Session.GetHabbo().UserRotation);
                        fuseMessage.AppendString("/flatctrl useradmin//");
                    }
                }
            }
        }
        public void RoomEntryInfo(GameClient Session, string mOwner, ServerMessage fuseMessage)
        {
            if (Session.GetHabbo().FlatType == "private")
            {
                fuseMessage.AppendInt32(1);
                fuseMessage.AppendInt32(Session.GetHabbo().RoomId);
                if (mOwner == Session.GetHabbo().Username)
                    fuseMessage.AppendBoolean(true);
                else
                    fuseMessage.AppendBoolean(false);
            }
            else if (Session.GetHabbo().FlatType == "public")
            {
                fuseMessage.AppendBoolean(false);
                fuseMessage.AppendString(Session.GetHabbo().FlatModel);
                fuseMessage.AppendBoolean(false);
            }
        }
        public void SendRoom(ServerMessage msg, int RoomId)
        {
            foreach (GameClient mClient in ClientMessageHandler.mRoomList)
            {
                if (mClient.GetHabbo().RoomId == RoomId)
                {
                    mClient.GetConnection().SendMessage(msg);
                }
            }
        }
        public RoomBots GetRoomBots()
        {
            return AleedaEnvironment.GetCache().GetRoomBots();
        }
        public PrivateRooms GetPrivateRooms()
        {
            return AleedaEnvironment.GetCache().GetPrivateRooms();
        }
    }
}
