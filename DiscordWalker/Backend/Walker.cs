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
            if (UnWalkedCodes == null) { return null; }
            for (int i = 0; i < MaxDiscordsWalked && i < UnWalkedCodes.Count; i++)
            {
                List<String> NewCodes = WalkDiscord(UnWalkedCodes.First(), JoinDelay, ref WalkedGuilds);
                if (NewCodes != null)
                {
                    WalkedCodes.Add(UnWalkedCodes.First());
                    NewCodes.ForEach(delegate (String S) { if (!WalkedCodes.Contains(S) && !UnWalkedCodes.Contains(S)) { UnWalkedCodes.Add(S); } });
                }
                UnWalkedCodes.RemoveAt(0);
                SaveWalkerState(WalkedGuilds,WalkedCodes,UnWalkedCodes);
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

            List<String> ValidCodes = new List<string> { },
                ChannelIDs = Fetch.TextChannels(GuildID);
            Actions.SendMessage(ChannelIDs.First(), Master.RandomMessage());
            foreach (string ChannelID in ChannelIDs)
            {
                List<JToken> ChannelMessages = Fetch.Messages(ChannelID, 100);
                foreach (JToken Message in ChannelMessages.Where(x => x["content"].ToString().Contains("https://discord.gg/")))
                {
                    string Code = ExtractInviteCode(Message["content"].ToString());
                    if (Actions.ServerExists(Code)) { ValidCodes.Add(Code); Console.Write(Code + ","); System.IO.File.AppendAllText("./Data/Codes.txt", Code + "\n"); }
                }
                foreach (JToken Message in ChannelMessages) { Master.StoreMessage(Message["content"].ToString() + "\n"); }
            }
            if (ValidCodes.Count == 0) { Console.WriteLine("No Valid Codes"); }
            int Delay = (JoinDelay + Master.Rnd.Next(-10, 10));
            Console.WriteLine("\nWaiting "+Delay+" Seconds before Continuing\n");
            Thread.Sleep(Delay * 1000);
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

        static void SaveWalkerState(List<String> WalkedGuilds, List<String> WalkedCodes, List<String> UnWalkedCodes)
        {
            WalkerState State = new WalkerState();
            State.WalkedGuilds = WalkedGuilds; State.WalkedCodes = WalkedCodes; State.UnWalkedCodes = UnWalkedCodes;
            System.IO.File.WriteAllText("./Data/WalkerState.txt", JToken.FromObject(State).ToString());
        }
    }

    public class WalkerState
    {
        public List<String> WalkedGuilds = new List<string> { },
                WalkedCodes = new List<string> { },
                UnWalkedCodes = new List<string> { };
    }
}
