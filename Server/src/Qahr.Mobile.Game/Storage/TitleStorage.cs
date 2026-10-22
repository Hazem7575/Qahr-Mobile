using Qahr.Mobile.Game.MsgServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public class TitleStorage
    {

        public uint ID = 0;
        public uint SubID = 0;
        public string Name = "";
        public uint Score = 0;

        public static Dictionary<uint, TitleStorage> Titles = new Dictionary<uint, TitleStorage>();

        public static void LoadDBInformation()
        {

            string[] baseText = System.IO.File.ReadAllLines(Program.ServerConfig.DbLocation + "title_type.txt");
            foreach (var bas_line in baseText)
            {
                var line = bas_line.Split(new string[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                TitleStorage title = new TitleStorage();
                title.ID = uint.Parse(line[0]);
                title.SubID = uint.Parse(line[1]);
                title.Name = line[2];
                title.Score = uint.Parse(line[7]);
                Titles.Add(title.ID, title);
            }
        }

        public static void CheckUpUser(Client.GameClient user, ServerSockets.Packet stream)
        {
            foreach (var title in user.Player.TitlesWithTime)
            {
                TimeSpan timeSpan = title.DateStamp - DateTime.Now;
                if (DateTime.Now > title.DateStamp)
                {
                    user.Player.RemoveSpecialTitleTime(title.titleID, stream);
                    var name = Enum.GetName(typeof(MsgTitleStorage.AllTitle), title.titleID);
                    if (name != null)
                        user.SendSysMesage(string.Format("Your Title [{0}] Has Expired", name), MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                }
            }
            #region Halos
            if (user.Player.FootprintOpen == 1)
            {
                user.Player.Footprint.Clear();
                user.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.FireflyFlit), stream);
                user.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.BlazesFootprint), stream);
                user.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.BloodwingSpecter), stream);
                user.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.LotusGrace), stream);
                user.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.LuckyStarshine), stream);
                user.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.SakuraGrace), stream);
                user.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.ZookosFootprint), stream);
                user.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.BlazesFootprint1), stream);
                user.Player.AddSpecialFootprint((Game.MsgServer.MsgTitleStorage.Footprint.LotusGrace1), stream);


            }
            if (user.Player.HaloActionOpen == 1)
            {
                user.Player.HaloAction.Clear();
                user.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.SupremeAlone2), stream);
                user.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.BlazesAction3), stream);
                user.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.BlazesAction), stream);
                user.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.FallingSakura), stream);
                user.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.MaleficMoon), stream);
                user.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.SupremeAlone), stream);
                user.Player.AddSpecialHaloAction((Game.MsgServer.MsgTitleStorage.HaloAction.ZookosAction), stream);

            }
            #endregion
            #region WingsTQ
            #region MoonlightWings
            if (CoatStorage.AmountStarMount5tar(user, 5) >= 1)
            {
                user.Player.AddSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.MoonlightWings, stream);
            }
            else
                user.Player.RemoveSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.MoonlightWings, stream);
            #endregion
            MsgGameItem itemSupremeh;
            #region Supreme
            if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.Bottle, out itemSupremeh))
            {
                if (itemSupremeh.ITEM_ID == 2100095 && !user.Player.WingsTitles.Contains(Game.MsgServer.MsgTitleStorage.WingsType.Supreme))
                    user.Player.AddSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.Supreme, stream);
                else if (itemSupremeh.ITEM_ID != 2100095 && user.Player.WingsTitles.Contains(Game.MsgServer.MsgTitleStorage.WingsType.Supreme))
                    user.Player.RemoveSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.Supreme, stream);
            }
            #endregion
            #region FlutteringFlag(Flame)
            var info = Game.MsgTournaments.MsgArena.Top3.Where(p => p != null).FirstOrDefault();
            if (info != null && user.Player.UID == info.UID)
            {
                Client.GameClient Target;
                if (Pool.GamePoll.TryGetValue(info.UID, out Target))
                {
                    user.Player.AddSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.FlutteringFlag2, stream);
                }
            }
            else
            {
                user.Player.RemoveSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.FlutteringFlag2, stream);
            }
            #endregion
            //#region FlutteringFlag
            //var infoS = Game.MsgTournaments.MsgArena.Top10.Where(p => p != null).ToArray();
            //if (infoS.Length > 0)
            //{
            //    if (infoS[1].UID == user.Player.UID
            //        || infoS[2].UID == user.Player.UID
            //        || infoS[3].UID == user.Player.UID
            //        || infoS[4].UID == user.Player.UID
            //        || infoS[5].UID == user.Player.UID
            //        || infoS[6].UID == user.Player.UID
            //        || infoS[7].UID == user.Player.UID
            //        || infoS[8].UID == user.Player.UID
            //        || infoS[9].UID == user.Player.UID)
            //    {
            //        user.Player.AddSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.FlutteringFlag, stream);
            //    }
            //    else
            //    {
            //        user.Player.RemoveSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.FlutteringFlag, stream);
            //    }
            //}
            //#endregion
            #region VioletCloudWing
            if (!user.Player.WingsTitles.Contains(Game.MsgServer.MsgTitleStorage.WingsType.VioletCloudWing))
            {
                if (user.PrestigeLevel >= 324)
                {
                    user.Player.AddSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.VioletCloudWing, stream);
                }
            }
            #endregion
            #region VioletLightning
            if (!user.Player.WingsTitles.Contains(Game.MsgServer.MsgTitleStorage.WingsType.VioletLightning))
            {
                if (user.PrestigeLevel >= 216)
                {
                    user.Player.AddSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.VioletLightning, stream);
                }
            }
            #endregion
            #region StarlightWings
            if (CoatStorage.AmountStarGarments5tar(user, 5) >= 1)
            {
                user.Player.AddSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.StarlightWings, stream);
            }
            else
                user.Player.RemoveSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.StarlightWings, stream);
            #endregion
            #region WingsofInfernal
            if (!user.Player.WingsTitles.Contains(Game.MsgServer.MsgTitleStorage.WingsType.WingsofInfernal))
            {
                if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3] != null)
                {
                    if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8 != null
                        && Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8.Length > 0
                        && Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8[0].UID == user.Player.UID)
                    {
                        user.Player.AddSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.WingsofInfernal, stream);
                    }
                }
            }
            else
            {
                if (Game.MsgTournaments.MsgEliteTournament.EliteGroups[3] != null)
                {
                    if (!(Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8 != null
                        && Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8.Length > 0
                        && Game.MsgTournaments.MsgEliteTournament.EliteGroups[3].Top8[0].UID == user.Player.UID))
                    {
                        user.Player.RemoveSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.WingsofInfernal, stream);
                    }
                }
            }
            #endregion
            #region RadiantWings&&Victor
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Victor))
            {
                if (user.Player.MyGuild != null && user.Player.MyGuild.CTF_Rank == 1 && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                {
                    user.Player.AddSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.RadiantWings, stream);
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Victor, stream);
                }
            }
            else
            {
                if (!(user.Player.MyGuild != null && user.Player.MyGuild.CTF_Rank == 1 && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader))
                {
                    user.Player.RemoveSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.RadiantWings, stream);
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Victor, stream);
                }
            }
            #endregion
            #region FragrantWings
            if (user.Equipment.TryGetEquip(Role.Flags.ConquerItem.Bottle, out itemSupremeh))
            {
                if (itemSupremeh.ITEM_ID == 2100075 && !user.Player.WingsTitles.Contains(Game.MsgServer.MsgTitleStorage.WingsType.FragrantWings))
                    user.Player.AddSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.FragrantWings, stream);
                else if (itemSupremeh.ITEM_ID != 2100075 && user.Player.WingsTitles.Contains(Game.MsgServer.MsgTitleStorage.WingsType.FragrantWings))
                    user.Player.RemoveSpecialWings(Game.MsgServer.MsgTitleStorage.WingsType.FragrantWings, stream);
            }
            #endregion
            #endregion

            #region TitleTQ
            #region Legendary
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.LegendaryNew))
            {
                if (user.MyPrestigePoints >= 700000)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.LegendaryNew, stream);
                }
                else
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.LegendaryNew, stream);
                }
            }
            else
            {
                if (user.MyPrestigePoints >= 700000)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.LegendaryNew, stream);
                }
                else
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.LegendaryNew, stream);
                }
            }
            #endregion
            #region Superman
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Superman))
            {
                if (user.MyPrestigePoints >= 600000)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Superman, stream);
                }
                else
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Superman, stream);
                }
            }
            else
            {
                if (user.MyPrestigePoints >= 600000)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Superman, stream);
                }
                else
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Superman, stream);
                }
            }
            #endregion
            #region GrandmasterNew
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.GrandmasterNew))
            {
                if (user.MyPrestigePoints >= 500000)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.GrandmasterNew, stream);
                }
                else
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.GrandmasterNew, stream);
                }
            }
            else
            {
                if (user.MyPrestigePoints >= 500000)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.GrandmasterNew, stream);
                }
                else
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.GrandmasterNew, stream);
                }
            }
            #endregion
            #region Elite
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Elite))
            {
                if (user.MyPrestigePoints >= 400000)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Elite, stream);
                }
                else
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Elite, stream);
                }
            }
            else
            {
                if (user.MyPrestigePoints >= 400000)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Elite, stream);
                }
                else
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Elite, stream);
                }
            }
            #endregion
            #region Scholar
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Scholar))
            {
                if (user.MyPrestigePoints >= 400000)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Scholar, stream);
                }
                else
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Scholar, stream);
                }
            }
            else
            {
                if (user.MyPrestigePoints >= 400000)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Scholar, stream);
                }
                else
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Scholar, stream);
                }
            }
            #endregion
            #region ExpertNew
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.ExpertNew))
            {
                if (user.MyPrestigePoints >= 300000)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.ExpertNew, stream);
                }
                else
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.ExpertNew, stream);
                }
            }
            else
            {
                if (user.MyPrestigePoints >= 300000)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.ExpertNew, stream);
                }
                else
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.ExpertNew, stream);
                }
            }
            #endregion
            #region Trainee
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Trainee))
            {
                if (user.MyPrestigePoints >= 200000)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Trainee, stream);
                }
                else
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Trainee, stream);
                }
            }
            else
            {
                if (user.MyPrestigePoints >= 200000)
                {
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Trainee, stream);
                }
                else
                {
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Trainee, stream);
                }
            }
            #endregion
            #region RisingStar
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.RisingStar))
            {
                if (user.Player.Achievement != null)
                {
                    if (user.Player.Achievement.Score() >= 300)
                    {
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.RisingStar, stream);
                    }
                    else
                    {
                        user.Player.RemoveSpecialTitle(MsgTitleStorage.TitleType.RisingStar, stream);
                    }
                }
            }
            #endregion
            #region Conqueror
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Conqueror))
            {
                if (user.Player.MyGuild != null && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader)
                {
                    if (Game.MsgTournaments.MsgSchedules.GuildWar.Winner != null && Game.MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == user.Player.GuildID && Game.MsgTournaments.MsgSchedules.GuildWar.Proces == Game.MsgTournaments.ProcesType.Dead)
                        user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Conqueror, stream);
                }
            }
            else
            {
                if (!(user.Player.MyGuild != null && user.Player.MyGuildMember != null && user.Player.MyGuildMember.Rank == Role.Flags.GuildMemberRank.GuildLeader))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Conqueror, stream);
                if (!(Game.MsgTournaments.MsgSchedules.GuildWar.Winner != null && Game.MsgTournaments.MsgSchedules.GuildWar.Winner.GuildID == user.Player.GuildID
                    && Game.MsgTournaments.MsgSchedules.GuildWar.Proces == Game.MsgTournaments.ProcesType.Dead))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Conqueror, stream);
            }
            #endregion
            #region Talent
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Talent))
            {
                if (user.Player.MyJiangHu != null && user.Player.MyJiangHu.Inner_Strength >= 80000)
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Talent, stream);
            }
            else
            {
                if (!(user.Player.MyJiangHu != null && user.Player.MyJiangHu.Inner_Strength >= 80000))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Talent, stream);
            }
            #endregion
            #region Grandmaster
            if (!user.Player.SpecialTitles.Contains(Game.MsgServer.MsgTitleStorage.TitleType.Grandmaster))
            {
                if (user.Player.MyChi != null && user.Player.MyChi.AllScore() >= 1500)
                    user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Grandmaster, stream);
            }
            else
            {
                if (!(user.Player.MyChi != null && user.Player.MyChi.AllScore() >= 1500))
                    user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Grandmaster, stream);
            }
            #endregion
            #region Fashionist
            if (CoatStorage.AmountStarGarments(user, 4) >= 5)
            {
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Fashionist, stream);
            }
            else
                user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.Fashionist, stream);
            #endregion
            #region ChosenOne
            if (user.Rune.Full())
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.ChosenOne, stream);
            else
                user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.ChosenOne, stream);
            #endregion
            #region SwiftChaser
            if (CoatStorage.AmountStarMount(user, 4) >= 5)
            {
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SwiftChaser, stream);
            }
            else
                user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SwiftChaser, stream);
            #endregion
            #region MonkeyRider
            if (CoatStorage.AmountMonkeyMounts(user, 0) >= 1)
            {
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.MonkeyRider, stream);
            }
            else
                user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.MonkeyRider, stream);
            #endregion
            #region SaintRider
            if (CoatStorage.AmountMonkeyMounts(user, 0) >= 8)
            {
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SaintRider, stream);
            }
            else
                user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SaintRider, stream);
            #endregion
            #region LunarRider
            if (user.MyWardrobe.Items[(byte)Role.Instance.Wardrobe.ItemsType.Mount].Values.Count(i => i.ITEM_ID == 200560) > 0)
            {
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.LunarRider, stream);
            }
            else
                user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.LunarRider, stream);
            #endregion
            #region SolarRider
            if (user.MyWardrobe.Items[(byte)Role.Instance.Wardrobe.ItemsType.Mount].Values.Count(i => i.ITEM_ID == 200559) > 0)
            {
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SolarRider, stream);
            }
            else
                user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.SolarRider, stream);
            #endregion
            #region ImmortalFate
            if (user.MyWardrobe.Items[(byte)Role.Instance.Wardrobe.ItemsType.Mount].Values.Count(i => i.ITEM_ID == 200595 || i.ITEM_ID == 200596) > 0)
            {
                user.Player.AddSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.ImmortalFate, stream);
            }
            else
                user.Player.RemoveSpecialTitle(Game.MsgServer.MsgTitleStorage.TitleType.ImmortalFate, stream);
            #endregion
            if (user.Player.Name.Contains(""))
            {

                user.Player.AddSpecialKillEffectsAction((Game.MsgServer.MsgTitleStorage.KillEffects)10000, stream);
                user.Player.AddSpecialKillEffectsAction((Game.MsgServer.MsgTitleStorage.KillEffects)10001, stream);
                user.Player.AddSpecialKillEffectsAction((Game.MsgServer.MsgTitleStorage.KillEffects)10002, stream);
                user.Player.AddSpecialKillEffectsAction((Game.MsgServer.MsgTitleStorage.KillEffects)10003, stream);
                user.Player.AddSpecialKillEffectsAction((Game.MsgServer.MsgTitleStorage.KillEffects)10005, stream);



            }
            #endregion
        }
    }
}
