
namespace TouhouSplits.Service.Serialization
{
    public interface IFileSerializer<T>
    {
        void Serialize(T obj, string filepath);
        T Deserialize(string filepath);
    }
}
