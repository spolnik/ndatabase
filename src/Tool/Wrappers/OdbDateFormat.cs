using System;
using System.Globalization;

namespace NDatabase.Tool.Wrappers
{
    /**To Wrap SimpleDatFormat
	 * @author olivier
	 */

    public sealed class OdbDateFormat
    {
        private readonly string _pattern;

        public OdbDateFormat(String pattern)
        {
            _pattern = pattern;
            //sdf = new SimpleDateFormat(pattern);
        }

        public String Format(DateTime date)
        {
            return date.ToString(_pattern);
        }

        public DateTime Parse(String text)
        {
            return DateTime.ParseExact(text, _pattern, CultureInfo.InvariantCulture);
        }
    }
}