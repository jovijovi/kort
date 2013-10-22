using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KORT.Data;
using KORT.Network;
using KORT.Language;

namespace KORT.Server
{
    public partial class KORTService
    {
        /// <summary>
        /// Regists the method.
        /// 注册方法(每个方法对应一项功能请求)
        /// </summary>
        static void RegistMethod()
        {
            RegistMethod(new Method
            {
                MethodName = Functions.Login,
                CheckRequest = ParameterValidator.LoginCheckRequest,
                CheckResponse = ParameterValidator.LoginCheckResponse,
                Logic = Login
            });
            RegistMethod(new Method
            {
                MethodName = Functions.Logout,
                CheckRequest = ParameterValidator.LogoutCheckRequest,
                CheckResponse = ParameterValidator.LogoutCheckResponse,
                Logic = Logout
            });

            #region Users
            RegistMethod(new Method
            {
                MethodName = Functions.GetUser,
                CheckRequest = ParameterValidator.GetUserCheckRequest,
                CheckResponse = ParameterValidator.GetUserCheckResponse,
                Logic = GetUser
            });
            RegistMethod(new Method
            {
                MethodName = Functions.GetUserList,
                CheckRequest = ParameterValidator.GetUserListCheckRequest,
                CheckResponse = ParameterValidator.GetUserListCheckResponse,
                Logic = GetUserList
            });
            RegistMethod(new Method
            {
                MethodName = Functions.AddUser,
                CheckRequest = ParameterValidator.AddUserCheckRequest,
                CheckResponse = ParameterValidator.AddUserCheckResponse,
                Logic = AddUser
            });
            RegistMethod(new Method
            {
                MethodName = Functions.ModifyUser,
                CheckRequest = ParameterValidator.ModifyUserCheckRequest,
                CheckResponse = ParameterValidator.ModifyUserCheckResponse,
                Logic = ModifyUser
            });
            RegistMethod(new Method
            {
                MethodName = Functions.DeleteUser,
                CheckRequest = ParameterValidator.DeleteUserCheckRequest,
                CheckResponse = ParameterValidator.DeleteUserCheckResponse,
                Logic = DeleteUser
            });
            #endregion

            RegistMethod(new Method
            {
                MethodName = Functions.Upload,
                CheckRequest = ParameterValidator.UploadCheckRequest,
                CheckResponse = ParameterValidator.UploadCheckResponse,
                Logic = Upload
            });

            RegistMethod(new Method
            {
                MethodName = Functions.Download,
                CheckRequest = ParameterValidator.DownloadCheckRequest,
                CheckResponse = ParameterValidator.DownloadCheckResponse,
                Logic = Download
            });

            RegistMethod(new Method
            {
                MethodName = Functions.GetLog,
                CheckRequest = ParameterValidator.GetLogCheckRequest,
                CheckResponse = ParameterValidator.GetLogCheckResponse,
                Logic = GetLog
            });
        }
    }

    public partial class FunctionKeyword
    {
        private static void BuildMap()
        {
            //2
            AddFunction(UserType.Null, Functions.Login);
            AddFunction(UserType.Normal, Functions.Login);
            AddFunction(UserType.Manager, Functions.Login);
            AddFunction(UserType.Operator, Functions.Login);
            AddFunction(UserType.Super, Functions.Login);

            AddFunction(UserType.Normal, Functions.Logout);
            AddFunction(UserType.Manager, Functions.Logout);
            AddFunction(UserType.Operator, Functions.Logout);
            AddFunction(UserType.Super, Functions.Logout);

            AddFunction(UserType.Super, Functions.AddUser);
            AddFunction(UserType.Super, Functions.ModifyUser);
            AddFunction(UserType.Super, Functions.DeleteUser);

            AddFunction(UserType.Normal, Functions.GetUser);
            AddFunction(UserType.Manager, Functions.GetUser);
            AddFunction(UserType.Operator, Functions.GetUser);
            AddFunction(UserType.Super, Functions.GetUser);

            AddFunction(UserType.Normal, Functions.GetUserList);
            AddFunction(UserType.Manager, Functions.GetUserList);
            AddFunction(UserType.Operator, Functions.GetUserList);
            AddFunction(UserType.Super, Functions.GetUserList);

            AddFunction(UserType.Normal, Functions.Upload);
            AddFunction(UserType.Manager, Functions.Upload);
            AddFunction(UserType.Operator, Functions.Upload);
            AddFunction(UserType.Super, Functions.Upload);

            AddFunction(UserType.Normal, Functions.Download);
            AddFunction(UserType.Manager, Functions.Download);
            AddFunction(UserType.Operator, Functions.Download);
            AddFunction(UserType.Super, Functions.Download);

            AddFunction(UserType.Operator, Functions.GetLog);
            AddFunction(UserType.Super, Functions.GetLog);

            AddFunction(UserType.Normal, Functions.AddMessage);
            AddFunction(UserType.Manager, Functions.AddMessage);
            AddFunction(UserType.Operator, Functions.AddMessage);
            AddFunction(UserType.Super, Functions.AddMessage);

            AddFunction(UserType.Normal, Functions.ModifyMessage);
            AddFunction(UserType.Manager, Functions.ModifyMessage);
            AddFunction(UserType.Operator, Functions.ModifyMessage);
            AddFunction(UserType.Super, Functions.ModifyMessage);

            //40
            AddFunction(UserType.Normal, Functions.DeleteMessage);
            AddFunction(UserType.Manager, Functions.DeleteMessage);
            AddFunction(UserType.Operator, Functions.DeleteMessage);
            AddFunction(UserType.Super, Functions.DeleteMessage);

            AddFunction(UserType.Normal, Functions.GetMessageList);
            AddFunction(UserType.Manager, Functions.GetMessageList);
            AddFunction(UserType.Operator, Functions.GetMessageList);
            AddFunction(UserType.Super, Functions.GetMessageList);
        }

        public static string GetFunctionsName(Functions function, string language)
        {
            return MessageHelper.GetMessage(function, language);
        }
    }
}
