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
    public class AddUserRequest
    {
        public User User { get; set; }
    }
    public class AddUserResult
    {
        public User User { get; set; }
    }

    public static class AddUserFieldKeyword
    {
        public const string User = "User";
    }

    public partial class ParameterValidator
    {
        public static bool AddUserCheckRequest(JObject request)
        {
            System.Diagnostics.Debug.WriteLine("AddUserCheckRequest");
            if (Check(request)
                && Check(request, AddUserFieldKeyword.User, JTokenType.Object)
                )
                return true;
            return false;
        }

        public static bool AddUserCheckResponse(JObject response)
        {
            System.Diagnostics.Debug.WriteLine("AddUserCheckResponse");
            if (Check(response) && Check(response, FieldKeyword.Success, JTokenType.Boolean))
            {
                var success = (bool)response[FieldKeyword.Success];
                if (CheckContent(response, FieldKeyword.ResultType, ResultType.Object, success)
                    && Check(response, FieldKeyword.Result, JTokenType.Object, success)
                    && Check(response[FieldKeyword.Result], AddUserFieldKeyword.User, JTokenType.Object, success)
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

