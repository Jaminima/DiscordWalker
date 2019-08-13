using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DiscordWalker.Backend.Discord
{
    public static class Fetch
    {
        public static List<string> TextChannels(string GuildID)
        {
            List<string> Rooms = new List<string> { };
            JToken Res = NetworkInterface.Request("guilds/" + GuildID + "/channels", WToken: true, Method: "GET");
            if (Res != null)
            {
                foreach (JToken Channel in Res.Where(x => x["type"].ToString() == "0")) { Rooms.Add(Channel["id"].ToString()); }
            }
            return Rooms;
        }

        public static string ServerID(string InviteCode)
        {
            JToken Res = NetworkInterface.Request("invite/" + InviteCode + "?with_counts=true", Method: "GET");
            if (Res != null)
            {
                return Res["guild"]["id"].ToString();
            }
            return null;
        }

        public static List<JToken> Messages(string ChannelID,int Limit=10)
        {
            List<JToken> MessageList = new List<JToken> { };
            JToken Res = NetworkInterface.Request("channels/" + ChannelID + "/messages?limit=" + Limit, WToken: true, Method: "GET");
            if (Res != null)
            {
                foreach (JToken Message in Res) { MessageList.Add(Message); }
            }
            return MessageList;
        }
    }
}
