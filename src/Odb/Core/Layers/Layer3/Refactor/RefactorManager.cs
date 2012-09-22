using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Core.Layers.Layer3.Refactor
{
    public sealed class RefactorManager : IRefactorManager
    {
        private readonly IStorageEngine _storageEngine;

        public RefactorManager(IStorageEngine storageEngine)
        {
            _storageEngine = storageEngine;
        }

        #region IRefactorManager Members

        public void AddField(string className, Type fieldType, string fieldName)
        {
            var metaModel = _storageEngine.GetSession(true).GetMetaModel();
            var ci = metaModel.GetClassInfo(className, true);
            // The real attribute id (-1) will be set in the ci.addAttribute
            var cai = new ClassAttributeInfo(-1, fieldName, fieldType.FullName, ci);
            ci.AddAttribute(cai);
            _storageEngine.GetObjectWriter().UpdateClassInfo(ci, true);
        }

        public void RemoveField(string className, string attributeName)
        {
            var metaModel = _storageEngine.GetSession(true).GetMetaModel();
            var ci = metaModel.GetClassInfo(className, true);
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

        public void RenameField(string className, string attributeName, string newAttributeName)
        {
            var metaModel = _storageEngine.GetSession(true).GetMetaModel();
            var ci = metaModel.GetClassInfo(className, true);
            var cai2 = ci.GetAttributeInfoFromName(attributeName);
            cai2.SetName(newAttributeName);
            _storageEngine.GetObjectWriter().UpdateClassInfo(ci, true);
        }

        #endregion
    }
}
