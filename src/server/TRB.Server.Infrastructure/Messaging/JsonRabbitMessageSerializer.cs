using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TRB.Server.Infrastructure.Messaging
{
    public class JsonRabbitMessageSerializer<T> : IRabbitMessageSerializer<T>
    {
        private readonly JsonSerializerSettings _settings = new()
        {
            TypeNameHandling = TypeNameHandling.Objects,
            NullValueHandling = NullValueHandling.Ignore
        };

        public string ContentType => "application/json";

        public byte[] Serialize(T message)
        {
            var json = JsonConvert.SerializeObject(message, typeof(T), Formatting.Indented, _settings);
            return Encoding.UTF8.GetBytes(json);
        }

        public T Deserialize(byte[] body)
        {
            var json = Encoding.UTF8.GetString(body);
            return JsonConvert.DeserializeObject<T>(json, _settings)!;
        }
    }

    public interface IRabbitMessageSerializer<T>
    {
        string ContentType { get; }
        byte[] Serialize(T message);
        T Deserialize(byte[] body);
    }
}
