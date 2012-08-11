using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;

namespace NDatabase.Odb.Impl.Core.Layers.Layer3.Refactor
{
    public class DefaultRefactorManager : IRefactorManager
    {
        protected IStorageEngine storageEngine;

        public DefaultRefactorManager(IStorageEngine storageEngine
            )
        {
            this.storageEngine = storageEngine;
        }

        #region IRefactorManager Members

        public virtual void AddField(string className, Type fieldType, string fieldName
            )
        {
            MetaModel metaModel = storageEngine.GetSession
                (true).GetMetaModel();
            ClassInfo ci = metaModel.GetClassInfo(className
                                                  , true);
            // The real attribute id (-1) will be set in the ci.addAttribute
            var cai = new ClassAttributeInfo
                (-1, fieldName, fieldType.FullName, ci);
            ci.AddAttribute(cai);
            storageEngine.GetObjectWriter().UpdateClassInfo(ci, true);
        }

        public virtual void ChangeFieldType(string className, string attributeName, Type
                                                                                        newType)
        {
        }

        // TODO Auto-generated method stub
        public virtual void RemoveClass(string className)
        {
        }

        // TODO Auto-generated method stub
        /// <exception cref="System.IO.IOException"></exception>
        public virtual void RemoveField(string className, string attributeName)
        {
            MetaModel metaModel = storageEngine.GetSession
                (true).GetMetaModel();
            ClassInfo ci = metaModel.GetClassInfo(className
                                                  , true);
            ClassAttributeInfo cai2 = ci.GetAttributeInfoFromName
                (attributeName);
            ci.RemoveAttribute(cai2);
            storageEngine.GetObjectWriter().UpdateClassInfo(ci, true);
        }

        /// <exception cref="System.IO.IOException"></exception>
        public virtual void RenameClass(string fullClassName, string newFullClassName)
        {
            MetaModel metaModel = storageEngine.GetSession
                (true).GetMetaModel();
            ClassInfo ci = metaModel.GetClassInfo(fullClassName
                                                  , true);
            ci.SetFullClassName(newFullClassName);
            storageEngine.GetObjectWriter().UpdateClassInfo(ci, true);
        }

        /// <exception cref="System.IO.IOException"></exception>
        public virtual void RenameField(string className, string attributeName, string newAttributeName
            )
        {
            MetaModel metaModel = storageEngine.GetSession
                (true).GetMetaModel();
            ClassInfo ci = metaModel.GetClassInfo(className
                                                  , true);
            ClassAttributeInfo cai2 = ci.GetAttributeInfoFromName
                (attributeName);
            cai2.SetName(newAttributeName);
            storageEngine.GetObjectWriter().UpdateClassInfo(ci, true);
        }

        #endregion
    }
}