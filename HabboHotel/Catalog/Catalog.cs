using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Aleeda.HabboHotel.Client;
using Aleeda.Net.Messages;
using Aleeda.Storage;

namespace Aleeda.HabboHotel.Catalog
{
    public class Catalog
    {

        #region Methods
        public List<CatalogPage> CataPage(int ID)
        {
            List<CatalogPage> rDetails = new List<CatalogPage>();
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("@id", ID);
                foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM catalogue_pages WHERE indexid = @id;").Rows)
                {
                    CatalogPage details = CatalogPage.Parse(row);
                    if (details != null)
                    {
                        rDetails.Add(details);
                    }
                }
            }
            return rDetails;
        }
        public List<CataProducts> CataProductsByPgID(int ID)
        {
            List<CataProducts> cDetails = new List<CataProducts>();
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("@id", Convert.ToInt32(ID));
                foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM catalogue_products WHERE page_id = @id;").Rows)
                {
                    CataProducts C = Aleeda.HabboHotel.Catalog.CataProducts.Parse(row);
                    if (C != null)
                    {
                        cDetails.Add(C);
                    }
                }
            }

            return cDetails;
        }
        public List<CataProducts> CataProductsByItemID(int ID)
        {
            List<CataProducts> cDetails = new List<CataProducts>();
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("@id", Convert.ToInt32(ID));
                foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM catalogue_products WHERE id = @id;").Rows)
                {
                    CataProducts C = Aleeda.HabboHotel.Catalog.CataProducts.Parse(row);
                    if (C != null)
                    {
                        cDetails.Add(C);
                    }
                }
            }

            return cDetails;
        }
        public List<CataProducts> CataProductsBySpriteID(int ID)
        {
            List<CataProducts> cDetails = new List<CataProducts>();
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("@id", Convert.ToInt32(ID));
                foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM catalogue_products WHERE sprite_id = @id;").Rows)
                {
                    CataProducts C = Aleeda.HabboHotel.Catalog.CataProducts.Parse(row);
                    if (C != null)
                    {
                        cDetails.Add(C);
                    }
                }
            }

            return cDetails;
        }
        public List<CatalogPage> AllCata()
        {
            List<CatalogPage> cDetails = new List<CatalogPage>();
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM catalogue_pages;").Rows)
                {
                    CatalogPage C = Aleeda.HabboHotel.Catalog.CatalogPage.Parse(row);
                    if (C != null)
                    {
                        cDetails.Add(C);
                    }
                }
            }

            return cDetails;
        }
        public List<CataProducts> AllProducts()
        {
            List<CataProducts> cDetails = new List<CataProducts>();
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                foreach (DataRow row in dbClient.ReadDataTable("SELECT * FROM catalogue_products").Rows)
                {
                    CataProducts C = Aleeda.HabboHotel.Catalog.CataProducts.Parse(row);
                    if (C != null)
                    {
                        cDetails.Add(C);
                    }
                }
            }

            return cDetails;
        }
        #endregion


        #region Handle Purchase
        public static void HandlePurchase(GameClient User, int id, string ExtraData)
        {
            uint Credits, Points;
            List<CataProducts> mProducts = AleedaEnvironment.GetHabboHotel().GetCatalog().CataProductsByItemID(id);

            foreach (CataProducts mItem in mProducts)
            {
                uint itmCredits = Convert.ToUInt32(mItem.Credits);
                Credits = User.GetHabbo().Coins - itmCredits;
                Points = User.GetHabbo().ActivityPoints - Convert.ToUInt32(mItem.Pixels);

                User.GetHabbo().Coins = Credits;
                User.GetHabbo().ActivityPoints = Points;

                ServerMessage notify = new ServerMessage(438);
                notify.AppendUInt32(User.GetHabbo().ActivityPoints);
                notify.AppendUInt32(Points);
                User.GetConnection().SendMessage(notify);


                using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
                {
                    dbClient.ExecuteQuery("UPDATE users SET coins = '" + Credits + "' WHERE username = '" + User.GetHabbo().Username + "'");
                    dbClient.ExecuteQuery("UPDATE users SET activitypoints = '" + Points + "' WHERE username = '" + User.GetHabbo().Username + "'");
                    for (int i = 0; i < mItem.Amount; i++)
                    {
                        if (mItem.CCTName.Contains("pet"))
                        {

                            string[] Bits = ExtraData.Split('\n');
                            string PetName = Bits[0];
                            string Race = Bits[1];
                            string Color = Bits[2];

                            Console.WriteLine(Race);

                            dbClient.ExecuteQuery("INSERT INTO user_pet_inventory (userid, petname, race, color) VALUES ('" + User.GetHabbo().ID + "','" + PetName + "','" + Race + "','" + Color + "')");
                            dbClient.ExecuteQuery("INSERT INTO user_inventory (`userid`, `sprite_id`, `type`, `name`) VALUES ('" + User.GetHabbo().ID + "', '1534', 's', 'petfood3')");
                        }
                        else
                        {
                            dbClient.ExecuteQuery("INSERT INTO user_inventory (`userid`, `sprite_id`, `type`, `name`) VALUES ('" + User.GetHabbo().ID + "', '" + mItem.ItemID + "', '" + mItem.Type + "', '" + mItem.CCTName + "')");
                        }
                    }
                }
            }
        #endregion
        }
    }
}
