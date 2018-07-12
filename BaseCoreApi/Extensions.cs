using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace BaseCoreApi
{
    public static class Extensions
    {
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T FromJson<T>(this object obj)
        {
            return JsonConvert.DeserializeObject<T>(obj as string);

        }
    }
}
