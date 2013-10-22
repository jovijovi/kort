using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KORT.Server
{
    public class Update
    {
        public class VersionInfo
        {
            public Version Version { get; set; }
            public string ProductName { get; set; }
            public string MD5 { get; set; }
            public string FileName { get; set; }

            public static string VersionInfoToJson(VersionInfo holder)
            {
                var jo = JObject.FromObject(holder);
                return jo.ToString();
            }

            public static VersionInfo JsonToVersionInfo(string json)
            {
                return JsonConvert.DeserializeObject<VersionInfo>(json);
            }
        }

        private string _serverAddrBase = "";
        public Update(string baseAddr)
        {
            _serverAddrBase = baseAddr+"/Web/";
        }

        private VersionInfo _version = null;
        private void Init()
        {
            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "version");
                System.Diagnostics.Debug.WriteLine(path);

                var cv = GetCurrentVersionInfo();
                if (!File.Exists(path))//if local doesn't exist, add local info file
                {
                    _version = cv;
                    SaveVersion();
                }
                else
                {
                    var str = File.ReadAllText(path);
                    _version = VersionInfo.JsonToVersionInfo(str);
                    if(_version.ProductName != cv.ProductName || _version.Version < cv.Version) //this should happen after update
                    {
                        _version = cv;
                        SaveVersion();
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                _version = null;
            }
        }

        private VersionInfo GetCurrentVersionInfo()
        {
            var currentVersion = new Version(Application.ProductVersion);
            return new VersionInfo { Version = currentVersion, ProductName = Application.ProductName };
        }

        private void SaveVersion()
        {
            if (_version == null) return;
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "version");
            //if (!File.Exists(path)) File.Create(path);
            File.WriteAllText(path, VersionInfo.VersionInfoToJson(_version), Encoding.UTF8);
        }
        
        public bool TryUpdate()
        {
            Init();
            if (_version == null) return false;
            try
            {
                using (var client = new WebClient())
                {
                    var versionStream = client.OpenRead(_serverAddrBase + "version");
                    if (versionStream == null) return false;//target not exist or download fail

                    var sr = new StreamReader(versionStream);
                    var str = sr.ReadToEnd();
                    var version = VersionInfo.JsonToVersionInfo(str);
                    sr.Close();
                    if (version.ProductName != _version.ProductName && version.Version > _version.Version)
                    {
                        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, version.FileName);
                        System.Diagnostics.Debug.WriteLine(path);
                        System.Console.WriteLine(path);
                        //exist?
                        if (File.Exists(path)) return true; //already exist
                        //download
                        client.DownloadFile(_serverAddrBase + version.FileName, path);
                        if (!File.Exists(path)) return false; //target not exist or download fail
                        //check md5
                        var md5 = KORT.Util.Tools.GetMD5(path);
                        if (md5 == version.MD5) return true; //get good file
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            
            return false;
        }

    }
}
