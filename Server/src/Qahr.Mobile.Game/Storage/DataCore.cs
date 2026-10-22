using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Qahr.Mobile.Role;

namespace Qahr.Mobile.Database
{
    public class DataCore
    {

        public static AtributesStatus AtributeStatus = new AtributesStatus();

        public static void SetCharacterSides(Player Player)
        {
            switch (Player.Body)
            {
                case 1008:
                    if (AtributesStatus.IsTrojan(Player.Class))
                        Player.Face = (ushort)Pool.GetRandom.Next(1, 102);
                    if (AtributesStatus.IsWarrior(Player.Class))
                        Player.Face = (ushort)Pool.GetRandom.Next(1, 102);
                    if (AtributesStatus.IsArcher(Player.Class))
                        Player.Face = (ushort)Pool.GetRandom.Next(1, 102);
                    if (AtributesStatus.IsNinja(Player.Class))
                        Player.Face = (ushort)Pool.GetRandom.Next(103, 107);
                    if (AtributesStatus.IsMonk(Player.Class))
                        Player.Face = (ushort)Pool.GetRandom.Next(109, 133);
                    if (AtributesStatus.IsPirate(Player.Class))
                        Player.Face = (ushort)Pool.GetRandom.Next(154, 158);
                    if (AtributesStatus.IsLee(Player.Class))
                        Player.Face = (ushort)Pool.GetRandom.Next(164, 168);
                    if (AtributesStatus.IsThunderStriker(Player.Class))
                        Player.Face = (ushort)Pool.GetRandom.Next(184, 193);
                    if (AtributesStatus.IsTaoist(Player.Class))
                        Player.Face = (ushort)Pool.GetRandom.Next(1, 102);
                    if (AtributesStatus.IsWindWalker(Player.Class))
                    {
                        Player.Face = (ushort)Pool.GetRandom.Next(174, 178);
                        break;
                    }
                    Player.Face = (ushort)296;
                    break;
                case 2007:
                    if (AtributesStatus.IsTrojan(Player.Class))
                        Player.Face = (ushort)Pool.GetRandom.Next(201, 290);
                    if (AtributesStatus.IsWarrior(Player.Class))
                        Player.Face = (ushort)Pool.GetRandom.Next(201, 290);
                    if (AtributesStatus.IsArcher(Player.Class))
                        Player.Face = (ushort)Pool.GetRandom.Next(201, 290);
                    if (AtributesStatus.IsNinja(Player.Class))
                        Player.Face = (ushort)Pool.GetRandom.Next(291, 295);
                    if (AtributesStatus.IsMonk(Player.Class))
                        Player.Face = (ushort)Pool.GetRandom.Next(300, 304);
                    if (AtributesStatus.IsPirate(Player.Class))
                        Player.Face = (ushort)Pool.GetRandom.Next(345, 349);
                    if (AtributesStatus.IsLee(Player.Class))
                        Player.Face = (ushort)Pool.GetRandom.Next(355, 359);
                    if (AtributesStatus.IsThunderStriker(Player.Class))
                        Player.Face = (ushort)Pool.GetRandom.Next(375, 384);
                    if (AtributesStatus.IsTaoist(Player.Class))
                        Player.Face = (ushort)Pool.GetRandom.Next(201, 290);
                    if (AtributesStatus.IsWindWalker(Player.Class))
                    {
                        Player.Face = (ushort)Pool.GetRandom.Next(365, 369);
                        break;
                    }
                    Player.Face = (ushort)296;
                    break;
            }
        }
       
        public static void LoadClient(Role.Player player)
        {
          
            player.Owner.Rune = new Role.Instance.Rune(player.Owner);
            player.Owner.Inventory = new Role.Instance.Inventory(player.Owner);
            player.Owner.Equipment = new Role.Instance.Equip(player.Owner);
            player.Owner.Warehouse = new Role.Instance.Warehouse(player.Owner);
            player.Owner.Collection = new Role.Instance.CollectionStorge(player.Owner);
            player.Owner.MyProfs = new Role.Instance.Proficiency(player.Owner);
            player.Owner.MyMail = new Role.Instance.Mail(player.Owner);
            player.Owner.MySpells = new Role.Instance.Spell(player.Owner);
            player.Owner.MainRelic = new Qahr.Mobile.MY_System.Relic.MainRelic(player.Owner);

            if (player.Owner.Achievement == null)
                player.Owner.Achievement = new AchievementCollection();

            player.Achievement = new Game.MsgServer.ClientAchievement(player.Owner.Achievement.Value, player.UID);

        }
    }
}
