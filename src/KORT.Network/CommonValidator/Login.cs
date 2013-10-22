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
    /*
     * 用户登录
     * FunctionKeyword.Login
     * */
    public class LoginRequest
    {
        public string User { get; set; }//用户名  必须，非空
        public string Passwd { get; set; }//密码    必须，非空
    }
    public class LoginResult
    {
        public string Token { get; set; }//令牌       必须，非空
        public string UserType { get; set; }//用户类型
        public string LastAddress { get; set; }//上次登陆地址
        public DateTime LastDatetime { get; set; }//上次登陆时间
        public string Address { get; set; }//本次登陆地址
        public DateTime Datetime { get; set; }//本次登陆时间
        public string UserName { get; set; }//用户名称
    }

    public static class LoginFieldKeyword
    {
        public const string User = "User";
        public const string Passwd = "Passwd";
        public const string Token = "Token";
        public const string UserType = "UserType";
        public const string LastAddress = "LastAddress";
        public const string LastDatetime = "LastDatetime";
        public const string Address = "Address";
        public const string Datetime = "Datetime";
        public const string EmployeeId = "EmployeeId";
    }

    public partial class ParameterValidator
    {
        /// <summary>
        /// Logins the check request.
        /// 检查登录时的请求数据(即参数)
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public static bool LoginCheckRequest(JObject request)
        {
            System.Diagnostics.Debug.WriteLine("LoginCheckRequest");
            if (Check(request)
                && Check(request, LoginFieldKeyword.User, JTokenType.String)
                && Check(request, LoginFieldKeyword.Passwd, JTokenType.String)
                )
                return true;
            return false;
        }

        /// <summary>
        /// Logins the check response.
        /// 检查登录后的反馈数据
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        public static bool LoginCheckResponse(JObject response)
        {
            System.Diagnostics.Debug.WriteLine("LoginCheckResponse");
            if (Check(response)&& Check(response, FieldKeyword.Success, JTokenType.Boolean))
            {
                var success = (bool) response[FieldKeyword.Success];
                if (CheckContent(response, FieldKeyword.ResultType, ResultType.List, success)
                    && Check(response, FieldKeyword.Result, JTokenType.Object, success)
                    && Check(response[FieldKeyword.Result], LoginFieldKeyword.Token, JTokenType.String, success)
                    && Check(response[FieldKeyword.Result], LoginFieldKeyword.UserType, JTokenType.String, success)
                    && Check(response[FieldKeyword.Result], LoginFieldKeyword.LastAddress, JTokenType.String, success)
                    && Check(response[FieldKeyword.Result], LoginFieldKeyword.LastDatetime, JTokenType.Date, success)
                    && Check(response[FieldKeyword.Result], LoginFieldKeyword.Address, JTokenType.String, success)
                    && Check(response[FieldKeyword.Result], LoginFieldKeyword.Datetime, JTokenType.Date, success)
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
