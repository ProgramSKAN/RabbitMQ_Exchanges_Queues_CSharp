using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public static class ObjectSerialize
    {
        public static byte[] Serialize(this object obj)
        {
            if (obj == null)
                return null;

            var json = JsonConvert.SerializeObject(obj);
            return Encoding.ASCII.GetBytes(json);
        }

        public static object DeSerialize(this byte[] bytes,Type type)
        {
            var json = Encoding.Default.GetString(bytes);
            return JsonConvert.DeserializeObject(json, type);
        }

        public static string DeSerializeText(this byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }
    }
}
