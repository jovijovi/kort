using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KORT.Data
{
    public enum MessageType
    {
        Info,   //信息
        Mail,   //邮件
        Rule    //规则
    }
    public class Message
    {
        public MessageType Type { get; set; }       //消息类型
        public string Title { get; set; }           //标题
        public string Content { get; set; }         //正文
        public DateTime GenerateTime { get; set; }  //自动生成时间
        public string SenderName { get; set; }      //发送者名称
        public string ReceiverName { get; set; }    //接收者名称
        public string Number { get; set; }          //编号

        public string AttachName { get; set; }      //附件名称
        public string AttachMD5 { get; set; }       //附件MD5
    }
}
