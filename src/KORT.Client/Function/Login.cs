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
        public Response<LoginResult> Login(LoginRequest loginRequest)
        {
            JObject request = JObject.FromObject(loginRequest);
            var result = Method(Functions.Login, request, ParameterValidator.LoginCheckRequest);  //调用 Login 登录方法
            try
            {
                var loginResponse = JsonConvert.DeserializeObject<Response<LoginResult>>(result.ToString());
                if (loginResponse.Success) Token = loginResponse.Result.Token;
                return loginResponse;
            }
            catch (Exception e)
            {
                return Response<LoginResult>.BuildResponse(e);
            }
        }

        public delegate void KORTClientLoginDelegate(Response<LoginResult> result);
        public event KORTClientLoginDelegate OnLogin;
        public IAsyncResult BeginLogin(LoginRequest loginRequest)
        {
            JObject request = JObject.FromObject(loginRequest);
            return BeginMethod(Functions.Login, request, ParameterValidator.LoginCheckRequest, LoginCallBackMethod);
        }
        private void LoginCallBackMethod(IAsyncResult ar)
        {
            var dn = (KORTClientMethodDelegate)ar.AsyncState;
            JObject result = dn.EndInvoke(ar);
            if (OnLogin == null) return;

            try
            {
                var loginResponse = JsonConvert.DeserializeObject<Response<LoginResult>>(result.ToString());
                if(loginResponse.Success)Token = loginResponse.Result.Token;
                OnLogin(loginResponse);
            }
            catch (Exception e)
            {
                OnLogin(Response<LoginResult>.BuildResponse(e));
            }
        }
    }
}
