
namespace TouhouSplits.Service.Serialization
{
    public interface IJsonSerializer<T>
    {
        void Serialize(T obj, string filepath);
        T Deserialize(string filepath);
    }
}
