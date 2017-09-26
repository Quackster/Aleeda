using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using IHI.Server.Plugins.Cecer1.IHIPathfinder.Standalone;
using Aleeda.HabboHotel.Client;
using Aleeda.Net.Messages;
using Aleeda.Storage;

namespace Aleeda.HabboHotel.Cache
{
    public class PrivateRooms
    {
        #region Fields
        public List<PrivateRooms> privateRooms;
        public int ID;
        public int OwnerID;
        public int MaxVisitors;
        public string Model;

        private string mName;
        private string mDescription;
        private int mStatus;
        private string mTags;
        private int mCategory;
        private int mRating;
        private int mPetsAllowed;
        private bool mPetsEatOtherFood;
        private string mThumbnail;
        private int mWall;
        private int mFloor;
        private string mLandScape;

        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }
        public string Thumbnail
        {
            get { return mThumbnail; }
            set { mThumbnail = value; }
        }
        public string Tags
        {
            get { return mTags; }
            set { mTags = value; }
        }
        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; }
        }
        public int Status
        {
            get { return mStatus; }
            set { mStatus = value; }
        }
        public int Category
        {
            get { return mCategory; }
            set { mCategory = value; }
        }
        public int Rating
        {
            get { return mRating; }
            set { mRating = value; }
        }
        public int PetsAllowed
        {
            get { return mPetsAllowed; }
            set { mPetsAllowed = value; }
        }
        public bool petsAllowed
        {
            get
            {
                if (mPetsAllowed != 1)
                    return false;
                else
                    return true;
            }
        }
        public bool PetsEatOtherFood
        {
            get { return mPetsEatOtherFood; }
            set { mPetsEatOtherFood = value; }
        }
        public int Wall
        {
            get { return mWall; }
            set { mWall = value; }
        }
        public int Floor
        {
            get { return mFloor; }
            set { mFloor = value; }
        }
        public string LandScape
        {
            get { return mLandScape; }
            set { mLandScape = value; }
        }
        #endregion

        #region Constructor
        public PrivateRooms()
        {
            //Initalize the List for private rooms
            privateRooms = new List<PrivateRooms>();

            new Rooms.Misc.Commands();
        }
        public PrivateRooms(int mID, int mOwnerID, string mName, string mDescription, int mStatus, string mTags, int mCategory, int mRating, int mPetsAllowed, bool mPetsEatOtherFood, int mMaxVisitors, string mThumbnail, int mWall, int mFloor, string mModel, string mLandScape)
        {
            this.ID = mID;
            this.OwnerID = mOwnerID;
            this.Name = mName;
            this.Description = mDescription;
            this.Status = mStatus;
            this.Tags = mTags;
            this.Category = mCategory;
            this.Rating = mRating;
            this.PetsAllowed = mPetsAllowed;
            this.PetsEatOtherFood = mPetsEatOtherFood;
            this.MaxVisitors = mMaxVisitors;
            this.Thumbnail = mThumbnail;
            this.Wall = mWall;
            this.Floor = mFloor;
            this.Model = mModel;
            this.LandScape = mLandScape;
        }
        #endregion

        #region Count values
        public int UsersInRoomCount(int id)
        {
            int i = 0;
            foreach (GameClient mClient in ClientMessageHandler.mRoomList)
            {
                if (mClient.GetHabbo().RoomId == id)
                {
                    ++i;
                }
            }
            return i;
        }
        public int ActiveRoomsCount()
        {
            int i = 0;
            foreach (PrivateRooms mRoom in privateRooms)
            {
                if (AleedaEnvironment.GetCache().GetPrivateRooms().UsersInRoomCount(mRoom.ID) != 0)
                {
                    i++;
                }
            }
            return i;
        }
        public int HighScoreRoomCount()
        {
            int i = 0;
            foreach (PrivateRooms mRoom in privateRooms)
            {
                if (mRoom.Rating != 0)
                {
                    i++;
                }
            }
            return i;
        }
        public int GetOwnRoomsCount(uint OwnerID)
        {
            int i = 0;
            foreach (PrivateRooms mRoom in privateRooms)
            {
                if ((uint)mRoom.OwnerID == OwnerID)
                {
                    i++;
                }
            }
            return i;
        }
        public int GetTagCount(int id)
        {
            int i = 0;
            foreach (PrivateRooms mRoom in privateRooms)
            {
                if (mRoom.ID == id)
                {
                    foreach (string Tag in mRoom.Tags.Split(','))
                    {
                        ++i;
                    }
                }
            }
            return i;
        }
        public void AppendTags(ServerMessage Response, int id)
        {
            foreach (PrivateRooms mRoom in privateRooms)
            {
                if (mRoom.ID == id)
                {
                    foreach (string Tag in mRoom.Tags.Split(','))
                    {
                        Response.AppendString(Tag);
                    }
                }
            }
        }
        public int RoomWithRightsCount(int id)
        {
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                return dbClient.ReadDataTable("SELECT * FROM room_rights WHERE roomid = '" + id + "'").Rows.Count;
            }
        }
        #endregion

        #region Serialize
        public ServerMessage SerializeOwnRooms(GameClient Session)
        {

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM private_rooms WHERE ownerid = '" + Session.GetHabbo().ID + "'").Rows)
                {
                    if (!containsRoom((Int32)row["id"]))
                        privateRooms.Add(new PrivateRooms((Int32)row["id"], (Int32)row["ownerid"], (String)row["name"], (String)row["description"], (Int32)row["status"], (String)row["tags"], (Int32)row["category"], (Int32)row["rating"], (Int32)row["petsAllowed"], false, 25, (String)row["thumbnail"], (Int32)row["wallpaper"], (Int32)row["floorpaper"], (String)row["model"], (String)row["landscape"]));
                }
            }
            ServerMessage Response = new ServerMessage(451); // "GC"
            Response.AppendString("HQA");
            Response.AppendInt32(GetOwnRoomsCount(Session.GetHabbo().ID));
            foreach (PrivateRooms mRoom in privateRooms)
            {
                if (Session.GetHabbo().ID == Convert.ToUInt32(mRoom.OwnerID))
                {
                    Response.AppendInt32(mRoom.ID);
                    Response.AppendBoolean(true);
                    Response.AppendString(mRoom.Name);
                    Response.AppendString(Session.GetHabbo().Username);
                    Response.AppendInt32(mRoom.Status);
                    Response.AppendInt32(UsersInRoomCount(mRoom.ID));
                    Response.AppendInt32(mRoom.MaxVisitors);
                    Response.AppendString(mRoom.Description);
                    Response.AppendBoolean(true);
                    Response.AppendBoolean(true); //Trading set on by default.
                    Response.AppendInt32(mRoom.Rating);
                    Response.AppendInt32(mRoom.Category);
                    Response.AppendString("N/A");
                    Response.AppendInt32(GetTagCount(mRoom.ID)); //TAG count
                    AppendTags(Response, mRoom.ID);
                    Response.Append(mRoom.Thumbnail);
                    Response.AppendBoolean(true);
                }
            }
            return Response;
        }
        public ServerMessage SerializeUsersInRooms()
        {
            ServerMessage Response = new ServerMessage();

            Response.Initialize(451); // "GC"
            Response.AppendString("HQA");
            Response.AppendInt32(ActiveRoomsCount());
            foreach (PrivateRooms mRoom in privateRooms)
            {
                if (UsersInRoomCount(mRoom.ID) != 0)
                {
                    Response.AppendInt32(mRoom.ID);
                    Response.AppendBoolean(true);
                    Response.AppendString(mRoom.Name);
                    Response.AppendString(AleedaEnvironment.GetHabboHotel().GetHabbos().GetHabbo((uint)mRoom.OwnerID).Username);
                    Response.AppendInt32(mRoom.Status);
                    Response.AppendInt32(UsersInRoomCount(mRoom.ID));
                    Response.AppendInt32(mRoom.MaxVisitors);
                    Response.AppendString(mRoom.Description);
                    Response.AppendInt32(mRoom.Category);
                    Response.AppendBoolean(false);
                    Response.AppendBoolean(true); //Trading set on by default.
                    Response.AppendInt32(mRoom.Rating);
                    Response.AppendInt32(mRoom.Category);
                    Response.AppendString("N/A");
                    Response.AppendInt32(GetTagCount(mRoom.ID)); //TAG count
                    AppendTags(Response, mRoom.ID);
                    Response.Append(mRoom.Thumbnail);
                    Response.AppendBoolean(true);
                }
            }
            return Response;
        }
        public ServerMessage SerializeHighestRoomRating()
        {
            ServerMessage Response = new ServerMessage();

            Response.Initialize(451); // "GC"
            Response.AppendString("HQA");
            Response.AppendInt32(HighScoreRoomCount());
            foreach (PrivateRooms mRoom in privateRooms)
            {
                if (mRoom.Rating > 0)
                {
                    Response.AppendInt32(mRoom.ID);
                    Response.AppendBoolean(true);
                    Response.AppendString(mRoom.Name);
                    Response.AppendString(AleedaEnvironment.GetHabboHotel().GetHabbos().GetHabbo((uint)mRoom.OwnerID).Username);
                    Response.AppendInt32(mRoom.Status);
                    Response.AppendInt32(UsersInRoomCount(mRoom.ID));
                    Response.AppendInt32(mRoom.MaxVisitors);
                    Response.AppendString(mRoom.Description);
                    Response.AppendInt32(mRoom.Category);
                    Response.AppendBoolean(false);
                    Response.AppendBoolean(true); //Trading set on by default.
                    Response.AppendInt32(mRoom.Rating);
                    Response.AppendInt32(mRoom.Category);
                    Response.AppendString("N/A");
                    Response.AppendInt32(GetTagCount(mRoom.ID)); //TAG count
                    AppendTags(Response, mRoom.ID);
                    Response.Append(mRoom.Thumbnail);
                    Response.AppendBoolean(true);
                }
            }
            return Response;
        }
        public void SerializeUsersWithRights(ServerMessage Message, int roomid)
        {
            Message.AppendInt32(RoomWithRightsCount(roomid));
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM room_rights WHERE roomid = '" + roomid + "'").Rows)
                {
                    Message.AppendUInt32(AleedaEnvironment.GetHabboHotel().GetHabbos().GetHabbo((string)row["user"]).ID);
                    Message.AppendString((string)row["user"]);
                }
            }
        }
        #endregion

        #region Handling
        public PrivateRooms getRoom(int id)
        {
            foreach (PrivateRooms mRoom in privateRooms.ToArray())
            {
                if (mRoom.ID == id)
                {
                    return mRoom;
                }
            }
            return null;
        }
        public void deleteRoom(int id)
        {
            foreach (PrivateRooms mRoom in privateRooms.ToArray())
            {
                if (mRoom.ID == id && UsersInRoomCount(mRoom.ID) < 1)
                {
                    privateRooms.Remove(mRoom);
                }
            }
        }
        public void deleteRoom(uint OwnerID)
        {
            foreach (PrivateRooms mRoom in privateRooms.ToArray())
            {
                if (UsersInRoomCount(mRoom.ID) < 1 && (uint)mRoom.OwnerID == OwnerID)
                {
                    privateRooms.Remove(mRoom);
                }
            }
        }
        public void newRoom(int id)
        {
            if (containsRoom(id) == false)
            {
                using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
                {
                    DataRow row = dbClient.ReadDataRow("SELECT * FROM private_rooms WHERE id = '" + id + "'");
                    {
                        privateRooms.Add(new PrivateRooms((Int32)row["id"], (Int32)row["ownerid"], (String)row["name"], (String)row["description"], (Int32)row["status"], (String)row["tags"], (Int32)row["category"], (Int32)row["rating"], (Int32)row["petsAllowed"], false, 25, (String)row["thumbnail"], (Int32)row["wallpaper"], (Int32)row["floorpaper"], (String)row["model"], (String)row["landscape"]));
                    }
                }
            }
        }
        public bool containsRoom(int id)
        {
            bool Contains = false;

            foreach (PrivateRooms mRoom in privateRooms.ToArray())
            {
                if (mRoom.ID == id)
                {
                    if (privateRooms.Contains(mRoom))
                        Contains = true;
                }
            }
            return Contains;
        }
        #endregion
    }
}
