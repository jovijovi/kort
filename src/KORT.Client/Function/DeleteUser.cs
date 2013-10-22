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
using KORT.Language;

namespace KORT.Client
{
    public partial class KORTClient
    {
        public Response<bool> DeleteUser(DeleteUserRequest requestObject)
        {
            JObject request = JObject.FromObject(requestObject);
            var result = Method(Functions.DeleteUser, request, ParameterValidator.DeleteUserCheckRequest);
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

        public delegate void KORTClientDeleteUserDelegate(Response<bool> result);
        public event KORTClientDeleteUserDelegate OnDeleteUser;
        public IAsyncResult BeginDeleteUser(DeleteUserRequest requestObject)
        {
            JObject request = JObject.FromObject(requestObject);
            return BeginMethod(Functions.DeleteUser, request, ParameterValidator.DeleteUserCheckRequest, DeleteUserCallBackMethod);
        }
        private void DeleteUserCallBackMethod(IAsyncResult ar)
        {
            var dn = (KORTClientMethodDelegate)ar.AsyncState;
            JObject result = dn.EndInvoke(ar);
            if (OnDeleteUser == null) return;

            try
            {
                var response = JsonConvert.DeserializeObject<Response<bool>>(result.ToString());
                OnDeleteUser(response);
            }
            catch (Exception e)
            {
                OnDeleteUser(Response<bool>.BuildResponse(e));
            }
        }
    }
}

