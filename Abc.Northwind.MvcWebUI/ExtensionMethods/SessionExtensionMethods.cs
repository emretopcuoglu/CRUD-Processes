using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Abc.Northwind.MvcWebUI.ExtensionMethods
{
    public static class SessionExtensionMethods
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            string objectString = JsonSerializer.Serialize(value);
            session.SetString(key, objectString);
        }

        public static T GetObject<T>(this ISession session,string key) where T : class
        {
            string objectString = session.GetString(key);
            if (string.IsNullOrWhiteSpace(objectString))
            {
                return null;
            }

            T value = JsonSerializer.Deserialize<T>(objectString);
            return value;
        }
    }
}
