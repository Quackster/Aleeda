using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Aleeda.Core;
using Aleeda.Storage;
using Aleeda.Net.Messages;
using Aleeda.HabboHotel.Client;
using Aleeda.HabboHotel.Habbos;

namespace Aleeda.HabboHotel.Habbos
{
    public class HabboFunctions
    {
        #region Fields
        private GameClient Session;
        private Queue<byte> Bytes;
        #endregion

        public HabboFunctions(GameClient Client)
        {
            this.Session = Client;
        }

        public void DepartFromRoom(bool enterNewRoom)
        {
            Session.GetMessageHandler().GetResponse().Initialize(29); // "@]"
            Session.GetMessageHandler().GetResponse().AppendRawInt32(Session.GetHabbo().UnitId);
            Session.GetMessageHandler().SendRoom();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("The room id [ ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(Session.GetHabbo().RoomId);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("] has been unloaded from the user ( ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(Session.GetHabbo().Username);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" )");

            //Remove the user collected.
            ClientMessageHandler.mRoomList.Remove(Session);

            //Deletes the room (from the cache) if there was only that user in the room.
            Session.GetMessageHandler().GetPrivateRooms().deleteRoom(Session.GetHabbo().RoomId);

            if (enterNewRoom)
            {
                //Go to hotel view;
                Session.GetMessageHandler().GetResponse().Initialize(18); // "@R"
                Session.GetMessageHandler().SendResponse();
            }

            Session.GetHabbo().RoomId = 0;
            Session.GetHabbo().pRoomId = 0;
            Session.SaveUserObject();
        }


        public void ArriveToRoom()
        {

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nThe room id [ ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(Session.GetHabbo().RoomId);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(" ] has been loaded from the user ( ");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(Session.GetHabbo().Username);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" )\n");

            //If the dictionary doesn't contain the room id
            if (!ClientMessageHandler.mRoomList.Contains(Session))
                 ClientMessageHandler.mRoomList.Add(Session);

            //Deletes the users rooms now where the room count is 0 (nullus, zilch, none, nothing)!
            Session.GetMessageHandler().GetPrivateRooms().deleteRoom(Session.GetHabbo().ID);

            Session.GetHabbo().RoomId = Session.GetHabbo().RoomId;
            Session.SaveUserObject();
        }

        /*public bool RequestToMove(int NewX, int NewY)
        {
            bool CanMove = false;
            int UserCount = AleedaEnvironment.GetCache().GetPrivateRooms().UsersInRoomCount(Session.GetHabbo().RoomId);

            if (UserCount < 2)
                CanMove = true;
            else
            {
                foreach (GameClient mClient in ClientMessageHandler.mRoomList)
                {
                    if (mClient.GetHabbo().Username != Session.GetHabbo().Username &&
                        mClient.GetHabbo().RoomId == Session.GetHabbo().RoomId)
                    {
                        if (Session.GetHabbo().ReqX != mClient.GetHabbo().X && Session.GetHabbo().ReqY != mClient.GetHabbo().Y)
                        {
                            Session.GetHabbo().X = NewX;
                            Session.GetHabbo().Y = NewY;
                            CanMove = true;
                        }
                    }
                }
            }
            return CanMove;
        }*/

        public int GetModelX()
        {
            string mStartingPosition;

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("model", Session.GetHabbo().FlatModel);
                mStartingPosition = dbClient.ReadString("SELECT door FROM room_models WHERE id = @model");
            }

            Encoding enc = Encoding.ASCII;

            //Gets the bytes from 'Door'
            byte[] myByteArray = enc.GetBytes(mStartingPosition);

            this.Bytes = new Queue<byte>(myByteArray);

            /*if (mStartingPosition == "" || mStartingPosition == null)
                return 0;
            else*/
                return PopInt();
        }
        public int GetModelY()
        {
            //Clear the byte queue.
            Bytes.Clear();

            string mStartingPosition;

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("model", Session.GetHabbo().FlatModel);
                mStartingPosition = dbClient.ReadString("SELECT door FROM room_models WHERE id = @model");
            }

            Encoding enc = Encoding.ASCII;

            //Gets the bytes from 'Door'
            byte[] myByteArray = enc.GetBytes(mStartingPosition);
            
            this.Bytes = new Queue<byte>(myByteArray);

            //Get rid of the X, aka garbage - no longer needed
            PopInt();

            /*if (mStartingPosition == "" || mStartingPosition == null)
                return 0;
            else*/
                return PopInt();
        }
        public int PopInt()
        {
            int i = 0;
            int result = Aleeda.Specialized.Encoding.WireEncoding.DecodeInt32(this.Bytes.ToArray(), out i);
            while (i > 0)
            {
                this.Bytes.Dequeue();
                i--;
            }
            return result;
        }
    }
}
