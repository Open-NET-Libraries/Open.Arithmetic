/*!
 * @author electricessence / https://github.com/electricessence/
 * Licensing: MIT https://github.com/electricessence/Genetic-Algorithm-Platform/blob/master/LICENSE.md
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Open.Collections;

namespace Open.Arithmetic
{

    public static class Triangular
    {

        public static uint Forward(uint n)
        {
            return n * (n + 1) / 2;
        }


        public static uint Reverse(uint n)
        {
            return (uint)(Math.Sqrt(8 * n + 1) - 1) / 2;
        }

        public static class Disperse
        {

            /**
             * Increases the number an element based on it's index.
             * @param source
             * @returns {Enumerable<T>}
             */
            public static IEnumerable<T> Increasing<T>(IEnumerable<T> source)
            {
                int i = 0;
                return source.SelectMany(
                    c => Enumerable.Repeat(c, Interlocked.Increment(ref i)));
            }

            /**
             * Increases the count of each element for each index.
             * @param source
             * @returns {Enumerable<T>}
             */
            public static IEnumerable<T> Decreasing<T>(IEnumerable<T> source)
            {
                var s = source.Memoize();
                int i = 0;
                return s.SelectMany(
                    c => s.Take(Interlocked.Increment(ref i)));
            }
        }
    }

}
