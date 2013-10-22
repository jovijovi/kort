using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using KORT.Data;

namespace KORT.Language
{
    public enum ErrorNumber
    {
        //server common
        CommonBadContext=100,
        CommonBadRequest,
        CommonBadParameter,
        CommonExpireToken,
        CommonFunctionNotExist,
        CommonWrongAuthority,
        ServerInMaintenance,
        SeeDetail,
        Other=10000,

        //for function
        BadParameter, //args:0=function name

        FailToGetClientInfo,
        UserIsDisabled,//args:0=user name
        LoginSuccess,
        TemplateNotExist,//args:0=template name
        FileNotExist,//args:0=file name

        //for data layer
        BadRequest,

        EmptyPasswd,
        WrongUserOrPasswd,
        KeepAdmin,
        EmptyString,
        HasChildrenDepartment,
        CanNotDeleteDefaultDepartment,
        BadIndexString,
        BadBirthday,
        BadPhoto,
    }

    public enum DataTypeNumber
    {
        GridFS,
        User,
        UserList,
        LogList,
        Message,
        MessageList
    }
    public enum ActNumber
    {
        Add,
        Get,
        GetList,
        Modify,
        Update,
        Delete,
        Reset
    }
    public enum Result
    {
        Success,
        Fail,
    }
    public enum Exist
    {
        Exist,
        NotExist
    }

    public enum MessageTemplate
    {
        Act_DataType_Result,
        DataType_Exist,
    }

    public class SupportLanguage
    {
        public const string Default = "default";
        public const string ChineseSimple = "zh-cn";
        public const string ChineseTranditional = "zh-tw";
        public const string English = "english";
    }

    public static class MessageHelper
    {
        public static string GetMessage(UserType userType, string language)
        {
            return GetMessage(userType.ToString(), language);
        }

        public static string GetMessage(Functions function, string language)
        {
            return GetMessage(function.ToString(), language);
        }

        private static string GetMessage(ErrorNumber errorNumber, string language)
        {
            return GetMessage(errorNumber.ToString(), language);
        }

        public static string GetMessage(DataTypeNumber dataTypeNumber, string language)
        {
            return GetMessage(dataTypeNumber.ToString(), language);
        }

        private static string GetMessage(ActNumber actNumber, string language)
        {
            return GetMessage(actNumber.ToString(), language);
        }

        private static string GetMessage(Result result, string language)
        {
            return GetMessage(result.ToString(), language);
        }

        private static string GetMessage(Exist exist, string language)
        {
            return GetMessage(exist.ToString(), language);
        }

        private static string GetMessageTemplate(MessageTemplate template, string language)
        {
            return GetMessage(template.ToString(), language);
        }

        public static string GetMessage(ErrorNumber errorNumber, string language, params object[] args)
        {
            return string.Format(GetMessage(errorNumber, language), args);
        }

        public static string GetMessage(ActNumber actNumber, DataTypeNumber dataTypeNumber, Result success, string language, string arg3)
        {
            return string.Format(GetMessageTemplate(MessageTemplate.Act_DataType_Result, language),
                                 GetMessage(actNumber, language), 
                                 GetMessage(dataTypeNumber, language),
                                 GetMessage(success, language), arg3);
        }

        public static string GetMessage(DataTypeNumber dataTypeNumber, Exist exist, string language, string arg2)
        {
            return string.Format(GetMessageTemplate(MessageTemplate.DataType_Exist, language),
                                 GetMessage(dataTypeNumber, language),
                                 GetMessage(exist, language), arg2);
        }

        public static Dictionary<string, Dictionary<string, string> > LanguagePool = new Dictionary<string, Dictionary<string, string>>();
        public static string GetMessage(string key, string language)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(language)) return key;
            if (LanguagePool.ContainsKey(language) && LanguagePool[language].ContainsKey(key))
                return LanguagePool[language][key];
            if (LanguagePool.ContainsKey(SupportLanguage.Default) && LanguagePool[SupportLanguage.Default].ContainsKey(key))
                return LanguagePool[SupportLanguage.Default][key];
            return key;
        }
        public static bool IsSupport(string language)
        {
            if (string.IsNullOrEmpty(language)) return false;
            if (LanguagePool.ContainsKey(language)) return true;
            return false;
        }
        static MessageHelper()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Language");
            if (!Directory.Exists(path)) return;
            var directory = new DirectoryInfo(path);
            foreach (FileInfo file in directory.GetFiles("*.json"))
            {
                try
                {
                    var name = file.Name.Replace(file.Extension, "");
                    var content = File.ReadAllText(file.FullName);
                    var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                    LanguagePool.Add(name, dictionary);
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine(exception.Message);
                    continue;
                }
            }
        }
    }
}