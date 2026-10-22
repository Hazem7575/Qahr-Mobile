using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.DBFunctionality;
using Qahr.Mobile.Game.MsgNpc;
using AdoCommand = MySqlConnector.MySqlCommand;

namespace Qahr.Mobile.Game.MsgNpc
{
    public class Dialog
    {
        private Client.GameClient client;
        public ServerSockets.Packet stream;

        public Dialog(Client.GameClient Client,ServerSockets.Packet _stream)
        {
            stream = _stream;
            client = Client;
        }

        public Dialog CreateMessageBox(string Text)
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.MessageBox, Text, ushort.MaxValue, 0, true));
            return this;
        }

        public Dialog AddText(string text)
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Dialog, text, 0, 0));
            return this;
        }

        public Dialog Text(string text)
        {
            return AddText(text);
        }

        public Dialog Option(string text, byte id)
        {
            return AddOption(text, id);
        }

        public Dialog Avatar(ushort id)
        {
            return AddAvatar(id);
        }
        public Dialog AddAvatar(ushort id)
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Avatar, "", id, 0));
            return this;
        }
        public Dialog AddPage(string url)
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Page, url, 0, 255));
            return this;
        }
        public Dialog AddOption(string text, byte id)
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Option, text, 0, id));
            return this;
        }
        public Dialog AddOption(string text)
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Option, text, 0, 255));
            return this;
        }
        public Dialog AddInput(string text, byte id, byte maxCharacters = 16)
        {
            client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Input, text, maxCharacters, id));
            return this;
        }
        public unsafe void FinalizeDialog(bool messagebox = false)
        {
            if (!messagebox)
                client.Send(stream.NpcReplyCreate(NpcReply.InteractTypes.Finish, "", 0, 0, false));
        }
        public static bool Exists(string username)
        {
            try
            {
                // [sqli] parameterized (was the string-building DBFunctionality.MySqlCommand builder); same row test.
                using (var conn = DataHolder.MySqlConnection)
                using (var cmd = new AdoCommand("SELECT * FROM `accounts` WHERE `Username` = @username", conn))
                {
                    cmd.Parameters.AddWithValue("@username", username ?? "");
                    cmd.CommandTimeout = 30;
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }
    }
}
