using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using Newtonsoft.Json.Linq;

namespace KORT.Network
{
    public partial class ParameterValidator
    {
        private static bool Check(JToken jo)
        {
            if (jo == null) return false;
            if (!jo.HasValues) return false;
            return true;
        }

        private static bool Check(JToken jo, string key, JTokenType type)
        {
            return Check(jo, key, type, true);
        }

        private static bool Check(JToken jo, string key, JTokenType type, bool isNeedCheck)
        {
            if (!isNeedCheck) return true;
            if (jo[key] == null)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Error: {0} is Null!", key));
                return false;
            }
            if (jo[key].Type != type)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Error: {0} should be {1}, not {2}!", key, type, jo[key].Type));
                return false;
            }
            return true;
        }

        private static bool CheckContent(JToken jo, string key, string value, bool isNeedCheck)
        {
            if (!isNeedCheck) return true;
            if (jo[key] == null) return false;
            if (jo[key].Type != JTokenType.String) return false;
            if (jo[key].ToString() != value) return false;
            return true;
        }
    }
}