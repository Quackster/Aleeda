using System;
using System.Collections.Generic;
using System.Text;

using Aleeda.Core;
using Aleeda.Net.Messages;
using Aleeda.HabboHotel.Client;
using Aleeda.HabboHotel.Habbos;

namespace Aleeda.HabboHotel.Rooms.User
{
    public class Leave
    {
        public static void LeaveRoom(GameClient Session, bool enterNewRoom)
        {
            ServerMessage fuseMessage = null;

            fuseMessage = new ServerMessage(29); // "@]"
            fuseMessage.AppendRawInt32(Session.GetHabbo().UnitId);
            new ClientMessageHandler(Session).SendRoom(fuseMessage);

            Console.WriteLine("The room id [ " + Session.GetHabbo().RoomId + " ] has been unloaded from the user ( " + Session.GetHabbo().Username + " )");

            //Remove the user collected.
            ClientMessageHandler.mRoomList.Remove(Session);

            if (enterNewRoom)
            {
                //Go to hotel view;
                fuseMessage = new ServerMessage(18); // "@R"
                Session.GetConnection().SendMessage(fuseMessage);
            }
            Session.GetHabbo().RoomId = 0;
            Session.SaveUserObject();
        }
    }
    public class Funcs
    {
        public static bool RequestToMove(GameClient Client, int NewX, int NewY)
        {
            bool CanMove = false;
            int UserCount = AleedaEnvironment.GetCache().GetPrivateRooms().UsersInRoomCount(Client.GetHabbo().RoomId);

            if (UserCount == 1)
                CanMove = true;
            else
            {
                foreach (GameClient mClient in ClientMessageHandler.mRoomList)
                {
                    if (mClient.GetHabbo().Username != Client.GetHabbo().Username &&
                        mClient.GetHabbo().RoomId == Client.GetHabbo().RoomId)
                    {
                        if (mClient.GetHabbo().X != Client.GetHabbo().ReqX && mClient.GetHabbo().Y != Client.GetHabbo().ReqY)
                        {
                            Client.GetHabbo().X = NewX;
                            Client.GetHabbo().Y = NewY;
                            CanMove = true;
                        }
                    }
                }
            }
            return CanMove;
        }
    }
}
