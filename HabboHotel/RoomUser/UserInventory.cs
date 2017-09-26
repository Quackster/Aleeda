using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;

namespace Aleeda.HabboHotel
{
    public class UserInventory
    {
        #region Fields
        private int mID;
        private int spriteID;
        private string mName;
        private string mType;
        private string petname;
        private int race;
        private string colour;
        #endregion

        #region Properties
        public int ID
        {
            get { return mID; }
        }
        public int SpriteID
        {
            get { return spriteID; }
        }
        public string Name
        {
            get { return mName; }
        }
        public string Type
        {
            get { return mType; }
        }
        public int Race
        {
            get { return race; }
        }
        public string Colour
        {
            get { return colour; }
        }
        public string Petname
        {
            get { return petname; }
        }
        #endregion

        public static UserInventory Parse(DataRow row, bool b)
        {
            UserInventory details = new UserInventory();
            try
            {
                if (!b)
                {
                    details.mID = (int)row["id"];
                    details.mName = (string)row["name"];
                    details.spriteID = (int)row["sprite_id"];
                    details.mType = (string)row["type"];
                }
                else if (b)
                {
                    details.mID = (int)row["id"];
                    details.petname = (string)row["petname"];
                    details.race = (int)row["race"];
                    details.colour = (string)row["color"];
                }
                return details;
            }
            catch (Exception ex)
            {
                AleedaEnvironment.GetLog().WriteUnhandledExceptionError("UserInventory.Parse", ex);
            }

            return null;
        }
        //This function is based on the one from "Holograph Emulator"
        public static string WallPositionCheck(string wallPosition)
        {
            //:w=3,2 l=9,63 l
            try
            {
                if (wallPosition.Contains(Convert.ToChar(13)))
                { return null; }
                if (wallPosition.Contains(Convert.ToChar(9)))
                { return null; }

                string[] posD = wallPosition.Split(' ');
                if (posD[2] != "l" && posD[2] != "r")
                    return null;

                string[] widD = posD[0].Substring(3).Split(',');
                int widthX = int.Parse(widD[0]);
                int widthY = int.Parse(widD[1]);
                if (widthX < 0 || widthY < 0 || widthX > 200 || widthY > 200)
                    return null;

                string[] lenD = posD[1].Substring(2).Split(',');
                int lengthX = int.Parse(lenD[0]);
                int lengthY = int.Parse(lenD[1]);
                if (lengthX < 0 || lengthY < 0 || lengthX > 200 || lengthY > 200)
                    return null;

                return ":w=" + widthX + "," + widthY + " " + "l=" + lengthX + "," + lengthY + " " + posD[2];
            }
            catch
            {
                return null;
            }
        }
    }
}
