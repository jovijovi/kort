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
    public class GetUserListRequest
    {
        public int PageSize { get; set; }
        public long PageIndex { get; set; }
    }
    public class GetUserListResult
    {
        public User[] Users { get; set; }
    }

    public static class GetUserListFieldKeyword
    {
        public const string PageSize = "PageSize";
        public const string PageIndex = "PageIndex";
        public const string Users = "Users";
    }

    public partial class ParameterValidator
    {
        public static bool GetUserListCheckRequest(JObject request)
        {
            System.Diagnostics.Debug.WriteLine("GetUserListCheckRequest");
            if (Check(request)
                && Check(request, GetUserListFieldKeyword.PageSize, JTokenType.Integer)
                && Check(request, GetUserListFieldKeyword.PageIndex, JTokenType.Integer)
                )
                return true;
            return false;
        }

        public static bool GetUserListCheckResponse(JObject response)
        {
            System.Diagnostics.Debug.WriteLine("GetUserListCheckResponse");
            if (Check(response) && Check(response, FieldKeyword.Success, JTokenType.Boolean))
            {
                var success = (bool)response[FieldKeyword.Success];
                if (CheckContent(response, FieldKeyword.ResultType, ResultType.Array, success)
                    && Check(response, FieldKeyword.Result, JTokenType.Object, success)
                    && Check(response[FieldKeyword.Result], GetUserListFieldKeyword.Users, JTokenType.Array, success)
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

