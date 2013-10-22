using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using KORT.Data;
using KORT.Network;

namespace KORT.Client
{
    public partial class KORTClient
    {
        public Response<GetLogResult> GetLog(GetLogRequest requestObject)
        {
            JObject request = JObject.FromObject(requestObject);
            var result = Method(Functions.GetLog, request, ParameterValidator.GetLogCheckRequest);
            try
            {
                var response = JsonConvert.DeserializeObject<Response<GetLogResult>>(result.ToString());
                return response;
            }
            catch (Exception e)
            {
                return Response<GetLogResult>.BuildResponse(e);
            }
        }

        public delegate void KORTClientGetLogDelegate(Response<GetLogResult> result);
        public event KORTClientGetLogDelegate OnGetLog;
        public IAsyncResult BeginGetLog(GetLogRequest requestObject)
        {
            JObject request = JObject.FromObject(requestObject);
            return BeginMethod(Functions.GetLog, request, ParameterValidator.GetLogCheckRequest, GetLogCallBackMethod);
        }
        private void GetLogCallBackMethod(IAsyncResult ar)
        {
            var dn = (KORTClientMethodDelegate)ar.AsyncState;
            JObject result = dn.EndInvoke(ar);
            if (OnGetLog == null) return;

            try
            {
                var response = JsonConvert.DeserializeObject<Response<GetLogResult>>(result.ToString());
                OnGetLog(response);
            }
            catch (Exception e)
            {
                OnGetLog(Response<GetLogResult>.BuildResponse(e));
            }
        }
    }
}

