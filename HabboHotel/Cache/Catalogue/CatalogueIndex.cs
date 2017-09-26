using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Aleeda.Storage;

namespace Aleeda.HabboHotel.Cache
{
    public class CatalogueIndex
    {
        #region Fields
        public List<CatalogueIndex> cataIndex;

        public int ID;
        public int PageId;
        public int MinRank;
        public int Icon;
        public int Colour;
        public string DisplayName;
        public bool IsTree;
        public int InCategory;
        #endregion

        #region Constructers
        public CatalogueIndex()
        {
            //Initalize the "cataPages" List.
            cataIndex = new List<CatalogueIndex>();

            //Run the queries and define them/put them into cache.
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM catalogue_categories;").Rows)
                {
                    cataIndex.Add(new CatalogueIndex(
                        Convert.ToInt32(row["indexid"]),
                        Convert.ToInt32(row["pageid"]),
                        Convert.ToInt32(row["minrank"]),
                        Convert.ToInt32(row["icon"]),
                        Convert.ToInt32(row["colour"]),
                        Convert.ToString(row["indexname"]), true, 0));
                }
                foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM catalogue_pages;").Rows)
                {
                    cataIndex.Add(new CatalogueIndex(
                        Convert.ToInt32(row["indexid"]), 0,
                        Convert.ToInt32(row["minrank"]),
                        Convert.ToInt32(row["style_icon"]), 0,
                        Convert.ToString(row["displayname"]), false,
                        Convert.ToInt32(row["in_category"])));
                }
            }

        }
        public CatalogueIndex(int mID, int mPageId, int mMinRank, int mIcon, int mColour, string mDisplayName, bool mIsTree, int mInCategory)
        {
            this.ID = mID;
            this.PageId = mPageId;
            this.MinRank = mMinRank;
            this.Icon = mIcon;
            this.Colour = mColour;
            this.DisplayName = mDisplayName;
            this.IsTree = mIsTree;
            this.InCategory = mInCategory;
        }
        public int getTreeCount()
        {
            int i = 0;
            foreach (CatalogueIndex c in cataIndex)
            {
                if (c.IsTree != false)
                {
                    ++i;
                }
            }
            return i;
        }
        public int getSubcatCount(int cat)
        {
            int i = 0;
            foreach (CatalogueIndex c in cataIndex)
            {
                if (c.InCategory == cat && !c.IsTree)
                {
                    ++i;
                }
            }
            return i;
        }
        public void Serialize(Net.Messages.ServerMessage Message, int Rank)
        {
            Message.AppendInt32(getTreeCount());
            foreach (CatalogueIndex t in cataIndex)
            {
                if (t.IsTree != false && Rank >= t.MinRank)
                {
                    Message.AppendBoolean(true);
                    Message.AppendInt32(t.Colour);
                    Message.AppendInt32(t.Icon);
                    Message.AppendInt32(Convert.ToBoolean(t.PageId) ? t.PageId : -1);
                    Message.AppendString(t.DisplayName);
                    Message.AppendInt32(getSubcatCount(t.ID));
                    foreach (CatalogueIndex c in cataIndex)
                    {
                        if (c.IsTree != true && c.InCategory == t.ID && Rank >= t.MinRank)
                        {
                            Message.AppendBoolean(true);
                            Message.AppendInt32(c.Colour);
                            Message.AppendInt32(c.Icon);
                            Message.AppendInt32(c.ID);
                            Message.AppendString(c.DisplayName);
                            Message.AppendInt32(0);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
