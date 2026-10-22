using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qahr.Mobile.Insults
{
    public class InsultManager : Dictionary<string, Insult>
    {
        public void VoteNotInsult(string text)
        {
            Insult currentWorking;
            if (TryGetValue(text, out currentWorking))
            {
                currentWorking.Votedfalse = (byte)(currentWorking.Votedtrue + 1);
            }
            else
            {
                Add(text, new Insult() { Text = text, Votedtrue = 0, Votedfalse = 1 });
            }
        }
        public void VoteInsult(string text)
        {
            Insult currentWorking;
            if (TryGetValue(text, out currentWorking))
            {
                currentWorking.Votedtrue += 1;
            }
            else
            {
                Add(text, new Insult() { Text = text, Votedtrue = 1, Votedfalse = 0 });
            }
        }
        public void CheckInsults(ref string Message)
        {
            string newMessage = "";
            foreach (Insult insult in Values)
            {
                if (Message.Contains(insult.Text))
                {
                    if (insult.Votedtrue >= insult.Votedfalse)
                    {
                        newMessage = Message.Replace(insult.Text, "***");
                    }
                }
            }
            if (newMessage == "")
            {


                return;
            }
            Message = newMessage;
        }
        public void Load()
        {
            unsafe
            {
                if (!File.Exists(Program.ServerConfig.DbLocation + "\\Insults.bin"))
                    File.Create(Program.ServerConfig.DbLocation + "\\Insults.bin").Dispose();
                WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                if (binary.Open(Program.ServerConfig.DbLocation + "\\Insults.bin", System.IO.FileMode.Open))
                {
                    Insult insult;
                    int count;
                    binary.Read(&count, sizeof(int));
                    for (int x = 0; x < count; x++)
                    {
                        binary.Read(&insult, sizeof(Insult));
                        Add(insult.Text, insult);
                    }
                    binary.Close();
                }
            }
        }
        public void Save()
        {
            unsafe
            {
                WindowsAPI.BinaryFile binary = new WindowsAPI.BinaryFile();
                if (binary.Open(Program.ServerConfig.DbLocation + "\\Insults.bin", System.IO.FileMode.Create))
                {
                    int count = Values.Count;
                    Insult workinginsult;
                    binary.Write(&count, sizeof(int));
                    foreach (Insult insult in Values)
                    {
                        workinginsult = insult;
                        binary.Write(&workinginsult, sizeof(Insult));
                    }
                }
                binary.Close();
            }
        }
    }
}
