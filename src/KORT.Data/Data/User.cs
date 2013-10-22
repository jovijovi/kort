using System;
using System.Collections.Generic;
using System.Linq;

namespace KORT.Data
{
    public enum UserType
    {
        Super,      //系统管理员
        Operator,   //业务操作员
        Manager,    //管理者
        Normal,     //普通用户
        Null        //非合法用户
    }

    public class User
    {
        /// <summary>
        /// Gets or sets the type.
        /// 用户类型
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public UserType Type { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// 用户名
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the passwd.
        /// 用户登录密码
        /// </summary>
        /// <value>
        /// The passwd.
        /// </value>
        public string Passwd { get; set; }
        /// <summary>
        /// 用户是否可用
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// 上一次登录时间
        /// </summary>
        /// <value>
        /// The last login time.
        /// </value>
        public DateTime LastLoginTime { get; set; }
        /// <summary>
        /// 上一次登录IP
        /// </summary>
        /// <value>
        /// The last login IP.
        /// </value>
        public string LastLoginIP { get; set; }
    }
}