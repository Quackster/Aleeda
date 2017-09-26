using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using Aleeda.Net.Messages;
using System.Net.Sockets;
using System.Threading;

using Aleeda.Storage;

namespace Aleeda.HabboHotel.Client
{
    public partial class ClientMessageHandler
    {
        #region Fields
        private const int HIGHEST_MESSAGEID = 4000; // class="com.sulake.habbo.communication.messages.outgoing.handshake::GenerateSecretKeyMessageComposer" />
        private GameClient Client;
        public static List<GameClient> mRoomList = new List<GameClient>();

        private ClientMessage Request;
        private ServerMessage Response;
        private Rooms.RoomSerialize Serialize;
        private Misc mQuery = new Misc();

        private delegate void RequestHandler();
        private RequestHandler[] mRequestHandlers;
        #endregion

        #region Constructor
        public ClientMessageHandler(GameClient pSession)
        {
            Client = pSession;
            mRequestHandlers = new RequestHandler[HIGHEST_MESSAGEID + 1];
            Serialize = new Rooms.RoomSerialize();
            Response = new ServerMessage(0);
        }
        #endregion

        #region Methods
        public ServerMessage GetResponse()
        {
            return Response;
        }

        /// <summary>
        /// Destroys all the resources in the ClientMessageHandler.
        /// </summary>
        public void Destroy()
        {
            Client = null;
            mRequestHandlers = null;

            Request = null;
            Response = null;
        }
        /// <summary>
        /// Invokes the matching request handler for a given ClientMessage.
        /// </summary>
        /// <param name="request">The ClientMessage object to process.</param>
        public void HandleRequest(ClientMessage request)
        {
            if (request.ID > HIGHEST_MESSAGEID)
                return; // Not in protocol
            if (mRequestHandlers[request.ID] == null)
                return; // Handler not registered

            Console.WriteLine(request.ID + request.GetContentString());

            // Handle request
            Request = request;
            mRequestHandlers[request.ID].Invoke();
            Request = null;
        }
        public Misc doQuery()
        {
            return mQuery;
        }
        /// <summary>
        /// Sends the current response ServerMessage on the stack.
        /// </summary>
        public void SendResponse()
        {
            if (Response.ID > 0)
            {
                Client.GetConnection().SendMessage(Response);
            }
        }
        public void SendRoom(ServerMessage Message)
        {
            foreach (GameClient mClient in mRoomList)
            {
                if (mClient.GetHabbo().RoomId == Client.GetHabbo().RoomId)
                {
                    mClient.GetConnection().SendMessage(Message);
                }
            }
        }
        public void SendRoom()
        {
            foreach (GameClient mClient in mRoomList)
            {
                if (mClient.GetHabbo().RoomId == Client.GetHabbo().RoomId)
                {
                    mClient.GetConnection().SendMessage(Response);
                }
            }
        }
        public void SendPublicRoom()
        {
            foreach (GameClient mClient in mRoomList)
            {
                if (mClient.GetHabbo().pRoomId == Client.GetHabbo().pRoomId)
                {
                    mClient.GetConnection().SendMessage(Response);
                }
            }
        }
        public void SendToUsersWithRights()
        {
            foreach (DataRow row in AleedaEnvironment.GetDatabase().GetClient().ReadDataTable("SELECT * FROM room_rights WHERE roomid = '" + Client.GetHabbo().RoomId + "'").Rows)
            {
                foreach (GameClient mClient in mRoomList)
                {
                    if (mClient.GetHabbo().Username == (String)row["user"])
                    {
                        mClient.GetConnection().SendMessage(Response);
                    }
                }
            }
        }
        #endregion
    }
}
