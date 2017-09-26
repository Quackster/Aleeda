using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Aleeda.HabboHotel.Rooms;
using Aleeda.Net.Messages;
using Aleeda.Storage;

namespace Aleeda.HabboHotel.Cache
{
    public class FloorItems
    {
        #region Fields
        private int mSpriteID;
        private int mID;
        private int mTrigger;
        private int mX;
        private int mY;
        private int mRotation;
        private int mRoomID;

        public List<FloorItems> floorItems;
        #endregion

        #region Properties
        public int SpriteID
        {
            get { return mSpriteID; }
            set { mSpriteID = value; }
        }
        public int ID
        {
            get { return mID; }
            set { mID = value; }
        }
        public int Trigger
        {
            get { return mTrigger; }
            set { mTrigger = value; }
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
        public int Rotation
        {
            get { return mRotation; }
            set { mRotation = value; }
        }
        public int RoomID
        {
            get { return mRoomID; }
            set { mRoomID = value; }
        }
        #endregion

        #region Constructers
        public FloorItems()
        {
            floorItems = new List<FloorItems>();

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM room_items WHERE isWallItem = 0;").Rows)
                {
                    floorItems.Add(new FloorItems(Convert.ToInt32(row["id"]), Convert.ToInt32(row["sprite_id"]), Convert.ToInt32(row["trigger"]), Convert.ToInt32(row["x_axis"]), Convert.ToInt32(row["y_axis"]), Convert.ToInt32(row["rotation"]), Convert.ToInt32(row["mID"])));
                }
            }
            //Console.WriteLine("Initializing Floor Item(s).");
            //Console.WriteLine("Successfully cached [ " + floorItems.Count + " ] floor items.\n");
        }
        public FloorItems(int mID, int mSpriteID, int mTrigger, int mX, int mY, int mRotation, int mRoomID)
        {
            this.ID = mID;
            this.SpriteID = mSpriteID;
            this.Trigger = mTrigger;
            this.X = mX;
            this.Y = mY;
            this.Rotation = mRotation;
            this.RoomID = mRoomID;
        }
        #endregion

        #region Methods
        public int roomItemCount(int room)
        {
            int i = 0;
            foreach (FloorItems mFloor in floorItems)
            {
                if (mFloor.RoomID == room)
                {
                    i++;
                }
            }
            return i;
        }
        public ServerMessage Serialize(ServerMessage Response, int RoomID)
        {
            Response.AppendInt32(roomItemCount(RoomID));
            foreach (FloorItems mItem in floorItems)
            {
                if (mItem.RoomID == RoomID)
                {
                    Response.AppendInt32(mItem.ID);
                    Response.AppendInt32(mItem.SpriteID);
                    Response.AppendInt32(mItem.X);
                    Response.AppendInt32(mItem.Y);
                    Response.AppendInt32(mItem.Rotation);
                    Response.AppendString("0.0");
                    Response.AppendInt32(0);
                    Response.Append(mItem.Trigger);
                    Response.AppendChar(2);
                    Response.AppendInt32(-1);
                    Response.AppendInt32(mItem.Trigger);
                }
            }
            return Response;
        }
        public void deleteItem(int i)
        {
            foreach (FloorItems mItem in floorItems.ToArray())
            {
                if (mItem.ID == i)
                {
                    floorItems.Remove(mItem);
                }
            }

        }
        public void newItem(int i)
        {
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM room_items WHERE id = '" + i + "'").Rows)
                {
                    floorItems.Add(new FloorItems(Convert.ToInt32(row["id"]), Convert.ToInt32(row["sprite_id"]), Convert.ToInt32(row["trigger"]), Convert.ToInt32(row["x_axis"]), Convert.ToInt32(row["y_axis"]), Convert.ToInt32(row["rotation"]), Convert.ToInt32(row["mID"])));
                }
            }
        }
        public FloorItems getItem(int i)
        {
            foreach (FloorItems mItem in this.floorItems.ToArray())
            {
                if (mItem.ID == i)
                {
                    return mItem;
                }
            }
            return null;
        }
        public bool xyCoordIsValid(int room, int x, int y)
        {
            bool returnB = true;

            foreach (FloorItems mItem in this.floorItems.ToArray())
            {
                if (mItem.RoomID == room && mItem.X == x && mItem.Y == y)
                {
                    returnB = false;
                }
            }
            return returnB;
        }
        public int currentRot(int room, int x, int y)
        {
            int r = 0;
            foreach (FloorItems mItem in this.floorItems.ToArray())
            {
                if (mItem.RoomID == room && mItem.X == x && mItem.Y == y)
                {
                    r = mItem.Rotation;
                }
            }
            return r;
        }
        public int currentId(int room, int x, int y)
        {
            int i = 0;
            foreach (FloorItems mItem in this.floorItems.ToArray())
            {
                if (mItem.RoomID == room && mItem.X == x && mItem.Y == y)
                {
                    i = mItem.ID;
                }
            }
            return i;
        }
        #endregion
    }
}
