using System.Collections.Generic;
using KORT.Data;
using KORT.Network;

namespace KORT.Server
{
    public partial class FunctionKeyword
    {
        #region common
        private static Dictionary<UserType, List<Functions>> _functionMap = new Dictionary<UserType, List<Functions>>();
        public static bool CheckFunction(UserType type, Functions function)
        {
            if(!_functionMap.ContainsKey(type)) return false;
            if(!_functionMap[type].Contains(function)) return false;
            return true;
        }

        static FunctionKeyword()
        {
            BuildMap();
        }

        private static void AddFunction(UserType type, Functions function)
        {
            if (!_functionMap.ContainsKey(type))
            {
                _functionMap.Add(type, new List<Functions>());
            }
            if (!_functionMap[type].Contains(function))
            {
                _functionMap[type].Add(function);
            }
        }
        #endregion
    }
}