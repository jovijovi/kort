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

namespace KORT.Server
{
    public partial class KORTService
    {
        public static void GetUserList(JObject request, ref JObject result, string language, ref Session session)
        {
            int pageSize;
            long pageIndex;
            try
            {
                pageSize = int.Parse(request[GetUserListFieldKeyword.PageSize].ToString());
                pageIndex = long.Parse(request[GetUserListFieldKeyword.PageIndex].ToString());
            }
            catch (Exception)
            {
                AddBadParameterInfo(ref result, Functions.GetUserList, language);
                return;
            }

            string message;
            var users = UserHelper.GetList(pageSize, pageIndex, language, out message);
            var resultObject = new GetUserListResult
                                   {
                                       Users = users
                                   };
            AddSuccessInfo(ref result, ResultType.Array, resultObject, message);
        }
    }
}

