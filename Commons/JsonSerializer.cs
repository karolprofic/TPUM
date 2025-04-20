using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Commons
{
    public class JsonSerializer
    {
        public string Serialize<T>(T objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize);
        }

        public T Deserialize<T>(string message)
        {
            return JsonConvert.DeserializeObject<T>(message);
        }

        public string? GetAction(string message)
        {
            JObject jObject = JObject.Parse(message);
            if (jObject.ContainsKey("Action"))
            {
                return (string)jObject["Action"];
            }
            return null;
        }
    }
}
