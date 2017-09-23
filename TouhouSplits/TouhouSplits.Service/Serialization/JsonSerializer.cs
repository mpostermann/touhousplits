using System;

namespace TouhouSplits.Service.Serialization
{
    public class JsonSerializer<T> : IJsonSerializer<T>
    {
        public T Deserialize(string filepath)
        {
            throw new NotImplementedException();
        }

        public void Serialize(T obj, string filepath)
        {
            throw new NotImplementedException();
        }
    }
}
