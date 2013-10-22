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
    public class GetUserRequest
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
    }
    public class GetUserResult
    {
        public User User { get; set; }
    }

    public static class GetUserFieldKeyword
    {
        public const string UserId = "UserId";
        public const string UserName = "UserName";
        public const string User = "User";
    }

    public partial class ParameterValidator
    {
        public static bool GetUserCheckRequest(JObject request)
        {
            System.Diagnostics.Debug.WriteLine("GetUserCheckRequest");
            if (Check(request)
                && Check(request, GetUserFieldKeyword.UserId, JTokenType.Integer)
                && Check(request, GetUserFieldKeyword.UserName, JTokenType.String)
                )
                return true;
            return false;
        }

        public static bool GetUserCheckResponse(JObject response)
        {
            System.Diagnostics.Debug.WriteLine("GetUserCheckResponse");
            if (Check(response) && Check(response, FieldKeyword.Success, JTokenType.Boolean))
            {
                var success = (bool)response[FieldKeyword.Success];
                if (CheckContent(response, FieldKeyword.ResultType, ResultType.Object, success)
                    && Check(response, FieldKeyword.Result, JTokenType.Object, success)
                    && Check(response[FieldKeyword.Result], GetUserFieldKeyword.User, JTokenType.Object, success)
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

