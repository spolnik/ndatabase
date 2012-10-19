namespace NDatabase2.Odb
{
    /// <summary>
    ///   The main interface of all Object Values query results of NDatabase ODB
    /// </summary>
    /// <author>osmadja</author>
    public interface IValues : IObjectSet<IObjectValues>
    {
        IObjectValues NextValues();
    }

    internal interface IInternalValues : IInternalObjectSet<IObjectValues>, IValues
    {
    }
}
