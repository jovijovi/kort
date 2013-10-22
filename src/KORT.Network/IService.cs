using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using KORT.Data;
using KORT.Network;
using KORT.Language;

namespace KORT.Network
{
    public class Response<T>
    {
        public bool Success { get; set; }//是否成功   必须
        public string ResultType { get; set; }
        public string CommonError { get; set; }// 错误号     必须
        public string ErrorDetail { get; set; }//错误细节   必须，ErrorNumber为Other时非空？
        public T Result { get; set; }

        public static Response<T> BuildResponse(Exception exception)
        {
            return new Response<T>
                       {
                           Success = false,
                           ResultType = KORT.Network.ResultType.Null,
                           CommonError = ErrorNumber.SeeDetail.ToString(),
                           ErrorDetail = exception.ToString()
                       };
        }
    }

    public class Method
    {
        public Functions MethodName { get; set; }
        public MethodCheckRequestParameters CheckRequest { get; set; }
        public MethodCheckRequestParameters CheckResponse { get; set; }
        public MethodLogic Logic { get; set; }
    }
    public delegate bool MethodCheckRequestParameters(JObject request);
    public delegate void MethodLogic(JObject request, ref JObject result, string language, ref Session session);
	
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Bare, 
            ResponseFormat = WebMessageFormat.Json, 
            RequestFormat = WebMessageFormat.Json,
            UriTemplate = "/KORT/{funcName}"
        )]
        Stream Method(string funcName, Stream stream);

        [OperationContract]
        [WebGet(
            UriTemplate = "/{*filePath}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json
        )]
        Stream GetFile(string filePath);
    }
}
