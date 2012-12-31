using System;
using System.Reflection;
using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Core.Layers.Layer3.Refactor
{
    public sealed class RefactorManager : IRefactorManager
    {
        private readonly MetaModel _metaModel;
        private readonly IObjectWriter _objectWriter;

        internal RefactorManager(IStorageEngine storageEngine)
        {
            _metaModel = storageEngine.GetSession().GetMetaModel();
            _objectWriter = storageEngine.GetObjectWriter();
        }

        #region IRefactorManager Members

        public void AddField(Type type, Type fieldType, string fieldName)
        {
            var ci = _metaModel.GetClassInfo(type, true);
            // The real attribute id (-1) will be set in the ci.addAttribute
            var cai = new ClassAttributeInfo(-1, fieldName, fieldType.FullName, ci);
            ci.AddAttribute(cai);
            _objectWriter.UpdateClassInfo(ci, true);
        }

        public void RemoveField(Type type, string attributeName)
        {
            var ci = _metaModel.GetClassInfo(type, true);
            var cai2 = ci.GetAttributeInfoFromName(attributeName);
            ci.RemoveAttribute(cai2);
            _objectWriter.UpdateClassInfo(ci, true);
        }

        public void RenameClass(string fullClassName, string newFullClassName)
        {
            var ci = _metaModel.GetClassInfo(fullClassName, true);

            var setFullClassName = ci.GetType().GetProperty("FullClassName",
                                                            BindingFlags.SetField | BindingFlags.NonPublic);
            var setMethod = setFullClassName.GetSetMethod(true);
            setMethod.Invoke(ci, new object[] {newFullClassName});

            _objectWriter.UpdateClassInfo(ci, true);
        }

        public void RenameField(Type type, string attributeName, string newAttributeName)
        {
            var ci = _metaModel.GetClassInfo(type, true);
            var cai2 = ci.GetAttributeInfoFromName(attributeName);
            cai2.SetName(newAttributeName);
            _objectWriter.UpdateClassInfo(ci, true);
        }

        #endregion
    }
}