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

namespace KORT.Server
{
    public partial class KORTService
    {
        /// <summary>
        /// Logouts the specified request.
        /// 服务端处理登录请求
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="result">The result.</param>
        /// <param name="language">The language.</param>
        public static void Logout(JObject request, ref JObject result, string language, ref Session session)
        {
            try
            {
                _session.Leave(session.Token);
                string message = "";
                AddSuccessInfo(ref result, ResultType.Boolean, true, message);
            }
            catch(Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
