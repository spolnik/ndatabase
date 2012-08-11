using System;

namespace NDatabase.Odb.Core.Layers.Layer3
{
    /// <summary>
    ///   <p>An interface for refactoring</p>
    /// </summary>
    public interface IRefactorManager
    {
        void RenameClass(string className, string newClassName);

        void RemoveClass(string className);

        void RenameField(string className, string attributeName, string newAttributeName);

        void AddField(string className, Type fieldType, string fieldName);

        void RemoveField(string className, string attributeName);

        void ChangeFieldType(string className, string attributeName, Type newType);
    }
}
