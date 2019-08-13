using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordWalker.Backend
{
    public static class Master
    {
        public static Random Rnd = new Random();

        static List<string> Messages = LoadMessages();

        static List<string> LoadMessages()
        {
            try { return System.IO.File.ReadAllLines("./Data/Messages.txt").ToList(); }
            catch { return new List<string> { }; }
        }

        public static void StoreMessage(string Message)
        {
            if (Message.Length <= 5) { return; }
            System.IO.File.AppendAllText("./Data/Messages.txt", Message + "\n");
            Messages.Add(Message);
        }

        public static string RandomMessage()
        {
            if (Messages.Count == 0) { return "Yee Haw Boys"; }
            return Messages[Rnd.Next(0, Messages.Count)];
        }

        public static void SaveCodes(List<String> Codes)
        {
            System.IO.File.WriteAllLines("./Data/Codes.txt", Codes);
            for (int i=0;i<Codes.Count;i++) { Codes[i] = "https://discord.gg/" + Codes[i]; if (i % 11 == 0) { Codes.Insert(i, "-------------"); i++; } }
            System.IO.File.WriteAllLines("./Data/Links.txt", Codes);
        }
    }
}
