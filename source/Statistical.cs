using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Open.Arithmetic
{
	public static class Statistical
	{
		/// <summary>
		/// Calculates the Variance of a set of numbers
		/// </summary>
		/// <param name="source">The set of numbers</param>
		/// <param name="sample">If true, will divide by (n-1) (population).  If false, will divide by (n) (sample).</param>
		/// <returns>The variance of a set of numbers.</returns>
		public static double Variance(this IEnumerable<double> source, bool sample = false)
		{
			if (source == null)
				throw new NullReferenceException();
			Contract.EndContractBlock();

			// Manual calculation to be sure LINQ isn't screwing up our double precision. :(

			double sum = 0;
			double sum2 = 0;
			var n = 0;
			foreach (var s in source)
			{
				if (double.IsNaN(s))
					return double.NaN;
				sum += s;
				sum2 += s * s;
				n++;
			}

			if (n == 0)
				return double.NaN; // Avoid divide by zero.

			// (sum2 - sum * sum / n) / n // Population
			// (sum2 - sum * sum / n) / (n - 1) // Sample

			// Reduce the amount of division in order to reduce the amount of double precision error.
			if (!sample)
				return sum2 / n - sum * sum / (n * n);

			if (n == 1)
				return double.NaN; // Avoid divide by zero.

			var n1 = n - 1;
			var n2 = n * n1;

			return sum2 / n1 - sum * sum / n2;
		}

		/// <summary>
		/// Returns a sequence of the products of related entries.
		/// </summary>
		/// <param name="source">The first sequence's values.</param>
		/// <param name="target">The second sequence's values.</param>
		/// <returns>The resultant product of each related entry.</returns>
		public static IEnumerable<double> Products(this IEnumerable<double> source, IEnumerable<double> target)
		{
			if (source == null)
				throw new NullReferenceException();
			if (target == null)
				throw new ArgumentNullException(nameof(target));
			Contract.EndContractBlock();

			using var sourceEnumerator = source.GetEnumerator();
			using var targetEnumerator = target.GetEnumerator();
			while (true)
			{
				var sv = sourceEnumerator.MoveNext();
				var tv = targetEnumerator.MoveNext();

				if (sv != tv)
					throw new Exception("Products: source and target enumerations have different counts.");

				if (!sv)
					break;

				yield return sourceEnumerator.Current * targetEnumerator.Current;
			}

		}

		/// <summary>
		/// Calculates the Variance of a set of numbers
		/// </summary>
		/// <param name="source">The first sequence's values.</param>
		/// <param name="target">The second sequence's values.</param>
		/// <param name="sample">If true, will divide by (n-1) (population).  If false, will divide by (n) (sample).</param>
		/// <returns>The variance of a set of numbers.</returns>
		public static double Covariance(this IEnumerable<double> source, IEnumerable<double> target, bool sample = false)
		{
			if (source == null)
				throw new NullReferenceException();
			if (target == null)
				throw new ArgumentNullException(nameof(target));
			Contract.EndContractBlock();

			var sourceList = source as IList<double> ?? source.ToArray();
			var targetList = target as IList<double> ?? target.ToArray();

			var n = sourceList.Count;
			if (targetList.Count != n)
				throw new ArgumentException("Covariance: source and target enumerations have different counts.");

			if (n < 1 || sample && n < 2)
				throw new ArgumentException("Covariance: not enough entries for calculation.");

			var prod = sourceList.Products(targetList).Sum();
			var sumA = sourceList.Sum();
			var sumB = targetList.Sum();

			if (!sample)
				return prod / n - sumA * sumB / (n * n);

			var n2 = n - 1;
			return prod / n2 - sumA * sumB / (n * n2);
		}


		// const string CORRELATION_FATAL = "FATAL: Covariance ({0}) must be less than or equal to its denominator. √({1} * {2}) = {3}";

		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		public static double Correlation(double covariance, double sourceVariance, double targetVariance)
		{
			if (double.IsNaN(targetVariance)
			|| sourceVariance == 0
			|| targetVariance == 0
			|| double.IsNaN(sourceVariance)
			|| double.IsNaN(targetVariance))
				return double.NaN;

			if (covariance == 0) return 0;

			var m = sourceVariance * targetVariance;
			if (m < 0) return double.NaN; // Cannot root a negative number (unless your a mathemetician).

			return covariance / Math.Sqrt(sourceVariance * targetVariance);
		}

		public static double Correlation(double covariance, IEnumerable<double> source, IEnumerable<double> target)
		{
			if (source == null)
				throw new NullReferenceException();
			if (target == null)
				throw new ArgumentNullException();
			Contract.EndContractBlock();

			return Correlation(covariance, source.Variance(), target.Variance());
		}

		public static double Correlation(this IEnumerable<double> source, IEnumerable<double> target)
		{
			if (source == null)
				throw new NullReferenceException();
			if (target == null)
				throw new ArgumentNullException();
			Contract.EndContractBlock();

			var sourceList = source as IList<double> ?? source.ToArray();
			var targetList = target as IList<double> ?? target.ToArray();

			return Correlation(sourceList.Covariance(targetList), sourceList, targetList);
		}
	}
}
