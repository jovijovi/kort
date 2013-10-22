using System;
using System.Collections.Generic;
using System.IO;
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
    public class DownloadRequest
    {
        public string FileName { get; set; }
    }
    public class DownloadResult
    {
        public long Length { get; set; }
        public string MD5 { get; set; }
        public byte[] Data { get; set; }
    }

    public static class DownloadFieldKeyword
    {
        public const string FileName = "FileName";
        public const string Length = "Length";
        public const string MD5 = "MD5";
        public const string Data = "Data";
    }

    public partial class ParameterValidator
    {
        public static bool DownloadCheckRequest(JObject request)
        {
            System.Diagnostics.Debug.WriteLine("DownloadCheckRequest");
            if (Check(request)
                && Check(request, DownloadFieldKeyword.FileName, JTokenType.String)
                )
                return true;
            return false;
        }

        public static bool DownloadCheckResponse(JObject response)
        {
            System.Diagnostics.Debug.WriteLine("DownloadCheckResponse");
            if (Check(response) && Check(response, FieldKeyword.Success, JTokenType.Boolean))
            {
                var success = (bool)response[FieldKeyword.Success];
                if (CheckContent(response, FieldKeyword.ResultType, ResultType.Object, success)
                    && Check(response, FieldKeyword.Result, JTokenType.Object, success)
                    && Check(response[FieldKeyword.Result], DownloadFieldKeyword.Length, JTokenType.Integer, success)
                    && Check(response[FieldKeyword.Result], DownloadFieldKeyword.MD5, JTokenType.String, success)
                    && Check(response[FieldKeyword.Result], DownloadFieldKeyword.Data, JTokenType.Bytes, success)
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

