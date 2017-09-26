using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Aleeda.Core;
using Aleeda.HabboHotel;
using Aleeda.HabboHotel.Rooms;
using Aleeda.Net.Messages;
using Aleeda.Specialized.Encoding;
using Aleeda.Storage;

namespace Aleeda.HabboHotel.Client
{
    public partial class ClientMessageHandler
    {
        private void Handler388()
        {
            if (mRoomList.Contains(Client))
            {
                Client.GetHabbo().Functions().DepartFromRoom(false);
            }

            int Id = Request.PopWiredInt32();

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("id", Id);
                string CCT = dbClient.ReadString("SELECT ccts FROM public_rooms WHERE id = @id;");

                Response.Initialize(453);
                Response.AppendInt32(Id);
                Response.AppendString(CCT);
                Response.AppendInt32(Id);
                SendResponse();
            }
        }
        private void OpenConnectionMessageComposer()
        {
            //Remove garbage
            Request.PopWiredInt32();

            //Public room ID
            int id = Request.PopWiredInt32();

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("id", id);
                string Model = dbClient.ReadString("SELECT model FROM public_rooms WHERE id = @id");

                Response.Initialize(69); // "AE"
                Response.AppendString(Model);
                Response.AppendBoolean(false);
                SendResponse();

                //Sets users current room id
                Client.GetHabbo().pRoomId = id;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\nThe *public* room id [ ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(Client.GetHabbo().pRoomId);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(" ] has been loaded from the user ( ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(Client.GetHabbo().Username);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" )\n");

                //If the dictionary doesn't contain the room id
                if (!ClientMessageHandler.mRoomList.Contains(Client))
                    ClientMessageHandler.mRoomList.Add(Client);

                //Sets the room model the user is currently in.
                Client.GetHabbo().FlatModel = Model;

                //Sets the type public/private
                Client.GetHabbo().FlatType = "public";

                //Make a random unit id
                Client.GetHabbo().UnitId = AleedaEnvironment.GenerateRandomNum(100, 1000);

                //Save the user
                //Client.SaveUserObject();

                Response.Initialize(166); // "Bf"
                Response.AppendString("/client/public/" + Model + "/0");
                SendResponse();
            }
        }

        /// <summary>
        /// Registers request handlers that process room Navigator queries etc.
        /// </summary>

        public void RegisterPublicRooms()
        {
            mRequestHandlers[388] = new RequestHandler(Handler388);
            mRequestHandlers[2] = new RequestHandler(OpenConnectionMessageComposer);
        }
    }
}
