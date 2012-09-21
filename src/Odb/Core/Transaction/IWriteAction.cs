using NDatabase.Odb.Core.Layers.Layer3;

namespace NDatabase.Odb.Core.Transaction
{
    public interface IWriteAction
    {
        byte[] GetBytes(int index);

        void ApplyTo(IFileSystemInterface fsi, int index);

        void AddBytes(byte[] bytes);

        void Persist(IFileSystemInterface fsi, int index);

        bool IsEmpty();
        void Clear();
    }
}
