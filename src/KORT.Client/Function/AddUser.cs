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
        public Response<AddUserResult> AddUser(AddUserRequest requestObject)
        {
            JObject request = JObject.FromObject(requestObject);
            var result = Method(Functions.AddUser, request, ParameterValidator.AddUserCheckRequest);
            try
            {
                var response = JsonConvert.DeserializeObject<Response<AddUserResult>>(result.ToString());
                return response;
            }
            catch (Exception e)
            {
                return Response<AddUserResult>.BuildResponse(e);
            }
        }

        public delegate void KORTClientAddUserDelegate(Response<AddUserResult> result);
        public event KORTClientAddUserDelegate OnAddUser;
        public IAsyncResult BeginAddUser(AddUserRequest requestObject)
        {
            JObject request = JObject.FromObject(requestObject);
            return BeginMethod(Functions.AddUser, request, ParameterValidator.AddUserCheckRequest, AddUserCallBackMethod);
        }
        private void AddUserCallBackMethod(IAsyncResult ar)
        {
            var dn = (KORTClientMethodDelegate)ar.AsyncState;
            JObject result = dn.EndInvoke(ar);
            if (OnAddUser == null) return;

            try
            {
                var response = JsonConvert.DeserializeObject<Response<AddUserResult>>(result.ToString());
                OnAddUser(response);
            }
            catch (Exception e)
            {
                OnAddUser(Response<AddUserResult>.BuildResponse(e));
            }
        }
    }
}

