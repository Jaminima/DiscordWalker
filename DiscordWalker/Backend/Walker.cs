using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DiscordWalker.Backend.Discord;
using Newtonsoft.Json.Linq;

namespace DiscordWalker.Backend
{
    public static class Walker
    {
        public static List<String> StartWalking(string StartInviteCode, int MaxDiscordsWalked=10, int JoinDelay=5)
        {
            List<String> WalkedGuilds = new List<string> { },
                WalkedCodes = new List<string> { },
                UnWalkedCodes = WalkDiscord(StartInviteCode, JoinDelay, ref WalkedGuilds);
            for (int i = 0; i < MaxDiscordsWalked && i < UnWalkedCodes.Count; i++)
            {
                List<String> NewCodes = WalkDiscord(UnWalkedCodes.First(), JoinDelay, ref WalkedGuilds);
                if (NewCodes != null)
                {
                    WalkedCodes.Add(UnWalkedCodes.First());
                    NewCodes.ForEach(delegate (String S) { if (!WalkedCodes.Contains(S) && !UnWalkedCodes.Contains(S)) { UnWalkedCodes.Add(S); } });
                }
                UnWalkedCodes.RemoveAt(0);
            }
            return WalkedCodes.Union(UnWalkedCodes).ToList();
        }

        static List<String> WalkDiscord(string InviteCode, int JoinDelay, ref List<String> WalkedGuilds)
        {
            string GuildID = Actions.JoinServer(InviteCode);
            if (GuildID == null) { return null; }
            if (WalkedGuilds.Contains(GuildID)) { return null; }
            WalkedGuilds.Add(GuildID);

            Console.WriteLine("Joined Guild: " + GuildID + " And Found These Valid Codes: ");

            List<String> ValidCodes = new List<string> { };
            foreach (string ChannelID in Fetch.TextChannels(GuildID))
            {
                foreach (JToken Message in Fetch.Messages(ChannelID, 100).Where(x => x["content"].ToString().Contains("https://discord.gg/")))
                {
                    string Code = ExtractInviteCode(Message["content"].ToString());
                    if (Actions.ServerExists(Code)) { ValidCodes.Add(Code); Console.Write(Code + ","); }
                }
            }
            if (ValidCodes.Count == 0) { Console.WriteLine("No Valid Codes"); }
            Console.WriteLine("\nWaiting "+JoinDelay+" Seconds before Continuing\n");
            Thread.Sleep((JoinDelay+Master.Rnd.Next(-10,10)) * 1000);
            return ValidCodes;
        }

        static string ExtractInviteCode(string Message)
        {
            string Code = "";
            foreach (Char C in Message.Split(new string[] { "https://discord.gg/" }, StringSplitOptions.None)[1])
            {
                if (Char.IsWhiteSpace(C)) { break; }
                Code += C;
            }
            return Code;
        }
    }
}
