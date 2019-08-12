﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordWalker.Backend.Discord;
using Newtonsoft.Json.Linq;

namespace DiscordWalker
{
    class Program
    {
        static void Main(string[] args)
        {
            NetworkInterface.SignIn(AccountDetails.Email, AccountDetails.Password);
            //StartInviteCode "" will load last state
            List<String> Codes = Backend.Walker.StartWalking("", 100,240);
            Console.ReadLine();
        }
    }
}
