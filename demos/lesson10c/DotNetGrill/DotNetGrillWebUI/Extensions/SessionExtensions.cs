using Newtonsoft.Json; // import for JsonConvert

namespace DotNetGrillWebUI.Extensions
{
    // Extension class must be static
    // T is a generic type, it will be replaced by the actual type when the method is called
    // For example: session.GetObject<Order>("TempOrder") > returns an Order object
    public static class SessionExtensions
    {
        // bring code from https://www.talkingdotnet.com/store-complex-objects-in-asp-net-core-session/
        // Store complex objects in session
        // Convert object to JSON string and store it in session as string
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        // Retrieve complex objects from session
        // Get the JSON string from session and convert it to object
        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
