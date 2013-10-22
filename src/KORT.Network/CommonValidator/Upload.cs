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
    public enum UploadImportType
    {
        BinaryData,
        Video,
        Music,
        Picture,
        Txt,
        Office,
        Others
    }
    public class UploadRequest
    {
        public string Name { get; set; }
        public string Data { get; set; }//base64
        public bool IsImport { get; set; }
        public UploadImportType ImportType { get; set; }
        public string LanguageInChange { get; set; }
        public DateTime Time { get; set; }
    }

    public static class UploadFieldKeyword
    {
        public const string Name = "Name";
        public const string Data = "Data";
        public const string IsImport = "IsImport";
        public const string ImportType = "ImportType";
        public const string LanguageInChange = "LanguageInChange";
        public const string Time = "Time";
    }

    public partial class ParameterValidator
    {
        public static bool UploadCheckRequest(JObject request)
        {
            System.Diagnostics.Debug.WriteLine("UploadCheckRequest");
            if (Check(request)
                && Check(request, UploadFieldKeyword.Name, JTokenType.String)
                && Check(request, UploadFieldKeyword.Data, JTokenType.String)
                && Check(request, UploadFieldKeyword.IsImport, JTokenType.Boolean)
                && Check(request, UploadFieldKeyword.ImportType, JTokenType.Integer)
                && Check(request, UploadFieldKeyword.LanguageInChange, JTokenType.String)
                && Check(request, UploadFieldKeyword.Time, JTokenType.Date)
                )
                return true;
            return false;
        }

        public static bool UploadCheckResponse(JObject response)
        {
            System.Diagnostics.Debug.WriteLine("UploadCheckResponse");
            if (Check(response) && Check(response, FieldKeyword.Success, JTokenType.Boolean))
            {
                var success = (bool)response[FieldKeyword.Success];
                if (CheckContent(response, FieldKeyword.ResultType, ResultType.Object, success)
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

