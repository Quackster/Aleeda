using System;
using System.Collections;
using System.Collections.Generic;
using Aleeda.HabboHotel.Cache;
using Aleeda.HabboHotel.Catalog;

namespace Aleeda.HabboHotel.Client
{
    public partial class ClientMessageHandler
    {
        /// <summary>
        /// 101 - "Ae"
        /// </summary> 
        private void GetCatalogIndex()
        {
            Response.Initialize(126); // "A~"
            Response.AppendBoolean(true);
            Response.AppendBoolean(false);
            Response.AppendBoolean(false);
            Response.AppendInt32(-1);
            Response.AppendChar(2);
            AleedaEnvironment.GetCache().GetCataIndex().Serialize(GetResponse(), Client.GetHabbo().Role);
            //Response.AppendString(Catalog.Catalog.getPageIndex(Client.GetHabbo().Role));
            SendResponse();
        }

        /// <summary>
        /// 102 = "Af"
        /// </summary>
        private void GetCatalogPage()
        {
            int pageID = Request.PopWiredInt32();

            cataloguePages cataPage = Aleeda.AleedaEnvironment.GetCache().GetCataloguePage().getPage(pageID);

            if (cataPage == null)
                return;

            Response.Initialize(127); // "A"
            Response.AppendInt32(pageID);
            if (cataPage.Layout != "")
            {
                switch (cataPage.Layout)
                {
                    case "pets":
                        {
                            Response.AppendString(cataPage.Layout);
                            Response.AppendInt32(2);
                            Response.AppendString(cataPage.ImgHeader);
                            Response.AppendString("");
                            Response.AppendInt32(4);
                            Response.AppendString(cataPage.LabelDesc);
                            Response.AppendString("Name your pet:");
                            Response.AppendString("Pick a colour:");
                            Response.AppendString("Pick a breed:");

                            //Get the items account
                            Response.AppendInt32(AleedaEnvironment.GetCache().GetCatalogueItems().GetItemCount(pageID));

                            //Serialize Item Page
                            AleedaEnvironment.GetCache().GetCatalogueItems().SerializeItemPage(pageID, Response);

                            //Needed for latest R63
                            Response.AppendInt32(-1);

                        }
                        break;
                    case "spaces":
                        {
                            Response.AppendString("spaces_new");
                            Response.AppendInt32(1);
                            Response.AppendString(cataPage.ImgHeader);
                            Response.AppendInt32(1);
                            Response.AppendString(cataPage.LabelDesc);

                            //Get the items account
                            Response.AppendInt32(AleedaEnvironment.GetCache().GetCatalogueItems().GetItemCount(pageID));

                            //Serialize Item Page
                            AleedaEnvironment.GetCache().GetCatalogueItems().SerializeItemPage(pageID, Response);

                            //Needed for latest R63
                            Response.AppendInt32(-1);
                        }
                        break;
                    default:
                        {
                            Response.AppendString(cataPage.Layout);
                            Response.AppendInt32(3);
                            Response.AppendString(cataPage.ImgHeader);
                            Response.AppendString(cataPage.ImgSide);

                            if (cataPage.LabelText != "")
                                Response.AppendString("catalog_special_txtbg2");
                            else
                                Response.AppendString(string.Empty);

                            Response.AppendInt32(3);
                            Response.AppendString(cataPage.LabelDesc);
                            Response.AppendString(cataPage.MoreDetails);
                            Response.AppendString(cataPage.LabelText);

                            //Get the items account
                            Response.AppendInt32(AleedaEnvironment.GetCache().GetCatalogueItems().GetItemCount(pageID));

                            //Serialize Item Page
                            AleedaEnvironment.GetCache().GetCatalogueItems().SerializeItemPage(pageID, Response);

                            //Needed for latest R63
                            Response.AppendInt32(-1);

                        }
                        break;
                }
            }
            else if (cataPage.Layout == "")
            {
                Response.Append(cataPage.HeaderElse);
                Response.Append(cataPage.ContentElse);
            }
            SendResponse();
        }
        /// <summary>
        /// 100 = "Ad"
        /// </summary>
        private void HandleProduct()
        {
            int pgid = Request.PopWiredInt32();
            int itemid = Request.PopWiredInt32();

            string ExtraData = Request.PopFixedString();
            //Console.WriteLine(ExtraData);


            Catalog.Catalog.HandlePurchase(Client, itemid, ExtraData);

            List<CataProducts> cItem = AleedaEnvironment.GetHabboHotel().GetCatalog().CataProductsByItemID(itemid);

            foreach (CataProducts mItem in cItem)
            {
                Response.Initialize(67); // "AC"
                Response.AppendInt32(mItem.ID);
                Response.AppendString(mItem.CCTName);
                Response.AppendInt32(0);
                Response.AppendInt32(0);
                Response.AppendInt32(0);
                Response.AppendInt32(1);
                Response.AppendInt32(mItem.Credits);
                Response.AppendInt32(mItem.Pixels);
                Response.AppendString(mItem.Type.ToLower());
                Response.AppendInt32(mItem.ItemID);
                Response.AppendString("", 2);
                Response.AppendInt32(1);
                Response.AppendInt32(-1);
            }
            SendResponse();

            Response.Initialize(101); //"Ae" = Update Inventory
            SendResponse();

            Response.Initialize(6); // "@F"
            Response.Append(Client.GetHabbo().Coins);
            Response.Append(".0");
            SendResponse();
        }
        private void GetPetRace()
        {
            /*L{a0 pet9{{2}}
             * QB (count)
             * QB (type)
             * H (for/count)
             * I (true)
             * H (false)*/


            string PetType = Request.PopFixedString();
            int count = 0;
            int Type = int.Parse(PetType.Substring(6));

            if (Type == 0)
                count = 25;
            else if (Type == 1)
                count = 25;
            else if (Type == 2)
                count = 12;
            else if (Type == 3)
                count = 7;
            else if (Type == 4)
                count = 4;
            else if (Type == 5)
                count = 7;
            else if (Type == 6)
                count = 13;
            else if (Type == 7)
                count = 8;
            else if (Type == 8)
                count = 13;
            else if (Type == 9)
                count = 9;
            else if (Type == 10)
                count = 0;
            else if (Type == 11)
                count = 13;
            else if (Type == 12)
                count = 6;

            Response.Initialize(827); // "L{"
            Response.AppendString("a0 pet" + Type);
            Response.AppendInt32(count);
            for (int i = 0; i < count; i++)
            {
                Response.AppendInt32(Type);
                Response.AppendInt32(i);
                Response.AppendBoolean(true);
                Response.AppendBoolean(false);
            }
            SendResponse();
        }
        private void CheckPetName()
        {
            string petname = Request.PopFixedString();

            bool IsValid = true;

            if (petname.Length < 1 || petname.Length > 16)
            {
                IsValid = false;
            }

            if (!AleedaEnvironment.IsValidAlphaNumeric(petname))
            {
                IsValid = false;
            }

            Response.Initialize(36);
            Response.AppendInt32(IsValid ? 0 : 2);
            SendResponse();
        }
        /// <summary>
        /// Registers handlers that have to do with the ingame Catalog.
        /// </summary>
        public void RegisterCatalog()
        {
            mRequestHandlers[101] = new RequestHandler(GetCatalogIndex);
            mRequestHandlers[102] = new RequestHandler(GetCatalogPage);
            mRequestHandlers[100] = new RequestHandler(HandleProduct);
            mRequestHandlers[3007] = new RequestHandler(GetPetRace);
            mRequestHandlers[42] = new RequestHandler(CheckPetName);
        }
    }
}
