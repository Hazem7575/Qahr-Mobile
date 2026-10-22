using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
   public class LeagueTable
    {

       public static void Load()
       {
           WindowsAPI.IniFile reader = new WindowsAPI.IniFile("");
           foreach (string fname in System.IO.Directory.GetFiles(Program.ServerConfig.DbLocation + "\\Unions\\"))
           {
               reader.FileName = fname;
               Role.Instance.Union union = new Role.Instance.Union();
               union.UID = reader.ReadUInt32("Info", "UID", 0);
               union.Name = reader.ReadString("Info", "Name", "None");
               union.Emperor = reader.ReadString("Info", "Emperor", "None");
               union.GoldBrick = reader.ReadUInt32("Info", "GoldBrick", 0);
               union.Treasury = reader.ReadUInt64("Info", "Treasury", 0);
               union.Stipend = reader.ReadUInt32("Info", "Stipend", 0);
               union.EmperrorUID = reader.ReadUInt32("Info", "EmperrorUID", 0);
               union.IsKingdom = reader.ReadByte("Info", "IsKingdom", 0);
               union.PlunderID = reader.ReadUInt32("Info", "PlunderID", 0);
               union.InvadingID = reader.ReadUInt32("Info", "InvadingID", 0);
               union.Bulletin = reader.ReadString("Info", "Bulletin", "None");
               union.Title = reader.ReadString("Info", "Title", "None");
               union.PlunderTarget = reader.ReadString("Info", "PlunderTarget", "None");
               union.InvadingUnion = reader.ReadString("Info", "InvadingUnion", "None");
               union.RecruitDeclaration = reader.ReadString("Info", "RecruitDeclaration", "None");
               if (Role.Instance.Union.UnionPoll.ContainsKey(union.UID) == false)
                   Role.Instance.Union.UnionPoll.TryAdd(union.UID, union);
           }

       }
       public static void Save()
       {
           var array = Role.Instance.Union.UnionPoll.Values.Where(p => p.CanSave).ToArray();
           foreach (var union in array)
           {
               WindowsAPI.IniFile write = new WindowsAPI.IniFile("\\Unions\\" + union.UID + ".ini");
               write.Write<uint>("Info", "UID", union.UID);
               write.WriteString("Info", "Name", union.Name);
               write.WriteString("Info", "Emperor", union.Emperor);
               write.Write<uint>("Info", "GoldBrick", union.GoldBrick);
               write.Write<ulong>("Info", "Treasury", union.Treasury);
               write.Write<uint>("Info", "Stipend", union.Stipend);
               write.Write<uint>("Info", "EmperrorUID", union.EmperrorUID);
               write.Write<uint>("Info", "IsKingdom", union.IsKingdom);
               write.Write<uint>("Info", "PlunderID", union.PlunderID);
               write.Write<uint>("Info", "InvadingID", union.InvadingID);
               write.WriteString("Info", "Bulletin", union.Bulletin);
               write.WriteString("Info", "Title", union.Title);
               write.WriteString("Info", "PlunderTarget", union.PlunderTarget);
               write.WriteString("Info", "InvadingUnion", union.InvadingUnion);
               write.WriteString("Info", "RecruitDeclaration", union.RecruitDeclaration);
               global::Qahr.Mobile.Database.Storage.SocialStorage.OnUnionSaved(union.UID);
           }
       }
    }
}
