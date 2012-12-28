using NDatabase2.Odb;

namespace NDatabase.Odb.Core.Trigger
{
    /// <summary>
    ///   A simple base class for all triggers.
    /// </summary>
    public abstract class Trigger
    {
        public IOdb Odb { get; set; }
    }
}
