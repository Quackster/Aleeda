using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Aleeda.HabboHotel.Client;
using Aleeda.Storage;
using Aleeda.HabboHotel.Cache;
using Aleeda.Net.Messages;

namespace Aleeda.HabboHotel.Habbos
{
    /// <summary>
    /// Represents a service user's account and avatar in the account and holds the information about the account.
    /// </summary>
    public class Habbo : IDataObject, ISerializableObject
    {
        #region Fields
        // Account
        private uint mID;
        private string mUsername;
        private string mPassword;
        private byte mRole;
        private DateTime mSignedUp;

        // Personal
        private string mEmail;
        private string mDateOfBirth;

        // Avatar
        private string mMotto;
        private string mFigure;
        private char mGender;

        // Variables
        private uint mCoins;
        private uint mFilms;
        private uint mGameTickets;
        private uint mActivityPoints;
        private int mRoom;
        private int mpRoomId;
        private int mVirtualID;
        private string mFlatModel;
        private string mFlatType;

        //Class variables
        private HabboFunctions HabboFunctions;

        //Items
        private int mCurrentFurniPlacementID;
        private string mWallPlacementMap;
        private bool mIsWallItem;
        private bool mIsOnItem;
        private int mUserRotation = 2;

        //Rooms
        private int mX;
        private int mY;
        private int mReqY;
        private int mReqX;
        private bool mIsDancing;
        private int mDanceId;
        private int mHomeRoom;
        private string mStatus;
        private string mMoveStatus = "";

        //pets
        private string mPetName;
        private string mPetRace;
        #endregion

        #region Properties
        public uint ID
        {
            get { return mID; }
        }
        public string Username
        {
            get { return mUsername; }
            set { mUsername = value; }
        }
        public string Password
        {
            get { return mPassword; }
            set { mPassword = value; }
        }
        public byte Role
        {
            get { return mRole; }
        }
        public DateTime SignedUp
        {
            get { return mSignedUp; }
            set { mSignedUp = value; }
        }
        public string Email
        {
            get { return mEmail; }
            set { mEmail = value; }
        }
        public string DateOfBirth
        {
            get { return mDateOfBirth; }
            set { mDateOfBirth = value; }
        }
        public string Motto
        {
            get { return mMotto; }
            set { mMotto = value; }
        }
        public string Figure
        {
            get { return mFigure; }
            set { mFigure = value; }
        }
        public char Gender
        {
            get { return mGender; }
            set { mGender = (value == 'M' || value == 'F') ? value : 'M'; }
        }
        public uint Coins
        {
            get { return mCoins; }
            set { mCoins = value; }
        }
        public uint Films
        {
            get { return mFilms; }
            set { mFilms = value; }
        }
        public uint GameTickets
        {
            get { return mGameTickets; }
            set { mGameTickets = value; }
        }
        public uint ActivityPoints
        {
            get { return mActivityPoints; }
            set { mActivityPoints = value; }
        }
        public int RoomId
        {
            get { return mRoom; }
            set { mRoom = value; }
        }
        public int pRoomId
        {
            get { return mpRoomId; }
            set { mpRoomId = value; }
        }
        public int UnitId
        {
            get { return mVirtualID; }
            set { mVirtualID = value; }
        }
        public string FlatModel
        {
            get { return mFlatModel; }
            set { mFlatModel = value; }
        }
        public string FlatType
        {
            get { return mFlatType; }
            set { mFlatType = value; }
        }
        public int ItemUsingID
        {
            get { return mCurrentFurniPlacementID; }
            set { mCurrentFurniPlacementID = value; }
        }
        public bool IsWallItem
        {
            get { return mIsWallItem; }
            set { mIsWallItem = value; }
        }
        public string WallPlacementMap
        {
            get { return mWallPlacementMap; }
            set { mWallPlacementMap = value; }
        }
        public bool IsDancing
        {
            get { return mIsDancing; }
            set { mIsDancing = value; }
        }
        public int DanceId
        {
            get { return mDanceId; }
            set { mDanceId = value; }
        }
        public int HomeRoom
        {
            get { return mHomeRoom; }
            set { mHomeRoom = value; }
        }
        public string PetName
        {
            get { return mPetName; }
            set { mPetName = value; }
        }
        public string PetRace
        {
            get { return mPetRace; }
            set { mPetRace = value; }
        }
        public int X
        {
            get { return mX; }
            set { mX = value; }
        }
        public int Y
        {
            get { return mY; }
            set { mY = value; }
        }
        public int ReqX
        {
            get { return mReqX; }
            set { mReqX = value; }
        }
        public int ReqY
        {
            get { return mReqY; }
            set { mReqY = value; }
        }
        public bool IsOnItem
        {
            get { return mIsOnItem; }
            set { mIsOnItem = value; }
        }
        public int UserRotation
        {
            get { return mUserRotation; }
            set { mUserRotation = value; }
        }
        public string Status
        {
            get { return mStatus; }
            set { mStatus = value; }
        }
        public string MoveStatus
        {
            get { return mMoveStatus; }
            set { mMoveStatus = value; }
        }
        public HabboFunctions Functions()
        {
            return HabboFunctions;
        }
        #endregion

        #region Methods
        public void Serialize(ServerMessage message)
        {
            HabboFunctions = new HabboFunctions(GetClient());

            message.AppendString(mID.ToString());
            message.AppendString(mUsername);
            message.AppendString(mFigure);
            message.AppendString(mGender.ToString());
            message.AppendString(mMotto.ToString());
            message.AppendBoolean(false);
            message.AppendChar(2);
            message.AppendBoolean(false);
            message.AppendBoolean(false);
            message.AppendBoolean(false);
            message.AppendBoolean(false);
        }

        public string ToOldProtocolString()
        {
            return "name=" + mUsername + Convert.ToChar(13) +
                      "figure=" + mFigure + Convert.ToChar(13) +
                      "sex=" + mGender.ToString() + Convert.ToChar(13) +
                      "customData=" + mMotto + Convert.ToChar(13) +
                      "ph_tickets=" + mGameTickets + Convert.ToChar(13) +
                      "photo_film=" + mFilms + Convert.ToChar(13) +
                      "ph_figure=" + "" + Convert.ToChar(13) +
                      "directMail=0" + Convert.ToChar(13);
        }

        #region Storage
        private void CheckinUserParams(ref DatabaseClient dbClient)
        {
            dbClient.AddParamWithValue("@id", mID);
            dbClient.AddParamWithValue("@username", mUsername);
            dbClient.AddParamWithValue("@password", mPassword);
            dbClient.AddParamWithValue("@role", mRole);
            dbClient.AddParamWithValue("@signedup", mSignedUp);

            dbClient.AddParamWithValue("@email", mEmail);
            dbClient.AddParamWithValue("@dob", mDateOfBirth);

            dbClient.AddParamWithValue("@motto", mMotto);
            dbClient.AddParamWithValue("@figure", mFigure);
            dbClient.AddParamWithValue("@gender", mGender);

            dbClient.AddParamWithValue("@coins", mCoins);
            dbClient.AddParamWithValue("@films", mFilms);
            dbClient.AddParamWithValue("@gametickets", mGameTickets);
            dbClient.AddParamWithValue("@activitypoints", mActivityPoints);
            dbClient.AddParamWithValue("@flat", mRoom);
            dbClient.AddParamWithValue("@homeroom", mHomeRoom);
        }
        private bool CheckoutUserParams(ref DataRow dRow)
        {
            if (dRow == null)
                return false;

            mID = (uint)dRow["id"];
            mUsername = (string)dRow["username"];
            mPassword = (string)dRow["password"];
            mRole = (byte)dRow["role"];
            mSignedUp = (DateTime)dRow["signedup"];

            mEmail = (string)dRow["email"];
            mDateOfBirth = (string)dRow["dob"];

            mMotto = (string)dRow["motto"];
            mFigure = (string)dRow["figure"];
            Gender = char.Parse(dRow["gender"].ToString());

            mCoins = (uint)dRow["coins"];
            mFilms = (uint)dRow["films"];
            mGameTickets = (uint)dRow["gametickets"];
            mActivityPoints = (uint)dRow["activitypoints"];
            mRoom = (int)dRow["flat"];
            mHomeRoom = (int)dRow["homeRoom"];
            return true;
        }
        
        public bool LoadByID(DatabaseManager database, uint ID)
        {
            DataRow result = null;
            using (DatabaseClient dbClient = database.GetClient())
            {
                dbClient.AddParamWithValue("@id", ID);
                result = dbClient.ReadDataRow("SELECT * FROM users WHERE id = @id LIMIT 1;");
            }

            return CheckoutUserParams(ref result);
        }
        public bool LoadByUsername(DatabaseManager database, string sUsername)
        {
            DataRow result = null;
            using (DatabaseClient dbClient = database.GetClient())
            {
                dbClient.AddParamWithValue("@username", sUsername);
                result = dbClient.ReadDataRow("SELECT * FROM users WHERE username = @username LIMIT 1;");
            }

            return CheckoutUserParams(ref result);
        }

        public bool LoadBySsoTicket(DatabaseManager database, string sTicket)
        {
            DataRow result = null;
            using (DatabaseClient dbClient = database.GetClient())
            {
                dbClient.AddParamWithValue("@ticket", sTicket);
                result = dbClient.ReadDataRow("SELECT * FROM users WHERE ssoticket = @ticket;");
                if (result != null)
                {
                    //dbClient.ExecuteQuery("UPDATE users SET ssoticket = NULL WHERE ssoticket = @ticket LIMIT 1;");
                }
            }

            return CheckoutUserParams(ref result);
        }
        public Aleeda.HabboHotel.Client.GameClient GetClient()
        {
            return AleedaEnvironment.GetHabboHotel().GetClients().GetClientOfHabbo(ID);
        }
        public bool INSERT(DatabaseClient dbClient)
        {
            CheckinUserParams(ref dbClient);
            dbClient.ExecuteQuery("INSERT INTO users" +
                "(username,password,role,signedup,email,dob,motto,figure,gender,coins,films,gametickets,activitypoints) " +
                "VALUES(@username,@password,@role,@signedup,@email,@dob,@motto,@figure,@gender,@coins,@films,@gametickets,@activitypoints);");

            return true;
        }
        public bool DELETE(DatabaseClient dbClient)
        {
            dbClient.AddParamWithValue("@id", mID);
            dbClient.ExecuteQuery("DELETE FROM users WHERE id = @id;");

            return true;
        }
        public bool UPDATE(DatabaseClient dbClient)
        {
            CheckinUserParams(ref dbClient);
            dbClient.ExecuteQuery("UPDATE users " +
                "SET username=@username,password=@password,role=@role,signedup=@signedup,email=@email,dob=@dob,motto=@motto,figure=@figure,gender=@gender,coins=@coins,films=@films,gametickets=@gametickets,activitypoints=@activitypoints,flat=@flat,homeRoom=@homeroom " +
                "WHERE id = @id;");

            return true;
        }
        public bool HasFuse(string Fuse)
        {
            int mRank = Role;
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("role", mRank);
                dbClient.AddParamWithValue("fuse", Fuse);
                if (dbClient.ReadBoolean("SELECT * FROM fuserights WHERE rank = @role AND fuse = @fuse") == true)
                    return true;
                else
                    return false;
            }
        }
        public void RemoveItem(int Id)
        {
            GetClient().GetMessageHandler().GetResponse().Initialize(99);
            GetClient().GetMessageHandler().GetResponse().AppendInt32(Id);
            GetClient().GetMessageHandler().SendRoom();

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("DELETE FROM user_inventory WHERE id = '" + Id + "' LIMIT 1");
            }
        }
        public void AddItem(int furniId, int spriteId, string FurnitureType)
        {
            string FurnitureName = "";

            List<Aleeda.HabboHotel.Catalog.CataProducts> cItem = AleedaEnvironment.GetHabboHotel().GetCatalog().CataProductsBySpriteID(spriteId);
            {
                foreach (Catalog.CataProducts cataItem in cItem) { FurnitureName = cataItem.CCTName; }
            }

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {

                dbClient.ExecuteQuery("INSERT INTO user_inventory (userid, sprite_id, type, name) VALUES ('" + mID + "','" + spriteId + "','" + FurnitureType + "','" + FurnitureName + "')");

                dbClient.ExecuteQuery("DELETE FROM room_items WHERE id = '" + furniId + "'");
            }

            GetClient().GetMessageHandler().GetResponse().Initialize(101);
            GetClient().GetMessageHandler().SendResponse();

            /*GetClient().GetMessageHandler().GetResponse().Initialize(832);
            GetClient().GetMessageHandler().GetResponse().AppendBoolean(true);
            if (FurnitureType == "s")
                GetClient().GetMessageHandler().GetResponse().AppendInt32(1);
            else
                GetClient().GetMessageHandler().GetResponse().AppendInt32(2);
            GetClient().GetMessageHandler().GetResponse().AppendInt32(1); // total count of furnis
            GetClient().GetMessageHandler().GetResponse().AppendInt32(furniId); // id of furni purchased on inventory
            GetClient().GetMessageHandler().SendResponse();*/
        }
        #endregion
        #endregion
    }
}
