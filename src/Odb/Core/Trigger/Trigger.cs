namespace NDatabase.Odb.Core.Trigger
{
    /// <summary>
    ///   A simple base class for all triggers.
    /// </summary>
    /// <remarks>
    ///   A simple base class for all triggers.
    /// </remarks>
    /// <author>olivier</author>
    public class Trigger
    {
        private IOdb _odb;

        public virtual void SetOdb(IOdb odb)
        {
            _odb = odb;
        }

        public virtual IOdb GetOdb()
        {
            return _odb;
        }
    }
}
