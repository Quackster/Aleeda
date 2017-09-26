using System;
using System.Data;
using System.Collections.Generic;
using Aleeda.Storage;
using Aleeda.Net.Messages;

namespace Aleeda.HabboHotel.Messenger
{
    public class MessengerBuddy : ISerializableObject
    {
        #region Fields
        private uint mID;
        private string mUsername;
        private string mFigure;
        private string mMotto;
        #endregion

        #region Properties
        public uint ID
        {
            get { return mID; }
        }
        public string Username
        {
            get { return mUsername; }
        }
        public string Figure
        {
            get { return mFigure; }
        }
        public string Motto
        {
            get { return mMotto; }
        }
        #endregion
        #region Methods
        public void Serialize(ServerMessage message)
        {
            message.AppendUInt32(mID);
            message.AppendString(mUsername);
            message.AppendBoolean(true);
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                DataTable isOnlineQuery = dbClient.ReadDataTable("SELECT * FROM users WHERE username = '" + mUsername + "'");
                foreach (DataRow isOnline in isOnlineQuery.Rows)
                {
                    if ((int)isOnline["online"] == 0)
                    {
                        message.AppendBoolean(false);
                    }
                    else
                    {
                        message.AppendBoolean(true);
                    }
                }
            }
            message.AppendBoolean(false);
            message.AppendString(mFigure);
            message.AppendBoolean(false);
            message.AppendString(mMotto);
            message.AppendString("1-1-2011");
            message.AppendString("");
            message.AppendString("");
            //message.AppendString("");
        }

        public static MessengerBuddy Parse(DataRow row)
        {
            MessengerBuddy buddy = new MessengerBuddy();
            try
            {
                buddy.mID = (uint)row["id"];
                buddy.mUsername = (string)row["username"];
                buddy.mFigure = (string)row["figure"];
                buddy.mMotto = (string)row["motto"];
                return buddy;
            }
            catch (Exception ex)
            {
                AleedaEnvironment.GetLog().WriteUnhandledExceptionError("MessengerBuddy.Parse", ex);
            }

            return null;
        }
        #endregion
    }
}
