using System.IO;

namespace TouhouSplits.Service.Serialization
{
    public interface IFileSerializer<T>
    {
        void Serialize(T obj, FileInfo filepath);
        T Deserialize(FileInfo filepath);
    }
}
