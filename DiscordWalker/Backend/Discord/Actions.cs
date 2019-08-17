using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DiscordWalker.Backend.Discord
{
    public static class Actions
    {
        public static string JoinServer(string InviteCode)
        {
            JToken Res = NetworkInterface.Request("invite/" + InviteCode, WToken:true);
            if (Res != null) { return Res["guild"]["id"].ToString(); }
            return null;
        }

        public static void LeaveServer(string GuildID)
        {
            NetworkInterface.Request("users/@me/guilds/" + GuildID, WToken: true,Method:"DELETE");
        }

        public static bool ServerExists(string InviteCode)
        {
            JToken Res = NetworkInterface.Request("invite/" + InviteCode + "?with_counts=true", Method: "GET");
            return Res != null;
        }

        public static string SendMessage(string ChannelID,string Message)
        {
            JToken Res = NetworkInterface.Request("channels/" + ChannelID + "/messages", "{\"content\":\"" + Message + "\",\"nonce\":\""+Master.Rnd.Next(0,int.MaxValue)+"\",\"tts\":false}",true);
            if (Res != null) { return Res["id"].ToString(); }
            return null;
        }
    }
}
