using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgServer
{
    public unsafe static partial class MsgBuilder
    {
        public static unsafe void GetStatus(this ServerSockets.Packet stream, out uint dwParam)
        {
          
            dwParam = stream.ReadUInt32();
            
        }
        public static unsafe void GetJiangStatus(this ServerSockets.Packet stream, out uint type,out uint UID, out MsgStatus status)
        {
            type = stream.ReadUInt32();
            UID = stream.ReadUInt32();
            status = new MsgStatus();
            status.MaxHitpoints = stream.ReadUInt32();
            status.MaxMana = stream.ReadUInt32();
            status.MaxAttack = stream.ReadUInt32();
            status.MinAttack = stream.ReadUInt32();
            status.Defence = stream.ReadUInt32();
            status.MagicAttack = stream.ReadUInt32();
            status.MagicDefence = stream.ReadUInt32();
            status.Dodge = stream.ReadUInt32();
            status.AgilityAtack = stream.ReadUInt32();
            status.Accuracy = stream.ReadUInt32();
            status.PhysicalPercent = stream.ReadUInt32();
            status.MagicPercent = stream.ReadUInt32();
            status.MDefence = stream.ReadUInt32();
            status.Damage = stream.ReadUInt32();
            status.ItemBless = stream.ReadUInt32();
            status.CriticalStrike = stream.ReadUInt32();
            status.SkillCStrike = stream.ReadUInt32();
            status.Immunity = stream.ReadUInt32();
            status.Penetration = stream.ReadUInt32();
            status.Block = stream.ReadUInt32();
            status.Breakthrough = stream.ReadUInt32();
            status.Counteraction = stream.ReadUInt32();
            status.Detoxication = stream.ReadUInt32();
            status.PhysicalDamageIncrease = stream.ReadUInt32();
            status.MagicDamageIncrease = stream.ReadUInt32();
            status.PhysicalDamageDecrease = stream.ReadUInt32();
            status.MagicDamageDecrease = stream.ReadUInt32();
            status.MetalResistance = stream.ReadUInt32();
            status.WoodResistance = stream.ReadUInt32();
            status.WaterResistance = stream.ReadUInt32();
            status.FireResistance = stream.ReadUInt32();
            status.EarthResistance = stream.ReadUInt32();
            status.RuneScore = stream.ReadUInt16();
            status.DashRate = stream.ReadUInt32();
            status.LuckyStrike = stream.ReadUInt32();
            status.Parry = stream.ReadUInt32();
            stream.ReadUInt32();
            status.DodgeRate = stream.ReadUInt32();
            status.HitRate = stream.ReadUInt32();
        }
        public static unsafe ServerSockets.Packet StatusJiangHuCreate(this ServerSockets.Packet stream, MsgStatus status)
        {
            stream.InitWriter();
            stream.Write((uint)1);//timer -> we abusing this for inter server
            stream.Write(status.UID);//8
            stream.Write(status.MaxHitpoints);
            stream.Write(status.MaxMana);
            stream.Write(status.MaxAttack);
            stream.Write(status.MinAttack);
            stream.Write(status.Defence);
            stream.Write(status.MagicAttack);
            stream.Write(status.MagicDefence);
            stream.Write(status.Dodge);
            stream.Write(status.AgilityAtack);
            stream.Write(status.Accuracy);
            stream.Write(status.PhysicalPercent);
            stream.Write(status.MagicPercent);
            stream.Write(status.MDefence);
            stream.Write(status.Damage);
            stream.Write(status.ItemBless);

            stream.Write(status.CriticalStrike);
            stream.Write(status.SkillCStrike);
            stream.Write(status.Immunity);
            stream.Write(status.Penetration);
            stream.Write(status.Block);
            stream.Write(status.Breakthrough);
            stream.Write(status.Counteraction);
            stream.Write(status.Detoxication);
            stream.Write(status.PhysicalDamageIncrease);
            stream.Write(status.MagicDamageIncrease);
            stream.Write(status.PhysicalDamageDecrease);
            stream.Write(status.MagicDamageDecrease);
            stream.Write(status.MetalResistance);
            stream.Write(status.WoodResistance);
            stream.Write(status.WaterResistance);
            stream.Write(status.FireResistance);
            stream.Write(status.EarthResistance);

            stream.Write(status.PrestigeLevel);
            stream.Write(status.RuneScore);
            stream.Write(status.DashRate);
            stream.Write(status.LuckyStrike);
            stream.Write(status.Parry);
            stream.Write(0);
            stream.Write(status.DodgeRate);
            stream.Write(status.HitRate);
            stream.Write(status.Resist);
            stream.Finalize(GamePackets.MsgPlayerAttriInfo);
            return stream;
        }
        public static unsafe ServerSockets.Packet StatusCreate(this ServerSockets.Packet stream, MsgStatus status)
        {
            stream.InitWriter();

            stream.Write(status.UID);//4
            stream.Write(status.MaxHitpoints);//8
            stream.Write(status.MaxMana);//12
            stream.Write(status.MaxAttack);//16
            stream.Write(status.MinAttack);//20
            stream.Write(status.Defence);//24
            stream.Write(status.MagicAttack);//28
            stream.Write(status.MagicDefence);//32
            stream.Write(status.Dodge);//36
            stream.Write(status.AgilityAtack);//40
            stream.Write(status.Accuracy);//44
            stream.Write(status.PhysicalPercent);//48
            stream.Write(status.MagicPercent);//52
            stream.Write(status.MDefence);//56
            stream.Write(status.Damage);//60
            stream.Write(status.ItemBless);//64

            stream.Write(status.CriticalStrike);//68
            stream.Write(status.SkillCStrike);//72
            stream.Write(status.Immunity);//76
            stream.Write(status.Penetration);//80
            stream.Write(status.Block);//84
            stream.Write(status.Breakthrough);//88
            stream.Write(status.Counteraction);//92
            stream.Write(status.Detoxication);//96
            stream.Write(status.PhysicalDamageIncrease); //100
            stream.Write(status.MagicDamageIncrease);//104
            stream.Write(status.PhysicalDamageDecrease);//108
            stream.Write(status.MagicDamageDecrease);//112
            stream.Write(status.MetalResistance);//116
            stream.Write(status.WoodResistance);//120
            stream.Write(status.WaterResistance);//124
            stream.Write(status.FireResistance);//128
            stream.Write(status.EarthResistance);//132
            stream.Write(status.PrestigeLevel);//136
            stream.Write(status.RuneScore);//140
            // QahrMobile: the mobile CMsgPlayerAttriInfo (180 bytes) continues with Phase 144, BattlePower 148,
            // DmgAddEffect 152, MonsterExpAdd 156, MonsterDmgAdd 160, BossDamageAdd 164, DamageAdd 168,
            // ReduceCommonDmg 172, GouYuHitRate 176 (CMsgPlayerAttriInfo::Process).
            stream.Write(0);//144
            stream.Write(0);//148
            stream.Write(0);//152
            stream.Write(0);//156
            stream.Write(0);//160
            stream.Write(0);//164
            stream.Write(0);//168
            stream.Write(0);//172
            stream.Write(0);//176
            stream.Finalize(GamePackets.MsgPlayerAttriInfo);
            return stream;
        }
    }
    public class MsgStatus
    {
        public int Stamp;
        public uint UID;
        public uint MaxHitpoints;
        public uint MaxMana;
        public uint MaxAttack;
        public uint MinAttack;
        public uint Defence;
        public uint MagicAttack;
        public uint MagicDefence;
        public uint Dodge;
        public uint AgilityAtack;
        public uint Accuracy;
        public uint PhysicalPercent;//52
        public uint MagicPercent;
        public uint Damage;
        public uint ItemBless;

        public uint CriticalStrike;//100 = 1;
        public uint SkillCStrike;//100 = 1;
        public uint Immunity;//100 = 1;
        public uint Penetration;//100 = 1;
        public uint Block;//100 = 1;
        public uint Breakthrough;
        public uint Counteraction;//10 = 1;
        public uint Detoxication;//1 =1

        public uint PhysicalDamageIncrease;
        public uint MagicDamageIncrease;
        public uint PhysicalDamageDecrease;
        public uint MagicDamageDecrease;

        public uint MetalResistance;
        public uint WoodResistance;
        public uint WaterResistance;
        public uint FireResistance;
        public uint EarthResistance;

        public uint MDefence;
        public ushort MaxVigor;
        public uint PrestigeLevel;
        public ushort TotalAnimaLevel;
        public uint RuneScore;
        public uint DashRate;
        public uint LuckyStrike;
        public uint Parry;
        public uint DodgeRate;//Finish
        public uint BloodthirstLevel;//Finish
        public uint EtherealLevel;//Finish
        public uint VenomLevel;//finish
        public uint ElanLevel;//finish
        public uint SolidLevel;//Finish
        public uint SweepLevel;//finish
        public uint HitRate;//finish
        public uint HawkeyeLevel;//finish
        public uint EdgeLevel;
        public uint MeditationLevel;
        public uint Superpower;
        public uint Oracle;
        public uint Numb;
        public uint Frost;
        public uint Bash;
        public uint Luck;
        public uint Crack;
        public uint Vigour;
        public uint Safeguard;
        public uint Discipline;
        public uint Demolition;
        public uint Resist;
        [PacketAttribute(GamePackets.MsgPlayerAttriInfo)]
        private unsafe static void MsgStautssHandler(Client.GameClient user, ServerSockets.Packet stream)
        {
            uint UID;
            stream.GetStatus(out UID);

            if (user.Player.UID == UID)
            {
                var info = new MsgUserAbilityScore.UserAbilityScore();
                info.type = 0;
                info.Level = user.Player.Level;
                info.UID = user.Player.UID;
                info.Items = new MsgUserAbilityScore.AbilityScore[user.PrestigePoints.Length];
                for (int x = 0; x < user.PrestigePoints.Length; x++)
                {
                    info.Items[x] = new MsgUserAbilityScore.AbilityScore();
                    info.Items[x].Position = (uint)(x + 1);
                    info.Items[x].Points = user.PrestigePoints[x];
                }
                user.Send(stream.UserAbilityScoreCreate(info));
                user.Send(stream.StatusCreate(user.Status));
            }
            else
            {
                Client.GameClient client;
                if (Pool.GamePoll.TryGetValue(UID, out client))
                {
                    var info = new MsgUserAbilityScore.UserAbilityScore();
                    info.type = 1;
                    info.Level = client.Player.Level;
                    info.UID = client.Player.UID;
                    info.Items = new MsgUserAbilityScore.AbilityScore[client.PrestigePoints.Length];
                    for (int x = 0; x < client.PrestigePoints.Length; x++)
                    {
                        info.Items[x] = new MsgUserAbilityScore.AbilityScore();
                        info.Items[x].Position = (uint)(x + 1);
                        info.Items[x].Points = client.PrestigePoints[x];
                    }
                    user.Send(stream.UserAbilityScoreCreate(info));
                    user.Send(stream.StatusCreate(client.Status));
                }
            }
        }
    }
}
