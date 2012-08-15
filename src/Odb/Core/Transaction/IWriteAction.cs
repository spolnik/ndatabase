using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;

namespace NDatabase.Odb.Core.Transaction
{
    public interface IWriteAction
    {
        byte[] GetBytes(int index);

        void ApplyTo(IFileSystemInterface fsi, int index);

        void AddBytes(byte[] bytes);

        void Persist(IFileSystemInterface fsi, int index);

        bool IsEmpty();

        long GetPosition();
    }
}
