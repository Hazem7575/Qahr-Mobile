using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Game.MsgFloorItem;
using Qahr.Mobile.MsgInterServer.Packets;
using Qahr.Mobile.Game.MsgNpc;
using Qahr.Mobile.WindowsAPI;
using Qahr.Mobile.Role;

namespace Qahr.Mobile.Game.MsgMonster
{
    public unsafe class MonsterRole : Role.IMapObj
    {


        public DateTime RemoveFloor = DateTime.Now;
        public int StampFloorSecounds = 0;
        public static List<uint> SpecialMonsters = new List<uint>()
        {
            3130,
            3134,
            4171,
            4212,
            4220,
            29300,
            29360,
            29363,
            29370,
            6819,
            60979,
            2751,
            5355,
            3970,
            7985,
            3971,
            7483,
            3976,
            3977,
            3978,
            3979,
            3981,
            3982,
            3983,
            3985,
            3986,
            2984
        };

        public Client.GameClient AttackerScarofEarthl;
        public Database.MagicType.Magic ScarofEarthl;

        public int ExtraDamage { get { return Family.extra_damage; } }
        public int BattlePower { get { return Family.extra_battlelev; } }
        public bool AllowDynamic { get; set; }
        public bool IsTrap() { return false; }
        public uint IndexInScreen { get; set; }

        public Client.GameClient OwnerFloor;
        public Database.MagicType.Magic DBSpell;
        public ushort SpellLevel = 0;
        public DateTime FloorStampTimer = new DateTime();
        public bool IsFloor = false;
        public Game.MsgFloorItem.MsgItemPacket FloorPacket;

        public string Name { get; set; }
        public bool BlackSpot = false;
        public DateTime Stamp_BlackSpot = new DateTime();


        public int SizeAdd { get { return Family.AttackRange; } }

        public byte PoisonLevel = 0;

        private DateTime DeadStamp = new DateTime();
        private DateTime FadeAway = new DateTime();
        public DateTime RespawnStamp = new DateTime();
        public DateTime MoveStamp = new DateTime();
        public object scoreObj = new object();
        public Dictionary<uint, MsgBossHarmRankingEntry> Hunters = new Dictionary<uint, MsgBossHarmRankingEntry>();
        public bool CanRespawn(Role.GameMap map)
        {
            DateTime Now = DateTime.Now;
            if (Now > RespawnStamp)
            {
                if (!map.MonsterOnTile(RespawnX, RespawnY))
                {
                    return true;
                }
            }
            return false;

        }
        public bool logged = false;
        public uint GetScorePosition(Client.GameClient killer)
        {
            lock (scoreObj)
            {
                if (this.Hunters == null) return 0;

                var a = Hunters.Values.OrderByDescending(p => p.HunterScore).ToArray();
                if (!Alive && !logged)
                {
                    string log = "[Bosses] Name: " + Name + " | Killer: " + killer.Player.Name + " | Hunters: ";
                    foreach (var hunter in a.Take(5))
                        log += hunter.HunterName + "     |      ";
                    Database.ServerDatabase.LoginQueue.Enqueue(log);
                    logged = true;
                }
                for (int x = 0; x < a.Length; x++)
                {
                    if (a[x].HunterUID == killer.Player.UID)
                        return (uint)(x + 1);
                }
                return 0;
            }
        }
        public void UpdateScore(ServerSockets.Packet stream, Client.GameClient killer, uint Damage)
        {
            try
            {
                lock (scoreObj)
                {
                    if (this.Hunters == null)
                        this.Hunters = new Dictionary<uint, MsgBossHarmRankingEntry>();
                    if (this.Hunters.ContainsKey(killer.Player.UID))
                        this.Hunters[killer.Player.UID].HunterScore += Damage;
                    else
                        this.Hunters.Add(killer.Player.UID, new MsgBossHarmRankingEntry() { HunterUID = killer.Player.UID, HunterScore = Damage, Rank = 0, HunterName = killer.Player.Name, ServerID = killer.Player.ServerID });

                    var array = this.Hunters.Values.OrderByDescending(p => p.HunterScore).ToArray();
                    for (int x = 0; x < array.Length; x++)
                    {
                        array[x].Rank = (uint)(x + 1);
                    }
                }
            }
            catch (Exception ex)
            {
                MyConsole.WriteException(ex);
                MyConsole.SaveException(ex);
            }
        }
        public void Respawn(bool SendEffect = true)
        {
            using (var rev = new ServerSockets.RecycledPacket())
            {
                var stream = rev.GetStream();

                ClearFlags(false);

                HitPoints = (uint)Family.MaxHealth;
                State = MobStatus.Idle;

                Game.MsgServer.ActionQuery action;

                action = new MsgServer.ActionQuery()
                {
                    ObjId = UID,
                    Type = MsgServer.ActionType.RemoveEntity
                };

                Send(stream.ActionCreate(action));

                Send(GetArray(stream, false));

                if (SendEffect)
                {
                    action.Type = ActionType.ReviveMonster;
                    Send(stream.ActionCreate(action));
                }

                if (Family.MaxHealth > ushort.MaxValue)
                {
                    Game.MsgServer.MsgUpdate Upd = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                    stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.MaxHitpoints, Family.MaxHealth);
                    Send(Upd.GetArray(stream));
                    stream = Upd.Append(stream, Game.MsgServer.MsgUpdate.DataType.Hitpoints, HitPoints);
                    Send(Upd.GetArray(stream));
                }

            }
        }
        public void SendSysMesage(string Messaj, Game.MsgServer.MsgMessage.ChatMode ChatType = Game.MsgServer.MsgMessage.ChatMode.TopLeft
          , Game.MsgServer.MsgMessage.MsgColor color = Game.MsgServer.MsgMessage.MsgColor.red)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();
                Server.SendGlobalPacket(new Game.MsgServer.MsgMessage(Messaj, color, ChatType).GetArray(stream));
            }
        }
        public void SendBossSysMesage(string KillerName, int StudyPoints, Game.MsgServer.MsgMessage.ChatMode ChatType = Game.MsgServer.MsgMessage.ChatMode.Center
           , Game.MsgServer.MsgMessage.MsgColor color = Game.MsgServer.MsgMessage.MsgColor.red)
        {
            //SendSysMesage("The " + Name.ToString() + " has been destroyed by the team " + KillerName.ToString() + "`s ! All team members received " + StudyPoints.ToString() + " Study Points!", ChatType, color);

        }
        public void GetBossReward(Client.GameClient killer, ServerSockets.Packet stream)
        {
            if (Family.ID == 4220) // ( NemesisTyrant ) - [ Dragon Island ]
            {
                foreach (var hunter in Hunters.Values)
                {
                    if (Pool.GamePoll.ContainsKey(hunter.HunterUID))
                    {
                        var client = Pool.GamePoll[hunter.HunterUID];
                        switch (this.GetScorePosition(client))
                        {
                            case 1: // Prize Rank (1)
                                {
                                    client.Player.ConquerPoints += 5000;
                                    client.SendSysMesage("You Got Rank (1) In Killing The Monster and received 5.000 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }


                            case 2: // Prize Rank (2) 
                                {
                                    client.Player.ConquerPoints += 3000;
                                    client.SendSysMesage("You Got Rank (2) In Killing The Monster and received 3.000 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }


                            case 3: // Prize Rank (3)  
                                {
                                    client.Player.ConquerPoints += 1500;
                                    client.SendSysMesage("You Got Rank (3) In Killing The Monster and received 1.500 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }
                        }
                    }
                }
            }





            if (Family.ID == 4171) // ( Snow Banshee ) - [ Dragon Island ]
            {
                foreach (var hunter in Hunters.Values)
                {
                    if (Pool.GamePoll.ContainsKey(hunter.HunterUID))
                    {
                        var client = Pool.GamePoll[hunter.HunterUID];
                        switch (this.GetScorePosition(client))
                        {
                            case 1: // Prize Rank (1)
                                {
                                    client.Player.ConquerPoints += 5000;
                                    client.SendSysMesage("You Got Rank (1) In Killing The Monster and received 5.000 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }


                            case 2: // Prize Rank (2) 
                                {
                                    client.Player.ConquerPoints += 3000;
                                    client.SendSysMesage("You Got Rank (2) In Killing The Monster and received 3.000 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }


                            case 3: // Prize Rank (3)  
                                {
                                    client.Player.ConquerPoints += 1500;
                                    client.SendSysMesage("You Got Rank (3) In Killing The Monster and received 1.500 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }
                        }
                    }
                }
            }




            if (Family.ID == 4212) // ( ThrillingSpook ) - [ Dragon Island ]
            {
                foreach (var hunter in Hunters.Values)
                {
                    if (Pool.GamePoll.ContainsKey(hunter.HunterUID))
                    {
                        var client = Pool.GamePoll[hunter.HunterUID];
                        switch (this.GetScorePosition(client))
                        {
                            case 1: // Prize Rank (1)
                                {
                                    client.Player.ConquerPoints += 5000;
                                    client.SendSysMesage("You Got Rank (1) In Killing The Monster and received 5.000 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }


                            case 2: // Prize Rank (2) 
                                {
                                    client.Player.ConquerPoints += 3000;
                                    client.SendSysMesage("You Got Rank (2) In Killing The Monster and received 3.000 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }


                            case 3: // Prize Rank (3)  
                                {
                                    client.Player.ConquerPoints += 1500;
                                    client.SendSysMesage("You Got Rank (3) In Killing The Monster and received 1.500 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }
                        }
                    }
                }
            }





            if (Family.ID == 3971) // ( DragonWraith ) - [ Deity Land ]
            {
                foreach (var hunter in Hunters.Values)
                {
                    if (Pool.GamePoll.ContainsKey(hunter.HunterUID))
                    {
                        var client = Pool.GamePoll[hunter.HunterUID];
                        switch (this.GetScorePosition(client))
                        {
                            case 1: // Prize Rank (1)
                                {
                                    client.Player.ConquerPoints += 5000;
                                    client.SendSysMesage("You Got Rank (1) In Killing The Monster and received 5.000 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }


                            case 2: // Prize Rank (2) 
                                {
                                    client.Player.ConquerPoints += 3000;
                                    client.SendSysMesage("You Got Rank (2) In Killing The Monster and received 3.000 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }


                            case 3: // Prize Rank (3)  
                                {
                                    client.Player.ConquerPoints += 1500;
                                    client.SendSysMesage("You Got Rank (3) In Killing The Monster and received 1.500 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }
                        }
                    }
                }
            }




            if (Family.ID == 3977) // ( ChillingSpook ) - [ Deity land ]
            {
                foreach (var hunter in Hunters.Values)
                {
                    if (Pool.GamePoll.ContainsKey(hunter.HunterUID))
                    {
                        var client = Pool.GamePoll[hunter.HunterUID];
                        switch (this.GetScorePosition(client))
                        {
                            case 1: // Prize Rank (1)
                                {
                                    client.Player.ConquerPoints += 5000;
                                    client.SendSysMesage("You Got Rank (1) In Killing The Monster and received 5.000 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }


                            case 2: // Prize Rank (2) 
                                {
                                    client.Player.ConquerPoints += 3000;
                                    client.SendSysMesage("You Got Rank (2) In Killing The Monster and received 3.000 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }


                            case 3: // Prize Rank (3)  
                                {
                                    client.Player.ConquerPoints += 1500;
                                    client.SendSysMesage("You Got Rank (3) In Killing The Monster and received 1.500 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }
                        }
                    }
                }
            }





            if (Family.ID == 3976) // ( BloodyBanshee ) - [ Deity land ]
            {
                foreach (var hunter in Hunters.Values)
                {
                    if (Pool.GamePoll.ContainsKey(hunter.HunterUID))
                    {
                        var client = Pool.GamePoll[hunter.HunterUID];
                        switch (this.GetScorePosition(client))
                        {
                            case 1: // Prize Rank (1)
                                {
                                    client.Player.ConquerPoints += 5000;
                                    client.SendSysMesage("You Got Rank (1) In Killing The Monster and received 5.000 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }


                            case 2: // Prize Rank (2) 
                                {
                                    client.Player.ConquerPoints += 3000;
                                    client.SendSysMesage("You Got Rank (2) In Killing The Monster and received 3.000 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }


                            case 3: // Prize Rank (3)  
                                {
                                    client.Player.ConquerPoints += 1500;
                                    client.SendSysMesage("You Got Rank (3) In Killing The Monster and received 1.500 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }
                        }
                    }
                }
            }





            if (Family.ID == 3978) // ( NetherTyrant ) - [ Deity land ]
            {
                foreach (var hunter in Hunters.Values)
                {
                    if (Pool.GamePoll.ContainsKey(hunter.HunterUID))
                    {
                        var client = Pool.GamePoll[hunter.HunterUID];
                        switch (this.GetScorePosition(client))
                        {
                            case 1: // Prize Rank (1)
                                {
                                    client.Player.ConquerPoints += 5000;
                                    client.SendSysMesage("You Got Rank (1) In Killing The Monster and received 5.000 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }


                            case 2: // Prize Rank (2) 
                                {
                                    client.Player.ConquerPoints += 3000;
                                    client.SendSysMesage("You Got Rank (2) In Killing The Monster and received 3.000 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }


                            case 3: // Prize Rank (3)  
                                {
                                    client.Player.ConquerPoints += 1500;
                                    client.SendSysMesage("You Got Rank (3) In Killing The Monster and received 1.500 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }
                        }
                    }
                }
            }




            if (Family.ID == 3970) // ( QueenofEvil ) - [ Deity land ]
            {
                foreach (var hunter in Hunters.Values)
                {
                    if (Pool.GamePoll.ContainsKey(hunter.HunterUID))
                    {
                        var client = Pool.GamePoll[hunter.HunterUID];
                        switch (this.GetScorePosition(client))
                        {
                            case 1: // Prize Rank (1)
                                {
                                    client.Player.ConquerPoints += 50000;
                                    client.SendSysMesage("You Got Rank (1) In Killing The Monster and received 50.000 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }


                            case 2: // Prize Rank (2) 
                                {
                                    client.Player.ConquerPoints += 30000;
                                    client.SendSysMesage("You Got Rank (2) In Killing The Monster and received 30.000 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }


                            case 3: // Prize Rank (3)  
                                {
                                    client.Player.ConquerPoints += 15000;
                                    client.SendSysMesage("You Got Rank (3) In Killing The Monster and received 15.000 CPs", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red, false, true);
                                    break;
                                }

                        }
                    }
                }
            }
        }
        public void Dead(ServerSockets.Packet stream, Client.GameClient killer, uint aUID, Role.GameMap GameMap)
        {
            if (Alive)
            {
                if (IsFloor)
                {

                    FloorPacket.DropType = MsgFloorItem.MsgDropID.RemoveEffect;
                    if (FloorPacket.m_ID == Game.MsgFloorItem.MsgItemPacket.Thundercloud)
                    {
                        ActionQuery _action;

                        _action = new ActionQuery()
                        {
                            ObjId = this.FloorPacket.m_UID,
                            Type = ActionType.RemoveEntity
                        };

                        this.View.SendScreen(stream.ActionCreate(_action), this.GMap);


                        GMap.View.LeaveMap<Role.IMapObj>(this);
                        HitPoints = 0;
                        GameMap.SetMonsterOnTile(X, Y, false);
                    }
                    else if (FloorPacket.m_ID == Game.MsgFloorItem.MsgItemPacket.AuroraLotus)
                    {
                        byte revivers = SpellLevel >= 6 ? (byte)2 : (byte)1;
                        foreach (var user in View.Roles(GameMap, Role.MapObjectType.Player))
                        {
                            if (revivers == 0)
                                break;
                            if (user.Alive == false)
                            {
                                if (Role.Core.GetDistance(user.X, user.Y, X, Y) < 5)
                                {
                                    revivers--;
                                    var player = user as Role.Player;
                                    if (player.ContainFlag(MsgUpdate.Flags.SoulShackle) == false)
                                        player.Revive(stream);

                                }
                            }
                            user.Send(GetArray(stream, false));
                        }
                        GMap.View.LeaveMap<Role.IMapObj>(this);
                    }
                    else if (FloorPacket.m_ID == Game.MsgFloorItem.MsgItemPacket.FlameLotus)
                    {
                        FloorPacket.DropType = MsgFloorItem.MsgDropID.RemoveEffect;
                        int counter = 1;
                        foreach (var user in View.Roles(GameMap, Role.MapObjectType.Player))
                        {
                            if (user.UID != OwnerFloor.Player.UID && Role.Core.GetDistance(user.X, user.Y, this.X, this.Y) < 5)
                            {
                                var player = user as Role.Player;

                                Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                Game.MsgServer.AttackHandler.Calculate.Magic.OnPlayer(this.OwnerFloor.Player, player, this.DBSpell, out AnimationObj);
                                Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(AnimationObj, this.OwnerFloor, player);
                                AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, 0);

                                InteractQuery Attack = new InteractQuery();
                                Attack.UID = this.UID;
                                Attack.OpponentUID = player.UID;
                                Attack.Damage = (int)AnimationObj.Damage;
                                Attack.Effect = (uint)AnimationObj.Effect;
                                Attack.X = player.X;
                                Attack.Y = player.Y;
                                Attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;

                                stream.InteractionCreate(Attack);

                                player.View.SendView(stream, true);

                                MsgSpell fireCurse;
                                if (OwnerFloor.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.FireCurse, out fireCurse))
                                {
                                    Database.MagicType.Magic fireCurseDB = Pool.Magic[(ushort)Role.Flags.SpellID.FireCurse][OwnerFloor.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.FireCurse].Level];
                                    using (var recycledPacket = new ServerSockets.RecycledPacket())
                                    {
                                        var streamm = recycledPacket.GetStream();
                                        MsgSpellAnimation FireCurseMsgSpell = new MsgSpellAnimation(this.UID
                         , player.UID, player.X, player.Y, fireCurse.ID
                         , fireCurse.Level, fireCurse.UseSpellSoul);
                                        FireCurseMsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { Damage = (uint)(fireCurseDB.DamageOnHuman * counter), Effect = MsgAttackPacket.AttackEffect.FireCurse, Hit = 1, UID = player.UID });
                                        Game.MsgServer.AttackHandler.ReceiveAttack.Player.Execute(FireCurseMsgSpell.Targets.FirstOrDefault(), this.OwnerFloor, player);
                                        FireCurseMsgSpell.SetStream(streamm); FireCurseMsgSpell.Send(player.Owner);
                                        counter++;
                                    }
                                }

                            }
                            user.Send(this.GetArray(stream, false));
                        }
                        foreach (var obj in View.Roles(GameMap, Role.MapObjectType.Monster))
                        {
                            if (obj.UID != this.UID && Role.Core.GetDistance(obj.X, obj.Y, this.X, this.Y) < 5)
                            {
                                var monster = obj as Game.MsgMonster.MonsterRole;

                                Game.MsgServer.MsgSpellAnimation.SpellObj AnimationObj;
                                Game.MsgServer.AttackHandler.Calculate.Magic.OnMonster(this.OwnerFloor.Player, monster, this.DBSpell, out AnimationObj);
                                Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(stream, AnimationObj, monster.OwnerFloor, monster);
                                AnimationObj.Damage = Game.MsgServer.AttackHandler.Calculate.Base.CalculateSoul(AnimationObj.Damage, 0);

                                InteractQuery Attack = new InteractQuery();
                                Attack.UID = this.UID;
                                Attack.OpponentUID = this.UID;
                                Attack.Damage = (int)AnimationObj.Damage;
                                Attack.Effect = (uint)AnimationObj.Effect;
                                Attack.X = this.X;
                                Attack.Y = this.Y;
                                Attack.AtkType = (ushort)MsgAttackPacket.AttackID.Physical;

                                stream.InteractionCreate(Attack);


                                monster.View.SendScreen(stream, this.OwnerFloor.Map);

                                MsgSpell fireCurse;
                                if (OwnerFloor.MySpells.ClientSpells.TryGetValue((ushort)Role.Flags.SpellID.FireCurse, out fireCurse))
                                {
                                    Database.MagicType.Magic fireCurseDB = Pool.Magic[(ushort)Role.Flags.SpellID.FireCurse][OwnerFloor.MySpells.ClientSpells[(ushort)Role.Flags.SpellID.FireCurse].Level];
                                    using (var recycledPacket = new ServerSockets.RecycledPacket())
                                    {
                                        var streamm = recycledPacket.GetStream();
                                        MsgSpellAnimation FireCurseMsgSpell = new MsgSpellAnimation(this.UID
                         , monster.UID, monster.X, monster.Y, fireCurse.ID
                         , fireCurse.Level, fireCurse.UseSpellSoul);
                                        FireCurseMsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj() { Damage = (uint)(fireCurseDB.DamageOnHuman * counter), Effect = MsgAttackPacket.AttackEffect.FireCurse, Hit = 1, UID = monster.UID });
                                        Game.MsgServer.AttackHandler.ReceiveAttack.Monster.Execute(streamm, FireCurseMsgSpell.Targets.FirstOrDefault(), this.OwnerFloor, monster);
                                        FireCurseMsgSpell.SetStream(streamm); FireCurseMsgSpell.Send(monster);
                                        counter++;
                                    }
                                }
                            }

                        }
                        GMap.View.LeaveMap<Role.IMapObj>(this);
                    }
                    HitPoints = 0;
                    GameMap.SetMonsterOnTile(X, Y, false);
                    return;
                }


                bool noRespawn =
    Family.ID == 4220 ||
    Family.ID == 4171 ||
    Family.ID == 4212 ||
    Family.ID == 3970 ||
    Family.ID == 3978 ||
    Family.ID == 3977 ||
    Family.ID == 3971 ||
    Family.ID == 2700 ||
    Family.ID == 3636 ||
    Family.ID == 40124 ||
    Family.ID == 3976;

                if (noRespawn)
                {
                    RespawnStamp = DateTime.Now.AddDays(9999);
                }
                else
                {
                    RespawnStamp = DateTime.Now.AddSeconds(8 + Family.RespawnTime);
                }

                if (BlackSpot)
                {
                    Send(stream.BlackspotCreate(false, UID));
                    BlackSpot = false;
                }
                ClearFlags(false);
                HitPoints = 0;
                AddFlag(MsgServer.MsgUpdate.Flags.Dead, Role.StatusFlagsBigVector32.PermanentFlag, true);
                DeadStamp = DateTime.Now;

                InteractQuery action = new InteractQuery()
                {
                    UID = aUID,
                    KilledMonster = 1,
                    X = this.X,
                    Y = this.Y,
                    AtkType = (ushort)MsgAttackPacket.AttackID.Death,
                    OpponentUID = UID
                };


                if (killer != null && killer.Player != null)
                {

                    #region XpSkill
                    if (killer.Player.OnXPSkill() == MsgUpdate.Flags.SuperCyclone || killer.Player.OnXPSkill() == MsgUpdate.Flags.Cyclone
                               || killer.Player.OnXPSkill() == MsgUpdate.Flags.Superman)
                    {
                        killer.Player.XPCount++;
                        killer.Player.KillCounter++;

                        if (killer.Player.OnXPSkill() != MsgServer.MsgUpdate.Flags.Normal)
                        {

                            action.SpellLevel = killer.Player.KillCounter;
                            killer.Player.UpdateXpSkill();
                        }

                    }
                    else if (killer.Player.OnXPSkill() == MsgUpdate.Flags.Normal)
                    {
                        killer.Player.XPCount++;
                    }
                    else if (killer.Player.OnXPSkill() != MsgUpdate.Flags.BladeFlurry)
                    {
                        if (killer.Player.OnXPSkill() != MsgUpdate.Flags.Omnipotence)
                        {
                            killer.Player.KillCounter++;
                            if (killer.Player.KillCounter % 4 == 0)
                                killer.Player.XPCount++;
                        }
                    }


                    #endregion

                    Send(stream.InteractionCreate(action));
                    // [QuestEngine] generic quest progress for every kill: kill counters, Gather counters and quest-item
                    // drops (only for players who have the quest and still need the item). Never throws.
                    global::Qahr.Mobile.QuestEngine.QuestEngineCore.OnMonsterKilled(this, killer, stream);
                    global::Qahr.Mobile.PuppetTheater.PuppetNpcs.OnKill(this, killer);// [PuppetTheater 2026-10-15] Vault Palace SilkSpecter / WeepWood puppet fragments
                    if (RemoveOnDead)
                    {
                        AddFlag(MsgUpdate.Flags.FadeAway, 1, false);
                        GMap.View.LeaveMap<Role.IMapObj>(this);
                        if (GMap.IsFlagPresent(X, Y, Role.MapFlagType.Monster))
                            GMap.cells[X, Y] &= ~Role.MapFlagType.Monster;



                    }
                    // [QuestEngine] a personal story boss / instance monster spawned by the quest engine: quest credit only
                    // (given above), no loot, no event drops - an accept/abandon loop can never farm it.
                    if (global::Qahr.Mobile.QuestEngine.QuestEngineCore.ConsumeQuestSpawn(this))
                        return;
                    if (global::Qahr.Mobile.NewYear2023.SpringHooks.OnKill(this, killer, stream)) return;// [NewYear2023 2026-10-15] Spring Festival group: personal daily monsters (no loot) + hunting drops (Spring Greeting Card)

                    #region Drop Game
                    if (Boss == 0 && Map != 10133 && Map != 3071 && Map != 10741)
                        if (killer.Player.OnAutoHunt == true && killer.Player.VipLevel == 0)
                        {
                            if (Role.Core.Rate(1, 2)) // Lucky Percentage
                            {
                                killer.Player.ConquerPoints += 1;
                            }
                           
                            if (Role.Core.Rate(1, 100)) // Lucky Percentage
                            {
                                DropItemID(killer, 722136, stream);
                                killer.SendSysMesage("An ExpBall (Event) has dropped! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                            if (Role.Core.Rate(1, 2000)) // Lucky Percentage
                            {
                                DropItemID(killer, 1088000, stream);
                                Game.MsgTournaments.MsgSchedules.SendSysMesage("" + killer.Player.Name + " has dropped a DragonBall!", Game.MsgServer.MsgMessage.ChatMode.World, Game.MsgServer.MsgMessage.MsgColor.yellow);
                            }
                        }


                        else if (killer.Player.VipLevel == 0)
                        {



                            if (Role.Core.Rate(1, 2)) // Lucky Percentage
                            {
                                killer.Player.ConquerPoints += 1;
                            }
             
                            if (Role.Core.Rate(1, 100)) // Lucky Percentage
                            {
                                DropItemID(killer, 722136, stream);
                                killer.SendSysMesage("An ExpBall (Event) has dropped! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                            if (Role.Core.Rate(1, 2000)) // Lucky Percentage
                            {
                                DropItemID(killer, 1088000, stream);
                                Game.MsgTournaments.MsgSchedules.SendSysMesage("" + killer.Player.Name + " has dropped a DragonBall!", Game.MsgServer.MsgMessage.ChatMode.World, Game.MsgServer.MsgMessage.MsgColor.yellow);
                            }
                        }

                    if (Boss == 0 && Map != 10133 && Map != 3071 && Map != 10741)
                        if (killer.Player.OnAutoHunt == true && killer.Player.VipLevel == 1)
                        {



                            if (Role.Core.Rate(1, 2)) // Lucky Percentage
                            {
                                killer.Player.ConquerPoints += 1;
                            }
                           
                            if (Role.Core.Rate(1, 100)) // Lucky Percentage
                            {
                                DropItemID(killer, 722136, stream);
                                killer.SendSysMesage("An ExpBall (Event) has dropped! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                            if (Role.Core.Rate(1, 2000)) // Lucky Percentage
                            {
                                DropItemID(killer, 1088000, stream);
                                Game.MsgTournaments.MsgSchedules.SendSysMesage("" + killer.Player.Name + " has dropped a DragonBall!", Game.MsgServer.MsgMessage.ChatMode.World, Game.MsgServer.MsgMessage.MsgColor.yellow);
                            }
                        }


                        else if (killer.Player.VipLevel == 1)
                        {



                            if (Role.Core.Rate(1, 2)) // Lucky Percentage
                            {
                                killer.Player.ConquerPoints += 1;
                            }
                           
                            if (Role.Core.Rate(1, 100)) // Lucky Percentage
                            {
                                DropItemID(killer, 722136, stream);
                                killer.SendSysMesage("An ExpBall (Event) has dropped! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                            if (Role.Core.Rate(1, 2000)) // Lucky Percentage
                            {
                                DropItemID(killer, 1088000, stream);
                                Game.MsgTournaments.MsgSchedules.SendSysMesage("" + killer.Player.Name + " has dropped a DragonBall!", Game.MsgServer.MsgMessage.ChatMode.World, Game.MsgServer.MsgMessage.MsgColor.yellow);
                            }
                        }

                    if (Boss == 0 && Map != 10133 && Map != 3071 && Map != 10741)
                        if (killer.Player.OnAutoHunt == true && killer.Player.VipLevel == 2)
                        {



                            if (Role.Core.Rate(1, 2)) // Lucky Percentage
                            {
                                killer.Player.ConquerPoints += 1;
                            }
                            
                            if (Role.Core.Rate(1, 100)) // Lucky Percentage
                            {
                                DropItemID(killer, 722136, stream);
                                killer.SendSysMesage("An ExpBall (Event) has dropped! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                            if (Role.Core.Rate(1, 2000)) // Lucky Percentage
                            {
                                DropItemID(killer, 1088000, stream);
                                Game.MsgTournaments.MsgSchedules.SendSysMesage("" + killer.Player.Name + " has dropped a DragonBall!", Game.MsgServer.MsgMessage.ChatMode.World, Game.MsgServer.MsgMessage.MsgColor.yellow);
                            }
                        }


                        else if (killer.Player.VipLevel == 2)
                        {



                            if (Role.Core.Rate(1, 2)) // Lucky Percentage
                            {
                                killer.Player.ConquerPoints += 1;
                            }
                            
                            if (Role.Core.Rate(1, 100)) // Lucky Percentage
                            {
                                DropItemID(killer, 722136, stream);
                                killer.SendSysMesage("An ExpBall (Event) has dropped! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                            if (Role.Core.Rate(1, 2000)) // Lucky Percentage
                            {
                                DropItemID(killer, 1088000, stream);
                                Game.MsgTournaments.MsgSchedules.SendSysMesage("" + killer.Player.Name + " has dropped a DragonBall!", Game.MsgServer.MsgMessage.ChatMode.World, Game.MsgServer.MsgMessage.MsgColor.yellow);
                            }
                        }

                    if (Boss == 0 && Map != 10133 && Map != 3071 && Map != 10741)
                        if (killer.Player.OnAutoHunt == true && killer.Player.VipLevel == 3)
                        {



                            if (Role.Core.Rate(1, 2)) // Lucky Percentage
                            {
                                killer.Player.ConquerPoints += 1;
                            }
                            
                            if (Role.Core.Rate(1, 100)) // Lucky Percentage
                            {
                                DropItemID(killer, 722136, stream);
                                killer.SendSysMesage("An ExpBall (Event) has dropped! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                            if (Role.Core.Rate(1, 2000)) // Lucky Percentage
                            {
                                DropItemID(killer, 1088000, stream);
                                Game.MsgTournaments.MsgSchedules.SendSysMesage("" + killer.Player.Name + " has dropped a DragonBall!", Game.MsgServer.MsgMessage.ChatMode.World, Game.MsgServer.MsgMessage.MsgColor.yellow);
                            }
                        }


                        else if (killer.Player.VipLevel == 3)
                        {



                            if (Role.Core.Rate(1, 2)) // Lucky Percentage
                            {
                                killer.Player.ConquerPoints += 1;
                            }
                            
                            if (Role.Core.Rate(1, 100)) // Lucky Percentage
                            {
                                DropItemID(killer, 722136, stream);
                                killer.SendSysMesage("An ExpBall (Event) has dropped! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                            if (Role.Core.Rate(1, 2000)) // Lucky Percentage
                            {
                                DropItemID(killer, 1088000, stream);
                                Game.MsgTournaments.MsgSchedules.SendSysMesage("" + killer.Player.Name + " has dropped a DragonBall!", Game.MsgServer.MsgMessage.ChatMode.World, Game.MsgServer.MsgMessage.MsgColor.yellow);
                            }
                        }

                    if (Boss == 0 && Map != 10133 && Map != 3071 && Map != 10741)
                        if (killer.Player.OnAutoHunt == true && killer.Player.VipLevel == 4)
                        {

                            
                                killer.Player.ConquerPoints += 1;

                            if (Role.Core.Rate(1, 100)) // Lucky Percentage
                            {
                                DropItemID(killer, 722136, stream);
                                killer.SendSysMesage("An ExpBall (Event) has dropped! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                            if (Role.Core.Rate(1, 2000)) // Lucky Percentage
                            {
                                DropItemID(killer, 1088000, stream);
                                Game.MsgTournaments.MsgSchedules.SendSysMesage("" + killer.Player.Name + " has dropped a DragonBall!", Game.MsgServer.MsgMessage.ChatMode.World, Game.MsgServer.MsgMessage.MsgColor.yellow);
                            }
                        }
                        else if (killer.Player.VipLevel == 4)
                        {

                            killer.Player.ConquerPoints += 1;

                            if (Role.Core.Rate(1, 100)) // Lucky Percentage
                            {
                                DropItemID(killer, 722136, stream);
                                killer.SendSysMesage("An ExpBall (Event) has dropped! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                            if (Role.Core.Rate(1, 2000)) // Lucky Percentage
                            {
                                DropItemID(killer, 1088000, stream);
                                Game.MsgTournaments.MsgSchedules.SendSysMesage("" + killer.Player.Name + " has dropped a DragonBall!", Game.MsgServer.MsgMessage.ChatMode.World, Game.MsgServer.MsgMessage.MsgColor.yellow);
                            }
                        }

                    if (Boss == 0 && Map != 10133 && Map != 3071 && Map != 10741)
                        if (killer.Player.OnAutoHunt == true && killer.Player.VipLevel == 5)
                        {


                            killer.Player.ConquerPoints += 1;

                            if (Role.Core.Rate(1, 100)) // Lucky Percentage
                            {
                                DropItemID(killer, 722136, stream);
                                killer.SendSysMesage("An ExpBall (Event) has dropped! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                            if (Role.Core.Rate(1, 2000)) // Lucky Percentage
                            {
                                DropItemID(killer, 1088000, stream);
                                Game.MsgTournaments.MsgSchedules.SendSysMesage("" + killer.Player.Name + " has dropped a DragonBall!", Game.MsgServer.MsgMessage.ChatMode.World, Game.MsgServer.MsgMessage.MsgColor.yellow);
                            }
                        }


                        else if (killer.Player.VipLevel == 5)
                        {


                            killer.Player.ConquerPoints += 1;

                            if (Role.Core.Rate(1, 100)) // Lucky Percentage
                            {
                                DropItemID(killer, 722136, stream);
                                killer.SendSysMesage("An ExpBall (Event) has dropped! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                            if (Role.Core.Rate(1, 2000)) // Lucky Percentage
                            {
                                DropItemID(killer, 1088000, stream);
                                Game.MsgTournaments.MsgSchedules.SendSysMesage("" + killer.Player.Name + " has dropped a DragonBall!", Game.MsgServer.MsgMessage.ChatMode.World, Game.MsgServer.MsgMessage.MsgColor.yellow);
                            }
                        }

                    if (Boss == 0 && Map != 10133 && Map != 3071 && Map != 10741)
                        if (killer.Player.OnAutoHunt == true && killer.Player.VipLevel == 6)
                        {

                            killer.Player.ConquerPoints += 2;
                      
                            if (Role.Core.Rate(1, 100)) // Lucky Percentage
                            {
                                killer.Inventory.AddItemWitchStack(722136, 0, 1, stream);
                                killer.SendSysMesage("You received x1 ExpBall (Event) In Your Inventory! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                            if (Role.Core.Rate(1, 2000)) // Lucky Percentage
                            {
                                killer.Player.ConquerPoints += 215;
                                Game.MsgTournaments.MsgSchedules.SendSysMesage("" + killer.Player.Name + " has dropped a DragonBall and received 215 CPs!", Game.MsgServer.MsgMessage.ChatMode.World, Game.MsgServer.MsgMessage.MsgColor.yellow);
                            }
                        }


                        else if (killer.Player.VipLevel == 6)
                        {

                            killer.Player.ConquerPoints += 2;

                            if (Role.Core.Rate(1, 100)) // Lucky Percentage
                            {
                                killer.Inventory.AddItemWitchStack(722136, 0, 1, stream);
                                killer.SendSysMesage("You received x1 ExpBall (Event) In Your Inventory! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                            if (Role.Core.Rate(1, 2000)) // Lucky Percentage
                            {
                                killer.Player.ConquerPoints += 215;
                                Game.MsgTournaments.MsgSchedules.SendSysMesage("" + killer.Player.Name + " has dropped a DragonBall and received 215 CPs!", Game.MsgServer.MsgMessage.ChatMode.World, Game.MsgServer.MsgMessage.MsgColor.yellow);
                            }
                        }

                    if (Boss == 0 && Map != 10133 && Map != 3071 && Map != 10741)
                        if (killer.Player.OnAutoHunt == true && killer.Player.VipLevel == 7)
                        {


                            killer.Player.ConquerPoints += 2;

                            if (Role.Core.Rate(1, 100)) // Lucky Percentage
                            {
                                killer.Inventory.AddItemWitchStack(722136, 0, 1, stream);
                                killer.SendSysMesage("You received x1 ExpBall (Event) In Your Inventory! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                            if (Role.Core.Rate(1, 2000)) // Lucky Percentage
                            {
                                killer.Player.ConquerPoints += 215;
                                Game.MsgTournaments.MsgSchedules.SendSysMesage("" + killer.Player.Name + " has dropped a DragonBall and received 215 CPs!", Game.MsgServer.MsgMessage.ChatMode.World, Game.MsgServer.MsgMessage.MsgColor.yellow);
                            }
                        }


                        else if (killer.Player.VipLevel == 7)
                        {

                            killer.Player.ConquerPoints += 2;

                            if (Role.Core.Rate(1, 100)) // Lucky Percentage
                            {
                                killer.Inventory.AddItemWitchStack(722136, 0, 1, stream);
                                killer.SendSysMesage("You received x1 ExpBall (Event) In Your Inventory! ", MsgMessage.ChatMode.System, MsgMessage.MsgColor.red);
                            }
                            if (Role.Core.Rate(1, 2000)) // Lucky Percentage
                            {
                                killer.Player.ConquerPoints += 215;
                                Game.MsgTournaments.MsgSchedules.SendSysMesage("" + killer.Player.Name + " has dropped a DragonBall and received 215 CPs!", Game.MsgServer.MsgMessage.ChatMode.World, Game.MsgServer.MsgMessage.MsgColor.yellow);
                            }
                        }

                    #endregion


                    #region SquidwardOctopus
                    if (killer.Player.Map == 3071)
                    {

                        if (Family.ID == 7022) // CoinsStealer
                        {
                            if (!killer.Inventory.HaveSpace(1))
                            {
                                killer.CreateBoxDialog("Your inventory is full!");
                                return;
                            }

                            if (Role.Core.Rate(1, 100))
                            {
                                killer.Inventory.Add(stream, 711609);

                                killer.Player.SendString(stream,
                                    MsgStringPacket.StringID.Effect,
                                    false,
                                    "dispel4");

                                killer.SendSysMesage(
                                    "You received a Gold Coin!",
                                    MsgMessage.ChatMode.System);
                            }
                            else if (Role.Core.Rate(2, 100))
                            {
                                killer.Inventory.Add(stream, 711610);

                                killer.Player.SendString(stream,
                                    MsgStringPacket.StringID.Effect,
                                    false,
                                    "dispel7");

                                killer.SendSysMesage(
                                    "You received a Silver Coin!",
                                    MsgMessage.ChatMode.System);
                            }
                            else if (Role.Core.Rate(5, 100))
                            {
                                killer.Inventory.Add(stream, 711611);

                                killer.Player.SendString(stream,
                                    MsgStringPacket.StringID.Effect,
                                    false,
                                    "dispel5");

                                killer.SendSysMesage(
                                    "You received a Copper Coin!",
                                    MsgMessage.ChatMode.System);
                            }
                        }

                        if (Family.ID == 2699) // SilverOctopus
                        {
                            if (!killer.Inventory.HaveSpace(1))
                            {
                                killer.CreateBoxDialog("Your inventory is full!");
                                return;
                            }

                            killer.Inventory.Add(stream, 711610);

                            killer.Player.SendString(stream,
                                MsgStringPacket.StringID.Effect,
                                false,
                                "dispel7");

                            killer.SendSysMesage(
                                "You received a Silver Coin!",
                                MsgMessage.ChatMode.System);
                        }
                    }

                    #endregion

                    #region Golden Octopus
                    if (killer.Player.Map == 3071)
                    {

                        if (Family.ID == 2700)
                        {
                            Database.ServerDatabase.LoginQueue.Enqueue(
                                "[Check Monsters] " +
                                "Monster Name : [Golden Octopus] " +
                                "Monster ID : [2700] " +
                                "Player Name : [ " + killer.Player.Name + " ] " +
                                "Player UID : [ " + killer.Player.UID + " ] " +
                                "Map : [ " + Map + " ] " +
                                "X : [ " + X + " ] " +
                                "Y : [ " + Y + " ] " +
                                "Dynamic ID : [ " + killer.Player.DynamicID + " ] " +
                                "Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
                            );

                            MsgTournaments.MsgSchedules.SendSysMesage(
                                "[ " + killer.Player.Name + " ] killed Big Octopus and x5 Gold Coins dropped on the ground!",
                                MsgServer.MsgMessage.ChatMode.Center,
                                MsgServer.MsgMessage.MsgColor.red);

                            Hunters.Clear();

                            if (killer.Player.DynamicID != killer.Player.UID)
                            {
                                for (int x = 1; x < 6; x++)
                                {
                                    uint id = 711609;

                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = id;

                                    Database.ItemType.DBItem DBItem;
                                    if (Pool.ItemsBase.TryGetValue(id, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }

                                    DataItem.Color = Role.Flags.Color.Red;

                                    ushort xx = (ushort)Program.GetRandom.Next(X - 5, X + 5);
                                    ushort yy = (ushort)Program.GetRandom.Next(Y - 4, Y + 4);

                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(
                                            DataItem,
                                            xx,
                                            yy,
                                            MsgFloorItem.MsgItem.ItemType.Item,
                                            0,
                                            DynamicID,
                                            Map,
                                            killer.Player.UID,
                                            true,
                                            GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {
                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    #endregion


                    #region CleansingDevil Monster Flags War
                    if (Family.ID == 3635)
                    {
                        if (Role.Core.Rate(1, 15))
                        {
                            killer.Player.FlagsPointss += 1;
                            //Game.MsgTournaments.MsgSchedules.SendSysMesage("[ " + killer.Player.Name + " ] killed Cleansing Devil and got 1 Pts in [Flags War].", MsgServer.MsgMessage.ChatMode.System, MsgServer.MsgMessage.MsgColor.red);
                            killer.SendSysMesage("You killed Cleansing Devil and got 1 Pts.");
                        }

                    }
                    #endregion


                    #region Shadow Castle

                    if (killer.Player.Map == 10133)
                    {

                        if (Role.Core.Rate(1, 50))
                        {
                            int reward = Pool.GetRandom.Next(0, 35);

                            switch (reward)
                            {
                                case 0:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(730003, 0, 1, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                "[ " + killer.Player.Name + " ] Got Stone +3 From Shadow Castle Event!",
                                MsgServer.MsgMessage.ChatMode.World,
                                MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 1:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(730004, 0, 1, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got Stone +4 From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 2:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(730003, 0, 1, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got Stone +3 From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 3:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(730004, 0, 1, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                "[ " + killer.Player.Name + " ] Got Stone +4 From Shadow Castle Event!",
                                MsgServer.MsgMessage.ChatMode.World,
                                MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 4:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(730003, 0, 1, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got Stone +3 From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 5:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(3314217, 0, 1, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got Steed +4 From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 6:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(3314216, 0, 1, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                  "[ " + killer.Player.Name + " ] Got Steed +3 From Shadow Castle Event!",
                                  MsgServer.MsgMessage.ChatMode.World,
                                  MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 7:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(3314216, 0, 1, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                  "[ " + killer.Player.Name + " ] Got Steed +3 From Shadow Castle Event!",
                                  MsgServer.MsgMessage.ChatMode.World,
                                  MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 8:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(3314217, 0, 1, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got Steed +4 From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 9:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(3314216, 0, 1, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got Steed +3 From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 10:
                                    {
                                        killer.Inventory.AddItemWitchStack(722136, 0, 5, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got x5 ExpBall(Event) From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 11:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Player.ConquerPoints += 100;

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                  "[ " + killer.Player.Name + " ] Got 100 CPs From Shadow Castle Event!",
                                  MsgServer.MsgMessage.ChatMode.World,
                                  MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 12:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Player.ConquerPoints += 200;

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got 200 CPs From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 13:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Player.ConquerPoints += 300;

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                  "[ " + killer.Player.Name + " ] Got 300 CPs From Shadow Castle Event!",
                                  MsgServer.MsgMessage.ChatMode.World,
                                  MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 14:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Player.ConquerPoints += 400;

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got 400 CPs From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 15:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Player.ConquerPoints += 500;

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got 500 CPs From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 16:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Player.ConquerPoints += 100;

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got 100 CPs From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 17:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Player.ConquerPoints += 150;

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got 150 CPs From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 18:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Player.Money += 1000000;

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got 1.000.000 Money From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 19:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Player.Money += 2000000;

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got 2.000.000 Money From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 20:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Player.Money += 3000000;

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got 3.000.000 Money From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 21:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Player.Money += 4000000;

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got 4.000.000 Money From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 22:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Player.Money += 5000000;

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                  "[ " + killer.Player.Name + " ] Got 5.000.000 Money From Shadow Castle Event!",
                                  MsgServer.MsgMessage.ChatMode.World,
                                  MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 23:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(722136, 0, 10, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got x10 ExpBall(Event) From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 24:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(722136, 0, 10, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got x10 ExpBall(Event) From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 25:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(722136, 0, 10, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got x10 ExpBall(Event) From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 26:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(722136, 0, 10, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got x10 ExpBall(Event) From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 27:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(722136, 0, 10, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got x10 ExpBall(Event) From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 28:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(729991, 0, 1, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got x1 Random[Scraps] From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 29:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(4200001, 0, 1, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got x1 P1Anima From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 30:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(4200002, 0, 1, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got x1 P2Anima From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 31:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(4200003, 0, 1, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got x1 P3Anima From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 32:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(3306366, 0, 1, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got x1 Red Rune Fragment From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 33:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(3306367, 0, 1, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got x1 Yellow Rune Fragment From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }

                                case 34:
                                    {
                                        if (!killer.Inventory.HaveSpace(1))
                                        {
                                            killer.CreateBoxDialog("Please make 1 more spaces in your inventory.");
                                            break;
                                        }

                                        killer.Inventory.AddItemWitchStack(3306368, 0, 1, stream, false);

                                        MsgTournaments.MsgSchedules.SendSysMesage(
                                 "[ " + killer.Player.Name + " ] Got x1 Blue Rune Fragment From Shadow Castle Event!",
                                 MsgServer.MsgMessage.ChatMode.World,
                                 MsgServer.MsgMessage.MsgColor.yellow);

                                        break;
                                    }
                            }
                        }
                    }

                    #endregion

                    #region Lucky Golden Boxes

                    if (Map == 10741) 
                    {
                        byte rand = (byte)Pool.GetRandom.Next(1, 43);
                        switch (rand)
                        {

                            case 1:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    killer.Player.ConquerPoints += 100;
                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (100 CPs)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }
                            case 2:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    killer.Player.ConquerPoints += 150;
                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (150 CPs)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }
                            case 3:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    killer.Player.ConquerPoints += 100;
                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (100 CPs)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }
                            case 4:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        break;
                                    }
                                    killer.Player.ConquerPoints += 200;
                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (200 CPs)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }
                            case 5:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    killer.Player.ConquerPoints += 150;
                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (150 CPs)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }
                            case 6:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    killer.Player.Money += 2500000;

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (2.500.000 Money)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 7:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    killer.Player.Money += 5000000;

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (5.000.000 Money)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 8:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    killer.Player.Money += 7500000;

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (7.500.000 Money)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 9:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    killer.Player.Money += 10000000;

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (10.000.000 Money)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 10:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    killer.Player.Money += 5000000;

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (5.000.000 Money)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }
                            case 11:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }

                                    killer.Inventory.AddItemWitchStack(730004, 0, 1, stream, false);

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (Stone +4)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 12:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }

                                    killer.Inventory.AddItemWitchStack(730003, 0, 1, stream, false);

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (Stone +3)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 13:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }

                                    killer.Inventory.AddItemWitchStack(730004, 0, 1, stream, false);

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (Stone +4)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 14:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }

                                    killer.Inventory.AddItemWitchStack(730003, 0, 1, stream, false);

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (Stone +3)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 15:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }

                                    killer.Inventory.AddItemWitchStack(3300055, 0, 1, stream, false);

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (HolyPhoenixCupFragment)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }
                            case 16:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }

                                    killer.Inventory.AddItemWitchStack(723694, 0, 1, stream, false);

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (PermanentStone)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 17:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }

                                    killer.Inventory.AddItemWitchStack(3005598, 0, 1, stream, false);

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (LuckyGarmentPack)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 18:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }

                                    killer.Inventory.AddItemWitchStack(728722, 0, 1, stream, false);

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received Infinity[Box]",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 19:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }

                                    killer.Inventory.AddItemWitchStack(728722, 0, 1, stream, false);

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received Infinity[Box]",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 20:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }

                                    killer.Inventory.AddItemWitchStack(3005598, 0, 1, stream, false);

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (LuckyGarmentPack)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }
                            case 21:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }

                                    killer.Inventory.AddItemWitchStack(1200005, 0, 1, stream, false);

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (ToughDrill)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 22:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }

                                    killer.Inventory.AddItemWitchStack(1088000, 0, 1, stream, false);

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (DragonBall)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 23:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }

                                    killer.Inventory.AddItemWitchStack(1088000, 0, 1, stream, false);

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (DragonBall)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 24:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    killer.Player.ConquerPoints += 100;

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (100 CPs)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 25:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    killer.Player.ConquerPoints += 150;

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (150 CPs)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }
                            case 26:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    killer.Player.ConquerPoints += 200;

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (200 CPs)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 27:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    killer.Player.ConquerPoints += 250;

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (250 CPs)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 28:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    killer.Player.ConquerPoints += 300;

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (300 CPs)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 29:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    killer.Player.ConquerPoints += 100;

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (100 CPs)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 30:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    killer.Player.ConquerPoints += 200;

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (200 CPs)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }
                            case 31:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }

                                    killer.Inventory.AddSteed(stream, 300000, 1, 4, false, 255, 0, 150);

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (Steed +4)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 32:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }

                                    killer.Inventory.AddSteed(stream, 300000, 1, 3, false, 255, 0, 150);

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (Steed +3)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 33:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    killer.Player.ConquerPoints += 50;

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (50 CPs)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 34:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    killer.Player.ConquerPoints += 100;

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (100 CPs)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 35:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box but received nothing. Good luck next time.",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }
                            case 36:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box but received nothing. Good luck next time.",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 37:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }

                                    killer.Inventory.AddItemWitchStack(722136, 0, 10, stream, false);

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (10 ExpBall Event)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 38:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }

                                    killer.Inventory.AddItemWitchStack(722136, 0, 5, stream, false);

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (5 ExpBall Event)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }

                            case 39:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }

                                    killer.Inventory.AddItemWitchStack(722136, 0, 10, stream, false);

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received (10 ExpBall Event)",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }
                            case 40:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box but received nothing. Good luck next time.",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }
                            case 41:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }
                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box but received nothing. Good luck next time.",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }
                            case 42:
                                {
                                    if (!killer.Inventory.HaveSpace(1))
                                    {
                                        killer.CreateBoxDialog("Please make 1 more space in your inventory.");
                                        return;
                                    }

                                    killer.Inventory.AddItemWitchStack(729991, 0, 1, stream, false);

                                    Game.MsgTournaments.MsgSchedules.SendSysMesage(
                                        "[ " + killer.Player.Name + " ] killed Lucky Golden Box and received x1 Random[Scraps]",
                                        MsgServer.MsgMessage.ChatMode.World,
                                        MsgServer.MsgMessage.MsgColor.yellow);
                                    break;
                                }
                        }
                    }

                    #endregion


    //                #region Godzilla Monster ( CP Castle )
    //                if (Family.ID == 3636)
    //                {
    //                    SendSysMesage(killer.Player.Name + " has defeated Godzilla Monster", MsgServer.MsgMessage.ChatMode.World, MsgServer.MsgMessage.MsgColor.red);

    //                    Database.ServerDatabase.LoginQueue.Enqueue(
    //"[Check Monsters] " +
    //"Monster Name : [Godzilla Monster] " +
    //"Monster ID : [3636] " +
    //"Player Name : [ " + killer.Player.Name + " ] " +
    //"Player UID : [ " + killer.Player.UID + " ] " +
    //"Map : [ " + Map + " ] " +
    //"X : [ " + X + " ] " +
    //"Y : [ " + Y + " ] " +
    //"Dynamic ID : [ " + killer.Player.DynamicID + " ] " +
    //"Date : [ " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ]"
    //);

    //                    Hunters.Clear();
    //                    #region Dropitems
    //                    if (killer.Player.DynamicID != killer.Player.UID)
    //                    {
    //                        for (int x = 1; x < 41; x++)
    //                        {
    //                            uint id = 0;

    //                            if (x == 1) id = 1088000;   // Dragon Ball
    //                            if (x == 2) id = 1088000;   // Dragon Ball
    //                            if (x == 3) id = 1088000;   // Dragon Ball
    //                            if (x == 4) id = 1088000;   // Dragon Ball
    //                            if (x == 5) id = 1088000;   // Dragon Ball
    //                            if (x == 6) id = 1088000;   // Dragon Ball
    //                            if (x == 7) id = 1088000;   // Dragon Ball
    //                            if (x == 8) id = 1088000;   // Dragon Ball
    //                            if (x == 9) id = 1088000;   // Dragon Ball
    //                            if (x == 10) id = 1088000;  // Dragon Ball

    //                            if (x == 11) id = 720659;  // SweetCPPack
    //                            if (x == 12) id = 720659;  // SweetCPPack
    //                            if (x == 13) id = 720659;  // SweetCPPack
    //                            if (x == 14) id = 720659;  // SweetCPPack
    //                            if (x == 15) id = 720659;  // SweetCPPack
    //                            if (x == 16) id = 720659;  // SweetCPPack
    //                            if (x == 17) id = 720659;  // SweetCPPack
    //                            if (x == 18) id = 720659;  // SweetCPPack
    //                            if (x == 19) id = 720659;  // SweetCPPack
    //                            if (x == 20) id = 720659;  // SweetCPPack
    //                            if (x == 21) id = 720659;  // SweetCPPack
    //                            if (x == 22) id = 720659;  // SweetCPPack
    //                            if (x == 23) id = 720659;  // SweetCPPack
    //                            if (x == 24) id = 720659;  // SweetCPPack
    //                            if (x == 25) id = 720659;  // SweetCPPack
    //                            if (x == 26) id = 720659;  // SweetCPPack
    //                            if (x == 27) id = 720659;  // SweetCPPack
    //                            if (x == 28) id = 720659;  // SweetCPPack
    //                            if (x == 29) id = 720659;  // SweetCPPack
    //                            if (x == 30) id = 720659;  // SweetCPPack

    //                            if (x == 31) id = 721028;  // CyanCPBag
    //                            if (x == 32) id = 721028;  // CyanCPBag
    //                            if (x == 33) id = 721028;  // CyanCPBag
    //                            if (x == 34) id = 721028;  // CyanCPBag
    //                            if (x == 35) id = 721028;  // CyanCPBag
    //                            if (x == 36) id = 721028;  // CyanCPBag
    //                            if (x == 37) id = 721028;  // CyanCPBag
    //                            if (x == 38) id = 721028;  // CyanCPBag
    //                            if (x == 39) id = 721028;  // CyanCPBag
    //                            if (x == 40) id = 721028;  // CyanCPBag

    //                            if (id == 0)
    //                                continue;


    //                            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
    //                            DataItem.ITEM_ID = id;
    //                            Database.ItemType.DBItem DBItem;
    //                            if (Pool.ItemsBase.TryGetValue(id, out DBItem))
    //                            {
    //                                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
    //                            }
    //                            DataItem.Color = Role.Flags.Color.Red;
    //                            ushort xx = (ushort)Program.GetRandom.Next(X - 10, X + 10);
    //                            ushort yy = (ushort)Program.GetRandom.Next(Y - 10, Y + 10);
    //                            if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
    //                            {
    //                                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

    //                                if (killer.Map.EnqueueItem(DropItem))
    //                                {

    //                                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
    //                                }
    //                            }
    //                        }
    //                    }
    //                    #endregion
    //                    return;
    //                }
    //                #endregion

                    #region DragonIsland

                    if (Map == 10137)
                    {
                        #region NemesisTyrant
                        if (Family.ID == 4220)
                        {
                            GetBossReward(killer, stream);
                            SendSysMesage(killer.Player.Name + " has defeated [Nemesis Tyrant] ", MsgServer.MsgMessage.ChatMode.World, MsgServer.MsgMessage.MsgColor.yellow);
                            Hunters.Clear();

                            #region Dropitems
                            if (killer.Player.DynamicID != killer.Player.UID)
                            {
                                for (int x = 1; x < 20; x++)
                                {
                                    uint id = 0;
                                    if (x == 1) id = 730004; // Stone(+4)
                                    if (x == 2) id = 730004; // Stone(+4)

                                    if (x == 3) id = 3004181; // Savage Bone
                                    if (x == 4) id = 3004181; // Savage Bone
                                    if (x == 5) id = 3004181; // Savage Bone

                                    if (x == 6) id = 723717; // Class5MoneyBag
                                    if (x == 7) id = 723717; // Class5MoneyBag
                                    if (x == 8) id = 723717; // Class5MoneyBag
                                    if (x == 9) id = 723717; // Class5MoneyBag
                                    if (x == 10) id = 723717; // Class5MoneyBag

                                    if (x == 11) id = 729991; // Random[Scraps]
                                    if (x == 12) id = 729991; // Random[Scraps]

                                    if (x == 13) id = 3315885; // 1000SacredClassEXPPack (Promotion)
                                    if (x == 14) id = 3315885; // 1000SacredClassEXPPack (Promotion)

                                    if (x == 15) id = 722057; // PowerEXPBall

                                    if (x == 16) id = 2169805; // P1 Jade Kylin

                                    if (x == 17) id = 3009002; // RadiantStarStone
                                    if (x == 18) id = 3009002; // RadiantStarStone
                                    if (x == 19) id = 3009002; // RadiantStarStone

                                    if (id == 0)
                                        continue;

                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = id;
                                    Database.ItemType.DBItem DBItem;
                                    if (Pool.ItemsBase.TryGetValue(id, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = (ushort)Program.GetRandom.Next(X - 5, X + 5);
                                    ushort yy = (ushort)Program.GetRandom.Next(Y - 4, Y + 4);
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {

                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }


                                    }
                                }
                            }
                            #endregion

                            return;
                        }
                        #endregion

                        #region Snow Banshee
                        if (Family.ID == 4171)
                        {
                            GetBossReward(killer, stream);
                            SendSysMesage(killer.Player.Name + " has defeated [Snow Banshee] ", MsgServer.MsgMessage.ChatMode.World, MsgServer.MsgMessage.MsgColor.yellow);
                            Hunters.Clear();

                            #region Dropitems
                            if (killer.Player.DynamicID != killer.Player.UID)
                            {
                                for (int x = 1; x < 20; x++)
                                {
                                    uint id = 0;
                                    if (x == 1) id = 730004; // Stone(+4)
                                    if (x == 2) id = 730004; // Stone(+4)

                                    if (x == 3) id = 3004181; // Savage Bone
                                    if (x == 4) id = 3004181; // Savage Bone
                                    if (x == 5) id = 3004181; // Savage Bone

                                    if (x == 6) id = 723717; // Class5MoneyBag
                                    if (x == 7) id = 723717; // Class5MoneyBag
                                    if (x == 8) id = 723717; // Class5MoneyBag
                                    if (x == 9) id = 723717; // Class5MoneyBag
                                    if (x == 10) id = 723717; // Class5MoneyBag

                                    if (x == 11) id = 729991; // Random[Scraps]
                                    if (x == 12) id = 729991; // Random[Scraps]

                                    if (x == 13) id = 3315885; // 1000SacredClassEXPPack (Promotion)
                                    if (x == 14) id = 3315885; // 1000SacredClassEXPPack (Promotion)

                                    if (x == 15) id = 722057; // PowerEXPBall

                                    if (x == 16) id = 2169805; // P1 Jade Kylin

                                    if (x == 17) id = 3009002; // RadiantStarStone
                                    if (x == 18) id = 3009002; // RadiantStarStone
                                    if (x == 19) id = 3009002; // RadiantStarStone

                                    if (id == 0)
                                        continue;




                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = id;
                                    Database.ItemType.DBItem DBItem;
                                    if (Pool.ItemsBase.TryGetValue(id, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = (ushort)Program.GetRandom.Next(X - 5, X + 5);
                                    ushort yy = (ushort)Program.GetRandom.Next(Y - 4, Y + 4);
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {

                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            #endregion

                            return;
                        }
                        #endregion


                        #region ThrillingSpook
                        if (Family.ID == 4212)
                        {
                            GetBossReward(killer, stream);
                            SendSysMesage(killer.Player.Name + " has defeated [Thrilling Spook] ", MsgServer.MsgMessage.ChatMode.World, MsgServer.MsgMessage.MsgColor.yellow);
                            Hunters.Clear();

                            #region Dropitems
                            if (killer.Player.DynamicID != killer.Player.UID)
                            {
                                for (int x = 1; x < 20; x++)
                                {
                                    uint id = 0;
                                    if (x == 1) id = 730004; // Stone(+4)
                                    if (x == 2) id = 730004; // Stone(+4)

                                    if (x == 3) id = 3004181; // Savage Bone
                                    if (x == 4) id = 3004181; // Savage Bone
                                    if (x == 5) id = 3004181; // Savage Bone

                                    if (x == 6) id = 723717; // Class5MoneyBag
                                    if (x == 7) id = 723717; // Class5MoneyBag
                                    if (x == 8) id = 723717; // Class5MoneyBag
                                    if (x == 9) id = 723717; // Class5MoneyBag
                                    if (x == 10) id = 723717; // Class5MoneyBag

                                    if (x == 11) id = 729991; // Random[Scraps]
                                    if (x == 12) id = 729991; // Random[Scraps]

                                    if (x == 13) id = 3315885; // 1000SacredClassEXPPack (Promotion)
                                    if (x == 14) id = 3315885; // 1000SacredClassEXPPack (Promotion)

                                    if (x == 15) id = 722057; // PowerEXPBall

                                    if (x == 16) id = 2169805; // P1 Jade Kylin

                                    if (x == 17) id = 3009002; // RadiantStarStone
                                    if (x == 18) id = 3009002; // RadiantStarStone
                                    if (x == 19) id = 3009002; // RadiantStarStone

                                    if (id == 0)
                                        continue;




                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = id;
                                    Database.ItemType.DBItem DBItem;
                                    if (Pool.ItemsBase.TryGetValue(id, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = (ushort)Program.GetRandom.Next(X - 5, X + 5);
                                    ushort yy = (ushort)Program.GetRandom.Next(Y - 4, Y + 4);
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {

                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            #endregion

                            return;
                        }
                        #endregion

                    }

                    #endregion
                    #region DeityLand
                    if (killer.Player.Map == 10250)
                    {
                        #region QueenofEvil
                        if (Family.ID == 3970)
                        {
                            GetBossReward(killer, stream);
                            SendSysMesage(killer.Player.Name + " has defeated [Queen of Evil] ", MsgServer.MsgMessage.ChatMode.World, MsgServer.MsgMessage.MsgColor.yellow);
                            Hunters.Clear();

                            #region Dropitems
                            if (killer.Player.DynamicID != killer.Player.UID)
                            {
                                for (int x = 1; x < 49; x++)
                                {
                                    uint id = 0;

                                    if (x == 1) id = 730004; // Stone(+4)
                                    if (x == 2) id = 730004; // Stone(+4)
                                    if (x == 3) id = 730004; // Stone(+4)
                                    if (x == 4) id = 730004; // Stone(+4)
                                    if (x == 5) id = 730004; // Stone(+4)

                                    if (x == 6) id = 3004181; // Savage Bone
                                    if (x == 7) id = 3004181; // Savage Bone
                                    if (x == 8) id = 3004181; // Savage Bone
                                    if (x == 9) id = 3004181; // Savage Bone
                                    if (x == 10) id = 3004181; // Savage Bone
                                    if (x == 11) id = 3004181; // Savage Bone
                                    if (x == 12) id = 3004181; // Savage Bone
                                    if (x == 13) id = 3004181; // Savage Bone
                                    if (x == 14) id = 3004181; // Savage Bone
                                    if (x == 15) id = 3004181; // Savage Bone

                                    if (x == 16) id = 723717; // Class5MoneyBag
                                    if (x == 17) id = 723717; // Class5MoneyBag
                                    if (x == 18) id = 723717; // Class5MoneyBag
                                    if (x == 19) id = 723717; // Class5MoneyBag
                                    if (x == 20) id = 723717; // Class5MoneyBag
                                    if (x == 21) id = 723717; // Class5MoneyBag
                                    if (x == 22) id = 723717; // Class5MoneyBag
                                    if (x == 23) id = 723717; // Class5MoneyBag
                                    if (x == 24) id = 723717; // Class5MoneyBag
                                    if (x == 25) id = 723717; // Class5MoneyBag

                                    if (x == 26) id = 729991; // Random[Scraps]
                                    if (x == 27) id = 729991; // Random[Scraps]
                                    if (x == 28) id = 729991; // Random[Scraps]
                                    if (x == 29) id = 729991; // Random[Scraps]
                                    if (x == 30) id = 729991; // Random[Scraps]
                                    if (x == 31) id = 729991; // Random[Scraps]
                                    if (x == 32) id = 729991; // Random[Scraps]
                                    if (x == 33) id = 729991; // Random[Scraps]
                                    if (x == 34) id = 729991; // Random[Scraps]
                                    if (x == 35) id = 729991; // Random[Scraps]

                                    if (x == 36) id = 3315885; // 1000SacredClassEXPPack (Promotion)
                                    if (x == 37) id = 3315885; // 1000SacredClassEXPPack (Promotion)
                                    if (x == 38) id = 3315885; // 1000SacredClassEXPPack (Promotion)
                                    if (x == 39) id = 3315885; // 1000SacredClassEXPPack (Promotion)

                                    if (x == 40) id = 722057; // PowerEXPBall
                                    if (x == 41) id = 722057; // PowerEXPBall

                                    if (x == 42) id = 2169805; // P1 Jade Kylin
                                    if (x == 43) id = 2169805; // P1 Jade Kylin

                                    if (x == 44) id = 3009002; // RadiantStarStone
                                    if (x == 45) id = 3009002; // RadiantStarStone
                                    if (x == 46) id = 3009002; // RadiantStarStone
                                    if (x == 47) id = 3009002; // RadiantStarStone
                                    if (x == 48) id = 3009002; // RadiantStarStone

                                    if (id == 0)
                                        continue;

                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = id;
                                    Database.ItemType.DBItem DBItem;
                                    if (Pool.ItemsBase.TryGetValue(id, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = (ushort)Program.GetRandom.Next(X - 15, X + 15);
                                    ushort yy = (ushort)Program.GetRandom.Next(Y - 15, Y + 15);
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {

                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            #endregion

                            return;
                        }
                        #endregion

                        #region NetherTyrant
                        if (Family.ID == 3978)
                        {
                            GetBossReward(killer, stream);
                            SendSysMesage(killer.Player.Name + " has defeated [Nether Tyrant] ", MsgServer.MsgMessage.ChatMode.World, MsgServer.MsgMessage.MsgColor.yellow);
                            Hunters.Clear();

                            #region Dropitems
                            if (killer.Player.DynamicID != killer.Player.UID)
                            {
                                for (int x = 1; x < 20; x++)
                                {
                                    uint id = 0;
                                    if (x == 1) id = 730004; // Stone(+4)
                                    if (x == 2) id = 730004; // Stone(+4)

                                    if (x == 3) id = 3004181; // Savage Bone
                                    if (x == 4) id = 3004181; // Savage Bone
                                    if (x == 5) id = 3004181; // Savage Bone

                                    if (x == 6) id = 723717; // Class5MoneyBag
                                    if (x == 7) id = 723717; // Class5MoneyBag
                                    if (x == 8) id = 723717; // Class5MoneyBag
                                    if (x == 9) id = 723717; // Class5MoneyBag
                                    if (x == 10) id = 723717; // Class5MoneyBag

                                    if (x == 11) id = 729991; // Random[Scraps]
                                    if (x == 12) id = 729991; // Random[Scraps]

                                    if (x == 13) id = 3315885; // 1000SacredClassEXPPack (Promotion)
                                    if (x == 14) id = 3315885; // 1000SacredClassEXPPack (Promotion)

                                    if (x == 15) id = 722057; // PowerEXPBall

                                    if (x == 16) id = 2169805; // P1 Jade Kylin

                                    if (x == 17) id = 3009002; // RadiantStarStone
                                    if (x == 18) id = 3009002; // RadiantStarStone
                                    if (x == 19) id = 3009002; // RadiantStarStone

                                    if (id == 0)
                                        continue;




                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = id;
                                    Database.ItemType.DBItem DBItem;
                                    if (Pool.ItemsBase.TryGetValue(id, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = (ushort)Program.GetRandom.Next(X - 5, X + 5);
                                    ushort yy = (ushort)Program.GetRandom.Next(Y - 4, Y + 4);
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {

                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            #endregion

                            return;
                        }
                        #endregion


                        #region BloodyBanshee
                        if (Family.ID == 3976)
                        {
                            GetBossReward(killer, stream);
                            SendSysMesage(killer.Player.Name + " has defeated [Bloody Banshee] ", MsgServer.MsgMessage.ChatMode.World, MsgServer.MsgMessage.MsgColor.yellow);
                            Hunters.Clear();

                            #region Dropitems
                            if (killer.Player.DynamicID != killer.Player.UID)
                            {
                                for (int x = 1; x < 20; x++)
                                {
                                    uint id = 0;
                                    if (x == 1) id = 730004; // Stone(+4)
                                    if (x == 2) id = 730004; // Stone(+4)

                                    if (x == 3) id = 3004181; // Savage Bone
                                    if (x == 4) id = 3004181; // Savage Bone
                                    if (x == 5) id = 3004181; // Savage Bone

                                    if (x == 6) id = 723717; // Class5MoneyBag
                                    if (x == 7) id = 723717; // Class5MoneyBag
                                    if (x == 8) id = 723717; // Class5MoneyBag
                                    if (x == 9) id = 723717; // Class5MoneyBag
                                    if (x == 10) id = 723717; // Class5MoneyBag

                                    if (x == 11) id = 729991; // Random[Scraps]
                                    if (x == 12) id = 729991; // Random[Scraps]

                                    if (x == 13) id = 3315885; // 1000SacredClassEXPPack (Promotion)
                                    if (x == 14) id = 3315885; // 1000SacredClassEXPPack (Promotion)

                                    if (x == 15) id = 722057; // PowerEXPBall

                                    if (x == 16) id = 2169805; // P1 Jade Kylin

                                    if (x == 17) id = 3009002; // RadiantStarStone
                                    if (x == 18) id = 3009002; // RadiantStarStone
                                    if (x == 19) id = 3009002; // RadiantStarStone

                                    if (id == 0)
                                        continue;




                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = id;
                                    Database.ItemType.DBItem DBItem;
                                    if (Pool.ItemsBase.TryGetValue(id, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = (ushort)Program.GetRandom.Next(X - 5, X + 5);
                                    ushort yy = (ushort)Program.GetRandom.Next(Y - 4, Y + 4);
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {

                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            #endregion

                            return;
                        }
                        #endregion


                        #region ChillingSpook
                        if (Family.ID == 3977)
                        {
                            GetBossReward(killer, stream);
                            SendSysMesage(killer.Player.Name + " has defeated [Chilling Spook] ", MsgServer.MsgMessage.ChatMode.World, MsgServer.MsgMessage.MsgColor.yellow);
                            Hunters.Clear();

                            #region Dropitems
                            if (killer.Player.DynamicID != killer.Player.UID)
                            {
                                for (int x = 1; x < 20; x++)
                                {
                                    uint id = 0;
                                    if (x == 1) id = 730004; // Stone(+4)
                                    if (x == 2) id = 730004; // Stone(+4)

                                    if (x == 3) id = 3004181; // Savage Bone
                                    if (x == 4) id = 3004181; // Savage Bone
                                    if (x == 5) id = 3004181; // Savage Bone

                                    if (x == 6) id = 723717; // Class5MoneyBag
                                    if (x == 7) id = 723717; // Class5MoneyBag
                                    if (x == 8) id = 723717; // Class5MoneyBag
                                    if (x == 9) id = 723717; // Class5MoneyBag
                                    if (x == 10) id = 723717; // Class5MoneyBag

                                    if (x == 11) id = 729991; // Random[Scraps]
                                    if (x == 12) id = 729991; // Random[Scraps]

                                    if (x == 13) id = 3315885; // 1000SacredClassEXPPack (Promotion)
                                    if (x == 14) id = 3315885; // 1000SacredClassEXPPack (Promotion)

                                    if (x == 15) id = 722057; // PowerEXPBall

                                    if (x == 16) id = 2169805; // P1 Jade Kylin

                                    if (x == 17) id = 3009002; // RadiantStarStone
                                    if (x == 18) id = 3009002; // RadiantStarStone
                                    if (x == 19) id = 3009002; // RadiantStarStone

                                    if (id == 0)
                                        continue;




                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = id;
                                    Database.ItemType.DBItem DBItem;
                                    if (Pool.ItemsBase.TryGetValue(id, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = (ushort)Program.GetRandom.Next(X - 5, X + 5);
                                    ushort yy = (ushort)Program.GetRandom.Next(Y - 4, Y + 4);
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {

                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            #endregion

                            return;
                        }
                        #endregion


                        #region DragonWraith
                        if (Family.ID == 3971)
                        {
                            GetBossReward(killer, stream);
                            SendSysMesage(killer.Player.Name + " has defeated [Dragon Wraith] ", MsgServer.MsgMessage.ChatMode.World, MsgServer.MsgMessage.MsgColor.yellow);
                            Hunters.Clear();

                            #region Dropitems
                            if (killer.Player.DynamicID != killer.Player.UID)
                            {
                                for (int x = 1; x < 20; x++)
                                {
                                    uint id = 0;
                                    if (x == 1) id = 730004; // Stone(+4)
                                    if (x == 2) id = 730004; // Stone(+4)

                                    if (x == 3) id = 3004181; // Savage Bone
                                    if (x == 4) id = 3004181; // Savage Bone
                                    if (x == 5) id = 3004181; // Savage Bone

                                    if (x == 6) id = 723717; // Class5MoneyBag
                                    if (x == 7) id = 723717; // Class5MoneyBag
                                    if (x == 8) id = 723717; // Class5MoneyBag
                                    if (x == 9) id = 723717; // Class5MoneyBag
                                    if (x == 10) id = 723717; // Class5MoneyBag

                                    if (x == 11) id = 729991; // Random[Scraps]
                                    if (x == 12) id = 729991; // Random[Scraps]

                                    if (x == 13) id = 3315885; // 1000SacredClassEXPPack (Promotion)
                                    if (x == 14) id = 3315885; // 1000SacredClassEXPPack (Promotion)

                                    if (x == 15) id = 722057; // PowerEXPBall

                                    if (x == 16) id = 2169805; // P1 Jade Kylin

                                    if (x == 17) id = 3009002; // RadiantStarStone
                                    if (x == 18) id = 3009002; // RadiantStarStone
                                    if (x == 19) id = 3009002; // RadiantStarStone

                                    if (id == 0)
                                        continue;




                                    MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
                                    DataItem.ITEM_ID = id;
                                    Database.ItemType.DBItem DBItem;
                                    if (Pool.ItemsBase.TryGetValue(id, out DBItem))
                                    {
                                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                                    }
                                    DataItem.Color = Role.Flags.Color.Red;
                                    ushort xx = (ushort)Program.GetRandom.Next(X - 5, X + 5);
                                    ushort yy = (ushort)Program.GetRandom.Next(Y - 4, Y + 4);
                                    if (killer.Map.AddGroundItem(ref xx, ref yy, 3))
                                    {
                                        MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                                        if (killer.Map.EnqueueItem(DropItem))
                                        {

                                            DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                                        }
                                    }
                                }
                            }
                            #endregion

                            return;
                        }
                        #endregion


                        #region Quest
                        if (killer.Player.QuestGUI.CheckQuest(3944, MsgQuestList.QuestListItem.QuestStatus.Available) || !killer.Player.QuestGUI.src.ContainsKey(3944))
                            killer.Player.QuestGUI.Accept(Database.QuestInfo.GetFinishQuest(21972, killer.Player.Class, 3944), 0);
                        if (killer.Player.QuestGUI.CheckQuest(3944, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            killer.Player.QuestGUI.IncreaseQuestObjectives(stream, 3944, 1);
                            uint[] Intentions;
                            killer.Player.QuestGUI.GetQuestObjectives(3944, out Intentions);
                            if (Intentions.Length > 0 && Intentions[0] >= 100)
                            {
                                killer.SendSysMesage("You have killed 100 devils in the DeityLand! Go claim your rewards from the Victoy Chest!");
                            }
                        }
                        #endregion

                        #region MeteorDove
                        if (Name == "MeteorDove")
                        {
                            for (ushort x = 0; x < 5; x++)
                                DropItemID(killer, 1088001, stream);
                        }
                        #endregion

                        #region Sprite
                        if (Name.Contains("Sprite"))
                        {
                            uint item = 0;
                            if (Name == "MeteorSprite") item = 1088001;
                            else if (Name == "DBSprite") item = 3303975;
                            else if (Name == "ChiSprite") item = 3008305;
                            else if (Name == "DragonSoulSprite") item = 3005133;
                            else if (Name == "SilverSprite")
                            {
                                DropItem(stream, killer.Player.UID, killer.Map, Database.ItemType.MoneyItemID((uint)50000), X, Y, MsgFloorItem.MsgItem.ItemType.Money, 50000, false, 0);
                            }
                            if (item > 0)
                                DropItemID(killer, item, stream);
                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");
                        }
                        #endregion

                        #region DropMap

                        if (MyMath.Success(1))
                        {
                            uint[] ItemsIDS = new uint[] { 3009001, 3009002, 3313368, 3310998, 3009002 };

                            uint ItemId = ItemsIDS[Program.GetRandom.Next(0, ItemsIDS.Length)];

                            DropItemID(killer, ItemId, stream);
                            killer.SendSysMesage(Name + " that you killed dropped 1 " + (Pool.ItemsBase[ItemId].Name) + "(s)!");
                            killer.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "angelwing");

                        }
                        #endregion


                    }
                    #endregion


                    #region Quest Epic&&Archives Archer
                    #region stage1
                    if (Map == 11223)
                    {
                        #region stage
                        #region Portal1
                        if (Family.ID == 4226)
                        {
                            ActionQuery naction = new ActionQuery()
                            {
                                ObjId = killer.Player.UID,
                                dwParam3 = 182,//158 //186
                                Type = 456, //137
                                PositionX = killer.Player.X,
                                PositionY = killer.Player.Y,

                            };
                            killer.Send(stream.ActionCreate(naction));
                            killer.Teleport(171, 165, 11223, killer.Player.UID);
                            var Item = new Game.MsgFloorItem.MsgItem(null, 161, 191, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, killer.Player.UID, 11223, 0, false, Pool.ServerMaps[11223], 60 * 60 * 1000);
                            Item.MsgFloor.m_ID = 3616;
                            Item.MsgFloor.m_Color = 2;
                            Item.MsgFloor.DropType = Game.MsgFloorItem.MsgDropID.Effect;

                            Item.GMap.View.EnterMap<Role.IMapObj>(Item);
                            killer.Player.Portal = 2;
                            killer.Player.QuestGUI.SendAutoPatcher("STR_ID_tArchersTask[Msg][Enter][5][6]@@", 11223, 160, 189, 0);


                        }
                        #endregion
                        #region Portal2
                        if (Family.ID == 4227)
                        {
                            ActionQuery naction = new ActionQuery()
                            {
                                ObjId = killer.Player.UID,
                                dwParam3 = 183,
                                Type = 456,
                                PositionX = killer.Player.X,
                                PositionY = killer.Player.Y,

                            };
                            killer.Send(stream.ActionCreate(naction));
                            killer.Teleport(171, 165, 11223, killer.Player.UID);
                            var Item = new Game.MsgFloorItem.MsgItem(null, 190, 153, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, killer.Player.UID, 11223, 0, false, Pool.ServerMaps[11223], 60 * 60 * 1000);
                            Item.MsgFloor.m_ID = 3616;
                            Item.MsgFloor.m_Color = 2;
                            Item.MsgFloor.DropType = Game.MsgFloorItem.MsgDropID.Effect;

                            Item.GMap.View.EnterMap<Role.IMapObj>(Item);

                            killer.Player.QuestGUI.SendAutoPatcher("STR_ID_tArchersTask[Msg][Enter][5][8]@@", 11223, 190, 153, 0);

                            killer.Player.Portal = 3;



                        }
                        #endregion
                        #region Portal3
                        if (Family.ID == 4228)
                        {
                            ActionQuery naction = new ActionQuery()
                            {
                                ObjId = killer.Player.UID,
                                dwParam3 = 184,
                                Type = 456,
                                PositionX = killer.Player.X,
                                PositionY = killer.Player.Y,

                            };
                            killer.Send(stream.ActionCreate(naction));
                            killer.Teleport(171, 165, 11223, killer.Player.UID);
                            var Item = new Game.MsgFloorItem.MsgItem(null, 146, 132, Game.MsgFloorItem.MsgItem.ItemType.Effect, 0, killer.Player.UID, 11223, 0, false, Pool.ServerMaps[11223], 60 * 60 * 1000);
                            Item.MsgFloor.m_ID = 3616;
                            Item.MsgFloor.m_Color = 2;
                            Item.MsgFloor.DropType = Game.MsgFloorItem.MsgDropID.Effect;

                            Item.GMap.View.EnterMap<Role.IMapObj>(Item);

                            killer.Player.QuestGUI.SendAutoPatcher("STR_ID_tArchersTask[Msg][Enter][5][10]@@", 11223, 146, 131, 0);

                            killer.Player.Portal = 4;



                        }
                        #endregion
                        #region Portal4
                        if (Family.ID == 4229)
                        {
                            ActionQuery naction = new ActionQuery()
                            {
                                ObjId = killer.Player.UID,
                                dwParam3 = 162,
                                Type = 456,
                                PositionX = killer.Player.X,
                                PositionY = killer.Player.Y,

                            };
                            killer.Send(stream.ActionCreate(naction));
                            killer.Teleport(170, 164, 11223);

                            killer.Player.QuestGUI.SendAutoPatcher("STR_ID_tArchersTask[Msg][Find][8]@@", 11223, 166, 161, (uint)NpcID.LuBan);
                            killer.Player.Portal = 0;
                            killer.Player.FinishStage1 = 1;



                        }
                        #endregion
                        #endregion

                        #region stage1
                        #region LuBan
                        if (Family.ID == 4230)
                        {
                            killer.MyArchives.AddItem(Role.Instance.Archives.TypeID.StoneCracker, 1, 0, 1, 0);
                            killer.Teleport(170, 164, 11223);
                            killer.Player.HaveArchives = 1;
                        }
                        #endregion
                        #endregion

                        #region stage2
                        #region MasterYan
                        if (Family.ID == 4233)
                        {
                            killer.MyArchives.AddItem(Role.Instance.Archives.TypeID.ColdMoon, 1, 0, 1, 0);
                            killer.Teleport(170, 164, 11223);
                            killer.Player.HaveArchives = 2;
                            killer.Player.QuestGUI.SendAutoPatcher("STR_ID_tArchersTask[Msg][KuiLeiBOSS]@@", 11223, 169, 174, (uint)NpcID.MasterYan);
                        }
                        #endregion
                        #endregion

                        #region MoDi
                        if (Family.ID == 4234)
                        {
                            killer.MyArchives.AddItem(Role.Instance.Archives.TypeID.ThornCutter, 1, 0, 1, 0);
                            killer.Teleport(170, 164, 11223);
                            killer.Player.HaveArchives = 3;

                        }
                        #endregion

                    }
                    #endregion

                    #endregion
                    #region Quest Archives Warrior

                    if (Map == 11040)
                    {

                        #region HuaMulan
                        if (Family.ID == 7130)
                        {
                            killer.MyArchives.AddItem(Role.Instance.Archives.TypeID.Dragonhowl, 1, 0, 1, 0);
                            killer.Teleport(104, 68, 11041);
                            killer.Player.HuaMulan = 1;
                            killer.Player.Archives = 1;

                        }
                        #endregion
                        #region Tyre
                        if (Family.ID == 7132)
                        {
                            killer.Player.MonsterTyre += 1;
                            var map = Pool.ServerMaps[11040];
                            if (!map.ContainMobID(7132, killer.Player.UID + 1))
                            {
                                Server.AddMapMonster(stream, map, 7132, 88, 72, 18, 18, 1, killer.Player.UID + 1, true);
                            }
                            if (killer.Player.MonsterTyre == 2)
                            {
                                killer.Teleport(104, 68, 11041);
                                killer.Player.Tyre = 2;
                                killer.MyArchives.AddItem(Role.Instance.Archives.TypeID.Bloodlust, 1, 0, 1, 0);
                                killer.Player.Archives = 2;

                            }

                        }
                        #endregion
                        #region Ares
                        if (Family.ID == 7131)
                        {
                            killer.Teleport(104, 68, 11041);
                            killer.MyArchives.AddItem(Role.Instance.Archives.TypeID.Redcurse, 1, 0, 1, 0);
                            killer.Player.Ares = 2;

                        }
                        #endregion

                    }


                    #endregion         
                    #region Quest TrojanArchives

                    if (Map == 10446)
                    {
                        killer.HundredWeapons.Unlocked = true;
                        killer.Send(stream.CreateHundredWeaponsInfo(killer, MsgHundredWeaponsInfo.ActionID.RequestInfo));
                        killer.Send(stream.ActionCreate(new ActionQuery()
                        {
                            Type = ActionType.OpenDialog,
                            ObjId = killer.Player.UID,
                            Timestamp = (uint)Time32.timeGetTime().GetHashCode(),
                            dwParam = DialogCommands.TrojanWeaponArchive,
                            PositionX = killer.Player.X,
                            PositionY = killer.Player.Y
                        }));
                        ActionQuery actions = new ActionQuery()
                        {
                            Type = 456,
                            ObjId = killer.Player.UID,
                            dwParam3 = 7,
                            PositionX = killer.Player.X,
                            PositionY = killer.Player.Y,

                        };
                        killer.Send(stream.ActionCreate(actions));
                        killer.HundredWeapons.LoadPowerFoucs(killer);
                    }


                    #endregion
                    #region DropItem

                    if (Family.Boss == 0)
                    {

                        ushort xx = X;
                        ushort yy = Y;
                        #region Item
                        if (Role.MyMath.Success(5))
                        {
                            //Database.ItemType.DBItem DbItem = null;
                            //byte ID_Quality;
                            //bool ID_Special;
                            // uint ID = Family.ItemGenerator.GenerateItem(Map, out ID_Quality, out ID_Special, out DbItem);
                            // if (ID != 0)
                            {
                                // [FileDrops] the AddGroundItem below reserved a floor cell (Item flag) for an item that is
                                // never dropped -> 5% of kills leaked a blocked cell forever. Disabled; the file table now
                                // runs from FileDrops.OnKill below (Infinity[DB]\drops.txt, OFF by default).
                                // bool drop = true;
                                // if (killer.Map.AddGroundItem(ref xx, ref yy) && drop)
                                // {
                                //     DropItem(stream, killer.Player.UID, killer.Map, ID, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, ID_Special, ID_Quality, killer, DbItem);
                                // }
                            }

                        }

                        #endregion
                    }
                    // [FileDrops] drop_* / drop_money / [SpecialDrop] from Monsters\*.ini, IN ADDITION to the hard-coded drops
                    // above. Off unless Infinity[DB]\drops.txt has file_drops=1. Skips bosses/event mobs; never throws.
                    FileDrops.OnKill(this, killer, stream);
                    #endregion
                }

            }

        }
        private bool DropItem(ServerSockets.Packet stream, uint OwnerItem, Role.GameMap map, uint ItemID, ushort XX, ushort YY, MsgFloorItem.MsgItem.ItemType typ
                 , uint amount, bool special, byte ID_Quality, Client.GameClient user = null, Database.ItemType.DBItem DBItem = null)
        {
            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();

            DataItem.ITEM_ID = ItemID;
            if (DataItem.Durability > 100)
            {
                DataItem.Durability = (ushort)Program.GetRandom.Next(100, DataItem.Durability / 10);
                DataItem.MaximDurability = DataItem.Durability;
            }

            else
            {
                DataItem.Durability = (ushort)Program.GetRandom.Next(1, 10);
                DataItem.MaximDurability = 10;
            }

            DataItem.Color = Role.Flags.Color.Red;
            if (typ == MsgFloorItem.MsgItem.ItemType.Item)
            {
                byte sockets;
                bool lucky = false;
                if (DataItem.IsEquip)
                {
                    if (!special)
                    {

                        lucky = (ID_Quality > 7); // q>unique
                        if (!lucky)
                            lucky = (DataItem.Plus = Family.ItemGenerator.GeneratePurity()) != 0;
                        if (!lucky)
                            lucky = (DataItem.Bless = Family.ItemGenerator.GenerateBless()) != 0;
                        if (!lucky)
                        {
                            if (DataItem.IsWeapon)
                            {
                                sockets = Family.ItemGenerator.GenerateSocketCount(DataItem.ITEM_ID);

                                if (sockets >= 1)
                                    DataItem.SocketOne = Role.Flags.Gem.EmptySocket;
                                if (sockets >= 2) // [FileDrops] was "else if" -> the 2nd socket never happened
                                    DataItem.SocketTwo = Role.Flags.Gem.EmptySocket;
                            }
                        }
                    }
                    if (DBItem != null)
                    {
                        DataItem.Durability = (ushort)Program.GetRandom.Next(1, DBItem.Durability / 10 + 10);
                        // [FileDrops] Next(min, max) threw when the type's durability was below the rolled one
                        DataItem.MaximDurability = (ushort)Program.GetRandom.Next(DataItem.Durability, Math.Max((int)DataItem.Durability, (int)DBItem.Durability));
                    }
                }
                else
                {
                    if (DBItem != null)
                        DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
                }
            }
            if (user != null)
            {
                if (user.Player.VipLevel >= 4)
                {
                    if (DataItem.ITEM_ID == 1088000)
                    {
                        user.Inventory.Update(DataItem, Role.Instance.AddMode.ADD, stream);
                        if (user.Inventory.Contain(1088000, 1) && user.Player.VipLevel == 6)
                        {
                            user.Inventory.Remove(1088000, 1, stream);
                            user.Inventory.Add(stream, 720028, 1);
                        }
                        return false; // [FileDrops] went to the bag, nothing on the floor
                    }
                }
            }

            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, XX, YY, typ, amount, DynamicID, Map, OwnerItem, true, map);

            if (map.EnqueueItem(DropItem))
            {
                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                return true;
            }
            return false;
        }

        /// <summary>[FileDrops] one file-table item on (x,y) (already reserved by AddGroundItem), owned by the killer.
        /// true = it is on the floor.</summary>
        internal bool DropFileItem(ServerSockets.Packet stream, Client.GameClient killer, Role.GameMap map, uint itemId, ushort x, ushort y,
            bool special, byte quality, Database.ItemType.DBItem db)
        {
            return DropItem(stream, killer.Player.UID, map, itemId, x, y, MsgFloorItem.MsgItem.ItemType.Item, 0, special, quality, killer, db);
        }

        /// <summary>[FileDrops] silver on (x,y), owned by the killer (team money rules as usual).</summary>
        internal bool DropFileMoney(ServerSockets.Packet stream, Client.GameClient killer, Role.GameMap map, uint amount, ushort x, ushort y)
        {
            return DropItem(stream, killer.Player.UID, map, Database.ItemType.MoneyItemID(amount), x, y, MsgFloorItem.MsgItem.ItemType.Money, amount, false, 0, killer);
        }
        public void AddFadeAway(int time, Role.GameMap map)
        {
            if (!Alive)
            {
                DateTime timer = DateTime.Now;
                if (timer > DeadStamp.AddSeconds(5))
                {
                    if (AddFlag(MsgServer.MsgUpdate.Flags.FadeAway, Role.StatusFlagsBigVector32.PermanentFlag, true))
                    {
                        FadeAway = timer;

                    }
                }
            }
        }
        public unsafe bool RemoveView(int time, Role.GameMap map)
        {
            if (ContainFlag(MsgServer.MsgUpdate.Flags.FadeAway) && State != MobStatus.Respawning)
            {
                DateTime timer = DateTime.Now;
                if (timer > FadeAway.AddSeconds(3))
                {
                    using (var rec = new ServerSockets.RecycledPacket())
                    {
                        var stream = rec.GetStream();

                        ActionQuery action;

                        action = new ActionQuery()
                        {
                            ObjId = UID,
                            Type = ActionType.RemoveEntity
                        };

                        Send(stream.ActionCreate(action));
                    }

                    State = MobStatus.Respawning;

                    map.View.MoveTo<Role.IMapObj>(this, RespawnX, RespawnY);

                    X = RespawnX;
                    Y = RespawnY;
                    Target = null;

                    return true;
                }
            }
            return false;
        }

        public void DropItemID(Client.GameClient killer, uint itemid, ServerSockets.Packet stream, byte range = 3, int mins = 0)
        {
            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
            DataItem.ITEM_ID = itemid;
            Database.ItemType.DBItem DBItem;
            if (itemid == 4034801 || itemid == 4035101 || itemid == 4035009 || itemid == 4034201 || itemid == 4034301)
            {
                return;
            }
            if (Pool.ItemsBase.TryGetValue(itemid, out DBItem))
            {
                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
            }
            DataItem.Color = Role.Flags.Color.Red;
            ushort xx = (ushort)Program.GetRandom.Next(X - 5, X + 5);
            ushort yy = (ushort)Program.GetRandom.Next(Y - 4, Y + 4);
            if (killer.Map.AddGroundItem(ref xx, ref yy, range))
            {
                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                if (killer.Map.EnqueueItem(DropItem))
                {
                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                }
            }
        }
        public void DropItemID2(Client.GameClient killer, uint itemid, ServerSockets.Packet stream, byte range = 3, int mins = 0)
        {
            MsgServer.MsgGameItem DataItem = new MsgServer.MsgGameItem();
            DataItem.ITEM_ID = itemid;
            Database.ItemType.DBItem DBItem;
            if (itemid == 4034801 || itemid == 4035101 || itemid == 4035009 || itemid == 4034201 || itemid == 4034301)
            {
                return;
            }
            if (Pool.ItemsBase.TryGetValue(itemid, out DBItem))
            {
                DataItem.Durability = DataItem.MaximDurability = DBItem.Durability;
            }
            DataItem.Color = Role.Flags.Color.Red;
            ushort xx = (ushort)Program.GetRandom.Next(X - 14, X + 14);
            ushort yy = (ushort)Program.GetRandom.Next(Y - 14, Y + 14);
            if (killer.Map.AddGroundItem(ref xx, ref yy, range))
            {
                MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(DataItem, xx, yy, MsgFloorItem.MsgItem.ItemType.Item, 0, DynamicID, Map, killer.Player.UID, true, GMap);

                if (killer.Map.EnqueueItem(DropItem))
                {
                    DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Visible);
                }
            }
        }
        public MonsterFamily Family;
        public MonsterView View;
        public MobStatus State;
        public Role.Player Target = null;
        public DateTime AttackSpeed = new DateTime();

        public Role.StatusFlagsBigVector32 BitVector;
        public void AddSpellFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool RemoveOnDead, int SecoundStamp = 0)
        {
            if (BitVector.ContainFlag((int)Flag))
                BitVector.TryRemove((int)Flag);
            AddFlag(Flag, Secounds, RemoveOnDead, SecoundStamp);
        }
        public bool AddFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool RemoveOnDead, int StampSecounds = 0)
        {
            if (!BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryAdd((int)Flag, Secounds, RemoveOnDead, StampSecounds);
                UpdateFlagOffset();
                return true;
            }
            return false;
        }
        public bool RemoveFlag(Game.MsgServer.MsgUpdate.Flags Flag, Role.GameMap map)
        {
            if (BitVector.ContainFlag((int)Flag))
            {
                BitVector.TryRemove((int)Flag);
                UpdateFlagOffset();
                return true;
            }
            return false;
        }
        public bool UpdateFlag(Game.MsgServer.MsgUpdate.Flags Flag, int Secounds, bool SetNewTimer, int MaxTime)
        {
            return BitVector.UpdateFlag((int)Flag, Secounds, SetNewTimer, MaxTime);
        }
        public void ClearFlags(bool SendScreem = false)
        {
            BitVector.GetClear();
            UpdateFlagOffset(SendScreem);
        }
        public bool ContainFlag(Game.MsgServer.MsgUpdate.Flags Flag)
        {
            return BitVector.ContainFlag((int)Flag);
        }
        private unsafe void UpdateFlagOffset(bool SendScreem = true)
        {
            if (SendScreem)
                SendUpdate(BitVector.bits, Game.MsgServer.MsgUpdate.DataType.StatusFlag);
        }

        public byte OpenBoss = 0;
        public uint Map { get; set; }
        public uint DynamicID { get; set; }

        public short GetMyDistance(ushort X2, ushort Y2)
        {
            return Role.Core.GetDistance(X, Y, X2, Y2);
        }
        public short OldGetDistance(ushort X2, ushort Y2)
        {
            return Role.Core.GetDistance(PX, PY, X2, Y2);
        }
        public bool InView(ushort X2, ushort Y2, byte distance)
        {
            return (!(OldGetDistance(X2, Y2) < distance) && GetMyDistance(X2, Y2) < distance);
        }


        public unsafe void Send(ServerSockets.Packet msg)
        {
            View.SendScreen(msg, GMap);
        }
        public void UpdateMonsterView(Role.RoleView Target, ServerSockets.Packet stream)
        {
            foreach (var player in View.Roles(GMap, Role.MapObjectType.Player))
            {
                if (InView(player.X, player.Y, MonsterView.ViewThreshold))
                    player.Send(GetArray(stream, false));
            }
        }
        public bool UpdateMapCoords(ushort New_X, ushort New_Y, Role.GameMap _map)
        {
            if (!_map.IsFlagPresent(New_X, New_Y, Role.MapFlagType.Monster))
            {
                _map.SetMonsterOnTile(X, Y, false);
                _map.SetMonsterOnTile(New_X, New_Y, true);
                _map.View.MoveTo<MonsterRole>(this, New_X, New_Y);
                X = New_X;
                Y = New_Y;
                return true;
            }
            return false;
        }
        public void RemoveRole(Role.IMapObj obj)
        {

        }
        public Role.MapObjectType ObjType { get; set; }

        public byte Boss = 0;
        public uint Mesh = 0;
        public uint UID { get; set; }
        public byte Level = 0;
        public uint HitPoints;
        public uint OwnerCount = 0;
        public uint OwnerFlag = 0;
        public uint OwnerUID = 0;
        public ushort RespawnX;
        public ushort RespawnY;


        public ushort PX = 0;
        public ushort PY = 0;
        public ushort _xx;
        public ushort _yy;

        public ushort X { get { return _xx; } set { PX = _xx; _xx = value; } }
        public ushort Y { get { return _yy; } set { PY = _yy; _yy = value; } }
        public Role.Flags.ConquerAction Action = Role.Flags.ConquerAction.None;
        public Role.Flags.ConquerAngle Facing = Role.Flags.ConquerAngle.East;
        public string LocationSpawn = "";
        public Role.GameMap GMap;
        public bool RemoveOnDead = false;
        public uint PetFlag = 0;


        public unsafe void SendString(ServerSockets.Packet stream, Game.MsgServer.MsgStringPacket.StringID id, params string[] args)
        {
            Game.MsgServer.MsgStringPacket packet = new Game.MsgServer.MsgStringPacket();
            packet.ID = id;
            packet.UID = UID;

            packet.Strings = args;
            Send(stream.StringPacketCreate(packet));
        }
        public MonsterRole(MonsterFamily Famil, uint _UID, string locationspawn, Role.GameMap _map)
        {
            AllowDynamic = false;
            GMap = _map;
            LocationSpawn = locationspawn;
            ObjType = Role.MapObjectType.Monster;
            Name = Famil.Name;
            Family = Famil;
            UID = _UID;
            Mesh = Famil.Mesh;
            Level = (byte)Famil.Level;
            HitPoints = (uint)Famil.MaxHealth;
            View = new MonsterView(this);
            State = MobStatus.Idle;
            BitVector = new Role.StatusFlagsBigVector32(450);
            Boss = Family.Boss;
            Facing = (Role.Flags.ConquerAngle)Pool.GetRandom.Next(0, 8);

        }
        public bool Alive { get { return HitPoints > 0; } }



        public unsafe ServerSockets.Packet GetArray(ServerSockets.Packet stream, bool view)
        {
            if (IsFloor && Mesh != 980)
            {
                return stream.ItemPacketCreate(this.FloorPacket);

            }
            stream.InitWriter();
            var proto = new Role.Player.SpawnPacketProto()
            {
                UID = UID,
                Mesh = Mesh,
                MonsterLevel = Level,
                Hitpoints = IsFloor ? (uint)StampFloorSecounds : (uint)HitPoints,
                X = X,
                Y = Y,
                Action = (ushort)Action,
                Level = Level,
                MonstersID = Family.ID,
                // Use byte[] Name instead of string[] Names
                Name = System.Text.Encoding.UTF8.GetBytes(Name ?? "")
            };
            proto.Boss = Boss > 0;
            if (Mesh == 980)
            {
                proto.OwnerPet = (uint)3;
                proto.Facing = (byte)this.OwnerFloor.Player.Angle;
                proto.OwnerUID = this.OwnerFloor.Player.UID;
                proto.GuildID = this.OwnerFloor.Player.GuildID;
                proto.OwnerPet1 = (uint)15;
                proto.MonstersID = (uint)4264;
            }
            if (this.Mesh == (uint)138 || this.Mesh == (uint)137 || this.Mesh == (uint)848)
            {
                proto.OwnerPet = this.PetFlag;
                proto.Facing = (byte)this.OwnerFloor.Player.Angle;
                proto.OwnerUID = this.OwnerFloor.Player.UID;
                proto.OwnerPet1 = (uint)15;
                proto.MonstersID = this.Family.ID;
            }
            proto.MaxLife = (uint)Family.MaxHealth;
            proto.StatusFlags = new ulong[BitVector.bits.Length];
            for (int x = 0; x < BitVector.bits.Length; x++)
                proto.StatusFlags[x] = BitVector.bits[x];
            // [MonsterHp 2026-10-15] official CDlgBossLife shows "%d/%d" = role life / MaxLife (Cn_Res STR_BOSSLIFE_STA_LIFE), so send the raw HP with MaxLife; the old 1/10000 scaling made the bar wrong
            if (false && !IsFloor && Boss > 0)
            {
                uint key = (uint)(Family.MaxHealth / 10000);
                if (key != 0)
                    proto.Hitpoints = (uint)(proto.Hitpoints / key);
                else
                    proto.Hitpoints = (uint)(proto.Hitpoints * Family.MaxHealth);
            }

            stream.ProtoBufferSerialize(proto);
            stream.Finalize(Game.GamePackets.MsgPlayer);
            return stream;
        }
        public unsafe void SendUpdate(uint[] Value, Game.MsgServer.MsgUpdate.DataType datatype)
        {
            using (var rec = new ServerSockets.RecycledPacket())
            {
                var stream = rec.GetStream();

                Game.MsgServer.MsgUpdate packet = new Game.MsgServer.MsgUpdate(stream, UID, 1);
                stream = packet.Append(stream, datatype, Value);
                stream = packet.GetArray(stream);
                Send(stream);
            }
        }
    }
}