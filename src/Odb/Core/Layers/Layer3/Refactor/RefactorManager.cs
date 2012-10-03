using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Core.Layers.Layer3.Refactor
{
    //TODO: check if auto ordering of fields didn't break the refactor manager 
    public sealed class RefactorManager : IRefactorManager
    {
        private readonly IStorageEngine _storageEngine;

        internal RefactorManager(IStorageEngine storageEngine)
        {
            _storageEngine = storageEngine;
        }

        #region IRefactorManager Members

        public void AddField(Type type, Type fieldType, string fieldName)
        {
            var metaModel = _storageEngine.GetSession(true).GetMetaModel();
            var ci = metaModel.GetClassInfo(type, true);
            // The real attribute id (-1) will be set in the ci.addAttribute
            var cai = new ClassAttributeInfo(-1, fieldName, fieldType.FullName, ci);
            ci.AddAttribute(cai);
            _storageEngine.GetObjectWriter().UpdateClassInfo(ci, true);
        }

        public void RemoveField(Type type, string attributeName)
        {
            var metaModel = _storageEngine.GetSession(true).GetMetaModel();
            var ci = metaModel.GetClassInfo(type, true);
            var cai2 = ci.GetAttributeInfoFromName(attributeName);
            ci.RemoveAttribute(cai2);
            _storageEngine.GetObjectWriter().UpdateClassInfo(ci, true);
        }

        public void RenameClass(string fullClassName, string newFullClassName)
        {
            var metaModel = _storageEngine.GetSession(true).GetMetaModel();
            var ci = metaModel.GetClassInfo(fullClassName, true);
            ci.FullClassName = newFullClassName;
            _storageEngine.GetObjectWriter().UpdateClassInfo(ci, true);
        }

        public void RenameField(Type type, string attributeName, string newAttributeName)
        {
            var metaModel = _storageEngine.GetSession(true).GetMetaModel();
            var ci = metaModel.GetClassInfo(type, true);
            var cai2 = ci.GetAttributeInfoFromName(attributeName);
            cai2.SetName(newAttributeName);
            _storageEngine.GetObjectWriter().UpdateClassInfo(ci, true);
        }

        #endregion
    }
}
