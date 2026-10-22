using System;
using Qahr.Mobile.Game.MsgServer;

namespace Qahr.Mobile.Game.MsgNpc
{
    // QahrMobile: scripts for the NPCs placed from the official Conquer Online Mobile video (Data\Npcs.txt uids
    // 9600001-9600016), plus the two older uids that the mobile now draws as other NPCs:
    //   5600 (was MountTrainer)  -> HeavenlyMaster <TowerofMystery> at Twin City 427,405
    //   9072 in the Market       -> Dain <LineageGuardian> in front of the RebirthMaster script
    // Titles come from the mobile npc.ini / npcx.ini "Note" lines; texts marked "mobile text" are copied from the mobile
    // client's luaui\publicfiles\wordfiles\npcdialoguetext_en.dat (CMyEncryptFile seed 9527).
    //
    // Routing: Procesor.ExecuteNpcInvoker.OnDequeue calls MobileVideoNpc(...) before the NpcAttribute lookup, so these
    // scripts work without a restart (the NpcAttribute registry is built once at server start). The attributes below
    // only matter after a restart, for the code paths that use the registry directly.
    public static class MobileVideoNpcIds
    {
        public const uint
            UserAgreement = 9600001,
            TaoistYun = 9600002,
            DragonFurnace = 9600003,
            WardenZhang = 9600004,
            Phoebe = 9600005,
            Aeneas = 9600006,
            DarkFurnace = 9600007,
            SarutobiSasuke = 9600008,
            CityGuard = 9600009,
            ViceCaptainDuan = 9600010,
            Rowland = 9600011,
            MagicArtisan = 9600012,
            SilverOneArmedBandit = 9600013,
            EquipmentSwapper = 9600014,
            Simon = 9600015,
            Suyi = 9600016,
            HeavenlyMasterTC = 5600,
            Dain = 9072;
    }

    public unsafe partial class NpcHandler
    {
        /// <summary>Returns true when the npc id is one of the mobile-video NPCs and its script ran.</summary>
        public static bool MobileVideoNpc(Client.GameClient client, ServerSockets.Packet stream, uint npcid, byte option, string input)
        {
            switch (npcid)
            {
                case MobileVideoNpcIds.UserAgreement: MobileUserAgreement(client, stream, option, input, npcid); return true;
                case MobileVideoNpcIds.TaoistYun: MobileTaoistYun(client, stream, option, input, npcid); return true;
                case MobileVideoNpcIds.DragonFurnace: MobileDragonFurnace(client, stream, option, input, npcid); return true;
                case MobileVideoNpcIds.WardenZhang: MobileWardenZhang(client, stream, option, input, npcid); return true;
                case MobileVideoNpcIds.Phoebe: MobilePhoebe(client, stream, option, input, npcid); return true;
                case MobileVideoNpcIds.Aeneas: MobileAeneas(client, stream, option, input, npcid); return true;
                case MobileVideoNpcIds.DarkFurnace: MobileDarkFurnace(client, stream, option, input, npcid); return true;
                case MobileVideoNpcIds.SarutobiSasuke: MobileSarutobiSasuke(client, stream, option, input, npcid); return true;
                case MobileVideoNpcIds.CityGuard: MobileCityGuard(client, stream, option, input, npcid); return true;
                case MobileVideoNpcIds.ViceCaptainDuan: MobileViceCaptainDuan(client, stream, option, input, npcid); return true;
                case MobileVideoNpcIds.Rowland: MobileRowland(client, stream, option, input, npcid); return true;
                case MobileVideoNpcIds.MagicArtisan: MobileMagicArtisan(client, stream, option, input, npcid); return true;
                case MobileVideoNpcIds.SilverOneArmedBandit: MobileSilverOneArmedBandit(client, stream, option, input, npcid); return true;
                case MobileVideoNpcIds.EquipmentSwapper: MobileEquipmentSwapper(client, stream, option, input, npcid); return true;
                case MobileVideoNpcIds.Simon: MobileSimon(client, stream, option, input, npcid); return true;
                case MobileVideoNpcIds.Suyi: MobileSuyi(client, stream, option, input, npcid); return true;
                case MobileVideoNpcIds.HeavenlyMasterTC:
                    // uid 5600 stands at Twin City 427,405 and is drawn as HeavenlyMaster <TowerofMystery>.
                    if (client.Player.Map != 1002) return false;
                    HeavenlyMaster(client, stream, option, input, npcid);
                    return true;
                case MobileVideoNpcIds.Dain:
                    // Only the Market copy of 9072 is drawn as Dain; the Twin City copy stays the plain RebirthMaster.
                    if (client.Player.Map != 1036) return false;
                    MobileDain(client, stream, option, input, npcid);
                    return true;
            }
            return false;
        }

        // UserAgreement (mobile npcx 6322). Same text as the login popup (UserAgreementDialog), but clicking the NPC
        // in town never disconnects: "Close" just closes the window.
        [NpcAttribute((NpcID)MobileVideoNpcIds.UserAgreement)]
        public static void MobileUserAgreement(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("~END USER AGREEMENT~");
                        data.AddText("1. By playing on this server you agree to follow the Rules of Conduct.");
                        data.AddText("2. Cheating, hacking, botting or exploiting bugs is strictly forbidden.");
                        data.AddText("3. Buying, selling or sharing accounts is at your own risk.");
                        data.AddText("4. The staff may suspend or ban any account that breaks these rules.");
                        data.AddText("5. Your game data may be reset or adjusted for maintenance/balance.");
                        if (UserAgreementDB.HasAgreed(client.Player.UID))
                            data.AddText(" You have already accepted this Agreement.");
                        else
                            data.AddOption("I Agree.", 1);
                        data.AddOption("Close.", 255);
                        data.AddAvatar(30);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        UserAgreementDB.SetAgreed(client.Player.UID);
                        client.CreateBoxDialog("Thank you for accepting the Agreement. Enjoy the game!");
                        break;
                    }
            }
        }

        // TaoistYun (mobile npc.ini NpcType366, no title). No server data or script exists for him in either client.
        [NpcAttribute((NpcID)MobileVideoNpcIds.TaoistYun)]
        public static void MobileTaoistYun(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            if (Option == 0)
            {
                data.AddText("The Tao follows nature, young one. Calm your mind before you go out to fight.")
                    .AddText(" I have nothing to teach you yet. Come back to me later.")
                    .AddOption("I see.", 255)
                    .AddAvatar(30).FinalizeDialog();
            }
        }

        // DragonFurnace <Upgrade Anima's Phase> (mobile npc.ini NpcType1537/2285/3439). The PC Anima furnace script
        // (NpcID.DragonFurnaceTwin 23736) is listed in Procesor.MaintinaceNpc, so the feature is closed here too.
        [NpcAttribute((NpcID)MobileVideoNpcIds.DragonFurnace)]
        public static void MobileDragonFurnace(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            if (Option == 0)
            {
                data.AddText("This ancient furnace contains wonder energy that may enhance your Anima.")
                    .AddText(" Animae are formed by the spirits of all creatures and empowered by the universe.")
                    .AddText(" The furnace is cold for now: Anima forging, fusing and phase upgrades are not open on this server yet.")
                    .AddOption("I see.", 255)
                    .AddAvatar(30).FinalizeDialog();
            }
        }

        // WardenZhang (mobile npcx 6511): the Twin City bounty warden, same role as the PC NpcID.WardenZhang (1002).
        // Bounty = the CPs a jailed murderer paid as bail to JailWarden (Pool.TheCrimePoll, keyed by the killer uid).
        // Note: the PC script subtracts the bounty (ConquerPoints -=); this one pays it out as its text says.
        [NpcAttribute((NpcID)MobileVideoNpcIds.WardenZhang)]
        public static void MobileWardenZhang(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("We're always in need of fresh bounty hunters. Murderers and thieves can evade our guards.")
                            .AddText(" If you PK those whose names are black (known criminals), you might get rewarded, if they pay their bail.")
                            .AddOption("I want to claim my reward.", 1)
                            .AddOption("None of my business.", 255)
                            .AddAvatar(37).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.InTrade) return;
                        Database.TheCrimeTable crimes;
                        if (Pool.TheCrimePoll.TryGetValue(client.Player.UID, out crimes))
                        {
                            string killerName;
                            int cps = crimes.Claim(out killerName);
                            if (crimes.Count == 0)
                                Pool.TheCrimePoll.TryRemove(client.Player.UID, out crimes);
                            if (cps > 0)
                            {
                                client.Player.ConquerPoints += (uint)cps;
                                data.AddText("For the arrest of " + killerName + ", you are awarded " + cps + " ConquerPoints.")
                                    .AddOption("Thanks.", 255)
                                    .AddAvatar(37).FinalizeDialog();
                                break;
                            }
                        }
                        data.AddText("No murderer you slew has paid bail yet. Only when he pays can you claim the bounty.")
                            .AddOption("I see.", 255)
                            .AddAvatar(37).FinalizeDialog();
                        break;
                    }
            }
        }

        // Phoebe <Masquerade> (mobile npcx 5137). The Masquerade event does not exist on the server.
        [NpcAttribute((NpcID)MobileVideoNpcIds.Phoebe)]
        public static void MobilePhoebe(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            if (Option == 0)
            {
                data.AddText("Welcome, guest! Behind a mask, everyone can be whoever they wish to be.")
                    .AddText(" The Masquerade is not being held right now. I'll call for you when the ball begins.")
                    .AddOption("I'll be waiting.", 255)
                    .AddAvatar(30).FinalizeDialog();
            }
        }

        // Aeneas <SpearOfTrojan> (mobile npcx 5624). Mobile text: an attendant of the Valhalla, like Sarutobi Sasuke.
        // The Valhalla / Susanoo Trial content is not on the server; for Trojans he opens the existing Epic Trojan
        // quest script (NpcID.SwapperStarry 832663, which has no NPC placed). Its options 1/2/3/10/12 come back here.
        [NpcAttribute((NpcID)MobileVideoNpcIds.Aeneas)]
        public static void MobileAeneas(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I am Aeneas, an attendant of the Valhalla. The spear of the Trojans has guarded these lands since the Dark Era.")
                            .AddText(" The Valhalla trials are still sealed, but a true Trojan can already walk the path of the Epic Trojan.");
                        if (Database.AtributesStatus.IsTrojan(client.Player.Class))
                            data.AddOption("Tell me about the Epic Trojan quest.", 100);
                        data.AddOption("Goodbye.", 255)
                            .AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 100:
                    {
                        SwapperStarry(client, stream, 0, Input, id);
                        break;
                    }
                default:
                    {
                        SwapperStarry(client, stream, Option, Input, id);
                        break;
                    }
            }
        }

        // Dark Furnace <Dark Furnace> (mobile npc.ini NpcType138). It carries a quest marker in the video; the quest
        // line behind it is not on the server.
        [NpcAttribute((NpcID)MobileVideoNpcIds.DarkFurnace)]
        public static void MobileDarkFurnace(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            if (Option == 0)
            {
                data.AddText("A dark flame flickers inside the furnace, but it gives off no heat.")
                    .AddText(" Whatever it was built to forge, it is not ready to be used yet.")
                    .AddOption("Leave it.", 255)
                    .AddAvatar(30).FinalizeDialog();
            }
        }

        // SarutobiSasuke <MartyrServant> (mobile npcx 5015). Mobile text: works in Ninja's Valhalla, a Martyr Servant;
        // the Trial of Legends tokens (itemtype 3305571-3305632) exist but the trial is not implemented.
        [NpcAttribute((NpcID)MobileVideoNpcIds.SarutobiSasuke)]
        public static void MobileSarutobiSasuke(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I am Sarutobi Sasuke. I serve in Ninja's Valhalla as a Martyr Servant.")
                            .AddText(" After becoming a Martyr Servant, one can establish a connection with the spirits, which grants us longevity.")
                            .AddOption("What is the Susanoo Trial?", 1)
                            .AddOption("Goodbye.", 255)
                            .AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("In the Valhalla of Ninja, the tradition of the Susanoo Trial still remains. Aeneas and I want to extend it to all classes.")
                            .AddText(" The Valhalla is still sealed. When the Trial of Legends opens, I will be the one to guide you.")
                            .AddOption("I'll wait.", 255)
                            .AddAvatar(30).FinalizeDialog();
                        break;
                    }
            }
        }

        // CityGuard (mobile npc.ini NpcType1466) at the Twin City south road.
        [NpcAttribute((NpcID)MobileVideoNpcIds.CityGuard)]
        public static void MobileCityGuard(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            if (Option == 0)
            {
                data.AddText("Halt! I guard the gate of Twin City. Keep the peace inside the walls.")
                    .AddText(" Those who murder others get black names and end up in jail. Warden Zhang pays a bounty for them.")
                    .AddOption("Understood.", 255)
                    .AddAvatar(30).FinalizeDialog();
            }
        }

        // ViceCaptainDuan (mobile npcx 6304), next to the CityGuard.
        [NpcAttribute((NpcID)MobileVideoNpcIds.ViceCaptainDuan)]
        public static void MobileViceCaptainDuan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            if (Option == 0)
            {
                data.AddText("I am Duan, vice captain of the Twin City guards. My men watch every road into the city.")
                    .AddText(" If you see monsters near the gate, report them to me. For now, all is quiet.")
                    .AddOption("Good to hear.", 255)
                    .AddAvatar(30).FinalizeDialog();
            }
        }

        // Rowland <Realm Guidance> (mobile npc.ini NpcType1093) at the Twin City south gate. Same NPC as the Market
        // Rowland (uid 45 = NpcID.MarketConductress); in Twin City he does the TwinCityConductress (10050) teleports.
        [NpcAttribute((NpcID)MobileVideoNpcIds.Rowland)]
        public static void MobileRowland(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            TwinCityConductress(client, stream, Option, Input, id);
        }

        // MagicArtisan (mobile npcx 5261) in the Market: the PC MagicArtisan <Degrade> is uid 9071
        // (NpcID.EquipmentBlacksmith), which opens the equipment degrade window.
        [NpcAttribute((NpcID)MobileVideoNpcIds.MagicArtisan)]
        public static void MobileMagicArtisan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            EquipmentBlacksmith(client, stream, Option, Input, id);
        }

        // SilverOne-armedBandit (mobile npc.ini NpcType1977). The slot window is opened by the client itself only for
        // NPC uids listed in ini\slotnpc.ini (9817 = 10000 silver; 9826-9828 = 3/10/100 CPs; ...) with the slot-machine
        // NPC type; the spin itself is MsgMachine.cs (MsgSlotAction). uid 9600013 / type 2 is neither, so the click
        // arrives here as a normal NPC talk.
        [NpcAttribute((NpcID)MobileVideoNpcIds.SilverOneArmedBandit)]
        public static void MobileSilverOneArmedBandit(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            if (Option == 0)
            {
                data.AddText("Silver One-armed Bandit: 10,000 silver per spin. Three matching symbols win the jackpot!")
                    .AddText(" The machine is being serviced and cannot take bets right now.")
                    .AddOption("Maybe later.", 255)
                    .AddAvatar(30).FinalizeDialog();
            }
        }

        // EquipmentSwapper (mobile npcx 5217) in the Market. No equipment swap feature exists on the server.
        [NpcAttribute((NpcID)MobileVideoNpcIds.EquipmentSwapper)]
        public static void MobileEquipmentSwapper(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            if (Option == 0)
            {
                data.AddText("I am the Equipment Swapper. I trade gear for gear, so that your equipment always suits your path.")
                    .AddText(" My counter is not open yet. Please come back later.")
                    .AddOption("All right.", 255)
                    .AddAvatar(30).FinalizeDialog();
            }
        }

        // Simon (mobile npcx 6486) at Twin City 430,404: same NPC as the PC Simon (uid 1152, NpcID.Simon, Labyrinth).
        [NpcAttribute((NpcID)MobileVideoNpcIds.Simon)]
        public static void MobileSimon(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Simon(client, stream, Option, Input, id);
        }

        // Suyi <Pitch Glory> (mobile npcx 6464): the Cuju (Pitch Glory) event host. Idle line and answer are mobile
        // text STR_NPCDIALOGUE_103559401_1 / STR_DLGSELECT_103559401_1_1. The Cuju event is not on the server.
        [NpcAttribute((NpcID)MobileVideoNpcIds.Suyi)]
        public static void MobileSuyi(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Cuju chases the wind. Winning and losing are both a joy.")
                            .AddOption("You are truly open-minded!", 1)
                            .AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        data.AddText("The Pitch Glory matches have not started yet. When they do, come and kick a ball with us!")
                            .AddOption("I will.", 255)
                            .AddAvatar(30).FinalizeDialog();
                        break;
                    }
            }
        }

        // Dain <LineageGuardian> (mobile npc.ini NpcType3448) = Market copy of uid 9072. A short Dain page (mobile text
        // STR_NPCDIALOGUE_36546_*), then the existing RebirthMaster script, whose options all pass through. The ids
        // 110-113 are not used by RebirthMaster.
        public static void MobileDain(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                case 113:
                    {
                        data.AddText("Dear the chosen one, Dain, the 6th leader of Wingley Family, is at your service. May I know why you're here?")
                            .AddOption("What is Lineage?", 110)
                            .AddOption("The past of Rebirth Master and Dain.", 111)
                            .AddOption("Rebirth.", 112)
                            .AddOption("Nothing.", 255)
                            .AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 110:
                    {
                        data.AddText("Lineage exists in your body in the form of Omnistone once your class is settled. Its power will be activated when you reach the required level.")
                            .AddText(" The Omnistone is the carrier of Lineage, and every class has already benefited from it when they step on the path.")
                            .AddOption("Back.", 113)
                            .AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 111:
                    {
                        data.AddText("In the Dark Era, my family was chosen by Lord Eloise to be the Lineage Guardians. A few years ago, my father died in an accident.")
                            .AddText(" Actually, Solon Wingley was selected by Lord Eloise as a guide for rebirth long ago. You must have encountered him before.")
                            .AddText(" The core knowledge related to Omnistones has always been passed down exclusively to the family leader.")
                            .AddOption("Back.", 113)
                            .AddAvatar(30).FinalizeDialog();
                        break;
                    }
                case 112:
                    {
                        RebirthMaster(client, stream, 0, Input, id);
                        break;
                    }
                default:
                    {
                        RebirthMaster(client, stream, Option, Input, id);
                        break;
                    }
            }
        }
    }
}
