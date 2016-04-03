using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDB.Messaging.Extensions
{
    /// <summary>
    /// <see cref="String"/> extension methods
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Compares a string against a wildcard pattern.
        /// </summary>
        /// <param name="input">The string to match.</param>
        /// <param name="mask">The wildcard pattern.</param>
        /// <returns><c>true</c> if the pattern is matched; otherwise <c>false</c></returns>
        public static bool Like(this string input, string mask)
        {
            var inputEnumerator = input.GetEnumerator();
            var maskEnumerator = mask.GetEnumerator();

            return Like(inputEnumerator, maskEnumerator);
        }

        private static bool Like(CharEnumerator inputEnumerator, CharEnumerator maskEnumerator)
        {
            while (maskEnumerator.MoveNext())
            {
                switch (maskEnumerator.Current)
                {
                    case '?':
                        if (!inputEnumerator.MoveNext())
                            return false;

                        break;
                    case '*':
                        do
                        {
                            var inputTryAhead = (CharEnumerator)inputEnumerator.Clone();
                            var maskTryAhead = (CharEnumerator)maskEnumerator.Clone();
                            if (Like(inputTryAhead, maskTryAhead))
                                return true;

                        } while (inputEnumerator.MoveNext());

                        return false;
                    case '\\': // escape 
                        maskEnumerator.MoveNext();
                        goto default;
                    default:
                        if (!inputEnumerator.MoveNext() || inputEnumerator.Current != maskEnumerator.Current)
                            return false;

                        break;
                }
            }

            return !inputEnumerator.MoveNext();
        }

    }
}
