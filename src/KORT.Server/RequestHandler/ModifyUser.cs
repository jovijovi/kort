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
        public static void ModifyUser(JObject request, ref JObject result, string language, ref Session session)
        {
            User user;
            try
            {
                user = JsonConvert.DeserializeObject<User>(request[AddUserFieldKeyword.User].ToString());
            }
            catch (Exception)
            {
                AddBadParameterInfo(ref result, Functions.ModifyUser, language);
                return;
            }

            string message;
            if (UserHelper.Modify(session.UserType, session.UserName, user, language, out message))
            {
                var resultObject = new ModifyUserResult
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

