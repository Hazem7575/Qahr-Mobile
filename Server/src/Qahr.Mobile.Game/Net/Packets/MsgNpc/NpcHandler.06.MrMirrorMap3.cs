using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.MsgInterServer.Packets;
using Qahr.Mobile.Game.MsgFloorItem;
using Qahr.Mobile.DBFunctionality;
using Qahr.Mobile.Game.MsgTournaments;
using System.IO;
using System.Net.Mail;
using Qahr.Mobile.Database;
using Qahr.Mobile.Role.Instance;

using Qahr.Mobile.Game.MsgNpc;

using Qahr.Mobile.Role;

namespace Qahr.Mobile.Game.MsgNpc
{
    public unsafe partial class NpcHandler
    {


        [NpcAttribute(NpcID.MrMirrorMap3)]
        public static void MrMirrorMap3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("It`s~the~chance~for~miracle.");
                        data.AddOption("Great!");
                        data.AddAvatar(247);
                        data.FinalizeDialog();
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.DivineJadeMap3)]
        public static void DivineJadeMap3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("This~clear~jade~glows~with~a~wonderful~light.~Infusing~the~Solar~Essences~into~this~jade~will~considerably~weaken~the~Flame~Devastator.");
                        if (client.Inventory.Contain(3003340, 1) && client.Player.StageEpicTrojanQuest >= 50)
                        {
                            data.AddOption("Enter~the~array.", 1);
                        }
                        else
                        {
                            data.AddOption("Leave.");
                        }
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Player.MessageBox("After~you~kill~the~Flame~Devastator,~your~universal~1-handed~weapon~will~turn~into~an~Epic~Weapon!", new Action<Client.GameClient>(p =>
                        {
                            var map = Pool.ServerMaps[10];
                            p.Teleport(40, 40, 10);
                            if (p.Player.StageEpicTrojanQuest == 50)
                            {
                                if (!map.ContainMobID(7485))
                                {
                                    using (var recBox = new ServerSockets.RecycledPacket()) Server.AddMapMonster(recBox.GetStream(), p.Map, 7485, p.Player.X, p.Player.Y, 3, 3, 1, p.Player.DynamicID, false, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "SuperXp-4");
                                }
                                p.SendSysMesage("The~roaring~of~the~Flame~Devastator~make~the~earth~shaking.~Go~and~test~its~power!");
                            }
                            else if (p.Player.StageEpicTrojanQuest == 51)
                            {
                                if (!map.ContainMobID(7486))
                                {
                                    using (var recBox = new ServerSockets.RecycledPacket()) Server.AddMapMonster(recBox.GetStream(), p.Map, 7486, p.Player.X, p.Player.Y, 3, 3, 1, p.Player.DynamicID, false, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "SuperXp-4");
                                }
                                p.SendSysMesage("The~roaring~of~the~Flame~Devastator~make~the~earth~shaking.~Go~and~test~its~power!");
                            }
                            else if (p.Player.StageEpicTrojanQuest == 52)
                            {
                                if (!map.ContainMobID(7487))
                                {
                                    using (var recBox = new ServerSockets.RecycledPacket()) Server.AddMapMonster(recBox.GetStream(), p.Map, 7487, p.Player.X, p.Player.Y, 3, 3, 1, p.Player.DynamicID, false, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "SuperXp-4");
                                }
                            }

                        }), null);

                        break;
                    }
            }
        }

        [NpcAttribute(NpcID.GeneralPakMap3)]
        public static void GeneralPakMap3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPakMap3, 0, 3280);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPakMap3, 0, 3274);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.StageEpicTrojanQuest >= 50)
                        {
                            if (client.Inventory.Contain(3003340, 1))
                            {
                                data.AddText("It`s~the~end.~Though~the~Flame~Devastator~was~weakened,~it`ll~still~fierce~enough~to~smash~the~world.");
                                data.AddText("~Look,~you~need~awaken~the~Solar~Blade`s~spirit,~and~then~combine~the~spirit~with~your~main~weapon~(must~be~universal~one-handed)~to~create~an~Epic~Weapon.");
                                data.AddText("~With~the~power~of~the~Epic~Weapon,~we~may~behead~the~Flame~Devastator.");
                                data.AddOption("I`ll~take~care.", 2);
                                data.AddAvatar(245);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The~Flame~Devastator~has~been~weakened,~but~I`m~afraid~normal~weapons~can`t~wound~this~demon.");
                                data.AddText("~Take~my~Solar~Blade!~If~you`re~able~to~awaken~its~spirit,~and~combine~the~spirit~with~your~main~weapon~(must~be~universal~one-handed)");
                                data.AddText("~to~create~an~Epic~Weapon,~you`ll~achieve~a~divine~power.");
                                data.AddOption("I~see.", 3);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("What~a~miracle!~The~Solar~signed~a~blood~contract~with~you!~But~for~this~reason,~the~Flame~Devastator~can~be~eliminated.");
                            data.AddText("~Now,~only~you~can~summon~the~spirit~of~Solar~Blade.~From~now~on,~you`re~the~owner~of~this~Epic~Weapon!");
                            data.AddOption("Go~back~to~300~years~later.", 1);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Inventory.HaveSpace(1))
                        {
                            client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId);
                            client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                            client.Inventory.Add(stream, 3003340);
                            client.Player.QuestGUI.SendAutoPatcher("The~Solar~Blade~is~already~in~your~inventory.~Make~good~use~of~it~to~eliminate~the~Flame~Devastator!", 3832, 101, 175, 10605);
                        }
                        else
                        {
                            data.AddText("You~inventory~is~full.~Make~some~room~for~the~Solar~Blade,~first.");
                            data.AddOption("I`ll~do~it~now.");
                            data.AddAvatar(245);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(3832, 101, 176, 10605);
                        break;
                    }
                case 1:
                    {
                        client.Teleport(Places.TwinCity);
                        client.CreateBoxDialog("You`re~in~Twin~City,~300~years~ago.");
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.DivineJadeMap2)]
        public static void DivineJadeMap2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPakMap2, 0, 3279);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPakMap3, 0, 3280);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("This~clear~jade~glows~with~a~wonderful~light.~Infusing~the~Solar~Essences~into~this~jade~will~considerably~weaken~the~Flame~Devastator.");
                        data.AddOption("Use~the~Solar~Essences.", 1);
                        data.FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        if (client.Inventory.Contain(3003338, 99))
                        {
                            client.Player.StageEpicTrojanQuest = 50;
                            client.Inventory.RemoveStackItem(3003338, 99, stream);
                            client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);
                            client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                            client.Teleport(client.Player.X, client.Player.Y, 3832);
                            client.Player.AddMapEffect(stream, 101, 175, "fire3");
                            client.Player.QuestGUI.SendAutoPatcher("You`ve~successfully~weakened~the~Flame~Devastator!~Hurry~and~report~back~to~General~Pack!", 3832, 154, 130, 10603);
                        }
                        else
                        {
                            data.AddText("You~should~have~99~Solar~Essences~before~you~can~weaken~the~Flame~Devastator.");
                            data.AddOption("I~see.", 2);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(3831, 154, 130, 10600);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GeneralPakMap2)]
        public static void GeneralPakMap2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPakMap2, 0, 3273);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPakMap2, 0, 3279);


            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        if (client.Player.StageEpicTrojanQuest >= 40)
                        {
                            if (client.Player.StageEpicTrojanQuest >= 41)
                            {
                                if (client.Inventory.Contain(3003338, 99))
                                {
                                    client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                    client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                    data.AddText("You`ve~extracted~99~Solar~Essences.~Hurry~and~go~weaken~the~Flame~Devastator~through~the~Divine~Jade!");
                                    data.AddOption("Head~to~the~Divine~Jade.", 40);
                                    data.FinalizeDialog();
                                    break;
                                }
                                if (client.Player.CanChangGeneralPakMaterial == 1)
                                {
                                    if (client.Player.EpicTrojanGeneralPakItemChange[0] == 0)
                                        client.Player.EpicTrojanGeneralPakItemChange = GetItemArrayEpicTrojanQuest();

                                    data.AddText("Trojan`s~blood~is~the~purest~solar~medium.~Combined~with~some~other~materials,~we~can~extract~the~Solar~Essences~from~the~blood.");
                                    data.AddOption("Extract~the~Solar~Essences.", 5);
                                    data.AddOption("Go~back~to~300~years~later.", 1);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~use~CPs,~instead~of~Trojan`s~blood,~to~continue~the~extracting.");
                                    data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanGeneralPakItemChange) + "\n~~~~~~~~~~~~~~~200~Study~Points\n~~~[Change~Times]~" + client.Player.ChangeGeneralPakEpicTrojan + "\n~~------------------------------------------------------~\n\n");
                                    data.AddOption("Extract~with~CPs.", 30);
                                    data.AddOption("Change~the~material.~(3~CPs)", 10);
                                    data.AddOption("Go~back~to~300~years~later.", 1);
                                    data.AddOption("I`ll~think~about~it.");
                                    data.FinalizeDialog();
                                }

                            }
                            else
                            {
                                data.AddText("Actually,~Monk~Misery~has~spent~too~much~energy~in~sacrificing~creature`s~souls~to~the~Flame~Devastator,~so~you~defeated~him~easily.");
                                data.AddText("~However,~he~sacrificed~his~own~soul~to~the~Devastator,~and~enabled~that~demon~to~break~through~the~array.");
                                data.AddOption("It`s~too~bad.", 3);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Hurry~and~go~defeat~Monk~Misery~at~the~Evil~Array!");
                            data.AddOption("I`m~on~the~way.", 1);
                            data.AddOption("Go~back~to~300~years~later.", 2);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Player.CanChangGeneralPakMaterial == 1)
                        {
                            data.AddText("Give~me~a~drop~of~Trojan`s~blood,~and~some~other~materials,~so~I~can~extract~4~Solar~Essences.~It`ll~take~25~days~to~extract~enough~essences.\n\n");
                            data.AddText("~~~~Extracting~Material:~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanGeneralPakItemChange) + "\n~~~~~~~~~~~~~~200~Study~Points\n\n");
                            data.AddOption("Extract~Solar~Essences.", 6);
                            data.AddOption("Change~the~material.", 7);
                            data.AddOption("Leave.");
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(client.Player.EpicTrojanGeneralPakItemChange[0], client.Player.EpicTrojanGeneralPakItemChange[1]))
                        {
                            if (client.Inventory.HaveSpace(1) && client.Player.CanChangGeneralPakMaterial == 1)
                            {
                                client.Player.CanChangGeneralPakMaterial = 0;
                                client.Inventory.Remove(client.Player.EpicTrojanGeneralPakItemChange[0], client.Player.EpicTrojanGeneralPakItemChange[1], stream);
                                client.Inventory.AddItemWitchStack(3003338, 0, 4, stream);
                                client.CreateBoxDialog("You~successfully~extracted~4~Solar~Essences~from~Trojan`s~blood.");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "meteor1");
                                if (client.Inventory.Contain(3003338, 99))
                                {
                                    client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                    client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                    data.AddText("You`ve~extracted~99~Solar~Essences.~Hurry~and~go~weaken~the~Flame~Devastator~through~the~Divine~Jade!");
                                    data.AddOption("Head~to~the~Divine~Jade.", 40);
                                    data.FinalizeDialog();
                                    break;
                                }
                                data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~use~CPs,~instead~of~Trojan`s~blood,~to~continue~the~extracting.");
                                data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanGeneralPakItemChange) + "\n~~~~~~~~~~~~~~~200~Study~Points\n~~~[Change~Times]~" + client.Player.ChangeGeneralPakEpicTrojan + "\n~~------------------------------------------------------~\n\n");
                                data.AddOption("Extract~with~CPs.", 30);
                                data.AddOption("Change~the~material.~(3~CPs)", 10);
                                data.AddOption("Go~back~to~300~years~later.", 1);
                                data.AddOption("I`ll~think~about~it.");
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You`re~carrying~a~full~inventory.~Make~some~room,~first.");
                                data.AddOption("Okay.");
                                data.FinalizeDialog();
                            }

                        }
                        else
                        {
                            data.AddText("You~don`t~have~enough~materials~to~extract~the~Solar~Essences.\n\n~~~~Extracting~Material:~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanGeneralPakItemChange) + ",~200~Study~Points.");
                            data.AddOption("Alright.");
                            data.AddOption("Change~the~material.", 7);
                            data.AddOption("Entrust~the~work.", 8);
                            data.AddAvatar(245);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        data.AddText("~~~~~~~~~~~~~~~~~~Extracting~the~Solar~Essences\n~~Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½\n");
                        data.AddText("~~~[Extracting~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanGeneralPakItemChange) + "\n~~~~~~~~~~~~~~~200~Study~Points\n~~~[Change~Times]~" + client.Player.ChangeGeneralPakEpicTrojan + "\n~~Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½\n\n");
                        data.AddOption("Extract~the~Solar~Essences.", 5);
                        data.AddOption("Change~the~material.~(3~CPs)", 10);
                        data.AddOption("Entrust~the~work.~(36~CPs)", 8);
                        data.AddOption("Leave.");
                        data.FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.ConquerPoints < 3)
                        {
                            data.AddText("Buddy,~I~can`t~help~you~if~you~don`t~have~enough~CPs.");
                            data.AddOption("Alright.");
                            data.AddAvatar(245);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.InTrade) return;
                            client.Player.ChangeGeneralPakEpicTrojan += 1;
                            client.Player.ConquerPoints -= 3;
                            client.Player.EpicTrojanGeneralPakItemChange = GetItemArrayEpicTrojanQuest();
                            if (client.Player.CanChangGeneralPakMaterial == 1)
                            {
                                data.AddText("Give~me~a~drop~of~Trojan`s~blood,~and~some~other~materials,~so~I~can~extract~4~Solar~Essences.~It`ll~take~25~days~to~extract~enough~essences.\n\n");
                                data.AddText("~~~~Extracting~Material:~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanGeneralPakItemChange) + "\n~~~~~~~~~~~~~~200~Study~Points\n\n");
                                data.AddOption("Extract~Solar~Essences.", 6);
                                data.AddOption("Change~the~material.", 7);
                                data.AddOption("Leave.");
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~use~CPs,~instead~of~Trojan`s~blood,~to~continue~the~extracting.");
                                data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanGeneralPakItemChange) + "\n~~~~~~~~~~~~~~~200~Study~Points\n~~~[Change~Times]~" + client.Player.ChangeGeneralPakEpicTrojan + "\n~~------------------------------------------------------~\n\n");
                                data.AddOption("Extract~with~CPs.", 30);
                                data.AddOption("Change~the~material.~(3~CPs)", 10);
                                data.AddOption("Go~back~to~300~years~later.", 1);
                                data.AddOption("I`ll~think~about~it.");
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 8:
                    {
                        data.AddText("I`ll~charge~36~CPs~for~collecting~the~extracting~materials,~except~Trojan`s~blood.~So,~what~you~say?");
                        data.AddOption("Do~the~job~for~me!~(36~CPs)", 9);
                        data.AddOption("Wait,~I~haven`t~decided.");
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 9:
                    {
                        if (client.Player.CanChangGeneralPakMaterial == 1)
                        {
                            if (client.Player.ConquerPoints < 36)
                            {
                                data.AddText("Buddy,~I~can`t~help~you~if~you~don`t~have~enough~CPs.");
                                data.AddOption("Alright.");
                                data.AddAvatar(245);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Inventory.HaveSpace(1))
                                {
                                    if (client.InTrade) return;
                                    client.Player.CanChangGeneralPakMaterial = 0;
                                    client.Player.ConquerPoints -= 36;
                                    client.Inventory.AddItemWitchStack(3003338, 0, 4, stream);
                                    client.CreateBoxDialog("You~successfully~extracted~4~Solar~Essences~from~Trojan`s~blood.");
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "meteor1");
                                    if (client.Inventory.Contain(3003338, 99))
                                    {
                                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                        data.AddText("You`ve~extracted~99~Solar~Essences.~Hurry~and~go~weaken~the~Flame~Devastator~through~the~Divine~Jade!");
                                        data.AddOption("Head~to~the~Divine~Jade.", 40);
                                        data.FinalizeDialog();
                                        break;
                                    }
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~use~CPs,~instead~of~Trojan`s~blood,~to~continue~the~extracting.");
                                    data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanGeneralPakItemChange) + "\n~~~~~~~~~~~~~~~200~Study~Points\n~~~[Change~Times]~" + client.Player.ChangeGeneralPakEpicTrojan + "\n~~------------------------------------------------------~\n\n");
                                    data.AddOption("Extract~with~CPs.", 30);
                                    data.AddOption("Change~the~material.~(3~CPs)", 10);
                                    data.AddOption("Go~back~to~300~years~later.", 1);
                                    data.AddOption("I`ll~think~about~it.");
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You`re~carrying~a~full~inventory.~Make~some~room,~first.");
                                    data.AddOption("Okay.");
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 30:
                    {
                        data.AddText("Currently,~you`ve~totally~extracted~" + client.Inventory.GetCountItem(3003338) + "~Solar~Essence(s).~You~need~to~collect~99~Solar~Essences~to~weaken~the~Flame~Devastator.");
                        data.AddOption("Extract~1~essence.~(90CPs)", 31);
                        if (client.Inventory.GetCountItem(3003338) < 82)
                            data.AddOption("Extract~18~essences.~(1600CPs)", 33);
                        else
                        {
                            data.AddOption("Leave.");
                            data.AddAvatar(245);
                        }
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        data.AddText("It`ll~cost~you~90~CPs~to~extract~a~Solar~Essence~directly~with~CPs.~Continue?");
                        data.AddOption("Yes.~Here~are~90~CPs.", 32);
                        data.AddOption("I~need~to~think~about~it.");
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 33:
                    {
                        data.AddText("It`ll~cost~you~1,600~CPs~to~extract~18~Solar~Essences.~Are~you~sure~you~want~to~start~extracting~by~that~way?");
                        data.AddOption("Yes!~Here`re~1,600~CPs.", 34);
                        data.AddOption("Wait,~I~haven`t~decided.");
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 34:
                    {
                        if (client.Player.ConquerPoints < 1600)
                        {
                            data.AddText("Come~on,~make~sure~you~bring~enough~CPs~with~you!");
                            data.AddOption("Alright.");
                            data.AddAvatar(245);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 1600;
                                client.CreateBoxDialog("You~successfully~extracted~18~Solar~Essence!");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "meteor1");


                                client.Inventory.AddItemWitchStack(3003338, 0, 18, stream);

                                if (client.Inventory.Contain(3003338, 99))
                                {
                                    client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                    client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                    data.AddText("You`ve~extracted~99~Solar~Essences.~Hurry~and~go~weaken~the~Flame~Devastator~through~the~Divine~Jade!");
                                    data.AddOption("Head~to~the~Divine~Jade.", 40);
                                    data.FinalizeDialog();
                                    break;
                                }

                                data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~use~CPs,~instead~of~Trojan`s~blood,~to~continue~the~extracting.");
                                data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanGeneralPakItemChange) + "\n~~~~~~~~~~~~~~~200~Study~Points\n~~~[Change~Times]~" + client.Player.ChangeGeneralPakEpicTrojan + "\n~~------------------------------------------------------~\n\n");
                                data.AddOption("Extract~with~CPs.", 30);
                                data.AddOption("Change~the~material.~(3~CPs)", 10);
                                data.AddOption("Go~back~to~300~years~later.", 1);
                                data.AddOption("I`ll~think~about~it.");
                                data.FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("Your~inventory~is~full.~Why~not~make~some~room,~and~then~come~back~to~extract~the~Solar~Essences?");
                                data.AddOption("Alright.");
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 32:
                    {
                        if (client.Player.ConquerPoints < 90)
                        {
                            data.AddText("Come~on,~make~sure~you~bring~enough~CPs~with~you!");
                            data.AddOption("Alright.");
                            data.AddAvatar(245);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.InTrade) return;
                                client.Player.ConquerPoints -= 90;
                                client.CreateBoxDialog("You~successfully~extracted~a~Solar~Essence!");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "meteor1");


                                client.Inventory.AddItemWitchStack(3003338, 0, 1, stream);

                                if (client.Inventory.Contain(3003338, 99))
                                {
                                    client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                    client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                    data.AddText("You`ve~extracted~99~Solar~Essences.~Hurry~and~go~weaken~the~Flame~Devastator~through~the~Divine~Jade!");
                                    data.AddOption("Head~to~the~Divine~Jade.", 40);
                                    data.FinalizeDialog();
                                    break;
                                }

                                data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~You~can~also~use~CPs,~instead~of~Trojan`s~blood,~to~continue~the~extracting.");
                                data.AddText("\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetArrayEpicTrojanItemStr(client.Player.EpicTrojanGeneralPakItemChange) + "\n~~~~~~~~~~~~~~~200~Study~Points\n~~~[Change~Times]~" + client.Player.ChangeGeneralPakEpicTrojan + "\n~~------------------------------------------------------~\n\n");
                                data.AddOption("Extract~with~CPs.", 30);
                                data.AddOption("Change~the~material.~(3~CPs)", 10);
                                data.AddOption("Go~back~to~300~years~later.", 1);
                                data.AddOption("I`ll~think~about~it.");
                                data.FinalizeDialog();

                            }
                            else
                            {
                                data.AddText("Your~inventory~is~full.~Why~not~make~some~room,~and~then~come~back~to~extract~the~Solar~Essences?");
                                data.AddOption("Alright.");
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 40:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(3831, 101, 176, 10602);

                        break;
                    }
                case 3:
                    {
                        data.AddText("Look,~the~Flame~Devastator~hasn`t~completely~recovered.~I~guess~we~can~only~use~the~Solar~Essences~to~weaken~his~power.");
                        data.AddOption("Let`s~start!", 4);
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        client.Player.StageEpicTrojanQuest = 41;
                        goto case 0;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(client.Player.Map, 082, 166, 0);
                        break;
                    }
                case 1:
                    {
                        client.Teleport(Places.TwinCity);
                        client.CreateBoxDialog("You`re~in~Twin~City,~300~years~ago.");
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.GeneralPak)]
        public static void GeneralPak(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {


            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PaksGhost, 0, 3275);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3270);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3276);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3271);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3277);
            var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.MonkMisery, 0, 3272);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.StageEpicTrojanQuest >= 20)
                        {
                            if (client.Player.StageEpicTrojanQuest >= 21)
                            {
                                if (client.Player.StageEpicTrojanQuest >= 30)
                                {
                                    if (client.Player.StageEpicTrojanQuest >= 31)
                                    {

                                        data.AddText("It~seems~Monk~Misery~has~united~with~the~Flame~Devastator~for~a~long~time.~They~are~getting~fiery.");
                                        data.AddText("~I`m~afraid~they`ll~carry~out~the~plan~earlier.~See,~it`s~not~wise~to~deal~with~an~unknown~thing.");
                                        data.AddText("~Go~collect~possible~information~from~Monk~Misery,~first.");
                                        data.AddOption("I`ll~go~find~Monk~Misery.", 7);
                                        data.AddAvatar(245);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        data.AddText("How~could~those~Ghost~Reavers~escape~from~the~Evil~Array~when~you~were~enhancing~its~power?");
                                        data.AddText("~The~Monk~Misery~looked~quite~suspicious~when~you~were~killing~the~Ghost~Reavers.");
                                        data.AddText("~He~was~reciting~some~strange~spells~and~talking~to~the~Flame~Devastator.");
                                        data.AddOption("Did~him?", 5);
                                        data.FinalizeDialog();
                                    }
                                }
                                else
                                {
                                    data.AddText("Though~the~Evil~Array~has~been~enhanced,~there~are~18~Ghost~Reavers~from~the~Flame~Devastator`s~army~sneaking~into~the~world.");
                                    data.AddText("~Be~careful,~those~Ghost~Reavers~are~trying~to~destroy~the~array.~They~should~die.");
                                    data.AddOption("How~to~find~them?", 3);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Though~the~Evil~Array~has~been~enhanced,~there~are~18~Ghost~Reavers~from~the~Flame~Devastator`s~army~sneaking~into~the~world.");
                                data.AddText("~Be~careful,~those~Ghost~Reavers~are~trying~to~destroy~the~array.~They~should~die.");
                                data.AddOption("How~to~find~them?", 3);
                                data.AddAvatar(245);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Years~ago,~the~Flame~Devastator,~a~vicious~demon,~terrorized~Twin~City.~At~that~time,~a~prominent~taoist~spent~out~his~power~and~built~the~Evil~Array");
                            data.AddText("~to~seal~the~gate~of~evils.~However,~with~time~goes~by,~the~Evil~Array~becomes~weak~and~the~sealed~Flame~Devastator~is~getting~unrest.");
                            data.AddText("~I`m~afraid~the~returning~of~the~Flame~Devastator~will~push~Twin~City~into~hell,~again.");
                            data.AddOption("What~can~we~do?", 1);
                            data.FinalizeDialog();
                        }
                        break;
                    }

                case 5:
                    {
                        data.AddText("What~a~shame!~I`m~the~General,~but~I~were~quite~unaware~of~that.~Anyway,~we~have~to~stop~them~no~matter~what~they~were~plotting.");
                        data.AddOption("Count~on~me!", 6);
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("It~seems~Monk~Misery~has~united~with~the~Flame~Devastator~for~a~long~time.~They~are~getting~fiery.");
                        data.AddText("~I`m~afraid~they`ll~carry~out~the~plan~earlier.~See,~it`s~not~wise~to~deal~with~an~unknown~thing.");
                        data.AddText("~Go~collect~possible~information~from~Monk~Misery,~first.");
                        data.AddOption("I`ll~go~find~Monk~Misery.", 7);
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId);
                        client.Player.QuestGUI.Accept(ActiveQuest6, 0);
                        client.Player.StageEpicTrojanQuest = 31;
                        client.Player.QuestGUI.SendAutoPatcher(3830, 082, 166, 10583);
                        break;
                    }
                case 1:
                    {
                        data.AddText("Re-enchant~the~Evil~Array.~It`s~the~only~way.~Would~you~like~to~take~this~critical~task~of~enchanting~the~array?");
                        data.AddText("~Find~Monk~Misery~at~the~Evil~Array,~and~he`ll~tell~you~what~to~do.");
                        data.AddOption("Count~on~me!", 2);
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(3830, 082, 166, (uint)NpcID.MonkMisery);
                        client.Player.StageEpicTrojanQuest = 11;
                        break;
                    }
                case 3:
                    {
                        data.AddText("Those~Ghost~Reaver~are~invisible~in~the~evil~gas.~I~heard~that~Mr.~Mirror~has~a~skill~to~reveal~those~demons.~Maybe,~you~can~ask~him~for~help.");
                        data.AddOption("Head~to~see~Mr.~Mirror.", 4);
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 4:
                    {
                        client.Player.StageEpicTrojanQuest = 21;
                        client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId);
                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
                        client.Player.QuestGUI.SendAutoPatcher(3830, 162, 217, (uint)NpcID.MrMirror);
                        break;
                    }

            }

        }

        [NpcAttribute(NpcID.MrMirror2)]
        public static void MrMirror2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3271);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.GeneralPak, 0, 3277);

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.EpicTrojanMrMirrorPrograss >= 18)
                        {
                            client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                            client.Player.MessageBox("All~the~18~Ghost~Reavers~in~the~Flame~Devastator`s~army~have~been~eliminated.~Report~back~to~General~Pak!", new Action<Client.GameClient>(p =>
                            {
                                client.Player.StageEpicTrojanQuest = 30;
                                client.Teleport(162, 218, 3830);
                                client.Player.QuestGUI.SendAutoPatcher(3830, 154, 130, 10581);

                            }), null);
                            break;
                        }
                        if (client.Player.StageEpicTrojanQuest >= 30)
                        {
                            data.AddText("Good~job!~You`ve~eliminated~18~Ghost~Reaver~in~the~army~of~the~Flame~Devastator.~Hurry~and~report~back~to~General~Pak!");
                            data.AddOption("Head~to~see~General~Pak.", 1);
                            data.AddAvatar(247);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.View.ContainMobInScreen("GhostReaver"))
                            {
                                data.AddText("The~Ghost~Reaver~has~already~appeared~in~the~Unreal~Land~of~Twin~City.~Go~eliminate~it,~first.");
                                data.AddOption("Okay.");
                                data.FinalizeDialog();
                                break;
                            }
                            if (client.Player.EpicTrijanKillGhostReaver == 0)
                            {

                                if (client.Player.EpicTrojanMr_MirrorItemChange[0] == 0)
                                    client.Player.EpicTrojanMr_MirrorItemChange = GetItemMr_MirrorEpicTrojanQuest();


                                data.AddText("So~far,~there~are~" + (18 - client.Player.EpicTrojanMrMirrorPrograss) + "~Ghost~Reavers~hiding~in~the~evil~gas.~You~need~to~provide~me~the~materials~below~to~reveal~the~Ghost~Reaver.\n\n");
                                data.AddText("~~~~Material~Required:~" + GetEpicTrojanItemStr(client.Player.EpicTrojanMr_MirrorItemChange) + "\n~~~~~~~~~~~~~~Chi~Token*1\n\n");
                                if (client.Player.CanChangeMr_MirrorEpicMaterial == 1)
                                    data.AddOption("Reveal~the~Reaver.", 2);
                                data.AddOption("Change~the~material.", 3);
                                data.FinalizeDialog();

                            }
                            else
                            {
                                if (client.Player.CanChangeMr_MirrorEpicMaterial == 1)
                                {
                                    data.AddText("The~Ghost~Reaver~hid~into~the~evil~gas,~again.~I`ll~reveal~another~Ghost~Reaver.~Seize~the~chance~and~beat~it~down!");
                                    data.AddOption("Just~do~it!", 20);
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~~You~can~also~use~320~CPs~to~call~down~a~thunder~to~kill~a~Ghost~Reaver~directly.");
                                    data.AddText("~So~far,~there~are~still~" + (18 - client.Player.EpicTrojanMrMirrorPrograss) + "~Ghost~Reavers~alive.\n\n~~------------------------------------------------------\n~~[Tomorrow`s~Material]~" + GetEpicTrojanItemStr(client.Player.EpicTrojanMr_MirrorItemChange) + "\n~~~~~~~~~~~~~~~Chi~Token*1\n~~~[Change~Times]~" + client.Player.ChangeMr_MirrorEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                    data.AddOption("Call~down~a~thunder.~(320~CPs)", 30);
                                    data.AddOption("Change~the~material.~(2~CPs)", 40);
                                    data.AddOption("I`ll~wait~till~tomorrow.", 50);
                                    data.FinalizeDialog();
                                }
                            }

                        }
                        break;
                    }
                case 20:
                    {
                        if (client.Inventory.Contain(client.Player.EpicTrojanMr_MirrorItemChange[0], client.Player.EpicTrojanMr_MirrorItemChange[1]) && client.Player.CanChangeMr_MirrorEpicMaterial == 1)
                        {
                            client.Player.CanChangeMr_MirrorEpicMaterial = 0;
                            client.Inventory.Remove(client.Player.EpicTrojanMr_MirrorItemChange[0], client.Player.EpicTrojanMr_MirrorItemChange[1], stream);
                            client.CreateBoxDialog("With~a~roaring~of~thunder,~the~Ghost~Reaver~fell~upon~the~unreal~space~of~Twin~City.~Go~kill~the~demon!");
                            Server.AddMapMonster(stream, client.Map, 7483, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "SuperXp-4");
                        }
                        else
                        {
                            data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~~You~can~also~use~320~CPs~to~call~down~a~thunder~to~kill~a~Ghost~Reaver~directly.");
                            data.AddText("~So~far,~there~are~still~" + (18 - client.Player.EpicTrojanMrMirrorPrograss) + "~Ghost~Reavers~alive.\n\n~~------------------------------------------------------\n~~[Tomorrow`s~Material]~" + GetEpicTrojanItemStr(client.Player.EpicTrojanMr_MirrorItemChange) + "\n~~~~~~~~~~~~~~~Chi~Token*1\n~~~[Change~Times]~" + client.Player.ChangeMr_MirrorEpicTrojan + "\n~~------------------------------------------------------\n\n");
                            data.AddOption("Call~down~a~thunder.~(320~CPs)", 30);
                            data.AddOption("Change~the~material.~(2~CPs)", 40);
                            data.AddOption("I`ll~wait~till~tomorrow.", 50);
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 40:
                    {
                        if (client.Player.ConquerPoints < 2)
                        {
                            data.AddText("You~don`t~have~enough~CPs~to~change~the~materials.");
                            data.AddOption("Alright.");
                            data.AddAvatar(247);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            client.Player.ChangeMr_MirrorEpicTrojan++;

                            client.Player.ConquerPoints -= 2;
                            client.Player.EpicTrojanMr_MirrorItemChange = GetItemMr_MirrorEpicTrojanQuest();
                            if (client.Inventory.Contain(client.Player.EpicTrojanMr_MirrorItemChange[0], client.Player.EpicTrojanMr_MirrorItemChange[1]) && client.Player.CanChangeMr_MirrorEpicMaterial == 1)
                            {
                                client.Player.CanChangeMr_MirrorEpicMaterial = 0;
                                client.Inventory.Remove(client.Player.EpicTrojanMr_MirrorItemChange[0], client.Player.EpicTrojanMr_MirrorItemChange[1], stream);
                                client.CreateBoxDialog("With~a~roaring~of~thunder,~the~Ghost~Reaver~fell~upon~the~unreal~space~of~Twin~City.~Go~kill~the~demon!");
                                Server.AddMapMonster(stream, client.Map, 7483, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "SuperXp-4");

                            }
                            else
                            {
                                data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.~~You~can~also~use~320~CPs~to~call~down~a~thunder~to~kill~a~Ghost~Reaver~directly.");
                                data.AddText("~So~far,~there~are~still~" + (18 - client.Player.EpicTrojanMrMirrorPrograss) + "~Ghost~Reavers~alive.\n\n~~------------------------------------------------------\n~~[Tomorrow`s~Material]~" + GetEpicTrojanItemStr(client.Player.EpicTrojanMr_MirrorItemChange) + "\n~~~~~~~~~~~~~~~Chi~Token*1\n~~~[Change~Times]~" + client.Player.ChangeMr_MirrorEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                data.AddOption("Call~down~a~thunder.~(320~CPs)", 30);
                                data.AddOption("Change~the~material.~(2~CPs)", 40);
                                data.AddOption("I`ll~wait~till~tomorrow.", 50);
                                data.FinalizeDialog();
                            }
                        }

                        break;
                    }
                case 30:
                    {
                        data.AddText("Are~you~sure~you~want~to~pay~320~CPs~to~call~down~a~thunder~to~kill~a~Ghost~Reaver?");
                        data.AddOption("Yes,~I`ve~decided.~(320~CPs)", 31);
                        data.AddOption("Wait,~I~haven`t~decided,~yet.");
                        data.AddAvatar(247);
                        data.FinalizeDialog();
                        break;
                    }
                case 31:
                    {
                        if (client.Player.ConquerPoints < 320)
                        {
                            data.AddText("I`m~afraid~I~can`t~help~you~if~you~don`t~have~27~CPs.");
                            data.AddOption("Alright.");
                            data.AddAvatar(247);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 320;
                            client.Player.EpicTrojanMrMirrorPrograss += 1;

                            MsgServer.MsgGameItem item = new MsgServer.MsgGameItem();
                            item.Color = (Role.Flags.Color)2;
                            item.ITEM_ID = 1033;
                            MsgFloorItem.MsgItem DropItem = new MsgFloorItem.MsgItem(item, 163, 228, MsgFloorItem.MsgItem.ItemType.Effect, 0, 0, client.Player.Map
                                   , 0, false, client.Map, 6);

                            if (client.Map.EnqueueItem(DropItem))
                                DropItem.SendAll(stream, MsgFloorItem.MsgDropID.Effect);

                            if (client.Player.EpicTrojanMrMirrorPrograss >= 18)
                            {
                                client.Player.StageEpicTrojanQuest = 30;
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                                client.Player.MessageBox("All~the~18~Ghost~Reavers~in~the~Flame~Devastator`s~army~have~been~eliminated.~Report~back~to~General~Pak!", new Action<Client.GameClient>(p =>
                                {


                                    client.Teleport(162, 218, 3830);
                                    client.Player.QuestGUI.SendAutoPatcher(3830, 154, 130, 10581);

                                }), null);
                            }
                            else
                            {
                                client.Player.MessageBox("A~Ghost~Reaver~fell~down~and~vanished.~So~far,~you~still~have~" + (18 - client.Player.EpicTrojanMrMirrorPrograss) + "~Ghost~Reaver(s)~to~deal~with.", new Action<Client.GameClient>(p =>
                                {

                                    using (var rec = new ServerSockets.RecycledPacket())
                                    {
                                        var pstream = rec.GetStream();
                                        client.ActiveNpc = (uint)Game.MsgNpc.NpcID.MrMirror2;
                                        Game.MsgNpc.NpcHandler.MrMirror2(client, pstream, 0, "", 0);
                                    }
                                }), null);
                            }


                        }
                        break;
                    }
                case 50:
                    {
                        client.Teleport(162, 218, 3830);
                        break;
                    }
                case 2:
                    {
                        if (client.Inventory.Contain(client.Player.EpicTrojanMr_MirrorItemChange[0], client.Player.EpicTrojanMr_MirrorItemChange[1]) && client.Player.CanChangeMr_MirrorEpicMaterial == 1)
                        {
                            client.Player.CanChangeMr_MirrorEpicMaterial = 0;
                            client.Inventory.Remove(client.Player.EpicTrojanMr_MirrorItemChange[0], client.Player.EpicTrojanMr_MirrorItemChange[1], stream);
                            client.CreateBoxDialog("With~a~roaring~of~thunder,~the~Ghost~Reaver~fell~upon~the~unreal~space~of~Twin~City.~Go~kill~the~demon!");
                            Server.AddMapMonster(stream, client.Map, 7483, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None);
                            foreach (var mob in client.Player.View.Roles(Role.MapObjectType.Monster))
                            {
                                client.Player.AddMapEffect(stream, mob.X, mob.Y, "SuperXp - 4");
                            }
                        }
                        else
                        {
                            data.AddText("You~don`t~have~enough~materials~to~reveal~the~Ghost~Reaver.\n\n~~~~Material~Required:~" + GetEpicTrojanItemStr(client.Player.EpicTrojanMr_MirrorItemChange) + ",~Chi~Token*1\n");
                            data.AddOption("Alright.");
                            data.AddOption("Change~the~material.", 3);
                            data.AddOption("Reveal~the~Reaver~immediately.", 4);
                            data.AddAvatar(247);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        data.AddText("~~~~~~~~~~~~~~~~~~~~Reaver~Elimination\n~~Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½\n");
                        data.AddText("~~~[Spell~Material]~" + GetEpicTrojanItemStr(client.Player.EpicTrojanMr_MirrorItemChange) + "\n~~~~~~~~~~~~~~~Chi~Token*1\n~~~[Change~Times]~" + client.Player.ChangeMr_MirrorEpicTrojan + "\n~~Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½\n\n");
                        if (client.Player.CanChangeMr_MirrorEpicMaterial == 1)
                            data.AddOption("Reveal~the~Reaver.", 2);
                        data.AddOption("Change~the~material.~(2~CPs)", 10);
                        data.AddOption("Immediately~reveal.~(27~CPs)", 4);
                        data.AddOption("Leave.");
                        data.FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.ConquerPoints < 2)
                        {
                            data.AddText("You~don`t~have~enough~CPs~to~change~the~materials.");
                            data.AddOption("Alright.");
                            data.AddAvatar(247);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            client.Player.ConquerPoints -= 2;
                            client.Player.ChangeMr_MirrorEpicTrojan++;
                            client.Player.EpicTrojanMr_MirrorItemChange = GetItemMr_MirrorEpicTrojanQuest();
                            data.AddText("~~~~~~~~~~~~~~~~~~~~Reaver~Elimination\n~~Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½\n");
                            data.AddText("~~~[Spell~Material]~" + GetEpicTrojanItemStr(client.Player.EpicTrojanMr_MirrorItemChange) + "\n~~~~~~~~~~~~~~~Chi~Token*1\n~~~[Change~Times]~" + client.Player.ChangeMr_MirrorEpicTrojan + "\n~~Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½\n\n");
                            if (client.Player.CanChangeMr_MirrorEpicMaterial == 1)
                                data.AddOption("Reveal~the~Reaver.", 2);
                            data.AddOption("Change~the~material.~(2~CPs)", 10);

                            data.AddOption("Immediately~reveal.~(27~CPs)", 4);
                            data.AddOption("Leave.");
                            data.FinalizeDialog();
                        }

                        break;
                    }
                case 4:
                    {
                        data.AddText("Are~you~sure~you~want~to~pay~27~CPs~to~immediately~reveal~the~Ghost~Reaver?");
                        data.AddOption("Yes,~I`ve~decided.~(27~CPs)", 5);
                        data.AddOption("No,~I~changed~my~mind.");
                        data.AddAvatar(247);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        if (client.Player.CanChangeMr_MirrorEpicMaterial == 1)
                        {
                            if (client.Player.ConquerPoints < 27)
                            {
                                data.AddText("I`m~afraid~I~can`t~help~you~if~you~don`t~have~27~CPs.");
                                data.AddOption("Alright.");
                                data.AddAvatar(247);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                client.Player.CanChangeMr_MirrorEpicMaterial = 0;
                                client.Player.ConquerPoints -= 27;
                                client.CreateBoxDialog("With~a~roaring~of~thunder,~the~Ghost~Reaver~fell~upon~the~unreal~space~of~Twin~City.~Go~kill~the~demon!");
                                Server.AddMapMonster(stream, client.Map, 7483, client.Player.X, client.Player.Y, 3, 3, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.None, "SuperXp-4");

                            }
                        }
                        break;
                    }
                case 1:
                    {
                        client.Teleport(162, 218, 3830);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.MrMirror)]
        public static void MrMirror(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.StageEpicTrojanQuest >= 21)
                        {
                            if (client.Player.StageEpicTrojanQuest >= 30)
                            {
                                data.AddText("You`ve~eliminated~all~the~18~Ghost~Reavers.~Hurry~and~report~back~to~General~Pak!");
                                data.AddOption("Head~to~see~General~Pak.", 2);
                                data.AddAvatar(247);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("The~evil~gas~is~going~to~swallow~Twin~City.~You~can~probably~find~those~Ghost~Reaver~in~the~Unreal~Land~that~I~created.");
                                data.AddOption("Enter~the~Unreal~Land.", 1);
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("9~stars~are~going~to~be~on~one~line.~Is~it~an~omen~of~blessing~or~misery?");
                            data.AddOption("Who~knows.");
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 1:
                    {
                        client.Teleport(163, 223, 3834);
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(3830, 154, 130, 10581);
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.SuperMok)]
        public static void SuperMok(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PaksGhost, 0, 3269);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SuperMok, 0, 3278);
            Dialog data = new Dialog(client, stream);
            if (client.Player.StageEpicTrojanQuest != 1)
            {
                data.AddText("Alas...~Father~always~forces~me~to~recite~formulas~of~forging.~They`re~useless.~You~don`t~know~what~I`m~talking~about,~right?");
                data.AddOption("No~idea.");
                data.AddAvatar(246);
                data.FinalizeDialog();
                return;
            }
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Class >= 2000)
                        {
                            data.AddText("Alas...~Father~always~forces~me~to~recite~formulas~of~forging.~They`re~useless.~You~don`t~know~what~I`m~talking~about,~right?");
                            data.AddOption("No~idea.");
                            data.AddAvatar(246);
                            data.FinalizeDialog();
                        }
                        else
                        {

                            if (client.Inventory.Contain(3003336, 27))
                            {
                                data.AddText("Wow,~you`ve~collected~27~Solar~Scraps!~Hurry~and~help~General~Pak~reforge~the~Solar~Blade!");
                                data.AddOption("Head~to~Pak`s~Ghost.", 3);
                                data.FinalizeDialog();
                            }
                            else
                            {
                                if (client.Player.EpicTrojanItemChange[0] == 0)
                                    client.Player.EpicTrojanItemChange = GetItemEpicTrojanQuest();
                                if (client.Player.CanChangeEpicMaterial == 1)
                                {
                                    data.AddText("Yeah,~I~know~the~secret~of~Solar~power.~It`s~quite~complicated,~so~I~only~forge~1~Solar~Scrap~in~a~day.~It`ll~take~27~days~in~total~to~forge~enough~scraps.\n\n");
                                    data.AddText("~The~material:~" + GetEpicTrojanItemStr(client.Player.EpicTrojanItemChange) + "\n\n");
                                    data.AddOption("Forge~a~Solar~Scrap.", 2);
                                    data.AddOption("Change~the~materials.", 4);
                                    data.AddOption("I~see.");
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.");
                                    data.AddText("~Would~you~like~to~buy~some~Energies?~\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetEpicTrojanItemStr(client.Player.EpicTrojanItemChange) + "\n~~~[Change~Times]~" + client.Player.ChangeEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                    data.AddOption("Yes,~please.", 10);
                                    data.AddOption("Change~the~material.~(1~CP)", 20);
                                    data.AddOption("Leave.");
                                    data.FinalizeDialog();
                                }
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(1002, 267, 194, (uint)NpcID.PaksGhost);
                        break;
                    }

                case 2:
                    {
                        if (client.Inventory.Contain(client.Player.EpicTrojanItemChange[0], client.Player.EpicTrojanItemChange[1]) && client.Player.CanChangeEpicMaterial == 1)
                        {
                            client.Player.CanChangeEpicMaterial = 0;
                            client.Player.EpicTrojanItemChange = GetItemEpicTrojanQuest();
                            client.Inventory.Remove(client.Player.EpicTrojanItemChange[0], client.Player.EpicTrojanItemChange[1], stream);
                            client.Inventory.AddItemWitchStack(3003336, 0, 1, stream);
                            client.CreateBoxDialog("You~received~a~Solar~Scrap!");
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e280");
                            if (client.Inventory.Contain(3003336, 27))
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                                data.AddText("Wow,~you`ve~collected~27~Solar~Scraps!~Hurry~and~help~General~Pak~reforge~the~Solar~Blade!");
                                data.AddOption("Head~to~Pak`s~Ghost.", 3);
                                data.AddAvatar(246);
                                data.FinalizeDialog();

                            }
                            else
                            {
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperCyclone))
                                {
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SuperCyclone);
                                    client.Player.XPCount = 100;
                                    client.SendSysMesage("You~acquired~a~Trojan`s~skill,~Super~Cyclone,~from~the~Solar~Scrap!");

                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BreathFocus))
                                {
                                    if (client.Inventory.Contain(3003336, 7))
                                    {
                                        client.SendSysMesage("You~acquired~a~Trojan`s~skill,~Breath~Focus,~from~7~Solar~Scraps!");
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BreathFocus);
                                    }
                                }

                                if (client.Player.CanChangeEpicMaterial == 1)
                                {
                                    data.AddText("Yeah,~I~know~the~secret~of~Solar~power.~It`s~quite~complicated,~so~I~only~forge~1~Solar~Scrap~in~a~day.~It`ll~take~27~days~in~total~to~forge~enough~scraps.\n\n");
                                    data.AddText("~The~material:~" + GetEpicTrojanItemStr(client.Player.EpicTrojanItemChange) + "\n\n");
                                    data.AddOption("Forge~a~Solar~Scrap.", 2);
                                    data.AddOption("Change~the~materials.", 3);
                                    data.AddOption("I~see.");
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.");
                                    data.AddText("~Would~you~like~to~buy~some~Energies?~\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetEpicTrojanItemStr(client.Player.EpicTrojanItemChange) + "\n~~~[Change~Times]~" + client.Player.ChangeEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                    data.AddOption("Yes,~please.", 10);
                                    data.AddOption("Change~the~material.~(1~CP)", 20);
                                    data.AddOption("Leave.");
                                    data.FinalizeDialog();
                                }

                            }


                        }
                        else
                        {
                            data.AddText("You~don`t~have~enough~materials~to~forge~the~Solar~Scrap!\n\n~~~~Forging~Material:~" + GetEpicTrojanItemStr(client.Player.EpicTrojanItemChange) + "\n");
                            data.AddOption("Alright.");
                            data.AddOption("Change~the~material.", 4);
                            data.AddOption("Entrust~forging~work.", 5);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        data.AddText("~~~~~~~~~~~~~~~~~~~Forging~Solar~Scraps\n~~Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½\n");
                        data.AddText("~~~[Material]~" + GetEpicTrojanItemStr(client.Player.EpicTrojanItemChange) + "\n~~~[Change~Times]~" + client.Player.ChangeEpicTrojan + "\n~~Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½Ã¯Â¿Â½\n\n");
                        data.AddOption("Forge~a~Solar~Scrap.", 2);
                        data.AddOption("Change~the~materials.~(1~CP)", 20);
                        data.AddOption("Entrust~with~the~forging.~(12~CPs).", 5);
                        data.AddOption("I~see.");
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {
                        data.AddText("It`ll~cost~you~12~CPs~to~entrust~me~with~the~whole~forging~work~of~Solar~Scrap.~So,~start~forging~the~Solar~Scrap?");
                        data.AddOption("Yes.~(12~CPs)", 6);
                        data.AddOption("Wait,~I~haven`t~decided.");
                        data.AddAvatar(246);
                        data.FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        if (client.Player.ConquerPoints < 12)
                        {
                            data.AddText("It~really~costs~a~lot~in~forging~the~Solar~Scrap.~Make~sure~you~have~enough~CPs~with~you.");
                            data.AddOption("Okay.");
                            data.AddAvatar(246);
                            data.FinalizeDialog();
                            break;
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1) && client.Player.CanChangeEpicMaterial == 1)
                            {
                                if (client.InTrade) return;
                                client.Player.EpicTrojanItemChange = GetItemEpicTrojanQuest();
                                client.Player.CanChangeEpicMaterial = 0;
                                client.Player.ConquerPoints -= 12;
                                client.Inventory.AddItemWitchStack(3003336, 0, 1, stream);
                                client.CreateBoxDialog("You~received~a~Solar~Scrap!");
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "zf2-e280");

                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperCyclone))
                                {
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SuperCyclone);
                                    client.Player.XPCount = 100;
                                    client.SendSysMesage("You~acquired~a~Trojan`s~skill,~Super~Cyclone,~from~the~Solar~Scrap!");

                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BreathFocus))
                                {
                                    if (client.Inventory.Contain(3003336, 7))
                                    {
                                        client.SendSysMesage("You~acquired~a~Trojan`s~skill,~Breath~Focus,~from~7~Solar~Scraps!");
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BreathFocus);
                                    }
                                }

                                if (client.Player.CanChangeEpicMaterial == 1)
                                {
                                    data.AddText("Yeah,~I~know~the~secret~of~Solar~power.~It`s~quite~complicated,~so~I~only~forge~1~Solar~Scrap~in~a~day.~It`ll~take~27~days~in~total~to~forge~enough~scraps.\n\n");
                                    data.AddText("~The~material:~" + GetEpicTrojanItemStr(client.Player.EpicTrojanItemChange) + "\n\n");
                                    data.AddOption("Forge~a~Solar~Scrap.", 2);
                                    data.AddOption("Change~the~materials.", 3);
                                    data.AddOption("I~see.");
                                    data.FinalizeDialog();
                                }
                                else
                                {
                                    data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.");
                                    data.AddText("~Would~you~like~to~buy~some~Energies?~\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetEpicTrojanItemStr(client.Player.EpicTrojanItemChange) + "\n~~~[Change~Times]~" + client.Player.ChangeEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                    data.AddOption("Yes,~please.", 10);
                                    data.AddOption("Change~the~material.~(1~CP)", 20);
                                    data.AddOption("Leave.");
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("I~can`t~put~anything~into~a~full~bag.~Make~some~room~in~your~inventory~for~the~Solar~Scrap.");
                                data.AddOption("I`ll~do~it~now.");
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 20:
                    {
                        if (client.Player.ConquerPoints < 1)
                        {
                            data.AddText("You~don`t~have~enough~CPs~to~refresh~the~materials.");
                            data.AddOption("Alright.");
                            data.AddAvatar(246);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 1;
                            client.Player.ChangeEpicTrojan += 1;
                            client.Player.EpicTrojanItemChange = GetItemEpicTrojanQuest();
                            if (client.Player.CanChangeEpicMaterial == 1)
                            {
                                data.AddText("Yeah,~I~know~the~secret~of~Solar~power.~It`s~quite~complicated,~so~I~only~forge~1~Solar~Scrap~in~a~day.~It`ll~take~27~days~in~total~to~forge~enough~scraps.\n\n");
                                data.AddText("~The~material:~" + GetEpicTrojanItemStr(client.Player.EpicTrojanItemChange) + "\n\n");
                                data.AddOption("Forge~a~Solar~Scrap.", 2);
                                data.AddOption("Change~the~materials.", 3);
                                data.AddOption("I~see.");
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("You~don`t~have~enough~Energies~to~continue.~1~Energy~will~be~restored~when~you~log~in~each~day.");
                                data.AddText("~Would~you~like~to~buy~some~Energies?~\n\n~~------------------------------------------------------\n~~~[Tomorrow`s~Material]~" + GetEpicTrojanItemStr(client.Player.EpicTrojanItemChange) + "\n~~~[Change~Times]~" + client.Player.ChangeEpicTrojan + "\n~~------------------------------------------------------\n\n");
                                data.AddOption("Yes,~please.", 10);
                                data.AddOption("Change~the~material.~(1~CP)", 20);
                                data.AddOption("Leave.");
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 10:
                    {
                        if (client.Inventory.Contain(3003335, 1))
                        {
                            if (client.Inventory.Contain(3003336, 27))
                            {
                                client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                                data.AddText("Wow,~you`ve~collected~27~Solar~Scraps!~Hurry~and~help~General~Pak~reforge~the~Solar~Blade!");
                                data.AddOption("Head~to~Pak`s~Ghost.", 3);
                                data.AddAvatar(246);
                                data.FinalizeDialog();

                            }
                            else
                            {
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperCyclone))
                                {
                                    client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SuperCyclone);
                                    client.Player.XPCount = 100;
                                    client.SendSysMesage("You~acquired~a~Trojan`s~skill,~Super~Cyclone,~from~the~Solar~Scrap!");

                                }
                                if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BreathFocus))
                                {
                                    if (client.Inventory.Contain(3003336, 7))
                                    {
                                        client.SendSysMesage("You~acquired~a~Trojan`s~skill,~Breath~Focus,~from~7~Solar~Scraps!");
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BreathFocus);
                                    }
                                }

                                data.AddText("So~far,~you`ve~forged~and~absorbed~" + client.Inventory.GetCountItem(3003336) + "~Solar~Scrap(s).~You~need~to~collect~27~scraps~to~reforge~the~Solar~Blade.");
                                data.AddText("~Each~time,~you~can~only~forge~1~Solar~Scrap.~So,~how~many~Energies~would~you~like~to~buy?");
                                data.AddOption("1~Energy~and~complete~the~quest.(300~CPs)", 11);
                                if (client.Inventory.GetCountItem(3003336) < 22)
                                    data.AddOption("6~Energies~and~complete~the~quest.(1800~CPs)", 13);
                                data.FinalizeDialog();
                            }

                        }
                        else
                        {
                            client.CreateBoxDialog("You~don`t~have~the~Solar~Blade`s~Remain.~Go~claim~that~from~General~Pak!");
                        }
                        break;
                    }
                case 13:
                    {
                        data.AddText("Are~you~sure~you~want~to~pay~1,800~CPs~to~absorb~6~Solar~Scraps?");
                        data.AddOption("6~Energies~and~complete~the~quest.(1800~CPs)", 14);
                        data.AddOption("Wait,~I~haven`t~decided.");
                        data.AddAvatar(246);
                        data.FinalizeDialog();
                        break;
                    }
                case 14:
                    {
                        if (client.Inventory.Contain(3003335, 1))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Player.ConquerPoints < 1800)
                                {
                                    client.CreateBoxDialog("You~don`t~have~enough~CPs~to~activate~the~power~of~the~Solar~Blade`s~Remain.");
                                }
                                else
                                {
                                    if (client.InTrade) return;
                                    client.Player.ConquerPoints -= 1800;
                                    client.Inventory.AddItemWitchStack(3003336, 0, 6, stream);
                                    client.SendSysMesage("You~successfully~activated~the~power~of~the~Solar~Blade`s~Remain,~and~obtained~6~Solar~Scrap!", MsgMessage.ChatMode.System);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "Attack35r");

                                    if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperCyclone))
                                    {
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SuperCyclone);
                                        client.Player.XPCount = 100;
                                        client.SendSysMesage("You~acquired~a~Trojan`s~skill,~Super~Cyclone,~from~the~Solar~Scrap!");

                                    }
                                    if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BreathFocus))
                                    {
                                        if (client.Inventory.Contain(3003336, 7))
                                        {
                                            client.SendSysMesage("You~acquired~a~Trojan`s~skill,~Breath~Focus,~from~7~Solar~Scraps!");
                                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BreathFocus);
                                        }
                                    }

                                    if (client.Inventory.Contain(3003336, 27))
                                    {
                                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                                        data.AddText("Wow,~you`ve~collected~27~Solar~Scraps!~Hurry~and~help~General~Pak~reforge~the~Solar~Blade!");
                                        data.AddOption("Head~to~Pak`s~Ghost.", 3);
                                        data.AddAvatar(246);
                                        data.FinalizeDialog();

                                    }
                                    else
                                    {
                                        data.AddText("So~far,~you`ve~forged~and~absorbed~" + client.Inventory.GetCountItem(3003336) + "~Solar~Scrap(s).~You~need~to~collect~27~scraps~to~reforge~the~Solar~Blade.");
                                        data.AddText("~Each~time,~you~can~only~forge~1~Solar~Scrap.~So,~how~many~Energies~would~you~like~to~buy?");
                                        data.AddOption("1~Energy~and~complete~the~quest.(300~CPs)", 11);
                                        if (client.Inventory.GetCountItem(3003336) < 22)
                                            data.AddOption("6~Energies~and~complete~the~quest.(1800~CPs)", 13);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                            else
                                client.CreateBoxDialog("Your~inventory~is~full.~You~need~to~make~some~room,~first.");
                        }
                        else
                            client.CreateBoxDialog("You~don`t~have~the~Solar~Blade`s~Remain.~Go~claim~that~from~General~Pak!");
                        break;
                    }
                case 11:
                    {
                        data.AddText("Are~you~sure~you~want~to~pay~300~CPs~to~absorb~1~Solar~Scrap?");
                        data.AddOption("1~Energy~and~complete~the~quest.(300~CPs)", 12);
                        data.AddOption("Wait,~I~haven`t~decided.");
                        data.AddAvatar(246);
                        data.FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.Contain(3003335, 1))
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                if (client.Player.ConquerPoints < 300)
                                {
                                    client.CreateBoxDialog("You~don`t~have~enough~CPs~to~activate~the~power~of~the~Solar~Blade`s~Remain.");
                                }
                                else
                                {
                                    if (client.InTrade) return;
                                    client.Player.ConquerPoints -= 300;
                                    client.Inventory.AddItemWitchStack(3003336, 0, 1, stream);
                                    client.SendSysMesage("You~successfully~activated~the~power~of~the~Solar~Blade`s~Remain,~and~obtained~1~Solar~Scrap!", MsgMessage.ChatMode.System);
                                    client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "Attack35r");

                                    if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.SuperCyclone))
                                    {
                                        client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.SuperCyclone);
                                        client.Player.XPCount = 100;
                                        client.SendSysMesage("You~acquired~a~Trojan`s~skill,~Super~Cyclone,~from~the~Solar~Scrap!");

                                    }
                                    if (!client.MySpells.ClientSpells.ContainsKey((ushort)Role.Flags.SpellID.BreathFocus))
                                    {
                                        if (client.Inventory.Contain(3003336, 7))
                                        {
                                            client.SendSysMesage("You~acquired~a~Trojan`s~skill,~Breath~Focus,~from~7~Solar~Scraps!");
                                            client.MySpells.Add(stream, (ushort)Role.Flags.SpellID.BreathFocus);
                                        }
                                    }

                                    if (client.Inventory.Contain(3003336, 27))
                                    {
                                        client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId);
                                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);

                                        data.AddText("Wow,~you`ve~collected~27~Solar~Scraps!~Hurry~and~help~General~Pak~reforge~the~Solar~Blade!");
                                        data.AddOption("Head~to~Pak`s~Ghost.", 3);
                                        data.AddAvatar(246);
                                        data.FinalizeDialog();

                                    }
                                    else
                                    {
                                        data.AddText("So~far,~you`ve~forged~and~absorbed~" + client.Inventory.GetCountItem(3003336) + "~Solar~Scrap(s).~You~need~to~collect~27~scraps~to~reforge~the~Solar~Blade.");
                                        data.AddText("~Each~time,~you~can~only~forge~1~Solar~Scrap.~So,~how~many~Energies~would~you~like~to~buy?");
                                        data.AddOption("1~Energy~and~complete~the~quest.(300~CPs)", 11);
                                        if (client.Inventory.GetCountItem(3003336) < 22)
                                            data.AddOption("6~Energies~and~complete~the~quest.(1800~CPs)", 13);
                                        data.FinalizeDialog();
                                    }
                                }
                            }
                            else
                                client.CreateBoxDialog("Your~inventory~is~full.~You~need~to~make~some~room,~first.");
                        }
                        else
                            client.CreateBoxDialog("You~don`t~have~the~Solar~Blade`s~Remain.~Go~claim~that~from~General~Pak!");
                        break;
                    }
            }
        }


        [NpcAttribute(NpcID.PaksGhost)]
        public static void PaksGhost(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PaksGhost, 0, 3269);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.SuperMok, 0, 3278);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PaksGhost, 0, 3275);


            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.Class >= 2000)
                        {
                            data.AddText("No...~No...~You`re~not~the~Trojan~I`m~looking~for.~It`s~been~300~years,~however,~I~still~can`t~find~that~chosen~man.");
                            data.AddText("\n\n~~~~(Level~120+~or~reborn~Trojans~will~receive~a~Trojan~Epic~Weapon~for~free~by~completing~a~specified~Trojan~Epic~Quest~every~day,~and~a~series~of~Epic~Quests~in~90~days.)");
                            data.AddOption("Um...");
                            data.AddAvatar(245);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Player.StageEpicTrojanQuest == 0)
                            {
                                data.AddText("Hey,~YOU!~You`re~the~chosen~man.~You~know,~I`ve~been~here~300~years,~waiting~for~you!");
                                data.AddOption("What`re~talking?", 2);
                                data.FinalizeDialog();
                            }
                            else if (client.Player.StageEpicTrojanQuest == 1)
                            {
                                data.AddText("It`s~not~easy~to~travel~back.~First,~you~need~to~collect~27~Solar~Scraps,~and~I`ll~use~them~to~reforge~the~Solar~Blade.");
                                data.AddText("~With~the~powerful~Solar~Blade,~I~can~send~you~back~in~time~for~300~years.");
                                if (client.Inventory.Contain(3003335, 1))
                                {
                                    if (client.Inventory.Contain(3003336, 27))
                                    {
                                        data.AddOption("Reforge~the~Solar~Blade.", 5);
                                        data.FinalizeDialog();
                                    }
                                    else
                                    {
                                        data.AddOption("How~to~collect~those~scraps?", 10);
                                        if (client.Inventory.Contain(3003336, 27))
                                        {
                                            data.AddOption("Reforge~the~Solar~Blade.", 5);
                                            data.AddAvatar(245);
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                                else
                                {
                                    data.AddOption("Give~me~the~Solar~Blade`s~Remain.", 12);
                                    data.FinalizeDialog();
                                }
                            }
                            else
                            {
                                data.AddText("Finally...~the~Solar~Blade~is~back!~With~its~power,~you~can~travel~back~in~time~for~300~years.~Remember~to~find~me~at~that~time!");
                                data.AddText("~See~you~in~Twin~City.");
                                data.AddText("\n\n~~~~(Level~120+~or~reborn~Trojans~will~receive~a~Trojan~Epic~Weapon~for~free~by~completing~a~specified~Trojan~Epic~Quest~every~day,~and~a~series~of~Epic~Quests~in~90~days.)");
                                data.AddOption("Go~back~to~300~years~ago.", 6);
                                data.AddOption("I`m~not~ready,~yet.");
                                data.FinalizeDialog();
                            }

                        }
                        break;

                    }
                case 2:
                    {
                        data.AddText("It`s~a~long~story,~but~I`ll~make~it~short.~300~years~ago,~I~was~ordered~to~defend~against~the~invasion~of~the~Flame~Devastator.");
                        data.AddText("~Before~the~battle,~I~made~a~secret~plan,~however,~the~Flame~Devastator~has~already~known~it~and~trapped~my~army.");
                        data.AddText("~No~one~survived.~Over~a~night,~Twin~City~was~ruined~and~thousands~of~civilians~were~killed.");
                        data.AddOption("What~a~sad~thing.", 3);
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        data.AddText("There~must~be~a~traitor~in~my~army!~How~can~I~rest~in~peace~without~digging~him~out?~So,~my~soul~lived~on~my~Solar~Blade`s~Remain,");
                        data.AddText("~waiting~for~a~chance.~Look,~you`re~the~chance!~Are~you~willing~to~travel~back~in~time~for~300~years,~and~root~out~this~cowardly~traitor?");
                        data.AddText("~So,~we~can~stop~the~Flame~Devastator~and~avoid~the~massacre~of~Twin~City.");
                        data.AddText("\n\n~~~~(Level~120+~or~reborn~Trojans~will~receive~a~Trojan~Epic~Weapon~for~free~by~completing~a~specified~Trojan~Epic~Quest~every~day,~and~a~series~of~Epic~Quests~in~90~days.)");
                        data.AddOption("I`ll~try~my~best!", 4);
                        data.AddOption("I`m~not~ready,~yet.");
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 5:
                    {

                        if (client.Inventory.Contain(3003335, 1))
                        {
                            if (client.Inventory.Contain(3003336, 27))
                            {
                                client.Inventory.Remove(3003335, 1, stream);
                                if (client.Inventory.Remove(3003336, 27, stream))
                                {
                                    client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId);

                                    client.Player.StageEpicTrojanQuest = 2;
                                    client.Player.QuestGUI.Accept(ActiveQuest3, 0);

                                    client.SendSysMesage("General~Pak~recited~some~words~to~the~Solar~Scraps.~With~a~flash~of~bright~light,~the~Solar~Blade~is~reshaped!~Hurry~and~awaken~this~blade~to~go~back~to~300~years~ago!", MsgMessage.ChatMode.System);
                                    client.Player.AddMapEffect(stream, 267, 194, "role-select1");
                                    //97199128 312                     1002 267 194 role-select1 on npc think
                                    data.AddText("Finally...~the~Solar~Blade~is~back!~With~its~power,~you~can~travel~back~in~time~for~300~years.~Remember~to~find~me~at~that~time!");
                                    data.AddText("~See~you~in~Twin~City.");
                                    data.AddText("\n\n~~~~(Level~120+~or~reborn~Trojans~will~receive~a~Trojan~Epic~Weapon~for~free~by~completing~a~specified~Trojan~Epic~Quest~every~day,~and~a~series~of~Epic~Quests~in~90~days.)");
                                    data.AddOption("Go~back~to~300~years~ago.", 6);
                                    data.AddOption("I`m~not~ready,~yet.");
                                    data.AddAvatar(245);
                                    data.FinalizeDialog();


                                }
                            }
                            else
                            {
                                data.AddText("You~need~to~collect~27~Solar~Scraps,~first.~With~them,~I~can`t~reforge~the~Solar~Blade.");
                                data.AddOption("I~see.");
                                data.FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("You~need~to~collect~27~Solar~Scraps,~first.~With~them,~I~can`t~reforge~the~Solar~Blade.");
                            data.AddOption("I~see.");
                            data.AddAvatar(245);
                            data.FinalizeDialog();
                        }
                        break;
                    }
                case 6:
                    {
                        if (client.Player.StageEpicTrojanQuest >= 40)
                        {
                            if (client.Player.StageEpicTrojanQuest >= 50)
                            {
                                client.Teleport(150, 144, 3832);
                                client.Player.QuestGUI.SendAutoPatcher("You~felt~dizzy,~and~found~yourself~in~Twin~City,~300~years~ago.", 3832, 154, 130, 10581);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession5");
                            }
                            else
                            {
                                client.Teleport(150, 144, 3831);
                                client.Player.QuestGUI.SendAutoPatcher("You~felt~dizzy,~and~found~yourself~in~Twin~City,~300~years~ago.", 3831, 154, 130, 10581);
                                client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession5");
                            }
                        }
                        else
                        {
                            client.Teleport(150, 144, 3830);
                            client.Player.QuestGUI.SendAutoPatcher("You~felt~dizzy,~and~found~yourself~in~Twin~City,~300~years~ago.", 3832, 154, 130, 10581);
                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, false, "accession5");
                        }
                        break;
                    }
                case 10:
                    {
                        data.AddText("You~can~probably~get~the~Solar~Scraps~from~Super~Mok.~She~holds~the~secret~of~solar~power.~Well,~you~need~to~provide~the~materials~for~making~the~Solar~Scraps.");
                        data.AddText("~Since~the~job~is~quite~exhausted,~she~can~only~make~1~Solar~Scrap~in~a~day.");
                        data.AddText("\n\n~~~~(Level~120+~or~reborn~Trojans~will~receive~a~Trojan~Epic~Weapon~for~free~by~completing~a~specified~Trojan~Epic~Quest~every~day,~and~a~series~of~Epic~Quests~in~90~days.)");
                        data.AddOption("I`ll~go~find~Super~Mok.", 11);
                        data.AddAvatar(245);
                        data.FinalizeDialog();
                        break;
                    }
                case 11:
                    {

                        client.Player.QuestGUI.SendAutoPatcher(1002, 428, 294, (uint)NpcID.SuperMok);
                        break;
                    }
                case 12:
                    {
                        if (client.Inventory.Contain(3003335, 1))
                        {
                            data.AddText("The~Solar~Blade`s~Remain~is~already~in~your~inventory.");
                            data.AddOption("Okay.");
                            data.AddAvatar(245);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1))
                            {
                                client.Inventory.Add(stream, 3003335);
                                client.Player.StageEpicTrojanQuest = 1;
                                data.AddText("This~is~the~Solar~Blade`s~Remain.~Keep~it~well!");
                                data.AddOption("I~will.");
                                data.FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Your~inventory~is~full.~Make~some~room,~first.");
                                data.AddOption("I`ll~do~it~now.");
                                data.FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 4:
                    {


                        if (client.Player.Level < 120)
                        {
                            data.AddText("Sorry,~the~Flame~Devastator~is~much~fiercer~than~you~imagined.~You~should~be~at~least~Level~120~to~challenge.");
                            data.AddOption("Alright.");
                            data.AddAvatar(245);
                            data.FinalizeDialog();
                        }
                        else
                        {
                            if (client.Inventory.HaveSpace(1))
                            {

                                if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                {
                                    if (client.Inventory.Contain(3003335, 1))
                                    {
                                        client.SendSysMesage("You~received~the~Solar~Blade`s~Remain.", MsgMessage.ChatMode.System);
                                        data.AddText("It`s~not~easy~to~travel~back.~First,~you~need~to~collect~27~Solar~Scraps,~and~I`ll~use~them~to~reforge~the~Solar~Blade.");
                                        data.AddText("~With~the~powerful~Solar~Blade,~I~can~send~you~back~in~time~for~300~years.");
                                        if (client.Inventory.Contain(3003335, 1))
                                        {
                                            if (client.Inventory.Contain(3003336, 27))
                                            {
                                                data.AddOption("Reforge~the~Solar~Blade.", 5);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                data.AddOption("How~to~collect~those~scraps?", 10);
                                                data.FinalizeDialog();
                                            }
                                        }
                                        else
                                        {
                                            data.AddOption("Give~me~the~Solar~Blade`s~Remain.", 12);
                                            data.FinalizeDialog();
                                        }
                                    }
                                    else
                                    {
                                        client.Inventory.Add(stream, 3003335);
                                        client.Player.StageEpicTrojanQuest = 1;
                                        client.SendSysMesage("You~received~the~Solar~Blade`s~Remain.", MsgMessage.ChatMode.System);
                                        data.AddText("It`s~not~easy~to~travel~back.~First,~you~need~to~collect~27~Solar~Scraps,~and~I`ll~use~them~to~reforge~the~Solar~Blade.");
                                        data.AddText("~With~the~powerful~Solar~Blade,~I~can~send~you~back~in~time~for~300~years.");
                                        if (client.Inventory.Contain(3003335, 1))
                                        {
                                            if (client.Inventory.Contain(3003336, 27))
                                            {
                                                data.AddOption("Reforge~the~Solar~Blade.", 5);
                                                data.FinalizeDialog();
                                            }
                                            else
                                            {
                                                data.AddOption("How~to~collect~those~scraps?", 10);
                                                data.FinalizeDialog();
                                            }
                                        }
                                        else
                                        {
                                            data.AddOption("Give~me~the~Solar~Blade`s~Remain.", 12);
                                            data.FinalizeDialog();
                                        }
                                    }
                                }
                                else
                                {
                                    client.Player.QuestGUI.Accept(ActiveQuest, 0);
                                    if (client.Inventory.Contain(3003335, 1))
                                    {
                                        client.SendSysMesage("You~received~the~Solar~Blade`s~Remain.", MsgMessage.ChatMode.System);
                                        data.AddText("It`s~not~easy~to~travel~back.~First,~you~need~to~collect~27~Solar~Scraps,~and~I`ll~use~them~to~reforge~the~Solar~Blade.");
                                        data.AddText("~With~the~powerful~Solar~Blade,~I~can~send~you~back~in~time~for~300~years.");
                                        if (client.Inventory.Contain(3003335, 1))
                                        {
                                            client.SendSysMesage("You~received~the~Solar~Blade`s~Remain.", MsgMessage.ChatMode.System);
                                            data.AddText("It`s~not~easy~to~travel~back.~First,~you~need~to~collect~27~Solar~Scraps,~and~I`ll~use~them~to~reforge~the~Solar~Blade.");
                                            data.AddText("~With~the~powerful~Solar~Blade,~I~can~send~you~back~in~time~for~300~years.");
                                            if (client.Inventory.Contain(3003335, 1))
                                            {
                                                if (client.Inventory.Contain(3003336, 27))
                                                {
                                                    data.AddOption("Reforge~the~Solar~Blade.", 5);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    data.AddOption("How~to~collect~those~scraps?", 10);
                                                    data.FinalizeDialog();
                                                }
                                            }
                                            else
                                            {
                                                data.AddOption("Give~me~the~Solar~Blade`s~Remain.", 12);
                                                data.FinalizeDialog();
                                            }
                                        }
                                        else
                                        {
                                            client.Inventory.Add(stream, 3003335);
                                            client.Player.StageEpicTrojanQuest = 1;
                                            client.SendSysMesage("You~received~the~Solar~Blade`s~Remain.", MsgMessage.ChatMode.System);
                                            data.AddText("It`s~not~easy~to~travel~back.~First,~you~need~to~collect~27~Solar~Scraps,~and~I`ll~use~them~to~reforge~the~Solar~Blade.");
                                            data.AddText("~With~the~powerful~Solar~Blade,~I~can~send~you~back~in~time~for~300~years.");
                                            if (client.Inventory.Contain(3003335, 1))
                                            {
                                                if (client.Inventory.Contain(3003336, 27))
                                                {
                                                    data.AddOption("Reforge~the~Solar~Blade.", 5);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    data.AddOption("How~to~collect~those~scraps?", 10);
                                                    data.FinalizeDialog();
                                                }
                                            }
                                            else
                                            {
                                                data.AddOption("Give~me~the~Solar~Blade`s~Remain.", 12);
                                                data.FinalizeDialog();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        client.Inventory.Add(stream, 3003335);
                                        client.Player.StageEpicTrojanQuest = 1;
                                        client.SendSysMesage("You~received~the~Solar~Blade`s~Remain.", MsgMessage.ChatMode.System);
                                        data.AddText("It`s~not~easy~to~travel~back.~First,~you~need~to~collect~27~Solar~Scraps,~and~I`ll~use~them~to~reforge~the~Solar~Blade.");
                                        data.AddText("~With~the~powerful~Solar~Blade,~I~can~send~you~back~in~time~for~300~years.");
                                        if (client.Inventory.Contain(3003335, 1))
                                        {
                                            client.SendSysMesage("You~received~the~Solar~Blade`s~Remain.", MsgMessage.ChatMode.System);
                                            data.AddText("It`s~not~easy~to~travel~back.~First,~you~need~to~collect~27~Solar~Scraps,~and~I`ll~use~them~to~reforge~the~Solar~Blade.");
                                            data.AddText("~With~the~powerful~Solar~Blade,~I~can~send~you~back~in~time~for~300~years.");
                                            if (client.Inventory.Contain(3003335, 1))
                                            {
                                                if (client.Inventory.Contain(3003336, 27))
                                                {
                                                    data.AddOption("Reforge~the~Solar~Blade.", 5);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    data.AddOption("How~to~collect~those~scraps?", 10);
                                                    data.FinalizeDialog();
                                                }
                                            }
                                            else
                                            {
                                                data.AddOption("Give~me~the~Solar~Blade`s~Remain.", 12);
                                                data.FinalizeDialog();
                                            }
                                        }
                                        else
                                        {
                                            client.Inventory.Add(stream, 3003335);
                                            client.Player.StageEpicTrojanQuest = 1;
                                            client.SendSysMesage("You~received~the~Solar~Blade`s~Remain.", MsgMessage.ChatMode.System);
                                            data.AddText("It`s~not~easy~to~travel~back.~First,~you~need~to~collect~27~Solar~Scraps,~and~I`ll~use~them~to~reforge~the~Solar~Blade.");
                                            data.AddText("~With~the~powerful~Solar~Blade,~I~can~send~you~back~in~time~for~300~years.");
                                            if (client.Inventory.Contain(3003335, 1))
                                            {
                                                if (client.Inventory.Contain(3003336, 27))
                                                {
                                                    data.AddOption("Reforge~the~Solar~Blade.", 5);
                                                    data.FinalizeDialog();
                                                }
                                                else
                                                {
                                                    data.AddOption("How~to~collect~those~scraps?", 10);
                                                    data.FinalizeDialog();
                                                }
                                            }
                                            else
                                            {
                                                data.AddOption("Give~me~the~Solar~Blade`s~Remain.", 12);
                                                data.FinalizeDialog();
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                data.AddText("Make~some~room~in~your~inventory,~first!~I`ll~give~you~my~Solar~Blade`s~Remain.~With~its~power,~you~may~go~back~to~300~years~ago.");
                                data.AddOption("I~see.");
                                data.FinalizeDialog();
                            }
                        }

                        break;
                    }
            }
        }





        //[NpcAttribute(NpcID.MammonEnvoy)]
        //public static void MammonEnvoy(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("Welcome~to~the~Prize~Center!~Did~you~bring~the~treasure~we~want?~So,~which~kind~of~coins~you~want~to~exchange,~the~Gold,~Silver~or~the~Copper?");
        //                data.AddOption("Gold.~(Remain:" + MsgTournaments.MsgSchedules.SquidwardOctopus.GoldRemain + ")", 1);
        //                data.AddOption("Silver.(Remain:" + MsgTournaments.MsgSchedules.SquidwardOctopus.SilverRemain + ")", 20);
        //                data.AddOption("Copper.(Remain:" + MsgTournaments.MsgSchedules.SquidWardOctopus.CopperRemain + ")", 30);
        //                data.AddOption("Send~me~to~Twin~City.", 40);
        //                data.AddOption("I`ll~think~it~over.", 255);
        //                data.AddAvatar(60);
        //                data.FinalizeDialog();
        //                break;
        //            }
        //        case 1:
        //            {
        //                data.AddText("Really?~You~got~a~Gold~Coin?~That`s~great!~So,~are~you~sure~you~want~to~exchange~now?~~You`ll~receive~a~Small~Lottery~Ticket~and~have~a~chance~to~get~a~rare~mount~armor.");
        //                data.AddOption("Yeah,~please!", 2);
        //                data.AddOption("Sorry,~wait!~Wait!", 255);
        //                data.AddAvatar(60);
        //                data.FinalizeDialog();
        //                break;
        //            }
        //        case 2:
        //            {
        //                if (MsgTournaments.MsgSchedules.SquidWardOctopus.GoldRemain > 0)
        //                {

        //                    if (client.Inventory.Contain(711609, 1))
        //                    {

        //                        MsgTournaments.MsgSchedules.SquidWardOctopus.GoldRemain -= 1;
        //                        client.Inventory.Remove(711609, 1, stream);
        //                        System.Random CPs = new System.Random();
        //                        uint CPss = (uint)CPs.Next(1000, 5000);
        //                        client.Player.ConquerPoints += CPss;
        //                        client.CreateBoxDialog("You~received~" + CPss + " CPs");
        //                        if (MsgTournaments.MsgSchedules.SquidWardOctopus.GoldRemain == 20)
        //                            Server.SendGlobalPacket(new MsgMessage("Mammon~Envoy~only~have~20~pieces~of~rewards~left~for~the~Gold~Coins,~now.~First~come~first~served!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
        //                        else if (MsgTournaments.MsgSchedules.SquidWardOctopus.GoldRemain == 10)
        //                            Server.SendGlobalPacket(new MsgMessage("Mammon~Envoy~only~have~10~pieces~of~rewards~left~for~the~Gold~Coins,~now.~First~come~first~served!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
        //                        else if (MsgTournaments.MsgSchedules.SquidWardOctopus.GoldRemain == 5)
        //                            Server.SendGlobalPacket(new MsgMessage("Mammon~Envoy~only~have~5~pieces~of~rewards~left~for~the~Gold~Coins,~now.~First~come~first~served!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
        //                        else if (MsgTournaments.MsgSchedules.SquidWardOctopus.GoldRemain <= 0)
        //                            Server.SendGlobalPacket(new MsgMessage("~All~the~rewards~for~the~Gold~Coins~have~been~taken.~Hurry~to~exchange~your~other~kinds~of~coins!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));




        //                    }
        //                    else
        //                    {
        //                        data.AddText("Are~you~kidding?~You~don`t~have~the~Gold~Coin~we~want.~It~isn`t~a~right~place~to~play~jokes,~okay?");
        //                        data.AddOption("Sorry,~bye!", 255);
        //                        data.AddAvatar(60);
        //                        data.FinalizeDialog();
        //                    }
        //                }
        //                else
        //                {
        //                    data.AddText("Sigh...~Sorry,~but~you~are~too~late.~All~the~rewards~I~prepared~for~the~Gold~Coins~have~been~taken.~Come~to~exchange~the~Gold~Coin~earlier,~next~time.");
        //                    data.AddOption("Alright.", 255);
        //                    data.AddAvatar(60);
        //                    data.FinalizeDialog();
        //                }
        //                break;
        //            }
        //        case 20:
        //            {
        //                data.AddText("Are~you~sure~you~want~to~exchange~the~Silver~Coin,~now?~You`ll~receive~a~gift~randomly~picked~from~a~Tiny~Class2~Money~Pouch,~a~Tiny~Horse~Racing~Points~Bag,~3~Praying~Stone~");
        //                data.AddText("~Scraps(S)~and~3~Special~EXP~Ball~Scraps.~Not~only~that~gift,~you~will~also~get~an~Endurance~Book.");
        //                data.AddOption("Yeah,~please!", 21);
        //                data.AddOption("No,~not~now!", 255);
        //                data.AddAvatar(60);
        //                data.FinalizeDialog();
        //                break;
        //            }
        //        case 21:
        //            {
        //                if (MsgTournaments.MsgSchedules.SquidWardOctopus.SilverRemain > 0)
        //                {
        //                    if (client.Inventory.Contain(711610, 1))
        //                    {


        //                        client.Inventory.Remove(711610, 1, stream);
        //                        System.Random CPs = new System.Random();
        //                        uint CPss = (uint)CPs.Next(500, 1000);
        //                        client.Player.ConquerPoints += CPss;
        //                        client.CreateBoxDialog("You~received~" + CPss + " CPs");

        //                        if (MsgTournaments.MsgSchedules.SquidWardOctopus.SilverRemain == 50)
        //                            Server.SendGlobalPacket(new MsgMessage("Mammon~Envoy~only~have~50~pieces~of~rewards~left~for~the~Silver~Coins,~now.~Quicken~your~pace!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
        //                        else if (MsgTournaments.MsgSchedules.SquidWardOctopus.SilverRemain == 30)
        //                            Server.SendGlobalPacket(new MsgMessage("Mammon~Envoy~only~have~30~pieces~of~rewards~left~for~the~Silver~Coins,~now.~Quicken~your~pace!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
        //                        else if (MsgTournaments.MsgSchedules.SquidWardOctopus.SilverRemain == 10)
        //                            Server.SendGlobalPacket(new MsgMessage("Mammon~Envoy~only~have~10~pieces~of~rewards~left~for~the~Silver~Coins,~now.~Quicken~your~pace!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
        //                        else if (MsgTournaments.MsgSchedules.SquidWardOctopus.SilverRemain <= 0)
        //                            Server.SendGlobalPacket(new MsgMessage("Pay~attention!~All~the~rewards~for~the~Silver~Coins~have~been~taken.~Hurry~to~exchange~your~other~kinds~of~coins!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));


        //                    }
        //                    else
        //                    {
        //                        data.AddText("Sorry,~but~I~can`t~find~it~in~your~inventory.~Did~you~lose~the~Silver~Coin~somewhere?~");
        //                        data.AddOption("Oh,~no!", 255);
        //                        data.AddAvatar(60);
        //                        data.FinalizeDialog();
        //                    }
        //                }
        //                else
        //                {
        //                    data.AddText("Sigh...~Sorry,~but~you~are~too~late.~All~the~rewards~I~prepared~for~the~Silver~Coins~have~been~taken.~Come~to~exchange~the~Silver~Coins~earlier,~next~time.");
        //                    data.AddOption("Alright.", 255);
        //                    data.AddAvatar(60);
        //                    data.FinalizeDialog();
        //                }
        //                break;
        //            }
        //        case 30:
        //            {
        //                data.AddText("You~have~a~Copper~Coin,~right?~Let~me~see~what~you~can~get.~Oh,~here.~You`ll~receive~a~gift~randomly~picked~from~a~Meteor,~a~Tiny~Class1~Money~Pouch,~a~Praying~Stone~Scrap(S)~and~a");
        //                data.AddText("~Special~EXP~Ball~Scrap.");
        //                data.AddOption("Yeah,~please!", 31);
        //                data.AddOption("Can~I~say~`NO`?", 255);
        //                data.AddAvatar(60);
        //                data.FinalizeDialog();
        //                break;
        //            }
        //        case 31:
        //            {
        //                if (MsgTournaments.MsgSchedules.SquidWardOctopus.CopperRemain > 0)
        //                {
        //                    if (client.Inventory.Contain(711611, 1))
        //                    {

        //                        MsgTournaments.MsgSchedules.SquidWardOctopus.CopperRemain -= 1;
        //                        client.Inventory.Remove(711611, 1, stream);
        //                        System.Random CPs = new System.Random();
        //                        uint CPss = (uint)CPs.Next(100, 500);
        //                        client.Player.ConquerPoints += CPss;
        //                        client.CreateBoxDialog("You~received~" + CPss + " CPs");
        //                        if (MsgTournaments.MsgSchedules.SquidWardOctopus.CopperRemain == 100)
        //                            Server.SendGlobalPacket(new MsgMessage("Mammon~Envoy~only~have~100~pieces~of~rewards~left~for~the~Copper~Coins,~now.~Quicken~your~pace!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
        //                        else if (MsgTournaments.MsgSchedules.SquidWardOctopus.CopperRemain == 50)
        //                            Server.SendGlobalPacket(new MsgMessage("Mammon~Envoy~only~have~50~pieces~of~rewards~left~for~the~Copper~Coins,~now.~Quicken~your~pace!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
        //                        else if (MsgTournaments.MsgSchedules.SquidWardOctopus.CopperRemain == 10)
        //                            Server.SendGlobalPacket(new MsgMessage("Mammon~Envoy~only~have~10~pieces~of~rewards~left~for~the~Copper~Coins,~now.~Quicken~your~pace!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
        //                        else if (MsgTournaments.MsgSchedules.SquidWardOctopus.CopperRemain <= 0)
        //                            Server.SendGlobalPacket(new MsgMessage("Pay~attention!~All~the~rewards~for~the~Copper~Coins~have~been~taken.~Hurry~to~exchange~your~other~kinds~of~coins!", MsgMessage.MsgColor.red, MsgMessage.ChatMode.System).GetArray(stream));
        //                    }
        //                    else
        //                    {
        //                        data.AddText("Stop~kidding~me,~okay?~You~don`t~have~any~Copper~Coins~with~you.~A~hero~wouldn`t~try~to~cheat~someone!");
        //                        data.AddOption("Sorry,~bye!", 255);
        //                        data.AddAvatar(60);
        //                        data.FinalizeDialog();
        //                    }

        //                }
        //                else
        //                {
        //                    data.AddText("Sigh...~Sorry,~but~you~are~too~late.~All~the~rewards~I~prepared~for~the~Copper~Coins~have~been~taken.~Come~to~exchange~the~Copper~Coins~earlier,~next~time.");
        //                    data.AddOption("Alright.", 255);
        //                    data.AddAvatar(60);
        //                    data.FinalizeDialog();
        //                }

        //                break;
        //            }
        //        case 40:
        //            {
        //                client.Teleport(302, 279, 10364);
        //                break;
        //            }
        //    }
        //}

        //[NpcAttribute(NpcID.SquidwardOctopus)]
        //public static void SquidwardOctopus(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                if (MsgTournaments.MsgSchedules.SquidWardOctopus.Process == MsgTournaments.ProcesType.Alive)
        //                {
        //                    data.AddText("Ladies~and~gentlemen,~the~`Treasure~In~The~Blue`~has~started!~No~matter~you~are~male~or~female,~old~or~young,~if~you~have~interested~in~treasure~hunting~or~pirates~and~have~reached,");
        //                    data.AddText("~Level~80,~sign~up~with~me~for~a~sea~adventure!~Rewards?~Of~course!");
        //                    data.AddOption("Send~me~to~the~Proud~Sea!", 30);
        //                    data.AddOption("Treasure~hunting?", 1);
        //                    data.AddOption("What~are~the~rewards?", 20);
        //                    data.AddOption("I`ll~think~it~over.", 255).FinalizeDialog();
        //                }
        //                else
        //                {
        //                    data.AddText("Hey! Have you ever heard the Proud Sea? Yeah, I mean the place where lots of pirates` ships sank and treasures lost. Well, our ship will be at sea at 12:30-13:29 and 20:30-21:29, Monday - Saturday.")
        //                                       .AddText("~Want to join us to hunt treasures? I promise that will be an unforgettable adventure. Moreover, you can win great rewards, if you find us the treasures. I await your decision!")
        //                                                  .AddOption("Treasure~hunting?", 1)
        //                                                  .AddOption("What~are~the~rewards?", 20)
        //                                         .AddOption("I`ll~think~it~over.", 255).FinalizeDialog();
        //                }
        //                break;
        //            }
        //        case 30:
        //            {
        //                data.AddText("It~seems~you~have~made~up~the~mind.~Wait,~have~you~put~away~all~the~valuables?~You~know,~the~Proud~Sea~is~so~dangerous~that~you~wouldn`t~know~what~strong~enemies~you~may~meet.~So,~I~suggest~you~recheck~your~inventory.");
        //                data.AddOption("Don`t~worry.~I`m~ready.", 31);
        //                data.AddOption("I`ll~be~back~soon.", 255);
        //                data.FinalizeDialog();
        //                break;
        //            }
        //        case 31:
        //            {
        //                if (client.Player.Level < 80)
        //                {
        //                    data.AddText("Um...~You~are~still~too~young.~Keep~practicing,~and~come~back~when~you~reach~Level~80.");
        //                    data.AddOption("Alright.", 255).FinalizeDialog();
        //                    break;
        //                }
        //                if (MsgTournaments.MsgSchedules.SquidWardOctopus.Process == MsgTournaments.ProcesType.Alive)
        //                {
        //                    byte rand = (byte)Pool.GetRandom.Next(0, 4);
        //                    switch (rand)
        //                    {
        //                        case 0:
        //                            client.Teleport(251, 392, 3071);
        //                            break;
        //                        case 1:
        //                            client.Teleport(72, 131, 3071);
        //                            break;
        //                        case 2:
        //                            client.Teleport(197, 64, 3071);
        //                            break;
        //                        case 3:
        //                            client.Teleport(348, 208, 3071);
        //                            break;

        //                    }

        //                }
        //                else
        //                {

        //                    data.AddText("Wow,~I~didn`t~expect~you~would~make~a~decision~so~fast,~but~the~ship~has~been~at~sea.~Why~not~come~earlier~next~time?~Remember~the~ship~will~put~to~sea~at~12:30-13:29~and~20:30-21:29~every~day.");
        //                    data.AddText("~Hope~to~see~you~next~time.");
        //                    data.AddOption("See~you.", 255).FinalizeDialog();
        //                }

        //                break;
        //            }

        //    }
        //}

        //[NpcAttribute(NpcID.PrizeCenterTeleporter1)]
        //public static void PrizeCenterTeleporter1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("Hello!~You~want~to~exchange~for~rewards~of~the~`Treasure~In~The~Blue`~event?~I~can~send~you~to~the~Prize~Center.~If~I~just~sent~you~to~somewhere,~you~need~to~wait~3~minutes~to~use");
        //                data.AddText("~the~teleporter~again,~since~there~are~too~many~people.~So,~where~do~you~want~to~go?");
        //                data.AddOption("The~Prize~Center.", 1);
        //                data.AddOption("Twin~City.", 2);
        //                data.AddOption("I`m~just~passing~by.", 255);
        //                data.AddAvatar(14).FinalizeDialog();
        //                break;
        //            }
        //        case 2:
        //            {
        //                client.Teleport(302, 279, 10364);

        //                break;
        //            }
        //        case 1:
        //            {
        //                if (DateTime.Now > client.Player.JoinPrizeNpcOctopus.AddSeconds(180))
        //                {

        //                    client.Teleport(33, 33, 1068);
        //                    client.Player.JoinPrizeNpcOctopus = DateTime.Now;
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry,~but~I~remember~I~have~sent~you~to~somewhere~just~now~and~it~hasn`t~been~3~minutes~since~that.~Please~be~patient,~okay?~");
        //                    data.AddOption("Alright.", 255);
        //                    data.AddAvatar(14).FinalizeDialog();

        //                }
        //                break;
        //            }
        //    }
        //}
        //[NpcAttribute(NpcID.PrizeCenterTeleporter2)]
        //public static void PrizeCenterTeleporter2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("Hello!~You~want~to~exchange~for~rewards~of~the~`Treasure~In~The~Blue`~event?~I~can~send~you~to~the~Prize~Center.~If~I~just~sent~you~to~somewhere,~you~need~to~wait~3~minutes~to~use");
        //                data.AddText("~the~teleporter~again,~since~there~are~too~many~people.~So,~where~do~you~want~to~go?");
        //                data.AddOption("The~Prize~Center.", 1);
        //                data.AddOption("Twin~City.", 2);
        //                data.AddOption("I`m~just~passing~by.", 255);
        //                data.AddAvatar(14).FinalizeDialog();
        //                break;
        //            }
        //        case 2:
        //            {
        //                client.Teleport(302, 279, 10364);

        //                break;
        //            }
        //        case 1:
        //            {
        //                if (DateTime.Now > client.Player.JoinPrizeNpcOctopus.AddSeconds(180))
        //                {

        //                    client.Teleport(33, 33, 1068);
        //                    client.Player.JoinPrizeNpcOctopus = DateTime.Now;
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry,~but~I~remember~I~have~sent~you~to~somewhere~just~now~and~it~hasn`t~been~3~minutes~since~that.~Please~be~patient,~okay?~");
        //                    data.AddOption("Alright.", 255);
        //                    data.AddAvatar(14).FinalizeDialog();

        //                }
        //                break;
        //            }
        //    }
        //}
        //[NpcAttribute(NpcID.PrizeCenterTeleporter3)]
        //public static void PrizeCenterTeleporter3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("Hello!~You~want~to~exchange~for~rewards~of~the~`Treasure~In~The~Blue`~event?~I~can~send~you~to~the~Prize~Center.~If~I~just~sent~you~to~somewhere,~you~need~to~wait~3~minutes~to~use");
        //                data.AddText("~the~teleporter~again,~since~there~are~too~many~people.~So,~where~do~you~want~to~go?");
        //                data.AddOption("The~Prize~Center.", 1);
        //                data.AddOption("Twin~City.", 2);
        //                data.AddOption("I`m~just~passing~by.", 255);
        //                data.AddAvatar(14).FinalizeDialog();
        //                break;
        //            }
        //        case 2:
        //            {
        //                client.Teleport(302, 279, 10364);

        //                break;
        //            }
        //        case 1:
        //            {
        //                if (DateTime.Now > client.Player.JoinPrizeNpcOctopus.AddSeconds(180))
        //                {

        //                    client.Teleport(33, 33, 1068);
        //                    client.Player.JoinPrizeNpcOctopus = DateTime.Now;
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry,~but~I~remember~I~have~sent~you~to~somewhere~just~now~and~it~hasn`t~been~3~minutes~since~that.~Please~be~patient,~okay?~");
        //                    data.AddOption("Alright.", 255);
        //                    data.AddAvatar(14).FinalizeDialog();

        //                }
        //                break;
        //            }
        //    }
        //}
        //[NpcAttribute(NpcID.PrizeCenterTeleporter4)]
        //public static void PrizeCenterTeleporter4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("Hello!~You~want~to~exchange~for~rewards~of~the~`Treasure~In~The~Blue`~event?~I~can~send~you~to~the~Prize~Center.~If~I~just~sent~you~to~somewhere,~you~need~to~wait~3~minutes~to~use");
        //                data.AddText("~the~teleporter~again,~since~there~are~too~many~people.~So,~where~do~you~want~to~go?");
        //                data.AddOption("The~Prize~Center.", 1);
        //                data.AddOption("Twin~City.", 2);
        //                data.AddOption("I`m~just~passing~by.", 255);
        //                data.AddAvatar(14).FinalizeDialog();
        //                break;
        //            }
        //        case 2:
        //            {
        //                client.Teleport(302, 279, 10364);

        //                break;
        //            }
        //        case 1:
        //            {
        //                if (DateTime.Now > client.Player.JoinPrizeNpcOctopus.AddSeconds(180))
        //                {

        //                    client.Teleport(33, 33, 1068);
        //                    client.Player.JoinPrizeNpcOctopus = DateTime.Now;
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry,~but~I~remember~I~have~sent~you~to~somewhere~just~now~and~it~hasn`t~been~3~minutes~since~that.~Please~be~patient,~okay?~");
        //                    data.AddOption("Alright.", 255);
        //                    data.AddAvatar(14).FinalizeDialog();

        //                }
        //                break;
        //            }
        //    }
        //}
        //[NpcAttribute(NpcID.PrizeCenterTeleporter5)]
        //public static void PrizeCenterTeleporter5(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("Hello!~You~want~to~exchange~for~rewards~of~the~`Treasure~In~The~Blue`~event?~I~can~send~you~to~the~Prize~Center.~If~I~just~sent~you~to~somewhere,~you~need~to~wait~3~minutes~to~use");
        //                data.AddText("~the~teleporter~again,~since~there~are~too~many~people.~So,~where~do~you~want~to~go?");
        //                data.AddOption("The~Prize~Center.", 1);
        //                data.AddOption("Twin~City.", 2);
        //                data.AddOption("I`m~just~passing~by.", 255);
        //                data.AddAvatar(14).FinalizeDialog();
        //                break;
        //            }
        //        case 2:
        //            {
        //                client.Teleport(302, 279, 10364);

        //                break;
        //            }
        //        case 1:
        //            {
        //                if (DateTime.Now > client.Player.JoinPrizeNpcOctopus.AddSeconds(180))
        //                {

        //                    client.Teleport(33, 33, 1068);
        //                    client.Player.JoinPrizeNpcOctopus = DateTime.Now;
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry,~but~I~remember~I~have~sent~you~to~somewhere~just~now~and~it~hasn`t~been~3~minutes~since~that.~Please~be~patient,~okay?~");
        //                    data.AddOption("Alright.", 255);
        //                    data.AddAvatar(14).FinalizeDialog();

        //                }
        //                break;
        //            }
        //    }
        //}
        //[NpcAttribute(NpcID.PrizeCenterTeleporter6)]
        //public static void PrizeCenterTeleporter6(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("Hello!~You~want~to~exchange~for~rewards~of~the~`Treasure~In~The~Blue`~event?~I~can~send~you~to~the~Prize~Center.~If~I~just~sent~you~to~somewhere,~you~need~to~wait~3~minutes~to~use");
        //                data.AddText("~the~teleporter~again,~since~there~are~too~many~people.~So,~where~do~you~want~to~go?");
        //                data.AddOption("The~Prize~Center.", 1);
        //                data.AddOption("Twin~City.", 2);
        //                data.AddOption("I`m~just~passing~by.", 255);
        //                data.AddAvatar(14).FinalizeDialog();
        //                break;
        //            }
        //        case 2:
        //            {
        //                client.Teleport(302, 279, 10364);

        //                break;
        //            }
        //        case 1:
        //            {
        //                if (DateTime.Now > client.Player.JoinPrizeNpcOctopus.AddSeconds(180))
        //                {

        //                    client.Teleport(33, 33, 1068);
        //                    client.Player.JoinPrizeNpcOctopus = DateTime.Now;
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry,~but~I~remember~I~have~sent~you~to~somewhere~just~now~and~it~hasn`t~been~3~minutes~since~that.~Please~be~patient,~okay?~");
        //                    data.AddOption("Alright.", 255);
        //                    data.AddAvatar(14).FinalizeDialog();

        //                }
        //                break;
        //            }
        //    }
        //}
        //[NpcAttribute(NpcID.PrizeCenterTeleporter7)]
        //public static void PrizeCenterTeleporter7(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("Hello!~You~want~to~exchange~for~rewards~of~the~`Treasure~In~The~Blue`~event?~I~can~send~you~to~the~Prize~Center.~If~I~just~sent~you~to~somewhere,~you~need~to~wait~3~minutes~to~use");
        //                data.AddText("~the~teleporter~again,~since~there~are~too~many~people.~So,~where~do~you~want~to~go?");
        //                data.AddOption("The~Prize~Center.", 1);
        //                data.AddOption("Twin~City.", 2);
        //                data.AddOption("I`m~just~passing~by.", 255);
        //                data.AddAvatar(14).FinalizeDialog();
        //                break;
        //            }
        //        case 2:
        //            {
        //                client.Teleport(302, 279, 10364);

        //                break;
        //            }
        //        case 1:
        //            {
        //                if (DateTime.Now > client.Player.JoinPrizeNpcOctopus.AddSeconds(180))
        //                {

        //                    client.Teleport(33, 33, 1068);
        //                    client.Player.JoinPrizeNpcOctopus = DateTime.Now;
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry,~but~I~remember~I~have~sent~you~to~somewhere~just~now~and~it~hasn`t~been~3~minutes~since~that.~Please~be~patient,~okay?~");
        //                    data.AddOption("Alright.", 255);
        //                    data.AddAvatar(14).FinalizeDialog();

        //                }
        //                break;
        //            }
        //    }
        //}
        //[NpcAttribute(NpcID.PrizeCenterTeleporter8)]
        //public static void PrizeCenterTeleporter8(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        //{
        //    Dialog data = new Dialog(client, stream);
        //    switch (Option)
        //    {
        //        case 0:
        //            {
        //                data.AddText("Hello!~You~want~to~exchange~for~rewards~of~the~`Treasure~In~The~Blue`~event?~I~can~send~you~to~the~Prize~Center.~If~I~just~sent~you~to~somewhere,~you~need~to~wait~3~minutes~to~use");
        //                data.AddText("~the~teleporter~again,~since~there~are~too~many~people.~So,~where~do~you~want~to~go?");
        //                data.AddOption("The~Prize~Center.", 1);
        //                data.AddOption("Twin~City.", 2);
        //                data.AddOption("I`m~just~passing~by.", 255);
        //                data.AddAvatar(14).FinalizeDialog();
        //                break;
        //            }
        //        case 2:
        //            {
        //                client.Teleport(302, 279, 10364);

        //                break;
        //            }
        //        case 1:
        //            {
        //                if (DateTime.Now > client.Player.JoinPrizeNpcOctopus.AddSeconds(180))
        //                {

        //                    client.Teleport(33, 33, 1068);
        //                    client.Player.JoinPrizeNpcOctopus = DateTime.Now;
        //                }
        //                else
        //                {
        //                    data.AddText("Sorry,~but~I~remember~I~have~sent~you~to~somewhere~just~now~and~it~hasn`t~been~3~minutes~since~that.~Please~be~patient,~okay?~");
        //                    data.AddOption("Alright.", 255);
        //                    data.AddAvatar(14).FinalizeDialog();

        //                }
        //                break;
        //            }
        //    }
        //}
        [NpcAttribute(NpcID.CloudSweeper)]
        public static void CloudSweeper(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Every day, when you team up with at least one friend, and conquer the Tower of Mystery at any challenge mode,")
                            .AddText("~you can claim a reward from me.")
                .AddOption("Claim~my~team~reward.", 1).AddAvatar(6)
         .FinalizeDialog();
                        break;
                    }
                case 1:
                    {

                        if (client.Player.TOMClaimTeamReward == 0)
                        {
                            if (client.Player.TowerOfMysterychallenge == 0)
                            {
                                if (!client.Inventory.HaveSpace(1))
                                {
                                    data.AddText("Please make 1 more space in your inventory.")
                                        .AddOption("Let me check.", 255).AddAvatar(6)
                                        .FinalizeDialog();
                                    break;
                                }
                                client.Inventory.Add(stream, 3008733);
                                client.Player.TOMClaimTeamReward = 1;
                                data.AddText("Ah, you`ve completed the challenge nicely. Here is a reward for you.")
               .AddOption("Thanks!", 255).AddAvatar(6)
        .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("To claim a reward from me, you need to team up with your friends and conquere the Tower of Mystery.")
                 .AddOption("I~see.", 255).AddAvatar(6)
          .FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("Look, you`ve already claimed a reward from me. No more for today.")
             .AddOption("Sorry,~I~forgot.", 255).AddAvatar(6)
      .FinalizeDialog();
                        }
                        break;
                    }
            }


        }


     //   [NpcAttribute(NpcID.PakYan)]
     //   public static void PakYan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
     //   {


     //       var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.GuidofFieryDragon, 0, 3631);
     //       var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.PakYan, 0, 3632);
     //       var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.FlameAltar, 0, 3649);
     //       Dialog data = new Dialog(client, stream);
     //       switch (Option)
     //       {
     //           case 0:
     //               {
     //                   if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
     //                   {
     //                       data.AddText("Good to see you back! Is everything all right?")
     //                .AddOption("I~have~good~news~for~you.", 4).AddAvatar(67)
     //         .FinalizeDialog();
     //                       break;
     //                   }
     //                   if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
     //                   {
     //                       data.AddText("Are you here to give our Bright Tribe a hand?")
     //                  .AddOption("Yes, exactly", 1).AddAvatar(67)
     //           .FinalizeDialog();
     //                       break;
     //                   }
     //                   else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
     //                   {
     //                       if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
     //                       {

     //                           data.AddText("Do you have any news about the Elder and others? I`m really worried about them.")
     //              .AddOption("I`ll go and check.", 3).AddAvatar(67)
     //       .FinalizeDialog();
     //                           break;
     //                       }
     //                       data.AddText("Two days ago, Chong Yan Elder led a group to find the Flame Temple which is not only our ")
     //                    .AddText("tribe`s holy zone, but also they key to complety activate the Mystery Pearl and stop the ")
     //                    .AddText("Fiery Dragon Volcano from eruption, However, there is still no message from them.")
     //              .AddOption("I'll go find them.", 3)
     //              .AddOption("Don`t worry, they`ll be fine.", 255).AddAvatar(67)
     //       .FinalizeDialog();
     //                   }
     //                   else
     //                   {
     //                       data.AddText("We`ve finally arrived home, and we`ll spare no effort to restore our")
     //                           .AddText("~homeland to its former glory.")
     //           .AddOption("Bright~Fortune.~(Daily)", 10).AddAvatar(67)
     //    .FinalizeDialog();
     //                   }

     //                   break;
     //               }
     //           case 10:
     //               {
     //                   if (client.Player.ClaimTowerAmulets == 0)
     //                   {
     //                       data.AddText("For our tribe`s helpers, I`ll never be stingy with rewards for them. Under the Fiery Dragon region, there are not only massive magma chambers,")
     //                         .AddText("~but also mythical treasures. Are you interested? You can claim Hunting Amulets from me every day. Wish you a harvest!")
     //                         .AddText("    Reward: <tip color=0xfff05050 desc=Bloody#Jade,#Bright#Star#Stone,#Blazing#CP#Fragment,#Champion#Points,#etc.>Treasure of Dragon</tip>")
     //         .AddOption("Claim~5~Hunting~Amulets.", 11).AddAvatar(67)
     //  .FinalizeDialog();
     //                   }
     //                   else
     //                   {
     //                       data.AddText("How is your treasure hunting today? It must be a harvest! If you`re still interested, come to me tomorrow.")
     //             .AddOption("Great!", 255).AddAvatar(67)
     //      .FinalizeDialog();
     //                   }
     //                   break;
     //               }
     //           case 11:
     //               {
     //                   if (!client.Inventory.HaveSpace(1))
     //                   {
     //                       data.AddText("Please make 1 more spaces in you inventory.")
     //              .AddOption("Let me check.", 255).AddAvatar(67)
     //       .FinalizeDialog();
     //                       break;
     //                   }
     //                   if (client.Player.ClaimTowerAmulets == 0)
     //                   {
     //                       client.Player.ClaimTowerAmulets = 1;
     //                       client.Inventory.AddItemWitchStack(3008993, 0, 5, stream);
     //                       client.Teleport(40, 347, 3998);
     //                       client.CreateBoxDialog("The Chief of Bright Tribe gave you 5 Hunting Amulets which will bring you surprises.");
     //                   }
     //                   break;
     //               }
     //           case 4:
     //               {
     //                   data.AddText("Really? We`ve been waiting hundreds of years for this moment! We can finally worship")
     //                       .AddText("~our ancestors in the Flame Temple. From now on, you`re the most honored friend to")
     //                       .AddText("~the Bright Tribe. I don`t know how to thank you enough.")
     //               .AddOption("It`s~my~pleasure.", 5).AddAvatar(67)
     //        .FinalizeDialog();
     //                   break;
     //               }
     //           case 5:
     //               {
     //                   data.AddText("The Elder and others have been rescued when you were on the journey to find the Flame Temple.")
     //                         .AddText("~They just can`t wait to see the Flame Altar in front of the temple. If the Mystery Pearl can be activated")
     //                         .AddText("~at the altar, we`ll be able to prevent the Fiery Dragon Volcano from eruption. Would you like to join us?")
     //                       .AddOption("Sure.", 6).AddAvatar(67)
     //                       .FinalizeDialog();
     //                   break;
     //               }
     //           case 6:
     //               {
     //                   if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
     //                   {
     //                       client.Player.Money += 1000;
     //                       client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
     //                       client.Inventory.Add(stream, 3200344, 3);
     //                   }
     //                   var action = new ActionQuery()
     //                   {
     //                       ObjId = client.Player.UID,
     //                       Type = ActionType.DrawStory,
     //                       dwParam = 1004

     //                   };
     //                   client.Send(stream.ActionCreate(action));
     //                   break;
     //               }
     //           case 1:
     //               {
     //                   if (!client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
     //                       client.Player.QuestGUI.Accept(ActiveQuest, 0);
     //                   var action = new ActionQuery()
     //                   {
     //                       ObjId = client.Player.UID,
     //                       Type = ActionType.DrawStory,
     //                       dwParam = 1001

     //                   };
     //                   client.Send(stream.ActionCreate(action));
     //                   data.AddText("For many times, I dreamed of returning home, but forgot almost everything upon waking.It`s saied ")
     //                       .AddText("the Fiery Dragon was a prosperous land, and there Bright ancestors lived in a golden age. Look at")
     //                       .AddText(" it now, our homeland has become utterly desolate. It`s really depressing.")
     //                 .AddOption("We still have hope.", 2).AddAvatar(67)
     //          .FinalizeDialog();
     //                   break;
     //               }
     //           case 2:
     //               {
     //                   if (!client.Inventory.HaveSpace(3))
     //                   {
     //                       data.AddText("Please make 3 more spaces in you inventory.")
     //              .AddOption("Let me check.", 255).AddAvatar(67)
     //       .FinalizeDialog();
     //                       break;
     //                   }
     //                   if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
     //                   {
     //                       client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);
     //                       client.SendSysMesage("You received 3 Twilight Star Stones!", MsgMessage.ChatMode.System);
     //                       client.Player.Money += 1000;
     //                       client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
     //                       client.Inventory.Add(stream, 3009000, 3);
     //                   }
     //                   data.AddText("Two days ago, Chong Yan Elder led a group to find the Flame Temple which is not only our ")
     //                      .AddText("tribe`s holy zone, but also they key to complety activate the Mystery Pearl and stop the ")
     //                      .AddText("Fiery Dragon Volcano from eruption, However, there is still no message from them.")
     //                .AddOption("I'll go find them.", 3)
     //                .AddOption("Don`t worry, they`ll be fine.", 255).AddAvatar(67)
     //         .FinalizeDialog();
     //                   break;
     //               }
     //           case 3:
     //               {
     //                   client.Player.QuestGUI.Accept(ActiveQuest2, 0);
     //                   client.Player.QuestGUI.SendAutoPatcher(ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
     //                   break;
     //               }
     //       }
     //   }

     //   [NpcAttribute(NpcID.FlameAltar)]
     //   public static void FlameAltar(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
     //   {
     //       var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ChingYan, 0, 3643);
     //       var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.FlameAltar, 0, 3644);
     //       var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.FlameAltar, 0, 3645);
     //       var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.FlameAltar, 0, 3646);
     //       var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.FlameAltar, 0, 3647);
     //       var ActiveQuest6 = Database.QuestInfo.GetFinishQuest((uint)NpcID.FlameAltar, 0, 3648);
     //       var ActiveQuest7 = Database.QuestInfo.GetFinishQuest((uint)NpcID.FlameAltar, 0, 3649);
     //       Dialog data = new Dialog(client, stream);
     //       switch (Option)
     //       {
     //           case 0:
     //               {
     //                   if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
     //                   {
     //                       data.AddText("You`ve finally arrived at relics of the Flame Temple. At the center of the temple square,")
     //                           .AddText("~there is a ruined pentagonal altar carved with complex patterns at each side.")
     //         .AddOption("It`s~amazing!", 1)
     //  .FinalizeDialog();
     //                       break;
     //                   }
     //                   if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
     //                   {
     //                       if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
     //                       {
     //                           data.AddText("(Hurry and go collect Building Stones to repair the ruined altar.)")
     //              .AddOption("Use~stones~to~repair~altar.", 3)
     //       .FinalizeDialog();
     //                       }
     //                       else
     //                       {
     //                           data.AddText("(You found two sides of the altar were seriously damaged, and Metal and Fire icons were absent from the elemental patterns.")
     //                         .AddText("~You should restore the Flame Altar, first. Look around and collect appropriate Building Stones.")
     //       .AddOption("Go~find~Building~Stones.", 2)
     //.FinalizeDialog();
     //                       }

     //                   }
     //                   else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
     //                   {
     //                       if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
     //                       {
     //                           data.AddText("(The altar is dead without the Wheel of Nature.")
     //                               .AddText("~Hurry and retrieve it from the Clawed Rock Devils.)")
     //              .AddOption("Place~the~wheel~on~the~altar.", 10)
     //       .FinalizeDialog();
     //                       }
     //                       else
     //                       {
     //                           data.AddText("(To further restore the altar, you`ll need to switch on the five elemental forces resting in the Wheel of Nature.")
     //                     .AddText("~But where is the Wheel of Nature? It must be stolen by the Clawed Rock Devils nearby.")
     //                     .AddText("~Go retrieve the wheel from the Clawed Rock Devils.)")
     //                     .AddOption("Go~retrieve~the~wheel.", 6)
     //                     .FinalizeDialog();
     //                       }

     //                   }
     //                   else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
     //                   {
     //                       if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
     //                       {
     //                           data.AddText("(The Metal Force is essential to motivate the Mystery Pearl. With the Metal Force,")
     //                               .AddText("~you can light the Metal icon on the altar.)")
     //              .AddOption("Spin~the~Wheel~of~Nature.", 13)
     //       .FinalizeDialog();
     //                       }
     //                       else
     //                       {
     //                           data.AddText("(It`s said the Mystery Pearl contains terrific power which can only be motivated by the purest Metal Force.")
     //                    .AddText("~Is it true? If you have the Earth Force, please transform it into Metal Force on the Wheel of Nature.)")
     //                    .AddOption("Go~make~the~Metal~Force.", 12)
     //                    .FinalizeDialog();
     //                       }

     //                   }
     //                   else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
     //                   {
     //                       if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
     //                       {
     //                           data.AddText("(Put the Lava Flower on the Wheel of Nature, and spin the wheel to extract Fire Force.")
     //                               .AddText("~When you light the Fire icon with the Fire Force, the Flame Altar will be fully restored and activated.)")
     //              .AddOption("Extract~the~Fire~Force.", 20)
     //       .FinalizeDialog();
     //                       }
     //                       else
     //                       {
     //                           data.AddText("(It`s said the Mystery Pearl contains terrific power which can only be motivated by the purest Metal Force.")
     //                    .AddText("~Is it true? If you have the Earth Force, please transform it into Metal Force on the Wheel of Nature.)")
     //                    .AddOption("Go~make~the~Metal~Force.", 15)
     //                    .FinalizeDialog();
     //                       }

     //                   }
     //                   else if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
     //                   {
     //                       if (client.Player.QuestGUI.CheckQuest(ActiveQuest6.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
     //                       {
     //                           data.AddText("(Insert the Essence of Star into the eyes of the altar, and you`ll achieve the final success against the Fiery Dragon Volcano.)")
     //              .AddOption("Extract~the~Essence~of~Star.", 23)
     //       .FinalizeDialog();
     //                       }
     //                       else
     //                       {
     //                           data.AddText("(It requires a large number of Star Ores to extract the Essence of Star.")
     //                            .AddText("~The Hellfire Beasts are known to carry such ores.")
     //           .AddOption("Go~collect~Star~Ores.", 22).FinalizeDialog();
     //                       }

     //                   }
     //                   else if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
     //                   {
     //                       if (client.Player.QuestGUI.CheckQuest(ActiveQuest7.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
     //                       {
     //                           data.AddText("(You`ve finished your job here. Hurry and report back to Chief Pak Yan.)")
     //           .AddOption("Okay.", 25)
     //    .FinalizeDialog();
     //                       }
     //                       else
     //                       {
     //                           data.AddText("(Everything is ready. Hurry and report back to Chief Pak Yan!)")
     //           .AddOption("Okay.", 25).FinalizeDialog();
     //                       }

     //                   }
     //                   else
     //                   {

     //                       data.AddText("(The Flame Altar is a special place for the Bright tribe to hold grand sacrifice events.)")
     //               .AddOption("I~see.", 255).FinalizeDialog();
     //                   }
     //                   break;
     //               }
     //           case 25:
     //               {
     //                   client.Player.QuestGUI.Accept(ActiveQuest7, 0);
     //                   client.Player.QuestGUI.SendAutoPatcher(ActiveQuest7.FinishNpcId.Map, ActiveQuest7.FinishNpcId.X, ActiveQuest7.FinishNpcId.Y, ActiveQuest7.FinishNpcId.ID);
     //                   break;
     //               }
     //           case 23:
     //               {
     //                   if (client.Inventory.Contain(3008748, 100))
     //                   {
     //                       data.AddText("(With the aid of the Essence of Star, the energy of Flame Altar becomes steady.")
     //       .AddText("~You`ve finished your job. Hurry and report back to Chief Pak Yan. He`ll take care of the rest")
     //       .AddText("~to suppress the Fiery Dragon Volcano with the Mystery Pearl.)")
     //                       .AddOption("I`ve~finally~made~it!", 24)
     // .FinalizeDialog();
     //                   }
     //                   else
     //                   {
     //                       data.AddText("(The airflow around the altar is getting unstable. You need to insert the Essence of Star into the eyes of the altar quickly.")
     //           .AddText("~Don`t jinx it. Hurry and go collect Star Ores from the Hellfire Beasts.)")
     //                           .AddOption("I`m~on~the~way.", 22)
     //     .FinalizeDialog();
     //                   }
     //                   break;
     //               }
     //           case 24:
     //               {
     //                   if (!client.Inventory.HaveSpace(2))
     //                   {
     //                       data.AddText("Please make 2 more spaces in you inventory.")
     //              .AddOption("Let me check.", 255)
     //       .FinalizeDialog();
     //                       break;
     //                   }
     //                   if (client.Player.QuestGUI.FinishQuest(ActiveQuest6.MissionId))
     //                   {

     //                       client.SendSysMesage("You received a Radiant Star Stone!", MsgMessage.ChatMode.System);
     //                       client.Inventory.RemoveStackItem(3008748, 100, stream);
     //                       client.Inventory.Add(stream, 3200346);
     //                       client.Inventory.Add(stream, 3009002);
     //                       client.Player.Money += 5000;
     //                       client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
     //                   }
     //                   data.AddText("(You`ve finished your job here. Hurry and report back to Chief Pak Yan.)")
     //            .AddOption("Okay.", 25)
     //     .FinalizeDialog();
     //                   break;
     //               }
     //           case 22:
     //               {
     //                   client.Player.QuestGUI.Accept(ActiveQuest6, 0);
     //                   client.Player.QuestGUI.SendAutoPatcher(3998, 522, 347, 0);
     //                   break;
     //               }
     //           case 21:
     //               {
     //                   if (!client.Inventory.HaveSpace(1))
     //                   {
     //                       data.AddText("Please make 1 more space in you inventory.")
     //              .AddOption("Let me check.", 255)
     //       .FinalizeDialog();
     //                       break;
     //                   }
     //                   if (client.Player.QuestGUI.FinishQuest(ActiveQuest5.MissionId))
     //                   {
     //                       client.Inventory.Remove(3008747, 10, stream);
     //                       client.Inventory.Add(stream, 3200345);
     //                       client.Player.Money += 5000;
     //                       client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
     //                   }
     //                   data.AddText("(It requires a large number of Star Ores to extract the Essence of Star.")
     //                           .AddText("~The Hellfire Beasts are known to carry such ores.")
     //          .AddOption("Go~collect~Star~Ores.", 22).FinalizeDialog();
     //                   break;
     //               }
     //           case 20:
     //               {
     //                   if (client.Inventory.Contain(3008747, 10))
     //                   {
     //                       data.AddText("(With the recovery of the Fire icon, the five elemental icons shine more brightly. You`re now very close to")
     //                        .AddText("~the ultimate success: place the Essence of Star at the eye of the altar to comfort the restless airflow,")
     //                        .AddText("~and then burn the power of Mystery Pearl to completely freeze the Fiery Dragon Volcano.)")
     //                        .AddOption("I`ll~try~my~best.", 21)
     //                        .FinalizeDialog();
     //                   }
     //                   else
     //                   {
     //                       data.AddText("(To fully activated the Flame Altar, you still need the Fire Force which can be extracted")
     //                           .AddText("~from the flowers blooming by the Lava Server.)")
     //                           .AddOption("Head~to~the~Lava~Server.", 15)
     //                           .FinalizeDialog();

     //                   }

     //                   break;
     //               }
     //           case 13:
     //               {
     //                   if (client.Inventory.Contain(3008745, 1))
     //                   {
     //                       data.AddText("(Rolling slowly, the Wheel of Nature successfully transformed the Earth Force into Metal Force,")
     //                            .AddText("~with the Metal icon shining brightly. Now, you can turn to switch on the Fire Force.)")
     //           .AddOption("Everything~will~go~well.", 14)
     //    .FinalizeDialog();
     //                   }
     //                   else
     //                   {
     //                       data.AddText("You don`t have the Earth Force which can be transformed into the Metal Force through the Wheel of Nature.")
     //                              .AddText("~When enough Essences of Earth are collected, you can extract the Earth Force from them.")
     //             .AddOption("Go~collect~Essence~of~Earth.", 12)
     //      .FinalizeDialog();
     //                   }
     //                   break;
     //               }
     //           case 15:
     //               {
     //                   client.Player.QuestGUI.Accept(ActiveQuest5, 0);
     //                   client.Player.QuestGUI.SendAutoPatcher(3998, 559, 685, 0);
     //                   break;
     //               }
     //           case 14:
     //               {
     //                   if (!client.Inventory.HaveSpace(1))
     //                   {
     //                       data.AddText("Please make 1 more space in you inventory.")
     //              .AddOption("Let me check.", 255)
     //       .FinalizeDialog();
     //                       break;
     //                   }
     //                   if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
     //                   {
     //                       client.Inventory.Remove(3008745, 1, stream);
     //                       client.Inventory.Add(stream, 3200346);
     //                       client.Player.Money += 5000;
     //                       client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
     //                   }
     //                   data.AddText("(The Metal element has been recovered, and it`s time to explore the Fire Force.")
     //                           .AddText("~As the totem of the Bright tribe, Fire is essential to their altar.")
     //                           .AddText("~Not far away from the Flame Temple, there is a Lava Server where special flowers are blooming.")
     //                           .AddText("~You can extract the purest Fire Force from the Lava Flowers.)")
     //          .AddOption("Head~to~the~Lava~Server.", 15).FinalizeDialog();
     //                   break;
     //               }
     //           case 16:
     //               {
     //                   break;
     //               }
     //           case 10:
     //               {
     //                   if (client.Inventory.Contain(3008743, 1))
     //                   {
     //                       data.AddText("(You placed the Wheel of Nature at the center of the altar, and saw the icons of five elements")
     //                            .AddText("~giving out faint light. What a good beginning! Before burning the power of Mystery Pearl, you")
     //                            .AddText("~need to switch on the five elemental forces through the Wheel of Nature.")
     //           .AddOption("Good.", 11)
     //    .FinalizeDialog();
     //                   }
     //                   else
     //                   {
     //                       data.AddText("(The Wheel of Nature is the key to fully restore the altar,")
     //                              .AddText("~which is also a must to energize the Mystery Pearl. Hurry and retrieve the wheel from the Clawed Rock Devils.)")
     //             .AddOption("I`m~on~the~way.", 6)
     //      .FinalizeDialog();
     //                   }
     //                   break;
     //               }
     //           case 12:
     //               {
     //                   client.Player.QuestGUI.Accept(ActiveQuest4, 0);
     //                   client.Player.QuestGUI.SendAutoPatcher(3998, 482, 216, 0);
     //                   break;
     //               }
     //           case 11:
     //               {
     //                   if (!client.Inventory.HaveSpace(1))
     //                   {
     //                       data.AddText("Please make 1 more space in you inventory.")
     //              .AddOption("Let me check.", 255)
     //       .FinalizeDialog();
     //                       break;
     //                   }
     //                   if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
     //                   {
     //                       client.Inventory.Remove(3008743, 1, stream);
     //                       client.Inventory.Add(stream, 3200346);
     //                       client.Player.Money += 5000;
     //                       client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
     //                   }
     //                   data.AddText("(It`s said the Mystery Pearl contains terrific power which can only be motivated by the purest Metal Force.")
     //                       .AddText("~Is it true? If you have the Earth Force, please transform it into Metal Force on the Wheel of Nature.)")
     //                       .AddOption("Go~make~the~Metal~Force.", 12)
     //                       .FinalizeDialog();

     //                   break;
     //               }
     //           case 6:
     //               {
     //                   client.Player.QuestGUI.Accept(ActiveQuest3, 0);
     //                   client.Player.QuestGUI.SendAutoPatcher(3998, 307, 102, 0);
     //                   break;
     //               }
     //           case 3:
     //               {
     //                   if (client.Inventory.Contain(3008742, 50))
     //                   {
     //                       data.AddText("(The altar has been roughly restored by using some unique Building Stones.)")
     //             .AddOption("I`m~so~tired.", 5)
     //      .FinalizeDialog();
     //                   }
     //                   else
     //                   {
     //                       data.AddText("(To restore the altar, you need to collect more Building Stones.)")
     //                .AddOption("Go~collect~more~stones.", 4)
     //         .FinalizeDialog();
     //                   }
     //                   break;
     //               }
     //           case 5:
     //               {
     //                   if (!client.Inventory.HaveSpace(1))
     //                   {
     //                       data.AddText("Please make 1 more space in you inventory.")
     //              .AddOption("Let me check.", 255)
     //       .FinalizeDialog();
     //                       break;
     //                   }
     //                   if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
     //                   {
     //                       client.Inventory.RemoveStackItem(3008742, 50, stream);
     //                       client.Inventory.Add(stream, 3200346);
     //                       client.Player.Money += 5000;
     //                       client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
     //                   }
     //                   data.AddText("(To further restore the altar, you`ll need to switch on the five elemental forces resting in the Wheel of Nature.")
     //                       .AddText("~But where is the Wheel of Nature? It must be stolen by the Clawed Rock Devils nearby.")
     //                       .AddText("~Go retrieve the wheel from the Clawed Rock Devils.)")
     //                       .AddOption("Go~retrieve~the~wheel.", 6)
     //                       .FinalizeDialog();

     //                   break;
     //               }
     //           case 4:
     //               {
     //                   client.Player.QuestGUI.SendAutoPatcher(3998, 596, 447, 0);
     //                   break;
     //               }
     //           case 1:
     //               {
     //                   if (!client.Inventory.HaveSpace(1))
     //                   {
     //                       data.AddText("Please make 1 more space in you inventory.")
     //              .AddOption("Let me check.", 255)
     //       .FinalizeDialog();
     //                       break;
     //                   }
     //                   if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
     //                   {
     //                       var action = new ActionQuery()
     //                       {
     //                           ObjId = client.Player.UID,
     //                           Type = ActionType.DrawStory,
     //                           dwParam = 1003

     //                       };
     //                       client.Send(stream.ActionCreate(action));

     //                       client.Inventory.Add(stream, 3200344);
     //                       client.Player.Money += 1000;
     //                       client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
     //                   }
     //                   data.AddText("(You found two sides of the altar were seriously damaged, and Metal and Fire icons were absent from the elemental patterns.")
     //                          .AddText("~You should restore the Flame Altar, first. Look around and collect appropriate Building Stones.")
     //        .AddOption("Go~find~Building~Stones.", 2)
     // .FinalizeDialog();
     //                   break;
     //               }
     //           case 2:
     //               {
     //                   client.Player.QuestGUI.Accept(ActiveQuest2, 0);
     //                   client.Player.QuestGUI.SendAutoPatcher(3998, 596, 447, 0);

     //                   break;
     //               }
     //       }

     //   }
     //   [NpcAttribute(NpcID.LavaFlower1)]
     //   public static void LavaFlower1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
     //   {
     //       if (client.Player.QuestGUI.CheckQuest(3647, MsgQuestList.QuestListItem.QuestStatus.Accepted))
     //       {
     //           if (client.Inventory.Contain(3008747, 10))
     //           {
     //               client.SendSysMesage("You've already picked enough Lava Flowers. No need to collect more.", MsgMessage.ChatMode.System);
     //               return;
     //           }
     //           client.Player.AddPick(stream, "Collecting...", 2);
     //       }
     //   }
     //   [NpcAttribute(NpcID.LavaFlower2)]
     //   public static void LavaFlower2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
     //   {
     //       if (client.Player.QuestGUI.CheckQuest(3647, MsgQuestList.QuestListItem.QuestStatus.Accepted))
     //       {
     //           if (client.Inventory.Contain(3008747, 10))
     //           {
     //               client.SendSysMesage("You've already picked enough Lava Flowers. No need to collect more.", MsgMessage.ChatMode.System);
     //               return;
     //           }
     //           client.Player.AddPick(stream, "Collecting...", 2);
     //       }
     //   }
     //   [NpcAttribute(NpcID.LavaFlower3)]
     //   public static void LavaFlower3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
     //   {
     //       if (client.Player.QuestGUI.CheckQuest(3647, MsgQuestList.QuestListItem.QuestStatus.Accepted))
     //       {
     //           if (client.Inventory.Contain(3008747, 10))
     //           {
     //               client.SendSysMesage("You've already picked enough Lava Flowers. No need to collect more.", MsgMessage.ChatMode.System);
     //               return;
     //           }
     //           client.Player.AddPick(stream, "Collecting...", 2);
     //       }
     //   }
     //   [NpcAttribute(NpcID.LavaFlower4)]
     //   public static void LavaFlower4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
     //   {
     //       if (client.Player.QuestGUI.CheckQuest(3647, MsgQuestList.QuestListItem.QuestStatus.Accepted))
     //       {
     //           if (client.Inventory.Contain(3008747, 10))
     //           {
     //               client.SendSysMesage("You've already picked enough Lava Flowers. No need to collect more.", MsgMessage.ChatMode.System);
     //               return;
     //           }
     //           client.Player.AddPick(stream, "Collecting...", 2);
     //       }
     //   }
     //   [NpcAttribute(NpcID.LavaFlower5)]
     //   public static void LavaFlower5(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
     //   {
     //       if (client.Player.QuestGUI.CheckQuest(3647, MsgQuestList.QuestListItem.QuestStatus.Accepted))
     //       {
     //           if (client.Inventory.Contain(3008747, 10))
     //           {
     //               client.SendSysMesage("You've already picked enough Lava Flowers. No need to collect more.", MsgMessage.ChatMode.System);
     //               return;
     //           }
     //           client.Player.AddPick(stream, "Collecting...", 2);
     //       }
     //   }
   //     [NpcAttribute(NpcID.LavaFlower6)]
   //     public static void LavaFlower6(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
   //     {
   //         if (client.Player.QuestGUI.CheckQuest(3647, MsgQuestList.QuestListItem.QuestStatus.Accepted))
   //         {
   //             if (client.Inventory.Contain(3008747, 10))
   //             {
   //                 client.SendSysMesage("You've already picked enough Lava Flowers. No need to collect more.", MsgMessage.ChatMode.System);
   //                 return;
   //             }
   //             client.Player.AddPick(stream, "Collecting...", 2);
   //         }
   //     }
   //     [NpcAttribute(NpcID.LavaFlower7)]
   //     public static void LavaFlower7(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
   //     {
   //         if (client.Player.QuestGUI.CheckQuest(3647, MsgQuestList.QuestListItem.QuestStatus.Accepted))
   //         {
   //             if (client.Inventory.Contain(3008747, 10))
   //             {
   //                 client.SendSysMesage("You've already picked enough Lava Flowers. No need to collect more.", MsgMessage.ChatMode.System);
   //                 return;
   //             }
   //             client.Player.AddPick(stream, "Collecting...", 2);
   //         }
   //     }
   //     [NpcAttribute(NpcID.ChingYan)]
   //     public static void ChingYan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
   //     {
   //         var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ChongYan, 0, 3640);
   //         var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ChingYan, 0, 3641);
   //         var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ChingYan, 0, 3642);
   //         var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ChingYan, 0, 3643);
   //         Dialog data = new Dialog(client, stream);
   //         switch (Option)
   //         {
   //             case 0:
   //                 {
   //                     if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
   //                     {
   //                         data.AddText("Cough... Cough... Who is there?! I`m not afraid of you! I`ll avenge for the sacrificed people!")
   //           .AddOption("The~Elder~sent~me~here.", 1).AddAvatar(90)
   //    .FinalizeDialog();
   //                         break;
   //                     }
   //                     if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
   //                     {
   //                         if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
   //                         {
   //                             data.AddText("Have you eli... eliminate the monsters?")
   //    .AddOption("Yes!", 4);
   //                             if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 15, 1) == false)
   //                                 data.AddOption("Give me one more drink.", 3);
   //                             data.AddAvatar(90)
   //                  .FinalizeDialog();
   //                         }
   //                         else
   //                         {
   //                             data.AddText("Um... I`ve never seen you before. The Elder... The Elder sent you here? You need to prove it.")
   //                       .AddText("~Kill the Lava Scorpions, and... and offer this bottle of Strong Drink to the sacrificed Bright heroes.")
   //                       .AddText("~Go, go kill the monsters, and I`ll believe you.")
   //       .AddOption("Okay.", 2).AddAvatar(90)
   //.FinalizeDialog();
   //                         }

   //                     }
   //                     else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
   //                     {
   //                         if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
   //                         {
   //                             data.AddText("Why are you still here? I`m incurable. Don`t waste your time.")
   //                                 .AddOption("Accept~the~treatment.", 7).AddAvatar(90)
   //          .FinalizeDialog();

   //                         }
   //                         else
   //                         {
   //                             data.AddText("Thanks for fulfilling my last wish! Now... Now I can die without anything to regret.")
   //                       .AddText("~In my next life, I may have a chance to thank you.")
   //                 .AddOption("Stop~it.~I`ll~save~you.", 6).AddAvatar(90)
   //          .FinalizeDialog();
   //                         }

   //                     }
   //                     else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
   //                     {
   //                         if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
   //                         {
   //                             data.AddText("Really? Oh, sorry... I was too nervous and angry. Monsters... Monsters killed my friends. It`s blood for blood!")
   //                   .AddOption("They`ll~pay~the~price.", 11).AddAvatar(90)
   //            .FinalizeDialog();
   //                         }
   //                         else
   //                         {
   //                             data.AddText("You`re looking for the Flame Temple? If not I can`t even walk, I`ll go with you.")
   //                           .AddText("~Don`t worry. I know how to contact my tribe. Just go and do your job.")
   //                    .AddOption("Take~care!", 11).AddAvatar(90)
   //             .FinalizeDialog();
   //                         }

   //                     }
   //                     else
   //                     {
   //                         data.AddText("Really? Oh, sorry... I was too nervous and angry. Monsters... Monsters killed my friends. It`s blood for blood!")
   //                   .AddOption("They`ll~pay~the~price.", 255).AddAvatar(90)
   //            .FinalizeDialog();
   //                     }


   //                     break;
   //                 }
   //             case 10:
   //                 {
   //                     if (!client.Inventory.HaveSpace(1))
   //                     {
   //                         data.AddText("Please make 1 more space in you inventory.")
   //                .AddOption("Let me check.", 255).AddAvatar(90)
   //         .FinalizeDialog();
   //                         break;
   //                     }
   //                     if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
   //                     {
   //                         client.Inventory.Remove(3008741, 10, stream);
   //                         client.Inventory.Add(stream, 3200345);
   //                         client.Player.Money += 3000;
   //                         client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
   //                     }

   //                     data.AddText("You`re looking for the Flame Temple? If not I can`t even walk, I`ll go with you.")
   //                         .AddText("~Don`t worry. I know how to contact my tribe. Just go and do your job.")
   //                  .AddOption("Take~care!", 11).AddAvatar(90)
   //           .FinalizeDialog();
   //                     break;
   //                 }
   //             case 11:
   //                 {
   //                     client.Player.QuestGUI.Accept(ActiveQuest4, 0);
   //                     client.Player.QuestGUI.SendAutoPatcher(ActiveQuest4.FinishNpcId.Map, ActiveQuest4.FinishNpcId.X, ActiveQuest4.FinishNpcId.Y, ActiveQuest4.FinishNpcId.ID);
   //                     break;
   //                 }
   //             case 7:
   //                 {
   //                     if (client.Inventory.Contain(3008741, 10))
   //                     {


   //                         data.AddText("How did you collect so many White Herbs? You`re too kind. I felt much better now.")
   //                             .AddOption("Take~a~good~rest.", 10).AddAvatar(90)
   //              .FinalizeDialog();
   //                     }
   //                     else
   //                     {
   //                         data.AddText("(Ching Yan was seriously injred. To save him, you need to collect enough White Herbs. ")
   //                           .AddText("There is nothing more important then saving a life. Hurry and pick the White Herbs!)")
   //                           .AddOption("I need more White Herbs.", 6).AddAvatar(90)
   //            .FinalizeDialog();
   //                     }
   //                     break;
   //                 }
   //             case 5:
   //                 {
   //                     if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 15, 1))
   //                     {
   //                         if (!client.Inventory.HaveSpace(1))
   //                         {
   //                             data.AddText("Please make 1 more space in you inventory.")
   //                    .AddOption("Let me check.", 255).AddAvatar(90)
   //             .FinalizeDialog();
   //                             break;
   //                         }
   //                         if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
   //                         {
   //                             client.Inventory.Remove(3008740, 1, stream);
   //                             client.Inventory.Add(stream, 3200346);
   //                             client.Player.Money += 5000;
   //                             client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
   //                         }
   //                     }
   //                     data.AddText("Thanks for fulfilling my last wish! Now... Now I can die without anything to regret.")
   //                         .AddText("~In my next life, I may have a chance to thank you.")
   //                   .AddOption("Stop~it.~I`ll~save~you.", 6).AddAvatar(90)
   //            .FinalizeDialog();
   //                     break;
   //                 }
   //             case 6:
   //                 {
   //                     client.Player.QuestGUI.Accept(ActiveQuest3, 0);
   //                     client.Player.QuestGUI.SendAutoPatcher(3998, 273, 273, 0);
   //                     break;
   //                 }
   //             case 4:
   //                 {
   //                     if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 15, 1))
   //                     {
   //                         data.AddText("Alas... My friends... You can rest in peace now.")
   //                             .AddOption("I`m~sorry.", 5)
   //                             .AddAvatar(90).FinalizeDialog();
   //                     }
   //                     else
   //                     {
   //                         data.AddText("Don`t try to cheat... cheat me! See, so many monsters still... still get away whit their crimesc! Hurry and kill them all!")
   //                                .AddOption("Got it!", 2)
   //                                .AddAvatar(90).FinalizeDialog();
   //                     }

   //                     break;
   //                 }
   //             case 3:
   //                 {
   //                     if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
   //                     {
   //                         if (client.Inventory.Contain(3200344, 1))
   //                         {
   //                             data.AddText("The drink is... is still in your inventory. Why did you ask more?")
   //                                 .AddOption("Sorry, I forgot it.", 255)
   //                                 .AddAvatar(90).FinalizeDialog();
   //                         }
   //                         else
   //                         {
   //                             if (!client.Inventory.HaveSpace(1))
   //                             {
   //                                 data.AddText("Please make 1 more space in you inventory.")
   //                        .AddOption("Let me check.", 255).AddAvatar(90)
   //                 .FinalizeDialog();
   //                                 break;
   //                             }
   //                             client.Inventory.Add(stream, 3200344);
   //                             data.AddText("Cough... You lost the drink? Alright, alright... I'll give you another one, but keep it well.")
   //                     .AddOption("Okay.", 255).AddAvatar(90)
   //              .FinalizeDialog();
   //                         }

   //                     }
   //                     break;
   //                 }
   //             case 1:
   //                 {
   //                     if (!client.Inventory.HaveSpace(1))
   //                     {
   //                         data.AddText("Please make 1 more space in you inventory.")
   //                .AddOption("Let me check.", 255).AddAvatar(90)
   //         .FinalizeDialog();
   //                         break;
   //                     }
   //                     if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
   //                     {
   //                         client.Inventory.Add(stream, 3200344);
   //                         client.Player.Money += 1000;
   //                         client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
   //                     }
   //                     data.AddText("Um... I`ve never seen you before. The Elder... The Elder sent you here? You need to prove it.")
   //                         .AddText("~Kill the Lava Scorpions, and... and offer this bottle of Strong Drink to the sacrificed Bright heroes.")
   //                         .AddText("~Go, go kill the monsters, and I`ll believe you.")
   //         .AddOption("Okay.", 2).AddAvatar(90)
   //  .FinalizeDialog();
   //                     break;
   //                 }
   //             case 2:
   //                 {
   //                     client.Player.QuestGUI.Accept(ActiveQuest2, 0);
   //                     if (!client.Inventory.Contain(3008740, 1))
   //                         client.Inventory.Add(stream, 3008740);
   //                     client.Player.QuestGUI.SendAutoPatcher(3998, 230, 273, 0);
   //                     break;
   //                 }
   //         }
   //     }

   //     [NpcAttribute(NpcID.WhiteHerb1)]
   //     public static void WhiteHerb1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
   //     {
   //         if (client.Player.QuestGUI.CheckQuest(3642, MsgQuestList.QuestListItem.QuestStatus.Accepted))
   //         {
   //             if (client.Inventory.Contain(3008741, 10))
   //             {
   //                 client.SendSysMesage("You've already picked enough number of White Herbs.", MsgMessage.ChatMode.System);
   //                 return;
   //             }
   //             client.Player.AddPick(stream, "Collecting...", 2);
   //         }
   //     }
   //     [NpcAttribute(NpcID.WhiteHerb2)]
   //     public static void WhiteHerb2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
   //     {
   //         if (client.Player.QuestGUI.CheckQuest(3642, MsgQuestList.QuestListItem.QuestStatus.Accepted))
   //         {
   //             if (client.Inventory.Contain(3008741, 10))
   //             {
   //                 client.SendSysMesage("You've already picked enough number of White Herbs.", MsgMessage.ChatMode.System);
   //                 return;
   //             }
   //             client.Player.AddPick(stream, "Collecting...", 2);
   //         }
   //     }
   //     [NpcAttribute(NpcID.WhiteHerb3)]
   //     public static void WhiteHerb3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
   //     {
   //         if (client.Player.QuestGUI.CheckQuest(3642, MsgQuestList.QuestListItem.QuestStatus.Accepted))
   //         {
   //             if (client.Inventory.Contain(3008741, 10))
   //             {
   //                 client.SendSysMesage("You've already picked enough number of White Herbs.", MsgMessage.ChatMode.System);
   //                 return;
   //             }
   //             client.Player.AddPick(stream, "Collecting...", 2);
   //         }
   //     }
   //     [NpcAttribute(NpcID.WhiteHerb4)]
   //     public static void WhiteHerb4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
   //     {
   //         if (client.Player.QuestGUI.CheckQuest(3642, MsgQuestList.QuestListItem.QuestStatus.Accepted))
   //         {
   //             if (client.Inventory.Contain(3008741, 10))
   //             {
   //                 client.SendSysMesage("You've already picked enough number of White Herbs.", MsgMessage.ChatMode.System);
   //                 return;
   //             }
   //             client.Player.AddPick(stream, "Collecting...", 2);
   //         }
   //     }
   //     [NpcAttribute(NpcID.WhiteHerb5)]
   //     public static void WhiteHerb5(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
   //     {
   //         if (client.Player.QuestGUI.CheckQuest(3642, MsgQuestList.QuestListItem.QuestStatus.Accepted))
   //         {
   //             if (client.Inventory.Contain(3008741, 10))
   //             {
   //                 client.SendSysMesage("You've already picked enough number of White Herbs.", MsgMessage.ChatMode.System);
   //                 return;
   //             }
   //             client.Player.AddPick(stream, "Collecting...", 2);
   //         }
   //     }
   //     [NpcAttribute(NpcID.WhiteHerb6)]
   //     public static void WhiteHerb6(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
   //     {
   //         if (client.Player.QuestGUI.CheckQuest(3642, MsgQuestList.QuestListItem.QuestStatus.Accepted))
   //         {
   //             if (client.Inventory.Contain(3008741, 10))
   //             {
   //                 client.SendSysMesage("You've already picked enough number of White Herbs.", MsgMessage.ChatMode.System);
   //                 return;
   //             }
   //             client.Player.AddPick(stream, "Collecting...", 2);
   //         }
   //     }



   //     [NpcAttribute(NpcID.WoundedBrightTribesman)]
   //     public static void WoundedBrightTribesman(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
   //     {

   //         Dialog data = new Dialog(client, stream);
   //         switch (Option)
   //         {
   //             case 0:
   //                 {

   //                     data.AddText("Ow, stupid, stupid! I promised to the Chief to keep the Elder safe, but I can`t even protect myself now.")
   //                         .AddText("~I won`t forgive myself if the Elder gets into any trouble.")
   //       .AddOption("Let~me~help~you~out.", 255).AddAvatar(32)
   //.FinalizeDialog();
   //                     break;
   //                 }
   //         }
   //     }


   //     [NpcAttribute(NpcID.BrightGuard)]
   //     public static void BrightGuard(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
   //     {

   //         Dialog data = new Dialog(client, stream);
   //         switch (Option)
   //         {
   //             case 0:
   //                 {

   //                     data.AddText("I`ve tried my best to protect the Elder. Hope the Elder is okay.")
   //                         .AddText("~Disgusting monsters, I`ll kill you all someday.")
   //       .AddOption("You~will.", 255).AddAvatar(30)
   //.FinalizeDialog();
   //                     break;
   //                 }
   //         }
   //     }
//        [NpcAttribute(NpcID.BrokenForgeFurnace)]
//        public static void BrokenForgeFurnace(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
//        {
//            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.ChongYan, 0, 3635);
//            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BrokenForgeFurnace, 0, 3636);
//            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BrokenForgeFurnace, 0, 3637);
//            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BrokenForgeFurnace, 0, 3638);
//            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BrokenForgeFurnace, 0, 3639);
//            Dialog data = new Dialog(client, stream);
//            switch (Option)
//            {
//                case 0:
//                    {
//                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
//                        {
//                            data.AddText("In the deep corner, you discovered a Forge Furnace which was broken but still majestic,")
//                                .AddText("~A huge number of monsters emerged from the furnace. It must be the root of the monster crisis.")

//                        .AddOption("Check~the~furnace~carefully.", 1)
//                           .FinalizeDialog();
//                        }
//                        else if (!client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
//                        {
//                            data.AddText("(You need to collect the Rune Fragments to complete the runes on the Forge Furnace first.)")
//                 .AddOption("Use the Rune Fragment.", 2)
//          .FinalizeDialog();

//                        }
//                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
//                        {
//                            data.AddText("(You read the runes on the Forge Furnace carefully, and understood the most words.)")
//         .AddOption("Read~the~rune.", 6)
//  .FinalizeDialog();
//                        }
//                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
//                        {
//                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
//                            {
//                                data.AddText("(When you`re approaching the Forge Furnace, the Holy Hammer started shaking suddenly.")
//                         .AddText("~You watched around and found a possible cause of the shaking - a hammering block.")
//                         .AddText("~Why not hit the block with your Holy Hammer?)")
//     .AddOption("Hit~the~block.", 10)
//.FinalizeDialog();
//                            }
//                            else
//                            {
//                                data.AddText("(Since the Rune Fragments were collected from the alien monsters,")
//                          .AddText("~you can probably find the Holy Hammer from their boss.)")
//      .AddOption("Head~to~see~their~boss.", 9)
//.FinalizeDialog();
//                            }
//                        }
//                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
//                        {
//                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
//                            {
//                                data.AddText("(This Forge furnace is broken, but still majestic. The ancient rune engraved on its surface tells ")
//                                    .AddText("the history of the Bringht Tribes.)")
//                                    .AddOption("Pick up the Metal Block", 14).FinalizeDialog();

//                            }
//                            else
//                            {
//                                data.AddText("(You looked inside the furnace, and found an ugly Black Metal Block which made ")
//                           .AddText("~you felt nervous. Perhaps, Chong Yan Elder knows more about this item.)")
//       .AddOption("Give~the~block~to~the~Elder.", 13)
//  .FinalizeDialog();
//                            }
//                        }
//                        else
//                        {
//                            data.AddText("(This Forge Furnace is broken, but still majestic. The ancient rune engraved on its surface")
//                         .AddText("~tells the history of the Bright Tribe.)")
//     .AddOption("What~a~huge~furnace.", 255)
//.FinalizeDialog();
//                        }
//                        break;
//                    }
//                case 1:
//                    {
//                        if (!client.Inventory.HaveSpace(1))
//                        {
//                            data.AddText("Please make 1 more space in you inventory.")
//                   .AddOption("Let me check.", 255)
//            .FinalizeDialog();
//                            break;
//                        }

//                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
//                        {

//                            client.Inventory.Add(stream, 3200344);
//                            client.Player.Money += 1000;
//                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
//                        }
//                        data.AddText("(You tried to read the rune on the furnace, but found too many parts missing.)")
//                  .AddOption("Collect~rune~fragments~nearby.", 2)
//           .FinalizeDialog();
//                        break;
//                    }
//                case 2:
//                    {
//                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
//                        {
//                            if (client.Inventory.Contain(3008752, 8))
//                            {
//                                data.AddText("(You`ve finally collected all the missing parts, and complete the runes on the furnace.)")
//              .AddOption("What~does~it~say?", 4)
//       .FinalizeDialog();
//                            }
//                            else
//                            {

//                                data.AddText("(You should have 8 Rune Fragments to complete the runes.)")
//                .AddOption("Go collect more.", 3)
//         .FinalizeDialog();
//                            }
//                        }
//                        else
//                        {
//                            client.Player.QuestGUI.SendAutoPatcher(3998, 226, 406, 0);
//                            client.Player.QuestGUI.Accept(ActiveQuest2, 0);
//                        }
//                        break;
//                    }
//                case 4:
//                    {
//                        if (!client.Inventory.HaveSpace(1))
//                        {
//                            data.AddText("Please make 1 more space in you inventory.")
//                   .AddOption("Let me check.", 255)
//            .FinalizeDialog();
//                            break;
//                        }
//                        if (client.Inventory.Contain(3008752, 8))
//                        {
//                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
//                            {
//                                client.Inventory.Remove(3008752, 8, stream);
//                                client.Inventory.Add(stream, 3200346);
//                                client.Player.Money += 5000;
//                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

//                            }
//                        }

//                        data.AddText("(You read the runes on the Forge Furnace carefully, and understood the most words.)")
//           .AddOption("Read~the~rune.", 6)
//    .FinalizeDialog();
//                        break;
//                    }
//                case 6:
//                    {
//                        var action = new ActionQuery()
//                        {
//                            ObjId = client.Player.UID,
//                            Type = ActionType.DrawStory,
//                            dwParam = 1002

//                        };
//                        client.Send(stream.ActionCreate(action));

//                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
//                        data.AddText("(It was the biggest forge furnace in ancient Bright, which had produced countless famous weapons.")
//                            .AddText("~According to the runes, the Master Blacksmith was the most brilliant craftsman for metal working,")
//                            .AddText("~and ranked only next to the Chief of Bright Tribe.)")
//        .AddOption("I~see.", 7)
// .FinalizeDialog();
//                        break;
//                    }
//                case 7:
//                    {
//                        data.AddText("You noted that a hammer icon always appeared when the Master Blacksmith was mentioned in the runes.")
//                            .AddText("~Is this Holy Hammer a symbol of the Master Blacksmith? If you can find it, something amazing")
//                            .AddText("~may happen. Do you want to have a try?")
//        .AddOption("Go~and~look~for~the~hammer.", 8)
// .FinalizeDialog();
//                        break;
//                    }
//                case 8:
//                    {
//                        if (!client.Inventory.HaveSpace(1))
//                        {
//                            data.AddText("Please make 1 more space in you inventory.")
//                   .AddOption("Let me check.", 255)
//            .FinalizeDialog();
//                            break;
//                        }
//                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest3.MissionId))
//                        {
//                            client.Inventory.Add(stream, 3200344);
//                            client.Player.Money += 1000;
//                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
//                        }
//                        data.AddText("(Since the Rune Fragments were collected from the alien monsters,")
//                         .AddText("~you can probably find the Holy Hammer from their boss.)")
//     .AddOption("Head~to~see~their~boss.", 9)
//.FinalizeDialog();
//                        break;
//                    }
//                case 9:
//                    {
//                        client.Player.QuestGUI.Accept(ActiveQuest4, 0);
//                        client.Player.QuestGUI.SendAutoPatcher(3998, 392, 706, 0);
//                        break;
//                    }
//                case 10:
//                    {
//                        if (client.Inventory.Contain(3008750, 1))
//                        {
//                            client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "Attack35r");
//                            data.AddText("(When you hit the block with the Holy Hammer, an invisible energy wave bursted out, ")
//                       .AddText("~killing the alien monsters in silence. No more monsters creep out from the furnace.")
//                       .AddText("~It seems the hammer power has fully suppressed the evil force in the furnace.)")
//   .AddOption("Great!", 12)
//.FinalizeDialog();
//                        }
//                        else
//                        {
//                            data.AddText("(You don`t have the Holy Hammer to hit the block. Go challenge the Violet Bat King, and you'll ")
//                         .AddText("have a change to get hammer after defeating him.)")
//     .AddOption("I see.", 11)
//.FinalizeDialog();
//                        }
//                        break;
//                    }
//                case 12:
//                    {
//                        if (!client.Inventory.HaveSpace(2))
//                        {
//                            data.AddText("Please make 2 more spaces in you inventory.")
//                   .AddOption("Let me check.", 255)
//            .FinalizeDialog();
//                            break;
//                        }
//                        if (client.Inventory.Contain(3008750, 1))
//                        {
//                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
//                            {
//                                client.Inventory.Remove(3008750, 1, stream);
//                                client.Inventory.Add(stream, 3009001, 2);
//                                client.Inventory.Add(stream, 3200346);
//                                client.Player.Money += 5000;
//                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
//                                client.SendSysMesage("You received 2 Bright Star Stones!", MsgMessage.ChatMode.System);
//                            }
//                        }
//                        data.AddText("(You looked inside the furnace, and found an ugly Black Metal Block which made ")
//                        .AddText("~you felt nervous. Perhaps, Chong Yan Elder knows more about this item.)")
//    .AddOption("Give~the~block~to~the~Elder.", 13)
//.FinalizeDialog();
//                        break;
//                    }
//                case 13:
//                    {
//                        if (!client.Inventory.HaveSpace(1))
//                        {
//                            data.AddText("Please make 1 more space in you inventory.")
//                   .AddOption("Let me check.", 255)
//            .FinalizeDialog();
//                            break;
//                        }
//                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
//                        if (!client.Inventory.Contain(3008751, 1))
//                            client.Inventory.Add(stream, 3008751);
//                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest5.FinishNpcId.Map, ActiveQuest5.FinishNpcId.X, ActiveQuest5.FinishNpcId.Y, 0);
//                        break;
//                    }
//                case 14:
//                    {
//                        if (!client.Inventory.Contain(3008751, 1))
//                        {
//                            if (!client.Inventory.HaveSpace(1))
//                            {
//                                data.AddText("Please make 1 more space in you inventory.")
//                       .AddOption("Let me check.", 255)
//                .FinalizeDialog();
//                                break;
//                            }
//                            client.Inventory.Add(stream, 3008751);
//                        }
//                        data.AddText("(The Black Metal Block is already in your inventory. Hurry and talk to Chong Yan Elder about this block.)")
//               .AddOption("I`ll go now.", 15)
//        .FinalizeDialog();
//                        break;
//                    }
//                case 15:
//                    {
//                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest5.FinishNpcId.Map, ActiveQuest5.FinishNpcId.X, ActiveQuest5.FinishNpcId.Y, 0);
//                        break;
//                    }
//                case 11:
//                    {
//                        client.Player.QuestGUI.SendAutoPatcher(3998, 392, 706, 0);
//                        break;
//                    }
//                case 3:
//                    {
//                        client.Player.QuestGUI.SendAutoPatcher(3998, 226, 406, 0);

//                        break;
//                    }
//            }
//        }

        [NpcAttribute(NpcID.ChongYan)]
        public static void ChongYan(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.RemainofBrightTribesman, 0, 3633);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ChongYan, 0, 3634);
            var ActiveQuest3 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ChongYan, 0, 3635);
            var ActiveQuest4 = Database.QuestInfo.GetFinishQuest((uint)NpcID.BrokenForgeFurnace, 0, 3639);
            var ActiveQuest5 = Database.QuestInfo.GetFinishQuest((uint)NpcID.ChongYan, 0, 3640);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("Why did you come back again?")
              .AddOption("I go something for you.", 10).AddAvatar(67)
       .FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("HELP! HELP! Could you help me out?")
                             .AddOption("What`s~wrong?", 1).AddAvatar(67)
                                .FinalizeDialog();
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Have you cleared those monsters nearby?")
                .AddOption("Yes, It's safe now.", 4).AddAvatar(67)
         .FinalizeDialog();


                            }
                            else
                            {
                                data.AddText("Hero, you saved my life, and I am eternally grateful! We got attacked by a horde of")
                              .AddText("~strange monsters. It happened all of a sudden, and we suffered heavy casualties.")
                              .AddText("~We got no way to go, and retreated here.")
                .AddOption("What~kind~of~monsters?", 2).AddAvatar(67)
         .FinalizeDialog();
                            }
                            break;
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest3.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {

                                data.AddText("We're okay. Please go find the Flame Temple and remember my words. It's so urgent!")
                                             .AddOption("I`ll start off now.", 255).AddAvatar(67)
                                      .FinalizeDialog();
                                break;
                            }
                            data.AddText("The Chief asked you to find us? If so, you must have known the importance of the Flame Temple.")
                              .AddText("~To stop the Fiery Dragon Volcano from eruption, we have to completely activate the Mystery Pearl")
                              .AddText("~at the altar in front of the temple. If we failed, the flowing lava will swallow all living things,")
                              .AddText("~repeating the disaster of our ancestors. How stupid I am to get stuck here at this critical moment.")
                              .AddText("~I feel so sorry to my people.")
                .AddOption("I~can~help~you.", 6).AddAvatar(67)
         .FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false &&
                        client.Player.QuestGUI.CheckQuest(ActiveQuest4.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished))
                        {
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest5.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                            {
                                data.AddText("Time is limited. Please hit the road now.")
                      .AddOption("Okay.", 12).AddAvatar(67)
                      .FinalizeDialog();
                            }
                            else
                            {
                                data.AddText("Now the monsters have been swept off, we`ll continue to find the Flame Temple.")
                            .AddText("~You can go first, and I`ll inform our Chief. If you see our lost tribesmen, please give them a hand.")
                            .AddOption("Got~it.", 12).AddAvatar(67)
                            .FinalizeDialog();
                            }
                        }
                        else
                        {
                            data.AddText("i got stuck here, you my people are still waiting for my news about the Flame Temple. ")
                      .AddText("I don't want to disappoint them.")
  .AddOption("You'll be okay.", 255).AddAvatar(67)
.FinalizeDialog();
                        }

                        break;
                    }
                case 10:
                    {
                        data.AddText("You meant you got this metal block from the Forge Furnace? It`s hard to tell what it is,")
                            .AddText("~but the material looks the same as that of the top Bright craftsman`s ID token.")
                            .AddText("~Well, I`ll go ask the scholars in our tribe.")
                            .AddOption("Is~there~anything~I~can~help?", 11).AddAvatar(67)
                            .FinalizeDialog();
                        break;
                    }
                case 12:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest5, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest5.FinishNpcId.Map, ActiveQuest5.FinishNpcId.X, ActiveQuest5.FinishNpcId.Y, ActiveQuest5.FinishNpcId.ID);

                        break;
                    }
                case 11:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                   .AddOption("Let me check.", 255).AddAvatar(67)
            .FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest4.MissionId))
                        {
                            client.Inventory.Remove(3008751, 1, stream);
                            client.Inventory.Add(stream, 3200344);
                            client.Player.Money += 5000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }

                        data.AddText("Now the monsters have been swept off, we`ll continue to find the Flame Temple.")
                         .AddText("~You can go first, and I`ll inform our Chief. If you see our lost tribesmen, please give them a hand.")
                         .AddOption("Got~it.", 12).AddAvatar(67)
                         .FinalizeDialog();
                        break;
                    }
                case 6:
                    {
                        data.AddText("It`s great! See that Fiery Dragon Volcano over there? The Flame Temple was built at the foot of the volcano.")
                            .AddText("~First, you need to break through the monster siege. When you find the Flame Temple and restore its altar,")
                            .AddText("~put this Mystery Pearl at the altar. Then, I`ll do the rest to completely dispel the crisis of Fiery Dragon.")
                                            .AddOption("I~see.~Take~care~of~yourself!", 7).AddAvatar(67)
                                     .FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest3, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest3.FinishNpcId.Map, ActiveQuest3.FinishNpcId.X, ActiveQuest3.FinishNpcId.Y, ActiveQuest3.FinishNpcId.ID);
                        break;
                    }
                case 5:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 10))
                        {
                            if (!client.Inventory.HaveSpace(1))
                            {
                                data.AddText("Please make 1 more space in you inventory.")
                       .AddOption("Let me check.", 255).AddAvatar(67)
                .FinalizeDialog();
                                break;
                            }
                            if (client.Player.QuestGUI.FinishQuest(ActiveQuest2.MissionId))
                            {
                                client.Inventory.Add(stream, 3200346);
                                client.Player.Money += 5000;
                                client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);

                            }
                            data.AddText("The Chief asked you to find us? If so, you must have known the importance of the Flame Temple.")
                                                            .AddText("~To stop the Fiery Dragon Volcano from eruption, we have to completely activate the Mystery Pearl")
                                                            .AddText("~at the altar in front of the temple. If we failed, the flowing lava will swallow all living things,")
                                                            .AddText("~repeating the disaster of our ancestors. How stupid I am to get stuck here at this critical moment.")
                                                            .AddText("~I feel so sorry to my people.")
                                              .AddOption("I~can~help~you.", 6).AddAvatar(67)
                                       .FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Player.QuestGUI.CheckObjectives(ActiveQuest2.MissionId, 10))
                        {

                            data.AddText("Thanks for your help! We`re finally safe now.")
                                             .AddOption("You`re~welcome.", 5).AddAvatar(67)
                                      .FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("There are still many monsters crawling around. Could you kill more?")
                                              .AddOption("Okay.", 3).AddAvatar(67)
                                       .FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("I don`t know, but I saw flame runes imprinted on their body. Every time I tried to approach them,")
                               .AddText("~I got burnt. You know, it`s impossible for me to kill so many monsters. Could you help me?")
                               .AddText("~Please kill the monsters surrounding us, and my people can be saved.")
                 .AddOption("Count~on~me!", 3).AddAvatar(67)
          .FinalizeDialog();
                        break;
                    }
                case 3:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(client.Player.Map, 145, 338, 0);
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                   .AddOption("Let me check.", 255).AddAvatar(67)
            .FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.Inventory.Add(stream, 3200344);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        data.AddText("Hero, you saved my life, and I am eternally grateful! We got attacked by a horde of")
                                .AddText("~strange monsters. It happened all of a sudden, and we suffered heavy casualties.")
                                .AddText("~We got no way to go, and retreated here.")
                  .AddOption("What~kind~of~monsters?", 2).AddAvatar(67)
           .FinalizeDialog();
                        break;
                    }
            }
        }
        [NpcAttribute(NpcID.RemainofBrightTribesman)]
        public static void RemainofBrightTribesman(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            //3633
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.PakYan, 0, 3632);
            var ActiveQuest2 = Database.QuestInfo.GetFinishQuest((uint)NpcID.RemainofBrightTribesman, 0, 3633);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                        {
                            data.AddText("You found a burnt body on the way to find the Elder.")
                             .AddOption("Have~a~check.", 1).AddAvatar(67)
                                .FinalizeDialog();
                        }
                        else if (client.Player.QuestGUI.CheckQuest(ActiveQuest2.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false)
                        {

                            data.AddText("We couldn`t afford any delay. Chong Yan Elder and other must have encountered terrific enemy.")
                         .AddText(" Hurry and follow their footsteps. Hope it`s not too late.")
                .AddOption("Let`s go!", 2).AddAvatar(67)
         .FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("(The Bright Tribesman`s body was left lying on the read.)")
                             .AddOption("Poor guy.", 255).AddAvatar(67)
                                .FinalizeDialog();

                        }
                        break;
                    }
                case 1:
                    {
                        if (!client.Inventory.HaveSpace(1))
                        {
                            data.AddText("Please make 1 more space in you inventory.")
                   .AddOption("Let me check.", 255).AddAvatar(67)
            .FinalizeDialog();
                            break;
                        }
                        if (client.Player.QuestGUI.FinishQuest(ActiveQuest.MissionId))
                        {
                            client.GainExpBall(300, true, Role.Flags.ExperienceEffect.angelwing);
                            client.Player.Money += 1000;
                            client.Player.SendUpdate(stream, client.Player.Money, MsgUpdate.DataType.Money);
                        }
                        data.AddText("(It seems the body has been burnt, and his remaining accessories belonged to the Bright Tribe.")
                            .AddText("~You found many bloodstains and footsteps along the road. The Bright tribesmen must have a")
                            .AddText("~tough fight here, and retreated to that direction.)")
                   .AddOption("Follow~the~footsteps.", 2).AddAvatar(67)
            .FinalizeDialog();
                        break;
                    }
                case 2:
                    {
                        client.Player.QuestGUI.Accept(ActiveQuest2, 0);
                        client.Player.QuestGUI.SendAutoPatcher(ActiveQuest2.FinishNpcId.Map, ActiveQuest2.FinishNpcId.X, ActiveQuest2.FinishNpcId.Y, ActiveQuest2.FinishNpcId.ID);
                        break;
                    }
            }

        }


        [NpcAttribute(NpcID.GuidofFieryDragon)]
        public static void GuidofFieryDragon(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {

                        data.AddText("Have you been to the Fiery Dragon region? The land was home to the Bright Tribe.")
                            .AddText("~I wonder what it looks like now.")
                     .AddOption("Send~me~to~Fiery~Dragon.", 1)
                     .AddOption("You`ll~return~home~soon.", 255).AddAvatar(7)
              .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Player.QuestGUI.SendAutoPatcher(4020, 73, 98, 0);
                        break;
                    }
            }

        }
        [NpcAttribute(NpcID.TowerOfMysteryLayerChange)]
        public static void TowerOfMysteryLayerChange(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.HeavenlyMaster, 0, 6126);
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("You can enter the Tower of Mystery " + Math.Min(9, (int)client.Player.JoinTowerOfMysteryLayer + 1).ToString() + "F through this passage. If you`ve made up your mind")
                                  .AddText("~to challenge this floor, I can help. While if you want to directly complete the challenge of this floor and get the reward,")
                                  .AddText("~you need to enable `Sweep` function by donating CPs or Meteor Scroll. So, what do you say?");
                        data.AddOption("Challenge~Floor~" + Math.Min(9, (int)client.Player.JoinTowerOfMysteryLayer + 1) + ".", 1);
                        if (client.Player.TOMChallengeToday == 1)
                            data.AddOption("Enable~`Sweep`~function.", 2);
                        else
                            data.AddOption("Enable~`Sweep`~function.", 3);
                        data.AddOption("Leave~the~tower.", 6)
                        .AddOption("I~need~to~think~about~it.", 255)
                        .AddAvatar(23)
                      .FinalizeDialog();

                        break;
                    }
                case 6:
                    {
                        data.AddText("Are you sure you want to leave the Tower of Mystery, now?")
                          .AddOption("Yes.", 7)
                          .AddOption("I~haven`t~decided,~yet!", 255).AddAvatar(23)
                   .FinalizeDialog();
                        break;
                    }
                case 7:
                    {
                        client.Teleport(85, 75, 4020);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 1:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Alright.", 255).AddAvatar(23)
                     .FinalizeDialog();
                            break;
                        }
                        if (client.Player.TOMChallengeToday == 1)
                        {

                            if ((client.Player.TowerOfMysteryChallengeFlag & 1u << (byte)Math.Min(8, (int)client.Player.JoinTowerOfMysteryLayer)) != 1u << (byte)Math.Min(8, (int)client.Player.JoinTowerOfMysteryLayer))
                            {
                                Game.MsgTournaments.MsgSchedules.TowerOfMystery.JoinLayer(client, 60, 60, (byte)Math.Min(8, (int)client.Player.JoinTowerOfMysteryLayer), stream);
                            }
                            else
                            {
                                switch (Math.Min(8, (int)client.Player.JoinTowerOfMysteryLayer))
                                {
                                    case 0:
                                        {
                                            data.AddText("You've already challenged, at Elite mode, and defeated the Floor 1`s guardian devil GreatPheasant(Elite).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 1:
                                        {
                                            data.AddText("You've already challenged, at Elite mode, and defeated the Floor 2`s guardian devil WingedLord(Elite).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 2:
                                        {
                                            data.AddText("You've already challenged, at Elite mode, and defeated the Floor 3`s guardian devil WaterTerror(Elite).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 3:
                                        {
                                            data.AddText("You've already challenged, at Elite mode, and defeated the Floor 4`s guardian devil RuthlessAsura(Elite).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 4:
                                        {
                                            data.AddText("You've already challenged, at Elite mode, and defeated the Floor 5`s guardian devil SoulStrangler(Elite).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 5:
                                        {
                                            data.AddText("You've already challenged, at Elite mode, and defeated the Floor 6`s guardian devil DarkGlutton(Elite).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 6:
                                        {
                                            data.AddText("You've already challenged, at Elite mode, and defeated the Floor 7`s guardian devil AlienDragon(Elite).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 7:
                                        {
                                            data.AddText("You've already challenged, at Elite mode, and defeated the Floor 8`s guardian devil ShadowSpider(Elite).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    default:
                                        {
                                            data.AddText("You've already challenged, at Elite mode, and defeated the Floor 9`s guardian devil FlameGiant(Elite).No need to sweep again.")
                                 .AddOption("Okay.", 255).AddAvatar(23)
                          .FinalizeDialog();
                                            break;
                                        }
                                }
                            }
                        }
                        else
                        {
                            if ((client.Player.TowerOfMysteryChallengeFlag & 1u << (byte)Math.Min(8, (int)client.Player.JoinTowerOfMysteryLayer)) != 1u << (byte)Math.Min(8, (int)client.Player.JoinTowerOfMysteryLayer))
                            {
                                Game.MsgTournaments.MsgSchedules.TowerOfMystery.JoinLayer(client, 60, 60, client.Player.JoinTowerOfMysteryLayer, stream);
                            }
                            else
                            {
                                switch (Math.Min(8, (uint)client.Player.JoinTowerOfMysteryLayer))
                                {
                                    case 0:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 1`s guardian devil GreatPheasant(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 1:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 2`s guardian devil WingedLord(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 2:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 3`s guardian devil WaterTerror(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 3:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 4`s guardian devil RuthlessAsura(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 4:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 5`s guardian devil SoulStrangler(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 5:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 6`s guardian devil DarkGlutton(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 6:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 7`s guardian devil AlienDragon(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 7:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 8`s guardian devil ShadowSpider(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(23)
                           .FinalizeDialog();
                                            break;
                                        }
                                    default:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 9`s guardian devil FlameGiant(common).No need to sweep again.")
                                 .AddOption("Okay.", 255).AddAvatar(23)
                          .FinalizeDialog();
                                            break;
                                        }
                                }
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Player.CanSweapTowerOfMystery)
                        {

                            data.AddText("                       Bright Tribe`s Reward\n")
                           .AddText("                    **************************************\n");
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>\n");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>Refined</tip>\n");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>2-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>Unique</tip>\n");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>3-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>Elite</tip>\n");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>4-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>Super</tip>\n");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>5-Stars</tip>\n");
                                        break;
                                    }
                            }
                            data.AddText("                    **************************************\n");
                            data.AddText("                       Refreshing Times: " + client.Player.TOMRefreshReward.ToString() + "");

                            if (client.Player.TOM_Reward != MsgTournaments.MsgTowerOfMystery.RewardTypes.Super)
                            {
                                data.AddOption("Refresh~reward.~(20~CP)", 70);
                                data.AddOption("Enable~`Sweep`.~(430~CPs)", 76);//todo
                                data.AddOption("Refresh~reward.~(50~Study~Pts)", 72);
                                data.AddOption("`Sweep`.~(2~DragonBalls)", 77);//todo
                                if (client.Inventory.Contain(3008735, 1))
                                    data.AddOption("Refresh~reward.~(FierceSpirit)", 74);
                            }
                            else
                            {
                                data.AddOption("Enable~`Sweep`.~(430~CPs)", 76);
                                data.AddOption("`Sweep`.~(2~DragonBalls)", 77);
                            }

                            data.AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(23)
                            .FinalizeDialog();
                        }
                        break;
                    }
                case 77:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Alright.", 255).AddAvatar(23)
                     .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.HaveSpace(5))
                        {
                            data.AddText("Thanks for your challenge. However, you inventory is too full to contain the reward.Please clear at least 5 empty spaces, first.")
                               .AddOption("Okay.", 255).AddAvatar(23)
                                .FinalizeDialog();

                            break;
                        }
                        if (client.Inventory.Contain(Database.ItemType.DragonBall, 2) == false)
                        {
                            data.AddText("It seems you don`t have enough DragonBalls to directly clear this floor.Please check it again.")
                           .AddOption("Alright.", 255).AddAvatar(23)
                    .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.TOM_SellectOptionC) != 1u << client.Player.TOM_SellectOptionC)
                        {
                            client.Player.TowerOfMysterychallenge -= 1;
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            client.Player.TowerOfMysteryChallengeFlag |= 1u << client.Player.TOM_SellectOptionC;
                            client.Inventory.Remove(Database.ItemType.DragonBall, 2, stream);
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        client.Inventory.Add(stream, 3008956);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        client.Inventory.Add(stream, 3008957);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                        client.Inventory.Add(stream, 3008958);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        client.Inventory.Add(stream, 3008959);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                        client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        client.Inventory.Add(stream, 3008960);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                            }
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            data.AddText("You've successfully cleared this floor, and you deserve this reward.Here you are.")
                               .AddOption("Thanks!", 255).AddAvatar(23)
                                .FinalizeDialog();
                        }
                        else
                        {
                            switch (client.Player.TOM_SellectOptionC)
                            {
                                case 0:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 1`s guardian devil GreatPheasant(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 1:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 2`s guardian devil WingedLord(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 2:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 3`s guardian devil WaterTerror(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 3:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 4`s guardian devil RuthlessAsura(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 4:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 5`s guardian devil SoulStrangler(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 5:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 6`s guardian devil DarkGlutton(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 6:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 7`s guardian devil AlienDragon(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 7:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 8`s guardian devil ShadowSpider(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                default:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 9`s guardian devil FlameGiant(common).No need to sweep again.")
                             .AddOption("Okay.", 255).AddAvatar(23)
                      .FinalizeDialog();
                                        break;
                                    }
                            }

                        }

                        break;
                    }
                case 76:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Alright.", 255).AddAvatar(23)
                     .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.HaveSpace(5))
                        {
                            data.AddText("Thanks for your challenge. However, you inventory is too full to contain the reward.Please clear at least 5 empty spaces, first.")
                               .AddOption("Okay.", 255).AddAvatar(23)
                                .FinalizeDialog();

                            break;
                        }
                        if (client.Player.ConquerPoints < 430)
                        {
                            data.AddText("It seems you don`t have enough ConquerPoints to `Sweep` the  reward. Please make sure you bring enough ConquerPoints, next time.")
                           .AddOption("Alright.", 255).AddAvatar(23)
                    .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.TOM_SellectOptionC) != 1u << client.Player.TOM_SellectOptionC)
                        {
                            if (client.InTrade) return;
                            client.Player.TowerOfMysterychallenge -= 1;
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            client.Player.TowerOfMysteryChallengeFlag |= 1u << client.Player.TOM_SellectOptionC;
                            client.Player.ConquerPoints -= 430;
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        client.Inventory.Add(stream, 3008956);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        client.Inventory.Add(stream, 3008957);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                        client.Inventory.Add(stream, 3008958);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        client.Inventory.Add(stream, 3008959);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                        client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        client.Inventory.Add(stream, 3008960);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                            }
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            data.AddText("You've successfully cleared this floor, and you deserve this reward.Here you are.")
                               .AddOption("Thanks!", 255).AddAvatar(23)
                                .FinalizeDialog();
                        }
                        else
                        {
                            switch (client.Player.TOM_SellectOptionC)
                            {
                                case 0:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 1`s guardian devil GreatPheasant(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 1:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 2`s guardian devil WingedLord(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 2:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 3`s guardian devil WaterTerror(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 3:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 4`s guardian devil RuthlessAsura(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 4:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 5`s guardian devil SoulStrangler(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 5:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 6`s guardian devil DarkGlutton(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 6:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 7`s guardian devil AlienDragon(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 7:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 8`s guardian devil ShadowSpider(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                default:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 9`s guardian devil FlameGiant(common).No need to sweep again.")
                             .AddOption("Okay.", 255).AddAvatar(23)
                      .FinalizeDialog();
                                        break;
                                    }
                            }

                        }

                        break;
                    }
                case 74:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 75)
                              .AddOption("I~haven`t~decided,~yet!", 255).AddAvatar(23)
                       .FinalizeDialog();

                            }
                            else
                                goto case 75;
                        }
                        break;
                    }
                case 75:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            client.Inventory.Remove(3008735, 1, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 3;
                        }
                        break;
                    }
                case 72:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 73)
                              .AddOption("I~haven`t~decided,~yet!", 255).AddAvatar(23)
                       .FinalizeDialog();

                            }
                            else
                                goto case 73;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255).AddAvatar(23)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 73:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            client.Player.SubClass.RemoveStudy(client, 50, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 3;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255).AddAvatar(23)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 70:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 71)
                              .AddOption("I~haven`t~decided,~yet!", 255).AddAvatar(23)
                       .FinalizeDialog();

                            }
                            else
                                goto case 71;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255).AddAvatar(23)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 71:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 20;
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 3;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255).AddAvatar(23)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.CanSweapTowerOfMystery)
                        {
                            data.AddText("                        Bright Tribe`s Reward         ")
                           .AddText("                     **************************************");
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>Refined</tip>");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>2-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>Unique</tip>");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>3-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>Elite</tip>");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>4-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>Super</tip>");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>5-Stars</tip>");
                                        break;
                                    }
                            }
                            data.AddText("                    **************************************");
                            data.AddText("                              Refreshing Times: " + client.Player.TOMRefreshReward.ToString() + "");

                            if (client.Player.TOM_Reward != MsgTournaments.MsgTowerOfMystery.RewardTypes.Super)
                            {
                                data.AddOption("Refresh~reward.~(20~CP)", 170);
                                data.AddOption("Enable~`Sweep`.~(860~CPs)", 176);//todo
                                data.AddOption("Refresh~reward.~(50~Study~Pts)", 172);
                                data.AddOption("`Sweep`.~(4~DragonBalls)", 177);//todo
                                if (client.Inventory.Contain(3008735, 1))
                                    data.AddOption("Refresh~reward.~(FierceSpirit)", 174);
                            }
                            else
                            {
                                data.AddOption("Enable~`Sweep`.~(860~CPs)", 176);
                                data.AddOption("`Sweep`.~(4~DragonBalls)", 177);
                            }

                            data.AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(23)
                            .FinalizeDialog();

                        }
                        break;
                    }
                case 177:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Alright.", 255).AddAvatar(23)
                     .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.HaveSpace(5))
                        {
                            data.AddText("Thanks for your challenge. However, you inventory is too full to contain the reward.Please clear at least 5 empty spaces, first.")
                               .AddOption("Okay.", 255).AddAvatar(23)
                                .FinalizeDialog();

                            break;
                        }
                        if (client.Inventory.Contain(Database.ItemType.DragonBall, 4) == false)
                        {
                            data.AddText("It seems you don`t have enough DragonBalls to directly clear this floor.Please check it again.")
                           .AddOption("Alright.", 255).AddAvatar(23)
                    .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.TOM_SellectOptionC) != 1u << client.Player.TOM_SellectOptionC)
                        {
                            client.Player.TowerOfMysterychallenge -= 1;
                            client.Player.TowerOfMysteryChallengeFlag |= 1u << client.Player.TOM_SellectOptionC;
                            client.Inventory.Remove(Database.ItemType.DragonBall, 4, stream);
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);

                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        client.Inventory.Add(stream, 3008963);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        client.Inventory.AddItemWitchStack(3007284, 0, 1, stream);
                                        client.Inventory.Add(stream, 3008964);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        client.Player.SubClass.AddStudyPoints(client, 40, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        client.Inventory.AddItemWitchStack(3007284, 0, 2, stream);
                                        client.Player.SubClass.AddStudyPoints(client, 80, stream);
                                        client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                        client.Inventory.Add(stream, 3008965);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        client.Inventory.AddItemWitchStack(3007284, 0, 3, stream);
                                        client.Player.SubClass.AddStudyPoints(client, 120, stream);
                                        client.Inventory.Add(stream, 3008966);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                        client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        client.Inventory.AddItemWitchStack(3007284, 0, 5, stream);
                                        client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                        client.Inventory.Add(stream, 3008967);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                            }
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            data.AddText("You've successfully cleared this floor, and you deserve this reward.Here you are.")
                               .AddOption("Thanks!", 255).AddAvatar(23)
                                .FinalizeDialog();
                        }
                        else
                        {
                            switch (client.Player.TOM_SellectOptionC)
                            {
                                case 0:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 1`s guardian devil GreatPheasant(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 1:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 2`s guardian devil WingedLord(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 2:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 3`s guardian devil WaterTerror(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 3:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 4`s guardian devil RuthlessAsura(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 4:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 5`s guardian devil SoulStrangler(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 5:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 6`s guardian devil DarkGlutton(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 6:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 7`s guardian devil AlienDragon(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 7:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 8`s guardian devil ShadowSpider(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                default:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 9`s guardian devil FlameGiant(Elite).No need to sweep again.")
                             .AddOption("Okay.", 255).AddAvatar(23)
                      .FinalizeDialog();
                                        break;
                                    }
                            }

                        }

                        break;
                    }
                case 176:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Alright.", 255).AddAvatar(23)
                     .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.HaveSpace(5))
                        {
                            data.AddText("Thanks for your challenge. However, you inventory is too full to contain the reward.Please clear at least 5 empty spaces, first.")
                               .AddOption("Okay.", 255).AddAvatar(23)
                                .FinalizeDialog();

                            break;
                        }
                        if (client.Player.ConquerPoints < 860)
                        {
                            data.AddText("It seems you don`t have enough ConquerPoints to `Sweep` the  reward. Please make sure you bring enough ConquerPoints, next time.")
                           .AddOption("Alright.", 255).AddAvatar(23)
                    .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.TOM_SellectOptionC) != 1u << client.Player.TOM_SellectOptionC)
                        {
                            if (client.InTrade) return;
                            client.Player.TowerOfMysterychallenge -= 1;
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            client.Player.TowerOfMysteryChallengeFlag |= 1u << client.Player.TOM_SellectOptionC;
                            client.Player.ConquerPoints -= 860;
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        client.Inventory.Add(stream, 3008963);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        client.Inventory.Add(stream, 3008964);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        client.Player.SubClass.AddStudyPoints(client, 40, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        client.Player.SubClass.AddStudyPoints(client, 80, stream);
                                        client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                        client.Inventory.Add(stream, 3008965);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        client.Player.SubClass.AddStudyPoints(client, 120, stream);
                                        client.Inventory.Add(stream, 3008966);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                        client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                        client.Inventory.Add(stream, 3008967);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                            }
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            data.AddText("You've successfully cleared this floor, and you deserve this reward.Here you are.")
                               .AddOption("Thanks!", 255).AddAvatar(23)
                                .FinalizeDialog();
                        }
                        else
                        {
                            switch (client.Player.TOM_SellectOptionC)
                            {
                                case 0:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 1`s guardian devil GreatPheasant(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 1:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 2`s guardian devil WingedLord(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 2:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 3`s guardian devil WaterTerror(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 3:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 4`s guardian devil RuthlessAsura(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 4:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 5`s guardian devil SoulStrangler(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 5:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 6`s guardian devil DarkGlutton(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 6:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 7`s guardian devil AlienDragon(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 7:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 8`s guardian devil ShadowSpider(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(23)
                       .FinalizeDialog();
                                        break;
                                    }
                                default:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 9`s guardian devil FlameGiant(Elite).No need to sweep again.")
                             .AddOption("Okay.", 255).AddAvatar(23)
                      .FinalizeDialog();
                                        break;
                                    }
                            }

                        }

                        break;
                    }
                case 174:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 175)
                              .AddOption("I~haven`t~decided,~yet!", 255).AddAvatar(23)
                       .FinalizeDialog();

                            }
                            else
                                goto case 175;
                        }
                        break;
                    }
                case 175:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            client.Inventory.Remove(3008735, 1, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 2;
                        }
                        break;
                    }
                case 172:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 173)
                              .AddOption("I~haven`t~decided,~yet!", 255).AddAvatar(23)
                       .FinalizeDialog();

                            }
                            else
                                goto case 173;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255).AddAvatar(23)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 173:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            client.Player.SubClass.RemoveStudy(client, 50, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 2;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255).AddAvatar(23)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 170:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 171)
                              .AddOption("I~haven`t~decided,~yet!", 255).AddAvatar(23)
                       .FinalizeDialog();

                            }
                            else
                                goto case 171;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255).AddAvatar(23)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 171:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 20;
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 2;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255).AddAvatar(23)
                      .FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.TowerofMysteryChallenge4)]
        public static void TowerofMysteryChallenge4(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.HeavenlyMaster, 0, 6126);
            Dialog data = new Dialog(client, stream);
            var Monster = Game.MsgTournaments.MsgSchedules.TowerOfMystery.GetMonster(client);
            if (client.Player.TOM_StartChallenge == true && client.Player.TOM_FinishChallenge == false)
                return;

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Leave~the~tower.", 21)
                     .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) == 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            data.AddText("You've successfully cleared this floor, and defeated the Floor " + (client.Player.JoinTowerOfMysteryLayer + 1).ToString() + "`s guardian devil " + Monster.Name + " need to sweep again.")
                              .AddOption("Enter~passage~to~next~floor.", 20)
                              .AddOption("Leave~the~tower.", 21)
                               .FinalizeDialog();
                            break;
                        }
                        data.AddText("Guguga! Guguguga! (You don`t understand the " + Monster.Name + "`s words,but its arrogant expression is really annoying.)")
                            .AddText("                    **************************************\n");
                        if (client.Player.TOMChallengeToday == 1)
                        {

                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>Refined</tip>");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>2-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>Unique</tip>");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>3-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>Elite</tip>");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>4-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>Super</tip>");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>5-Stars</tip>");
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>\n");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>Refined</tip>\n");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>2-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>Unique</tip>\n");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>3-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>Elite</tip>\n");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>4-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>Super</tip>\n");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>5-Stars</tip>\n");
                                        break;
                                    }
                            }
                        }
                        data.AddText("                    **************************************\n");
                        data.AddText("                       Refreshing Times: " + client.Player.TOMRefreshReward.ToString() + "");

                        if (client.Player.TOM_Reward != MsgTournaments.MsgTowerOfMystery.RewardTypes.Super)
                        {
                            data.AddOption("Refresh~reward.~(20~CP)", 2);
                            data.AddOption("Refresh~reward.~(50~Study~Pts)", 4);
                            data.AddOption("Refresh~reward.~(FierceSpirit)", 6);
                        }
                        if (client.Player.TOM_FinishChallenge == true)
                            data.AddOption("Claim~the~reward.", 15);
                        else
                        {
                            data.AddOption("Unseal~devils~for~challenge.", 10);
                            data.AddOption("Return~to~tower~passage.", 22);
                        }

                        data.AddOption("I`ll~think~about~it.", 255)
                        .FinalizeDialog();



                        break;
                    }
                case 21:
                    {
                        client.Teleport(85, 75, 4020);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 20:
                    {
                        client.Player.JoinTowerOfMysteryLayer = (byte)Math.Min(client.Player.JoinTowerOfMysteryLayer + 1, 8);
                        client.Teleport(45, 35, 4009);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 22:
                    {
                        client.Teleport(45, 35, 4009);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 1:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Leave~the~tower.", 21)
                     .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) == 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            data.AddText("You've successfully cleared this floor, and defeated the Floor " + (client.Player.JoinTowerOfMysteryLayer + 1).ToString() + "`s guardian devil " + Monster.Name + " need to sweep again.")
                              .AddOption("Enter~passage~to~next~floor.", 20)
                              .AddOption("Leave~the~tower.", 21)
                               .FinalizeDialog();
                            break;
                        }
                        data.AddText("Guguga! Guguguga! (You don`t understand the " + Monster.Name + "`s words,but its arrogant expression is really annoying.)")
                            .AddText("                    **************************************\n");
                        if (client.Player.TOMChallengeToday == 1)
                        {

                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>Refined</tip>");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>2-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>Unique</tip>");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>3-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>Elite</tip>");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>4-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>Super</tip>");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>5-Stars</tip>");
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>\n");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>Refined</tip>\n");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>2-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>Unique</tip>\n");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>3-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>Elite</tip>\n");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>4-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>Super</tip>\n");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>5-Stars</tip>\n");
                                        break;
                                    }
                            }
                        }
                        data.AddText("                    **************************************\n");
                        data.AddText("                       Refreshing Times: " + client.Player.TOMRefreshReward.ToString() + "");

                        if (client.Player.TOM_Reward != MsgTournaments.MsgTowerOfMystery.RewardTypes.Super)
                        {
                            data.AddOption("Refresh~reward.~(20~CP)", 2);
                            data.AddOption("Refresh~reward.~(50~Study~Pts)", 4);
                            data.AddOption("Refresh~reward.~(FierceSpirit)", 6);
                        }
                        if (client.Player.TOM_FinishChallenge == true)
                            data.AddOption("Claim~the~reward.", 15);
                        else
                        {
                            data.AddOption("Unseal~devils~for~challenge.", 10);
                            data.AddOption("Return~to~tower~passage.", 22);
                        }

                        data.AddOption("I`ll~think~about~it.", 255)
                        .FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) != 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            client.Player.TowerOfMysterychallenge -= 1;
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            client.Player.TowerOfMysteryChallengeFlag |= 1u << client.Player.JoinTowerOfMysteryLayer;

                            if (client.Player.TOMChallengeToday == 1)
                            {
                                switch (client.Player.TOM_Reward)
                                {
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                        {
                                            client.Inventory.Add(stream, 3008963);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008964);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 40, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 2, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 80, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008965);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 3, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 120, stream);
                                            client.Inventory.Add(stream, 3008966);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 5, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                            client.Inventory.Add(stream, 3008967);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                switch (client.Player.TOM_Reward)
                                {
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                        {
                                            client.Inventory.Add(stream, 3008956);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                        {
                                            client.Inventory.Add(stream, 3008957);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                        {
                                            client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008958);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                        {
                                            client.Inventory.Add(stream, 3008959);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                        {
                                            client.Inventory.Add(stream, 3008960);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                }
                            }
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            data.AddText("You've successfully cleared this floor, and you deserve this reward.Here you are.")
                               .AddOption("Thanks!", 1).AddAvatar(14)
                                .FinalizeDialog();
                        }
                        break;

                    }
                case 10:
                    {
                        client.SendSysMesage("The Seal on[" + Monster.Name + "] has been removed, and " + Monster.Name + " appeared at the center of this floor. Raise your weapon and knock down [" + Monster.Name + "]!", MsgMessage.ChatMode.System);

                        Server.AddMapMonster(stream, client.Map, Monster.ID, client.Player.X, client.Player.Y, 8, 8, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                        if (client.Player.JoinTowerOfMysteryLayer == 1)
                            Server.AddMapMonster(stream, client.Map, (uint)(40932 + client.Player.TOMChallengeToday * 10), client.Player.X, client.Player.Y, 8, 8, 3, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);

                        client.Player.TOM_StartChallenge = true;

                        foreach (var npc in client.Player.View.Roles(Role.MapObjectType.Npc))
                        {
                            ActionQuery action;

                            action = new ActionQuery()
                            {
                                ObjId = npc.UID,
                                Type = ActionType.RemoveEntity
                            };
                            client.Player.View.SendView(stream.ActionCreate(action), true);
                        }
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "new_gamebegin");
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 7)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 7;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have a Fierce Spirit to refresh the reward level to 3-star or above. You can collect it by opening the Outstanding Exploit Pack every day.")
                             .AddOption("I~see.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            client.Inventory.Remove(3008735, 1, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have a Fierce Spirit to refresh the reward level to 3-star or above. You can collect it by opening the Outstanding Exploit Pack every day.")
                             .AddOption("I~see.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 5)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 5;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            client.Player.SubClass.RemoveStudy(client, 50, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 3)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 3;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 20;
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.TowerofMysteryChallenge3)]
        public static void TowerofMysteryChallenge3(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.HeavenlyMaster, 0, 6126);
            Dialog data = new Dialog(client, stream);
            var Monster = Game.MsgTournaments.MsgSchedules.TowerOfMystery.GetMonster(client);
            if (client.Player.TOM_StartChallenge == true && client.Player.TOM_FinishChallenge == false)
                return;

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Leave~the~tower.", 21)
                     .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) == 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            data.AddText("You've successfully cleared this floor, and defeated the Floor " + (client.Player.JoinTowerOfMysteryLayer + 1).ToString() + "`s guardian devil " + Monster.Name + " need to sweep again.")
                              .AddOption("Enter~passage~to~next~floor.", 20)
                              .AddOption("Leave~the~tower.", 21)
                               .FinalizeDialog();
                            break;
                        }
                        data.AddText("Guguga! Guguguga! (You don`t understand the " + Monster.Name + "`s words,but its arrogant expression is really annoying.)")
                            .AddText("                    **************************************\n");
                        if (client.Player.TOMChallengeToday == 1)
                        {

                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>Refined</tip>");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>2-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>Unique</tip>");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>3-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>Elite</tip>");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>4-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>Super</tip>");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>5-Stars</tip>");
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>\n");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>Refined</tip>\n");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>2-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>Unique</tip>\n");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>3-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>Elite</tip>\n");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>4-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>Super</tip>\n");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>5-Stars</tip>\n");
                                        break;
                                    }
                            }
                        }
                        data.AddText("                    **************************************\n");
                        data.AddText("                       Refreshing Times: " + client.Player.TOMRefreshReward.ToString() + "");

                        if (client.Player.TOM_Reward != MsgTournaments.MsgTowerOfMystery.RewardTypes.Super)
                        {
                            data.AddOption("Refresh~reward.~(20~CP)", 2);
                            data.AddOption("Refresh~reward.~(50~Study~Pts)", 4);
                            data.AddOption("Refresh~reward.~(FierceSpirit)", 6);
                        }
                        if (client.Player.TOM_FinishChallenge == true)
                            data.AddOption("Claim~the~reward.", 15);
                        else
                        {
                            data.AddOption("Unseal~devils~for~challenge.", 10);
                            data.AddOption("Return~to~tower~passage.", 22);
                        }

                        data.AddOption("I`ll~think~about~it.", 255)
                        .FinalizeDialog();



                        break;
                    }
                case 21:
                    {
                        client.Teleport(85, 75, 4020);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 20:
                    {
                        client.Player.JoinTowerOfMysteryLayer = (byte)Math.Min(client.Player.JoinTowerOfMysteryLayer + 1, 8);
                        client.Teleport(45, 35, 4009);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 22:
                    {
                        client.Teleport(45, 35, 4009);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 1:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Leave~the~tower.", 21)
                     .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) == 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            data.AddText("You've successfully cleared this floor, and defeated the Floor " + (client.Player.JoinTowerOfMysteryLayer + 1).ToString() + "`s guardian devil " + Monster.Name + " need to sweep again.")
                              .AddOption("Enter~passage~to~next~floor.", 20)
                              .AddOption("Leave~the~tower.", 21)
                               .FinalizeDialog();
                            break;
                        }
                        data.AddText("Guguga! Guguguga! (You don`t understand the " + Monster.Name + "`s words,but its arrogant expression is really annoying.)")
                            .AddText("                    **************************************\n");
                        if (client.Player.TOMChallengeToday == 1)
                        {

                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>Refined</tip>");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>2-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>Unique</tip>");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>3-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>Elite</tip>");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>4-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>Super</tip>");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>5-Stars</tip>");
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>\n");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>Refined</tip>\n");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>2-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>Unique</tip>\n");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>3-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>Elite</tip>\n");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>4-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>Super</tip>\n");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>5-Stars</tip>\n");
                                        break;
                                    }
                            }
                        }
                        data.AddText("                    **************************************\n");
                        data.AddText("                       Refreshing Times: " + client.Player.TOMRefreshReward.ToString() + "");

                        if (client.Player.TOM_Reward != MsgTournaments.MsgTowerOfMystery.RewardTypes.Super)
                        {
                            data.AddOption("Refresh~reward.~(20~CP)", 2);
                            data.AddOption("Refresh~reward.~(50~Study~Pts)", 4);
                            data.AddOption("Refresh~reward.~(FierceSpirit)", 6);
                        }
                        if (client.Player.TOM_FinishChallenge == true)
                            data.AddOption("Claim~the~reward.", 15);
                        else
                        {
                            data.AddOption("Unseal~devils~for~challenge.", 10);
                            data.AddOption("Return~to~tower~passage.", 22);
                        }

                        data.AddOption("I`ll~think~about~it.", 255)
                        .FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) != 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            client.Player.TowerOfMysterychallenge -= 1;
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            client.Player.TowerOfMysteryChallengeFlag |= 1u << client.Player.JoinTowerOfMysteryLayer;

                            if (client.Player.TOMChallengeToday == 1)
                            {
                                switch (client.Player.TOM_Reward)
                                {
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                        {
                                            client.Inventory.Add(stream, 3008963);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008964);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 40, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 2, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 80, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008965);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 3, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 120, stream);
                                            client.Inventory.Add(stream, 3008966);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 5, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                            client.Inventory.Add(stream, 3008967);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                switch (client.Player.TOM_Reward)
                                {
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                        {
                                            client.Inventory.Add(stream, 3008956);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                        {
                                            client.Inventory.Add(stream, 3008957);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                        {
                                            client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008958);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                        {
                                            client.Inventory.Add(stream, 3008959);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                        {
                                            client.Inventory.Add(stream, 3008960);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                }
                            }
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            data.AddText("You've successfully cleared this floor, and you deserve this reward.Here you are.")
                               .AddOption("Thanks!", 1).AddAvatar(14)
                                .FinalizeDialog();
                        }
                        break;

                    }
                case 10:
                    {
                        client.SendSysMesage("The Seal on[" + Monster.Name + "] has been removed, and " + Monster.Name + " appeared at the center of this floor. Raise your weapon and knock down [" + Monster.Name + "]!", MsgMessage.ChatMode.System);

                        Server.AddMapMonster(stream, client.Map, Monster.ID, client.Player.X, client.Player.Y, 8, 8, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                        if (client.Player.JoinTowerOfMysteryLayer == 1)
                            Server.AddMapMonster(stream, client.Map, (uint)(40932 + client.Player.TOMChallengeToday * 10), client.Player.X, client.Player.Y, 8, 8, 3, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);

                        client.Player.TOM_StartChallenge = true;

                        foreach (var npc in client.Player.View.Roles(Role.MapObjectType.Npc))
                        {
                            ActionQuery action;

                            action = new ActionQuery()
                            {
                                ObjId = npc.UID,
                                Type = ActionType.RemoveEntity
                            };
                            client.Player.View.SendView(stream.ActionCreate(action), true);
                        }
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "new_gamebegin");
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 7)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 7;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have a Fierce Spirit to refresh the reward level to 3-star or above. You can collect it by opening the Outstanding Exploit Pack every day.")
                             .AddOption("I~see.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            client.Inventory.Remove(3008735, 1, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have a Fierce Spirit to refresh the reward level to 3-star or above. You can collect it by opening the Outstanding Exploit Pack every day.")
                             .AddOption("I~see.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 5)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 5;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            client.Player.SubClass.RemoveStudy(client, 50, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 3)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 3;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 20;
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.TowerofMysteryChallenge2)]
        public static void TowerofMysteryChallenge2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.HeavenlyMaster, 0, 6126);
            Dialog data = new Dialog(client, stream);
            var Monster = Game.MsgTournaments.MsgSchedules.TowerOfMystery.GetMonster(client);
            if (client.Player.TOM_StartChallenge == true && client.Player.TOM_FinishChallenge == false)
                return;

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Leave~the~tower.", 21)
                     .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) == 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            data.AddText("You've successfully cleared this floor, and defeated the Floor " + (client.Player.JoinTowerOfMysteryLayer + 1).ToString() + "`s guardian devil " + Monster.Name + " need to sweep again.")
                              .AddOption("Enter~passage~to~next~floor.", 20)
                              .AddOption("Leave~the~tower.", 21)
                               .FinalizeDialog();
                            break;
                        }
                        data.AddText("Guguga! Guguguga! (You don`t understand the " + Monster.Name + "`s words,but its arrogant expression is really annoying.)")
                            .AddText("                    **************************************\n");
                        if (client.Player.TOMChallengeToday == 1)
                        {

                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>Refined</tip>");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>2-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>Unique</tip>");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>3-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>Elite</tip>");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>4-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>Super</tip>");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>5-Stars</tip>");
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>\n");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>Refined</tip>\n");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>2-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>Unique</tip>\n");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>3-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>Elite</tip>\n");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>4-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>Super</tip>\n");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>5-Stars</tip>\n");
                                        break;
                                    }
                            }
                        }
                        data.AddText("                    **************************************\n");
                        data.AddText("                       Refreshing Times: " + client.Player.TOMRefreshReward.ToString() + "");

                        if (client.Player.TOM_Reward != MsgTournaments.MsgTowerOfMystery.RewardTypes.Super)
                        {
                            data.AddOption("Refresh~reward.~(20~CP)", 2);
                            data.AddOption("Refresh~reward.~(50~Study~Pts)", 4);
                            data.AddOption("Refresh~reward.~(FierceSpirit)", 6);
                        }
                        if (client.Player.TOM_FinishChallenge == true)
                            data.AddOption("Claim~the~reward.", 15);
                        else
                        {
                            data.AddOption("Unseal~devils~for~challenge.", 10);
                            data.AddOption("Return~to~tower~passage.", 22);
                        }

                        data.AddOption("I`ll~think~about~it.", 255)
                        .FinalizeDialog();



                        break;
                    }
                case 21:
                    {
                        client.Teleport(85, 75, 4020);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 20:
                    {
                        client.Player.JoinTowerOfMysteryLayer = (byte)Math.Min(client.Player.JoinTowerOfMysteryLayer + 1, 8);
                        client.Teleport(45, 35, 4009);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 22:
                    {
                        client.Teleport(45, 35, 4009);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 1:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Leave~the~tower.", 21)
                     .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) == 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            data.AddText("You've successfully cleared this floor, and defeated the Floor " + (client.Player.JoinTowerOfMysteryLayer + 1).ToString() + "`s guardian devil " + Monster.Name + " need to sweep again.")
                              .AddOption("Enter~passage~to~next~floor.", 20)
                              .AddOption("Leave~the~tower.", 21)
                               .FinalizeDialog();
                            break;
                        }
                        data.AddText("Guguga! Guguguga! (You don`t understand the " + Monster.Name + "`s words,but its arrogant expression is really annoying.)")
                            .AddText("                    **************************************\n");
                        if (client.Player.TOMChallengeToday == 1)
                        {

                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>Refined</tip>");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>2-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>Unique</tip>");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>3-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>Elite</tip>");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>4-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>Super</tip>");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>5-Stars</tip>");
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>\n");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>Refined</tip>\n");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>2-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>Unique</tip>\n");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>3-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>Elite</tip>\n");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>4-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>Super</tip>\n");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>5-Stars</tip>\n");
                                        break;
                                    }
                            }
                        }
                        data.AddText("                    **************************************\n");
                        data.AddText("                       Refreshing Times: " + client.Player.TOMRefreshReward.ToString() + "");

                        if (client.Player.TOM_Reward != MsgTournaments.MsgTowerOfMystery.RewardTypes.Super)
                        {
                            data.AddOption("Refresh~reward.~(20~CP)", 2);
                            data.AddOption("Refresh~reward.~(50~Study~Pts)", 4);
                            data.AddOption("Refresh~reward.~(FierceSpirit)", 6);
                        }
                        if (client.Player.TOM_FinishChallenge == true)
                            data.AddOption("Claim~the~reward.", 15);
                        else
                        {
                            data.AddOption("Unseal~devils~for~challenge.", 10);
                            data.AddOption("Return~to~tower~passage.", 22);
                        }

                        data.AddOption("I`ll~think~about~it.", 255)
                        .FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) != 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            client.Player.TowerOfMysterychallenge -= 1;
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            client.Player.TowerOfMysteryChallengeFlag |= 1u << client.Player.JoinTowerOfMysteryLayer;

                            if (client.Player.TOMChallengeToday == 1)
                            {
                                switch (client.Player.TOM_Reward)
                                {
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                        {
                                            client.Inventory.Add(stream, 3008963);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008964);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 40, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 2, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 80, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008965);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 3, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 120, stream);
                                            client.Inventory.Add(stream, 3008966);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 5, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                            client.Inventory.Add(stream, 3008967);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                switch (client.Player.TOM_Reward)
                                {
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                        {
                                            client.Inventory.Add(stream, 3008956);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                        {
                                            client.Inventory.Add(stream, 3008957);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                        {
                                            client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008958);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                        {
                                            client.Inventory.Add(stream, 3008959);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                        {
                                            client.Inventory.Add(stream, 3008960);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                }
                            }
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            data.AddText("You've successfully cleared this floor, and you deserve this reward.Here you are.")
                               .AddOption("Thanks!", 1).AddAvatar(14)
                                .FinalizeDialog();
                        }
                        break;

                    }
                case 10:
                    {
                        client.SendSysMesage("The Seal on[" + Monster.Name + "] has been removed, and " + Monster.Name + " appeared at the center of this floor. Raise your weapon and knock down [" + Monster.Name + "]!", MsgMessage.ChatMode.System);

                        Server.AddMapMonster(stream, client.Map, Monster.ID, client.Player.X, client.Player.Y, 8, 8, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                        if (client.Player.JoinTowerOfMysteryLayer == 1)
                            Server.AddMapMonster(stream, client.Map, (uint)(40932 + client.Player.TOMChallengeToday * 10), client.Player.X, client.Player.Y, 8, 8, 3, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);

                        client.Player.TOM_StartChallenge = true;

                        foreach (var npc in client.Player.View.Roles(Role.MapObjectType.Npc))
                        {
                            ActionQuery action;

                            action = new ActionQuery()
                            {
                                ObjId = npc.UID,
                                Type = ActionType.RemoveEntity
                            };
                            client.Player.View.SendView(stream.ActionCreate(action), true);
                        }
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "new_gamebegin");
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 7)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 7;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have a Fierce Spirit to refresh the reward level to 3-star or above. You can collect it by opening the Outstanding Exploit Pack every day.")
                             .AddOption("I~see.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            client.Inventory.Remove(3008735, 1, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have a Fierce Spirit to refresh the reward level to 3-star or above. You can collect it by opening the Outstanding Exploit Pack every day.")
                             .AddOption("I~see.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 5)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 5;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            client.Player.SubClass.RemoveStudy(client, 50, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 3)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 3;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 20;
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.TowerofMysteryChallenge1)]
        public static void TowerofMysteryChallenge1(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.HeavenlyMaster, 0, 6126);
            Dialog data = new Dialog(client, stream);
            var Monster = Game.MsgTournaments.MsgSchedules.TowerOfMystery.GetMonster(client);
            if (client.Player.TOM_StartChallenge == true && client.Player.TOM_FinishChallenge == false)
                return;

            switch (Option)
            {
                case 0:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Leave~the~tower.", 21)
                     .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) == 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            data.AddText("You've successfully cleared this floor, and defeated the Floor " + (client.Player.JoinTowerOfMysteryLayer + 1).ToString() + "`s guardian devil " + Monster.Name + " need to sweep again.")
                              .AddOption("Enter~passage~to~next~floor.", 20)
                              .AddOption("Leave~the~tower.", 21)
                               .FinalizeDialog();
                            break;
                        }
                        data.AddText("Guguga! Guguguga! (You don`t understand the " + Monster.Name + "`s words,but its arrogant expression is really annoying.)")
                            .AddText("                    **************************************\n");
                        if (client.Player.TOMChallengeToday == 1)
                        {

                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>Refined</tip>");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>2-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>Unique</tip>");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>3-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>Elite</tip>");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>4-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>Super</tip>");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>5-Stars</tip>");
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>\n");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>Refined</tip>\n");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>2-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>Unique</tip>\n");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>3-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>Elite</tip>\n");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>4-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>Super</tip>\n");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>5-Stars</tip>\n");
                                        break;
                                    }
                            }
                        }
                        data.AddText("                    **************************************\n");
                        data.AddText("                       Refreshing Times: " + client.Player.TOMRefreshReward.ToString() + "");

                        if (client.Player.TOM_Reward != MsgTournaments.MsgTowerOfMystery.RewardTypes.Super)
                        {
                            data.AddOption("Refresh~reward.~(20~CP)", 2);
                            data.AddOption("Refresh~reward.~(50~Study~Pts)", 4);
                            data.AddOption("Refresh~reward.~(FierceSpirit)", 6);
                        }
                        if (client.Player.TOM_FinishChallenge == true)
                            data.AddOption("Claim~the~reward.", 15);
                        else
                        {
                            data.AddOption("Unseal~devils~for~challenge.", 10);
                            data.AddOption("Return~to~tower~passage.", 22);
                        }

                        data.AddOption("I`ll~think~about~it.", 255)
                        .FinalizeDialog();



                        break;
                    }
                case 21:
                    {
                        client.Teleport(85, 75, 4020);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 20:
                    {
                        client.Player.JoinTowerOfMysteryLayer = (byte)Math.Min(client.Player.JoinTowerOfMysteryLayer + 1, 8);
                        client.Teleport(45, 35, 4009);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 22:
                    {
                        client.Teleport(45, 35, 4009);
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "movego");
                        break;
                    }
                case 1:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Leave~the~tower.", 21)
                     .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) == 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            data.AddText("You've successfully cleared this floor, and defeated the Floor " + (client.Player.JoinTowerOfMysteryLayer + 1).ToString() + "`s guardian devil " + Monster.Name + " need to sweep again.")
                              .AddOption("Enter~passage~to~next~floor.", 20)
                              .AddOption("Leave~the~tower.", 21)
                               .FinalizeDialog();
                            break;
                        }
                        data.AddText("Guguga! Guguguga! (You don`t understand the " + Monster.Name + "`s words,but its arrogant expression is really annoying.)")
                            .AddText("                    **************************************\n");
                        if (client.Player.TOMChallengeToday == 1)
                        {

                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>Refined</tip>");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>2-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>Unique</tip>");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>3-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>Elite</tip>");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>4-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>Super</tip>");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>5-Stars</tip>");
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>\n");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>Refined</tip>\n");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>2-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>Unique</tip>\n");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>3-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>Elite</tip>\n");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>4-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>Super</tip>\n");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>5-Stars</tip>\n");
                                        break;
                                    }
                            }
                        }
                        data.AddText("                    **************************************\n");
                        data.AddText("                       Refreshing Times: " + client.Player.TOMRefreshReward.ToString() + "");

                        if (client.Player.TOM_Reward != MsgTournaments.MsgTowerOfMystery.RewardTypes.Super)
                        {
                            data.AddOption("Refresh~reward.~(20~CP)", 2);
                            data.AddOption("Refresh~reward.~(50~Study~Pts)", 4);
                            data.AddOption("Refresh~reward.~(FierceSpirit)", 6);
                        }
                        if (client.Player.TOM_FinishChallenge == true)
                            data.AddOption("Claim~the~reward.", 15);
                        else
                        {
                            data.AddOption("Unseal~devils~for~challenge.", 10);
                            data.AddOption("Return~to~tower~passage.", 22);
                        }

                        data.AddOption("I`ll~think~about~it.", 255)
                        .FinalizeDialog();
                        break;
                    }
                case 15:
                    {
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.JoinTowerOfMysteryLayer) != 1u << client.Player.JoinTowerOfMysteryLayer)
                        {
                            client.Player.TowerOfMysterychallenge -= 1;
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            client.Player.TowerOfMysteryChallengeFlag |= 1u << client.Player.JoinTowerOfMysteryLayer;

                            if (client.Player.TOMChallengeToday == 1)
                            {
                                switch (client.Player.TOM_Reward)
                                {
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                        {
                                            client.Inventory.Add(stream, 3008963);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008964);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 40, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 2, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 80, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008965);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 3, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 120, stream);
                                            client.Inventory.Add(stream, 3008966);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                        {
                                            client.Inventory.AddItemWitchStack(3007284, 0, 5, stream);
                                            client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                            client.Inventory.Add(stream, 3008967);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                switch (client.Player.TOM_Reward)
                                {
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                        {
                                            client.Inventory.Add(stream, 3008956);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                        {
                                            client.Inventory.Add(stream, 3008957);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                        {
                                            client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                            client.Inventory.Add(stream, 3008958);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                        {
                                            client.Inventory.Add(stream, 3008959);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                            client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            break;
                                        }
                                    case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                        {
                                            client.Inventory.Add(stream, 3008960);
                                            client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                            client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                            client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                            Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                            break;
                                        }
                                }
                            }
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            data.AddText("You've successfully cleared this floor, and you deserve this reward.Here you are.")
                               .AddOption("Thanks!", 1).AddAvatar(14)
                                .FinalizeDialog();
                        }
                        break;

                    }
                case 10:
                    {
                        client.SendSysMesage("The Seal on[" + Monster.Name + "] has been removed, and " + Monster.Name + " appeared at the center of this floor. Raise your weapon and knock down [" + Monster.Name + "]!", MsgMessage.ChatMode.System);

                        Server.AddMapMonster(stream, client.Map, Monster.ID, client.Player.X, client.Player.Y, 8, 8, 1, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);
                        if (client.Player.JoinTowerOfMysteryLayer == 1)
                            Server.AddMapMonster(stream, client.Map, (uint)(40932 + client.Player.TOMChallengeToday * 10), client.Player.X, client.Player.Y, 8, 8, 3, client.Player.DynamicID, true, MsgFloorItem.MsgItemPacket.EffectMonsters.Night);

                        client.Player.TOM_StartChallenge = true;

                        foreach (var npc in client.Player.View.Roles(Role.MapObjectType.Npc))
                        {
                            ActionQuery action;

                            action = new ActionQuery()
                            {
                                ObjId = npc.UID,
                                Type = ActionType.RemoveEntity
                            };
                            client.Player.View.SendView(stream.ActionCreate(action), true);
                        }
                        client.Player.SendString(stream, MsgStringPacket.StringID.Effect, true, "new_gamebegin");
                        break;
                    }
                case 6:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 7)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 7;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have a Fierce Spirit to refresh the reward level to 3-star or above. You can collect it by opening the Outstanding Exploit Pack every day.")
                             .AddOption("I~see.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 7:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            client.Inventory.Remove(3008735, 1, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have a Fierce Spirit to refresh the reward level to 3-star or above. You can collect it by opening the Outstanding Exploit Pack every day.")
                             .AddOption("I~see.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 4:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 5)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 5;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 5:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            client.Player.SubClass.RemoveStudy(client, 50, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 2:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 3)
                              .AddOption("I~haven`t~decided,~yet!", 255)
                       .FinalizeDialog();

                            }
                            else
                                goto case 3;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 3:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 20;
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 1;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255)
                      .FinalizeDialog();
                        }
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.TowerofMysteryConductor)]
        public static void TowerofMysteryConductor(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("Where do you want to go? I may be able to help you.")
                            .AddOption("Twin~City.", 1)
                            //  .AddOption("Market.", 2)
                       .AddOption("Nothing.", 255)
                       .AddAvatar(1)
                       .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport(Places.TwinCity);
                        break;
                    }
                case 2:
                    {
                        client.Teleport(Places.TwinCity);
                        break;
                    }

            }
        }
        [NpcAttribute(NpcID.TowerofMysteryConductor2)]
        public static void TowerofMysteryConductor2(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            Dialog data = new Dialog(client, stream);
            switch (Option)
            {
                case 0:
                    {
                        data.AddText("I`m a guide to serve heroes in the Fiery Dragon region. If you want to return to Wind Plain, I can help.")
                            .AddOption("Return~to~Wind~Plain.", 1)
                            // .AddOption("Send~me~to~Market.", 2)
                       .AddOption("I`ll~tell~you~if~I~need.", 255)
                       .AddAvatar(7)
                       .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Teleport(Places.TwinCity);
                        break;
                    }
                case 2:
                    {
                        client.Teleport(Places.Market);
                        break;
                    }

            }
        }

        [NpcAttribute(NpcID.TowerKeeper)]
        public static void TowerKeeper(Client.GameClient client, ServerSockets.Packet stream, byte Option, string Input, uint id)
        {
            var ActiveQuest = Database.QuestInfo.GetFinishQuest((uint)NpcID.HeavenlyMaster, 0, 6126);

            Dialog data = new Dialog(client, stream);

            if (client.Player.Level < 90)
            {
                data.AddText("I do appreciate your courage and sacrifice; I really do. But we need someone who is stronger than either")
             .AddText(" of us to challenge the Tower of Mystery, and then untie the invisible ropes that have bound the")
             .AddText(" Bright Tribe for years.\n")
             .AddText("  Requirement: above Level 90");
                data.AddOption("I~see..", 255)
           .AddAvatar(14)
           .FinalizeDialog();
                return;
            }
            switch (Option)
            {
                case 0:
                    {


                        if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Finished) == false
                            && client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted) == false)
                            client.Player.QuestGUI.Accept(ActiveQuest, 0);
                        if (client.Player.MyTowerOfMysteryLayer >= 9)
                        {
                            if (client.Player.TOMSelectChallengeToday == 0)
                            {
                                data.AddText("You must be here to challenge the Tower of Mystery. Since the tower is extremely dangerous, I suggest you to")
                            .AddText(" ~make adequate preparations, and team up with your friends for the challenge. If you`re ready, I`ll lead you into the Mystery.")
                            .AddText("    You can unlock Elite mode after completing all floors of the tower at Common mode.");
                                data.AddOption("Start~the~challenge.~(Elite)", 91);
                                data.AddOption("Start~the~challenge.~(Common)", 90);
                                data.AddOption("Could~you~tell~me~more~details?", 95);
                                data.AddOption("I`ll~talk~to~you~later.", 255)
                           .AddAvatar(14)
                           .FinalizeDialog();
                                break;
                            }
                        }
                        if (client.Player.TOMSelectChallengeToday == 1 && client.Player.TOMChallengeToday == 1)
                        {

                            data.AddText("You can challenge any floor of the tower every day, but you can only claim the reward up to 3 times in a day.")
                         .AddText("    Today`s challenge mode: Elite\n")
                         .AddText("    Remaining challenge chances: " + client.Player.TowerOfMysterychallenge + "");

                            data.AddOption("Continue~to~challenge~" + (Math.Min(8, client.Player.MyTowerOfMysteryLayerElite + 1)).ToString() + "F.", 110);
                            data.AddOption("Choose~a~floor~to~challenge.", 115);

                            data.AddOption("I`ll~talk~to~you~later.", 255)
                            .AddAvatar(14)
                            .FinalizeDialog();
                            break;
                        }
                        else if (client.Player.MyTowerOfMysteryLayer == 0)
                        {

                            data.AddText("You must be here to challenge the Tower of Mystery. Since the tower is extremely dangerous, I suggest you to")
                                .AddText("~make adequate preparations, and team up with your friends for the challenge. If you`re ready, I`ll lead you into the Mystery.")
                                .AddText("    You can unlock Elite mode after completing all floors of the tower at Common mode.");
                            if (client.Player.MyTowerOfMysteryLayer == 0)
                            {
                                data.AddOption("Start~the~challenge.~(Common)", 1);
                                data.AddOption("Could~you~tell~me~more~details?", 2);
                            }
                            else
                            {
                                //Continue~to~challenge~9F.
                                data.AddOption("Continue~to~challenge~" + (Math.Min(8, (int)client.Player.MyTowerOfMysteryLayer) + 1).ToString() + "F.", 10);
                                data.AddOption("Choose~a~floor~to~challenge.", 15);


                            }
                            data.AddOption("I`ll~talk~to~you~later.", 255)
                            .AddAvatar(14)
                            .FinalizeDialog();
                        }
                        else
                        {
                            data.AddText("You can challenge any floor of the tower every day, but you can only claim the reward up to 3 times in a day.")
                               .AddText("    Today`s challenge mode: Common\n")
                               .AddText("    Remaining challenge chances: " + client.Player.TowerOfMysterychallenge + "");

                            data.AddOption("Continue~to~challenge~" + (Math.Min(8, (int)client.Player.MyTowerOfMysteryLayer) + 1).ToString() + "F.", 10);
                            data.AddOption("Choose~a~floor~to~challenge.", 15);

                            data.AddOption("I`ll~talk~to~you~later.", 255)
                            .AddAvatar(14)
                            .FinalizeDialog();
                        }
                        break;
                    }
                case 95:
                    {
                        if (client.Player.MyTowerOfMysteryLayer >= 9)
                        {
                            if (client.Player.TOMSelectChallengeToday == 0)
                            {

                                data.AddText("There is no limit on the number of times you can challenge the tower, but only 3 chances to claim the reward every day.")
                                  .AddText("~Alternatively, you can use CPs or Meteor Scrolls to start `sweeping` and directly get the reward. If you create a team")
                                  .AddText("~to challenge the tower at Elite mode, you can also claim a Concerted Effort Pack from the Cloud Sweeper after completing it.")
                                 .AddOption("Start~the~challenge.~(Elite)", 91)
                                .AddOption("Start~the~challenge.~(Common)", 90)
                             .AddOption("I`ll~talk~to~you~later.", 255)
                             .AddAvatar(14)
                             .FinalizeDialog();

                            }
                        }
                        break;
                    }
                case 91:
                    {
                        if (client.Player.MyTowerOfMysteryLayer >= 9)
                        {
                            if (client.Player.TOMSelectChallengeToday == 0)
                            {
                                client.Player.TOMSelectChallengeToday = 1;
                                client.Player.TOMChallengeToday = 1;
                                data.AddText("You can challenge any floor of the tower every day, but you can only claim the reward up to 3 times in a day.")
                           .AddText("    Today`s challenge mode: Elite\n")
                           .AddText("    Remaining challenge chances: " + client.Player.TowerOfMysterychallenge + "");

                                data.AddOption("Continue~to~challenge~" + (Math.Min(8, (int)client.Player.MyTowerOfMysteryLayerElite) + 1).ToString() + "F.", 110);
                                data.AddOption("Choose~a~floor~to~challenge.", 115);

                                data.AddOption("I`ll~talk~to~you~later.", 255)
                                .AddAvatar(14)
                                .FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 90:
                    {
                        if (client.Player.MyTowerOfMysteryLayer >= 9)
                        {
                            if (client.Player.TOMSelectChallengeToday == 0)
                            {
                                client.Player.TOMSelectChallengeToday = 1;
                                client.Player.TOMChallengeToday = 0;

                                data.AddText("You can challenge any floor of the tower every day, but you can only claim the reward up to 3 times in a day.")
                              .AddText("    Today`s challenge mode: Common")
                              .AddText("    Remaining challenge chances: " + client.Player.MyTowerOfMysteryLayer + "");

                                data.AddOption("Continue~to~challenge~" + (Math.Min(8, (int)client.Player.MyTowerOfMysteryLayer) + 1).ToString() + "F.", 10);
                                data.AddOption("Choose~a~floor~to~challenge.", 15);

                                data.AddOption("I`ll~talk~to~you~later.", 255)
                                .AddAvatar(14)
                                .FinalizeDialog();
                            }
                        }
                        break;
                    }
                case 110:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Alright.", 255).AddAvatar(14)
                     .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << (byte)Math.Min(8, (int)client.Player.MyTowerOfMysteryLayerElite)) != 1u << (byte)Math.Min(8, (int)client.Player.MyTowerOfMysteryLayerElite))
                        {
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.JoinLayer(client, 60, 60, (byte)Math.Min(8, (int)client.Player.MyTowerOfMysteryLayerElite), stream);
                        }
                        else
                        {
                            switch (Math.Min(8, (int)client.Player.MyTowerOfMysteryLayerElite))
                            {
                                case 0:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 1`s guardian devil GreatPheasant(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 1:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 2`s guardian devil WingedLord(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 2:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 3`s guardian devil WaterTerror(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 3:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 4`s guardian devil RuthlessAsura(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 4:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 5`s guardian devil SoulStrangler(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 5:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 6`s guardian devil DarkGlutton(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 6:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 7`s guardian devil AlienDragon(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 7:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 8`s guardian devil ShadowSpider(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                default:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 9`s guardian devil FlameGiant(Elite).No need to sweep again.")
                             .AddOption("Okay.", 255).AddAvatar(14)
                      .FinalizeDialog();
                                        break;
                                    }
                            }
                        }

                        break;
                    }
                case 115:
                    {
                        data.AddText("Which floor of the tower would you like to challenge now?");
                        for (int x = 0; x <= Math.Min(8, (int)client.Player.MyTowerOfMysteryLayerElite); x++)
                            data.AddOption("Floor~" + (x + 1).ToString() + ".", (byte)(130 + x));


                        data.AddOption("Let~me~think~about~it.", 255)
                     .AddAvatar(14)
                     .FinalizeDialog();
                        break;
                    }
                case 1:
                    {
                        client.Player.TOMSelectChallengeToday = 1;
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Alright.", 255).AddAvatar(14)
                     .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << (byte)Math.Min(8, (int)client.Player.MyTowerOfMysteryLayer)) != 1u << (byte)Math.Min(8, (int)client.Player.MyTowerOfMysteryLayer))
                        {
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.JoinLayer(client, 60, 60, (byte)Math.Min(8, (int)client.Player.MyTowerOfMysteryLayer), stream);
                        }
                        break;
                    }
                case 2:
                    {
                        data.AddText("There is no limit on the number of times you can challenge the tower, but only 3 chances to claim the reward every day.")
                           .AddText("~Alternatively, you can use CPs or Meteor Scrolls to start `sweeping` and directly get the reward. If you create a team")
                           .AddText("~to challenge the tower at Elite mode, you can also claim a Concerted Effort Pack from the Cloud Sweeper after completing it.")
                           .AddOption("Start~the~challenge.~(Common)", 1)
                      .AddOption("I`ll~talk~to~you~later.", 255)
                      .AddAvatar(14)
                      .FinalizeDialog();
                        break;
                    }
                case 10:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Alright.", 255).AddAvatar(14)
                     .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << (byte)Math.Min(8, (int)client.Player.MyTowerOfMysteryLayer)) != 1u << (byte)Math.Min(8, (int)client.Player.MyTowerOfMysteryLayer))
                        {
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.JoinLayer(client, 60, 60, client.Player.MyTowerOfMysteryLayer, stream);

                        }
                        else
                        {
                            switch (Math.Min(8, (uint)client.Player.MyTowerOfMysteryLayer))
                            {
                                case 0:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 1`s guardian devil GreatPheasant(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 1:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 2`s guardian devil WingedLord(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 2:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 3`s guardian devil WaterTerror(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 3:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 4`s guardian devil RuthlessAsura(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 4:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 5`s guardian devil SoulStrangler(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 5:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 6`s guardian devil DarkGlutton(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 6:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 7`s guardian devil AlienDragon(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 7:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 8`s guardian devil ShadowSpider(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                default:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 9`s guardian devil FlameGiant(common).No need to sweep again.")
                             .AddOption("Okay.", 255).AddAvatar(14)
                      .FinalizeDialog();
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                case 15:
                    {
                        data.AddText("Which floor of the tower would you like to challenge now?");
                        for (int x = 0; x <= Math.Min(8, (int)client.Player.MyTowerOfMysteryLayer); x++)
                            data.AddOption("Floor~" + (x + 1).ToString() + ".", (byte)(30 + x));


                        data.AddOption("Let~me~think~about~it.", 255)
                     .AddAvatar(14)
                     .FinalizeDialog();
                        break;
                    }

                #region EliteCommon
                case 160:
                    {
                        if (client.Player.CanSweapTowerOfMystery)
                        {
                            data.AddText("                        Bright Tribe`s Reward         ")
                           .AddText("                     **************************************");
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>Refined</tip>");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment,#a#Training#Course#Pack>2-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>Unique</tip>");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Course#Packs>3-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>Elite</tip>");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#3#Training#Course#Packs>4-Stars</tip>");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>Super</tip>");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#5#Training#Course#Packs>5-Stars</tip>");
                                        break;
                                    }
                            }
                            data.AddText("                    **************************************");
                            data.AddText("                              Refreshing Times: " + client.Player.TOMRefreshReward.ToString() + "");

                            if (client.Player.TOM_Reward != MsgTournaments.MsgTowerOfMystery.RewardTypes.Super)
                            {
                                data.AddOption("Refresh~reward.~(20~CP)", 170);
                                data.AddOption("Enable~`Sweep`.~(860~CPs)", 176);//todo
                                data.AddOption("Refresh~reward.~(50~Study~Pts)", 172);
                                data.AddOption("`Sweep`.~(4~DragonBalls)", 177);//todo
                                if (client.Inventory.Contain(3008735, 1))
                                    data.AddOption("Refresh~reward.~(FierceSpirit)", 174);
                            }
                            else
                            {
                                data.AddOption("Enable~`Sweep`.~(860~CPs)", 176);
                                data.AddOption("`Sweep`.~(4~DragonBalls)", 177);
                            }

                            data.AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(14)
                            .FinalizeDialog();
                        }
                        break;
                    }
                case 177:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Alright.", 255).AddAvatar(14)
                     .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.HaveSpace(5))
                        {
                            data.AddText("Thanks for your challenge. However, you inventory is too full to contain the reward.Please clear at least 5 empty spaces, first.")
                               .AddOption("Okay.", 255).AddAvatar(14)
                                .FinalizeDialog();

                            break;
                        }
                        if (client.Inventory.Contain(Database.ItemType.DragonBall, 4) == false)
                        {
                            data.AddText("It seems you don`t have enough DragonBalls to directly clear this floor.Please check it again.")
                           .AddOption("Alright.", 255).AddAvatar(14)
                    .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.TOM_SellectOptionC) != 1u << client.Player.TOM_SellectOptionC)
                        {
                            client.Player.TowerOfMysterychallenge -= 1;
                            client.Player.TowerOfMysteryChallengeFlag |= 1u << client.Player.TOM_SellectOptionC;
                            client.Inventory.Remove(Database.ItemType.DragonBall, 4, stream);
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);

                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        client.Inventory.Add(stream, 3008963);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        client.Inventory.AddItemWitchStack(3007284, 0, 1, stream);
                                        client.Inventory.Add(stream, 3008964);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        client.Player.SubClass.AddStudyPoints(client, 40, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        client.Inventory.AddItemWitchStack(3007284, 0, 2, stream);
                                        client.Player.SubClass.AddStudyPoints(client, 80, stream);
                                        client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                        client.Inventory.Add(stream, 3008965);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        client.Inventory.AddItemWitchStack(3007284, 0, 3, stream);
                                        client.Player.SubClass.AddStudyPoints(client, 120, stream);
                                        client.Inventory.Add(stream, 3008966);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                        client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        client.Inventory.AddItemWitchStack(3007284, 0, 5, stream);
                                        client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                        client.Inventory.Add(stream, 3008967);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                            }
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            data.AddText("You've successfully cleared this floor, and you deserve this reward.Here you are.")
                               .AddOption("Thanks!", 255).AddAvatar(14)
                                .FinalizeDialog();
                        }
                        else
                        {
                            switch (client.Player.TOM_SellectOptionC)
                            {
                                case 0:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 1`s guardian devil GreatPheasant(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 1:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 2`s guardian devil WingedLord(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 2:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 3`s guardian devil WaterTerror(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 3:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 4`s guardian devil RuthlessAsura(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 4:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 5`s guardian devil SoulStrangler(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 5:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 6`s guardian devil DarkGlutton(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 6:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 7`s guardian devil AlienDragon(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 7:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 8`s guardian devil ShadowSpider(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                default:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 9`s guardian devil FlameGiant(Elite).No need to sweep again.")
                             .AddOption("Okay.", 255).AddAvatar(14)
                      .FinalizeDialog();
                                        break;
                                    }
                            }

                        }

                        break;
                    }
                case 176:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Alright.", 255).AddAvatar(14)
                     .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.HaveSpace(5))
                        {
                            data.AddText("Thanks for your challenge. However, you inventory is too full to contain the reward.Please clear at least 5 empty spaces, first.")
                               .AddOption("Okay.", 255).AddAvatar(14)
                                .FinalizeDialog();

                            break;
                        }
                        if (client.Player.ConquerPoints < 860)
                        {
                            data.AddText("It seems you don`t have enough ConquerPoints to `Sweep` the  reward. Please make sure you bring enough ConquerPoints, next time.")
                           .AddOption("Alright.", 255).AddAvatar(14)
                    .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.TOM_SellectOptionC) != 1u << client.Player.TOM_SellectOptionC)
                        {
                            if (client.InTrade) return;
                            client.Player.TowerOfMysterychallenge -= 1;
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            client.Player.TowerOfMysteryChallengeFlag |= 1u << client.Player.TOM_SellectOptionC;
                            client.Player.ConquerPoints -= 860;
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        client.Inventory.Add(stream, 3008963);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        client.Inventory.Add(stream, 3008964);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        client.Player.SubClass.AddStudyPoints(client, 40, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        client.Player.SubClass.AddStudyPoints(client, 80, stream);
                                        client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                        client.Inventory.Add(stream, 3008965);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        client.Player.SubClass.AddStudyPoints(client, 120, stream);
                                        client.Inventory.Add(stream, 3008966);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                        client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        client.Player.SubClass.AddStudyPoints(client, 150, stream);
                                        client.Inventory.Add(stream, 3008967);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                            }
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            data.AddText("You've successfully cleared this floor, and you deserve this reward.Here you are.")
                               .AddOption("Thanks!", 255).AddAvatar(14)
                                .FinalizeDialog();
                        }
                        else
                        {
                            switch (client.Player.TOM_SellectOptionC)
                            {
                                case 0:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 1`s guardian devil GreatPheasant(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 1:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 2`s guardian devil WingedLord(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 2:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 3`s guardian devil WaterTerror(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 3:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 4`s guardian devil RuthlessAsura(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 4:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 5`s guardian devil SoulStrangler(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 5:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 6`s guardian devil DarkGlutton(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 6:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 7`s guardian devil AlienDragon(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 7:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 8`s guardian devil ShadowSpider(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                default:
                                    {
                                        data.AddText("You've already challenged, at Elite mode, and defeated the Floor 9`s guardian devil FlameGiant(Elite).No need to sweep again.")
                             .AddOption("Okay.", 255).AddAvatar(14)
                      .FinalizeDialog();
                                        break;
                                    }
                            }

                        }

                        break;
                    }
                case 174:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 175)
                              .AddOption("I~haven`t~decided,~yet!", 255).AddAvatar(14)
                       .FinalizeDialog();

                            }
                            else
                                goto case 175;
                        }
                        break;
                    }
                case 175:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            client.Inventory.Remove(3008735, 1, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 160;
                        }
                        break;
                    }
                case 172:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 173)
                              .AddOption("I~haven`t~decided,~yet!", 255).AddAvatar(14)
                       .FinalizeDialog();

                            }
                            else
                                goto case 173;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255).AddAvatar(14)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 173:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            client.Player.SubClass.RemoveStudy(client, 50, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 160;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255).AddAvatar(14)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 170:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 171)
                              .AddOption("I~haven`t~decided,~yet!", 255).AddAvatar(14)
                       .FinalizeDialog();

                            }
                            else
                                goto case 171;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255).AddAvatar(14)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 171:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 20;
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 160;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255).AddAvatar(14)
                      .FinalizeDialog();
                        }
                        break;
                    }
                #endregion
                case 60:
                    {
                        if (client.Player.CanSweapTowerOfMystery)
                        {

                            data.AddText("                       Bright Tribe`s Reward\n")
                           .AddText("                    **************************************\n");
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>Normal</tip>\n");
                                        data.AddText(" 											<tip color=0xffffffff desc=Item#Reward:#a#Blazing#CP#Fragment>1-Star</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>Refined</tip>\n");
                                        data.AddText(" 											<tip color=0xff5d86c9 desc=Item#Reward:#a#Blazing#CP#Fragment>2-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>Unique</tip>\n");
                                        data.AddText(" 											<tip color=0xff00fa00 desc=Item#Reward:#a#Blazing#CP#Fragment,#50#Chi#Points>3-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>Elite</tip>\n");
                                        data.AddText(" 											<tip color=0xffc501e7 desc=Item#Reward:#a#Blazing#CP#Fragment,#2#Training#Talent#Packs>4-Stars</tip>\n");
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>Super</tip>\n");
                                        data.AddText(" 											<tip color=0xffffae00 desc=Item#Reward:#a#Blazing#CP#Fragment,#150#Chi#Points>5-Stars</tip>\n");
                                        break;
                                    }
                            }
                            data.AddText("                    **************************************\n");
                            data.AddText("                       Refreshing Times: " + client.Player.TOMRefreshReward.ToString() + "");

                            if (client.Player.TOM_Reward != MsgTournaments.MsgTowerOfMystery.RewardTypes.Super)
                            {
                                data.AddOption("Refresh~reward.~(20~CP)", 70);
                                data.AddOption("Enable~`Sweep`.~(430~CPs)", 76);//todo
                                data.AddOption("Refresh~reward.~(50~Study~Pts)", 72);
                                data.AddOption("`Sweep`.~(2~DragonBalls)", 77);//todo
                                if (client.Inventory.Contain(3008735, 1))
                                    data.AddOption("Refresh~reward.~(FierceSpirit)", 74);
                            }
                            else
                            {
                                data.AddOption("Enable~`Sweep`.~(430~CPs)", 76);
                                data.AddOption("`Sweep`.~(2~DragonBalls)", 77);
                            }

                            data.AddOption("I`ll~think~about~it.", 255)
                            .AddAvatar(14)
                            .FinalizeDialog();
                        }
                        break;
                    }
                case 77:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Alright.", 255).AddAvatar(14)
                     .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.HaveSpace(5))
                        {
                            data.AddText("Thanks for your challenge. However, you inventory is too full to contain the reward.Please clear at least 5 empty spaces, first.")
                               .AddOption("Okay.", 255).AddAvatar(14)
                                .FinalizeDialog();

                            break;
                        }
                        if (client.Inventory.Contain(Database.ItemType.DragonBall, 2) == false)
                        {
                            data.AddText("It seems you don`t have enough DragonBalls to directly clear this floor.Please check it again.")
                           .AddOption("Alright.", 255).AddAvatar(14)
                    .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.TOM_SellectOptionC) != 1u << client.Player.TOM_SellectOptionC)
                        {
                            client.Player.TowerOfMysterychallenge -= 1;
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            client.Player.TowerOfMysteryChallengeFlag |= 1u << client.Player.TOM_SellectOptionC;
                            client.Inventory.Remove(Database.ItemType.DragonBall, 2, stream);
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        client.Inventory.Add(stream, 3008956);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        client.Inventory.Add(stream, 3008957);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                        client.Inventory.Add(stream, 3008958);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        client.Inventory.Add(stream, 3008959);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                        client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        client.Inventory.Add(stream, 3008960);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                            }
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            data.AddText("You've successfully cleared this floor, and you deserve this reward.Here you are.")
                               .AddOption("Thanks!", 255).AddAvatar(14)
                                .FinalizeDialog();
                        }
                        else
                        {
                            switch (client.Player.TOM_SellectOptionC)
                            {
                                case 0:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 1`s guardian devil GreatPheasant(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 1:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 2`s guardian devil WingedLord(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 2:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 3`s guardian devil WaterTerror(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 3:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 4`s guardian devil RuthlessAsura(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 4:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 5`s guardian devil SoulStrangler(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 5:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 6`s guardian devil DarkGlutton(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 6:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 7`s guardian devil AlienDragon(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 7:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 8`s guardian devil ShadowSpider(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                default:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 9`s guardian devil FlameGiant(common).No need to sweep again.")
                             .AddOption("Okay.", 255).AddAvatar(14)
                      .FinalizeDialog();
                                        break;
                                    }
                            }

                        }

                        break;
                    }
                case 76:
                    {
                        if (client.Player.TowerOfMysterychallenge == 0)
                        {
                            data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                            .AddOption("Alright.", 255).AddAvatar(14)
                     .FinalizeDialog();
                            break;
                        }
                        if (!client.Inventory.HaveSpace(5))
                        {
                            data.AddText("Thanks for your challenge. However, you inventory is too full to contain the reward.Please clear at least 5 empty spaces, first.")
                               .AddOption("Okay.", 255).AddAvatar(14)
                                .FinalizeDialog();

                            break;
                        }
                        if (client.Player.ConquerPoints < 430)
                        {
                            data.AddText("It seems you don`t have enough ConquerPoints to `Sweep` the  reward. Please make sure you bring enough ConquerPoints, next time.")
                           .AddOption("Alright.", 255).AddAvatar(14)
                    .FinalizeDialog();
                            break;
                        }
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << client.Player.TOM_SellectOptionC) != 1u << client.Player.TOM_SellectOptionC)
                        {
                            if (client.InTrade) return;
                            client.Player.TowerOfMysterychallenge -= 1;
                            if (client.Player.QuestGUI.CheckQuest(ActiveQuest.MissionId, MsgQuestList.QuestListItem.QuestStatus.Accepted))
                                client.Player.QuestGUI.IncreaseQuestObjectives(stream, ActiveQuest.MissionId, 1);
                            client.Player.TowerOfMysteryChallengeFlag |= 1u << client.Player.TOM_SellectOptionC;
                            client.Player.ConquerPoints -= 430;
                            switch (client.Player.TOM_Reward)
                            {
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Normal:
                                    {
                                        client.Inventory.Add(stream, 3008956);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Refined:
                                    {
                                        client.Inventory.Add(stream, 3008957);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 1, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Unique:
                                    {
                                        client.Inventory.AddItemWitchStack(3006542, 0, 1, stream);
                                        client.Inventory.Add(stream, 3008958);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 2, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 50;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite:
                                    {
                                        client.Inventory.Add(stream, 3008959);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 3, stream);
                                        client.Inventory.AddItemWitchStack(3006542, 0, 2, stream);
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        break;
                                    }
                                case MsgTournaments.MsgTowerOfMystery.RewardTypes.Super:
                                    {
                                        client.Inventory.Add(stream, 3008960);
                                        client.Inventory.AddItemWitchStack(3002030, 0, 5, stream);
                                        client.Player.MyChi.ChiPoints = client.Player.MyChi.ChiPoints + 150;
                                        client.Inventory.AddItemWitchStack(3008727, 0, 1, stream);
                                        Game.MsgServer.MsgChiInfo.MsgHandleChi.SendInfo(client, Game.MsgServer.MsgChiInfo.Action.Upgrade);
                                        break;
                                    }
                            }
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            data.AddText("You've successfully cleared this floor, and you deserve this reward.Here you are.")
                               .AddOption("Thanks!", 255).AddAvatar(14)
                                .FinalizeDialog();
                        }
                        else
                        {
                            switch (client.Player.TOM_SellectOptionC)
                            {
                                case 0:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 1`s guardian devil GreatPheasant(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 1:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 2`s guardian devil WingedLord(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 2:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 3`s guardian devil WaterTerror(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 3:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 4`s guardian devil RuthlessAsura(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 4:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 5`s guardian devil SoulStrangler(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 5:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 6`s guardian devil DarkGlutton(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 6:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 7`s guardian devil AlienDragon(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 7:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 8`s guardian devil ShadowSpider(common).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                default:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 9`s guardian devil FlameGiant(common).No need to sweep again.")
                             .AddOption("Okay.", 255).AddAvatar(14)
                      .FinalizeDialog();
                                        break;
                                    }
                            }

                        }

                        break;
                    }
                case 74:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 75)
                              .AddOption("I~haven`t~decided,~yet!", 255).AddAvatar(14)
                       .FinalizeDialog();

                            }
                            else
                                goto case 75;
                        }
                        break;
                    }
                case 75:
                    {
                        if (client.Inventory.Contain(3008735, 1))
                        {
                            client.Inventory.Remove(3008735, 1, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 60;
                        }
                        break;
                    }
                case 72:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 73)
                              .AddOption("I~haven`t~decided,~yet!", 255).AddAvatar(14)
                       .FinalizeDialog();

                            }
                            else
                                goto case 73;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255).AddAvatar(14)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 73:
                    {
                        if (client.Player.SubClass.StudyPoints >= 50)
                        {
                            client.Player.SubClass.RemoveStudy(client, 50, stream);
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 60;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough Study Points to refresh the reward level. Come and see me when you have the thing ready.")
                             .AddOption("Alright.", 255).AddAvatar(14)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 70:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.Player.TOM_Reward == MsgTournaments.MsgTowerOfMystery.RewardTypes.Elite)
                            {
                                data.AddText("The Tower of Mystery provides 5 levels of rewards from 1-star to 5-star. Currently, you can claim a 4-star reward. Are you sure you want to refresh the level of reward?")
                              .AddOption("Yes.", 71)
                              .AddOption("I~haven`t~decided,~yet!", 255).AddAvatar(14)
                       .FinalizeDialog();

                            }
                            else
                                goto case 71;
                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255).AddAvatar(14)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 71:
                    {
                        if (client.Player.ConquerPoints >= 20)
                        {
                            if (client.InTrade) return;
                            client.Player.ConquerPoints -= 20;
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.GenerateReward(client);
                            goto case 60;

                        }
                        else
                        {
                            data.AddText("It seems you don`t have enough CPs to refresh the level of reward. Please make sure you bring enough money, next time.")
                             .AddOption("Alright.", 255).AddAvatar(14)
                      .FinalizeDialog();
                        }
                        break;
                    }
                case 150:
                case 151:
                case 152:
                case 153:
                case 154:
                case 155:
                case 156:
                case 157:
                case 158:
                    {
                        if ((client.Player.TowerOfMysteryChallengeFlag & 1u << (byte)(Option - 150)) != 1u << (byte)(Option - 150))
                        {
                            if (client.Player.TowerOfMysterychallenge == 0)
                            {
                                data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                    .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                                .AddOption("Alright.", 255).AddAvatar(14)
                         .FinalizeDialog();
                                break;
                            }
                            Game.MsgTournaments.MsgSchedules.TowerOfMystery.JoinLayer(client, 60, 60, (byte)(Option - 150), stream);
                        }
                        else
                        {
                            switch ((byte)(Option - 150))
                            {
                                case 0:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 1`s guardian devil GreatPheasant(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 1:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 2`s guardian devil WingedLord(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 2:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 3`s guardian devil WaterTerror(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 3:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 4`s guardian devil RuthlessAsura(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 4:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 5`s guardian devil SoulStrangler(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 5:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 6`s guardian devil DarkGlutton(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 6:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 7`s guardian devil AlienDragon(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                case 7:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 8`s guardian devil ShadowSpider(Elite).No need to sweep again.")
                              .AddOption("Okay.", 255).AddAvatar(14)
                       .FinalizeDialog();
                                        break;
                                    }
                                default:
                                    {
                                        data.AddText("You've already challenged, at Common mode, and defeated the Floor 9`s guardian devil FlameGiant(Elite).No need to sweep again.")
                             .AddOption("Okay.", 255).AddAvatar(14)
                      .FinalizeDialog();
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                default:
                    {
                        if (Option >= 130 && Option < 140)
                        {

                            data.AddText("Okay, you`ve selected a floor. If you want to challenge this floor in person,")
                           .AddText("~I`ll send you there. While if you want to enable `Sweep` function,")
                           .AddText("~you can directly get the reward without challenging it in person. So, what do you say?")
                           .AddOption("Challenge~it~in~person.", (byte)(Option - 130 + 50));

                            client.Player.TOM_SellectOptionC = (byte)(Option - 130);
                            if (client.Player.MyTowerOfMysteryLayer != Option - 130)
                            {
                                client.Player.CanSweapTowerOfMystery = true;
                                data.AddOption("Enable~`Sweep`.", 160);
                            }
                            else
                                client.Player.CanSweapTowerOfMystery = false;
                            data.AddOption("I`ll~talk~to~you~later.", 255)
                            .AddAvatar(14)
                            .FinalizeDialog();
                        }
                        else if (Option >= 30 && Option < 40)
                        {

                            data.AddText("Okay, you`ve selected a floor. If you want to challenge this floor in person,")
                           .AddText("~I`ll send you there. While if you want to enable `Sweep` function,")
                           .AddText("~you can directly get the reward without challenging it in person. So, what do you say?")
                           .AddOption("Challenge~it~in~person.", (byte)(Option - 30 + 50));

                            client.Player.TOM_SellectOptionC = (byte)(Option - 30);
                            if (client.Player.MyTowerOfMysteryLayer != Option - 30)
                            {
                                client.Player.CanSweapTowerOfMystery = true;
                                data.AddOption("Enable~`Sweep`.", 60);
                            }
                            else
                                client.Player.CanSweapTowerOfMystery = false;
                            data.AddOption("I`ll~talk~to~you~later.", 255)
                            .AddAvatar(14)
                            .FinalizeDialog();
                        }
                        else if (client.Player.MyTowerOfMysteryLayer >= Option - 50)
                        {
                            if (client.Player.TowerOfMysterychallenge == 0)
                            {
                                data.AddText("You`ve already challenged the Tower of Mystery 3 times today, and you can`t")
                                    .AddText("~use CPs or Meteor Scrolls to get the reward. I suggest you retry tomorrow.")
                                .AddOption("Alright.", 255).AddAvatar(14)
                         .FinalizeDialog();
                                break;
                            }
                            if ((client.Player.TowerOfMysteryChallengeFlag & 1u << (byte)(Option - 50)) != 1u << (byte)(Option - 50))
                            {
                                Game.MsgTournaments.MsgSchedules.TowerOfMystery.JoinLayer(client, 60, 60, (byte)(Option - 50), stream);
                            }
                            else
                            {
                                switch ((byte)(Option - 50))
                                {
                                    case 0:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 1`s guardian devil GreatPheasant(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(14)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 1:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 2`s guardian devil WingedLord(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(14)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 2:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 3`s guardian devil WaterTerror(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(14)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 3:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 4`s guardian devil RuthlessAsura(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(14)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 4:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 5`s guardian devil SoulStrangler(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(14)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 5:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 6`s guardian devil DarkGlutton(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(14)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 6:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 7`s guardian devil AlienDragon(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(14)
                           .FinalizeDialog();
                                            break;
                                        }
                                    case 7:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 8`s guardian devil ShadowSpider(common).No need to sweep again.")
                                  .AddOption("Okay.", 255).AddAvatar(14)
                           .FinalizeDialog();
                                            break;
                                        }
                                    default:
                                        {
                                            data.AddText("You've already challenged, at Common mode, and defeated the Floor 9`s guardian devil FlameGiant(common).No need to sweep again.")
                                 .AddOption("Okay.", 255).AddAvatar(14)
                          .FinalizeDialog();
                                            break;
                                        }
                                }
                            }

                        }
                        break;
                    }

            }
        }
    }
}

