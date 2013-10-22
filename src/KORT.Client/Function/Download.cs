using System;
using System.Collections.Generic;
using System.IO;
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
        public Response<DownloadResult> Download(DownloadRequest requestObject)
        {
            JObject request = JObject.FromObject(requestObject);
            var result = Method(Functions.Download, request, ParameterValidator.DownloadCheckRequest);
            try
            {
                var response = JsonConvert.DeserializeObject<Response<DownloadResult>>(result.ToString());
                return response;
            }
            catch (Exception e)
            {
                return Response<DownloadResult>.BuildResponse(e);
            }
        }

        public delegate void KORTClientDownloadDelegate(Response<DownloadResult> result);
        public event KORTClientDownloadDelegate OnDownload;
        public IAsyncResult BeginDownload(DownloadRequest requestObject)
        {
            JObject request = JObject.FromObject(requestObject);
            return BeginMethod(Functions.Download, request, ParameterValidator.DownloadCheckRequest, DownloadCallBackMethod);
        }
        private void DownloadCallBackMethod(IAsyncResult ar)
        {
            var dn = (KORTClientMethodDelegate)ar.AsyncState;
            JObject result = dn.EndInvoke(ar);
            if (OnDownload == null) return;

            try
            {
                var response = JsonConvert.DeserializeObject<Response<DownloadResult>>(result.ToString());
                OnDownload(response);
            }
            catch (Exception e)
            {
                OnDownload(Response<DownloadResult>.BuildResponse(e));
            }
        }
    }
}

