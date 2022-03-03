using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonCore
{
    public class JsonHelper
    {
        public static string GetJson(object Obj)
        {
            return JsonConvert.SerializeObject(Obj);
        }

        public static T ProcessToJson<T>(string Json)
        {
            return JsonConvert.DeserializeObject<T>(Json);
        }

        public static JObject GetObj(string JsonStr)
        {
            return JObject.Parse(JsonStr);
        }
    }
}
