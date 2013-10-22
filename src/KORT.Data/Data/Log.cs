using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace KORT.Data
{
    public class LogItem
    {
        public DateTime Time { get; set; }
        public string UserName { get; set; }
        public string FunctionName { get; set; }
        public string Parameter { get; set; }
        public string Result { get; set; }
        public double ElapsedTime { get; set; }
    }
}
