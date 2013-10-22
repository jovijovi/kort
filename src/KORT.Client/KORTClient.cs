using System;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using log4net.Appender;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using KORT.Data;
using KORT.Network;
using KORT.Language;
using KORT.Util;

namespace KORT.Client
{
    public partial class KORTClient
    {
        static log4net.ILog _log;
        static KORTClient()
        {
            //setup logger
            var repository = log4net.LogManager.CreateRepository("KORTClientRepository");
            
            var appender = new FileAppender(new log4net.Layout.PatternLayout("%r %d [%t] %c %-5p - %m%n"),
                                            System.IO.Path.Combine(
                                                AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                "Log/Client_" + DateTime.Now.ToShortDateString() + ".log"), 
                                            true);

            appender.Encoding = Encoding.UTF8;
            //Off\Fatal\CommonError\Warn\Info\Debug\All
            appender.Threshold = log4net.Core.Level.Info;
            
            log4net.Config.BasicConfigurator.Configure(repository, appender);

            repository.Configured = true;

            _log = log4net.LogManager.GetLogger("KORTClientRepository", "KORTClient");
        }

        public string Token { get; set; }
        public string Language { get; set; }
        public Uri ServerAddr { get; set; }

        public JObject Method(Functions funcName, JObject request, MethodCheckRequestParameters check)
        {
            if (!string.IsNullOrEmpty(Token)) request[FieldKeyword.Token] = Token;
            if (!string.IsNullOrEmpty(Language)) request[FieldKeyword.Language] = Language;
            JObject response = new JObject();
            if (ServerAddr==null|| request == null || (check != null && !check(request)))
            {
                response[FieldKeyword.Success] = false;
                response[FieldKeyword.CommonError] = ErrorNumber.CommonBadParameter.ToString();
                return response;
            }

            var webBinding = new WebHttpBinding();
            webBinding.AllowCookies = true;
            webBinding.MaxReceivedMessageSize = 1000 * 1024 * 1024;
            webBinding.ReaderQuotas.MaxStringContentLength = 1000 * 1024 * 1024;
            webBinding.SendTimeout = new TimeSpan(0, 500, 0);
            webBinding.ReceiveTimeout = new TimeSpan(0, 500, 0);
            
            using (var factory = new WebChannelFactory<IService>(webBinding, ServerAddr))
            {
                factory.Endpoint.Behaviors.Add(new WebHttpBehavior());
                var session = factory.CreateChannel();
                if (session == null || (session as IContextChannel)==null)
                {
                    response[FieldKeyword.Success] = false;
                    response[FieldKeyword.CommonError] = ErrorNumber.CommonBadContext.ToString();
                }
                else
                    using (OperationContextScope scope = new OperationContextScope(session as IContextChannel))
                    {
                        var temp = request.ToString();
                        Stream stream = new MemoryStream(KORT.Util.Tools.GZipCompress(Encoding.UTF8.GetBytes(temp)));
                        System.Diagnostics.Debug.WriteLine(request.ToString());
                        try
                        {
                            using (var responseStream = session.Method(funcName.ToString(), stream))
                            {
                                using (var decompressStream = new MemoryStream())
                                {
                                    KORT.Util.Tools.GZipDecompress(responseStream, decompressStream);
                                    decompressStream.Position = 0;
                                    StreamReader reader = new StreamReader(decompressStream, Encoding.UTF8);
                                    string text = reader.ReadToEnd();
                                    System.Diagnostics.Debug.WriteLine(text);
                                    response = JObject.Parse(text);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                            response[FieldKeyword.Success] = false;
                            response[FieldKeyword.CommonError] = ErrorNumber.Other.ToString();
                            response[FieldKeyword.ErrorDetail] = e.Message;
                        }
                    }
                return response;
            }
        }

        private IAsyncResult BeginMethod(Functions funcName, JObject request, MethodCheckRequestParameters check, AsyncCallback callBackMethod)
        {
            var methodDelegate = new KORTClientMethodDelegate(Method);
            return methodDelegate.BeginInvoke(funcName, request, check, callBackMethod, methodDelegate);
        }
        private delegate JObject KORTClientMethodDelegate(Functions funcName, JObject request, MethodCheckRequestParameters check);
    }
}