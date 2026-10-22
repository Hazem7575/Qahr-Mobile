using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.MsgInterServer.Instance
{
    public class Guilds
    {
        public static System.Counter CounterUID = new System.Counter(1);

        public static void AddToGuild(ServerSockets.Packet stream, Client.GameClient user, uint UID, Role.Flags.GuildMemberRank rank, string guildname, string LeaderName)
        {
            Role.Instance.Guild guild;
            if (Role.Instance.Guild.GuildPoll.TryGetValue(UID, out guild))
            {
                guild.CanSave = false;
                guild.AddPlayer(user.Player, stream);
            }
            else
            {
                guild = new Role.Instance.Guild(null, guildname, stream);
                guild.CanSave = false;
                guild.Info.LeaderName = LeaderName;
                guild.Info.GuildID = UID;
                guild.AddPlayer(user.Player, stream);

                Role.Instance.Guild.GuildPoll.TryAdd(UID, guild);
            }

        }
    }
}
