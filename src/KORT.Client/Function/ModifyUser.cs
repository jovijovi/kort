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
        public Response<ModifyUserResult> ModifyUser(ModifyUserRequest requestObject)
        {
            JObject request = JObject.FromObject(requestObject);
            var result = Method(Functions.ModifyUser, request, ParameterValidator.ModifyUserCheckRequest);
            try
            {
                var response = JsonConvert.DeserializeObject<Response<ModifyUserResult>>(result.ToString());
                return response;
            }
            catch (Exception e)
            {
                return Response<ModifyUserResult>.BuildResponse(e);
            }
        }

        public delegate void KORTClientModifyUserDelegate(Response<ModifyUserResult> result);
        public event KORTClientModifyUserDelegate OnModifyUser;
        public IAsyncResult BeginModifyUser(ModifyUserRequest requestObject)
        {
            JObject request = JObject.FromObject(requestObject);
            return BeginMethod(Functions.ModifyUser, request, ParameterValidator.ModifyUserCheckRequest, ModifyUserCallBackMethod);
        }
        private void ModifyUserCallBackMethod(IAsyncResult ar)
        {
            var dn = (KORTClientMethodDelegate)ar.AsyncState;
            JObject result = dn.EndInvoke(ar);
            if (OnModifyUser == null) return;

            try
            {
                var response = JsonConvert.DeserializeObject<Response<ModifyUserResult>>(result.ToString());
                OnModifyUser(response);
            }
            catch (Exception e)
            {
                OnModifyUser(Response<ModifyUserResult>.BuildResponse(e));
            }
        }
    }
}

