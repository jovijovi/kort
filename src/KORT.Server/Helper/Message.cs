using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KORT.Data;
using KORT.Language;

namespace KORT.Helper
{
    public class MessagePlatform
    {
        //add
        public static bool Add(Message msg, out string message, string language)
        {
            if (msg == null
                || string.IsNullOrEmpty(msg.Title)
                || string.IsNullOrEmpty(msg.Content))
            {
                message = MessageHelper.GetMessage(ErrorNumber.BadRequest, language);
                return false;
            }

            if (String.IsNullOrEmpty(msg.SenderName))
            {
                var sender = UserHelper.Get(msg.SenderName, out message, language); //sender should exist
                if (sender == null)
                {
                    message = MessageHelper.GetMessage(DataTypeNumber.User, Exist.NotExist, language, msg.SenderName);
                    return false;
                }
            }

            if (String.IsNullOrEmpty(msg.ReceiverName))
            {
                var receiver = UserHelper.Get(msg.ReceiverName, out message, language);//receiver should exist
                if (receiver == null)
                {
                    message = MessageHelper.GetMessage(DataTypeNumber.User, Exist.NotExist, language, msg.ReceiverName);
                    return false;
                }
            }

            try
            {
                msg.GenerateTime = DateTime.Now;                

                var db = DBHelper.Open();
                db.Message.Insert(msg);

                message = MessageHelper.GetMessage(ActNumber.Add, DataTypeNumber.Message, Result.Success, language, msg.Title);
                return true;
            }
            catch (Exception e)
            {
                message = e.ToString();
                return false;
            }
        }

        ////delete
        //public static bool Delete(long msgId, long currentUserId, out string message, string language)
        //{
        //    try
        //    {                
        //        var db = DBHelper.Open();

        //        var list = DbEntry.From<Message>().Where(u => u.Id == msgId).Select();
        //        if (list.Count == 0)
        //        {
        //            message = MessageHelper.GetMessage(DataTypeNumber.Message, Exist.NotExist, language, msgId.ToString());
        //            return true;
        //        }
        //        else
        //        {
        //            if (currentUserId != list[0].SenderId)
        //            {
        //                message = MessageHelper.GetMessage(ErrorNumber.CommonWrongAuthority, language, "");
        //                return false;
        //            }

        //            DbEntry.Delete(list[0]);

        //            message = MessageHelper.GetMessage(ActNumber.Delete, DataTypeNumber.Message, Result.Success, language, list[0].Title);
        //            return true;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        message = e.ToString();
        //        return false;
        //    }
        //}

        ////modify
        //public static bool Modify(Message msg, long msgId, long currentUserId, bool isVisible, out string message, string language)
        //{
        //    try
        //    {
        //        if (msg == null
        //            || string.IsNullOrEmpty(msg.Title)
        //            || string.IsNullOrEmpty(msg.Content))
        //        {
        //            message = MessageHelper.GetMessage(ErrorNumber.BadRequest, language);
        //            return false;
        //        }

        //        Future<int> totalCount;
        //        var db = DBHelper.Open();                
        //        List<dynamic> list = db.Users.Query()
        //            .Where(db.Message.Name == user.Name)
        //            .WithTotalCount(out totalCount)
        //            .ToList();
        //        if (0 == totalCount)
        //        {
        //            message = MessageHelper.GetMessage(DataTypeNumber.Message, Exist.NotExist, language, msgId.ToString());
        //            return false;
        //        }
        //        else
        //        {
        //            list[0].IsVisible = isVisible;

        //            if (list[0].Type == MessageType.Rule)
        //            {
        //                var t_list = DbEntry.From<Message>().Where(u => u.Title == msg.Title && u.Type == MessageType.Rule).Select();
        //                if (t_list.Count > 0 && t_list[0].Id != msgId)
        //                {
        //                    list[0].Title += "_" + DateTime.Now.ToString("yyyMMddHHmmss");
        //                }
        //                else
        //                {
        //                    list[0].Title = msg.Title;
        //                }
        //                list[0].Content = msg.Content;
        //            }

        //            DbEntry.Update(list[0]);
        //            message = MessageHelper.GetMessage(ActNumber.Modify, DataTypeNumber.Message, Result.Success, language, list[0].Title);
        //            return true;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        message = e.ToString();
        //        return false;
        //    }
        //}

        ///get
        ////by sender
        //public static Message[] GetMessagesBySender(long senderId, MessageType type, int pageSize, long pageIndex, DateTime from, DateTime to, string language, out string message)
        //{
        //    try
        //    {
        //        Condition c = Condition.True;
        //        c = c && CK.K["GenerateTime"].Ge(from);
        //        c = c && CK.K["GenerateTime"].Le(to);
        //        c = c && CK.K["Type"].Eq(type);
        //        if (senderId > 0)
        //        {
        //            c = c && CK.K["SenderId"].Eq(senderId);
        //        }
        //        var ps = DbEntry
        //            .From<Message>()
        //            .Where(c)
        //            .OrderByDescending(m => m.GenerateTime)
        //            .ThenBy(m => m.Id)
        //            .PageSize(pageSize)
        //            .GetPagedSelector();
        //        var list = ((List<Message>)ps.GetCurrentPage(pageIndex)).ToArray();
        //        message = MessageHelper.GetMessage(ActNumber.Get, DataTypeNumber.MessageList, Result.Success, language, "");
        //        return list;
        //    }
        //    catch (Exception e)
        //    {
        //        message = e.ToString();
        //        return null;
        //    }
        //}
        ////by receiver
        //public static Message[] GetMessagesByReceiver(long receiverId, MessageType type, int pageSize, long pageIndex, DateTime from, DateTime to, string language, out string message)
        //{
        //    try
        //    {
        //        if (type == MessageType.Comment)
        //        {
        //            var receiver = UserHelper.Get(receiverId, out message, language);
        //            if (receiver == null)
        //            {
        //                message = MessageHelper.GetMessage(DataTypeNumber.User, Exist.NotExist, language, receiverId.ToString());
        //                return null;
        //            }
        //            if (receiver.Type != UserType.Super)
        //            {
        //                message = MessageHelper.GetMessage(ErrorNumber.CommonWrongAuthority, language, "");
        //                return null;
        //            }
        //        }
        //        Condition c = Condition.True;
        //        c = c && CK.K["GenerateTime"].Ge(from);
        //        c = c && CK.K["GenerateTime"].Le(to);
        //        c = c && CK.K["Type"].Eq(type);
        //        c = c && CK.K["IsVisible"].Eq(true);
        //        if (type == MessageType.Mail) c = c && CK.K["ReceiverId"] == receiverId;
        //        var ps = DbEntry
        //            .From<Message>()
        //            .Where(c)
        //            .OrderByDescending(m => m.GenerateTime)
        //            .ThenBy(m => m.Id)
        //            .PageSize(pageSize)
        //            .GetPagedSelector();
        //        var list = ((List<Message>)ps.GetCurrentPage(pageIndex)).ToArray();
        //        message = MessageHelper.GetMessage(ActNumber.Get, DataTypeNumber.MessageList, Result.Success, language, "");
        //        return list;
        //    }
        //    catch (Exception e)
        //    {
        //        message = e.ToString();
        //        return null;
        //    }
        //}        
    }
}
