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
using KORT.Language;

namespace KORT.Server
{
    public partial class KORTService
    {
        public static void Download(JObject request, ref JObject result, string language, ref Session session)
        {
            string fileName;
            try
            {
                fileName = request[DownloadFieldKeyword.FileName].ToString();
            }
            catch (Exception)
            {
                AddBadParameterInfo(ref result, Functions.Download, language);
                return;
            }
            
            var path = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"File"), fileName);
            System.Diagnostics.Debug.WriteLine(path);
            if(File.Exists(path))
            {
                var fileInfo = new FileInfo(path);
                var data = File.ReadAllBytes(path);
                var md5 = KORT.Util.Tools.GetMD5(data);
                result.Add(FieldKeyword.Success, true);
                result.Add(FieldKeyword.ResultType, ResultType.Object);
                var resultObject = new DownloadResult
                                       {
                                           Length = fileInfo.Length,
                                           MD5 = md5,
                                           Data = data
                                       };
                result.Add(FieldKeyword.Result, JObject.FromObject(resultObject));
            }
            else
            {
                AddFailInfo(ref result, ErrorNumber.SeeDetail.ToString(), MessageHelper.GetMessage(ErrorNumber.FileNotExist, language, fileName));
            }
        }
    }
}

