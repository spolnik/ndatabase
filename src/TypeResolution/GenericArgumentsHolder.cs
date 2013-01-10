using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NDatabase.TypeResolution
{
    /// <summary>
    /// Holder for the generic arguments when using type parameters.
    /// </summary>
    /// <remarks>
    /// <p>
    /// Type parameters can be applied to classes, interfaces, 
    /// structures, methods, delegates, etc...
    /// </p>
    /// </remarks>
    internal class GenericArgumentsHolder
    {
        private static readonly Regex ClrPattern = new Regex(
              "^"
              + @"(?'name'\w[\w\d\.]+)"
              + @"`\d+\s*\["
              + @"(?'args'(?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!)))"
              + @"\]"
              + @"(?'remainder'.*)"
              + @"$"
              , RegexOptions.CultureInvariant | RegexOptions.Compiled
            );

        private static readonly Regex CSharpPattern = new Regex(
              "^"
              + @"(?'name'\w[\w\d\.]+)"
              + @"<"
              + @"(?'args'.*)"
              + @">"
              + @"(?'remainder'.*)"
              + @"$"
              , RegexOptions.CultureInvariant | RegexOptions.Compiled
            );

        /// <summary>
        /// The generic arguments prefix.
        /// </summary>
        public const char GenericArgumentsQuotePrefix = '[';

        /// <summary>
        /// The generic arguments suffix.
        /// </summary>
        public const char GenericArgumentsQuoteSuffix = ']';

        /// <summary>
        /// The generic arguments prefix.
        /// </summary>
        public const char GenericArgumentsPrefix = '<';

        /// <summary>
        /// The generic arguments suffix.
        /// </summary>
        public const char GenericArgumentsSuffix = '>';

        /// <summary>
        /// The character that separates a list of generic arguments.
        /// </summary>
        public const char GenericArgumentsSeparator = ',';

        private string _unresolvedGenericTypeName;
        private string _unresolvedGenericMethodName;
        private string[] _unresolvedGenericArguments;
        private string _arrayDeclaration;

        /// <summary>
        /// Creates a new instance of the GenericArgumentsHolder class.
        /// </summary>
        /// <param name="value">
        /// The string value to parse looking for a generic definition
        /// and retrieving its generic arguments.
        /// </param>
        public GenericArgumentsHolder(string value)
        {
            ParseGenericTypeDeclaration(value);
        }

        /// <summary>
        /// The (unresolved) generic type name portion 
        /// of the original value when parsing a generic type.
        /// </summary>
        public string GenericTypeName
        {
            get { return _unresolvedGenericTypeName; }
        }

        /// <summary>
        /// The (unresolved) generic method name portion 
        /// of the original value when parsing a generic method.
        /// </summary>
        public string GenericMethodName
        {
            get { return _unresolvedGenericMethodName; }
        }

        /// <summary>
        /// Is the string value contains generic arguments ?
        /// </summary>
        /// <remarks>
        /// <p>
        /// A generic argument can be a type parameter or a type argument.
        /// </p>
        /// </remarks>
        public bool ContainsGenericArguments
        {
            get
            {
                return (_unresolvedGenericArguments != null &&
                    _unresolvedGenericArguments.Length > 0);
            }
        }

        /// <summary>
        /// Is generic arguments only contains type parameters ?
        /// </summary>
        public bool IsGenericDefinition
        {
            get
            {
                if (_unresolvedGenericArguments == null)
                    return false;

                foreach (string arg in _unresolvedGenericArguments)
                {
                    if (arg.Length > 0)
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Returns the array declaration portion of the definition, e.g. "[,]"
        /// </summary>
        /// <returns></returns>
        public string GetArrayDeclaration()
        {
            return _arrayDeclaration;    
        }

        /// <summary>
        /// Is this an array type definition?
        /// </summary>
        public bool IsArrayDeclaration
        {
            get { return _arrayDeclaration != null; }
        }

        /// <summary>
        /// Returns an array of unresolved generic arguments types.
        /// </summary>
        /// <remarks>
        /// <p>
        /// A empty string represents a type parameter that 
        /// did not have been substituted by a specific type.
        /// </p>
        /// </remarks>
        /// <returns>
        /// An array of strings that represents the unresolved generic 
        /// arguments types or an empty array if not generic.
        /// </returns>
        public string[] GetGenericArguments()
        {
            return _unresolvedGenericArguments ?? Enumerable.Empty<string>().ToArray();
        }

        private void ParseGenericTypeDeclaration(string originalString)
        {
            if (originalString.IndexOf('[') == -1 && originalString.IndexOf('<') == -1)
            {
                // nothing to do
                _unresolvedGenericTypeName = originalString;
                _unresolvedGenericMethodName = originalString;
                return;
            }

            originalString = originalString.Trim();

            var isClrStyleNotation = originalString.IndexOf('`') > -1;

            var match = (isClrStyleNotation)
                        ? ClrPattern.Match(originalString)
                        : CSharpPattern.Match(originalString);

            if (!match.Success)
            {
                _unresolvedGenericTypeName = originalString;
                _unresolvedGenericMethodName = originalString;
                return;
            }

            var @group = match.Groups["args"];
            _unresolvedGenericArguments = ParseGenericArgumentList(@group.Value);

            string name = match.Groups["name"].Value;
            string remainder = match.Groups["remainder"].Value.Trim();

            // check, if we're dealing with an array type declaration
            if (remainder.Length > 0 && remainder.IndexOf('[') > -1)
            {
                string[] remainderParts = Split(remainder, ",", false, false, "[]");
                string arrayPart = remainderParts[0].Trim();
                if (arrayPart[0] == '[' && arrayPart[arrayPart.Length-1] == ']')
                {
                    _arrayDeclaration = arrayPart;
                    remainder = ", " + string.Join(",", remainderParts, 1, remainderParts.Length - 1);
                }               
            }
            
            _unresolvedGenericMethodName = name + remainder;
            _unresolvedGenericTypeName = name + "`" + _unresolvedGenericArguments.Length + remainder;
        }

        private static string[] ParseGenericArgumentList(string originalArgs)
        {
            string[] args = Split(originalArgs, ",", true, false, "[]<>"     );
            // remove quotes if necessary
            for(int i=0;i<args.Length;i++)
            {
                string arg = args[i];
                if (arg.Length > 1 && arg[0] == '[')
                {
                    args[i] = arg.Substring(1, arg.Length - 2);
                }
            }
            return args;
        }

        /// <summary>
        /// Tokenize the given <see cref="System.String"/> into a
        /// <see cref="System.String"/> array.
        /// </summary>
        /// <remarks>
        /// <p>
        /// If <paramref name="s"/> is <see langword="null"/>, returns an empty
        /// <see cref="System.String"/> array.
        /// </p>
        /// <p>
        /// If <paramref name="delimiters"/> is <see langword="null"/> or the empty
        /// <see cref="System.String"/>, returns a <see cref="System.String"/> array with one
        /// element: <paramref name="s"/> itself.
        /// </p>
        /// </remarks>
        /// <param name="s">The <see cref="System.String"/> to tokenize.</param>
        /// <param name="delimiters">
        /// The delimiter characters, assembled as a <see cref="System.String"/>.
        /// </param>
        /// <param name="trimTokens">
        /// Trim the tokens via <see cref="System.String.Trim()"/>.
        /// </param>
        /// <param name="ignoreEmptyTokens">
        /// Omit empty tokens from the result array.
        /// </param>
        /// <param name="quoteChars">
        /// Pairs of quote characters. <paramref name="delimiters"/> within a pair of quotes are ignored
        /// </param>
        /// <returns>An array of the tokens.</returns>
        public static string[] Split(
            string s, string delimiters, bool trimTokens, bool ignoreEmptyTokens, string quoteChars)
        {
            if (s == null)
            {
                return new string[0];
            }
            if (string.IsNullOrEmpty(delimiters))
            {
                return new string[] { s };
            }
            if (quoteChars == null)
            {
                quoteChars = string.Empty;
            }

            if (quoteChars.Length % 2 != 0)
                throw new Exception("the number of quote characters must be even");

            var delimiterChars = delimiters.ToCharArray();

            // scan separator positions
            var delimiterPositions = new int[s.Length];
            int count = MakeDelimiterPositionList(s, delimiterChars, quoteChars, delimiterPositions);

            List<string> tokens = new List<string>(count + 1);
            int startIndex = 0;
            for (int ixSep = 0; ixSep < count; ixSep++)
            {
                string token = s.Substring(startIndex, delimiterPositions[ixSep] - startIndex);
                if (trimTokens)
                {
                    token = token.Trim();
                }
                if (!(ignoreEmptyTokens && token.Length == 0))
                {
                    tokens.Add(token);
                }
                startIndex = delimiterPositions[ixSep] + 1;
            }
            // add remainder            
            if (startIndex < s.Length)
            {
                string token = s.Substring(startIndex);
                if (trimTokens)
                {
                    token = token.Trim();
                }
                if (!(ignoreEmptyTokens && token.Length == 0))
                {
                    tokens.Add(token);
                }
            }
            else if (startIndex == s.Length)
            {
                if (!(ignoreEmptyTokens))
                {
                    tokens.Add(string.Empty);
                }
            }

            return tokens.ToArray();
        }

        private static int MakeDelimiterPositionList(string s, char[] delimiters, string quoteChars, int[] delimiterPositions)
        {
            int count = 0;
            int quoteNestingDepth = 0;
            char expectedQuoteOpenChar = '\0';
            char expectedQuoteCloseChar = '\0';

            for (int ixCurChar = 0; ixCurChar < s.Length; ixCurChar++)
            {
                char curChar = s[ixCurChar];

                for (int ixCurDelim = 0; ixCurDelim < delimiters.Length; ixCurDelim++)
                {
                    if (delimiters[ixCurDelim] == curChar)
                    {
                        if (quoteNestingDepth == 0)
                        {
                            delimiterPositions[count] = ixCurChar;
                            count++;
                            break;
                        }
                    }

                    if (quoteNestingDepth == 0)
                    {
                        // check, if we're facing an opening char
                        for (int ixCurQuoteChar = 0; ixCurQuoteChar < quoteChars.Length; ixCurQuoteChar += 2)
                        {
                            if (quoteChars[ixCurQuoteChar] == curChar)
                            {
                                quoteNestingDepth++;
                                expectedQuoteOpenChar = curChar;
                                expectedQuoteCloseChar = quoteChars[ixCurQuoteChar + 1];
                                break;
                            }
                        }
                    }
                    else
                    {
                        // check if we're facing an expected open or close char
                        if (curChar == expectedQuoteOpenChar)
                        {
                            quoteNestingDepth++;
                        }
                        else if (curChar == expectedQuoteCloseChar)
                        {
                            quoteNestingDepth--;
                        }
                    }
                }
            }
            return count;
        }
    }
}
