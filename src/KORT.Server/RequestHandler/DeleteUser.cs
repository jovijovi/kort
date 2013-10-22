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
        public static void DeleteUser(JObject request, ref JObject result, string language, ref Session session)
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
                AddBadParameterInfo(ref result, Functions.DeleteUser, language);
                return;
            }

            string message;
            bool ret = false;

            if (!String.IsNullOrEmpty(userName))
            {
                ret = UserHelper.Delete(userName, language, out message);
            }
            else
            {
                ret = UserHelper.Delete(userId, language, out message);
            }

            if (ret)
            {
                AddSuccessInfo(ref result, ResultType.Boolean, true, message);
            }
            else
            {
                AddFailInfo(ref result, ErrorNumber.SeeDetail.ToString(), message);
            }
        }
    }
}

