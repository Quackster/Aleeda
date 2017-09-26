using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using Aleeda.Core;
using Aleeda.Storage;
using Aleeda.HabboHotel.Client;
using Aleeda.Net.Messages;

namespace Aleeda.HabboHotel.Cache.Rooms.Misc
{
    class Commands
    {
        public List<String> Command;
        public GameClient Session;
        private ServerMessage Response;
        private string[] args;
        private string command;

        public Commands()
        {
            Command = new List<String>();
            Response = new ServerMessage(0);

            string[] arr = { "send" , "manip_chat", "ha", "reload_bots", "reload_bot", "roomalert", "change_desc", "change_name", "add_tag", "remove_tag", "remove_tags", "get_coords" };

            foreach (string command in arr)
            {
                Command.Add(command);
            }
        }
        public void InitCommand(GameClient Session, string[] args, string command)
        {
            this.Session = Session;

            if (Command.Contains(args[0]))
            {
                this.args = args;
                this.command = command;

                MethodInfo Info = typeof(Commands).GetMethod(args[0]);
                Info.Invoke(this, null);
            }

        }
        #region Commands
        public void send()
        {
            GameClient User = AleedaEnvironment.GetHabboHotel().GetClients().GetClientOfHabbo(AleedaEnvironment.GetHabboHotel().GetHabbos().GetHabbo(args[1]).ID);

            if (User.GetHabbo().HasFuse("fuse_admin") || User.GetHabbo().HasFuse("fuse_sysadmin"))
            {
                int mID = OldB64.Decode(args[2].Substring(0, 2));

                int mMessageLength = args[0].Length + args[1].Length + 4;
                string mMessage = command.Substring(mMessageLength);

                Response.Initialize((uint)mID);
                Response.Append(mMessage);
                User.GetConnection().SendMessage(Response);
            }
        }
        public void manip_chat()
        {
            if (Session.GetHabbo().HasFuse("fuse_admin") || Session.GetHabbo().HasFuse("fuse_sysadmin"))
            {
                int mMessageLength = args[0].Length + args[1].Length + 2;
                string mMessage = command.Substring(mMessageLength);

                GameClient User = AleedaEnvironment.GetHabboHotel().GetClients().GetClientOfHabbo(AleedaEnvironment.GetHabboHotel().GetHabbos().GetHabbo(args[1]).ID);
                GameClient.ManipulateChat(User.GetHabbo().Username, mMessage);
            }
        }
        public void ha()
        {
            if (Session.GetHabbo().HasFuse("fuse_admin") || Session.GetHabbo().HasFuse("fuse_sysadmin"))
            {
                using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
                {
                    int mLength = args[0].Length + 1;
                    string mMessage = command.Substring(mLength);

                    Response.Initialize(139); // "BK"
                    Response.Append(mMessage);
                    Server.SendHotelMsg(Response);
                }
            }
        }
        public void reload_bots()
        {
            if (Session.GetHabbo().HasFuse("fuse_admin") || Session.GetHabbo().HasFuse("fuse_sysadmin"))
            {
                AleedaEnvironment.GetCache().GetRoomBots().Reload();
            }
        }
        public void reload_bot()
        {
            if (Session.GetHabbo().HasFuse("fuse_admin") || Session.GetHabbo().HasFuse("fuse_sysadmin"))
            {
                AleedaEnvironment.GetCache().GetRoomBots().ReloadBotID(int.Parse(args[1]));
            }
        }
        public void roomalert()
        {
            if (Session.GetHabbo().HasFuse("fuse_admin") || Session.GetHabbo().HasFuse("fuse_sysadmin") || Session.GetHabbo().HasFuse("fuse_mod"))
            {
                int mLength = args[0].Length + 1;
                string mMessage = command.Substring(mLength);

                ServerMessage msg = new ServerMessage(139); // "BK"
                msg.Append(mMessage);
                Session.GetMessageHandler().SendRoom(msg);
            }
        }
        public void change_desc()
        {
            if (Session.GetHabbo().HasFuse("fuse_admin") || Session.GetHabbo().HasFuse("fuse_sysadmin") || Session.GetHabbo().HasFuse("fuse_mod") || RoomUser.CalculateRights(Session.GetHabbo().RoomId, Session.GetHabbo().ID))
            {
                int Length = args[0].Length + 1;
                string Message = command.Substring(Length);

                using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
                {
                    dbClient.AddParamWithValue("description", Message);
                    dbClient.ExecuteQuery("UPDATE private_rooms SET description = @description WHERE id = '" + Session.GetHabbo().RoomId + "'");
                }

                //Set the variable
                AleedaEnvironment.GetCache().GetPrivateRooms().getRoom(Session.GetHabbo().RoomId).Description = Message;

                ServerMessage msg = new ServerMessage(139); // "BK"
                msg.Append("The description has been changed! Please reload the room if you want.");
                Session.GetMessageHandler().SendRoom(msg);
            }
        }
        public void change_name()
        {
            if (Session.GetHabbo().HasFuse("fuse_admin") || Session.GetHabbo().HasFuse("fuse_sysadmin") || Session.GetHabbo().HasFuse("fuse_mod") || RoomUser.CalculateRights(Session.GetHabbo().RoomId, Session.GetHabbo().ID))
            {
                int Length = args[0].Length + 1;
                string Message = command.Substring(Length);

                using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
                {
                    dbClient.AddParamWithValue("name", Message);
                    dbClient.ExecuteQuery("UPDATE private_rooms SET name = @name WHERE id = '" + Session.GetHabbo().RoomId + "'");
                }

                //Set the variable
                AleedaEnvironment.GetCache().GetPrivateRooms().getRoom(Session.GetHabbo().RoomId).Name = Message;
                ServerMessage msg = new ServerMessage(139); // "BK"
                msg.Append("The name of the room has been changed! Please reload the room if you want.");
                Session.GetMessageHandler().SendRoom(msg);
            }
        }
        public void add_tag()
        {
            if (Session.GetHabbo().HasFuse("fuse_admin") || Session.GetHabbo().HasFuse("fuse_sysadmin") || Session.GetHabbo().HasFuse("fuse_mod") || RoomUser.CalculateRights(Session.GetHabbo().RoomId, Session.GetHabbo().ID))
            {
                int Length = args[0].Length + 1;
                string Tag = command.Substring(Length);
                string CurrentTags = "";
                int CurrentTagCount = 0;

                using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
                {
                    CurrentTags = dbClient.ReadString("SELECT tags FROM private_rooms WHERE id = '" + Session.GetHabbo().RoomId + "'");

                    foreach (string tag in CurrentTags.Split(','))
                    {
                        ++CurrentTagCount;
                    }

                    if (CurrentTagCount != 4)
                    {
                        CurrentTags = Tag + "," + CurrentTags;

                        dbClient.AddParamWithValue("tag", CurrentTags);
                        dbClient.ExecuteQuery("UPDATE private_rooms SET tags = @tag WHERE id = '" + Session.GetHabbo().RoomId + "'");
                    }
                    //Set the variable
                    AleedaEnvironment.GetCache().GetPrivateRooms().getRoom(Session.GetHabbo().RoomId).Tags = CurrentTags;

                    ServerMessage msg = new ServerMessage(139); // "BK"
                    msg.Append("Added more tags!. ( " + Tag + " )");
                    Session.GetMessageHandler().SendRoom(msg);
                }
            }
        }
        public void remove_tag()
        {
            if (Session.GetHabbo().HasFuse("fuse_admin") || Session.GetHabbo().HasFuse("fuse_sysadmin") || Session.GetHabbo().HasFuse("fuse_mod") || RoomUser.CalculateRights(Session.GetHabbo().RoomId, Session.GetHabbo().ID))
            {
                int Length = args[0].Length + 1;
                string Tag = command.Substring(Length);

                using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
                {
                    string CurrentTags = dbClient.ReadString("SELECT tags FROM private_rooms WHERE id = '" + Session.GetHabbo().RoomId + "'");

                    CurrentTags = CurrentTags.Replace(Tag + ",", "");

                    dbClient.AddParamWithValue("tag", CurrentTags);
                    dbClient.ExecuteQuery("UPDATE private_rooms SET tags = @tag WHERE id = '" + Session.GetHabbo().RoomId + "'");

                    //Set the variable
                    AleedaEnvironment.GetCache().GetPrivateRooms().getRoom(Session.GetHabbo().RoomId).Tags = CurrentTags;

                    ServerMessage msg = new ServerMessage(139); // "BK"
                    msg.Append("Removed the tag ( " + Tag + " )");
                    Session.GetMessageHandler().SendRoom(msg);
                }
            }
        }
        public void remove_tags()
        {
            if (Session.GetHabbo().HasFuse("fuse_admin") || Session.GetHabbo().HasFuse("fuse_sysadmin") || Session.GetHabbo().HasFuse("fuse_mod") || RoomUser.CalculateRights(Session.GetHabbo().RoomId, Session.GetHabbo().ID))
            {
                using (DatabaseClient dbClient = AleedaEnvironment.GetDatabase().GetClient())
                {
                    dbClient.ExecuteQuery("UPDATE private_rooms SET tags = '' WHERE id = '" + Session.GetHabbo().RoomId + "'");

                    //Set the variable
                    AleedaEnvironment.GetCache().GetPrivateRooms().getRoom(Session.GetHabbo().RoomId).Tags = "";

                    ServerMessage msg = new ServerMessage(139); // "BK"
                    msg.Append("Removed all tags");
                    Session.GetMessageHandler().SendRoom(msg);
                }
            }
        }
        public void get_coords()
        {
            ServerMessage msg = new ServerMessage(139); // "BK"
            msg.Append("X: " + Session.GetHabbo().X + "\n");
            msg.Append("Y: " + Session.GetHabbo().X + "\n");
            Session.GetConnection().SendMessage(msg);

        }

        #endregion
    }
}
