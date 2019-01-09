using System.Collections.Generic;
using System.Linq;

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
            var inputEnumerator = input.AsEnumerable().GetEnumerator();
            var maskEnumerator = mask.AsEnumerable().GetEnumerator();

            return Like(inputEnumerator, maskEnumerator, input, mask);
        }

        private static bool Like(IEnumerator<char> inputEnumerator, IEnumerator<char> maskEnumerator, string input, string mask)
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
                            var inputTryAhead = input.AsEnumerable().GetEnumerator();
                            while (inputEnumerator.Current != inputTryAhead.Current && inputTryAhead.MoveNext()) ;

                            var maskTryAhead = mask.AsEnumerable().GetEnumerator();
                            while (maskEnumerator.Current != maskTryAhead.Current && maskTryAhead.MoveNext()) ;

                            if (Like(inputTryAhead, maskTryAhead, input, mask))
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