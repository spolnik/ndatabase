using System;

namespace NDatabase.Odb.Core.Layers.Layer3
{
    /// <summary>
    ///   <p>An interface for refactoring</p>
    /// </summary>
    public interface IRefactorManager
    {
        void RenameClass(string className, string newClassName);

        void RenameField(Type type, string attributeName, string newAttributeName);

        void AddField(Type type, Type fieldType, string fieldName);

        void RemoveField(Type type, string attributeName);
    }
}
