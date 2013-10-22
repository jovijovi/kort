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

namespace KORT.Network
{
    public class LogoutRequest
    {
    }
    public partial class ParameterValidator
    {
        /// <summary>
        /// Logouts the check request.
        /// 检查登录时的请求数据(即参数)
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public static bool LogoutCheckRequest(JObject request)
        {
            System.Diagnostics.Debug.WriteLine("LogoutCheckRequest");
            if (Check(request)
                )
                return true;
            return false;
        }

        /// <summary>
        /// Logouts the check response.
        /// 检查登录后的反馈数据
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        public static bool LogoutCheckResponse(JObject response)
        {
            System.Diagnostics.Debug.WriteLine("LogoutCheckResponse");
            if (Check(response)&& Check(response, FieldKeyword.Success, JTokenType.Boolean))
            {
                var success = (bool) response[FieldKeyword.Success];
                if (CheckContent(response, FieldKeyword.ResultType, ResultType.List, success)
                    && Check(response, FieldKeyword.Result, JTokenType.Object, success)
                    && CheckContent(response, FieldKeyword.ResultType, ResultType.Null, !success)
                    && Check(response, FieldKeyword.CommonError, JTokenType.String, !success)
                    && Check(response, FieldKeyword.ErrorDetail, JTokenType.String, !success)
                    )
                    return true;
            }
            return false;
        }
    }
}
