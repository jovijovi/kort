using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace KORT.Helper
{
    public static class DBHelper
    {
        private static bool _isWorking = true;  //默认数据库运行
        public static bool IsWorking()
        {
            return _isWorking;
        }

 
        #region Config

        private static readonly string _configFileName = "database.ini";

        private static bool GetDatabaseConfig(out string serverIP, out string databaseName)
        {
            StreamReader din = null;
            String str = null;

            serverIP = "127.0.0.1";
            databaseName = "TestDatabase";

            try
            {
                //配置文件
                string filename = System.IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, _configFileName);
                
                din = File.OpenText(filename);  //读取配置文件

                while (null != (str = din.ReadLine()))
                {
                    //Example:
                    //SERVERIP=127.0.0.1
                    Regex RegexAppServerIP = new Regex("SERVERIP=(?<serverip>[\\s\\S]*)");
                    Match m = RegexAppServerIP.Match(str);
                    if (m.Success)
                    {
                        serverIP = m.Groups["serverip"].Value;
                    }

                    //Example:
                    //DATABASENAME=TestDatabase
                    Regex RegexAppDatabaseName = new Regex("DATABASENAME=(?<databasename>[\\s\\S]*)");
                    m = RegexAppDatabaseName.Match(str);
                    if (m.Success)
                    {
                        databaseName = m.Groups["databasename"].Value;
                    }
                }

                din.Close();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Helper

        /// <summary>
        /// Opens the database. Use Simple.Data.MongoDB
        /// </summary>
        /// <returns></returns>
        public static dynamic Open()
        {
            //TODO: 打开数据库
            return null;
        }

        /// <summary>
        /// 重置数据库(含管理员)
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="databaseName">Name of the database.</param>
        public static void Reset(string connectionString, string databaseName)
        {
            //TODO: 重置数据库

            //TODO: 初始化 Admin
        }

        /// <summary>
        /// 清空数据库
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="databaseName">Name of the database.</param>
        public static void Empty(string connectionString, string databaseName)
        {
            //TODO: 清空数据库
        }

        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        public static void InitDatabase(string databaseName)
        {
            string first = System.IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "FirstRun");
            if (!File.Exists(first)) return;
            
            //TODO: 初始化数据库数据
        }

        /// <summary>
        /// 初始化默认管理员
        /// </summary>
        private static void InitAdmin()
        {
            //TODO: 初始化默认管理员
        }

        #endregion

    }
}
