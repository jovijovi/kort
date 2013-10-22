using System;
using System.Collections.Generic;
using System.Text;

namespace KORT.Data
{
    //和 KORT.Network 中的 Functions 内容相同, 此处用于多语言支持及其他功能
    public enum Functions
    {
        Login,// 登录
        Logout,

        GetUser,// 获取用户列表
        AddUser,// 增加用户
        ModifyUser,// 修改用户
        DeleteUser,// 删除用户
        GetUserList,

        Upload,
        Download,

        GetLog,

        AddMessage,
        ModifyMessage,
        DeleteMessage,
        GetMessageList
    }
}
