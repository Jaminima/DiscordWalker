using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordUserAPI;
using Newtonsoft.Json.Linq;

namespace DiscordWalker
{
    class Program
    {
        static void Main(string[] args)
        {
            Instance Instance = new Instance(AccountDetails.Email, AccountDetails.Password);
            //StartInviteCode "" will load last state
            List<String> Codes = Backend.Walker.StartWalking(Instance,"", 10, 180);
            Console.WriteLine("Finished Walking");
            Console.ReadLine();
        }
    }
}
