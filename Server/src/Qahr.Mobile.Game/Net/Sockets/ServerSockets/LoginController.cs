using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile.ServerSockets
{
    public class LoginController
    {
        private static Dictionary<string, byte> LoginControllerContainer = new Dictionary<string, byte>();
        public static bool CanLogin(string IP)
        {
            byte logintimes = 0;
            if (LoginControllerContainer.TryGetValue(IP, out logintimes))
            {
                return logintimes < 3;
            }
            else
            {
                LoginControllerContainer.Add(IP, 0);
                return true;
            }
        }
        public static void LoggedIn(string IP)
        {
            try
            {
                if (LoginControllerContainer.ContainsKey(IP))
                {
                    byte times = LoginControllerContainer[IP];
                    LoginControllerContainer.Remove(IP);
                    times += 1;
                    LoginControllerContainer.Add(IP, times);
                }
            }
            catch { }
        }
        public static void Disconnect(string IP)
        {
            try
            {
                if (LoginControllerContainer.ContainsKey(IP))
                {
                    byte times = LoginControllerContainer[IP];
                    LoginControllerContainer.Remove(IP);
                    if (times > 0)
                    {
                        times -= 1;
                    }
                    LoginControllerContainer.Add(IP, times);
                }
            }
            catch { }
        }
    }
}
