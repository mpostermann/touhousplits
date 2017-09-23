using System.IO;
using System.Runtime.Serialization.Json;

namespace TouhouSplits.Service.Serialization
{
    public class JsonSerializer<T> : IJsonSerializer<T>
    {
        public T Deserialize(string filepath)
        {
            using (FileStream stream = File.OpenRead(filepath)) {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }

        public void Serialize(T obj, string filepath)
        {
            using (FileStream stream = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.Write)) {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                serializer.WriteObject(stream, obj);
            }
        }
    }
}
