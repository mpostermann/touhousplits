using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Runtime.Serialization.Json;

namespace TouhouSplits.Service.Serialization
{
    public class JsonSerializer<T> : IFileSerializer<T>
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
            string json = SerializeToString(obj);
            json = PrettifyJson(json);

            using (FileStream fileStream = File.Open(filepath, FileMode.OpenOrCreate, FileAccess.Write)) {
                using (StreamWriter sw = new StreamWriter(fileStream)) {
                    sw.Write(json);
                    sw.Flush();
                }
            }
        }

        private string SerializeToString(T obj)
        {
            using (MemoryStream memStream = new MemoryStream()) {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                serializer.WriteObject(memStream, obj);
                memStream.Flush();
                memStream.Position = 0;

                using (StreamReader memReader = new StreamReader(memStream)) {
                    return memReader.ReadToEnd();
                }
            }
        }

        private string PrettifyJson(string json)
        {
            return JValue.Parse(json).ToString(Formatting.Indented);
        }
    }
}
