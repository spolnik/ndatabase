using System.Globalization;

namespace NDatabase.Odb.Core
{
    internal class Release
    {
        internal static string ReleaseNumber = "1.0";

        internal static string ReleaseBuild = 10.ToString(CultureInfo.InvariantCulture);

        internal static string ReleaseDate = "11-08-2012-09-04-00";
    }
}