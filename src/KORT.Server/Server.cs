using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using Newtonsoft.Json.Linq;
using KORT.Network;

namespace KORT.Server
{
    public class KORTServer
    {
        public Uri ServerAddr { get; set; }
        private ServiceHost _host;

        public void Start()
        {
            _host = KORTServer.GetServiceHost(ServerAddr);
            _host.Open();
        }

        public void Stop()
        {
            if(_host!=null)_host.Close();
        }

        private static Binding GetBinding()
        {
            var binding = new WebHttpBinding();
            binding.Security.Mode = WebHttpSecurityMode.None;//BasicHttpSecurityMode.None;
            binding.MaxReceivedMessageSize = 1000 * 1024 * 1024;
            binding.ReaderQuotas.MaxStringContentLength = 1000 * 1024 * 1024;//10 * 1024 * 1024
            binding.SendTimeout = new TimeSpan(0, 500, 0);//new TimeSpan(0, 5, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 500, 0);//new TimeSpan(0, 5, 0);
            return binding;
        }
        public static ServiceHost GetServiceHost(Uri baseAddress)
        {
            ServiceHost host = new ServiceHost(typeof(KORTService), baseAddress);
            ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(IService), GetBinding(), baseAddress);
            WebHttpBehavior httpBehavior = new WebHttpBehavior {DefaultBodyStyle = WebMessageBodyStyle.Bare };
            endpoint.Behaviors.Add(httpBehavior);
            return host;
        }
    }
}