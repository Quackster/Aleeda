using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Aleeda.HabboHotel.Rooms;
using Aleeda.Net.Messages;
using Aleeda.Storage;

namespace Aleeda.HabboHotel.Cache
{
    public class WallItems
    {
        #region Fields
        private string mWall;
        private int mSpriteID;
        private int mID;
        private int mTrigger;
        private int mRoom;

        public int ID
        {
            get { return mID; }
            set { mID = value; }
        }
        public int SpriteID
        {
            get { return mSpriteID; }
            set { mSpriteID = value; }
        }
        public int Trigger
        {
            get { return mTrigger; }
            set { mTrigger = value; }
        }
        public string Wall
        {
            get { return mWall; }
            set { mWall = value; }
        }
        public int Room
        {
            get { return mRoom; }
            set { mRoom = value; }
        }

        public List<WallItems> wallItems = new List<WallItems>();
        #endregion

        #region Constructers
        public WallItems()
        {
            //Initalize the "wallItems" List.
            //wallItems = new List<WallItems>();

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                
                foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM room_items WHERE isWallItem = 1;").Rows)
                {
                    wallItems.Add(new WallItems((String)row["wall_item"], Convert.ToInt32(row["mID"]), Convert.ToInt32(row["sprite_id"]), Convert.ToInt32(row["id"]), Convert.ToInt32(row["trigger"])));
                }
            }
            //Console.WriteLine("Initializing Wall Item(s).");
            //Console.WriteLine("Successfully cached [ " + wallItems.Count + " ] wall items.\n");
        }
        public WallItems(string mWall, int mRoom, int mSpriteID, int mID, int mTrigger)
        {
            this.Wall = mWall;
            this.Room = mRoom;
            this.SpriteID = mSpriteID;
            this.ID = mID;
            this.Trigger = mTrigger;
        }
        #endregion

        #region Methods
        public int RoomWallItemCount(int id)
        {
            int i = 0;
            foreach (WallItems mItem in this.wallItems)
            {
                if (mItem.Room == id)
                {
                    ++i;
                }
            }
            return i;
        }
        public ServerMessage SerializeWallDisplay(ServerMessage Response, int id)
        {
            Response.AppendInt32(RoomWallItemCount(id));
            foreach (WallItems mItem in this.wallItems)
            {
                if (mItem.Room == id)
                {
                    Response.AppendRawInt32(mItem.ID);
                    Response.AppendChar(2);
                    Response.AppendInt32(mItem.SpriteID);
                    Response.AppendString(mItem.Wall);
                    Response.Append(mItem.Trigger);
                    Response.AppendChar(2);
                    Response.AppendChar(2);
                }
            }
            return Response;
        }
        public void deleteItem(int i)
        {
            foreach (WallItems mItem in this.wallItems.ToArray())
            {
                if (mItem.ID == i)
                {
                    wallItems.Remove(mItem);
                }
            }
        }
        public void newItem(int i)
        {
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {

                foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM room_items WHERE id = '" + i + "'").Rows)
                {
                    wallItems.Add(new WallItems((String)row["wall_item"], Convert.ToInt32(row["mID"]), Convert.ToInt32(row["sprite_id"]), Convert.ToInt32(row["id"]), Convert.ToInt32(row["trigger"])));
                }
            }
        }
        public WallItems getItem(int i)
        {
            foreach (WallItems mItem in this.wallItems.ToArray())
            {
                if (mItem.ID == i)
                {
                    return mItem;
                }
            }
            return null;
        }
        public void reloadItem(int i)
        {
            this.deleteItem(i);
            this.newItem(i);
        }
        #endregion
    }
}
