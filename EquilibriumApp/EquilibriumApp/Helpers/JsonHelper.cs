using Newtonsoft.Json;
using System;

namespace EquilibriumApp.Mobile.Helpers
{
    public static class JsonHelper
    {
        public static T Deserialize<T>(this string json)
        {
            try
            {
                if (string.IsNullOrEmpty(json))
                {
                    return default(T);
                }
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static string Serialize<T>(this T obj)
        {
            if (obj == null)
            {
                return "";
            }
            return JsonConvert.SerializeObject(obj);
        }
    }
}