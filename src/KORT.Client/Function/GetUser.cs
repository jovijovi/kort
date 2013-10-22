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
        public Response<GetUserResult> GetUser(GetUserRequest requestObject)
        {
            JObject request = JObject.FromObject(requestObject);
            var result = Method(Functions.GetUser, request, ParameterValidator.GetUserCheckRequest);
            try
            {
                var response = JsonConvert.DeserializeObject<Response<GetUserResult>>(result.ToString());
                return response;
            }
            catch (Exception e)
            {
                return Response<GetUserResult>.BuildResponse(e);
            }
        }

        public delegate void KORTClientGetUserDelegate(Response<GetUserResult> result);
        public event KORTClientGetUserDelegate OnGetUser;
        public IAsyncResult BeginGetUser(GetUserRequest requestObject)
        {
            JObject request = JObject.FromObject(requestObject);
            return BeginMethod(Functions.GetUser, request, ParameterValidator.GetUserCheckRequest, GetUserCallBackMethod);
        }
        private void GetUserCallBackMethod(IAsyncResult ar)
        {
            var dn = (KORTClientMethodDelegate)ar.AsyncState;
            JObject result = dn.EndInvoke(ar);
            if (OnGetUser == null) return;

            try
            {
                var response = JsonConvert.DeserializeObject<Response<GetUserResult>>(result.ToString());
                OnGetUser(response);
            }
            catch (Exception e)
            {
                OnGetUser(Response<GetUserResult>.BuildResponse(e));
            }
        }
    }
}

