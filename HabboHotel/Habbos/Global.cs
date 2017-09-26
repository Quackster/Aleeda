using System;
using System.Data;
using Aleeda.Storage;

namespace Aleeda.HabboHotel.Client
{
    public partial class ClientMessageHandler
    {
        /// <summary>
        /// 196 - "CD"
        /// </summary>
        private void Pong()
        {
            Client.PingOK = true;
        }
        /// <summary>
        /// 416 - "F`"
        /// </summary>
        private void FAQ()
        {
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                DataTable Table = dbClient.ReadDataTable("SELECT * FROM faq_help");

                Response.Initialize(416); // "F`"
                Response.AppendInt32(0);
                Response.AppendInt32(Table.Rows.Count);
                foreach (DataRow Row in Table.Rows)
                {
                    Response.AppendInt32(Convert.ToInt32(Row["id"]));
                    Response.AppendString(Convert.ToString(Row["question"]));
                }
                SendResponse();
            }
        }
        /// <summary>
        /// 418 - "Fb"
        /// </summary>
        private void FAQview()
        {
            int mID = Request.PopWiredInt32();

            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("id", mID);
                string mAnswer = dbClient.ReadString("SELECT answer FROM faq_help WHERE id = @id");

                Response.Initialize(520); // "HH"
                Response.AppendInt32(mID);
                Response.Append(mAnswer);

                //Help tool message
                Response.AppendChar(13);
                Response.AppendChar(10);
                Response.AppendChar(13);
                Response.AppendChar(10);
                Response.AppendString("The Help Tool is for account help, payment problems and ban queries. If you need general help with playing the game, please read the FAQs.");
                SendResponse();
            }
        }
        /// <summary>
        /// Registers request handlers available from start of client.
        /// </summary>
        public void RegisterGlobal()
        {
            mRequestHandlers[196] = new RequestHandler(Pong);
            mRequestHandlers[416] = new RequestHandler(FAQ);
            mRequestHandlers[418] = new RequestHandler(FAQview);
        }
    }
}
