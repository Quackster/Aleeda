using System;
using System.Data;
using Aleeda.Storage;
using Aleeda.Net.Messages;
using System.Collections.Generic;
using System.Text;

namespace Aleeda.HabboHotel.Catalog
{
    public class CatalogPage
    {
        #region Fields
        private int mID;
        private string mLayout;
        private string mImgHeader;
        private string mImgSide;
        private string mLabelText; 
        private string mLabelDesc;
        private string mMoreDetails;
        private string mHeaderElse;
        private string mContentElse;
        #endregion

        #region Properties
        public int ID
        {
            get { return mID; }
        }
        public string Layout
        {
            get { return mLayout; }
        }
        public string ImgHeader
        {
            get { return mImgHeader; }
        }
        public string ImgSide
        {
            get { return mImgSide; }
        }
        public string LabelText
        {
            get { return mLabelText; }
        }
        public string LabelDesc
        {
            get { return mLabelDesc; }
        }
        public string MoreDetails
        {
            get { return mMoreDetails; }
        }
        public string HeaderElse
        {
            get { return mHeaderElse; }
        }
        public string ContentElse
        {
            get { return mContentElse; }
        }
        #endregion

        public static CatalogPage Parse(DataRow row)
        {
            CatalogPage details = new CatalogPage();
            try
            {
                details.mID = Convert.ToInt32(row["indexid"]);
                details.mLayout = Convert.ToString(row["style_layout"]);
                details.mImgHeader = Convert.ToString(row["img_header"]); 
                details.mImgSide = Convert.ToString(row["img_side"]);
                details.mLabelText = Convert.ToString(row["label_text"]);
                details.mLabelDesc = Convert.ToString(row["label_description"]);
                details.mMoreDetails = Convert.ToString(row["label_moredetails"]);
                details.mHeaderElse = Convert.ToString(row["header_else"]);
                details.mContentElse = Convert.ToString(row["content_else"]);
                return details;
            }
            catch (Exception ex )
            {
                AleedaEnvironment.GetLog().WriteUnhandledExceptionError("CatalogPage.Parse", ex);
            }

            return null;
        }
    }
}
