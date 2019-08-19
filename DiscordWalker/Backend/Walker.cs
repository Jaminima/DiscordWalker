﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DiscordUserAPI;
using Newtonsoft.Json.Linq;

namespace DiscordWalker.Backend
{
    public static class Walker
    {
        public static List<String> StartWalking(Instance Instance,string StartInviteCode="", int MaxDiscordsWalked=10, int JoinDelay=5)
        {
            List<String> SeenGuilds = new List<string> { },
                WalkedCodes = new List<string> { },
                UnWalkedCodes;
            if (StartInviteCode=="")
            {
                WalkerState State = LoadWalkerState();
                if (State == null) { return null; }
                SeenGuilds = State.SeenGuilds; WalkedCodes = State.WalkedCodes; UnWalkedCodes = State.UnWalkedCodes;
            }
            else {
                UnWalkedCodes = WalkDiscord(StartInviteCode, ref SeenGuilds, Instance);
                MaxDiscordsWalked--;

                SaveWalkerState(SeenGuilds, WalkedCodes, UnWalkedCodes);
                Master.SaveCodes(WalkedCodes.Union(UnWalkedCodes).ToList());

                if (MaxDiscordsWalked!=0)
                {
                    Console.WriteLine("\nWaiting " + JoinDelay + " Seconds before Continuing\n"); Thread.Sleep(JoinDelay * 1000);
                }
            }

            if (UnWalkedCodes == null) { return null; }

            for (int i = 0; i < MaxDiscordsWalked && i < UnWalkedCodes.Count; i++)
            {
                List<String> NewCodes = WalkDiscord(UnWalkedCodes.First(),  ref SeenGuilds, Instance);
                if (NewCodes != null)
                {
                    WalkedCodes.Add(UnWalkedCodes.First());
                    NewCodes.ForEach(delegate (String S) { if (!WalkedCodes.Contains(S) && !UnWalkedCodes.Contains(S)) { UnWalkedCodes.Add(S); } });
                }
                UnWalkedCodes.RemoveAt(0);
                SaveWalkerState(SeenGuilds, WalkedCodes,UnWalkedCodes);
                Master.SaveCodes(WalkedCodes.Union(UnWalkedCodes).ToList());

                if (i+1 < MaxDiscordsWalked && i+1 < UnWalkedCodes.Count)
                {
                    int Delay = (JoinDelay + Master.Rnd.Next(-10, 10));
                    Console.WriteLine("\nWaiting " + Delay + " Seconds before Continuing\n");
                    Thread.Sleep(Delay * 1000);
                }
            }
            return WalkedCodes.Union(UnWalkedCodes).ToList();
        }

        static List<String> WalkDiscord(string InviteCode, ref List<String> SeenGuilds, Instance Instance)
        {
            string GuildID = Instance.Actions.JoinServer(InviteCode);
            if (GuildID == null) { Console.WriteLine("Failed to join Guild"); return null;  }
            SeenGuilds.Add(GuildID);

            Console.WriteLine("Joined Guild: " + GuildID + " And Found These Valid Codes: ");

            List<String> ValidCodes = new List<string> { },
                ChannelIDs = Instance.Fetch.TextChannels(GuildID);
            Instance.Actions.SendMessage(ChannelIDs.First(), Master.RandomMessage());
            foreach (string ChannelID in ChannelIDs)
            {
                List<JToken> ChannelMessages = Instance.Fetch.Messages(ChannelID, 100);
                foreach (JToken Message in ChannelMessages.Where(x => x["content"].ToString().Contains("https://discord.gg/")))
                {
                    string Code = ExtractInviteCode(Message["content"].ToString()),
                        ServerID = Instance.Fetch.ServerID(Code);
                    if (ServerID!=null && !SeenGuilds.Contains(ServerID)) { SeenGuilds.Add(ServerID); ValidCodes.Add(Code); Console.Write(Code + ","); }
                }
                foreach (JToken Message in ChannelMessages) { Master.StoreMessage(Message["content"].ToString()); }
                Thread.Sleep(500);
            }
            Instance.Actions.LeaveServer(GuildID);
            if (ValidCodes.Count == 0) { Console.WriteLine("No Valid Codes"); }
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

        static void SaveWalkerState(List<String> SeenGuilds, List<String> WalkedCodes, List<String> UnWalkedCodes)
        {
            WalkerState State = new WalkerState();
            State.SeenGuilds = SeenGuilds; State.WalkedCodes = WalkedCodes; State.UnWalkedCodes = UnWalkedCodes;
            System.IO.File.WriteAllText("./Data/WalkerState.txt", JToken.FromObject(State).ToString());
        }

        static WalkerState LoadWalkerState()
        {
            try
            {
                JToken WalkerJSON = JToken.Parse(System.IO.File.ReadAllText("./Data/WalkerState.txt"));
                return WalkerJSON.ToObject<WalkerState>();
            }
            catch { return null; }
        }
    }

    public class WalkerState
    {
        public List<String> SeenGuilds = new List<string> { },
                WalkedCodes = new List<string> { },
                UnWalkedCodes = new List<string> { };
    }
}
