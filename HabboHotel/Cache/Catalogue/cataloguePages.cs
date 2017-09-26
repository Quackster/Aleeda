using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Aleeda.Storage;

namespace Aleeda.HabboHotel.Cache
{
    public class cataloguePages
    {
        #region Fields
        public List<cataloguePages> cataPages;

        public int ID;
        public string Layout;
        public string ImgHeader;
        public string ImgSide;
        public string LabelText; 
        public string LabelDesc;
        public string MoreDetails;
        public string HeaderElse;
        public string ContentElse;
        #endregion

        #region Constructer
        public cataloguePages()
        {
            //Initalize the "cataPages" List.
            cataPages = new List<cataloguePages>();

            //Run the queries and define them/put them into cache.
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM catalogue_pages;").Rows)
                {

                        cataPages.Add(new cataloguePages(
                            Convert.ToInt32(row["indexid"]),
                            Convert.ToString(row["style_layout"]), 
                            Convert.ToString(row["img_header"]),
                            Convert.ToString(row["img_side"]), 
                            Convert.ToString(row["label_text"]),
                            Convert.ToString(row["label_description"]), 
                            Convert.ToString(row["label_moredetails"]), 
                            Convert.ToString(row["header_else"]),
                            Convert.ToString(row["content_else"])));
                }
            }

            //Console.WriteLine("Initializing Catalogue Page(s).");
            //Console.WriteLine("Successfully cached [ " + cataPages.Count + " ] pages.\n");
        }
        public cataloguePages(int mID, string mLayout, string mImgHeader, string mImgSide, string mLabelText, string mLabelDesc, string mMoreDetails, string mHeaderElse, string mContentElse)
        {
            this.ID = mID;
            this.Layout = mLayout;
            this.ImgHeader = mImgHeader;
            this.ImgSide = mImgSide;
            this.LabelText = mLabelText;
            this.LabelDesc = mLabelDesc;
            this.MoreDetails = mMoreDetails;
            this.HeaderElse = mHeaderElse;
            this.ContentElse = mContentElse;
        }
        #endregion

        public cataloguePages getPage(int Id)
        {
            foreach (cataloguePages mPages in cataPages)
            {
                if (mPages.ID == Id)
                {
                    return mPages;
                }
            }
            return null;
        }
    }
}
