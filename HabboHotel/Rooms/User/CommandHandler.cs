using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Aleeda.Storage;
using Aleeda.HabboHotel.Client;
using Aleeda.Net.Messages;
using Aleeda.Core;

namespace Aleeda.HabboHotel.Rooms
{
    public class Commands
    {
        public static ClientMessageHandler mRoom;
        public static void CommandHandler(string command, ServerMessage Response, GameClient Session)
        {
            try
            {
                string[] args = command.Split(' ');

                switch (args[0])
                {
                    case ":send":
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
                        break;
                    case ":manip_chat":
                        {
                            if (Session.GetHabbo().HasFuse("fuse_admin") || Session.GetHabbo().HasFuse("fuse_sysadmin"))
                            {
                                int mMessageLength = args[0].Length + args[1].Length + 2;
                                string mMessage = command.Substring(mMessageLength);

                                GameClient User = AleedaEnvironment.GetHabboHotel().GetClients().GetClientOfHabbo(AleedaEnvironment.GetHabboHotel().GetHabbos().GetHabbo(args[1]).ID);
                                GameClient.ManipulateChat(User.GetHabbo().Username, mMessage);
                            }
                        }
                        break;
                    case ":ha":
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
                        break;
                    case ":reload_bots":
                        {
                            if (Session.GetHabbo().HasFuse("fuse_admin") || Session.GetHabbo().HasFuse("fuse_sysadmin"))
                            {
                                AleedaEnvironment.GetCache().GetRoomBots().Reload();
                            }
                        }
                        break;
                    case ":reload_bot":
                        {
                            if (Session.GetHabbo().HasFuse("fuse_admin") || Session.GetHabbo().HasFuse("fuse_sysadmin"))
                            {
                                AleedaEnvironment.GetCache().GetRoomBots().ReloadBotID(int.Parse(args[1]));
                            }
                        }
                        break;
                    case ":roomalert":
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
                        break;
                    case ":change_desc":
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
                        break;
                    case ":change_name":
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
                        break;
                    case ":add_tag":
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
                        break;
                    case ":remove_tag":
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
                        break;
                    case ":remove_tags":
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
                        break;
                    case ":get_coords":
                        {
                            ServerMessage msg = new ServerMessage(139); // "BK"
                            msg.Append("X: " + Session.GetHabbo().X + "\n");
                            msg.Append("Y: " + Session.GetHabbo().X + "\n");
                            Session.GetConnection().SendMessage(msg);

                        }
                        break;
                    default:
                        {

                            ServerMessage msg = new ServerMessage(24); // "@X"
                            msg.AppendInt32(Session.GetHabbo().UnitId);
                            msg.AppendString(command);
                            msg.AppendInt32(0);
                            new ClientMessageHandler(Session).SendRoom(msg);
                        }
                        break;
                }
            }
            catch { }
        }
    }
}
