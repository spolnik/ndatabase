using System.Collections;
using System.Text;
using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Tool
{
    /// <summary>
    ///   An utility class to build a string description from a list of ObjectInfoHeader
    /// </summary>
    /// <author>osmadja</author>
    public static class ObjectInfoHeaderListDisplay
    {
        public static string Build(IList objectInfoHeaderList, bool withDetail)
        {
            var buffer = new StringBuilder();
            buffer.Append(objectInfoHeaderList.Count).Append(" objects : ");

            foreach (ObjectInfoHeader objectInfoHeader in objectInfoHeaderList)
            {
                if (withDetail)
                {
                    buffer.Append("(").Append(objectInfoHeader.GetPreviousObjectOID()).Append(" <= ").Append(
                        objectInfoHeader.GetOid()).Append(" => ").Append(objectInfoHeader.GetNextObjectOID()).Append(
                            ") ");
                }
                else
                {
                    buffer.Append(objectInfoHeader.GetOid()).Append(" ");
                }
            }

            return buffer.ToString();
        }
    }
}
