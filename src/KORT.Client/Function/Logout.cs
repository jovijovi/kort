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
        public Response<bool> Logout(LogoutRequest logoutRequest)
        {
            JObject request = JObject.FromObject(logoutRequest);
            var result = Method(Functions.Logout, request, ParameterValidator.LogoutCheckRequest);  //调用 Logout 登录方法
            try
            {
                var logoutResponse = JsonConvert.DeserializeObject<Response<bool>>(result.ToString());
                return logoutResponse;
            }
            catch (Exception e)
            {
                return Response<bool>.BuildResponse(e);
            }
        }

        public delegate void KORTClientLogoutDelegate(Response<bool> result);
        public event KORTClientLogoutDelegate OnLogout;
        public IAsyncResult BeginLogout(LogoutRequest logoutRequest)
        {
            JObject request = JObject.FromObject(logoutRequest);
            return BeginMethod(Functions.Logout, request, ParameterValidator.LogoutCheckRequest, LogoutCallBackMethod);
        }
        private void LogoutCallBackMethod(IAsyncResult ar)
        {
            var dn = (KORTClientMethodDelegate)ar.AsyncState;
            JObject result = dn.EndInvoke(ar);
            if (OnLogout == null) return;

            try
            {
                var logoutResponse = JsonConvert.DeserializeObject<Response<bool>>(result.ToString());
                OnLogout(logoutResponse);
            }
            catch (Exception e)
            {
                OnLogout(Response<bool>.BuildResponse(e));
            }
        }
    }
}
