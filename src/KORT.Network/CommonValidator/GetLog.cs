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
    public class GetLogRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string UserName { get; set; }
        public string FunctionName { get; set; }
        public int PageSize { get; set; }
        public long PageIndex { get; set; }
    }
    public class GetLogResult
    {
        public LogItem[] Logs { get; set; }
    }

    public static class GetLogFieldKeyword
    {
        public const string From = "From";
        public const string To = "To";
        public const string UserName = "UserName";
        public const string FunctionName = "FunctionName";
        public const string PageSize = "PageSize";
        public const string PageIndex = "PageIndex";
        public const string Logs = "Logs";
    }

    public partial class ParameterValidator
    {
        public static bool GetLogCheckRequest(JObject request)
        {
            System.Diagnostics.Debug.WriteLine("GetLogCheckRequest");
            if (Check(request)
                && Check(request, GetLogFieldKeyword.From, JTokenType.Date)
                && Check(request, GetLogFieldKeyword.To, JTokenType.Date)
                && Check(request, GetLogFieldKeyword.UserName, JTokenType.String)
                && Check(request, GetLogFieldKeyword.FunctionName, JTokenType.String)
                && Check(request, GetLogFieldKeyword.PageSize, JTokenType.Integer)
                && Check(request, GetLogFieldKeyword.PageIndex, JTokenType.Integer)
                )
                return true;
            return false;
        }

        public static bool GetLogCheckResponse(JObject response)
        {
            System.Diagnostics.Debug.WriteLine("GetLogCheckResponse");
            if (Check(response) && Check(response, FieldKeyword.Success, JTokenType.Boolean))
            {
                var success = (bool)response[FieldKeyword.Success];
                if (CheckContent(response, FieldKeyword.ResultType, ResultType.Array, success)
                    && Check(response, FieldKeyword.Result, JTokenType.Object, success)
                    && Check(response[FieldKeyword.Result], GetLogFieldKeyword.Logs, JTokenType.Array, success)
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

