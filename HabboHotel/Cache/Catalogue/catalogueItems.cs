using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;

using Aleeda.Net.Messages;
using Aleeda.Storage;

namespace Aleeda.HabboHotel.Cache
{
    public class catalogueItems
    {
        #region Fields
        public List<catalogueItems> cataItems;

        public int ID;
        public int pageID;
        public int SpriteID;
        public string CCT;
        public int Pixels;
        public int Credits;
        public int Amount;
        public string Type;
        #endregion

        #region Constructers
        public catalogueItems()
        {
            //Initalize the "cataPages" List.
            cataItems = new List<catalogueItems>();

            //Run the queries and define them/put them into cache.
            foreach (Catalog.CataProducts mItem in AleedaEnvironment.GetHabboHotel().GetCatalog().AllProducts())
            {
                cataItems.Add(new catalogueItems(mItem.ID, mItem.PageID, mItem.ItemID, mItem.CCTName, mItem.Pixels, mItem.Credits, mItem.Amount, mItem.Type));
            }

            //Console.WriteLine("Initializing Catalogue Item(s).");
            //Console.WriteLine("Successfully cached [ " + cataItems.Count + " ] items.\n");
        }
        public catalogueItems(int mID, int pageId, int mSpriteID, string mCCT, int mPixels, int mCredits, int mAmount, string mType)
        {
            this.ID = mID;
            this.pageID = pageId;
            this.SpriteID = mSpriteID;
            this.CCT = mCCT;
            this.Pixels = mPixels;
            this.Credits = mCredits;
            this.Amount = mAmount;
            this.Type = mType;
        }
        #endregion

        #region Methods
        public void SerializeItemPage(int pageID, ServerMessage Response)
        {
            foreach (catalogueItems mItem in cataItems)
            {
                if (mItem.pageID == pageID)
                {
                    Response.AppendInt32(mItem.ID);
                    Response.AppendString(mItem.CCT);
                    Response.AppendInt32(mItem.Credits);
                    Response.AppendInt32(mItem.Pixels);
                    Response.AppendInt32(0);
                    Response.AppendInt32(1);
                    Response.AppendString(mItem.Type.ToLower());
                    if (mItem.CCT.Contains("_single_"))
                    {
                        Response.AppendInt32(mItem.SpriteID);
                        Response.Append(mItem.CCT.Split('_')[2]);
                    }
                    else
                        Response.AppendInt32(mItem.SpriteID);
                    Response.AppendChar(2);
                    Response.AppendInt32(mItem.Amount);
                    Response.AppendInt32(-1);
                    Response.AppendInt32(0);
                }
            }
        }
        public int GetItemCount(int pageid)
        {
            int i = 0;
            foreach (catalogueItems mItem in cataItems)
            {
                if (mItem.pageID == pageid)
                {
                    ++i;
                }
            }
            return i;
        }
        public string ReturnCCTName(int spriteid)
        {
            string i = "";
            foreach (catalogueItems mItem in cataItems)
            {
                if (mItem.SpriteID == spriteid)
                {
                    i = mItem.CCT;

                }
            }
            return i;
        }
        #endregion
    }
}
