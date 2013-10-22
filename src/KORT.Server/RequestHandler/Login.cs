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
using KORT.Helper;
using KORT.Language;

namespace KORT.Server
{
    public partial class KORTService
    {
        /// <summary>
        /// Logins the specified request.
        /// 服务端处理登录请求
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="result">The result.</param>
        /// <param name="language">The language.</param>
        public static void Login(JObject request, ref JObject result, string language, ref Session session)
        {
            var context = OperationContext.Current;//提供方法执行的上下文环境 
            var properties = context.IncomingMessageProperties;//获取传进的消息属性
            var endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;//获取消息发送的远程终结点IP和端口
            if(endpoint==null)
            {
                AddFailInfo(ref result, ErrorNumber.SeeDetail.ToString(), MessageHelper.GetMessage(ErrorNumber.FailToGetClientInfo, language));
                return;
            }

            string message;
            UserType type;
            User user = new User { Name = request[LoginFieldKeyword.User].ToString(), Passwd = request[LoginFieldKeyword.Passwd].ToString() };
            if (UserHelper.VerifyUser(user, out type, language, out message))
            {
                if(!user.IsEnabled)
                {
                    AddFailInfo(ref result, ErrorNumber.SeeDetail.ToString(), MessageHelper.GetMessage(ErrorNumber.UserIsDisabled, language, user.Name));
                    return;
                }
                var loginTime = DateTime.Now;
                var lastuser = UserHelper.Get(user.Name, out message, language);
                session = new Session
                              {
                                  UserType = type,
                                  UserName = lastuser.Name,
                                  Language = language
                              };
                
                string token = _session.Join(request[LoginFieldKeyword.User].ToString(), session);

                var resultObject = new LoginResult
                {
                    Token = token,
                    UserType = type.ToString(),
                    LastAddress = lastuser.LastLoginIP,
                    LastDatetime = lastuser.LastLoginTime,
                    Address = endpoint.Address,
                    Datetime = loginTime,
                    UserName = lastuser.Name
                };
                if (!UserHelper.UpdateLoginInfo(user, loginTime, endpoint.Address, language, out message))
                {
                    AddFailInfo(ref result, ErrorNumber.SeeDetail.ToString(), message);
                }
                
                AddSuccessInfo(ref result, ResultType.List, resultObject, MessageHelper.GetMessage(ErrorNumber.LoginSuccess, language));
                
            }
            else
            {
                AddFailInfo(ref result, ErrorNumber.SeeDetail.ToString(), message);
            }
        }
    }
}
