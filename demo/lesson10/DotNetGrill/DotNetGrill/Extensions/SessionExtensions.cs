using Newtonsoft.Json;

namespace DotNetGrill.Extensions
{
    // naming convention [OBJECT TO EXTEND] + [Extensions]
    // class must be static
    public static class SessionExtensions
    {
        // this ISession indicates that the method will extend any object that implements ISession
        // such as HttpContext.Session object
        public static void SetObject(this ISession session, string key, object value)
        {
            // converts object to string representation in JSON format
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            // retrieve string representation in JSON format
            var value = session.GetString(key);
            // deserialize the object and reconstruct the object
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
