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
        public static void GetLog(JObject request, ref JObject result, string language, ref Session session)
        {
            DateTime from;
            DateTime to;
            string userName;
            string functionName;
            int pageSize;
            long pageIndex;
            try
            {
                from = DateTime.Parse(request[GetLogFieldKeyword.From].ToString());
                to = DateTime.Parse(request[GetLogFieldKeyword.To].ToString());
                userName = request[GetLogFieldKeyword.UserName].ToString();
                functionName = request[GetLogFieldKeyword.FunctionName].ToString();
                pageSize = int.Parse(request[GetLogFieldKeyword.PageSize].ToString());
                pageIndex = long.Parse(request[GetLogFieldKeyword.PageIndex].ToString());
            }
            catch (Exception)
            {
				AddBadParameterInfo(ref result, Functions.GetLog, language);
                return;
            }
            
            string message;
            var logs = LogHelper.GetLogs(from, to, userName, functionName, pageSize, pageIndex, language, out message);
            var resultObject = new GetLogResult
                                   {
                                       Logs = logs
                                   };
            AddSuccessInfo(ref result, ResultType.Array, resultObject, message);
        }
    }
}

