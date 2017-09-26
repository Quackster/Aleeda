using System;
using System.Data;
using Aleeda.Storage;
using Aleeda.Net.Messages;
using System.Collections.Generic;
using System.Text;

namespace Aleeda.HabboHotel.Catalog
{
    public class CataProducts
    {
        #region Fields
        private int cPageId;
        private int cID;
        private int cItemID;
        private string cCCT;
        private int cPixels;
        private int cCredits;
        private int cAmount;
        private string cType;
        #endregion

        #region Properties
        public int PageID
        {
            get { return cPageId; }
        }
        public int ItemID
        {
            get { return cItemID; }
        }
        public string CCTName
        {
            get { return cCCT; }
        }
        public int ID
        {
            get { return cID; }
        }
        public int Pixels
        {
            get { return cPixels; }
        }
        public int Credits
        {
            get { return cCredits; }
        }
        public int Amount
        {
            get { return cAmount; }
        }
        public string Type
        {
            get { return cType; }
        }
        #endregion

        public static CataProducts Parse(DataRow row)
        {
            CataProducts details = new CataProducts();
            try
            {
                details.cPageId = Convert.ToInt32(row["page_id"]);
                details.cID = Convert.ToInt32(row["id"]);
                details.cType = Convert.ToString(row["type"]);
                details.cItemID = Convert.ToInt32(row["sprite_id"]);
                details.cCCT = Convert.ToString(row["sprite_name"]);
                details.cCredits = Convert.ToInt32(row["credits"]);
                details.cPixels = Convert.ToInt32(row["pixels"]);
                details.cAmount = Convert.ToInt32(row["combo_count"]);
                return details;
            }
            catch (Exception ex)
            {
                AleedaEnvironment.GetLog().WriteUnhandledExceptionError("CatalogPage.Parse", ex);
            }

            return null;
        }
    }
}