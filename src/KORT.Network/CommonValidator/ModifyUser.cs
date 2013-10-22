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
    public class ModifyUserRequest
    {
        public User User { get; set; }
    }
    public class ModifyUserResult
    {
        public User User { get; set; }
    }

    public static class ModifyUserFieldKeyword
    {
        public const string User = "User";
    }

    public partial class ParameterValidator
    {
        public static bool ModifyUserCheckRequest(JObject request)
        {
            System.Diagnostics.Debug.WriteLine("ModifyUserCheckRequest");
            if (Check(request)
                && Check(request, ModifyUserFieldKeyword.User, JTokenType.Object)
                )
                return true;
            return false;
        }

        public static bool ModifyUserCheckResponse(JObject response)
        {
            System.Diagnostics.Debug.WriteLine("ModifyUserCheckResponse");
            if (Check(response) && Check(response, FieldKeyword.Success, JTokenType.Boolean))
            {
                var success = (bool)response[FieldKeyword.Success];
                if (CheckContent(response, FieldKeyword.ResultType, ResultType.Object, success)
                    && Check(response, FieldKeyword.Result, JTokenType.Object, success)
                    && Check(response[FieldKeyword.Result], ModifyUserFieldKeyword.User, JTokenType.Object, success)
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

