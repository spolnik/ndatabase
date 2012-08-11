using System;
using System.Text;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   to keep informations about an attribute of a class : <pre>- Its type
    ///                                                          - its name
    ///                                                          - If it is an index</pre>
    /// </summary>
    /// <author>olivier s</author>
    [Serializable]
    public sealed class ClassAttributeInfo
    {
        /// <summary>
        ///   can be null
        /// </summary>
        private readonly Type _nativeClass;

        private OdbType _attributeType;

        private ClassInfo _classInfo;

        private string _className;

        private string _fullClassName;
        private int _id;
        private bool _isIndex;
        private string _name;
        private string _namespace;

        public ClassAttributeInfo()
        {
        }

        public ClassAttributeInfo(int attributeId, string name, string fullClassName, ClassInfo info)
            : this(attributeId, name, null, fullClassName, info)
        {
        }

        public ClassAttributeInfo(int attributeId, string name, Type nativeClass, string fullClassName, ClassInfo info)
        {
            //private transient static int nb=0;
            _id = attributeId;
            _name = name;
            _nativeClass = nativeClass;
            SetFullClassName(fullClassName);
            
            if (nativeClass != null)
            {
                _attributeType = OdbType.GetFromClass(nativeClass);
            }
            else
            {
                if (fullClassName != null)
                    _attributeType = OdbType.GetFromName(fullClassName);
            }

            _classInfo = info;
            _isIndex = false;
        }

        public ClassInfo GetClassInfo()
        {
            return _classInfo;
        }

        public void SetClassInfo(ClassInfo classInfo)
        {
            _classInfo = classInfo;
        }

        public bool IsIndex()
        {
            return _isIndex;
        }

        public void SetIndex(bool isIndex)
        {
            _isIndex = isIndex;
        }

        public string GetName()
        {
            return _name;
        }

        public void SetName(string name)
        {
            _name = name;
        }

        public bool IsNative()
        {
            return _attributeType.IsNative();
        }

        public bool IsNonNative()
        {
            return !_attributeType.IsNative();
        }

        public void SetFullClassName(string fullClassName)
        {
            _fullClassName = fullClassName;
            SetClassName(OdbClassUtil.GetClassName(fullClassName));
            SetNamespace(OdbClassUtil.GetNamespace(fullClassName));
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();

            buffer.Append("id=").Append(_id).Append(" name=").Append(_name).Append(" | is Native=").Append(IsNative()).
                Append(" | type=").Append(GetFullClassname()).Append(" | isIndex=").Append(_isIndex);

            return buffer.ToString();
        }

        public string GetClassName()
        {
            return _className;
        }

        public void SetClassName(string className)
        {
            _className = className;
        }

        public string GetNamespace()
        {
            return _namespace;
        }

        public void SetNamespace(string @namespace)
        {
            _namespace = @namespace;
        }

        public string GetFullClassname()
        {
            if (_fullClassName != null)
                return _fullClassName;

            if (string.IsNullOrEmpty(_namespace))
            {
                _fullClassName = _className;
                return _className;
            }

            _fullClassName = string.Format("{0}.{1}", _namespace, _className);
            return _fullClassName;
        }

        public void SetAttributeType(OdbType attributeType)
        {
            _attributeType = attributeType;
        }

        public OdbType GetAttributeType()
        {
            return _attributeType;
        }

        public Type GetNativeClass()
        {
            return _nativeClass;
        }

        public int GetId()
        {
            return _id;
        }

        public void SetId(int id)
        {
            _id = id;
        }
    }
}
