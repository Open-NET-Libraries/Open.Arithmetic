/*!
 * @author electricessence / https://github.com/electricessence/
 * Licensing: MIT https://github.com/electricessence/Open/blob/dotnet-core/LICENSE.md
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Open.Formatting;

namespace Open.Arithmetic
{
	public static class MathExtensions
	{

		static void ValidateIntPower(int power)
		{
			if (power < 0)
				throw new ArgumentOutOfRangeException("power", power, "In order to maintain the interger math, power cannot be negative.");
		}

		/// <summary>
		/// Raise a number to the power of another.  Uses integer math only.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="power"></param>
		/// <returns>The value to the power.</returns>
		public static int PowerOf(this int value, int power)
		{
			ValidateIntPower(power);

			if (value == 0 || value == 1)
				return value;

			if (value == -1)
				return power % 2 == 0 ? 1 : -1;

			int v = 1;
			for (var i = 1; i < power; i++)
				v *= value;
			return v;
		}

		/// <summary>
		/// Raise a number to the power of another.  Uses integer math only.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="power"></param>
		/// <returns>The value to the power.</returns>
		public static long PowerOf(this long value, int power)
		{
			ValidateIntPower(power);

			if (value == 0L || value == 1L)
				return value;

			if (value == -1L)
				return power % 2 == 0 ? 1L : -1L;

			long v = 1L;
			for (var i = 1; i < power; i++)
				v *= value;
			return v;
		}

		/// <summary>
		/// Converts a set of booleans (0|1) to a 32 bit integer.
		/// </summary>
		/// <param name="source">A array of 32 booleans or less.</param>
		/// <returns>32 bit integer</returns>
		public static int AsInteger(this bool[] source)
		{
			if (source == null) throw new NullReferenceException();
			if (source.Length > 32) throw new ArgumentOutOfRangeException("Array cannot be greater than 32 bits.");

			int result = 0;

			for (int i = 0; i < source.Length; i++)
				if (source[i])
					result += 2.PowerOf(i);

			return result;
		}

		/// <summary>
		/// Converts a set of booleans (0|1) to a 32 bit integer.
		/// </summary>
		/// <param name="source">A array of 32 booleans or less.</param>
		/// <returns>32 bit integer</returns>
		public static long AsLong(this bool[] source)
		{
			if (source == null) throw new NullReferenceException();
			if (source.Length > 64) throw new ArgumentOutOfRangeException("Array cannot be greater than 32 bits.");

			long result = 0;

			for (int i = 0; i < source.Length; i++)
				if (source[i])
					result += 2L.PowerOf(i);

			return result;
		}

		public static double Product(this IEnumerable<double> source)
		{

			if (source == null) throw new NullReferenceException();

			var any = false;
			var result = 1d;
			foreach (var s in source)
			{
				if (double.IsNaN(s)) return double.NaN;
				any = true;
				result *= s;
			}

			return any ? result : double.NaN;
		}

		public static double AverageDouble(this IEnumerable<double> source)
		{
			double sum = 0;
			double count = 0;
			foreach (var s in source)
			{
				if (double.IsNaN(s))
					return double.NaN;
				sum += s;
				count++;
			}

			if (count == 0)
				return double.NaN;

			return sum / count;
		}

		public static double Quotient(this IEnumerable<double> source)
		{
			if (source == null) throw new NullReferenceException();

			var index = 0;
			var result = double.NaN;
			foreach (var s in source)
			{
				if (double.IsNaN(s)) return double.NaN;
				if (index == 0)
				{
					if (s == 0) return 0;
					result = s;
				}
				else
				{
					result /= s;
				}

				index++;
			}

			return result;
		}


		public static double QuotientOf(this IEnumerable<double> divisors, double numerator)
		{
			var any = false;
			var result = numerator;
			foreach (var s in divisors)
			{
				if (s == 0 || double.IsNaN(s)) return double.NaN;
				result /= s;
				any = true;
			}

			return any ? result : double.NaN;
		}

		public static double Difference(this IEnumerable<double> source)
		{
			if (source == null) throw new NullReferenceException();

			var any = false;
			var result = 0d;
			foreach (var s in source)
			{
				if (double.IsNaN(s)) return double.NaN;
				if (!any)
					result = s;
				else
					result -= s;

				any = true;

			}

			return any ? result : double.NaN;
		}



		/// <summary>
		/// Ensures addition tolerance by trimming off unexpected imprecision.
		/// </summary>
		public static double SumAccurate(this double source, double value)
		{
			var result = source + value;
			var vp = source.DecimalPlaces();
			if (vp > 15)
				return result;
			var ap = value.DecimalPlaces();
			if (ap > 15)
				return result;

			var digits = Math.Max(vp, ap);

			return Math.Round(result, digits);
		}

		/// <summary>
		/// Ensures addition tolerance by trimming off unexpected imprecision.
		/// </summary>
		public static double ProductAccurate(this double source, double value)
		{
			var result = source * value;
			var vp = source.DecimalPlaces();
			if (vp > 15)
				return result;
			var ap = value.DecimalPlaces();
			if (ap > 15)
				return result;

			var digits = Math.Max(vp, ap);

			return Math.Round(result, digits);
		}

		/// Ensures addition tolerance by using integer math.
		public static double SumUsingIntegers(this double source, double value)
		{
			var x = Math.Pow(10, Math.Max(source.DecimalPlaces(), value.DecimalPlaces()));

			var v = (long)(source * x);
			var a = (long)(value * x);
			var result = v + a;
			return (double)result / x;
		}



		public static bool IsPrime(this int n)
		{
			var a = System.Math.Abs(n);
			if (a == 2 || a == 3)
				return true;

			if (a % 2 == 0 || a % 3 == 0)
				return false;

			var divisor = 6;
			while (divisor * divisor - 2 * divisor + 1 <= a)
			{

				if (a % (divisor - 1) == 0)
					return false;

				if (a % (divisor + 1) == 0)
					return false;

				divisor += 6;

			}

			return true;

		}

		public static bool IsPrime(this double n)
		{
			return (System.Math.Floor(n) == n)
				? IsPrime((int)n)
				: false;
		}

        public static ulong NextPrime(this ulong n)
        {
            if (n == 0)
                return 0;

            while (!IsPrime(n))
            {
                if (n == ulong.MaxValue)
                    throw new Exception(string.Format("The next prime is greater than the UInt64.MaxValue of {0}.", ulong.MaxValue));

                n++;
            }

            return n;
        }

        public static int NextPrime(this int n)
		{
			if (n == 0)
				return 0;

            var sign = n < 0 ? -1 : 1;
            n = Math.Abs(n);

            while (!IsPrime(++n))
            { }

			return sign * n;
		}

		public static int NextPrime(this double a)
		{
			return NextPrime((int)a);
		}

		public static IEnumerable<int> Multiples(this int a)
		{
			yield return a < 1 ? -1 : 1;
			if (a < 1) a = Math.Abs(a);

			for (var i = 2; a != 1 && i <= a; i = i.NextPrime())
			{
				while (a % i == 0)
				{
					yield return i;
					a /= i;
				}
			}
		}


		public static IEnumerable<double> Multiples(this double a)
		{
			if (double.IsNaN(a))
			{
				yield return a;
			}
			else
			{
				yield return a < 1 ? -1 : 1;
				if (a < 1) a = Math.Abs(a);

				if (double.IsInfinity(a) || Math.Floor(a) != a)
				{
					yield return a;
				}
				else
				{
					for (var i = 2; a != 1 && i <= a; i = i.NextPrime())
					{
						while (a % i == 0)
						{
							yield return i;
							a /= i;
						}
					}
				}
			}
		}

		// returns all the primes that do not include 1 or the number itself.
		public static IEnumerable<double> DivisibleMultiples(this double a)
		{
			var m = a.Multiples().Skip(1);

			foreach (var i in m)
			{
				if (i == a) break;
				yield return i;
			}
		}

		// returns all the primes that do not include 1 or the number itself.
		public static IEnumerable<int> DivisibleMultiples(this int a)
		{
			var m = a.Multiples().Skip(1);

			foreach (var i in m)
			{
				if (i == a) break;
				yield return i;
			}
		}

	}
}


