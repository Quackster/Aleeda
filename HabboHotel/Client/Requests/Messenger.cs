using System;
using System.Collections.Generic;
using System.Data;
using Aleeda.Core;
using Aleeda.HabboHotel.Messenger;
using Aleeda.Net.Messages;
using Aleeda.Storage;

namespace Aleeda.HabboHotel.Client
{
    public partial class ClientMessageHandler
    {
        /// <summary>
        /// 15 - "@O"
        /// </summary>
        private void FriendListUpdate()
        {

        }

        /// <summary>
        /// 33 - "@a"
        /// </summary>
        private void SendMsg()
        {
            uint buddyID = Request.PopWiredUInt32();
            string sText = Request.PopFixedString();

            // Buddy in list?
            if (Client.GetMessenger().GetBuddy(buddyID) != null)
            {
                // Buddy online?
                GameClient buddyClient = AleedaEnvironment.GetHabboHotel().GetClients().GetClientOfHabbo(buddyID);
                if (buddyClient == null)
                {
                    Response.Initialize(261); // "DE"
                    Response.AppendInt32(5); // Error code
                    Response.AppendUInt32(Client.GetHabbo().ID);
                    SendResponse();
                }
                else
                {
                    ServerMessage notify = new ServerMessage(134); // "BF"
                    notify.AppendUInt32(Client.GetHabbo().ID);
                    notify.AppendString(sText);
                    buddyClient.GetConnection().SendMessage(notify);
                }
            }
        }

        /// <summary>
        /// 34 - "@b"
        /// </summary>
        private void SendRoomInvite()
        {
            // TODO: check if this session is in room

            // Determine how many receivers
            int amount = Request.PopWiredInt32();
            List<GameClient> receivers = new List<GameClient>(amount);

            // Get receivers
            for (int i = 0; i < amount; i++)
            {
                // User in buddy list?
                uint buddyID = Request.PopWiredUInt32();
                if (Client.GetMessenger().GetBuddy(buddyID) != null)
                {
                    // User online?
                    GameClient buddyClient = AleedaEnvironment.GetHabboHotel().GetClients().GetClientOfHabbo(buddyID);
                    if (buddyClient != null)
                    {
                        receivers.Add(buddyClient);
                    }
                }
            }

            // Parse text
            string sText = Request.PopFixedString();

            // Notify the receivers
            ServerMessage notify = new ServerMessage(135); // "BG"
            //...
            foreach (GameClient receiver in receivers)
            {
                receiver.GetConnection().SendMessage(notify);
            }
        }

        /// <summary>
        /// 37 - "@e"
        /// </summary>
        private void AcceptBuddy()
        {
            int amount = Request.PopWiredInt32();
            for (int i = 0; i < amount; i++)
            {
                uint buddyID = Request.PopWiredUInt32();

                using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
                {
                    dbClient.ExecuteQuery("INSERT INTO messenger_buddylist (`userid`, `fromid`, `accepted`) VALUES ('" + Client.GetHabbo().ID + "','" + buddyID + "', '1')");
                }
            }
        }

        /// <summary>
        /// 38 - "@f"
        /// </summary>
        private void DeclineBuddy()
        {
            int amount = Request.PopWiredInt32();
            for (int i = 0; i < amount; i++)
            {
                uint buddyID = Request.PopWiredUInt32();
                using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
                {
                    dbClient.ExecuteQuery("DELETE * FROM messenger_request WHERE fromid = '" + Client.GetHabbo().ID + "' AND toid = '" + buddyID + "'");
                }
            }
        }
        
        /// <summary>
        /// 39 - "@g"
        /// </summary>
        private void RequestBuddy()
        {
                using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
                {
                    dbClient.AddParamWithValue("user", Request.PopFixedString());
                    DataRow row = dbClient.ReadDataRow("SELECT * FROM users WHERE username = @user;");

                    dbClient.ExecuteQuery("INSERT INTO messenger_request (`fromid`, `toid`) VALUES ('" + Client.GetHabbo().ID + "','" + (uint)row["id"] + "')");
                    
                    ServerMessage Response = new ServerMessage(132); // "BD"
                    Response.AppendUInt32(Client.GetHabbo().ID);
                    Response.AppendString(Client.GetHabbo().Username);
                    Response.AppendString(Client.GetHabbo().Figure);
                    if (AleedaEnvironment.GetHabboHotel().GetClients().GetClientOfHabbo((string)row["username"]) != null)
                    {
                        AleedaEnvironment.GetHabboHotel().GetClients().GetClientOfHabbo((string)row["username"]).GetConnection().SendMessage(Response);
                    }
            }
        }

        /// <summary>
        /// 40 - "@h"
        /// </summary>
        private void RemoveBuddy()
        {
            uint amount = Request.PopUInt32();
            for (int i = 0; i < amount; i++)
            {
                uint buddyID = Request.PopWiredUInt32();
                if (Client.GetMessenger().GetBuddy(buddyID) != null)
                {

                }
            }
        }

        /// <summary>
        /// 41 - "@i"
        /// </summary>
        private void HabboSearch()
        {

            // Parse search criteria
            string sCriteria = Request.PopFixedString();

            // Query Habbos with names similar to criteria
            List<MessengerBuddy> matches = AleedaEnvironment.GetHabboHotel().GetMessenger().SearchHabbos(sCriteria);

            // Build response
            Response.Initialize(435); // "Fs"
            Response.AppendInt32(0);
            Response.AppendInt32(matches.Count);
            foreach (MessengerBuddy match in matches)
            {
                if (Client.GetHabbo().Username == match.Username)
                {
                    Response.AppendInt32((int)match.ID);
                    Response.AppendString((string)match.Username);
                    Response.AppendChar(2);
                    Response.AppendInt32(1);
                    Response.AppendInt32(1);
                    Response.AppendChar(2);
                    Response.AppendInt32(1);
                    Response.AppendString((string)match.Figure);
                    Response.AppendChar(2);
                    Response.AppendChar(2);
                }
                else if (mBoolean.FindExistingFriend(match.ID, Client.GetHabbo().Username) == true)
                {
                    Response.AppendInt32((int)match.ID);
                    Response.AppendString((string)match.Username);
                    Response.AppendChar(2);
                    Response.AppendInt32(1);
                    Response.AppendInt32(1);
                    Response.AppendChar(2);
                    Response.AppendInt32(1);
                    Response.AppendString((string)match.Figure);
                    Response.AppendChar(2);
                    Response.AppendChar(2);
                }
                else
                {
                    Response.AppendInt32((int)match.ID);
                    Response.AppendString((string)match.Username);
                    Response.AppendString("cake");
                    Response.AppendInt32(0);
                    Response.AppendInt32(0);
                    Response.AppendChar(2);
                    Response.AppendInt32(1);
                    Response.AppendChar(2);
                    Response.AppendChar(2);
                    Response.AppendChar(2);
                }
            }
            SendResponse();
        }

        /// <summary>
        /// 233 - "Ci"
        /// </summary>
        private void GetBuddyRequests()
        {
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("owner", Client.GetHabbo().ID);
                DataTable DataQuery = dbClient.ReadDataTable("SELECT * FROM messenger_request WHERE toid = @owner LIMIT 1");

                Response.Initialize(314); // "Dz"
                Response.AppendInt32(DataQuery.Rows.Count); // Amount
                Response.AppendInt32(DataQuery.Rows.Count); // Amount
                foreach (DataRow Row in DataQuery.Rows)
                {
                    dbClient.AddParamWithValue("from", Convert.ToUInt32(Row["fromid"]));
                    DataRow FromName = dbClient.ReadDataRow("SELECT * FROM users WHERE id = @from");

                    Response.AppendUInt32((uint)FromName["id"]);// User ID
                    Response.AppendString((string)FromName["username"]);
                    Response.AppendString(Convert.ToString((int)Row["id"])); // Request 
                }
                SendResponse();
            }
        }


        /// <summary>
        /// 262 - "DF"
        /// </summary>
        private void FollowFriend()
        {

        }

        /// <summary>
        /// Registers the request handlers for the in-game messenger. ('Console')
        /// </summary>
        public void RegisterMessenger()
        {
            /*mRequestHandlers[15] = new RequestHandler(FriendListUpdate);
            mRequestHandlers[33] = new RequestHandler(SendMsg);
            mRequestHandlers[34] = new RequestHandler(SendRoomInvite);
            mRequestHandlers[37] = new RequestHandler(AcceptBuddy);
            mRequestHandlers[38] = new RequestHandler(DeclineBuddy);
            mRequestHandlers[39] = new RequestHandler(RequestBuddy);
            mRequestHandlers[40] = new RequestHandler(RemoveBuddy);
            mRequestHandlers[41] = new RequestHandler(HabboSearch);
            mRequestHandlers[233] = new RequestHandler(GetBuddyRequests);
            mRequestHandlers[262] = new RequestHandler(FollowFriend);*/
        }
    }
}
