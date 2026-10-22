
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.Database
{
    public static class InnerPowerTable
    {


        public enum AtributeType : byte
        {
            MaxHP = 1,
            PAttack = 2,
            MAttack = 3,
            PDefense = 4,
            MDefense = 5,
            FinalPAttack = 6,
            FinalMAttack = 7,
            FinalPDamage = 8,
            FinalMDamage = 9,
            PStrike = 10,
            MStrike = 11,
            Immunity = 12,
            Break = 13,
            Conteraction = 14
        }


        public static uint Count = 0;
        public static Stage[] Stages;

        public class Stage
        {
            public ushort ID;
            public string Name = "";
            public byte NeiGongNum;
            public AtributeType[] SpecialAtributesType;
            public uint[] AtributesValues;
            public NeiGong[] NeiGongAtributes;

            public class NeiGong
            {
                public byte ID;
                public byte MaxLevel;
                public AtributeType[] AtributesType;
                public uint[] AtributesValues;
                public uint[] ProgressNeiGongValue;
                public uint ItemID;
                public uint ReqLev;

                public bool CheckAccount(uint reborn, uint level)
                {
                    byte r_reborn = (byte)(ReqLev / 1000);
                    byte r_level = (byte)(ReqLev % 1000);
                    return reborn >= r_reborn && level >= r_level;
                }
            }
        }

        public static bool GetDBInfo(uint ID, out Stage stage, out Stage.NeiGong gong)
        {
            foreach (var m_stage in Stages)
            {
                foreach (var m_gong in m_stage.NeiGongAtributes)
                {
                    if (m_gong.ID == ID)
                    {
                        stage = m_stage;
                        gong = m_gong;
                        return true;
                    }
                }
            }
            stage = null;
            gong = null;
            return false;
        }


        public static void LoadDBInformation()
        {
            WindowsAPI.IniFile Reader = new WindowsAPI.IniFile("NeiGongInfo.ini");
            Count = Reader.ReadUInt32("NeiGong", "Num", 0);
            Stages = new Stage[Count];

            for (int x = 1; x <= Count; x++)
            {
                Stage stage = new Stage();
                stage.ID = Reader.ReadUInt16(x.ToString(), "id", 0);
                stage.Name = Reader.ReadString(x.ToString(), "Name", "");
                stage.NeiGongNum = Reader.ReadByte(x.ToString(), "NeiGongNum", 0);
                //----- special atributes type-------------------------
                string aAttriType = Reader.ReadString(x.ToString(), "AttriType", "");
                string[] atr_t = aAttriType.Split('-');
                stage.SpecialAtributesType = new AtributeType[stage.NeiGongNum];
                for (int y = 0; y < atr_t.Length; y++)
                    stage.SpecialAtributesType[y] = (AtributeType)byte.Parse(atr_t[y]);
                //--------------------------------------

                string atributesvalue = Reader.ReadString(x.ToString(), "AttriValue", "");
                stage.AtributesValues = new uint[stage.NeiGongNum];
                string[] a_atrs = atributesvalue.Split('-');
                for (int y = 0; y < a_atrs.Length; y++)
                    stage.AtributesValues[y] = uint.Parse(a_atrs[y]);

                stage.NeiGongAtributes = new Stage.NeiGong[stage.NeiGongNum];

                for (int i = 1; i <= stage.NeiGongNum; i++)
                {
                    string key = x.ToString() + "-" + i.ToString();
                    Stage.NeiGong gong = new Stage.NeiGong();
                    gong.ID = Reader.ReadByte(key, "Type", 0);
                    gong.MaxLevel = Reader.ReadByte(key, "MaxLev", 0);

                    string g_atributetype = Reader.ReadString(key, "AttriType", "");

                    string[] gg_atributes = g_atributetype.Split('-');
                    gong.AtributesType = new AtributeType[gg_atributes.Length];
                    for (int y = 0; y < gg_atributes.Length; y++)
                        gong.AtributesType[y] = (AtributeType)byte.Parse(gg_atributes[y]);

                    string AttriValue = Reader.ReadString(key, "AttriValue", "");

                    string[] g_AttriValue = AttriValue.Split('-');
                    gong.AtributesValues = new uint[g_AttriValue.Length];
                    for (int y = 0; y < g_AttriValue.Length; y++)
                        gong.AtributesValues[y] = uint.Parse(g_AttriValue[y]);

                    string NeiGongValue = Reader.ReadString(key, "NeiGongValue", "");

                    string[] g_NeiGongValue = NeiGongValue.Split('-');
                    gong.ProgressNeiGongValue = new uint[g_NeiGongValue.Length];
                    for (int y = 0; y < g_NeiGongValue.Length; y++)
                        gong.ProgressNeiGongValue[y] = uint.Parse(g_NeiGongValue[y]);

                    gong.ReqLev = Reader.ReadUInt32(key, "ReqLev", 0);
                    gong.ItemID = Reader.ReadUInt32(key, "ReqItemType", 0);


                    stage.NeiGongAtributes[i - 1] = gong;
                }
                Stages[x - 1] = stage;
            }
        }

        public static void Save()
        {



            using (Database.DBActions.Write _wr = new Database.DBActions.Write("InnerPower.txt"))
            {
                var dictionary = Role.Instance.InnerPower.InnerPowerPolle.Values;
                foreach (var inner in dictionary)
                {
                    if (inner.Name != null && !inner.Name.Contains("[Bot]") && !inner.Name.Contains("[Shop]") && inner.UID < 3999900001)
                        _wr.Add(inner.ToString());
                }
                _wr.Execute(DBActions.Mode.Open);
            }
        }
        public static void Load()
        {

            using (Database.DBActions.Read r = new Database.DBActions.Read("InnerPower.txt"))
            {
                if (r.Reader())
                {
                    int count = r.Count;
                    for (uint x = 0; x < count; x++)
                    {
                        Database.DBActions.ReadLine reader = new DBActions.ReadLine(r.ReadString(""), '/');
                        Role.Instance.InnerPower item = new Role.Instance.InnerPower(reader.Read(""), reader.Read((uint)0));
                        item.Potency = reader.Read((int)0);
                        int Stages = reader.Read((int)0);
                        for (int i = 0; i < Stages; i++)
                        {
                            var Stage = item.Stages[i];
                            Stage.ID = reader.Read((ushort)0);
                            Stage.UnLocked = reader.Read((byte)0) == 1;
                            int count_neigongs = reader.Read((int)0);
                            for (int y = 0; y < count_neigongs; y++)
                            {
                                var neigon = Stage.NeiGongs[y];
                                neigon.ID = reader.Read((byte)0);
                                neigon.Score = reader.Read((byte)0);
                                neigon.Unlocked = reader.Read((byte)0) == 1;
                                neigon.level = reader.Read((byte)0);
                                neigon.Complete = reader.Read((byte)0) == 1;
                            }
                        }
                        Role.Instance.InnerPower.InnerPowerPolle.TryAdd(item.UID, item);
                        Role.Instance.InnerPower.InnerPowerRank.UpdateRank(item);
                    }
                }
            }
        }

    }
}
