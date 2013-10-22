using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using KORT.Server;

namespace KORT.WindowsService
{
    public partial class MainService : ServiceBase
    {
        private KORTServer _server;
        private Uri _serverAddrS = new Uri("http://0.0.0.0:8080/");

        public MainService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //TODO: 载入数据库相关配置

            //read app.config
            int port = 0;
            if (!int.TryParse(KORT.WindowsService.Properties.Settings.Default.Port, out port)) { port = 8080; }
            if (port < 1 || port > 65535) { port = 8080; }

            //open server
            string uri = "http://0.0.0.0:" + port + "/";
            _serverAddrS = new Uri(uri);
            _server = new KORTServer { ServerAddr = _serverAddrS };
            _server.Start();
        }

        protected override void OnStop()
        {
            //stop server
            _server.Stop();
        }
    }
}
