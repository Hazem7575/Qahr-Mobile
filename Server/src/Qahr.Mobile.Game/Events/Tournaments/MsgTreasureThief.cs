using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Game.MsgTournaments
{
    public class MsgTreasureThief : ITournament
    {
        public const ushort MapID = 3030;

        // عدد الصناديق في الخريطة = الضعف
        public const int MaxBoxes = 30;

        public ProcesType Process { get; set; }

        public int CurrentBoxes = 0;

        public DateTime StartTimer = new DateTime();
        public DateTime BoxesStamp = new DateTime();

        Role.GameMap _map;

        public Role.GameMap Map
        {
            get
            {
                if (_map == null)
                    _map = Pool.ServerMaps[MapID];

                return _map;
            }
        }

        public TournamentType Type { get; set; }

        public MsgTreasureThief(TournamentType _type)
        {
            Type = _type;
            Process = ProcesType.Dead;
        }

        public bool InTournament(Client.GameClient user)
        {
            return user.Player.Map == MapID;
        }

        public void Open()
        {
            if (Process != ProcesType.Alive)
            {
                // Reset boxes
                CurrentBoxes = 0;

                // Create 30 boxes
                Create();

                // Reset player scores
                foreach (var user in Pool.GamePoll.Values)
                    user.Player.CurrentTreasureBoxes = 0;

                // ==========================================
                // Start event timer from the exact start time
                // ==========================================
                StartTimer = DateTime.Now;

                // Event is Alive
                Process = ProcesType.Alive;

                // Respawn/check boxes every 30 seconds
                BoxesStamp = DateTime.Now.AddSeconds(30);

                // Optional invitation
                // MsgSchedules.SendInvitation(
                //     "Treasure Thief",
                //     "",
                //     325,
                //     442,
                //     1002,
                //     0,
                //     60);

                MsgSchedules.SendSysMesage(
                    "Treasure Thief Event has now begun!",
                    MsgServer.MsgMessage.ChatMode.Center,
                    MsgServer.MsgMessage.MsgColor.red);
            }
        }


        public void Finish()
        {
            if (Process != ProcesType.Alive)
                return;

#if Arabic
    MsgSchedules.SendSysMesage(
        "Treasure Thief Event has Ended.",
        MsgServer.MsgMessage.ChatMode.Center,
        MsgServer.MsgMessage.MsgColor.red);
#else
            MsgSchedules.SendSysMesage(
                "Treasure Thief has Finished.",
                MsgServer.MsgMessage.ChatMode.Center,
                MsgServer.MsgMessage.MsgColor.red);
#endif

            // ==========================================
            // Stop event
            // ==========================================
            Process = ProcesType.Dead;

            // ==========================================
            // Get all players currently inside Map 3030
            // ==========================================
            var players = Map.Values.ToArray();

            foreach (var user in players)
            {
                if (user != null && user.Player.Map == MapID)
                {
                    user.Teleport(Places.TwinCity);
                }
            }

            // ==========================================
            // Reset event data
            // ==========================================
            CurrentBoxes = 0;
            BoxesStamp = DateTime.MinValue;
        }

        public bool Join(Client.GameClient user, ServerSockets.Packet stream)
        {
            if (Process == ProcesType.Alive)
            {
                ushort x = 0;
                ushort y = 0;

                Map.GetRandCoord(ref x, ref y);

                user.Teleport(x, y, MapID);

                return true;
            }

            return false;
        }

        private void Create()
        {
            GenerateBoxes();
        }

        private void GenerateBoxes()
        {
            // لو عندنا بالفعل 30 صندوق لا نضيف أي شيء
            if (CurrentBoxes >= MaxBoxes)
                return;

            // إضافة الصناديق الناقصة فقط حتى نصل إلى 30
            for (int i = CurrentBoxes; i < MaxBoxes; i++)
            {
                byte rand = (byte)Pool.GetRandom.Next(0, 5);

                ushort x = 0;
                ushort y = 0;

                Map.GetRandCoord(ref x, ref y);

                Game.MsgNpc.Npc np = Game.MsgNpc.Npc.Create();

                // Generate unique UID
                while (true)
                {
                    np.UID = (uint)Pool.GetRandom.Next(10000, 100000);

                    if (Map.View.Contain(np.UID, x, y) == false)
                        break;
                }

                np.NpcType = Role.Flags.NpcType.Talker;

                switch (rand)
                {
                    case 0:
                        np.Mesh = 26586;
                        break;

                    case 1:
                        np.Mesh = 26586;
                        break;

                    case 2:
                        np.Mesh = 26586;
                        break;

                    case 3:
                        np.Mesh = 26586;
                        break;

                    case 4:
                        np.Mesh = 26586;
                        break;

                    default:
                        np.Mesh = 26586;
                        break;
                }

                np.Map = MapID;
                np.X = x;
                np.Y = y;

                Map.AddNpc(np);
            }

            CurrentBoxes = MaxBoxes;
        }

        public void CheckUp()
        {
            // Event is not running
            if (Process != ProcesType.Alive)
                return;

            DateTime now = DateTime.Now;

            // ==========================================
            // Event duration = 5 minutes
            // ==========================================
            if (now >= StartTimer.AddMinutes(5))
            {
                Finish();
                return;
            }

            // ==========================================
            // Respawn boxes every 30 seconds
            // ==========================================
            if (now >= BoxesStamp)
            {
                GenerateBoxes();

                BoxesStamp = now.AddSeconds(30);
            }
        }

        public void Reward(
            Client.GameClient user,
            Game.MsgNpc.Npc npc,
            ServerSockets.Packet stream)
        {
            // إنقاص عدد الصناديق
            if (CurrentBoxes > 0)
                CurrentBoxes--;

            byte rand = (byte)Pool.GetRandom.Next(1, 40);
            switch (rand)
            {

                case 1:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.ConquerPoints += 100;

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got 100 CPs while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 2:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.ConquerPoints += 150;

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got 150 CPs while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 3:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.ConquerPoints += 100;

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got 100 CPs while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 4:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.ConquerPoints += 200;

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got 200 CPs while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 5:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.ConquerPoints += 150;

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got 150 CPs while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 6:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.Money += 2500000;

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got 2.500.000 Money while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 7:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.Money += 5000000;

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got 5.000.000 Money while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 8:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.Money += 7500000;

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got 7.500.000 Money while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 9:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.Money += 10000000;

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got 10.000.000 Money while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 10:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.Money += 5000000;

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got 5.000.000 Money while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 11:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Inventory.AddItemWitchStack(730004, 0, 1, stream, false);

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got Stone +4 while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 12:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Inventory.AddItemWitchStack(730005, 0, 1, stream, false);

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got Stone +5 while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 13:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Inventory.AddItemWitchStack(730004, 0, 1, stream, false);

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got Stone +4 while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 14:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Inventory.AddItemWitchStack(730003, 0, 1, stream, false);

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got Stone +3 while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 15:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Inventory.AddItemWitchStack(3300055, 0, 1, stream, false);

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got HolyPhoenixCupFragment while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 16:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Inventory.AddItemWitchStack(723694, 0, 1, stream, false);

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got PermanentStone while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 17:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Inventory.AddItemWitchStack(3005598, 0, 1, stream, false);

                        MsgSchedules.SendSysMesage(
                           "[ " + user.Player.Name +
                            " ] Got LuckyGarmentPack while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 18:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Inventory.AddItemWitchStack(728722, 0, 1, stream, false);

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got Godzilla [Box] while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 19:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Inventory.AddItemWitchStack(728722, 0, 1, stream, false);

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got Godzilla [Box] while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 20:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Inventory.AddItemWitchStack(729023, 0, 1, stream, false);

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got +3StonePack while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 21:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Inventory.AddItemWitchStack(1200005, 0, 1, stream, false);

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got ToughDrill while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 22:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Inventory.AddItemWitchStack(1088000, 0, 1, stream, false);

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got DragonBall while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 23:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Inventory.AddItemWitchStack(1088000, 0, 1, stream, false);

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got DragonBall while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 24:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.ConquerPoints += 100;

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got 100 CPs while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 25:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.ConquerPoints += 150;

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got 150 CPs while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 26:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.ConquerPoints += 200;

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got 200 CPs while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 27:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.ConquerPoints += 250;

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got 250 CPs while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 28:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.ConquerPoints += 300;

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got 300 CPs while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 29:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.ConquerPoints += 100;

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got 100 CPs while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 30:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.ConquerPoints += 200;

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got 200 CPs while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 31:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Inventory.AddSteed(stream, 300000, 1, 4, false, 255, 0, 150);

                        MsgSchedules.SendSysMesage(
                           "[ " + user.Player.Name +
                            " ] Got Steed +4 while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 32:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Inventory.AddSteed(stream, 300000, 1, 5, false, 255, 0, 150);

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got Steed +5 while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 33:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.ConquerPoints += 50;

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got 50 CPs while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 34:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.ConquerPoints += 100;

                        MsgSchedules.SendSysMesage(
                           "[ " + user.Player.Name +
                            " ] Got 100 CPs while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 35:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Inventory.AddItemWitchStack(722136, 0, 10, stream, false);

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got x10 ExpBall(Event) while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 36:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Inventory.AddItemWitchStack(722136, 0, 20, stream, false);

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got x20 ExpBall(Event) while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 37:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Inventory.AddItemWitchStack(722136, 0, 30, stream, false);

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got x30 ExpBall(Event) while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 38:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.GodzillaPoints += 5;

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got 5 [Godzilla Points] while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }

                case 39:
                    {
                        if (!user.Inventory.HaveSpace(1))
                        {
                            user.CreateBoxDialog("Please make 1 more space in your inventory.");
                            return;
                        }

                        user.Player.GodzillaPoints += 10;

                        MsgSchedules.SendSysMesage(
                            "[ " + user.Player.Name +
                            " ] Got 10 [Godzilla Points] while opening the TreasureBox!",
                            MsgServer.MsgMessage.ChatMode.World,
                            MsgServer.MsgMessage.MsgColor.yellow);
                        break;
                    }
            }

            // Player score
            user.Player.CurrentTreasureBoxes += 1;

            // Effect
            user.Player.SendString(
                stream,
                MsgServer.MsgStringPacket.StringID.Effect,
                true,
                "accession1");

            // Remove opened box
            Map.RemoveNpc(npc, stream);

            // Update ranking
            ShuffleGuildScores(stream);
        }

        public void ShuffleGuildScores(ServerSockets.Packet stream)
        {
            foreach (var user in Map.Values)
            {
#if Arabic
                Game.MsgServer.MsgMessage msg =
                    new Game.MsgServer.MsgMessage(
                        "---Your Score: " +
                        user.Player.CurrentTreasureBoxes +
                        "---",
                        MsgServer.MsgMessage.MsgColor.yellow,
                        MsgServer.MsgMessage.ChatMode.FirstRightCorner);
#else
                Game.MsgServer.MsgMessage msg =
                    new Game.MsgServer.MsgMessage(
                        "Your Score: ( " +
                        user.Player.CurrentTreasureBoxes +
                        " )",
                        MsgServer.MsgMessage.MsgColor.yellow,
                        MsgServer.MsgMessage.ChatMode.FirstRightCorner);
#endif

                user.Send(msg.GetArray(stream));
            }

            var array = Map.Values
                .OrderByDescending(
                    p => p.Player.CurrentTreasureBoxes)
                .ToArray();

            for (int x = 0;
                 x < Math.Min(10, array.Length);
                 x++)
            {
                var element = array[x];

#if Arabic
                Game.MsgServer.MsgMessage msg =
                    new Game.MsgServer.MsgMessage(
                        "No " +
                        (x + 1).ToString() +
                        "- " +
                        element.Player.Name +
                        " Opened " +
                        element.Player.CurrentTreasureBoxes.ToString() +
                        " Boxes!",
                        MsgServer.MsgMessage.MsgColor.yellow,
                        MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
#else
                Game.MsgServer.MsgMessage msg =
                    new Game.MsgServer.MsgMessage(
                        "No " +
                        (x + 1).ToString() +
                        "- " +
                        element.Player.Name +
                        " Opened " +
                        element.Player.CurrentTreasureBoxes.ToString() +
                        " Boxes!",
                        MsgServer.MsgMessage.MsgColor.yellow,
                        MsgServer.MsgMessage.ChatMode.ContinueRightCorner);
#endif

                Send(msg.GetArray(stream));
            }
        }

        public void Send(ServerSockets.Packet stream)
        {
            foreach (var user in Map.Values)
                user.Send(stream);
        }
    }
}