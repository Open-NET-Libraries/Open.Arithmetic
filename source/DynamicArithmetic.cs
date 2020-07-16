﻿using System;

namespace Open.Arithmetic.Dynamic
{
	/// <summary>
	/// Acts as a substitute for missing cross-type operators.
	/// </summary>
	public static class DynamicArithmetic
	{
		public static T1 AddValue<T1, T2>(this T1 a, T2 b)
			where T1 : struct, IComparable
			where T2 : struct, IComparable => (dynamic)a + b;

		public static T1 SubtractValue<T1, T2>(this T1 a, T2 b)
			where T1 : struct, IComparable
			where T2 : struct, IComparable => (dynamic)a - b;

		public static T1 MultiplyBy<T1, T2>(this T1 a, T2 b)
			where T1 : struct, IComparable
			where T2 : struct, IComparable => (dynamic)a * b;

		public static T1 DivideBy<T1, T2>(this T1 a, T2 b)
			where T1 : struct, IComparable
			where T2 : struct, IComparable => (dynamic)a / b;

		public static T PowerOf<T>(this T a, uint power)
			where T : struct, IComparable
		{
			if (power == 0)
				return (T)((dynamic)1);

			var result = a;

			for (uint i = 1; i < power; i++)
				// ReSharper disable once RedundantCast
				result *= (dynamic)a;

			return result;
		}
	}
}
