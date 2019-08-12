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
    }
}
