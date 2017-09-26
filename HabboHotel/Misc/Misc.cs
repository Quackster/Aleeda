using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Aleeda.Storage;

namespace Aleeda.HabboHotel
{
    public class Misc
    {
        #region Insert
        public void doInsertQuery(string Table, Dictionary<string, object> mcollection)
        {
            int ValueInt32 = 0, KeyInt32 = 0;

            //Inialize the query builder
            StringBuilder queryBuilder = new StringBuilder("INSERT INTO " + Table + " ");

            //Initalize the key builder
            StringBuilder keyBuilder = new StringBuilder("(");

            //Initalize the insert builder
            StringBuilder valueBuilder = new StringBuilder(") VALUES (");

            foreach (string Key in mcollection.Keys)
            {
                KeyInt32++;

                if (KeyInt32 != mcollection.Count)
                    keyBuilder.Append("`" + Filter(Key) + "`, ");
                else
                    keyBuilder.Append("`" + Filter(Key) + "`");
            }
            foreach (object Value in mcollection.Values)
            {
                ValueInt32++;

                if (ValueInt32 != mcollection.Count)
                    valueBuilder.Append("'" + Filter(Convert.ToString(Value)) + "', ");
                else
                    valueBuilder.Append("'" + Filter(Convert.ToString(Value)) + "')");
            }
            using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery(queryBuilder.ToString() + keyBuilder.ToString() + valueBuilder.ToString());
            }

            //Clears them
            mcollection.Clear();
        }

        #endregion

        #region Filter!
        private string Filter(string textF)
        {
            string text = Convert.ToString(textF);
            text = text.Replace("\x00", "");
            text = text.Replace("\n", "");
            text = text.Replace("\r", "");
            text = text.Replace("\\", "");
            text = text.Replace("'", "");
            text = text.Replace("\"", "");
            text = text.Replace("\x1a", "");

            return text;
        }
        private bool IsTextValidated(string strTextEntry)
        {
            Regex objNotWholePattern = new Regex("[^0-9]");
            return !objNotWholePattern.IsMatch(strTextEntry)
                 && (strTextEntry != "");
        }
        #endregion
    }
}
