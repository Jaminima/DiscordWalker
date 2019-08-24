using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using DiscordUserAPI;
using Newtonsoft.Json.Linq;

namespace DiscordWalker
{
    class Program
    {
        static void Main(string[] args)
        {
            //StartInviteCode "" will load last state
            List<String> Codes = Backend.Walker.StartWalking(GetInstances(),"", 10, 180);
            Console.WriteLine("Finished Walking");
            Console.ReadLine();
        }

        static List<Instance> GetInstances()
        {
            List<Instance> Instances = new List<Instance> { };
            string[] Details = File.ReadAllLines("./Data/Accounts.txt");
            for (int i = 0; i < Details.Length; i += 2)
            {
                try { Instances.Add(new Instance(Details[i], Details[i + 1])); Console.WriteLine(Details[i] + " Signed In"); Thread.Sleep(5000); }
                catch { Console.WriteLine(Details[i] + " Failed To Login"); }
            }
            return Instances;
        }
    }
}
