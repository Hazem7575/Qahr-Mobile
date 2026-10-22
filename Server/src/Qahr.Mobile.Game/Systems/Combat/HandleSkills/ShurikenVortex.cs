using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgFloorItem;
namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class ShurikenVortex
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (Attack.SpellID == (ushort)Role.Flags.SpellID.ShurikenEffect)
            {
                DBSpell = Pool.Magic[(ushort)Role.Flags.SpellID.ShurikenEffect][0];
                MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                               , 0, Attack.X, Attack.Y, (ushort)Role.Flags.SpellID.ShurikenVortex
                               , 0, 0);
                uint Experience = 0;
                foreach (Role.IMapObj target in user.Player.View.Roles(Role.MapObjectType.Monster))
                {
                    MsgMonster.MonsterRole attacked = target as MsgMonster.MonsterRole;
                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
                    {
                        if (CheckAttack.CanAttackMonster.Verified(user, attacked, DBSpell))
                        {
                            MsgSpellAnimation.SpellObj AnimationObj;
                            Calculate.Physical.OnMonster(user.Player, attacked, DBSpell, out AnimationObj);
                            Experience += ReceiveAttack.Monster.Execute(stream, AnimationObj, user, attacked);
                            MsgSpell.Targets.Enqueue(AnimationObj);
                        }
                    }
                }
                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.Player))
                {
                    var attacked = targer as Role.Player;
                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
                    {
                        if (CheckAttack.CanAttackPlayer.Verified(user, attacked, DBSpell))
                        {
                            MsgSpellAnimation.SpellObj AnimationObj;
                            Calculate.Physical.OnPlayer(user.Player, attacked, DBSpell, out AnimationObj);
                            AnimationObj.Damage = AnimationObj.Damage * 30 / 100;
                            ReceiveAttack.Player.Execute(AnimationObj, user, attacked);
                            MsgSpell.Targets.Enqueue(AnimationObj);
                        }
                    }

                }
                foreach (Role.IMapObj targer in user.Player.View.Roles(Role.MapObjectType.SobNpc))
                {
                    var attacked = targer as Role.SobNpc;
                    if (Calculate.Base.GetDistance(user.Player.X, user.Player.Y, attacked.X, attacked.Y) < DBSpell.Range)
                    {
                        if (CheckAttack.CanAttackNpc.Verified(user, attacked, DBSpell))
                        {
                            MsgSpellAnimation.SpellObj AnimationObj;
                            Calculate.Physical.OnNpcs(user.Player, attacked, DBSpell, out AnimationObj);
                            Experience += ReceiveAttack.Npc.Execute(stream, AnimationObj, user, attacked);
                            MsgSpell.Targets.Enqueue(AnimationObj);
                        }
                    }
                }
                Updates.IncreaseExperience.Up(stream, user, Experience);
                MsgSpell.SetStream(stream);
                MsgSpell.Send(user);

                return;
            }


            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.FireArrow:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID, 0, Attack.X, Attack.Y, ClientSpell.ID, ClientSpell.Level, ClientSpell.UseSpellSoul);
                            if (!user.Player.FloorSpells.ContainsKey(ClientSpell.ID))
                                user.Player.FloorSpells.TryAdd(ClientSpell.ID, new Role.FloorSpell.ClientFloorSpells(user.Player.UID, user.Player.X, user.Player.Y, ClientSpell.SoulLevel, DBSpell, user.Map));
                            Role.FloorSpell floorSpell = new Role.FloorSpell(3600, Attack.X, Attack.Y, 14, DBSpell, 3100, 1000);
                            floorSpell.FloorPacket.m_X = Attack.X;
                            floorSpell.FloorPacket.m_Y = Attack.Y;
                            floorSpell.FloorPacket.OwnerX = user.Player.X;
                            floorSpell.FloorPacket.OwnerY = user.Player.Y;
                            floorSpell.FloorPacket.GuildID = user.Player.GuildID;
                            floorSpell.FloorPacket.ItemOwnerUID = user.Player.UID;
                            floorSpell.FloorPacket.Name = "STR_TRAP_ID_3600@@";
                            floorSpell.FloorPacket.m_Color = (byte)28;
                            user.Player.FloorSpells[ClientSpell.ID].AddItem(floorSpell);
                            user.Player.View.SendView(stream.ItemPacketCreate(floorSpell.FloorPacket), true);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.ShurikenVortex:
                        {
                            if (user.Player.ContainFlag(MsgUpdate.Flags.Ride))
                                user.Player.RemoveFlag(MsgUpdate.Flags.Ride);

                            if (!user.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.ShurikenEffect))
                                user.MySpells.Add(stream, (ushort)Role.Flags.SpellID.ShurikenEffect);
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                            , 0, Attack.X, Attack.Y, ClientSpell.ID
                            , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            user.Player.RemoveFlag(MsgUpdate.Flags.XPList);
                            user.Player.OpenXpSkill(MsgUpdate.Flags.ShurikenVortex, 20, 1);

                            MsgSpell.Targets.Enqueue(new MsgSpellAnimation.SpellObj(user.Player.UID, 0, MsgAttackPacket.AttackEffect.None));
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            break;
                        }
                }
            }
        }
    }
}
