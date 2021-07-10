using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Open.Arithmetic
{
	public static class Statistical
	{

		static double Variance(int n, double sum, double sum2, bool sample)
		{

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
		/// Calculates the Variance of a set of numbers
		/// </summary>
		/// <param name="source">The set of numbers</param>
		/// <param name="sample">If true, will divide by (n-1) (population).  If false, will divide by (n) (sample).</param>
		/// <returns>The variance of a set of numbers.</returns>
		public static double Variance(this in ReadOnlySpan<double> source, bool sample = false)
		{
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

			return Variance(n, sum, sum2, sample);
		}

		/// <inheritdoc cref="Variance(in ReadOnlySpan{double}, bool)"/>
		public static double Variance(this in Span<double> source, bool sample = false)
		{
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

			return Variance(n, sum, sum2, sample);
		}

		/// <inheritdoc cref="Variance(in ReadOnlySpan{double}, bool)"/>
		public static double Variance(this IEnumerable<double> source, bool sample = false)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
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

			return Variance(n, sum, sum2, sample);
		}

		/// <summary>
		/// Returns a sum of a sequence of the products of related entries.
		/// </summary>
		/// <param name="source">The first sequence's values.</param>
		/// <param name="target">The second sequence's values.</param>
		/// <returns>The resultant sum of the product of each related entry.</returns>
		public static double ProductsSum(this in ReadOnlySpan<double> source, in ReadOnlySpan<double> target)
		{
			var len = source.Length;
			if (len != target.Length) throw new ArgumentException("Products: source and target enumerations have different counts.");
			double sum = 0;
			for (var i = 0; i < len; ++i) sum += source[i] * target[i];
			return sum;
		}

		/// <inheritdoc cref="ProductsSum(ReadOnlySpan{double}, ReadOnlySpan{double})"/>
		public static double ProductsSum(this in Span<double> source, in ReadOnlySpan<double> target)
		{
			var len = source.Length;
			if (len != target.Length) throw new ArgumentException("Products: source and target enumerations have different counts.");
			double sum = 0;
			for (var i = 0; i < len; ++i) sum += source[i] * target[i];
			return sum;
		}

		/// <summary>
		/// Returns a sequence of the products of related entries.
		/// </summary>
		/// <param name="source">The first sequence's values.</param>
		/// <param name="target">The second sequence's values.</param>
		/// <returns>The resultant product of each related entry.</returns>
		public static IEnumerable<double> Products(this IEnumerable<double> source, IEnumerable<double> target)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
			if (target is null)
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

		static (double, double, int) DualSums(in ReadOnlySpan<double> source, in ReadOnlySpan<double> target, bool sample)
		{

			var count = source.Length;
			if (target.Length != count)
				throw new ArgumentException("Covariance: source and target enumerations have different counts.");

			if (count < 1 || sample && count < 2)
				throw new ArgumentException("Covariance: not enough entries for calculation.");

			double s = 0, t = 0;
			for (var i = 0; i < count; ++i)
			{
				s += source[i];
				t += target[i];
			}

			return (s, t, count);
		}

		static (double, double, int) DualSums(IEnumerable<double> source, IEnumerable<double> target, bool sample)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
			if (target is null)
				throw new ArgumentNullException(nameof(target));
			Contract.EndContractBlock();

			if (source is IReadOnlyCollection<double> sc && target is IReadOnlyCollection<double> tc)
			{
				var len = sc.Count;
				if (tc.Count != len)
					throw new ArgumentException("Covariance: source and target enumerations have different counts.");

				if (len < 1 || sample && len < 2)
					throw new ArgumentException("Covariance: not enough entries for calculation.");
			}

			using var sourceEnumerator = source.GetEnumerator();
			using var targetEnumerator = target.GetEnumerator();
			var count = 0;
			double s = 0, t = 0;
			while (true)
			{
				var sv = sourceEnumerator.MoveNext();
				var tv = targetEnumerator.MoveNext();

				if (sv != tv)
					throw new Exception("Covariance: source and target enumerations have different counts.");

				if (!sv)
					break;

				++count;
				s += sourceEnumerator.Current;
				t += targetEnumerator.Current;
			}

			if (count < 1 || sample && count < 2)
				throw new ArgumentException("Covariance: not enough entries for calculation.");

			return (s, t, count);
		}

		static double Covariance(int n, double sumA, double sumB, double prod, bool sample)
		{
			if (!sample)
				return prod / n - sumA * sumB / (n * n);

			var n2 = n - 1;
			return prod / n2 - sumA * sumB / (n * n2);
		}

		/// <summary>
		/// Calculates the Variance of a set of numbers
		/// </summary>
		/// <param name="source">The first sequence's values.</param>
		/// <param name="target">The second sequence's values.</param>
		/// <param name="sample">If true, will divide by (n-1) (population).  If false, will divide by (n) (sample).</param>
		/// <returns>The variance of a set of numbers.</returns>
		public static double Covariance(this in ReadOnlySpan<double> source, in ReadOnlySpan<double> target, bool sample = false)
		{
			var (sumA, sumB, n) = DualSums(in source, in target, sample);

			var prod = ProductsSum(in source, in target);

			return Covariance(n, sumA, sumB, prod, sample);
		}

		/// <inheritdoc cref="Covariance(IEnumerable{double}, IEnumerable{double}, bool)"/>
		public static double Covariance(this IEnumerable<double> source, IEnumerable<double> target, bool sample = false)
		{
			var (sumA, sumB, n) = DualSums(source, target, sample);

			var prod = source.Products(target).Sum();

			return Covariance(n, sumA, sumB, prod, sample);
		}

		// const string CORRELATION_FATAL = "FATAL: Covariance ({0}) must be less than or equal to its denominator. √({1} * {2}) = {3}";

		/// <summary>
		/// Caclculates the correlation value from the covariance, source variance, and target variance values.
		/// </summary>
		public static double Correlation(double covariance, double sourceVariance, double targetVariance)
		{
			if (double.IsNaN(targetVariance)
			|| double.IsNaN(sourceVariance)
			|| double.IsNaN(targetVariance))
				return double.NaN;

			// Perfect correlation.  Essentially 0/0 is 1.
			if (covariance == 0 && sourceVariance == 0 && targetVariance == 0) return 1;

			// Cannot divide by zero.
			if (sourceVariance == 0 || targetVariance == 0) return double.NaN;

			// Obvious result.
			if (covariance == 0) return 0;

			var m = sourceVariance * targetVariance;
			if (m < 0) return double.NaN; // Cannot root a negative number (unless your a mathemetician).

			return covariance / Math.Sqrt(sourceVariance * targetVariance);
		}

		/// <inheritdoc cref="Correlation(double, double, double)"/>
		public static double Correlation(double covariance, IEnumerable<double> source, IEnumerable<double> target)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
			if (target is null)
				throw new ArgumentNullException(nameof(target));
			Contract.EndContractBlock();

			return Correlation(covariance, source.Variance(), target.Variance());
		}

		/// <inheritdoc cref="Correlation(double, double, double)"/>
		public static double Correlation(double covariance, in ReadOnlySpan<double> source, in ReadOnlySpan<double> target)
			=> Correlation(covariance, Variance(in source), Variance(in target));

		public static double Correlation(this in ReadOnlySpan<double> source, in ReadOnlySpan<double> target)
			=> Correlation(Covariance(source, target), in source, in target);

		public static double Correlation(this in Span<double> source, in ReadOnlySpan<double> target)
			=> Correlation((ReadOnlySpan<double>)source, in target);

		public static double Correlation(this IEnumerable<double> source, IEnumerable<double> target)
		{
			if (source is null)
				throw new ArgumentNullException(nameof(source));
			if (target is null)
				throw new ArgumentNullException(nameof(target));
			Contract.EndContractBlock();

			var sourceList = source as IReadOnlyCollection<double> ?? source.ToArray();
			var targetList = target as IReadOnlyCollection<double> ?? target.ToArray();

			return Correlation(sourceList.Covariance(targetList), sourceList, targetList);
		}
	}
}
