using System;
using System.Collections.Generic;
using System.Data;
using Aleeda.Storage;
using Aleeda.HabboHotel.Client.Utilities;
using Aleeda.HabboHotel.Messenger;
using Aleeda.Net.Messages;

namespace Aleeda.HabboHotel.Client
{
    public partial class ClientMessageHandler
    {
        private const bool ENABLE_MESSENGER = true;

        /// <summary>
        /// 7 - "@G"
        /// </summary>
        public void InfoRetrieve()
        {
            if (Client.GetHabbo() != null)
            {
                Response.Initialize(5); // "@E"
                Response.AppendObject(Client.GetHabbo());
                SendResponse();
            }
        }

        /// <summary>
        /// 8 - "@H"
        /// </summary>
        public void GetCredits()
        {
            if (Client.GetHabbo() != null)
            {
                Response.Initialize(6); // "@F"
                Response.Append(Client.GetHabbo().Coins);
                Response.Append(".0");

                SendResponse();
            }
        }
        /// <summary>
        /// 12 - "@L"
        /// </summary>
        private void MessengerInit()
        {
            if (Client.InitializeMessenger())
            {
                // Register handlers
                RegisterMessenger();

                // Send initialization message
                Response.Initialize(12); // "@L"
                Response.AppendInt32(600);
                Response.AppendInt32(200);
                Response.AppendInt32(600);
                Response.AppendBoolean(false);
                Response.AppendInt32(Client.GetMessenger().GetBuddies().Count);
                using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
                {
                    foreach (MessengerBuddy buddy in Client.GetMessenger().GetBuddies())
                    {
                        Response.AppendObject(buddy);
                    }
                }
                SendResponse();
            }
        }
        /// <summary>
        /// 26 - "@Z"
        /// </summary>
        public void ScrGetUserInfo()
        {
            string sSubscription = Request.PopFixedString();
            Response.Initialize(7); // "@G"
            Response.AppendString(sSubscription);
            Response.AppendInt32(10);
            Response.AppendInt32(1);
            Response.AppendInt32(1);
            Response.AppendBoolean(true);
            SendResponse();
        }

        /// <summary>
        /// 157 - "B]"
        /// </summary>
        public void GetBadges()
        {

        }

        /// <summary>
        /// 233 - "EA"
        /// </summary>
        private void GetIgnoredUsers()
        {
            Response.Initialize(420); // "Fd"
            Response.AppendString("Aaron");
            Response.AppendString("office.boy");
            Response.AppendString("Phoenix");
            SendResponse();
        }

        /// <summary>
        /// 370 - "Er"
        /// </summary>
        private void GetAchievements()
        {
            // Get achievements from Database
            List<string> achievements = AleedaEnvironment.GetHabboHotel().GetAchievements().GetAchievements(Client.GetHabbo().ID);

            // Build response
            Response.Initialize(229); // "Ce"
            Response.AppendInt32(achievements.Count);
            foreach (string achievement in achievements)
            {
                Response.AppendString(achievement);
            }
            SendResponse();
        }

        /// <summary>
        /// Registers request handlers available from successful login.
        /// </summary>
        public void RegisterUser()
        {
            mRequestHandlers[7] = new RequestHandler(InfoRetrieve);
            mRequestHandlers[8] = new RequestHandler(GetCredits);
            mRequestHandlers[12] = new RequestHandler(MessengerInit);
            mRequestHandlers[26] = new RequestHandler(ScrGetUserInfo);
            mRequestHandlers[157] = new RequestHandler(GetBadges);
            mRequestHandlers[233] = new RequestHandler(GetIgnoredUsers);
            mRequestHandlers[370] = new RequestHandler(GetAchievements);
        }
    }
}
