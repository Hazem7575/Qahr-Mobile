using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer.AttackHandler
{
    public class CallPet
    {
        public unsafe static void Execute(Client.GameClient user, InteractQuery Attack, ServerSockets.Packet stream, Dictionary<ushort, Database.MagicType.Magic> DBSpells)
        {
            if (user.Player.MyClones.Count != 0)
            {
                foreach (var clone in user.Player.MyClones.GetValues())
                    clone.RemoveThat(user);

                user.Player.MyClones.Clear();
                return;
            }

            Database.MagicType.Magic DBSpell;
            MsgSpell ClientSpell;
            if (CheckAttack.CanUseSpell.Verified(Attack, user, DBSpells, out ClientSpell, out DBSpell))
            {
                switch (ClientSpell.ID)
                {
                    case (ushort)Role.Flags.SpellID.Thundercloud:
                        {
                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                             , 0, Attack.X, Attack.Y, ClientSpell.ID
                             , ClientSpell.Level, ClientSpell.UseSpellSoul);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);
                            int count = 1;
                            byte DoubleThunder = 0;
                            if (user.Rune.IsEquipped("DoubleThunder", ref DoubleThunder))
                            {
                                DoubleThunder = (byte)(98 + DoubleThunder * 2);
                                count = 2;
                            }
                            for (int i = 0; i < count; i++)
                            {

                                Game.MsgMonster.MonsterFamily famil;
                                if (Pool.MonsterFamilies.TryGetValue(1, out famil))
                                {
                                    Game.MsgMonster.MonsterFamily Monster = famil.Copy();

                                    Monster.SpawnX = Attack.X;

                                    Monster.SpawnY = Attack.Y;
                                    Monster.MaxSpawnX = (ushort)(Attack.X + 1);
                                    Monster.MaxSpawnY = (ushort)(Attack.Y + 1);
                                    Monster.MapID = user.Player.Map;
                                    Monster.SpawnCount = 2;
                                    Game.MsgMonster.MonsterRole rolemonster = user.Map.MonstersColletion.Add(Monster, true, user.Player.DynamicID, true);
                                    if (rolemonster == null)
                                    {

                                        return;
                                    }
                                    rolemonster.Family.ID = 4264U;
                                    rolemonster.IsFloor = true;
                                    rolemonster.FloorStampTimer = DateTime.Now.AddSeconds(7);
                                    rolemonster.Family.SpellId = 13190;
                                    rolemonster.Family.AttackSpeed = 750;
                                    rolemonster.Family.Settings = Game.MsgMonster.MonsterSettings.Lottus;

                                    rolemonster.FloorPacket = new MsgFloorItem.MsgItemPacket();
                                    rolemonster.FloorPacket.m_UID = rolemonster.UID;
                                    rolemonster.FloorPacket.m_ID = MsgFloorItem.MsgItemPacket.Thundercloud;
                                    rolemonster.FloorPacket.m_X = Attack.X;
                                    rolemonster.FloorPacket.m_Y = Attack.Y;
                                    rolemonster.FloorPacket.MaxLife = 25;
                                    rolemonster.FloorPacket.Life = 25;
                                    rolemonster.FloorPacket.DropType = MsgFloorItem.MsgDropID.Effect;
                                    rolemonster.FloorPacket.m_Color = 13;
                                    rolemonster.FloorPacket.ItemOwnerUID = user.Player.UID;
                                    rolemonster.FloorPacket.GuildID = user.Player.GuildID;
                                    rolemonster.FloorPacket.FlowerType = 2;//2;
                                    rolemonster.FloorPacket.Timer = Role.Core.TqTimer(rolemonster.FloorStampTimer);
                                    rolemonster.PetFlag = 3;
                                    if (DBSpell.Level >= 5)
                                        rolemonster.PetFlag = 9;
                                    rolemonster.OwnerFlag = 10010U;
                                    rolemonster.OwnerUID = user.Player.UID;
                                    rolemonster.OwnerCount = 3U;
                                    rolemonster.HitPoints = 25U;
                                    rolemonster.Family.MaxHealth = 10000;
                                    rolemonster.Boss = 1;
                                    rolemonster.Level = 140;
                                    rolemonster.Name = "Thundercloud";
                                    rolemonster.Mesh = 980;
                                    rolemonster.StampFloorSecounds = 10000;
                                    rolemonster.FloorStampTimer = DateTime.Now.AddSeconds(1);
                                    rolemonster.RemoveFloor = DateTime.Now.AddSeconds(25);

                                    rolemonster.DBSpell = Pool.Magic[13190][0];
                                    rolemonster.OwnerFloor = user;
                                    user.Map.View.EnterMap<Role.IMapObj>(rolemonster);
                                    user.Send(stream.CreatePetInfo(rolemonster.UID, 4264, rolemonster.PetFlag, rolemonster.Mesh, 14, rolemonster.X, rolemonster.Y, rolemonster.Name));
                                    rolemonster.Send(rolemonster.GetArray(stream, false));

                                    ActionQuery action = new ActionQuery()
                                    {
                                        ObjId = rolemonster.UID,
                                        Type = ActionType.ReviveMonster,
                                        PositionX = rolemonster.X,
                                        PositionY = rolemonster.Y
                                    };
                                    user.Player.View.SendView(stream.ActionCreate(action), true);

                                    user.Map.GetRandCoord(ref Attack.X, ref Attack.Y, 5);

                                }
                            }
                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 1000, DBSpells);
                            break;
                        }
                    case (ushort)Role.Flags.SpellID.ShadowClone:
                        {
                            if (user.Player.Map == 10741)
                            {
                                user.SendSysMesage("Sorry, You Can`t Use Shadow Clone In Gold Box .");
                                return;

                            }

                            MsgSpellAnimation MsgSpell = new MsgSpellAnimation(user.Player.UID
                                , 0, Attack.X, Attack.Y, ClientSpell.ID
                                , ClientSpell.Level, ClientSpell.UseSpellSoul);

                            if (ClientSpell.Level >= 4)
                                Role.Clone.CreateShadowClone(user.Player, stream);
                            else Role.Clone.CreateShadowClone(user.Player, 3, stream);



                            foreach (var clone in user.Player.MyClones.GetValues())
                            {

                                ActionQuery action = new ActionQuery()
                                {
                                    ObjId = clone.UID,
                                    Type = ActionType.ReviveMonster,
                                    PositionX = user.Player.X,
                                    PositionY = user.Player.Y
                                };
                                user.Player.View.SendView(stream.ActionCreate(action), true);
                            }

                            Updates.UpdateSpell.CheckUpdate(stream, user, Attack, 1000, DBSpells);
                            MsgSpell.SetStream(stream);
                            MsgSpell.Send(user);

                            break;
                        }
                }
            }
        }
    }
}
