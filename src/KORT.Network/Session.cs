using System;
using KORT.Data;

namespace KORT.Network
{
    public abstract class AbstractSession
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public DateTime LastVisit { get; set; }
    }

    public class Session : AbstractSession
    {
        public UserType UserType { get; set; }
        public string Language { get; set; }
    }
}