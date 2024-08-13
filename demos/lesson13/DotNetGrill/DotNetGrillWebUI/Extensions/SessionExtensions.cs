using Newtonsoft.Json;

namespace DotNetGrillWebUI.Extensions
{
    // Extension classes must be static
    // Name must start with the class it extends
    // Name must end with the word "Extensions"
    // Code from https://www.talkingdotnet.com/store-complex-objects-in-asp-net-core-session/
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
