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
        public Response<bool> Upload(UploadRequest requestObject)
        {
            JObject request = JObject.FromObject(requestObject);
            var result = Method(Functions.Upload, request, ParameterValidator.UploadCheckRequest);
            try
            {
                var response = JsonConvert.DeserializeObject<Response<bool>>(result.ToString());
                return response;
            }
            catch (Exception e)
            {
                return Response<bool>.BuildResponse(e);
            }
        }

        public delegate void KORTClientUploadDelegate(Response<bool> result);
        public event KORTClientUploadDelegate OnUpload;
        public IAsyncResult BeginUpload(UploadRequest requestObject)
        {
            JObject request = JObject.FromObject(requestObject);
            return BeginMethod(Functions.Upload, request, ParameterValidator.UploadCheckRequest, UploadCallBackMethod);
        }
        private void UploadCallBackMethod(IAsyncResult ar)
        {
            var dn = (KORTClientMethodDelegate)ar.AsyncState;
            JObject result = dn.EndInvoke(ar);
            if (OnUpload == null) return;

            try
            {
                var response = JsonConvert.DeserializeObject<Response<bool>>(result.ToString());
                OnUpload(response);
            }
            catch (Exception e)
            {
                OnUpload(Response<bool>.BuildResponse(e));
            }
        }
    }
}

