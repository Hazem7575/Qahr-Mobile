using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Game.MsgTournaments
{
    public class MsgTowerOfMystery
    {
        public enum RewardTypes : byte
        {
            Normal = 0,
            Refined = 1,
            Unique = 2,
            Elite = 3,
            Super = 5,
            Count = 6

        }
       

        public Game.MsgMonster.MonsterFamily GetMonster(Client.GameClient user)
        {
            Game.MsgMonster.MonsterFamily monster = null;

            switch (user.Player.JoinTowerOfMysteryLayer)
            {
                case 0:
                    Pool.MonsterFamilies.TryGetValue((uint)(7976 + user.Player.TOMChallengeToday * 10), out monster);
                    break;
                case 1:
                    Pool.MonsterFamilies.TryGetValue((uint)(7977 + user.Player.TOMChallengeToday * 10), out monster);
                    break;
                case 2:
                    Pool.MonsterFamilies.TryGetValue((uint)(7979 + user.Player.TOMChallengeToday * 10), out monster);
                    break;
                case 3:
                    Pool.MonsterFamilies.TryGetValue((uint)(7980 + user.Player.TOMChallengeToday * 10), out monster);
                    break;
                case 4:
                    Pool.MonsterFamilies.TryGetValue((uint)(7981 + user.Player.TOMChallengeToday * 10), out monster);
                    break;
                case 5:
                    Pool.MonsterFamilies.TryGetValue((uint)(7982 + user.Player.TOMChallengeToday * 10), out monster);
                    break;
                case 6:
                    Pool.MonsterFamilies.TryGetValue((uint)(7983 + user.Player.TOMChallengeToday * 10), out monster);
                    break;
                case 7:
                    Pool.MonsterFamilies.TryGetValue((uint)(7984 + user.Player.TOMChallengeToday * 10), out monster);
                    break;
                default:
                    Pool.MonsterFamilies.TryGetValue((uint)(7985 + user.Player.TOMChallengeToday * 10), out monster);
                    break;
            }
            return monster;
        }
        public void GenerateReward(Client.GameClient user)
        {
            user.Player.TOMRefreshReward += 1;
            if (Role.Core.Rate(10, 100))
            {
                user.Player.TOM_Reward = RewardTypes.Super;
            }
            else if (Role.Core.Rate(20, 100))
                user.Player.TOM_Reward = RewardTypes.Elite;
            else if (Role.Core.Rate(30, 100))
                user.Player.TOM_Reward = RewardTypes.Unique;
            else if (Role.Core.Rate(40, 100))
                user.Player.TOM_Reward = RewardTypes.Refined;
            else
                user.Player.TOM_Reward = RewardTypes.Normal;

        }

        public void JoinLayer(Client.GameClient user, ushort x, ushort y, byte layer, ServerSockets.Packet stream)
        {
            layer = (byte)Math.Min(8, (int)layer);
            user.Player.JoinTowerOfMysteryLayer = layer;
            user.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");

            user.Player.TOM_StartChallenge = false;
            user.Player.TOM_FinishChallenge = false;
            switch (Math.Min(8, (int)layer))
            {
                //case 0:
                //    {
                //        var DBmap = Pool.ServerMaps[4000];
                //        user.Teleport(x, y, DBmap.ID, DBmap.GenerateDynamicID());
                //        if (user.Player.TOMChallengeToday == 0)
                //            user.SendSysMesage("You've already arrived (Common) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, GreatPheasant(common), was sealed. Be careful!", MsgMessage.ChatMode.System);
                //        else
                //            user.SendSysMesage("You've already arrived (Elite) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, GreatPheasant(Elite), was sealed. Be careful!", MsgMessage.ChatMode.System);
                //        break;
                //    }
                //case 1:
                //    {
                //        var DBmap = Pool.ServerMaps[4000];
                //        user.Teleport(x, y, DBmap.ID, DBmap.GenerateDynamicID());
                //        if (user.Player.TOMChallengeToday == 0)
                //            user.SendSysMesage("You've already arrived (Common) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, WingedLord(common), was sealed. Be careful!", MsgMessage.ChatMode.System);
                //        else
                //            user.SendSysMesage("You've already arrived (Elite) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, WingedLord(Elite), was sealed. Be careful!", MsgMessage.ChatMode.System);
                //        break;
                //    }
                //case 2:
                //    {
                //        var DBmap = Pool.ServerMaps[4000];
                //        user.Teleport(x, y, DBmap.ID, DBmap.GenerateDynamicID());
                //        if (user.Player.TOMChallengeToday == 0)
                //            user.SendSysMesage("You've already arrived (Common) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, WaterTerror(common), was sealed. Be careful!", MsgMessage.ChatMode.System);
                //        else
                //            user.SendSysMesage("You've already arrived (Elite) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, WaterTerror(Elite), was sealed. Be careful!", MsgMessage.ChatMode.System);
                //        break;
                //    }
                case 3:
                    {

                        var DBmap = Pool.ServerMaps[4003];
                        user.Teleport(x, y, DBmap.ID, DBmap.GenerateDynamicID());
                        if (user.Player.TOMChallengeToday == 0)
                            user.SendSysMesage("You've already arrived (Common) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, RuthlessAsura(common), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        else
                            user.SendSysMesage("You've already arrived (Elite) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, RuthlessAsura(Elite), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        break;
                    }
                case 4:
                    {
                        var DBmap = Pool.ServerMaps[4003];
                        user.Teleport(x, y, DBmap.ID, DBmap.GenerateDynamicID());
                        if (user.Player.TOMChallengeToday == 0)
                            user.SendSysMesage("You've already arrived (Common) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, SoulStrangler(common), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        else
                            user.SendSysMesage("You've already arrived (Elite) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, SoulStrangler(Elite), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        break;
                    }
                case 5:
                    {
                        var DBmap = Pool.ServerMaps[4003];
                        user.Teleport(x, y, DBmap.ID, DBmap.GenerateDynamicID());
                        if (user.Player.TOMChallengeToday == 0)
                            user.SendSysMesage("You've already arrived (Common) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, DarkGlutton(common), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        else
                            user.SendSysMesage("You've already arrived (Elite) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, DarkGlutton(Elite), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        break;
                    }
                case 6:
                    {
                        var DBmap = Pool.ServerMaps[4006];
                        user.Teleport(x, y, DBmap.ID, DBmap.GenerateDynamicID());
                        if (user.Player.TOMChallengeToday == 0)
                            user.SendSysMesage("You've already arrived (Common) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, AlienDragon(common), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        else
                            user.SendSysMesage("You've already arrived (Elite) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, AlienDragon(Elite), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        break;
                    }
                case 7:
                    {
                        var DBmap = Pool.ServerMaps[4006];
                        user.Teleport(x, y, DBmap.ID, DBmap.GenerateDynamicID());
                        if (user.Player.TOMChallengeToday == 0)
                            user.SendSysMesage("You've already arrived (Common) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, ShadowSpider(common), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        else
                            user.SendSysMesage("You've already arrived (Elite) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, ShadowSpider(Elite), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        break;
                    }
                case 8:
                    {
                        var DBmap = Pool.ServerMaps[4008];
                        user.Teleport(x, y, DBmap.ID, DBmap.GenerateDynamicID());
                        if (user.Player.TOMChallengeToday == 0)
                            user.SendSysMesage("You've already arrived (Common) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, FlameGiant(common), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        else
                            user.SendSysMesage("You've already arrived (Elite) in Tower of Mystery " + (layer + 1).ToString() + "F when an aggresive devil, FlameGiant(Elite), was sealed. Be careful!", MsgMessage.ChatMode.System);
                        break;
                    }
            }
        }
    }
}
