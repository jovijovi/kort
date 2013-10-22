using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Timers;

namespace KORT.Network
{
    public class SessionHolder<T> where T : AbstractSession
    {
        /// <summary>
        /// 随机密码
        /// </summary>
        private class RandomStr
        {
            /********
            *  Const and Function
            *  ********/

            private static readonly int defaultLength = 8;

            private static int GetNewSeed()
            {
                byte[] rndBytes = new byte[4];
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(rndBytes);
                return BitConverter.ToInt32(rndBytes, 0);
            }

            /********
             *  getRndCode of all char .
             *  ********/

            private static string BuildRndCodeAll(int strLen)
            {
                System.Random RandomObj = new System.Random(GetNewSeed());
                string buildRndCodeReturn = null;
                for (int i = 0; i < strLen; i++)
                {
                    buildRndCodeReturn += (char)RandomObj.Next(33, 125);
                }
                return buildRndCodeReturn;
            }

            public static string GetRndStrOfAll()
            {
                return BuildRndCodeAll(defaultLength);
            }

            public static string GetRndStrOfAll(int LenOf)
            {
                return BuildRndCodeAll(LenOf);
            }

            /********
             *  getRndCode of only .
             *  ********/

            private static string sCharLow = "abcdefghijklmnopqrstuvwxyz";
            private static string sCharUpp = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            private static string sNumber = "0123456789";

            private static string BuildRndCodeOnly(string StrOf, int strLen)
            {
                System.Random RandomObj = new System.Random(GetNewSeed());
                string buildRndCodeReturn = null;
                for (int i = 0; i < strLen; i++)
                {
                    buildRndCodeReturn += StrOf.Substring(RandomObj.Next(0, StrOf.Length - 1), 1);
                }
                return buildRndCodeReturn;
            }

            public static string GetRndStrOnlyFor()
            {
                return BuildRndCodeOnly(sCharLow + sNumber, defaultLength);
            }

            public static string GetRndStrOnlyFor(int LenOf)
            {
                return BuildRndCodeOnly(sCharLow + sNumber, LenOf);
            }

            public static string GetRndStrOnlyFor(bool bUseUpper, bool bUseNumber)
            {
                string strTmp = sCharLow;
                if (bUseUpper) strTmp += sCharUpp;
                if (bUseNumber) strTmp += sNumber;

                return BuildRndCodeOnly(strTmp, defaultLength);
            }

            public static string GetRndStrOnlyFor(int LenOf, bool bUseUpper, bool bUseNumber)
            {
                string strTmp = sCharLow;
                if (bUseUpper) strTmp += sCharUpp;
                if (bUseNumber) strTmp += sNumber;

                return BuildRndCodeOnly(strTmp, LenOf);
            }
        }      

        private System.Timers.Timer _timer = new Timer();
        private Dictionary<string, T> sessions = new Dictionary<string, T>();
        public SessionHolder()
        {
            _timer.Interval = 1000 * 60 * 1440;//1440min = 24h
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Start();
        }
        ~SessionHolder()
        {
            _timer.Stop();
        }

        const int TokenLength = 32;

        public string Join(string userName, T session)
        {
            lock (sessions)
            {
                //remove session with same user name;
                var list = sessions.Where(s => s.Value.UserName == userName).ToArray();
                if(list.Count()>0)
                {
                    foreach (var pair in list)
                    {
                        sessions.Remove(pair.Key);
                        System.Diagnostics.Debug.WriteLine("SessionHolder: Remove " + pair.Key);
                    }
                }
                //generate new token
                string token = RandomStr.GetRndStrOfAll(TokenLength);
                while (sessions.ContainsKey(token))
                {
                    token = RandomStr.GetRndStrOfAll(TokenLength);
                }
                //register session
                session.UserName = userName;
                session.Token = token;
                session.LastVisit = DateTime.Now;
                sessions.Add(token, session);
                System.Diagnostics.Debug.WriteLine("SessionHolder: Add " + token);
                return token;
            }
        }

        public T GetSession(string token)
        {
            if(string.IsNullOrEmpty(token)) return null;
            lock (sessions)
            {
                if (sessions.ContainsKey(token))//&&sessions[token].UserName == userName)
                {
                    return sessions[token];
                }
                return null;
            }
        }

        public bool Hit(string token)
        {
            lock (sessions)
            {
                if (sessions.ContainsKey(token))//&&sessions[token].UserName == userName)
                {
                    System.Diagnostics.Debug.WriteLine("SessionHolder: Hit " + token);
                    sessions[token].LastVisit = DateTime.Now;
                    return true;
                }
                return false;
            }
        }

        public void Leave(string token)
        {
            lock (sessions)
            {
                if (sessions.ContainsKey(token))//&&sessions[token].UserName == userName)
                {
                    sessions.Remove(token);
                }
            }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (sessions)
            {
                var list = sessions.Where(s => (DateTime.Now - s.Value.LastVisit) < new TimeSpan(0, 10, 0)).ToArray();
                foreach (var pair in list)
                {
                    sessions.Remove(pair.Key);
                    System.Diagnostics.Debug.WriteLine("SessionHolder: Expire " + pair.Key);
                }
            }
        }
    }
}