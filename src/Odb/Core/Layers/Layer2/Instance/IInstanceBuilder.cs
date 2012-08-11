using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Core.Layers.Layer2.Instance
{
    public interface IInstanceBuilder
    {
        /// <summary>
        ///   Builds a Non Native Object instance TODO Perf checks the IFs Builds a non native object using The object info
        /// </summary>
        /// <param name="objectInfo"> </param>
        /// <returns> The instance </returns>
        object BuildOneInstance(NonNativeObjectInfo objectInfo);

        /// <summary>
        ///   Returns the session id of this instance builder (odb database identifier)
        /// </summary>
        /// <returns> </returns>
        string GetSessionId();
    }
}