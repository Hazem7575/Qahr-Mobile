using Qahr.Mobile.Client;
using Qahr.Mobile.Database;
using Qahr.Mobile.Database.DBActions;
using Qahr.Mobile.Game.MsgServer;
using Qahr.Mobile.Game.MsgServer.AttackHandler;
using Qahr.Mobile.Game.MsgServer.AttackHandler.Calculate;
using Qahr.Mobile.ServerSockets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Role.Instance
{
    public class MyTableBetDic
    {

        public GameClient user;
        public MyTableBetDic(GameClient client)
        {
            user = client;
        }
        public uint JiangHuFameRanking
        {
            get
            {
                uint Score = 0;
                Score = user.Player.TableBetDice;

                return Score;
            }
        }
        public uint JiangHuFameRankingPrevious
        {
            get
            {
                uint Score = 0;
                Score = user.Player.TableBetDice;

                return Score;
            }
        }
        public uint JiangHuFameRankingTotal
        {
            get
            {
                uint Score = 0;
                Score = user.Player.TableBetDice;

                return Score;
            }
        }


        public void UpdateRank()
        {
            #region ViodtagonClub

            if (JiangHuFameRanking >= 5000)
            {
                var Ranks = new TableBetDiceRank.Entry()
                {
                    Type = TableBetDiceRank.Type.JiangHuFameRanking,
                    TotalPoints = this.JiangHuFameRanking,
                    UID = user.Player.UID,
                    Name = user.Player.Name,
                    Level = (byte)user.Player.Level,
                    Class = user.Player.Class,
                    Mesh = user.Player.Mesh
                };
                Ranks.AddInfo(user);
                TableBetDiceRank.Ranks[TableBetDiceRank.Type.JiangHuFameRanking].UpdateItem(Ranks);
            }
            if (JiangHuFameRankingPrevious >= 5000)
            {
                var Ranks = new TableBetDiceRank.Entry()
                {
                    Type = TableBetDiceRank.Type.JiangHuFameRankingPrevious,
                    TotalPoints = this.JiangHuFameRankingPrevious,
                    UID = user.Player.UID,
                    Name = user.Player.Name,
                    Level = (byte)user.Player.Level,
                    Class = user.Player.Class,
                    Mesh = user.Player.Mesh
                };
                Ranks.AddInfo(user);
                TableBetDiceRank.Ranks[TableBetDiceRank.Type.JiangHuFameRankingPrevious].UpdateItem(Ranks);
            }
            if (JiangHuFameRankingTotal >= 5000)
            {
                var Ranks = new TableBetDiceRank.Entry()
                {
                    Type = TableBetDiceRank.Type.JiangHuFameRankingTotal,
                    TotalPoints = this.JiangHuFameRankingTotal,
                    UID = user.Player.UID,
                    Name = user.Player.Name,
                    Level = (byte)user.Player.Level,
                    Class = user.Player.Class,
                    Mesh = user.Player.Mesh
                };
                Ranks.AddInfo(user);
                TableBetDiceRank.Ranks[TableBetDiceRank.Type.JiangHuFameRankingTotal].UpdateItem(Ranks);
            }
            #endregion
        }

    }
}
