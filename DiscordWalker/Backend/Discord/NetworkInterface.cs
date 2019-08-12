using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;

namespace DiscordWalker.Backend.Discord
{
    public static class NetworkInterface
    {
        public static string Token = "";

        public static bool SignIn(string Email,string Password)
        {
            JToken Res = Request("auth/login", "{\"email\":\"" + Email + "\",\"password\":\"" + Password + "\"}");
            if (Res != null && Res["token"] != null)
            {
                Token = Res["token"].ToString();
                return true;
            }
            return false;
        }

        public static JToken Request(string URL, string Data="", Boolean WToken=false, string Method = "POST")
        {
            Method = Method.ToUpper();
            byte[] ReqData = Encoding.ASCII.GetBytes(Data);
            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create("https://discordapp.com/api/v6/" + URL);
            Req.Method = Method; Req.ContentType = "application/json";
            if (WToken) { Req.Headers.Add("authorization", Token); }
            if (Method == "POST")
            {
                Stream ReqStream = Req.GetRequestStream();
                ReqStream.Write(ReqData, 0, ReqData.Length);
                ReqStream.Close();
            }

            try
            {
                WebResponse Res = Req.GetResponse();
                string ResData = new StreamReader(Res.GetResponseStream()).ReadToEnd();
                if (Method == "DELETE") { return null; }
                else { try { return JToken.Parse(ResData); } catch { return JToken.Parse("{Content:" + ResData + "}"); } }
            }
            catch (WebException e)
            {
                if (((HttpWebResponse)e.Response).StatusCode.ToString() == "429")
                {
                    Console.WriteLine(e.Message);
                    DateTime DT = DateTime.Parse(e.Response.Headers["Date"]).AddSeconds(int.Parse(e.Response.Headers["Retry-After"]));
                    TimeSpan ST = (TimeSpan)(DT - DateTime.Now);
                    Console.WriteLine("Time Till Expried " + Math.Floor(ST.TotalHours) + ":" + ST.Minutes + ":" + ST.Seconds);
                }
                return null;
            }
        }
    }
}
