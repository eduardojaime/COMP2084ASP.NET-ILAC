using Newtonsoft.Json;

namespace DotNetGrillWebUI.Extensions
{
    // For extension methods to work, the class must be static
    // Refer to link https://www.talkingdotnet.com/store-complex-objects-in-asp-net-core-session/
    public static class SessionExtensions
    {
        // Behind scenes, SetObject converts the object to a JSON string
        // and stores it in the session as a string
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        // GetObject retrieves the JSON string from the session
        // and converts it back to the original object
        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
