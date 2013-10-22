using System;
using System.Collections.Generic;
using System.Linq;
using KORT.Data;
using KORT.Language;

namespace KORT.Helper
{
    public class UserHelper
    {
        /// <summary>
        /// Adds the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="language">The language.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static bool Add(User user, string language, out string message)
        {
            if (user == null || string.IsNullOrEmpty(user.Name))
            {
                message = MessageHelper.GetMessage(ErrorNumber.BadRequest, language);
                return false;
            }
            try
            {
                int totalCount;
                var db = DBHelper.Open();
                List<dynamic> list = db.Users.Query()
                    .Where(db.Users.Name == user.Name)
                    .WithTotalCount(out totalCount)
                    .ToList();
                if (0 == totalCount)
                {
                    db.Users.Insert(user);
                    message = MessageHelper.GetMessage(ActNumber.Add, DataTypeNumber.User, Result.Success, language, user.Name);
                    return true;
                }
                else
                {
                    message = MessageHelper.GetMessage(DataTypeNumber.User, Exist.Exist, language, user.Name);
                    return false;
                }
            }
            catch (Exception e)
            {
                message = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// Deletes the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="language">The language.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static bool Delete(User user, string language, out string message)
        {
            if (user == null || string.IsNullOrEmpty(user.Name))
            {
                message = MessageHelper.GetMessage(ErrorNumber.BadRequest, language);
                return false;
            }
            try
            {
                int totalCount;
                var db = DBHelper.Open();
                List<dynamic> list = db.Users.Query()
                    .Where(db.Users.Name == user.Name)
                    .WithTotalCount(out totalCount)
                    .ToList();
                if (0 == totalCount)
                {
                    message = MessageHelper.GetMessage(DataTypeNumber.User, Exist.NotExist, language, user.Name);
                    return true;
                }
                int count = db.Users.DeleteById(list[0]._id);
                if (count == 1)
                {
                    message = MessageHelper.GetMessage(ActNumber.Delete, DataTypeNumber.User, Result.Success, language, user.Name);
                    return true;
                }
                else
                {
                    message = MessageHelper.GetMessage(ActNumber.Delete, DataTypeNumber.User, Result.Fail, language, user.Name);
                    return false;
                }
            }
            catch (Exception e)
            {
                message = e.ToString();
                return false;
            }
        }
        public static bool Delete(long userId, string language, out string message)
        {
            try
            {
                var db = DBHelper.Open();
                var list = db.Users.FindById(userId);
                if (list == null)
                {
                    message = MessageHelper.GetMessage(DataTypeNumber.User, Exist.NotExist, language, userId.ToString());
                    return true;
                }
                int count = db.Users.DeleteById(userId);
                if (count == 1)
                {
                    message = MessageHelper.GetMessage(ActNumber.Delete, DataTypeNumber.User, Result.Success, language, list[0].Name);
                    return true;
                }
                else
                {
                    message = MessageHelper.GetMessage(ActNumber.Delete, DataTypeNumber.User, Result.Fail, language, list[0].Name);
                    return false;
                }
            }
            catch (Exception e)
            {
                message = e.ToString();
                return false;
            }
        }
        public static bool Delete(string userName, string language, out string message)
        {
            if (string.IsNullOrEmpty(userName))
            {
                message = MessageHelper.GetMessage(ErrorNumber.BadRequest, language);
                return false;
            }
            try
            {
                int totalCount;
                var db = DBHelper.Open();
                List<dynamic> list = db.Users.Query()
                    .Where(db.Users.Name == userName)
                    .WithTotalCount(out totalCount)
                    .ToList();
                if (0 == totalCount)
                {
                    message = MessageHelper.GetMessage(DataTypeNumber.User, Exist.NotExist, language, userName);
                    return true;
                }
                int count = db.Users.DeleteById(list[0]._id);
                if (count == 1)
                {
                    message = MessageHelper.GetMessage(ActNumber.Delete, DataTypeNumber.User, Result.Success, language, userName);
                    return true;
                }
                else
                {
                    message = MessageHelper.GetMessage(ActNumber.Delete, DataTypeNumber.User, Result.Fail, language, userName);
                    return false;
                }
            }
            catch (Exception e)
            {
                message = e.ToString();
                return false;
            }
        }

        //modify
        public static bool Modify(UserType actor, string actorName, User user, string language, out string message)
        {
            if (user == null || string.IsNullOrEmpty(user.Name))
            {
                message = MessageHelper.GetMessage(ErrorNumber.BadRequest, language);
                return false;
            }
            try
            {
                int totalCount;
                var db = DBHelper.Open();                
                List<dynamic> list = db.Users.Query()
                    .Where(db.Users.Name == user.Name)
                    .WithTotalCount(out totalCount)
                    .ToList();
                if (0 == totalCount)
                {
                    message = MessageHelper.GetMessage(DataTypeNumber.User, Exist.NotExist, language, user.Name);
                    return false;
                }

                if (actor == UserType.Super)
                {
                    if (!CheckModifyType((UserType)list[0].Type, user.Type, language, out message)
                        || !CheckModifyPasswd(list[0].Passwd, user.Passwd, language, out message)
                        || !CheckModifyIsEnabled(list[0].IsEnabled, user.IsEnabled, language, out message))
                        return false;

                    list[0].Type = user.Type;
                    if (!string.IsNullOrEmpty(user.Passwd)) //Fix: if do not modify password, leave it empty. do not allow empty password.
                        list[0].Passwd = user.Passwd;
                    list[0].IsEnabled = user.IsEnabled;
                    db.Users.Update(list[0]);
                    message = MessageHelper.GetMessage(ActNumber.Modify, DataTypeNumber.User, Result.Success, language, user.Name);
                    return true;
                }
                else if (actorName == list[0].Name)
                {
                    if (!CheckModifyPasswd(list[0].Passwd, user.Passwd, language, out message))
                        return false;

                    list[0].Passwd = user.Passwd;
                    db.Users.Update(list[0]);
                    message = MessageHelper.GetMessage(ActNumber.Modify, DataTypeNumber.User, Result.Success, language, user.Name);
                    return true;
                }
                else
                {
                    message = MessageHelper.GetMessage(ActNumber.Modify, DataTypeNumber.User, Result.Fail, language, user.Name);
                    return false;
                }
            }
            catch (Exception e)
            {
                message = e.ToString();
                return false;
            }
        }
        private static bool CheckModifyType(UserType oldValue, UserType newValue, string language, out string message)
        {
            message = "";
            if (oldValue == newValue) return true;
            if (oldValue == UserType.Super)
            {
                int totalCount;
                var db = DBHelper.Open();
                List<dynamic> list = db.Users.Query()
                    .Where(db.Users.Type == UserType.Super)
                    .WithTotalCount(out totalCount)
                    .ToList();
                if (0 == totalCount)
                {
                    message = MessageHelper.GetMessage(ErrorNumber.KeepAdmin, language, "");
                    return false;
                }
            }
            return true;
        }
        private static bool CheckModifyPasswd(string oldValue, string newValue, string language, out string message)
        {
            message = "";
            if (oldValue == newValue) return true;
            /*
            if (string.IsNullOrEmpty(newValue))
            {
                message = MessageHelper.GetMessage(ErrorNumber.EmptyPasswd, language, "");
                return false;
            }
             * */
            return true;
        }
        private static bool CheckModifyIsEnabled(bool oldValue, bool newValue, string language, out string message)
        {
            message = "";
            if (oldValue == newValue) return true;
            return true;
        }

        public static bool UpdateLoginInfo(User user, DateTime lastLoginTime, string lastLoginIP, string language, out string message)
        {
            if (user == null || string.IsNullOrEmpty(user.Name))
            {
                message = MessageHelper.GetMessage(ErrorNumber.BadRequest, language);
                return false;
            }
            try
            {
                int totalCount;
                var db = DBHelper.Open();                
                List<dynamic> list = db.Users.Query()
                    .Where(db.Users.Name == user.Name)
                    .WithTotalCount(out totalCount)
                    .ToList();
                if (0 == totalCount)
                {
                    message = MessageHelper.GetMessage(DataTypeNumber.User, Exist.NotExist, language, user.Name);
                    return false;
                }

                user.LastLoginTime = list[0].LastLoginTime;
                user.LastLoginIP = list[0].LastLoginIP;
                list[0].LastLoginTime = lastLoginTime;
                list[0].LastLoginIP = lastLoginIP;
                db.Users.Update(list[0]);
                message = MessageHelper.GetMessage(ActNumber.Update, DataTypeNumber.User, Result.Success, language, user.Name);
                return true;
            }
            catch (Exception e)
            {
                message = e.ToString();
                return false;
            }
        }

        //get
        public static User Get(long userId, out string message, string language)
        {
            message = "";
            try
            {
                var db = DBHelper.Open();
                var list = db.Users.FindById(userId);
                if (list == null)
                {
                    message = MessageHelper.GetMessage(DataTypeNumber.User, Exist.NotExist, language, userId.ToString());
                    return null;
                }
                else
                {
                    message = MessageHelper.GetMessage(ActNumber.Get, DataTypeNumber.User, Result.Success, language, list[0].Name);
                    list.Passwd = "";//erase the password for security
                    return list;
                }
            }
            catch (Exception e)
            {
                message = e.ToString();
                return null;
            }
        }
        public static User Get(string name, out string message, string language)
        {
            message = "";
            try
            {
                int totalCount;
                var db = DBHelper.Open();
                List<dynamic> list = db.Users.Query()
                    .Where(db.Users.Name == name)
                    .WithTotalCount(out totalCount)
                    .ToList();
                if (0 == totalCount)
                {
                    message = MessageHelper.GetMessage(DataTypeNumber.User, Exist.NotExist, language, name);
                    return null;
                }
                else
                {
                    message = MessageHelper.GetMessage(ActNumber.Get, DataTypeNumber.User, Result.Success, language, name);
                    list[0].Passwd = "";//erase the password for security
                    return list[0];
                }
            }
            catch (Exception e)
            {
                message = e.ToString();
                return null;
            }
        }

        public static User[] GetList(int pageSize,  //max: int.MaxValue
            long pageIndex, //from 0
            string language,
            out string message)
        {
            try
            {
                //TODO: mongodb 的分页性能问题
                //目前使用的是 Skip 和 Take (即 skip 和 limit) 方法

                int skip = (int)(pageIndex * pageSize);
                int take = pageSize;

                int totalCount;
                var db = DBHelper.Open();
                List<dynamic> list = db.Users.Query()
                    .OrderByDescending(db.Users._id)
                    .Skip(skip)
                    .Take(take)
                    .WithTotalCount(out totalCount)
                    .ToList();

                List<User> users = new List<User>();
                foreach (var user in list)
                {
                    user.Passwd = "";//erase the password for security
                    User u = user;
                    users.Add(u);
                }
                message = MessageHelper.GetMessage(ActNumber.Get, DataTypeNumber.UserList, Result.Success, language, "");
                return users.ToArray();
            }
            catch (Exception e)
            {
                message = e.ToString();
                return null;
            }
        }
        public static bool VerifyUser(User user, out UserType type, string language, out string message)
        {
            message = "";
            if (user == null || string.IsNullOrEmpty(user.Name))
            {
                message = MessageHelper.GetMessage(ErrorNumber.BadRequest, language);
                type = UserType.Null;
                return false;
            }
            try
            {
                int totalCount;
                var db = DBHelper.Open();
                List<dynamic> list = db.Users.Query()
                    .Where(db.Users.Name == user.Name)
                    .WithTotalCount(out totalCount)
                    .ToList();
                if (1 == totalCount)
                {
                    if (KORT.Util.Tools.CryptoString(user.Passwd) == list[0].Passwd)
                    {
                        message = MessageHelper.GetMessage(ActNumber.Get, DataTypeNumber.User, Result.Success, language, user.Name);
                        type = (UserType)list[0].Type;
                        user.IsEnabled = list[0].IsEnabled;
                        return true;
                    }
                }
                message = MessageHelper.GetMessage(ErrorNumber.WrongUserOrPasswd, language, "");
                type = UserType.Null;
                return false;
            }
            catch (Exception e)
            {
                message = e.ToString();
                type = UserType.Null;
                return false;
            }
        }
    }
}