//using Qahr.Mobile.Client;
//using Qahr.Mobile.Database;
//using Qahr.Mobile.Game.MsgServer;
//using Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate;
//using Qahr.Mobile.Game.MsgTournaments;
//using Qahr.Mobile.Role;
//using Qahr.Mobile.Role.Instance;
//using Qahr.Mobile.ServerSockets;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Time32 = System.Time32;

//namespace Qahr.Mobile
//{

//    public enum BotType
//    {
//        HeroBot,
//        QualifierBot,
//        Bots,
//        EventBots,
//    }
//    public enum BotLevel
//    {
//        BP300 = 0,
//        BP350 = 1,
//        BP400 = 2,
//        BP450 = 3,
//        BP480 = 4,
//        BP500 = 5,
//        FullBP = 6
//    }
//    public enum BotState
//    {
//        Find = 0,
//        Jump = 1,
//        Attacking = 2,
//        Chat = 3,
//        Qualifier = 4,
//        EventBots=5,

//    }
//    public class Config
//    {
//        public DateTime LastJump, LastAttack, LastTalk;
//        public uint UID, TargetUID, JumpStamp, AttackStamp, HitRate;
//    }
//    public unsafe class BotAttack
//    {

//        public static uint GetSpell(GameClient AI)
//        {
//            #region Trojan
//            if (AtributesStatus.IsTrojan(AI.Player.Class))
//            {
//                uint[] Spell = { (uint)Qahr.Mobile.Role.Flags.SpellID.ChaoticDance,(uint)Qahr.Mobile.Role.Flags.SpellID.FastBlader, (ushort)Qahr.Mobile.Role.Flags.SpellID.ScrenSword, (ushort)Qahr.Mobile.Role.Flags.SpellID.Hercules, (ushort)Qahr.Mobile.Role.Flags.SpellID.FatalCross , (ushort)Qahr.Mobile.Role.Flags.SpellID.Celestial  };
//                return Spell[Program.GetRandom.Next(0, Spell.Length)];
//            }
//            #endregion

//            #region Warrior
//            if (AtributesStatus.IsWarrior(AI.Player.Class))
//            {
//                uint[] Spell = { (uint)Qahr.Mobile.Role.Flags.SpellID.ChaoticDance, (uint)Qahr.Mobile.Role.Flags.SpellID.WaveofBlood, (ushort)Qahr.Mobile.Role.Flags.SpellID.Ironbone };
//                return Spell[Qahr.Mobile.Pool.GetRandom.Next(0, Spell.Length)];
//            }
//            #endregion

//            #region Archer
//            if (AtributesStatus.IsArcher(AI.Player.Class))
//            {
//                uint[] Spell = { (uint)Qahr.Mobile.Role.Flags.SpellID.ArrowBlades, (uint)Qahr.Mobile.Role.Flags.SpellID.ScatterFire, (uint)Qahr.Mobile.Role.Flags.SpellID.MortalWound, (uint)Qahr.Mobile.Role.Flags.SpellID.CrackShot };
//                return Spell[Qahr.Mobile.Pool.GetRandom.Next(0, Spell.Length)];
//            }
//            #endregion

//            #region Ninja
//            if (AtributesStatus.IsNinja(AI.Player.Class))
//            {
//                uint[] Spell = { (uint)Qahr.Mobile.Role.Flags.SpellID.SuperTwofoldBlade, (uint)Qahr.Mobile.Role.Flags.SpellID.TwilightDance, (uint)Qahr.Mobile.Role.Flags.SpellID.SickleWind, (uint)Qahr.Mobile.Role.Flags.SpellID.WildFireball, (uint)Qahr.Mobile.Role.Flags.SpellID.FlameofDestruction, (uint)Qahr.Mobile.Role.Flags.SpellID.DustDetachment, (uint)Qahr.Mobile.Role.Flags.SpellID.ChaoticDance };
//                return Spell[Qahr.Mobile.Pool.GetRandom.Next(0, Spell.Length)];
//            }
//            #endregion

//            #region Monk
//            if (AtributesStatus.IsMonk(AI.Player.Class))
//            {
//                uint[] Spell = { (uint)Qahr.Mobile.Role.Flags.SpellID.ChaoticDance, (uint)(ushort)Qahr.Mobile.Role.Flags.SpellID.RadiantPalm };
//                return Spell[Qahr.Mobile.Pool.GetRandom.Next(0, Spell.Length)];
//            }
//            #endregion

//            #region Pirate
//            if (AtributesStatus.IsPirate(AI.Player.Class))
//            {
//                uint[] Spell = { (uint)Qahr.Mobile.Role.Flags.SpellID.ChaoticDance, (ushort)Qahr.Mobile.Role.Flags.SpellID.BladeTempest, 
//                                   (ushort)Qahr.Mobile.Role.Flags.SpellIDPirate.HolySanction, (ushort)Qahr.Mobile.Role.Flags.SpellIDPirate.GiantGun, 
//                                   (ushort)Qahr.Mobile.Role.Flags.SpellIDPirate.SandMist, (ushort)Qahr.Mobile.Role.Flags.SpellIDPirate.Drukyle };
//                return Spell[Program.GetRandom.Next(0, Spell.Length)];
//            }
//            #endregion

//            #region Dragon-Warrior
//            if (AtributesStatus.IsLee(AI.Player.Class))
//            {
//                uint[] Spell = { (uint)Qahr.Mobile.Role.Flags.SpellID.ChaoticDance, (ushort)Qahr.Mobile.Role.Flags.SpellID.CrackingSwipe, (ushort)Qahr.Mobile.Role.Flags.SpellID.SplittingSwipe, (ushort)Qahr.Mobile.Role.Flags.SpellID.DragonSlash };
//                return Spell[Program.GetRandom.Next(0, Spell.Length)];
//            }
//            #endregion

//            #region ThunderStrike
//            if (AtributesStatus.IsThunderStriker(AI.Player.Class))
//            {
//                uint[] Spell = { (uint)Qahr.Mobile.Role.Flags.SpellID.ChaoticDance, (ushort)Qahr.Mobile.Role.Flags.SpellID.CrackingShock, (ushort)Qahr.Mobile.Role.Flags.SpellID.ThunderBlast, (ushort)Qahr.Mobile.Role.Flags.SpellID.Megabolt };
//                return Spell[Program.GetRandom.Next(0, Spell.Length)];
//            }
//            #endregion

//            #region Water
//            if (AtributesStatus.IsWater(AI.Player.Class))
//            {
//                uint[] Spell = { (uint)Qahr.Mobile.Role.Flags.SpellID.ChaoticDance, (uint)Qahr.Mobile.Role.Flags.SpellID.ChainBolt, (ushort)Qahr.Mobile.Role.Flags.SpellID.Thunder };
//                return Spell[Program.GetRandom.Next(0, Spell.Length)];
//            }
//            #endregion

//            #region Fire
//            if (AtributesStatus.IsFire(AI.Player.Class))
//            {
//                uint[] Spell = {(uint)Qahr.Mobile.Role.Flags.SpellID.ChaoticDance, (ushort)Qahr.Mobile.Role.Flags.SpellID.FireMeteor, (ushort)Qahr.Mobile.Role.Flags.SpellID.FireCircle,
//                    (ushort)Qahr.Mobile.Role.Flags.SpellID.Tornado, (ushort)Qahr.Mobile.Role.Flags.SpellID.Bomb, (ushort)Qahr.Mobile.Role.Flags.SpellID.FireofHell };
//                return Spell[Program.GetRandom.Next(0, Spell.Length)];
//            }
//            #endregion

//            #region Windwalker
//            if (AtributesStatus.IsWindWalker(AI.Player.Class))
//            {
//                uint[] Spell = { (uint)Qahr.Mobile.Role.Flags.SpellID.ChaoticDance, (ushort)Qahr.Mobile.Role.Flags.SpellID.RageofWar, (ushort)Qahr.Mobile.Role.Flags.SpellID.BurntFrost };
//                return Spell[Program.GetRandom.Next(0, Spell.Length)];
//            }
//            #endregion
           
//            return 0;
//        }

//        public static Dictionary<uint, BotAttack> Ais = new Dictionary<uint, BotAttack>();

//        public static ConcurrentDictionary<uint, GameClient> Pool = new ConcurrentDictionary<uint, GameClient>();

//        public static uint[] Body = { 1008, 2007 };

//        public BotLevel Level;

//        public BotState State;

//        public BotType Type;

//        public bool HeroGathering = false;

//        public GameClient Bot = null;

//        public IMapObj Target = null;

//        public int HP;

//        public Time32 LastBotJump;

//        public uint Damage = 0;

//        public uint UID;

//        private int ShootChance = 0;

//        private Time32 LastAttack;

//        public Config configuration = new Config();
//        public void SetLevel(BotLevel Level)
//        {
//            switch (Level)
//            {

//                case BotLevel.BP300:
//                    ShootChance = 30;
//                    Damage = 5000;
//                    break;

//                case BotLevel.BP350:
//                    ShootChance = 35;
//                    Damage = 10000;
//                    break;

//                case BotLevel.BP400:
//                    ShootChance = 40;
//                    Damage = 15000;
//                    break;
//                case BotLevel.BP450:
//                    ShootChance = 50;
//                    Damage = 25000;
//                    break;
//                case BotLevel.BP480:
//                    ShootChance = 60;
//                    Damage = 25000;
//                    break;
//                case BotLevel.BP500:
//                    ShootChance = 70;
//                    Damage = 25000;
//                    break;

//                case BotLevel.FullBP:
//                    ShootChance = 100;
//                    Damage = 35000;
//                    break;
//            }
//        }

//        public unsafe IMapObj GetTarget()
//        {
//            if (Target != null)
//            {
//                var Game = Qahr.Mobile.Pool.GamePoll.Values.Where(p => p.Player.UID == Target.UID).FirstOrDefault();
//                 if (Game != null && Game.Player.GuildID != Bot.Player.GuildID && Game.Player.Alive)
//                     Target = null;
//            }
//            if (Target != null)
//            {
//                if (Target.Map != Bot.Player.Map)
//                {
//                    Target = null;
//                }
//            }
//            if (Target == null)
//            {

//                foreach (IMapObj Obj in Bot.Player.View.Roles(MapObjectType.Player).Where(p => p.UID < 9000000))
//                {
//                    var Game = Qahr.Mobile.Pool.GamePoll.Values.Where(p => p.Player.UID == Obj.UID).FirstOrDefault();
//                    if (Game != null && Game.Player.GuildID != Bot.Player.GuildID && Game.Player.Alive)
//                    {
//                         Target = Obj;
//                         break;
//                    }
//                    else
//                        return null;
//                }
//            }
//            return Target;
//        }
//        public void Tele(uint MapID, ushort X, ushort Y)
//        {
//            Bot.Teleport((ushort)Program.GetRandom.Next(X - 5, X + 5), (ushort)Program.GetRandom.Next(Y - 5, Y + 5), MapID);
//        }
//        public void Revive()
//        {
//            using (var rec = new ServerSockets.RecycledPacket())
//            {
//                var stream = rec.GetStream();
//                if (!Bot.Player.Alive && DateTime.Now > Bot.Player.DeadStamp.AddSeconds(20))
//                    Bot.Player.Revive(stream);
//            }
//        }

//        public void JumpBot()
//        {
//            if (Bot.MyBot.Type == BotType.EventBots)
//            {
//                if (Bot != null && Bot.Map != null && Bot.Player != null && Bot.Player.Map == 700 && Bot.Player.Alive || (Bot.Player.Event && Bot != null && Bot.Map != null && Bot.Player != null && Bot.Player.Alive))
//                {
//                    ushort X = (ushort)Program.GetRandom.Next(Bot.Player.X - 10, Bot.Player.X + 10);
//                    ushort Y = (ushort)Program.GetRandom.Next(Bot.Player.Y - 10, Bot.Player.Y + 10);
//                    if (Target == null)
//                    {
//                        if (Bot.Player.Map == 10250 || Bot.Player.Map == 10137)
//                        {
//                            foreach (var role in Bot.Player.View.Roles(MapObjectType.Monster))
//                            {
//                                Qahr.Mobile.Game.MsgMonster.MonsterRole Attacked = role as Qahr.Mobile.Game.MsgMonster.MonsterRole;
//                                if (Attacked.Family.ID == 4212 || Attacked.Family.ID == 4220
//                                    || Attacked.Family.ID == 4171 || Attacked.Family.ID == 3971 || Attacked.Family.ID == 3977
//                                    || Attacked.Family.ID == 3976 || Attacked.Family.ID == 3978 || Attacked.Family.ID == 3970)
//                                {
//                                    X = (ushort)Program.GetRandom.Next(Attacked.X - 5, Attacked.X + 5);
//                                    Y = (ushort)Program.GetRandom.Next(Attacked.Y - 5, Attacked.Y + 5);
//                                }
//                            }
//                        }
//                        else
//                        {
//                            X = (ushort)Program.GetRandom.Next(Bot.Player.X - 15, Bot.Player.X + 15);
//                            Y = (ushort)Program.GetRandom.Next(Bot.Player.Y - 15, Bot.Player.Y + 15);
//                        }
//                        if (Bot.Player.Map == 1138)
//                        {
//                            X = (ushort)Program.GetRandom.Next(84 - 15, 84 + 15);
//                            Y = (ushort)Program.GetRandom.Next(99 - 15, 99 + 15);
//                        }
//                        if (Bot.Player.Map == 1038)
//                        {
//                            X = (ushort)Program.GetRandom.Next(84 - 15, 84 + 15);
//                            Y = (ushort)Program.GetRandom.Next(99 - 15, 99 + 15);
//                        }

//                    }
//                    if (Target != null)
//                    {
//                        X = (ushort)Program.GetRandom.Next(Target.X - 15, Target.X + 15);
//                        Y = (ushort)Program.GetRandom.Next(Target.Y - 15, Target.Y + 15);
//                    }
//                    if (!Bot.Map.ValidLocation(X, Y))
//                    {
//                        X = Bot.Player.X;
//                        Y = Bot.Player.Y;
//                    }
//                    using (var rec = new RecycledPacket())
//                    {
//                        var stream = rec.GetStream();
//                        ActionQuery action = new ActionQuery()//try
//                        {
//                            ObjId = Bot.Player.UID,
//                            Type = ActionType.Jump,
//                            PositionX = Bot.Player.X,
//                            PositionY = Bot.Player.Y,
//                            TargetPositionX = X,
//                            TargetPositionY = Y,
//                        };
//                        Bot.Player.View.SendView(stream.ActionCreate(action), true);
//                        Bot.Player.Angle = Core.GetAngle(Bot.Player.X, Bot.Player.Y, X, Y);
//                        Bot.Player.Action = Flags.ConquerAction.Jump;
//                        Bot.Map.View.MoveTo<IMapObj>(Bot.Player, X, Y);
//                        Bot.Player.X = X;
//                        Bot.Player.Y = Y;
//                        Bot.Player.View.Role(false, stream);
//                        Bot.Player.LastMove = DateTime.Now;
//                        LastBotJump = Time32.Now;
//                    }
//                }
//            }
//            else
//            {

//                if (Bot != null && Bot.Map != null && Bot.Player != null && Bot.Player.Map == 700 && Bot.Player.Alive)
//                {
//                    ushort X = (ushort)Program.GetRandom.Next(Bot.Player.X - 1, Bot.Player.X + 1);
//                    ushort Y = (ushort)Program.GetRandom.Next(Bot.Player.Y - 1, Bot.Player.Y + 1);
//                    if (Target == null)
//                    {
//                        X = (ushort)Program.GetRandom.Next(Bot.Player.X - 10, Bot.Player.X + 10);
//                        Y = (ushort)Program.GetRandom.Next(Bot.Player.Y - 10, Bot.Player.Y + 10);
//                    }
//                    if (Target != null)
//                    {
//                        X = (ushort)Program.GetRandom.Next(Target.X - 3, Target.X + 3);
//                        Y = (ushort)Program.GetRandom.Next(Target.Y - 3, Target.Y + 3);
//                    }
//                    if (Target != null)
//                    {
//                        X = (ushort)Program.GetRandom.Next(Target.X - 3, Target.X + 3);
//                        Y = (ushort)Program.GetRandom.Next(Target.Y - 3, Target.Y + 3);
//                    }
//                    if (!Bot.Map.ValidLocation(X, Y))
//                    {
//                        X = Bot.Player.X;
//                        Y = Bot.Player.Y;
//                    }
//                    using (var rec = new RecycledPacket())
//                    {
//                        var stream = rec.GetStream();
//                        ActionQuery action = new ActionQuery()//try
//                        {
//                            ObjId = Bot.Player.UID,
//                            Type = ActionType.Jump,
//                            PositionX = Bot.Player.X,
//                            PositionY = Bot.Player.Y,
//                            TargetPositionX = X,
//                            TargetPositionY = Y,
//                        };
//                        Bot.Player.View.SendView(stream.ActionCreate(action), true);
//                        Bot.Player.Angle = Core.GetAngle(Bot.Player.X, Bot.Player.Y, X, Y);
//                        Bot.Player.Action = Flags.ConquerAction.Jump;
//                        Bot.Map.View.MoveTo<IMapObj>(Bot.Player, X, Y);
//                        Bot.Player.X = X;
//                        Bot.Player.Y = Y;
//                        Bot.Player.View.Role(false, stream);
//                        Bot.Player.LastMove = DateTime.Now;
//                        LastBotJump = Time32.Now;
//                    }
//                }
//            }
//        }
//        public void AttackerBot()
//        {
//            if ((Bot != null && Bot.Map != null && Bot.Player.View != null && Bot.Player != null && Bot.Player.HitPoints > 0 && Bot.Player.Map == 700) || Bot.Player.Event&&Bot != null && Bot.Map != null && Bot.Player.View != null && Bot.Player != null && Bot.Player.HitPoints > 0 )
//            {
//                if (Target == null)
//                    return;
//                if (!Target.Alive)
//                    return;
//                GameClient attacked;
//                if (Qahr.Mobile.Pool.GamePoll.TryGetValue(Target.UID, out attacked))
//                {
//                    uint SpellID = GetSpell(Bot);
//                    if (SpellID == (ushort)Qahr.Mobile.Role.Flags.SpellID.DustDetachment)
//                    {
//                        if (!Bot.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.DustDetachment))
//                        {
//                            using (var rec = new ServerSockets.RecycledPacket())
//                                Bot.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.DustDetachment, 4);
//                        }
//                    }
//                    if (Bot.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
//                    {
//                        using (var rec = new RecycledPacket())
//                        {
//                            var stream = rec.GetStream();
//                            if (!attacked.Player.AllowAttack())
//                                return;
//                            Bot.Player.Mana += 10000;
//                            Bot.Player.Stamina += 100;
//                            Bot.Player.UpdateArrowBlades(stream, 3);
//                            Bot.Player.SendUpdate(stream, Bot.Player.Stamina, MsgUpdate.DataType.Stamina);
//                            MsgAttackPacket.ProcescMagic(Bot, stream, new InteractQuery()
//                            {
//                                OpponentUID = Target.UID,
//                                UID = Bot.Player.UID,
//                                X = Target.X,
//                                Y = Target.Y,
//                                SpellID = (ushort)SpellID
//                            }, true);
//                        }
//                    }
//                }
//                Bot.MyBot.LastAttack = Time32.Now;
//            }
//        }
//        public void AttackerMonsterBot()
//        {
//            if (Bot.Player.Event && Bot != null && Bot.Map != null && Bot.Player.View != null && Bot.Player != null && Bot.Player.HitPoints > 0)
//            {
//                if (Bot.Player.Map == 10250 || Bot.Player.Map == 10137)
//                {
//                    foreach (var role in Bot.Player.View.Roles(MapObjectType.Monster))
//                    {
//                        if (Core.GetDistance(role.X, role.Y, Bot.Player.X, Bot.Player.Y) <= (short)12)
//                        {
//                            Qahr.Mobile.Game.MsgMonster.MonsterRole Attacked = role as Qahr.Mobile.Game.MsgMonster.MonsterRole;
//                            if (Attacked.Family.ID == 4212 || Attacked.Family.ID == 4220
//                                || Attacked.Family.ID == 4171 || Attacked.Family.ID == 3971 || Attacked.Family.ID == 3977
//                                || Attacked.Family.ID == 3976 || Attacked.Family.ID == 3978 || Attacked.Family.ID == 3970)
//                            {
//                                if (!Attacked.ContainFlag(MsgUpdate.Flags.Ghost) && !Attacked.Name.Contains("Guard"))
//                                {
//                                    uint SpellID = GetSpell(Bot);
//                                    if (SpellID == (ushort)Qahr.Mobile.Role.Flags.SpellID.DustDetachment)
//                                    {
//                                        if (!Bot.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.DustDetachment))
//                                        {
//                                            using (var rec = new ServerSockets.RecycledPacket())
//                                                Bot.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.DustDetachment, 4);
//                                        }
//                                    }
//                                    if (Bot.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
//                                    {
//                                        using (var rec = new RecycledPacket())
//                                        {
//                                            var stream = rec.GetStream();
//                                            Bot.Player.Mana += 10000;
//                                            Bot.Player.Stamina += 100;
//                                            Bot.Player.UpdateArrowBlades(stream, 3);
//                                            MsgAttackPacket.ProcescMagic(Bot, stream, new InteractQuery()
//                                            {
//                                                OpponentUID = role.UID,
//                                                UID = Bot.Player.UID,
//                                                X = role.X,
//                                                Y = role.Y,
//                                                SpellID = (ushort)SpellID
//                                            }, true);
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
                  
//                }
//            }
//        }
//        public void AttackerNpc()
//        {
//            foreach (var role in Bot.Player.View.Roles(MapObjectType.SobNpc))
//            {
//                var attacked = role as Role.SobNpc;
//                if (attacked.Map == 1138 
//                    || attacked.Map == 1038 
//                    || attacked.Map == 2057
//                    || attacked.Map == 12020
//                    || attacked.Map == 12022
//                    || attacked.Map == 12066
//                    || attacked.Map == 12067
//                    || attacked.Map == 12025)
//                {
//                    if (attacked.Map == 1138)
//                    {
//                        if (attacked.GuildID == Bot.Player.GuildID)
//                            return;
//                    }
//                    if (attacked.Map == 1038)
//                    {
//                        if (attacked.GuildID == Bot.Player.GuildID)
//                            return;
//                    }
//                    if (attacked.Map == 12020
//                 || attacked.Map == 12022
//                 || attacked.Map == 12066
//                 || attacked.Map == 12067
//                 || attacked.Map == 12025)
//                    {
//                        if (attacked.Name == Bot.Player.MyClan.Name)
//                            return;
//                    }
//                    uint SpellID = GetSpell(Bot);
//                    if (SpellID == (ushort)Qahr.Mobile.Role.Flags.SpellID.DustDetachment)
//                    {
//                        if (!Bot.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.DustDetachment))
//                        {
//                            using (var rec = new ServerSockets.RecycledPacket())
//                                Bot.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.DustDetachment, 4);
//                        }
//                    }
//                    if (Bot.MySpells.ClientSpells.ContainsKey((ushort)SpellID))
//                    {
//                        using (var rec = new RecycledPacket())
//                        {
//                            var stream = rec.GetStream();
//                            Bot.Player.Mana += 10000;
//                            Bot.Player.Stamina += 100;
//                            Bot.Player.UpdateArrowBlades(stream, 3);
//                            MsgAttackPacket.ProcescMagic(Bot, stream, new InteractQuery()
//                            {
//                                OpponentUID = role.UID,
//                                UID = Bot.Player.UID,
//                                X = role.X,
//                                Y = role.Y,
//                                SpellID = (ushort)SpellID
//                            }, true);
//                        }
//                    }
//                }
//            }
//        }
//        public void Remover()
//        {
//            if (HeroGathering)
//                return;
//            if (Target == null)
//                return;
//            GameClient client;
//            if (!Qahr.Mobile.Pool.GamePoll.TryGetValue(Target.UID, out client))
//            {
//                using (var rec = new RecycledPacket())
//                {
//                    var stream = rec.GetStream();
//                    Bot.Map.RemoveAI(Bot);
//                }
//            }
//            if (Qahr.Mobile.Pool.GamePoll.TryGetValue(Target.UID, out client))
//            {
//                if (client.Player.Map != Bot.Player.Map)
//                {
//                    using (var rec = new RecycledPacket())
//                    {
//                        var stream = rec.GetStream();
//                        Bot.Map.RemoveAI(Bot);
//                    }
//                }
//            }
//        }
//    }
//    public unsafe class BotHandle
//    {
//        public static string[] AvailableNames = { "Ares", "Apolo", "Asclepio", "Baco", "Cupido", "Dionisio", "Eros", "Febo", "Forcis", "Hades", "Hefesto", "Hércules", "Hermes", "Helios", "Marte", "Mercurio", "Morfeo", "Neptuno", "Perseo", "Poseidón", "Silvano", "Teseo", "Ulises", "Vulcano", "Zeus", "Aamon", "Belia", "Paimon", "Agares", "Sidragaso", "Balban", "Baphomet", "Bael", "Baalzephon", "Alouqua", "Abrahel", "Abyzou ", "Aradia", "Astartea", "Habondia", "Lilith", "Nahama", "Noctiluca", "Perséfone", "Perisas", "Tamar", "Zalir", "Zemunín", "Asmodeo ", "Behemoth", "Lucifer", "Mammón", "Leviatán", "Astaroth", "Diablo", "Baal", "Dante", "Asta", "Megicula", "Lumier", "Lier", "Deku", "Bakugo", "Rafael", "Odin", "Thor", "Kratos", "Agumon", "Valhalla", "Arcangel", "Meliodas", "Zeldris", "Escanor", "King", "Ban", "Estarossa", "Mael", "LightYagami", "Kira", "RiasGremory", "Bastardo", "TheNorth", "Winter", "NightKing", "Cain", "Athena", "Yui", "Yagoromo", "Indra", "Naruto", "Sasuke", "Luffy", "RoronoaZoro", "TrafalgarLaw", "RossMaria", "LaPerversa", "FelizSanchez", "DaniloLadron", "BlackClover", "Julios", "Luck", "Noel", "Yami", "Lie", "Luffy", "Name", "Zoro", "Usopp", "Franki", "Robin", "Brook", "Enel", "ElRaro", "BocaDePiano", "Alofoke", "ElMeloso", "Ditamae", "Pistola", "YQueFue", "Ranchero", "TheEnd", "TheFinal", "FinalSpace", "TheLostCity", "Indra", "Digimon", "Pokemon", "JetEd", "Blue", "Red", "Green", "Online", "Fake", "True", "False", "Int", "String", "Float", "Uint", "ushort", "Byte", "Ulong", "Long", "Array", "Koneko", "Isee", "ElAbusador", "TeCogi", "Peligrosa", "Danger", "FlowFactory", "RD", "Anuel", "MykeTowers", "Mozart", "RochyRD", "Mace", "Falchion", "Montante", "Battleaxe", "Zweihander", "Hatchet", "Billhook", "Club", "Hammer", "Caltrop", "Maul", "Sledgehammer", "Longbow", "Bludgeon", "Harpoon", "Crossbow", "Lance", "Angon", "Pike", "Tiger Claw", "Fire Lance", "Poleaxe", "Brass Knuckle", "Matchlock", "Quarterstaff", "Gauntlet", "Bullwhip", "War Hammer", "Katar", "Flying Claw", "Spear", "Dagger", "Slungshot", "Katana", "Gladius", "Aspis", "Saber", "Cutlass", "Blade", "Broadsword", "Scimitar", "Lockback", "Claymore", "Espada", "Machete", "Grizzly", "Wolverine", "Deathstalker", "Snake", "Wolf", "Scorpion", "Vulture", "Claw", "Boomslang", "Falcon", "Fang", "Viper", "Ram", "Grip", "Sting", "Boar", "Black Mamba", "Lash", "Tusk", "Goshawk", "Gnaw", "Amazon", "Majesty", "Anomoly", "Malice", "Banshee", "Mannequin", "Belladonna", "Minx", "Beretta", "Mirage", "Black Beauty", "Nightmare", "Calypso", "Nova", "Carbon", "Pumps", "Cascade", "Raven", "Colada", "Resin", "Cosma", "Riveter", "Cougar", "Rogue", "Countess", "Roulette", "Enchantress", "Shadow", "Enigma", "Siren", "Femme Fatale", "Stiletto", "Firecracker", "Tattoo", "Geisha", "T-Back", "Goddess", "Temperance", "Half Pint", "Tequila", "Harlem", "Terror", "Heroin", "Thunderbird", "Infinity", "Ultra", "Insomnia", "Vanity", "Ivy", "Velvet", "Legacy", "Vixen", "Lithium", "Voodoo", "Lolita", "Wicked", "Lotus", "Widow", "Mademoiselle", "Xenon", "Kahina", "Teuta", "Isis", "Dihya", "Artemis", "Nefertiti", "RunningEagle", "Atalanta", "Sekhmet", "Colestah", "Athena", "Ishtar", "Calamity Jane", "Enyo", "Ashtart", "Pearl Heart", "Bellona", "Juno", "Belle Starr", "White Tights", "Tanit", "Hua Mulan", "Shieldmaiden", "Devi", "Boudica", "Valkyrie", "Selkie", "Medb", "Cleo", "Venus", "Fate", "Beguile", "Deviant", "Illusion", "Crafty", "Variance", "Delusion", "Deceit", "Caprice", "Deception", "Waylay", "Aberr", "Myth", "Ambush", "Variant", "Daydream", "Feint", "Hero", "Night Terror", "Catch-22", "Villain", "Figment", "Puzzler", "Daredevil", "Virtual", "Curio", "Mercenary", "Chicanery", "Prodigy", "Voyager", "Trick", "Breach", "Wanderer", "Vile", "Miss Fortune", "Audacity", "Horror", "Vex", "Swagger", "Dismay", "Grudge", "Nerve", "Phobia", "Enmity", "Egomania", "Fright", "Animus", "Scheme", "Panic", "Hostility", "Paramour", "Agony", "Rancor", "X-hibit", "Inferno", "Malevolence", "Charade", "Blaze", "Poison", "Hauteur", "Crucible", "Spite", "Vainglory", "Haunter", "Spitefulness", "Narcissus", "Bane", "Venom", "Brass", "Volcano", "Vampire", "Hulk" };

//        public static string[] AvailableNames2 = { "Net Video", "Net Youssef" };
//        public static void ThreadAction()
//        {
//            foreach (var Bot in BotAttack.Pool.Values)
//            {
//                if (Bot != null)
//                {
//                    if (!Bot.Player.Event)
//                    {
//                        Bot.MyBot.GetTarget();
//                        Bot.MyBot.JumpBot();
//                        Bot.MyBot.AttackerBot();
//                        Bot.MyBot.Remover();
//                    }
//                    if (Bot.MyBot.Type== BotType.EventBots)
//                    {
//                        Bot.MyBot.Revive();
//                        #region GuildWar
//                        if (!Bot.Player.EndTeleGuildWar && Game.MsgTournaments.MsgSchedules.GuildWar.Proces == Game.MsgTournaments.ProcesType.Alive || Game.MsgTournaments.MsgSchedules.GuildWar.Proces == Game.MsgTournaments.ProcesType.Idle)
//                        {
//                            Bot.MyBot.Tele(1002, 322, 318);
//                            Bot.MyBot.Tele(1038, 349, 340);
//                            Bot.Player.EndTeleGuildWar = true;
//                        }
//                        if (Game.MsgTournaments.MsgSchedules.GuildWar.Proces == Game.MsgTournaments.ProcesType.Alive || Game.MsgTournaments.MsgSchedules.GuildWar.Proces == Game.MsgTournaments.ProcesType.Idle)
//                        {
//                            Bot.MyBot.GetTarget();
//                            Bot.MyBot.JumpBot();
//                            if (Bot.MyBot.Target != null)
//                                Bot.MyBot.AttackerBot();
//                            else
//                                Bot.MyBot.AttackerNpc();
//                        }
//                        if (Bot.Player.EndTeleGuildWar && Game.MsgTournaments.MsgSchedules.GuildWar.Proces == Game.MsgTournaments.ProcesType.Dead)
//                        {
//                            Bot.Player.EndTeleGuildWar = false;
//                            ushort X = (ushort)Program.GetRandom.Next(410 - 20, 410 + 20);
//                            ushort y = (ushort)Program.GetRandom.Next(354 - 20, 354 + 20);
//                            var Map = Pool.ServerMaps[1002];
//                            Bot.Teleport(X, y, (uint)Map.ID, 0, false, true);
//                        }
//                        #endregion
//                        #region SuperGuildWar
//                        if (!Bot.Player.EndTeleSuperGuildWar && Game.MsgTournaments.MsgSchedules.SuperGuildWar.Proces == Game.MsgTournaments.ProcesType.Alive || Game.MsgTournaments.MsgSchedules.SuperGuildWar.Proces == Game.MsgTournaments.ProcesType.Idle)
//                        {
//                            Bot.MyBot.Tele(1002, 322, 318);
//                            Bot.MyBot.Tele(1138, 84, 99);
//                            Bot.Player.EndTeleSuperGuildWar = true;
//                        }
//                        if (Game.MsgTournaments.MsgSchedules.SuperGuildWar.Proces == Game.MsgTournaments.ProcesType.Alive || Game.MsgTournaments.MsgSchedules.SuperGuildWar.Proces == Game.MsgTournaments.ProcesType.Idle)
//                        {
//                            Bot.MyBot.GetTarget();
//                            Bot.MyBot.JumpBot();
//                            if (Bot.MyBot.Target != null)
//                                Bot.MyBot.AttackerBot();
//                            else
//                                Bot.MyBot.AttackerNpc();
//                        }
//                        if (Bot.Player.EndTeleSuperGuildWar && Game.MsgTournaments.MsgSchedules.SuperGuildWar.Proces == Game.MsgTournaments.ProcesType.Dead)
//                        {
//                            Bot.Player.EndTeleSuperGuildWar = false;
//                            ushort X = (ushort)Program.GetRandom.Next(410 - 20, 410 + 20);
//                            ushort y = (ushort)Program.GetRandom.Next(354 - 20, 354 + 20);
//                            var Map = Pool.ServerMaps[1002];
//                            Bot.Teleport(X, y, (uint)Map.ID, 0, false, true);
//                        }
//                        #endregion
//                        #region CaptureTheFlag
//                        if (!Bot.Player.EndTeleCaptureTheFlag && Game.MsgTournaments.MsgSchedules.CaptureTheFlag.Proces == Game.MsgTournaments.ProcesType.Alive || Game.MsgTournaments.MsgSchedules.CaptureTheFlag.Proces == Game.MsgTournaments.ProcesType.Idle)
//                        {
//                            Bot.MyBot.Tele(1002, 322, 318);
//                            var basse = Game.MsgTournaments.MsgSchedules.CaptureTheFlag.Bases.Values.FirstOrDefault(p => p.IsX2);
//                            Bot.MyBot.Tele(basse.Npc.Map, basse.Npc.X, basse.Npc.Y);
//                            Bot.Player.EndTeleCaptureTheFlag = true;
//                        }
//                        if (Game.MsgTournaments.MsgSchedules.CaptureTheFlag.Proces == Game.MsgTournaments.ProcesType.Alive || Game.MsgTournaments.MsgSchedules.CaptureTheFlag.Proces == Game.MsgTournaments.ProcesType.Idle)
//                        {
//                            Bot.MyBot.GetTarget();
//                            Bot.MyBot.JumpBot();
//                            if (Bot.MyBot.Target != null)
//                                Bot.MyBot.AttackerBot();
//                            else
//                                Bot.MyBot.AttackerNpc();
//                        }
//                        if (Bot.Player.EndTeleCaptureTheFlag && Game.MsgTournaments.MsgSchedules.CaptureTheFlag.Proces == Game.MsgTournaments.ProcesType.Dead)
//                        {
//                            Bot.Player.EndTeleCaptureTheFlag = false;
//                            ushort X = (ushort)Program.GetRandom.Next(410 - 20, 410 + 20);
//                            ushort y = (ushort)Program.GetRandom.Next(354 - 20, 354 + 20);
//                            var Map = Pool.ServerMaps[1002];
//                            Bot.Teleport(X, y, (uint)Map.ID, 0, false, true);
//                        }
//                        #endregion


//                        #region ClanTwin
//                        if (!Bot.Player.EndTeleClanTwin && Game.MsgTournaments.MsgSchedules.ClanTwin.Proces == Game.MsgTournaments.ProcesType.Alive || Game.MsgTournaments.MsgSchedules.ClanTwin.Proces == Game.MsgTournaments.ProcesType.Idle)
//                        {
//                            Bot.MyBot.Tele(1002, 322, 318);
//                            Bot.MyBot.Tele(1038, 349, 340);
//                            Bot.Player.EndTeleClanTwin = true;
//                        }
//                        if (Game.MsgTournaments.MsgSchedules.ClanTwin.Proces == Game.MsgTournaments.ProcesType.Alive || Game.MsgTournaments.MsgSchedules.ClanTwin.Proces == Game.MsgTournaments.ProcesType.Idle)
//                        {
//                            Bot.MyBot.GetTarget();
//                            Bot.MyBot.JumpBot();
//                            if (Bot.MyBot.Target != null)
//                                Bot.MyBot.AttackerBot();
//                            else
//                                Bot.MyBot.AttackerNpc();
//                        }
//                        if (Bot.Player.EndTeleClanTwin && Game.MsgTournaments.MsgSchedules.ClanTwin.Proces == Game.MsgTournaments.ProcesType.Dead)
//                        {
//                            Bot.Player.EndTeleClanTwin = false;
//                            ushort X = (ushort)Program.GetRandom.Next(410 - 20, 410 + 20);
//                            ushort y = (ushort)Program.GetRandom.Next(354 - 20, 354 + 20);
//                            var Map = Pool.ServerMaps[1002];
//                            Bot.Teleport(X, y, (uint)Map.ID, 0, false, true);
//                        }
//                        #endregion
//                        #region ClanPhoenix
//                        if (!Bot.Player.EndTeleClanPhoenix && Game.MsgTournaments.MsgSchedules.ClanPhoenix.Proces == Game.MsgTournaments.ProcesType.Alive || Game.MsgTournaments.MsgSchedules.ClanPhoenix.Proces == Game.MsgTournaments.ProcesType.Idle)
//                        {
//                            Bot.MyBot.Tele(1002, 322, 318);
//                            Bot.MyBot.Tele(1038, 349, 340);
//                            Bot.Player.EndTeleClanPhoenix = true;
//                        }
//                        if (Game.MsgTournaments.MsgSchedules.ClanPhoenix.Proces == Game.MsgTournaments.ProcesType.Alive || Game.MsgTournaments.MsgSchedules.ClanPhoenix.Proces == Game.MsgTournaments.ProcesType.Idle)
//                        {
//                            Bot.MyBot.GetTarget();
//                            Bot.MyBot.JumpBot();
//                            if (Bot.MyBot.Target != null)
//                                Bot.MyBot.AttackerBot();
//                            else
//                                Bot.MyBot.AttackerNpc();
//                        }
//                        if (Bot.Player.EndTeleClanPhoenix && Game.MsgTournaments.MsgSchedules.ClanPhoenix.Proces == Game.MsgTournaments.ProcesType.Dead)
//                        {
//                            Bot.Player.EndTeleClanPhoenix = false;
//                            ushort X = (ushort)Program.GetRandom.Next(410 - 20, 410 + 20);
//                            ushort y = (ushort)Program.GetRandom.Next(354 - 20, 354 + 20);
//                            var Map = Pool.ServerMaps[1002];
//                            Bot.Teleport(X, y, (uint)Map.ID, 0, false, true);
//                        }
//                        #endregion
//                        #region ClanApe
//                        if (!Bot.Player.EndTeleClanApe && Game.MsgTournaments.MsgSchedules.ClanApe.Proces == Game.MsgTournaments.ProcesType.Alive || Game.MsgTournaments.MsgSchedules.ClanApe.Proces == Game.MsgTournaments.ProcesType.Idle)
//                        {
//                            Bot.MyBot.Tele(1002, 322, 318);
//                            Bot.MyBot.Tele(1038, 349, 340);
//                            Bot.Player.EndTeleClanApe = true;
//                        }
//                        if (Game.MsgTournaments.MsgSchedules.ClanApe.Proces == Game.MsgTournaments.ProcesType.Alive || Game.MsgTournaments.MsgSchedules.ClanApe.Proces == Game.MsgTournaments.ProcesType.Idle)
//                        {
//                            Bot.MyBot.GetTarget();
//                            Bot.MyBot.JumpBot();
//                            if (Bot.MyBot.Target != null)
//                                Bot.MyBot.AttackerBot();
//                            else
//                                Bot.MyBot.AttackerNpc();
//                        }
//                        if (Bot.Player.EndTeleClanApe && Game.MsgTournaments.MsgSchedules.ClanApe.Proces == Game.MsgTournaments.ProcesType.Dead)
//                        {
//                            Bot.Player.EndTeleClanApe = false;
//                            ushort X = (ushort)Program.GetRandom.Next(410 - 20, 410 + 20);
//                            ushort y = (ushort)Program.GetRandom.Next(354 - 20, 354 + 20);
//                            var Map = Pool.ServerMaps[1002];
//                            Bot.Teleport(X, y, (uint)Map.ID, 0, false, true);
//                        }
//                        #endregion
//                        #region ClanDesert
//                        if (!Bot.Player.EndTeleClanDesert && Game.MsgTournaments.MsgSchedules.ClanDesert.Proces == Game.MsgTournaments.ProcesType.Alive || Game.MsgTournaments.MsgSchedules.ClanDesert.Proces == Game.MsgTournaments.ProcesType.Idle)
//                        {
//                            Bot.MyBot.Tele(1002, 322, 318);
//                            Bot.MyBot.Tele(1038, 349, 340);
//                            Bot.Player.EndTeleClanDesert = true;
//                        }
//                        if (Game.MsgTournaments.MsgSchedules.ClanDesert.Proces == Game.MsgTournaments.ProcesType.Alive || Game.MsgTournaments.MsgSchedules.ClanDesert.Proces == Game.MsgTournaments.ProcesType.Idle)
//                        {
//                            Bot.MyBot.GetTarget();
//                            Bot.MyBot.JumpBot();
//                            if (Bot.MyBot.Target != null)
//                                Bot.MyBot.AttackerBot();
//                            else
//                                Bot.MyBot.AttackerNpc();
//                        }
//                        if (Bot.Player.EndTeleClanDesert && Game.MsgTournaments.MsgSchedules.ClanDesert.Proces == Game.MsgTournaments.ProcesType.Dead)
//                        {
//                            Bot.Player.EndTeleClanDesert = false;
//                            ushort X = (ushort)Program.GetRandom.Next(410 - 20, 410 + 20);
//                            ushort y = (ushort)Program.GetRandom.Next(354 - 20, 354 + 20);
//                            var Map = Pool.ServerMaps[1002];
//                            Bot.Teleport(X, y, (uint)Map.ID, 0, false, true);
//                        }
//                        #endregion
//                        #region ClanBird
//                        if (!Bot.Player.EndTeleClanBird && Game.MsgTournaments.MsgSchedules.ClanBird.Proces == Game.MsgTournaments.ProcesType.Alive || Game.MsgTournaments.MsgSchedules.ClanBird.Proces == Game.MsgTournaments.ProcesType.Idle)
//                        {
//                            Bot.MyBot.Tele(1002, 322, 318);
//                            Bot.MyBot.Tele(1038, 349, 340);
//                            Bot.Player.EndTeleClanBird = true;
//                        }
//                        if (Game.MsgTournaments.MsgSchedules.ClanBird.Proces == Game.MsgTournaments.ProcesType.Alive || Game.MsgTournaments.MsgSchedules.ClanBird.Proces == Game.MsgTournaments.ProcesType.Idle)
//                        {
//                            Bot.MyBot.GetTarget();
//                            Bot.MyBot.JumpBot();
//                            if (Bot.MyBot.Target != null)
//                                Bot.MyBot.AttackerBot();
//                            else
//                                Bot.MyBot.AttackerNpc();
//                        }
//                        if (Bot.Player.EndTeleClanBird && Game.MsgTournaments.MsgSchedules.ClanBird.Proces == Game.MsgTournaments.ProcesType.Dead)
//                        {
//                            Bot.Player.EndTeleClanBird = false;
//                            ushort X = (ushort)Program.GetRandom.Next(410 - 20, 410 + 20);
//                            ushort y = (ushort)Program.GetRandom.Next(354 - 20, 354 + 20);
//                            var Map = Pool.ServerMaps[1002];
//                            Bot.Teleport(X, y, (uint)Map.ID, 0, false, true);
//                        }
//                        #endregion
//                        if (Game.MsgTournaments.MsgSchedules.GuildWar.Proces == 0
//                            || Game.MsgTournaments.MsgSchedules.SuperGuildWar.Proces == Game.MsgTournaments.ProcesType.Dead
//                            || Game.MsgTournaments.MsgSchedules.CaptureTheFlag.Proces == Game.MsgTournaments.ProcesType.Dead
//                            || Game.MsgTournaments.MsgSchedules.ClanTwin.Proces == Game.MsgTournaments.ProcesType.Dead
//                            || Game.MsgTournaments.MsgSchedules.ClanPhoenix.Proces == Game.MsgTournaments.ProcesType.Dead
//                            || Game.MsgTournaments.MsgSchedules.ClanApe.Proces == Game.MsgTournaments.ProcesType.Dead
//                            || Game.MsgTournaments.MsgSchedules.ClanBird.Proces == Game.MsgTournaments.ProcesType.Dead
//                            || Game.MsgTournaments.MsgSchedules.ClanDesert.Proces == Game.MsgTournaments.ProcesType.Dead)
//                        {
//                            var map = Pool.ServerMaps[10137];
//                            if ((Bot.Player.Map == 10250 || Bot.Player.Map == 10137) && (map.ContainMobID(4212) || map.ContainMobID(4220) || map.ContainMobID(4171)))
//                            {
//                                Bot.MyBot.GetTarget();
//                                Bot.MyBot.JumpBot();
//                                if (Bot.MyBot.Target != null)
//                                    Bot.MyBot.AttackerBot();
//                                else
//                                    Bot.MyBot.AttackerMonsterBot();
//                            }
//                            if (!Bot.Player.EndTele && map.ContainMobID(4212) && DateTime.Now > Bot.Player.TeleStamp.AddSeconds(20))
//                            {
//                                Bot.Player.TeleStamp = DateTime.Now;
//                                Bot.MyBot.Tele((ushort)map.ID, 349, 635);
//                                Bot.Player.EndTele = true;
//                                Bot.Player.End2Tele = false;
//                            }
//                            if (!Bot.Player.EndTele && map.ContainMobID(4220) && DateTime.Now > Bot.Player.TeleStamp.AddSeconds(20))
//                            {
//                                Bot.Player.TeleStamp = DateTime.Now;
//                                Bot.MyBot.Tele((ushort)map.ID, 568, 372);
//                                Bot.Player.EndTele = true;
//                                Bot.Player.End2Tele = false;
//                            }
//                            if (!Bot.Player.EndTele && map.ContainMobID(4171) && DateTime.Now > Bot.Player.TeleStamp.AddSeconds(20))
//                            {
//                                Bot.Player.TeleStamp = DateTime.Now;
//                                Bot.MyBot.Tele((ushort)map.ID, 658, 718);
//                                Bot.Player.EndTele = true;
//                                Bot.Player.End2Tele = false;
//                            }
//                            var map2 = Pool.ServerMaps[10250];
//                            if (!Bot.Player.EndTele && map2.ContainMobID(3971) && DateTime.Now > Bot.Player.TeleStamp.AddSeconds(20))
//                            {
//                                Bot.Player.TeleStamp = DateTime.Now;
//                                Bot.MyBot.Tele((ushort)map2.ID, 163, 415);
//                                Bot.Player.EndTele = true;
//                            }
//                            if (!Bot.Player.EndTele && map2.ContainMobID(3977) && DateTime.Now > Bot.Player.TeleStamp.AddSeconds(20))
//                            {
//                                Bot.Player.TeleStamp = DateTime.Now;
//                                Bot.MyBot.Tele((ushort)map2.ID, 1020, 698);
//                                Bot.Player.EndTele = true;
//                            }
//                            if (!Bot.Player.EndTele && map2.ContainMobID(3976) && DateTime.Now > Bot.Player.TeleStamp.AddSeconds(20))
//                            {
//                                Bot.Player.TeleStamp = DateTime.Now;
//                                Bot.MyBot.Tele((ushort)map2.ID, 484, 176);
//                                Bot.Player.EndTele = true;
//                            }
//                            if (!Bot.Player.EndTele && map2.ContainMobID(3978) && DateTime.Now > Bot.Player.TeleStamp.AddSeconds(20))
//                            {
//                                Bot.Player.TeleStamp = DateTime.Now;
//                                Bot.MyBot.Tele((ushort)map2.ID, 872, 1135);
//                                Bot.Player.EndTele = true;
//                            }
//                            if (!Bot.Player.EndTele && map2.ContainMobID(3970) && DateTime.Now > Bot.Player.TeleStamp.AddSeconds(20))
//                            {
//                                Bot.Player.TeleStamp = DateTime.Now;
//                                Bot.MyBot.Tele((ushort)map2.ID, 640, 600);
//                                Bot.Player.EndTele = true;
//                            }
//                            if ((Bot.Player.Map == 10250 || Bot.Player.Map == 10137) && (map2.ContainMobID(3970) || map2.ContainMobID(3978) || map2.ContainMobID(3976) || map2.ContainMobID(3977) || map2.ContainMobID(3971)))
//                            {
//                                Bot.MyBot.GetTarget();
//                                Bot.MyBot.JumpBot();
//                                if (Bot.MyBot.Target != null)
//                                    Bot.MyBot.AttackerBot();
//                                else
//                                    Bot.MyBot.AttackerMonsterBot();
//                            }
//                            if (Bot.Player.End2Tele && DateTime.Now > Bot.Player.TeleStamp.AddSeconds(20))
//                            {
//                                Bot.Player.End2Tele = false;
//                                ushort X = (ushort)Program.GetRandom.Next(410 - 20, 410 + 20);
//                                ushort y = (ushort)Program.GetRandom.Next(354 - 20, 354 + 20);
//                                var Map = Pool.ServerMaps[1002];
//                                Bot.Teleport(X, y, (uint)Map.ID, 0, false, true);
//                            }
//                        }
//                    }
//                }
//            }
//        }
//        public static BotType Type;
//        public static Counter UID1 = new Counter(3999950000);
//        public static Counter UID2 = new Counter(9000000);
//        public static string[] BotEvent = { "Ares", "Apolo", "Asclepio", "Baco", "Cupido", "Dionisio", "Eros", "Febo", "Forcis", "Hades", "Hefesto", "Hércules", "Hermes", "Helios", "Marte", "Mercurio", "Morfeo", "Neptuno", "Perseo", "Poseidón"};
//        public static void XY(string Name, out ushort x, out ushort y)
//        {
//            x = 0;
//            y = 0;
//            if (Name == "Ares")
//            {
//                x = 414;
//                y = 349;
//            }
//            if (Name == "Apolo")
//            {
//                x = 413;
//                y = 356;
//            }
//            if (Name == "Asclepio")
//            {
//                x = 404;
//                y = 359;
//            }
//            if (Name == "Baco")
//            {
//                x = 413;
//                y = 372;
//            }
//            if (Name == "Cupido")
//            {
//                x = 428;
//                y = 372;
//            }
//            if (Name == "Dionisio")
//            {
//                x = 409;
//                y = 378;
//            }
//            if (Name == "Eros")
//            {
//                x = 427;
//                y = 383;
//            }
//            if (Name == "Febo")
//            {
//                x = 395;
//                y = 374;
//            }
//            if (Name == "Forcis")
//            {
//                x = 375;
//                y = 387;
//            }
//            if (Name == "Hades")
//            {
//                x = 336;
//                y = 369;
//            }
//            if (Name == "Hefesto")
//            {
//                x = 343;
//                y = 377;
//            }
//            if (Name == "Hércules")
//            {
//                x = 340;
//                y = 388;
//            }
//            if (Name == "Hermes")
//            {
//                x = 351;
//                y = 399;
//            }
//            if (Name == "Helios")
//            {
//                x = 343;
//                y = 412;
//            }
//            if (Name == "Marte")
//            {
//                x = 349;
//                y = 425;
//            }
//            if (Name == "Mercurio")
//            {
//                x = 350;
//                y = 439;
//            }
//            if (Name == "Morfeo")
//            {
//                x = 341;
//                y = 441;
//            }
//            if (Name == "Neptuno")
//            {
//                x = 325;
//                y = 439;
//            }
//            if (Name == "Perseo")
//            {
//                x = 367;
//                y = 303;
//            }
//            if (Name == "Poseidón")
//            {
//                x = 360;
//                y = 317;
//            }

//        }
//        public static void UID(string Name, out uint UID)
//        {
//            UID = 0;
//            if (Name == "Ares")
//            {
//                UID = 9000000;
//            }
//            if (Name == "Apolo")
//            {
//                UID = 9000001;
//            }
//            if (Name == "Asclepio")
//            {
//                UID = 9000002;
//            }
//            if (Name == "Baco")
//            {
//                UID = 9000003;
//            }
//            if (Name == "Cupido")
//            {
//                UID = 9000004;
//            }
//            if (Name == "Dionisio")
//            {
//                UID = 9000005;
//            }
//            if (Name == "Eros")
//            {
//                UID = 9000006;
//            }
//            if (Name == "Febo")
//            {
//                UID = 9000007;
//            }
//            if (Name == "Forcis")
//            {
//                UID = 9000008;
//            }
//            if (Name == "Hades")
//            {
//                UID = 9000009;
//            }
//            if (Name == "Hefesto")
//            {
//                UID = 9000010;
//            }
//            if (Name == "Hércules")
//            {
//                UID = 9000011;
//            }
//            if (Name == "Hermes")
//            {
//                UID = 9000012;
//            }
//            if (Name == "Helios")
//            {
//                UID = 9000013;
//            }
//            if (Name == "Marte")
//            {
//                UID = 9000014;
//            }
//            if (Name == "Mercurio")
//            {
//                UID = 9000015;
//            }
//            if (Name == "Morfeo")
//            {
//                UID = 9000016;
//            }
//            if (Name == "Neptuno")
//            {
//                UID = 9000017;
//            }
//            if (Name == "Perseo")
//            {
//                UID = 9000018;
//            }
//            if (Name == "Poseidón")
//            {
//                UID = 9000019;
//            }

//        }
//        public static uint Classes(string Name)
//        {
           
//            if (Name == "Ares")
//            {
//                return 5057;
//            }
//            if (Name == "Apolo")
//            {
//                return 1057;
//            }
//            if (Name == "Asclepio")
//            {
//                return 1057;
//            }
//            if (Name == "Baco")
//            {
//                return 5057;
//            }
//            if (Name == "Cupido")
//            {
//                return 5057;
//            }
//            if (Name == "Dionisio")
//            {
//                return 5057;
//            }
//            if (Name == "Eros")
//            {
//                return 5057;
//            }
//            if (Name == "Febo")
//            {
//                return 5057;
//            }
//            if (Name == "Forcis")
//            {
//                return 5057;
//            }
//            if (Name == "Hades")
//            {
//                return 5057;
//            }
//            if (Name == "Hefesto")
//            {
//                return 7057;
//            }
//            if (Name == "Hércules")
//            {
//                return 7057;
//            }
//            if (Name == "Hermes")
//            {
//                return 7057;
//            }
//            if (Name == "Helios")
//            {
//                return 4057;
//            }
//            if (Name == "Marte")
//            {
//                return 4057;
//            }
//            if (Name == "Mercurio")
//            {
//                return 2057;
//            }
//            if (Name == "Morfeo")
//            {
//                return 2057;
//            }
//            if (Name == "Neptuno")
//            {
//                return 8057;
//            }
//            if (Name == "Perseo")
//            {
//                return 14057;
//            }
//            if (Name == "Poseidón")
//            {
//                return 6057;
//            }
//            return 0;
//        }
//        public static uint NobilityRank(string Name)
//        {
//            if (Name == "Ares")
//            {
//                return 200000000;
//            }
//            if (Name == "Apolo")
//            {
//                return 150000000;
//            }
//            if (Name == "Asclepio")
//            {
//                return 130000000;
//            }
//            if (Name == "Baco")
//            {
//                return 100000000;
//            }
//            if (Name == "Cupido")
//            {
//                return 70000000;
//            }
//            if (Name == "Dionisio")
//            {
//                return 66000000;
//            }
//            if (Name == "Eros")
//            {
//                return 50000000;
//            }
//            if (Name == "Febo")
//            {
//                return 43000000;
//            }
//            if (Name == "Forcis")
//            {
//                return 41000000;
//            }
//            if (Name == "Hades")
//            {
//                return 39000000;
//            }
//            if (Name == "Hefesto")
//            {
//                return 37500000;
//            }
//            if (Name == "Hércules")
//            {
//                return 34000000;
//            }
//            if (Name == "Hermes")
//            {
//                return 20000000;
//            }
//            if (Name == "Helios")
//            {
//                return 18500000;
//            }
//            if (Name == "Marte")
//            {
//                return 16000000;
//            }
//            if (Name == "Mercurio")
//            {
//                return 14500000;
//            }
//            if (Name == "Morfeo")
//            {
//                return 12003000;
//            }
//            if (Name == "Neptuno")
//            {
//                return 12000000;
//            }
//            if (Name == "Perseo")
//            {
//                return 10500000;
//            }
//            if (Name == "Poseidón")
//            {
//                return 10000000;
//            }
//            return 0;
//        }
//        public static uint AnimaID(string Name)
//        {
//            if (Name == "Ares")
//            {
//                return 4200014;
//            }
//            if (Name == "Apolo")
//            {
//                return 4200014;
//            }
//            if (Name == "Asclepio")
//            {
//                return 4200014;
//            }
//            if (Name == "Baco")
//            {
//                return 4200014;
//            }
//            if (Name == "Cupido")
//            {
//                return 4200013;
//            }
//            if (Name == "Dionisio")
//            {
//                return 4200013;
//            }
//            if (Name == "Eros")
//            {
//                return 4200013;
//            }
//            if (Name == "Febo")
//            {
//                return 4200013;
//            }
//            if (Name == "Forcis")
//            {
//                return 4200013;
//            }
//            if (Name == "Hades")
//            {
//                return 4200012;
//            }
//            if (Name == "Hefesto")
//            {
//                return 4200012;
//            }
//            if (Name == "Hércules")
//            {
//                return 4200012;
//            }
//            if (Name == "Hermes")
//            {
//                return 4200012;
//            }
//            if (Name == "Helios")
//            {
//                return 4200012;
//            }
//            if (Name == "Marte")
//            {
//                return 4200012;
//            }
//            if (Name == "Mercurio")
//            {
//                return 4200012;
//            }
//            if (Name == "Morfeo")
//            {
//                return 4200012;
//            }
//            if (Name == "Neptuno")
//            {
//                return 4200012;
//            }
//            if (Name == "Perseo")
//            {
//                return 4200012;
//            }
//            if (Name == "Poseidón")
//            {
//                return 4200012;
//            }
//            return 0;
//        }
//        public static uint CupID(string Name)
//        {
//            if (Name == "Ares")
//            {
//                return 2168715;
//            }
//            if (Name == "Apolo")
//            {
//                return 2168715;
//            }
//            if (Name == "Asclepio")
//            {
//                return 2168715;
//            }
//            if (Name == "Baco")
//            {
//                return 2168725;
//            }
//            if (Name == "Cupido")
//            {
//                return 2168725;
//            }
//            if (Name == "Dionisio")
//            {
//                return 2168725;
//            }
//            if (Name == "Eros")
//            {
//                return 2168735;
//            }
//            if (Name == "Febo")
//            {
//                return 2168735;
//            }
//            if (Name == "Forcis")
//            {
//                return 2168735;
//            }
//            if (Name == "Hades")
//            {
//                return 2169035;
//            }
//            if (Name == "Hefesto")
//            {
//                return 2169035;
//            }
//            if (Name == "Hércules")
//            {
//                return 2169035;
//            }
//            if (Name == "Hermes")
//            {
//                return 2169035;
//            }
//            if (Name == "Helios")
//            {
//                return 2169035;
//            }
//            if (Name == "Marte")
//            {
//                return 2169035;
//            }
//            if (Name == "Mercurio")
//            {
//                return 2169255;
//            }
//            if (Name == "Morfeo")
//            {
//                return 2169255;
//            }
//            if (Name == "Neptuno")
//            {
//                return 2169255;
//            }
//            if (Name == "Perseo")
//            {
//                return 2169255;
//            }
//            if (Name == "Poseidón")
//            {
//                return 2169255;
//            }
//            return 0;
//        }
//        public static GameClient CreateBots(Qahr.Mobile.ServerSockets.Packet stream, BotType Type, BotLevel Level, uint Map, ushort X, ushort Y, bool HeroGathering = false, byte Eventbase = 0, uint guildid = 0)
//        {
//            GameClient Bot = Clases();
//            Bot.MyBot = new BotAttack();
//            Bot.MyBot.Bot = Bot;
//            Bot.MyBot.SetLevel(Level);
//            string name = "";
//            Bot.Player.Name = name;
//            Bot.MyBot.HeroGathering = HeroGathering;
           
                
//            Bot.MyBot.Level = Level;
//            Bot.MyBot.Type = Type;
//            switch (Type)
//            {
//                case BotType.HeroBot:
//                    {
//                        Bot.Player.UID = UID1.Next;
//                        Bot.Player.RealUID = UID2.Next;

//                        Bot.Player.DynamicID = Map;
//                        Bot.Player.X = (ushort)X;
//                        Bot.Player.Y = (ushort)Y;
//                        Bot.Player.Map = (ushort)Map;
//                        Bot.Map = Pool.ServerMaps[Map];
//                        Bot.Player.Event = true;
                      
//                        Bot.Player.Level = (ushort)140;
//                        Bot.Player.Face = (ushort)Pool.GetRandom.Next(134, 154);
//                        Bot.Player.Hair = (ushort)Pool.GetRandom.Next(1, 76);
//                        Bot = RellenarInformacion(Bot, stream);
//                        Bot.Player.View.SendView(Bot.Player.GetArray(stream, false), false);
//                        Bot.FullLoading = true;
//                        Bot.Player.CompleteLogin = true;
//                        Bot.MyBot.Bot = Bot;
//                        Bot.Map.AddAI(Bot);
//                        Bot.Player.Strength = (ushort)160;
//                        Bot.Player.Vitality = (ushort)900;
//                        Bot.Player.Agility = (ushort)120;
//                        Bot.Player.Spirit = (ushort)105;
//                        LoadChi(Bot);
//                        LoadJiangHu(Bot);
//                        BotsEquipment(stream, Bot);
//                        AddSkill(stream, Bot);


//                        Pool.GamePoll.TryAdd(Bot.Player.UID, Bot);
//                        Bot.Equipment.QueryEquipment(Bot.Equipment.Alternante);
//                        Bot.Player.HitPoints = (int)Bot.Status.MaxHitpoints;
//                        Bot.Equipment.OnDequeue();
//                        Role.GameMap.EnterMap((int)Bot.Player.Map);//???
//                        break;
//                    }
//                case BotType.QualifierBot:
//                    {

//                    TryOtherName:
//                        string names = AvailableNames[Pool.GetRandom.Next(0, AvailableNames.Length)];
//                    foreach (var value in Pool.GamePoll.Values.Where(p => p.Fake))
//                        {
//                            if (value.Player.Name == names)
//                                goto TryOtherName;
//                        }
//                        Bot.Player.Name = names;
//                        Bot.Player.UID = UID1.Next;
//                        Bot.Player.RealUID = UID2.Next;
//                        switch (Pool.GetRandom.Next(1, 12))
//                        {
//                            case 1: Bot.Player.Class = 1005; break;
//                            case 2: Bot.Player.Class = 2005; break;
//                            case 3: Bot.Player.Class = 4005; break;
//                            case 4: Bot.Player.Class = 5005; break;
//                            case 5: Bot.Player.Class = 6005; break;
//                            case 6: Bot.Player.Class = 7005; break;
//                            case 7: Bot.Player.Class = 8005; break;
//                            case 8: Bot.Player.Class = 9005; break;
//                            case 9: Bot.Player.Class = 13005; break;
//                            case 10: Bot.Player.Class = 14005; break;
//                            case 11: Bot.Player.Class = 16005; break;
//                        }
//                        Bot.Player.NobilityRank = Nobility.NobilityRank.King;
//                        Bot.Player.DynamicID = Map;
//                        Bot.Player.X = (ushort)X;
//                        Bot.Player.Y = (ushort)Y;
//                        Bot.Player.Map = (ushort)Map;
//                        Bot.Map = Pool.ServerMaps[Map];
//                        Bot.Player.Level = (ushort)140;
//                        Bot.Player.Face = (ushort)Pool.GetRandom.Next(134, 154);
//                        Bot.Player.Hair = (ushort)Pool.GetRandom.Next(1, 76);
//                        Bot = RellenarInformacion(Bot, stream);
//                        Bot.Player.View.SendView(Bot.Player.GetArray(stream, false), false);
//                        Bot.FullLoading = true;
//                        Bot.Player.CompleteLogin = true;
//                        Bot.MyBot.Bot = Bot;
//                        Bot.Map.AddAI(Bot);
//                        Bot.Player.Strength = (ushort)160;
//                        Bot.Player.Vitality = (ushort)900;
//                        Bot.Player.Agility = (ushort)120;
//                        Bot.Player.Spirit = (ushort)105;
//                        LoadChi(Bot);
//                        LoadJiangHu(Bot);
//                        BotsEquipment(stream, Bot);
//                        AddSkill(stream, Bot);


//                        Pool.GamePoll.TryAdd(Bot.Player.UID, Bot);
//                        Bot.Equipment.QueryEquipment(Bot.Equipment.Alternante);
//                        Bot.Player.HitPoints = (int)Bot.Status.MaxHitpoints;
//                        Bot.Equipment.OnDequeue();
//                        Role.GameMap.EnterMap((int)Bot.Player.Map);//???
//                        Bot.ArenaStatistic.ApplayInfo(Bot.Player);
//                        Bot.ArenaStatistic.Info.ArenaPoints = 4000;
//                        MsgSchedules.Arena.DoSignup(stream, Bot);
//                        break;
//                    }

//                case BotType.EventBots:
//                    {
//                        Client.GameClient pclient = new Client.GameClient(null);
//                        pclient.Fake = true;
//                        GameClient Bot2 = Clases();
//                        pclient.MyBot = new BotAttack();
//                        pclient.MyBot.Bot = Bot2;
//                        pclient.MyBot.SetLevel(Level);
//                        pclient.Player = new Role.Player(pclient);
//                        pclient.Inventory = new Role.Instance.Inventory(pclient);
//                        pclient.Equipment = new Role.Instance.Equip(pclient);
//                        pclient.Warehouse = new Role.Instance.Warehouse(pclient);
//                        pclient.MyProfs = new Role.Instance.Proficiency(pclient);
//                        pclient.MyVendor = new Role.Instance.Vendor(pclient);
//                        pclient.MySpells = new Role.Instance.Spell(pclient);
//                        pclient.Achievement = new Database.AchievementCollection();

                       
//                        pclient.Player.ServerID = (ushort)Database.GroupServerList.MyServerInfo.ID;
//                        pclient.Player.HitPoints = int.MaxValue;
//                        pclient.Status.MaxHitpoints = uint.MaxValue;



//                        pclient.Status = new Game.MsgServer.MsgStatus();
//                    TryOtherName:
//                        string names = BotEvent[Pool.GetRandom.Next(0, BotEvent.Length)];
//                        foreach (var value in Pool.GamePoll.Values.Where(p => p.Fake))
//                        {
//                            if (value.Player.Name == names)
//                                goto TryOtherName;
//                        }
//                        pclient.Player.Name = names;
//                        pclient.Player.Body = (ushort)BotAttack.Body[Pool.GetRandom.Next(0, BotAttack.Body.Length)];
//                        uint UIDS;
//                        UID(pclient.Player.Name, out UIDS);
//                        pclient.MyBot.Type = BotType.EventBots;
//                        pclient.Player.UID = UIDS;
//                        pclient.Player.InnerPower = new Role.Instance.InnerPower(pclient.Player.Name, pclient.Player.UID);
//                        pclient.HundredWeapons = new Role.Instance.HundredWeapons(pclient);
//                        pclient.Player.SubClass = new Role.Instance.SubClass();
//                        pclient.Player.MyUnion = new Role.Instance.Union();
//                        pclient.Player.MyChi = new Role.Instance.Chi(pclient.Player.UID);
//                        pclient.Player.MyJiangHu = new Role.Instance.JiangHu(pclient.Player.UID);
//                        pclient.Player.Associate = new Role.Instance.Associate.MyAsociats(pclient.Player.UID);
//                        pclient.Player.Nobility = new Role.Instance.Nobility(pclient);
//                        pclient.Rune = new Role.Instance.Rune(pclient);
//                        pclient.Beasts = new Role.Instance.Beasts(pclient);
//                        pclient.Player.MyClan = new Role.Instance.Clan();
//                        ushort XX;
//                        ushort YY;
//                        XY(pclient.Player.Name, out XX, out YY);
//                        pclient.Player.X = XX;
//                        pclient.Player.Y = YY;
//                        pclient.Player.Map = (ushort)Map;
//                        pclient.Map = Pool.ServerMaps[Map];
//                        pclient.Player.Class = Classes(pclient.Player.Name);
//                        //pclient.Player.Nobility.Donation = NobilityRank(pclient.Player.Name);
//                        //pclient.Send(stream.NobilityIconCreate(pclient.Player.Nobility));
//                        //Pool.NobilityRanking.UpdateRank(pclient.Player.Nobility);
//                        pclient.Player.Level = 140;
//                        pclient.Player.ServerID = (ushort)Database.GroupServerList.MyServerInfo.ID;
//                        pclient.Player.Face = (ushort)Pool.GetRandom.Next(134, 154);
//                        pclient.Player.Hair = (ushort)Pool.GetRandom.Next(1, 76);
//                        switch (Pool.GetRandom.Next(1, 3))
//                        {
//                            case 1: pclient.Player.Action = Flags.ConquerAction.Sit; break;
//                            case 2: pclient.Player.Action = Flags.ConquerAction.None; break;
//                        }
//                        pclient.Player.Angle = Role.Flags.ConquerAngle.SouthWest;
//                        pclient.Player.CompleteLogin = true;
//                        pclient.FullLoading = true;
//                        pclient.Player.Reborn = 2;
//                        pclient.MyBot.Bot = pclient;
//                        pclient.Map.AddAI(pclient);
//                        pclient.Player.Strength = (ushort)160;
//                        pclient.Player.Vitality = (ushort)900;
//                        pclient.Player.Agility = (ushort)120;
//                        pclient.Player.Spirit = (ushort)105;
//                        #region OpenChi
//                        if (!Role.Instance.Chi.ChiPool.ContainsKey(pclient.Player.UID))
//                            Role.Instance.Chi.ChiPool.TryAdd(pclient.Player.UID, pclient.Player.MyChi);
//                        pclient.Player.MyChi.Dragon.UnLocked = true;
//                        pclient.Player.MyChi.Phoenix.UnLocked = true;
//                        pclient.Player.MyChi.Tiger.UnLocked = true;
//                        pclient.Player.MyChi.Turtle.UnLocked = true;
//                        pclient.Player.MyChi.Dragon.UID = pclient.Player.UID;
//                        pclient.Player.MyChi.Dragon.Name = pclient.Player.Name;
//                        pclient.Player.MyChi.Phoenix.UID = pclient.Player.UID;
//                        pclient.Player.MyChi.Phoenix.Name = pclient.Player.Name;
//                        pclient.Player.MyChi.Tiger.UID = pclient.Player.UID;
//                        pclient.Player.MyChi.Tiger.Name = pclient.Player.Name;
//                        pclient.Player.MyChi.Turtle.UID = pclient.Player.UID;
//                        pclient.Player.MyChi.Turtle.Name = pclient.Player.Name;
//                        pclient.Player.MyChi.Dragon.Reroll(MsgChiInfo.LockedFlags.None);
//                        pclient.Player.MyChi.Phoenix.Reroll(MsgChiInfo.LockedFlags.None);
//                        pclient.Player.MyChi.Tiger.Reroll(MsgChiInfo.LockedFlags.None);
//                        pclient.Player.MyChi.Turtle.Reroll(MsgChiInfo.LockedFlags.None);
//                        MsgChiInfo.MsgHandleChi.SendInfo(pclient, MsgChiInfo.Action.Send);


//                        #endregion

//                        #region OpenJiang

//                        pclient.Player.MyJiangHu = new Role.Instance.JiangHu(pclient.Player.UID);
//                        pclient.Player.MyJiangHu.CustomizedName = pclient.Player.Name;
//                        pclient.Player.MyJiangHu.Name = pclient.Player.Name;

//                        pclient.Player.MyJiangHu.ActiveJiangMode(pclient);
//                        pclient.Player.MyJiangHu.Level = (byte)pclient.Player.Level;

//                        pclient.Player.MyJiangHu.SendInfo(pclient, MsgJiangHuInfo.JiangMode.SetName, false, pclient.Player.UID.ToString(), "1", "1");

//                        pclient.Send(stream.JiangHuStatusCreate(pclient.Player.Name, 1, pclient.Player.MyJiangHu.Talent
//                         , 0, pclient.Player.SubClass.StudyPoints, pclient.Player.MyJiangHu.FreeTimeToday));

//                        pclient.Player.SubClass.AddStudyPoints(pclient, 100, stream);
//                        pclient.Player.MyJiangHu.CreateTime();
//                        pclient.Player.MyJiangHu.SendInfo(pclient, MsgJiangHuInfo.JiangMode.UpdateTime, false, pclient.Player.MyJiangHu.FreeCourse.ToString(), pclient.Player.MyJiangHu.Time.ToString());
//                        pclient.Player.MyJiangHu.SendInfo(pclient, MsgJiangHuInfo.JiangMode.UpdateTalent, true, pclient.Player.UID.ToString(), pclient.Player.MyJiangHu.Talent.ToString());


//                        #endregion

//                        #region Get (ModeAtributes)

//                        #region JiangSuper
//                        for (byte x = 1; x < 10; x++)
//                        {
//                            if (pclient.Player.MyJiangHu != null)
//                            {
//                                pclient.Player.MyJiangHu.CreateRollSuper(stream, pclient, x, 1, 14);
//                                pclient.Player.MyJiangHu.ApplayNewStar(pclient);
//                            }
//                        }
//                        for (byte x = 1; x < 10; x++)
//                        {
//                            if (pclient.Player.MyJiangHu != null)
//                            {
//                                pclient.Player.MyJiangHu.CreateRollSuper(stream, pclient, x, 2, 12);
//                                pclient.Player.MyJiangHu.ApplayNewStar(pclient);
//                            }
//                        }
//                        for (byte x = 1; x < 10; x++)
//                        {
//                            if (pclient.Player.MyJiangHu != null)
//                            {
//                                pclient.Player.MyJiangHu.CreateRollSuper(stream, pclient, x, 3, 2);
//                                pclient.Player.MyJiangHu.ApplayNewStar(pclient);
//                            }
//                        }
//                        for (byte x = 1; x < 10; x++)
//                        {
//                            if (pclient.Player.MyJiangHu != null)
//                            {
//                                pclient.Player.MyJiangHu.CreateRollSuper(stream, pclient, x, 4, 10);
//                                pclient.Player.MyJiangHu.ApplayNewStar(pclient);
//                            }
//                        }
//                        for (byte x = 1; x < 10; x++)
//                        {
//                            if (pclient.Player.MyJiangHu != null)
//                            {
//                                pclient.Player.MyJiangHu.CreateRollSuper(stream, pclient, x, 5, 1);
//                                pclient.Player.MyJiangHu.ApplayNewStar(pclient);
//                            }
//                        }
//                        for (byte x = 1; x < 10; x++)
//                        {
//                            if (pclient.Player.MyJiangHu != null)
//                            {
//                                pclient.Player.MyJiangHu.CreateRollSuper(stream, pclient, x, 6, 12);
//                                pclient.Player.MyJiangHu.ApplayNewStar(pclient);
//                            }
//                        }
//                        for (byte x = 1; x < 10; x++)
//                        {
//                            if (pclient.Player.MyJiangHu != null)
//                            {
//                                pclient.Player.MyJiangHu.CreateRollSuper(stream, pclient, x, 7, 2);
//                                pclient.Player.MyJiangHu.ApplayNewStar(pclient);
//                            }
//                        }
//                        for (byte x = 1; x < 10; x++)
//                        {
//                            if (pclient.Player.MyJiangHu != null)
//                            {
//                                pclient.Player.MyJiangHu.CreateRollSuper(stream, pclient, x, 8, 10);
//                                pclient.Player.MyJiangHu.ApplayNewStar(pclient);
//                            }
//                        }
//                        for (byte x = 1; x < 10; x++)
//                        {
//                            if (pclient.Player.MyJiangHu != null)
//                            {
//                                pclient.Player.MyJiangHu.CreateRollSuper(stream, pclient, x, 9, 1);
//                                pclient.Player.MyJiangHu.ApplayNewStar(pclient);
//                            }
//                        }
//                        #endregion

//                        #region [MaxHP]+[P-Attack]+[PStrike]+[Immunity]
//                        MsgMessage.NpcCommands(pclient, new Game.MsgServer.MsgMessage("@chipower 1 0 6", 0, 0));
//                        MsgMessage.NpcCommands(pclient, new Game.MsgServer.MsgMessage("@chipower 1 1 7", 0, 0));
//                        MsgMessage.NpcCommands(pclient, new Game.MsgServer.MsgMessage("@chipower 1 2 3", 0, 0));
//                        MsgMessage.NpcCommands(pclient, new Game.MsgServer.MsgMessage("@chipower 1 3 1", 0, 0));
//                        MsgMessage.NpcCommands(pclient, new Game.MsgServer.MsgMessage("@chipower 2 0 6", 0, 0));
//                        MsgMessage.NpcCommands(pclient, new Game.MsgServer.MsgMessage("@chipower 2 1 7", 0, 0));
//                        MsgMessage.NpcCommands(pclient, new Game.MsgServer.MsgMessage("@chipower 2 2 3", 0, 0));
//                        MsgMessage.NpcCommands(pclient, new Game.MsgServer.MsgMessage("@chipower 2 3 1", 0, 0));
//                        MsgMessage.NpcCommands(pclient, new Game.MsgServer.MsgMessage("@chipower 3 0 6", 0, 0));
//                        MsgMessage.NpcCommands(pclient, new Game.MsgServer.MsgMessage("@chipower 3 1 7", 0, 0));
//                        MsgMessage.NpcCommands(pclient, new Game.MsgServer.MsgMessage("@chipower 3 2 3", 0, 0));
//                        MsgMessage.NpcCommands(pclient, new Game.MsgServer.MsgMessage("@chipower 3 3 1", 0, 0));
//                        MsgMessage.NpcCommands(pclient, new Game.MsgServer.MsgMessage("@chipower 4 0 6", 0, 0));
//                        MsgMessage.NpcCommands(pclient, new Game.MsgServer.MsgMessage("@chipower 4 1 7", 0, 0));
//                        MsgMessage.NpcCommands(pclient, new Game.MsgServer.MsgMessage("@chipower 4 2 3", 0, 0));
//                        MsgMessage.NpcCommands(pclient, new Game.MsgServer.MsgMessage("@chipower 4 3 1", 0, 0));
//                        #endregion

//                        #region UpdateChi
//                        Role.Instance.Chi.ComputeStatus(pclient.Player.MyChi);
//                        pclient.Equipment.QueryEquipment(pclient.Equipment.Alternante, false);
//                        MsgChiInfo.MsgHandleChi.SendInfo(pclient, MsgChiInfo.Action.Send);
//                        MsgChiInfo.MsgHandleChi.SendInfo(pclient, MsgChiInfo.Action.Upgrade);
//                        Pool.ChiRanking.Upadte(Pool.ChiRanking.Dragon, pclient.Player.MyChi.Dragon);
//                        Pool.ChiRanking.Upadte(Pool.ChiRanking.Dragon, pclient.Player.MyChi.Phoenix);
//                        Pool.ChiRanking.Upadte(Pool.ChiRanking.Dragon, pclient.Player.MyChi.Tiger);
//                        Pool.ChiRanking.Upadte(Pool.ChiRanking.Dragon, pclient.Player.MyChi.Turtle);
//                        #endregion

//                        #endregion
//                        BotsEquipment(stream, pclient);
//                        RuneAll(stream, pclient);
//                        AddSkill(stream, pclient);
//                        pclient.Player.PkMode = Flags.PKMode.Team;
//                        pclient.Player.Event = true;
//                        pclient.Player.Eventcode = Eventbase;
//                        if (pclient.Player.MyGuild == null && pclient.Player.MyGuildMember == null)
//                        {
//                            Guild Guild;
//                            if (Guild.GuildPoll.TryGetValue(guildid, out Guild))
//                            {
//                                Guild.AddPlayer(pclient.Player, stream);
//                            }
//                        }
//                        pclient.Map.Enquer(pclient);
//                        pclient.Send(pclient.Player.GetArray(stream, false));
//                        Pool.GamePoll.TryAdd(pclient.Player.UID, pclient);
//                        pclient.Player.View.Role(false, stream);
//                        #region LearnNewSubClass
//                        pclient.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.MartialArtist, pclient, stream);
//                        pclient.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Warlock, pclient, stream);
//                        pclient.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.ChiMaster, pclient, stream);
//                        pclient.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Sage, pclient, stream);
//                        pclient.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Apothecary, pclient, stream);
//                        pclient.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Wrangler, pclient, stream);
//                        pclient.Player.SubClass.LearnNewSubClass(Database.DBLevExp.Sort.Performer, pclient, stream);
//                        #endregion
//                        foreach (var m_stage in pclient.Player.InnerPower.Stages)
//                        {
//                            foreach (var m_gong in m_stage.NeiGongs)
//                            {
//                                m_stage.UnLocked = m_gong.Unlocked = true;
//                                m_gong.Score = 100;
//                                Database.InnerPowerTable.Stage DBStage = null;
//                                Database.InnerPowerTable.Stage.NeiGong DBGong = null;
//                                if (Database.InnerPowerTable.GetDBInfo(m_gong.ID, out DBStage, out DBGong))
//                                    m_gong.level = DBGong.MaxLevel;
//                            }
//                        }
//                        if (!pclient.MyAstredge.Items.ContainsKey(Astredge.TypeID.ViodragonClub))
//                        {
//                            pclient.MyAstredge.AddItem(Astredge.TypeID.ViodragonClub, 9);
//                        }
//                        pclient.Equipment.QueryEquipment(pclient.Equipment.Alternante);
//                        pclient.Player.HitPoints = (int)pclient.Status.MaxHitpoints;
//                        pclient.Equipment.OnDequeue();
//                        Role.GameMap.EnterMap((int)pclient.Player.Map);//???
//                        pclient.Teleport(pclient.Player.X, pclient.Player.Y, 1002,0,false,true);
//                        pclient.Player.AddFlag(MsgUpdate.Flags.CastPray, Role.StatusFlagsBigVector32.PermanentFlag, true);
//                        var client = Pool.GamePoll.Values.Where(p => p.Player.UID == 1293557).FirstOrDefault();
//                        if (client == null) break;
//                        if (client.Player.MyClan == null) break;
//                        if (client.Player.MyClanMember.Rank != Role.Instance.Clan.Ranks.Leader)
//                            if (client.Player.MyClan.Members.Count >= Role.Instance.Clan.MaxPlayersInClan(client.Player.MyClan.Level))
//                            {
//                                client.SendSysMesage("I'm sorry , but your clan already is at the max number of members of " + Role.Instance.Clan.MaxPlayersInClan(client.Player.MyClan.Level) + ".");
//                                break;
//                            }
//                        client.Player.MyClan.AddMember(pclient, Role.Instance.Clan.Ranks.Member, stream);
                       
//                        break;
//                    }
//            }

//            return Bot;
//        }

//        public static void AddSkill(Qahr.Mobile.ServerSockets.Packet stream, GameClient pclient)
//        {

//            #region Trojan
//            if (AtributesStatus.IsTrojan(pclient.Player.Class))
//            {
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.ScrenSword))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.ScrenSword, 4);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.FastBlader))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.FastBlader, 4);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.Hercules))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.Hercules, 4);
//                }

//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.FatalCross))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.FatalCross, 4);
//                }

//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.Celestial))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.Celestial, 4);
//                }
//            }
//            #endregion

//            #region Warrior
//            if (AtributesStatus.IsWarrior(pclient.Player.Class))
//            {
//                if (!pclient.MyArchives.Items.ContainsKey(Archives.TypeID.Dragonhowl))
//                {
//                    pclient.MyArchives.AddItem(Archives.TypeID.Dragonhowl, 1, 0, 1, 0);

//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.WaveofBlood))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.WaveofBlood, 4);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.Ironbone))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.Ironbone, 4);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.Hercules))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.Hercules, 4);
//                }
//            }
//            #endregion

//            #region Archer
//            if (AtributesStatus.IsArcher(pclient.Player.Class))
//            {
//                if (!pclient.MyArchives.Items.ContainsKey(Archives.TypeID.StoneCracker))
//                {
//                    pclient.MyArchives.AddItem(Archives.TypeID.StoneCracker, 1, 0, 1, 0);

//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.ArrowBlades))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.ArrowBlades, 9);
//                }
//            }
//            #endregion

//            #region IsNinja
//            if (AtributesStatus.IsNinja(pclient.Player.Class))
//            {
//                pclient.MyNinja.Unlocked = true;
//                if (pclient.MyNinja.Unlocked)
//                {
//                    pclient.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Dawn);
//                    pclient.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Rest);
//                    pclient.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Life);
//                    pclient.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Pain);
//                    pclient.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Limlt);
//                    pclient.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_View);
//                    pclient.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Shock);
//                    pclient.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Death);
//                    pclient.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Dawn_2);
//                    pclient.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Rest_2);
//                    pclient.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Life_2);
//                    pclient.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Pain_2);
//                    pclient.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Limlt_2);
//                    pclient.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_View_2);
//                    pclient.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Shock_2);
//                    pclient.MyNinja.AddFlag((ulong)Role.Instance.Ninja.Flags.Gate_of_Death_2);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.SuperTwofoldBlade))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.SuperTwofoldBlade, 4);
//                }

//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.WaterShockwave))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.WaterShockwave, 4);
//                }

//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.LightningKylin))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.LightningKylin, 4);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.WaterPrison))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.WaterPrison, 4);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.WhirlShuriken))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.WhirlShuriken, 4);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.SickleWind))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.SickleWind, 4);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.FlameofDestruction))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.FlameofDestruction, 4);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.WildFireball))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.WildFireball, 4);
//                }
//            }
//            #endregion

//            #region IsMonk
//            if (AtributesStatus.IsMonk(pclient.Player.Class))
//            {

//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.RadiantPalm))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.RadiantPalm, 4);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.WhirlwindKick))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.WhirlwindKick, 4);
//                }
//            }
//            #endregion

//            #region Pirate
//            if (AtributesStatus.IsPirate(pclient.Player.Class))
//            {
//                if (!pclient.MyArchives.Items.ContainsKey(Archives.TypeID.LavaNut))
//                {
//                    pclient.MyArchives.AddItem(Archives.TypeID.LavaNut, 4, 0, 1, 0);

//                }
//                if (!pclient.MyArchives.Items.ContainsKey(Archives.TypeID.FrozenNut))
//                {
//                    pclient.MyArchives.AddItem(Archives.TypeID.FrozenNut, 4, 0, 1, 0);

//                }
//                if (!pclient.MyArchives.Items.ContainsKey(Archives.TypeID.ThunderNut))
//                {
//                    pclient.MyArchives.AddItem(Archives.TypeID.ThunderNut, 4, 0, 1, 0);

//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.BladeTempest))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.BladeTempest, 4);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.SeaBurial))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.SeaBurial, 4);
//                }

//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellIDPirate.HolySanction))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellIDPirate.HolySanction, 4);
//                }

//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellIDPirate.GiantGun))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellIDPirate.GiantGun, 4);
//                }

//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellIDPirate.SandMist))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellIDPirate.SandMist, 4);
//                }

//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellIDPirate.Drukyle))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellIDPirate.Drukyle, 4);
//                }
//            }
//            #endregion

//            #region Lee
//            if (AtributesStatus.IsLee(pclient.Player.Class))
//            {
//                if (!pclient.MyArchives.Items.ContainsKey(Archives.TypeID.Dragon))
//                {
//                    pclient.MyArchives.AddItem(Archives.TypeID.Dragon, 1, 0, 1, 0);

//                }
                
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.CrackingSwipe))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.CrackingSwipe, 4);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.SplittingSwipe))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.SplittingSwipe, 4);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.DragonSlash))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.DragonSlash, 4);
//                }

//            }
//            #endregion

//            #region ThunderStriker
//            if (AtributesStatus.IsThunderStriker(pclient.Player.Class))
//            {
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.CrackingShock))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.CrackingShock, 4);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.ThunderBlast))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.ThunderBlast, 4);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.Megabolt))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.Megabolt, 4);
//                }

//            }
//            #endregion

//            #region Water
//            if (AtributesStatus.IsWater(pclient.Player.Class))
//            {
//                if (!pclient.MyArchives.Items.ContainsKey(Archives.TypeID.Vicissitude))
//                {
//                    pclient.MyArchives.AddItem(Archives.TypeID.Vicissitude, 1, 0, 1, 0);

//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.Thunder))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.Thunder, 3);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.ChainBolt))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.ChainBolt, 3);
//                }
//            }
//            #endregion

//            #region Fire
//            if (AtributesStatus.IsFire(pclient.Player.Class))
//            {
//                if (!pclient.MyArchives.Items.ContainsKey(Archives.TypeID.Vicissitude))
//                {
//                    pclient.MyArchives.AddItem(Archives.TypeID.Vicissitude, 1, 0, 1, 0);

//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.Tornado))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.Tornado, 3);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.FireofHell))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.FireofHell, 3);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.Bomb))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.Bomb, 3);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.FireMeteor))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.FireMeteor, 3);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.FireCircle))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.FireCircle, 3);
//                }
//            }
//            #endregion

//            #region Windwalker
//            if (AtributesStatus.IsWindWalker(pclient.Player.Class))
//            {
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.RageofWar))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.RageofWar, 4);
//                }
//                if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.BurntFrost))
//                {
//                    using (var rec = new ServerSockets.RecycledPacket())
//                        pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.BurntFrost, 4);
//                }
//            }
//            #endregion

//            if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.ChaoticDance))
//            {
//                using (var rec = new ServerSockets.RecycledPacket())
//                    pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.ChaoticDance, 4);
//            }
//            if (!pclient.MySpells.ClientSpells.ContainsKey((ushort)Qahr.Mobile.Role.Flags.SpellID.ChaoticDanceAttack))
//            {
//                using (var rec = new ServerSockets.RecycledPacket())
//                    pclient.MySpells.Add(rec.GetStream(), (ushort)Qahr.Mobile.Role.Flags.SpellID.ChaoticDanceAttack, 4);
//            }
//        }
//        public static GameClient Clases()
//        {
//            GameClient pclient = new GameClient(null);

//            pclient.Fake = true;
//            pclient.Player = new Role.Player(pclient);
//            pclient.Inventory = new Role.Instance.Inventory(pclient);
//            pclient.Equipment = new Role.Instance.Equip(pclient);
//            pclient.Warehouse = new Role.Instance.Warehouse(pclient);
//            pclient.MyProfs = new Role.Instance.Proficiency(pclient);
//            pclient.MyVendor = new Role.Instance.Vendor(pclient);
//            pclient.MySpells = new Role.Instance.Spell(pclient);
//            pclient.Achievement = new Database.AchievementCollection();
//            pclient.Player.UID = Pool.ClientCounter.Next;
//            pclient.Player.ServerID = (ushort)Database.GroupServerList.MyServerInfo.ID;
//            pclient.Player.HitPoints = int.MaxValue;
//            pclient.Status.MaxHitpoints = uint.MaxValue;
//            pclient.Status = new Game.MsgServer.MsgStatus();
//            pclient.Player.InnerPower = new Role.Instance.InnerPower(pclient.Player.Name, pclient.Player.UID);
//            pclient.HundredWeapons = new Role.Instance.HundredWeapons(pclient);
//            pclient.Player.SubClass = new Role.Instance.SubClass();
//            pclient.Player.MyUnion = new Role.Instance.Union();
//            pclient.Player.MyChi = new Role.Instance.Chi(pclient.Player.UID);
//            pclient.Player.MyJiangHu = new Role.Instance.JiangHu(pclient.Player.UID);
//            pclient.Player.Associate = new Role.Instance.Associate.MyAsociats(pclient.Player.UID);
//            pclient.Rune = new Role.Instance.Rune(pclient);
//            pclient.Beasts = new Role.Instance.Beasts(pclient);
//            pclient.Player.MyClan = new Role.Instance.Clan();

//            return pclient;
//        }
//        public static GameClient RellenarInformacion(GameClient pclient, Qahr.Mobile.ServerSockets.Packet stream)
//        {
//            pclient.Player.Body = (ushort)BotAttack.Body[Pool.GetRandom.Next(0, BotAttack.Body.Length)];
//            pclient.Player.VipLevel = 4;
//            pclient.Player.NobilityRank = Nobility.NobilityRank.Earl;
//            pclient.Player.Angle = Flags.ConquerAngle.SouthEast;
//            DataCore.AtributeStatus.GetStatus(pclient.Player);
//            pclient.Player.Reborn = 2;
//            pclient.Player.FirstClass = 13005;
//            pclient.Player.SecoundeClass = 2005;
//            pclient.Player.Flowers = new Flowers(pclient.Player.UID, pclient.Player.Name);
//            pclient.MyHouse = new House(pclient.Player.UID);
//            if (Role.Core.Rate(50))
//                pclient.Player.Action = Flags.ConquerAction.Sit;
//            else
//                pclient.Player.Action = Flags.ConquerAction.Sit;

//            return pclient;
//        }
//        public static MsgGameItem AddRandomRelicL1(ServerSockets.Packet stream, bool bound = false)
//        {
//            Game.MsgServer.MsgGameItem ItemDat = new Game.MsgServer.MsgGameItem();
//            ItemDat.UID = Pool.ITEM_Counter.Next;
//            ItemDat.StackSize = 1;
//            ItemDat.ITEM_ID = BaseFunc.RandFromGivingNums(new uint[5] { 4100001, 4100002, 4100003, 4100004, 4100005 });
//            ItemDat.Durability = ItemDat.MaximDurability = (ushort)((Pool.GetRandom.Next(10, 99) * 100) + 99);
//            if (bound)
//                ItemDat.Bound = 1;
//            ItemDat.RelicAttributes = new RelicAttribute[5];
//            int count = Math.Min(Pool.GetRandom.Next(1, 6), 4);
//            var Attrs = Database.RuneTable.RandomAttributes.Where(i => i.ItemID == ItemDat.ITEM_ID).ToArray();
//            for (int q = 0; q < count; q++)
//            {
//            again:
//                MsgChiInfo.ChiAttribute type = Database.ItemType.RelicAttribute(ItemDat.ITEM_ID);
//                ItemDat.RelicAttributes[q].Type = type;
//            reroll:
//                ItemDat.RelicAttributes[q].Value = (ushort)Pool.GetRandom.Next((int)Attrs.Where(i => i.Attribute == type).LastOrDefault().Min, (int)Attrs.Where(i => i.Attribute == type).LastOrDefault().Max + 1);
//                while ((double)ItemDat.RelicAttributes[q].Value / (double)Attrs.Where(i => i.Attribute == type).LastOrDefault().Max * 100d < 60 && Role.Core.Rate(55))
//                    goto reroll;
//                if (ItemDat.RelicAttributes[q].Value >= Attrs.Where(i => i.Attribute == type).LastOrDefault().Max)
//                    ItemDat.RelicAttributes[q].Value = (ushort)Attrs.Where(i => i.Attribute == type).LastOrDefault().Max;
//                ItemDat.RelicAttributes[q].Epic = Attrs.Where(i => i.Attribute == type).LastOrDefault().dwParam;
//                if ((ItemDat.RelicAttributes.Count(i => i.Type == type) > 1)
//                    || (type == MsgChiInfo.ChiAttribute.None))
//                    goto again;
//            }
//            try
//            {
//                return ItemDat;
//            }
//            catch (Exception e)
//            {
//                MyConsole.SaveException(e);
//            }
//            return null;
//        }
//        public static void BotsEquipment(Qahr.Mobile.ServerSockets.Packet stream, GameClient pclient)
//        {
//            uint PerfectionLevel = 54;
//            uint SoulArmor = 822071;
//            uint SoulH = 820073;
//            var FiveStar = Database.CoatStorage.Garments.Values.Where(p => p.Stars == 5).ToArray();
//            int Garment = Pool.GetRandom.Next(0, FiveStar.Length);
//            pclient.Equipment.AddEx(stream, (uint)FiveStar[Garment].ID, Flags.ConquerItem.Garment, 0, 1, 0, (Flags.Gem)0, (Flags.Gem)0);
//           // MyConsole.WriteLine(Pool.ItemsBase[360012].Description);
//            var LeftWeaponAccessory = Pool.ItemsBase.Values.Where(p => p.Description == "1-handedaccessory").ToArray();
//            int LeftWeaponAccessorys = Pool.GetRandom.Next(0, LeftWeaponAccessory.Length);
//            pclient.Equipment.AddEx(stream, (uint)LeftWeaponAccessory[LeftWeaponAccessorys].ID, Flags.ConquerItem.LeftWeaponAccessory, 0, 1, 0, (Flags.Gem)0, (Flags.Gem)0);


//            var RightWeaponAccessory = Pool.ItemsBase.Values.Where(p => p.Description == "1-handedaccessory").ToArray();
//            int RightWeaponAccessorys = Pool.GetRandom.Next(0, RightWeaponAccessory.Length);
//            pclient.Equipment.AddEx(stream, (uint)RightWeaponAccessory[RightWeaponAccessorys].ID, Flags.ConquerItem.RightWeaponAccessory, 0, 1, 0, (Flags.Gem)0, (Flags.Gem)0);//




//            pclient.Equipment.AddEx(stream, (uint)CupID(pclient.Player.Name), Flags.ConquerItem.Bottle, 0, 1, 0, (Flags.Gem)0, (Flags.Gem)0);// 
//            var Relic = AddRandomRelicL1(stream, false);
//            pclient.Equipment.AddEx(stream, (uint)Relic.ITEM_ID, Flags.ConquerItem.Relics, 0, 1, 0, (Flags.Gem)0, (Flags.Gem)0);//

//            pclient.Equipment.AddEx(stream, (uint)200001, Flags.ConquerItem.SteedMount, 0, 1, 0, (Flags.Gem)0, (Flags.Gem)0);//
//            pclient.Equipment.AddEx(stream, 300000, Flags.ConquerItem.Steed, 12, 0, 0, (Flags.Gem)0, (Flags.Gem)0, true, Role.Flags.ItemEffect.None, PerfectionLevel);//Steed
//            pclient.Equipment.AddEx(stream, 203009, Flags.ConquerItem.RidingCrop, 12, 0, 0, (Flags.Gem)0, (Flags.Gem)0, true, Role.Flags.ItemEffect.None, PerfectionLevel);//Crop
//            pclient.Equipment.AddEx(stream, 201009, Flags.ConquerItem.Fan, 12, 1, 0, (Flags.Gem)103, (Flags.Gem)103, true, Role.Flags.ItemEffect.None, PerfectionLevel);//Fan
//            pclient.Equipment.AddEx(stream, 202009, Flags.ConquerItem.Tower, 12, 1, 0, (Flags.Gem)123, (Flags.Gem)123, true, Role.Flags.ItemEffect.None, PerfectionLevel);//Tower
//            pclient.Equipment.AddEx(stream, 204009, Flags.ConquerItem.Wing, (byte)Pool.GetRandom.Next(1, 8), 1, 0, (Flags.Gem)103, (Flags.Gem)123, true, Role.Flags.ItemEffect.None, PerfectionLevel);//Wing
//            pclient.Equipment.AddEx(stream, 120269, Flags.ConquerItem.Necklace, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//Necklace
//            pclient.Equipment.AddEx(stream, (uint)150269, Flags.ConquerItem.Ring, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//Ring
//            pclient.Equipment.AddEx(stream, (uint)160249, Flags.ConquerItem.Boots, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//Boot
//            if (AtributesStatus.IsTrojan(pclient.Player.Class))
//            {
//                pclient.Equipment.AddEx(stream, (uint)614439, Flags.ConquerItem.RightWeapon, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//SkyBlade
//                pclient.Equipment.AddEx(stream, (uint)481439, Flags.ConquerItem.LeftWeapon, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//SquallSword
//                pclient.Equipment.AddEx(stream, (uint)130309, Flags.ConquerItem.Armor, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulArmor);//ObsidianArmor
//                pclient.Equipment.AddEx(stream, (uint)118309, Flags.ConquerItem.Head, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulH);//PeerlessCoronet
//            }
//            else if (AtributesStatus.IsWarrior(pclient.Player.Class))
//            {
//                pclient.Equipment.AddEx(stream, (uint)624439, Flags.ConquerItem.RightWeapon, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//Spear
//                pclient.Equipment.AddEx(stream, (uint)624439, Flags.ConquerItem.LeftWeapon, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//Shield
//                pclient.Equipment.AddEx(stream, (uint)131309, Flags.ConquerItem.Armor, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulArmor);//Armor
//                pclient.Equipment.AddEx(stream, (uint)111309, Flags.ConquerItem.Head, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulH);//Head
//            }
//            if (AtributesStatus.IsArcher(pclient.Player.Class))
//            {
//                pclient.Equipment.AddEx(stream, (uint)606429, Flags.ConquerItem.RightWeapon, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//Bow
//                pclient.Equipment.AddEx(stream, (uint)606429, Flags.ConquerItem.LeftWeapon, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//Bow
//                pclient.Equipment.AddEx(stream, (uint)133309, Flags.ConquerItem.Armor, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulArmor);//Armor
//                pclient.Equipment.AddEx(stream, (uint)113309, Flags.ConquerItem.Head, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulH);//Head
//            }
//            else if (AtributesStatus.IsNinja(pclient.Player.Class))
//            {
//                pclient.Equipment.AddEx(stream, (uint)616439, Flags.ConquerItem.RightWeapon, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//Katana
//                pclient.Equipment.AddEx(stream, (uint)616439, Flags.ConquerItem.LeftWeapon, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//Katana
//                pclient.Equipment.AddEx(stream, (uint)135309, Flags.ConquerItem.Armor, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulArmor);//Armor
//                pclient.Equipment.AddEx(stream, (uint)112309, Flags.ConquerItem.Head, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulH);//Head
//            }
//            if (AtributesStatus.IsMonk(pclient.Player.Class))
//            {
//                pclient.Equipment.AddEx(stream, (uint)622439, Flags.ConquerItem.RightWeapon, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//Beads
//                pclient.Equipment.AddEx(stream, (uint)622439, Flags.ConquerItem.LeftWeapon, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//Beads
//                pclient.Equipment.AddEx(stream, (uint)136309, Flags.ConquerItem.Armor, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulArmor);//Armor
//                pclient.Equipment.AddEx(stream, (uint)143309, Flags.ConquerItem.Head, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulH);//Head
//            }
//            else if (AtributesStatus.IsPirate(pclient.Player.Class))
//            {
//                pclient.Equipment.AddEx(stream, (uint)671439, Flags.ConquerItem.RightWeapon, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//SkyBlade
//                pclient.Equipment.AddEx(stream, (uint)670439, Flags.ConquerItem.LeftWeapon, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//SquallSword
//                pclient.Equipment.AddEx(stream, (uint)130309, Flags.ConquerItem.Armor, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulArmor);//ObsidianArmor
//                pclient.Equipment.AddEx(stream, (uint)118309, Flags.ConquerItem.Head, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulH);//PeerlessCoronet
//            }
//            if (AtributesStatus.IsLee(pclient.Player.Class))
//            {
//                pclient.Equipment.AddEx(stream, (uint)617439, Flags.ConquerItem.RightWeapon, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//SkyBlade
//                pclient.Equipment.AddEx(stream, (uint)617439, Flags.ConquerItem.LeftWeapon, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//SquallSword
//                pclient.Equipment.AddEx(stream, (uint)130309, Flags.ConquerItem.Armor, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulArmor);//ObsidianArmor
//                pclient.Equipment.AddEx(stream, (uint)118309, Flags.ConquerItem.Head, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulH);//PeerlessCoronet
//            }
//            else if (AtributesStatus.IsWater(pclient.Player.Class))
//            {
//                pclient.Equipment.AddEx(stream, (uint)620439, Flags.ConquerItem.RightWeapon, 12, 7, 255, (Flags.Gem)3, (Flags.Gem)3, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//BackSword
//                pclient.Equipment.AddEx(stream, (uint)619439, Flags.ConquerItem.LeftWeapon, 12, 7, 255, (Flags.Gem)3, (Flags.Gem)3, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//BackSword
//                pclient.Equipment.AddEx(stream, (uint)134309, Flags.ConquerItem.Armor, 12, 7, 255, (Flags.Gem)3, (Flags.Gem)3, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulArmor);//Armor
//                pclient.Equipment.AddEx(stream, (uint)114309, Flags.ConquerItem.Head, 12, 7, 255, (Flags.Gem)3, (Flags.Gem)3, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulH);//Head
//            }
//            if (AtributesStatus.IsFire(pclient.Player.Class))
//            {
//                pclient.Equipment.AddEx(stream, (uint)620439, Flags.ConquerItem.RightWeapon, 12, 7, 255, (Flags.Gem)3, (Flags.Gem)3, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//BackSword
//                pclient.Equipment.AddEx(stream, (uint)619439, Flags.ConquerItem.LeftWeapon, 12, 7, 255, (Flags.Gem)3, (Flags.Gem)3, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//BackSword
//                pclient.Equipment.AddEx(stream, (uint)134309, Flags.ConquerItem.Armor, 12, 7, 255, (Flags.Gem)3, (Flags.Gem)3, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulArmor);//Armor
//                pclient.Equipment.AddEx(stream, (uint)114309, Flags.ConquerItem.Head, 12, 7, 255, (Flags.Gem)3, (Flags.Gem)3, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulH);//Head
//            }
//            else if (AtributesStatus.IsWindWalker(pclient.Player.Class))
//            {
//                pclient.Equipment.AddEx(stream, (uint)626439, Flags.ConquerItem.RightWeapon, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//SkyBlade
//                pclient.Equipment.AddEx(stream, (uint)626439, Flags.ConquerItem.LeftWeapon, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//SquallSword
//                pclient.Equipment.AddEx(stream, (uint)130309, Flags.ConquerItem.Armor, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulArmor);//ObsidianArmor
//                pclient.Equipment.AddEx(stream, (uint)118309, Flags.ConquerItem.Head, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulH);//PeerlessCoronet
//            }
//            if (AtributesStatus.IsThunderStriker(pclient.Player.Class))
//            {
//                pclient.Equipment.AddEx(stream, (uint)681439, Flags.ConquerItem.RightWeapon, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//SkyBlade
//                pclient.Equipment.AddEx(stream, (uint)680439, Flags.ConquerItem.LeftWeapon, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name));//SquallSword
//                pclient.Equipment.AddEx(stream, (uint)130309, Flags.ConquerItem.Armor, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulArmor);//ObsidianArmor
//                pclient.Equipment.AddEx(stream, (uint)118309, Flags.ConquerItem.Head, 12, 7, 255, (Flags.Gem)13, (Flags.Gem)13, true, Role.Flags.ItemEffect.None, PerfectionLevel, AnimaID(pclient.Player.Name), SoulH);//PeerlessCoronet
//            }
//        }
//        public static void LoadChi(Client.GameClient user)
//        {
//            for (int i = 1; i < 5; i++)
//            {
//                var Power = user.Player.MyChi.Where(p => (int)p.Type == i).FirstOrDefault();
//                if (Power == null) continue;
//                if (!Power.UnLocked)
//                {
//                    Power.UnLocked = true;
//                    Power.UID = user.Player.UID;
//                    Power.Name = user.Player.Name;
//                }
//                Power.Attributes[0] = new Qahr.Mobile.Role.Instance.Chi.ChiAttribute();
//                Power.Attributes[0].Type = MsgChiInfo.ChiAttribute.HPAdd;
//                Power.Attributes[0].Value = 2500;
//                Power.Attributes[1] = new Qahr.Mobile.Role.Instance.Chi.ChiAttribute();
//                Power.Attributes[1].Type = MsgChiInfo.ChiAttribute.AddAttack;
//                Power.Attributes[1].Value = 1700;
//                Power.Attributes[2] = new Qahr.Mobile.Role.Instance.Chi.ChiAttribute();
//                Power.Attributes[2].Type = MsgChiInfo.ChiAttribute.Immunity;
//                Power.Attributes[2].Value = 182;
//                Power.Attributes[3] = new Qahr.Mobile.Role.Instance.Chi.ChiAttribute();
//                Power.Attributes[3].Type = MsgChiInfo.ChiAttribute.CriticalStrike;
//                Power.Attributes[3].Value = 182;
//                Power.Exp = 600000;

//            }
//        }
//        public static void LoadJiangHu(Client.GameClient user)
//        {
//            Role.Instance.JiangHu jiang = new Role.Instance.JiangHu(user.Player.UID);
//            jiang.Name = user.Player.Name;
//            jiang.CustomizedName = user.Player.Name;
//            jiang.Level = (byte)user.Player.Level;
//            jiang.Talent = 0;
//            jiang.FreeTimeToday = 0;
//            jiang.OnJiangMode = false;
//            jiang.FreeCourse = 0;
//            jiang.StartCountDwon = DateTime.Now;
//            jiang.CountDownEnd = DateTime.Now;
//            jiang.RoundBuyPoints = 0;
//            uint _Stage = 0;
//            byte Level = 0;

//            Role.Instance.JiangHu.Stage.AtributesType Type = Role.Instance.JiangHu.Stage.AtributesType.None;
//            foreach (var Stage in jiang.ArrayStages)
//            {
//                Stage.Activate = true;
//                foreach (var Star in Stage.ArrayStars)
//                {

//                    switch (_Stage)
//                    {
//                        case 0:
//                            {
//                                Type = Role.Instance.JiangHu.Stage.AtributesType.Breakthrough;
//                                Level = 5;
//                                break;
//                            }
//                        case 1:
//                            {
//                                Type = Role.Instance.JiangHu.Stage.AtributesType.Immunity;
//                                Level = 5;
//                                break;
//                            }
//                        case 2:
//                            {
//                                Type = Role.Instance.JiangHu.Stage.AtributesType.PAttack;
//                                Level = 5;
//                                break;
//                            }
//                        case 3:
//                            {
//                                Type = Role.Instance.JiangHu.Stage.AtributesType.CriticalStrike;
//                                Level = 5;
//                                break;
//                            }
//                        case 4:
//                            {
//                                Type = Role.Instance.JiangHu.Stage.AtributesType.MaxLife;
//                                Level = 5;
//                                break;
//                            }
//                        case 5:
//                            {
//                                Type = Role.Instance.JiangHu.Stage.AtributesType.Immunity;
//                                Level = 5;
//                                break;
//                            }
//                        case 6:
//                            {
//                                Type = Role.Instance.JiangHu.Stage.AtributesType.PAttack;
//                                Level = 5;
//                                break;
//                            }
//                        case 7:
//                            {
//                                Type = Role.Instance.JiangHu.Stage.AtributesType.CriticalStrike;
//                                Level = 5;
//                                break;
//                            }
//                        case 8:
//                            {
//                                Type = Role.Instance.JiangHu.Stage.AtributesType.MaxLife;
//                                Level = 5;
//                                break;
//                            }
//                    }
//                    Star.Activate = true;
//                    Star.UID = jiang.ValueToRoll(Type, Level);
//                    Star.Typ = jiang.GetValueType(Star.UID);
//                    Star.Level = jiang.GetValueLevel(Star.UID);
//                }
//                _Stage++;
//            }
//            jiang.CreateStatusAtributes(user);
//            user.Player.MyJiangHu = jiang;
//        }
//        public static void RuneAll(Qahr.Mobile.ServerSockets.Packet stream, GameClient pclient)
//        {
//            var runes = Pool.ItemsBase.Values.Where(i => (Database.ItemType.ItemPosition(i.ID) == (ushort)Role.Flags.ConquerItem.RedRune || Database.ItemType.ItemPosition(i.ID) == (ushort)Role.Flags.ConquerItem.BlueRune
//                || Database.ItemType.ItemPosition(i.ID) == (ushort)Role.Flags.ConquerItem.YellowRune)).ToArray();
//            foreach (var Rune in runes)
//            {
//                if (Database.ItemType.MaxRuneLevel(Rune.ID) == Rune.ID % 100)
//                {
//                    Game.MsgServer.MsgGameItem item = new Game.MsgServer.MsgGameItem();
//                    item.UID = Pool.ITEM_Counter.Next;
//                    item.ITEM_ID = Rune.ID;
//                    item.Durability = item.MaximDurability = Rune.Durability;
//                    if (pclient.Rune.Objects.Count(i => i.Position == (ushort)Role.Flags.ConquerItem.RunesCollection && i.ITEM_ID / 100 == Rune.ID / 100) == 0)
//                    {
//                        if (pclient.Rune.Add(item))
//                        {
//                            item.Position = (ushort)Role.Flags.ConquerItem.RunesCollection;
//                        }
//                    }
                     
//                }
//            }
//            foreach (var Equip in pclient.Rune.items.Values)
//            {
//                MsgGameItem item1;
//                if (Database.ItemType.isRedRune(Equip.ITEM_ID))
//                {
//                    if (pclient.Rune.TryGetItem2(Equip.ITEM_ID, out item1))
//                    {
//                        if (pclient.Rune.Equip(item1, (byte)101))
//                        {
//                            pclient.Send(stream.ItemUsageCreate(Game.MsgServer.MsgItemUsuagePacket.ItemUsuageID.EquipRune, Equip.UID, 101, 0, 0, 0,0, 0));
//                            pclient.Equipment.QueryEquipment(pclient.Equipment.Alternante);
//                            Database.RuneRank.Update(pclient);
//                        }
//                    }
//                }
//                if (Database.ItemType.isBlueRune(Equip.ITEM_ID))
//                {
//                    if (pclient.Rune.TryGetItem2(Equip.ITEM_ID, out item1))
//                    {
//                        if (pclient.Rune.Equip(item1, (byte)102))
//                        {
//                            pclient.Send(stream.ItemUsageCreate(Game.MsgServer.MsgItemUsuagePacket.ItemUsuageID.EquipRune, Equip.UID, 102, 0, 0, 0,0, 0));
//                            pclient.Equipment.QueryEquipment(pclient.Equipment.Alternante);
//                            Database.RuneRank.Update(pclient);
//                        }
//                    }
//                }
//                if (Database.ItemType.isYellowRune(Equip.ITEM_ID))
//                {
//                    if (pclient.Rune.TryGetItem2(4030909, out item1))
//                    {
//                        if (pclient.Rune.Equip(item1, (byte)103))
//                        {
//                            pclient.Send(stream.ItemUsageCreate(Game.MsgServer.MsgItemUsuagePacket.ItemUsuageID.EquipRune, Equip.UID, 103, 0, 0, 0,0,0));
//                            pclient.Equipment.QueryEquipment(pclient.Equipment.Alternante);
//                            Database.RuneRank.Update(pclient);
//                        }
//                    }
//                    if (pclient.Rune.TryGetItem2(4031509, out item1))
//                    {
//                        if (pclient.Rune.Equip(item1, (byte)104))
//                        {
//                            pclient.Send(stream.ItemUsageCreate(Game.MsgServer.MsgItemUsuagePacket.ItemUsuageID.EquipRune, Equip.UID, 104, 0, 0, 0,0, 0));
//                            pclient.Equipment.QueryEquipment(pclient.Equipment.Alternante);
//                            Database.RuneRank.Update(pclient);
//                        }
//                    }
//                    if (pclient.Rune.TryGetItem2(4031009, out item1))
//                    {
//                        if (pclient.Rune.Equip(item1, (byte)105))
//                        {
//                            pclient.Send(stream.ItemUsageCreate(Game.MsgServer.MsgItemUsuagePacket.ItemUsuageID.EquipRune, Equip.UID, 105, 0, 0, 0,0, 0));
//                            pclient.Equipment.QueryEquipment(pclient.Equipment.Alternante);
//                            Database.RuneRank.Update(pclient);
//                        }
//                    }
//                }
//            }

//            pclient.Send(stream.CreateRuneStorage(new MsgRuneStorage.MsgRuneStorageProto() { Type = 2, HPAdd = pclient.Rune.HPClient }));
//            pclient.Send(stream.CreateBeastsInfo(new MsgBeastsInfo.MsgBeastsInfoProto() { UID = pclient.Player.UID, Level = pclient.Beasts.Level, Activated = pclient.Rune.EquippedCount >= 5, FixedLevel = pclient.Beasts.FixedLevel, Points = pclient.Beasts.Points, Unknown2 = 1, Flag = pclient.Beasts.Flag }));
//            pclient.Rune.Show(stream);
//            if (Database.AtributesStatus.IsNinja(pclient.Player.Class))
//            {
//                foreach (var i in NinjaFile.gouyu_type.Values)
//                {
//                    if (i.Level == 9)
//                    {
//                        pclient.MyNinja.AddItem(i.ItemID, 9, 0, 0, 0);
//                    }
//                }
//                for (byte x = 1; x < 8; x++)
//                {
//                    if (x == 1 && pclient.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Dawn)
//                                    || x == 2 && pclient.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Rest)
//                                    || x == 3 && pclient.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Life)
//                                    || x == 4 && pclient.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Pain)
//                                    || x == 5 && pclient.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Limlt)
//                                    || x == 6 && pclient.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_View)
//                                    || x == 7 && pclient.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Shock)
//                                    || x == 8 && pclient.MyNinja.ContainsFlag((ulong)Ninja.Flags.Gate_of_Death)
//                                    )
//                    {
//                        Ninja.Item item;
//                        if (pclient.MyNinja.TryGetValue((uint)100*x, out item))
//                        {
//                            var Info = new MsgGouYuOpt.MsgGouYuOptProto();
//                            Info.Type = MsgGouYuOpt.MsgGouYuOptProto.TypeID.Equip;
//                            item.Position = Info.Position;
//                            Info.ID = item.ItemID;
//                            Info.Position = item.Position;
//                            Info.Points = item.Level;
//                            pclient.Send(stream.CreateNinjaInfo(Info));

//                            pclient.MyNinja.GetLevel();
//                            pclient.MyNinja.Alternate();
//                            pclient.Equipment.QueryEquipment(pclient.Equipment.Alternante);
//                        }
//                    }
//                }
//            }

//            if (Database.AtributesStatus.IsPirate(pclient.Player.Class))
//            {
//                foreach (var i in dict_lottery.DaoqiItem.Values)
//                {
//                    if (i.ID >= 1 && i.ID <= 25)
//                    {
//                        pclient.MyArchives.AddRune(i.IDItem);
//                    }
//                }
//                Archives.Item weapon;
//                if (pclient.MyArchives.Items.TryGetValue(Archives.TypeID.ThunderNut, out weapon))
//                {
//                    var pQuery = new MsgCombatGearTao.ProtoStructure();
//                    weapon.Jades[0].JadeID = 1007;
//                    weapon.Jades[1].JadeID = 2007;
//                    weapon.Jades[2].JadeID = 3007;
//                    weapon.Jades[3].JadeID = 5007;
//                    weapon.Jades[4].JadeID = 6007;
//                    weapon.Jades[5].JadeID = 7007;
//                    pQuery.unk4 = 1;
//                    var Rune = weapon.Jades.ToArray()[pQuery.Postion - 1];
//                    pclient.MyArchives.AddRuneSkill((uint)Rune.JadeID);
//                    pclient.Send(stream.CreateTao(pQuery));
//                    pclient.Equipment.QueryEquipment(pclient.Equipment.Alternante);

//                }
//                if (pclient.MyArchives.Items.TryGetValue(Archives.TypeID.FrozenNut, out weapon))
//                {
//                    var pQuery = new MsgCombatGearTao.ProtoStructure();
//                    weapon.Jades[0].JadeID = 8007;
//                    weapon.Jades[1].JadeID = 9007;
//                    weapon.Jades[2].JadeID = 10007;
//                    weapon.Jades[3].JadeID = 11007;
//                    weapon.Jades[4].JadeID = 12007;
//                    weapon.Jades[5].JadeID = 13007;
//                    pQuery.unk4 = 1;
//                    var Rune = weapon.Jades.ToArray()[pQuery.Postion - 1];
//                    pclient.MyArchives.AddRuneSkill((uint)Rune.JadeID);
//                    pclient.Send(stream.CreateTao(pQuery));
//                    pclient.Equipment.QueryEquipment(pclient.Equipment.Alternante);

//                }
//                if (pclient.MyArchives.Items.TryGetValue(Archives.TypeID.LavaNut, out weapon))
//                {
//                    var pQuery = new MsgCombatGearTao.ProtoStructure();
//                    weapon.Jades[0].JadeID = 14007;
//                    weapon.Jades[1].JadeID = 15007;
//                    weapon.Jades[2].JadeID = 16007;
//                    weapon.Jades[3].JadeID = 17007;
//                    weapon.Jades[4].JadeID = 18007;
//                    weapon.Jades[5].JadeID = 19007;
//                    pQuery.unk4 = 1;
//                    var Rune = weapon.Jades.ToArray()[pQuery.Postion - 1];
//                    pclient.MyArchives.AddRuneSkill((uint)Rune.JadeID);
//                    pclient.Send(stream.CreateTao(pQuery));
//                    pclient.Equipment.QueryEquipment(pclient.Equipment.Alternante);

//                }
//            }
//        }
       
//    }
//}
