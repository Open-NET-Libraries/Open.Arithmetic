﻿/*!
 * @author electricessence / https://github.com/electricessence/
 * Licensing: MIT https://github.com/electricessence/Genetic-Algorithm-Platform/blob/master/LICENSE.md
 */

using Open.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Open.Arithmetic
{

	public static class Triangular
	{
		public static readonly short MaxInt16 = Reverse(short.MaxValue);
		public static readonly ushort MaxUInt16 = Reverse(ushort.MaxValue);
		public static readonly int MaxInt32 = Reverse(int.MaxValue);
		public static readonly uint MaxUInt32 = Reverse(uint.MaxValue);
		public static readonly long MaxInt64 = Reverse(long.MaxValue);
		public static readonly ulong MaxUInt64 = Reverse(ulong.MaxValue);

		/// <summary>
		/// Returns the total count of triangular numbers bound by n.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Must be at least zero.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Result will exceed maximum value for a 32 bit integer.</exception>
		public static int ForwardInt(int n)
		{
			if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), n, "Must be at least zero.");
			if (n > MaxInt32) throw new ArgumentOutOfRangeException(nameof(n), n, "Result will exceed maximum value for a 32 bit integer.");
			Contract.EndContractBlock();

			return (int)Forward(n);
		}

		/// <summary>
		/// Returns the total count of triangular numbers bound by n.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Must be at least zero.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Result will exceed maximum value for an unsigned 32 bit integer.</exception>
		public static uint ForwardInt(uint n)
		{
			if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), n, "Must be at least zero.");
			if (n > MaxUInt32) throw new ArgumentOutOfRangeException(nameof(n), n, "Result will exceed maximum value for an unsigned 32 bit integer.");
			Contract.EndContractBlock();

			return (uint)Forward(n);
		}

		/// <summary>
		/// Returns the total count of triangular numbers bound by n.
		/// </summary>
		public static ulong Forward(long n)
		{
			if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), n, "Must be at least zero.");
			Contract.EndContractBlock();

			return Forward((ulong)n);
		}

		/// <summary>
		/// Returns the total count of triangular numbers bound by n.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Result will exceed maximum value for an unsigned 64 bit integer.</exception>
		public static ulong Forward(ulong n)
		{
			if (n > MaxUInt64) throw new ArgumentOutOfRangeException(nameof(n), n, "Result will exceed maximum value for an unsigned 64 bit integer.");
			Contract.EndContractBlock();

			return n * (n + 1) / 2;
		}

		static double ReverseInternal(double n)
			=> (Math.Sqrt(8 * n + 1) - 1) / 2;

		/// <summary>
		/// Returns the source value (possible decimal) from an index in a triangular set.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Cannot be NaN.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Must be finite.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Must be at least zero.</exception>
		public static double Reverse(double n)
		{
			if (double.IsNaN(n)) throw new ArgumentOutOfRangeException(nameof(n), n, "Cannot be NaN.");
			if (double.IsInfinity(n)) throw new ArgumentOutOfRangeException(nameof(n), n, "Must be finite.");
			if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), n, "Must be at least zero.");
			Contract.EndContractBlock();

			return ReverseInternal(n);
		}

		/// <summary>
		/// Returns the source index from an index in a triangular set.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Must be at least zero.</exception>
		public static ulong Reverse(ulong n)
			=> (ulong)ReverseInternal(n);

		/// <summary>
		/// Returns the source index from an index in a triangular set.
		/// </summary>
		public static uint Reverse(uint n)
			=> (uint)ReverseInternal(n);

		/// <summary>
		/// Returns the source index from an index in a triangular set.
		/// </summary>
		public static short Reverse(short n)
			=> (short)ReverseInternal(n);

		/// <summary>
		/// Returns the source index from an index in a triangular set.
		/// </summary>
		public static ushort Reverse(ushort n)
			=> (ushort)ReverseInternal(n);

		/// <summary>
		/// Returns the source index from an index in a triangular set.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Must be at least zero.</exception>
		public static long Reverse(long n)
		{
			if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), n, "Must be at least zero.");
			Contract.EndContractBlock();

			return (long)ReverseInternal(n);
		}

		/// <summary>
		/// Returns the source index from an index in a triangular set.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Must be at least zero.</exception>
		public static int Reverse(int n)
		{
			if (n < 0) throw new ArgumentOutOfRangeException(nameof(n), n, "Must be at least zero.");
			Contract.EndContractBlock();

			return (int)ReverseInternal(n);
		}

		public static class Disperse
		{

			/**
             * Increases the repeat of an element based on its index.
			 * 1, 2, 2, 3, 3, 3, 4, 4, 4, 4, ...
             * @param source
             * @returns {Enumerable<T>}
             */
			public static IEnumerable<T> Increasing<T>(IEnumerable<T> source)
				=> source.SelectMany((c, i) => Enumerable.Repeat(c, i));

			/**
             * Starting with the first item, increases the size of the loop of items until there is no more.
			 * The quantity/total of an item is lessened as the loop spreads the items out as it progresses.
			 * Given an entire set, the first item is represented the most and the last item is represented the least.
			 * 1, 1, 2, 1, 2, 3, 1, 2, 3, 4, 1, 2, 3, 4, 5, ...
             * @param source
             * @returns {Enumerable<T>}
             */
			public static IEnumerable<T> Decreasing<T>(IEnumerable<T> source)
			{
				var s = source.Memoize();
				return s.SelectMany((c, i) => s.Take(i));
			}

			/**
			 * Effectively the same as Disperse.Increasing but in reverse.
			 * Given a fininte set, calling Disperse.Increasing(sourse).Reverse() could produce the desired result as well.
			 * 1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 4, 4, 5
			 * @param source
			 * @returns {Enumerable<T>}
			 */
			public static IEnumerable<T> Descending<T>(IReadOnlyList<T> source)
				=> source.SelectMany((c, i) => Enumerable.Repeat(c, source.Count - i));
		}
	}

}
