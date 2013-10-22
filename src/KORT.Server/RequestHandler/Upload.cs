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
        public static void Upload(JObject request, ref JObject result, string language, ref Session session)
        {
            string name;
            string data;
            bool isImport;
            UploadImportType importType;
            string languageInChange;
            DateTime time;
            try
            {
                name = request[UploadFieldKeyword.Name].ToString();
                data = request[UploadFieldKeyword.Data].ToString();
                isImport = bool.Parse(request[UploadFieldKeyword.IsImport].ToString());
                importType = (UploadImportType)Enum.Parse(typeof(UploadImportType), request[UploadFieldKeyword.ImportType].ToString());
                languageInChange = request[UploadFieldKeyword.LanguageInChange].ToString();
                time = DateTime.Parse(request[UploadFieldKeyword.Time].ToString());
            }
            catch (Exception)
            {
                AddBadParameterInfo(ref result, Functions.Upload, language);
                return;
            }

            string message = "";
            
            //save data
            if (!string.IsNullOrEmpty(name)
                && data.Length > 0)
            {
                byte[] d = Convert.FromBase64String(data);

                if(isImport)
                {
                    if (!SaveDataToFile(name, "Import", d, out message, language))
                    {
                        AddFailInfo(ref result, ErrorNumber.SeeDetail.ToString(), message);
                        return;
                    }
                    var path = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Import"), name);
                    bool r = true;
                    switch (importType)
                    {
                            case UploadImportType.BinaryData:
                            //r = EmployeeHelper.UltraImportEmployeeData(path, companyId, languageInChange, language, out message);
                            break;                           
                    }
                    if (!r)
                    {
                        AddFailInfo(ref result, ErrorNumber.SeeDetail.ToString(), message);
                        return;
                    }
                }
                else
                {
                    if (!SaveDataToFile(name, d, out message, language))
                    {
                        AddFailInfo(ref result, ErrorNumber.SeeDetail.ToString(), message);
                        return;
                    }
                }
            }
            AddSuccessInfo(ref result, ResultType.Boolean, true, message);
        }

        public static bool SaveDataToFile(string fileName, byte[] data, out string message, string language)
        {
            return SaveDataToFile(fileName, "File", data, out message, language);
        }

        public static bool SaveDataToFile(string fileName, string dir, byte[] data, out string message, string language)
        {
            if (data == null || data.Length == 0)
            {
                message = MessageHelper.GetMessage(ErrorNumber.BadRequest, language);
                return false;
            }

            try
            {
                var d = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir);
                if (!Directory.Exists(d)) Directory.CreateDirectory(d);
                var path = Path.Combine(d, fileName);
                System.Diagnostics.Debug.WriteLine(path);
                if (File.Exists(path)) File.Delete(path);
                File.WriteAllBytes(path, data);
                message = "";
                return true;
            }
            catch (Exception e)
            {
                message = e.ToString();
                return false;
            }
        }
    }
}

