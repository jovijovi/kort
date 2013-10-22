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
        public Response<GetUserListResult> GetUserList(GetUserListRequest requestObject)
        {
            JObject request = JObject.FromObject(requestObject);
            var result = Method(Functions.GetUserList, request, ParameterValidator.GetUserListCheckRequest);
            try
            {
                var response = JsonConvert.DeserializeObject<Response<GetUserListResult>>(result.ToString());
                return response;
            }
            catch (Exception e)
            {
                return Response<GetUserListResult>.BuildResponse(e);
            }
        }

        public delegate void KORTClientGetUserListDelegate(Response<GetUserListResult> result);
        public event KORTClientGetUserListDelegate OnGetUserList;
        public IAsyncResult BeginGetUserList(GetUserListRequest requestObject)
        {
            JObject request = JObject.FromObject(requestObject);
            return BeginMethod(Functions.GetUserList, request, ParameterValidator.GetUserListCheckRequest, GetUserListCallBackMethod);
        }
        private void GetUserListCallBackMethod(IAsyncResult ar)
        {
            var dn = (KORTClientMethodDelegate)ar.AsyncState;
            JObject result = dn.EndInvoke(ar);
            if (OnGetUserList == null) return;

            try
            {
                var response = JsonConvert.DeserializeObject<Response<GetUserListResult>>(result.ToString());
                OnGetUserList(response);
            }
            catch (Exception e)
            {
                OnGetUserList(Response<GetUserListResult>.BuildResponse(e));
            }
        }
    }
}

