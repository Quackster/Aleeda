using System;
using System.Collections.Generic;
using System.Data;
using Aleeda.HabboHotel.Client;
using Aleeda.Net.Messages;
using Aleeda.Storage;

namespace Aleeda.HabboHotel.Cache
{
    public class RoomBots
    {
        #region Fields
        public int botID;
        public int botRoomID;
        public string botName;
        public string botMotto;
        public string botFigure;
        public int botX;
        public int botY;
        public int botRotation;
        public string botMessages;
        public List<RoomBots> roomBots;

        private int mbotVirtualId;
        private bool mIsKicked;

        public int botVirtualId
        {
            get { return mbotVirtualId; }
            set { mbotVirtualId = value; }
        }
        public bool IsKicked
        {
            get { return mIsKicked; }
            set { mIsKicked = value; }
        }
        #endregion

        #region Constructors
        public RoomBots()
        {
            //Initalize the list for room bots
            roomBots = new List<RoomBots>();

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                DataTable botTable = dbClient.ReadDataTable("SELECT * FROM room_bots;");

                foreach (DataRow row in botTable.Rows)
                {
                    roomBots.Add(new RoomBots((int)row["id"], (int)row["roomId"], (int)row["virtualId"], (string)row["name"], (string)row["motto"], (string)row["figure"], (int)row["x"], (int)row["y"], (int)row["rotation"], (string)row["messages"], false));
                }
            }
            //Console.WriteLine("Initializing RoomId Bot(s).");
            //Console.WriteLine("Successfully cached [ " + roomBots.Count + " ] bots.\n");
        }
        public RoomBots(int mbotID, int mbotRoomID, int mbotVirtualId, string mbotName, string mbotMotto, string mbotFigure, int mbotX, int mbotY, int mbotRotation, string mbotMessages, bool mIsKicked)
        {
            this.botID = mbotID;
            this.botRoomID = mbotRoomID;
            this.botVirtualId = mbotVirtualId;
            this.botName = mbotName;
            this.botMotto = mbotMotto;
            this.botFigure = mbotFigure;
            this.botX = mbotX;
            this.botY = mbotY;
            this.botRotation = mbotRotation;
            this.botMessages = mbotMessages;
            this.IsKicked = mIsKicked;
        }
        #endregion

        #region Methods
        public int RoomBotCounts(int RoomId)
        {
            int i = 0;
            foreach (RoomBots mBot in roomBots)
            {
                if (mBot.botRoomID == RoomId && mBot.IsKicked == false)
                {
                    ++i;
                }
            }
            return i;
        }
        public void RoomReloadBots()
        {
            //Initalize the list for room bots
            roomBots = new List<RoomBots>();

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                DataTable botTable = dbClient.ReadDataTable("SELECT * FROM room_bots;");

                foreach (DataRow row in botTable.Rows)
                {
                    roomBots.Add(new RoomBots((int)row["id"], (int)row["roomId"], (int)row["virtualId"], (string)row["name"], (string)row["motto"], (string)row["figure"], (int)row["x"], (int)row["y"], (int)row["rotation"], (string)row["messages"], true));
                }
            }
        }
        public void ReloadBotID(int i)
        {
            foreach (RoomBots mBot in roomBots)
            {
                if (mBot.botID == i)
                {
                    roomBots.Remove(mBot);
                }
            }
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                DataTable botTable = dbClient.ReadDataTable("SELECT * FROM room_bots WHERE id = '" + i + "'");

                foreach (DataRow row in botTable.Rows)
                {
                    roomBots.Add(new RoomBots((int)row["id"], (int)row["roomId"], (int)row["virtualId"], (string)row["name"], (string)row["motto"], (string)row["figure"], (int)row["x"], (int)row["y"], (int)row["rotation"], (string)row["messages"], true));
                }
            }
        }
        public void Reload()
        {
            roomBots.Clear();
            RoomReloadBots();
        }
        public bool RequestToMove(GameClient Session, int NewX, int NewY)
        {
            bool CanMove = false;

            if (RoomBotCounts(Session.GetHabbo().RoomId) < 1)
            {
                Session.GetHabbo().X = NewX;
                Session.GetHabbo().Y = NewY;
                CanMove = true;
            }
            else
            {
                foreach (RoomBots mBot in roomBots)
                {
                    if (mBot.botRoomID == Session.GetHabbo().RoomId && mBot.IsKicked == false)
                    {
                        string NewCoord = wireEncoding.encodeVL64(mBot.botX) + wireEncoding.encodeVL64(mBot.botY) + "0.0";

                        if (Session.GetHabbo().ReqX != mBot.botX && Session.GetHabbo().ReqY != mBot.botY)
                        {
                            Session.GetHabbo().X = NewX;
                            Session.GetHabbo().Y = NewY;
                            CanMove = true;
                        }

                    }
                }
            }
            return CanMove;
        }
        public RoomBots getBotWithVirtualID(int id)
        {
            foreach (RoomBots mBot in roomBots)
            {
                if (mBot.botVirtualId == id)
                {
                    return mBot;
                }
            }
            return null;
        }
        #endregion

        #region Serialize
        public void LoadBots(int RoomId, ServerMessage Message)
        {
            foreach (RoomBots mBot in roomBots)
            {
                if (mBot.botRoomID == RoomId && mBot.IsKicked == false)
                {
                    Message.AppendInt32(mBot.botID);
                    Message.AppendString(mBot.botName);
                    Message.AppendString(mBot.botMotto);
                    Message.AppendString(mBot.botFigure);
                    Message.AppendInt32(mBot.botVirtualId);
                    Message.AppendInt32(mBot.botX);
                    Message.AppendInt32(mBot.botY);
                    Message.AppendString("0.0");
                    Message.AppendInt32(4);
                    Message.AppendInt32(3);
                }
            }
        }
        public void LoadStatus(int RoomId, ServerMessage Message)
        {
            foreach (RoomBots mBot in roomBots)
            {
                if (mBot.botRoomID == RoomId && mBot.IsKicked == false)
                {
                    Message.AppendInt32(mBot.botVirtualId);
                    Message.AppendInt32(mBot.botX);
                    Message.AppendInt32(mBot.botY);
                    Message.AppendString("0.0");
                    Message.AppendInt32(mBot.botRotation);
                    Message.AppendInt32(mBot.botRotation);
                    Message.AppendString("/flatctrl useradmin//");
                }
            }
        }
        public void LoadAnyBotChats(string user, int RoomId)
        {
            ServerMessage Response = new ServerMessage();

            foreach (RoomBots mBot in roomBots)
            {
                if (mBot.botRoomID == RoomId && mBot.IsKicked)
                {
                    foreach (string botMsg in mBot.botMessages.Split(','))
                    {
                        Response.Initialize(25); // "@Y"
                        Response.AppendInt32(mBot.botVirtualId);
                        Response.AppendString(botMsg);
                        Response.AppendInt32(0);
                        Send(user, Response);
                    }
                }
            }
        }
        public void Send(string user, ServerMessage Message)
        {
            foreach (Client.GameClient mClient in Client.ClientMessageHandler.mRoomList)
            {
                if (mClient.GetHabbo().Username == user)
                {
                    mClient.GetConnection().SendMessage(Message);
                }
            }
        }
        #endregion
    }
}
