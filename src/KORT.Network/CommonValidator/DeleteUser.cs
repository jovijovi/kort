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
    public class DeleteUserRequest
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
    }

    public static class DeleteUserFieldKeyword
    {
        public const string UserId = "UserId";
        public const string UserName = "UserName";
    }

    public partial class ParameterValidator
    {
        public static bool DeleteUserCheckRequest(JObject request)
        {
            System.Diagnostics.Debug.WriteLine("DeleteUserCheckRequest");
            if (Check(request)
                && Check(request, DeleteUserFieldKeyword.UserId, JTokenType.Integer)
                && Check(request, DeleteUserFieldKeyword.UserName, JTokenType.String)
                )
                return true;
            return false;
        }

        public static bool DeleteUserCheckResponse(JObject response)
        {
            System.Diagnostics.Debug.WriteLine("DeleteUserCheckResponse");
            if (Check(response) && Check(response, FieldKeyword.Success, JTokenType.Boolean))
            {
                var success = (bool)response[FieldKeyword.Success];
                if (CheckContent(response, FieldKeyword.ResultType, ResultType.Boolean, success)
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

