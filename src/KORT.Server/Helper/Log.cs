using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using log4net.Appender;
using log4net.Core;
using KORT.Data;
using KORT.Language;

namespace KORT.Helper
{
    public static class LogHelper
    {
        //add
        public static bool SaveLog(string userName, string functionName, string parameter, string result, double time)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(functionName))
            {
                return false;
            }
            var log = new LogItem();
            log.Time = DateTime.Now;
            log.UserName = userName;
            log.FunctionName = functionName;
            log.Parameter = parameter.Length < 1 * 1024 ? parameter : parameter.Length.ToString();
            log.Result = result.Length < 1 * 1024 ? result : result.Length.ToString();
            log.ElapsedTime = time;

            var db = DBHelper.Open();
            db.LogItem.Insert(log);

            return true;
        }

        //get
        public static LogItem[] GetLogs(DateTime from, DateTime to, string userName, string functionName,
                                    int pageSize, long pageIndex, 
                                    string language, out string message)
        {            
            try
            {
                //TODO: mongodb 的分页性能问题
                //目前使用的是 Skip 和 Take (即 skip 和 limit) 方法

                int skip = (int)(pageIndex * pageSize);
                int take = pageSize;
                int totalCount;

                var db = DBHelper.Open();
                List<dynamic> list = db.LogItem.Query()
                    .Where(db.LogItem.Time >= from && db.LogItem.Time <= to)
                    .OrderByDescending(db.LogItem.Time)
                    .Skip(skip)
                    .Take(take)
                    .WithTotalCount(out totalCount)
                    .ToList();

                List<LogItem> logs = new List<LogItem>();
                foreach (var log in list)
                {
                    LogItem logItem = log;
                    logs.Add(logItem);
                }
                message = MessageHelper.GetMessage(ActNumber.Get, DataTypeNumber.LogList, Result.Success, language, "");
                return logs.ToArray();
            }
            catch (Exception e)
            {
                message = e.ToString();
                return null;
            }
        }
    }
}
