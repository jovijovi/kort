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
        public static void GetUser(JObject request, ref JObject result, string language, ref Session session)
        {
            long userId;
            string userName;
            try
            {
                userId = long.Parse(request[GetUserFieldKeyword.UserId].ToString());
                userName = request[GetUserFieldKeyword.UserName].ToString();
            }
            catch (Exception)
            {
                AddBadParameterInfo(ref result, Functions.GetUser, language);
                return;
            }

            string message;
            User user = null;
            if (!String.IsNullOrEmpty(userName))
            {
                user = UserHelper.Get(userName, out message, language);
            }
            else
            {
                user = UserHelper.Get(userId, out message, language);
            }
            
            if (user != null)
            {
                var resultObject = new GetUserResult
                                       {
                                           User = user
                                       };
                AddSuccessInfo(ref result, ResultType.Object, resultObject, message);
            }
            else
            {
                AddFailInfo(ref result, ErrorNumber.SeeDetail.ToString(), message);
            }
        }
    }
}

