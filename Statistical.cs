using System;
using System.Collections.Generic;
using System.Linq;

namespace Open.Arithmetic
{
	public static class Statistical
	{
		public static double Variance(this IEnumerable<double> source)
		{
			if (source == null)
				throw new NullReferenceException();

			// Manual calculation to be sure LINQ isn't screwing up our double precision. :(

			double sum = 0;
			double sum2 = 0;
			int count = 0;
			foreach (var s in source)
			{
				if (double.IsNaN(s))
					return double.NaN;
				sum += s;
				sum2 += s * s;
				count++;
			}

			if (count == 0)
				return double.NaN;

			// Reduce the amount of division in order to reduce the amount of double precision error.
			return sum2 / count - sum * sum / (count * count);
		}

		public static IEnumerable<double> Products(this IEnumerable<double> source, IEnumerable<double> target)
		{
			if (source == null)
				throw new NullReferenceException();
			if (target == null)
				throw new ArgumentNullException();

			var sourceEnumerator = source.GetEnumerator();
			var targetEnumerator = target.GetEnumerator();

			while (true)
			{
				bool sv = sourceEnumerator.MoveNext();
				bool tv = targetEnumerator.MoveNext();

				if (sv != tv)
					throw new Exception("Products: source and target enumerations have different counts.");

				if (!sv || !tv)
					break;

				yield return sourceEnumerator.Current * targetEnumerator.Current;
			}
		}

		public static double Covariance(this IEnumerable<double> source, IEnumerable<double> target)
		{
			if (source == null)
				throw new NullReferenceException();
			if (target == null)
				throw new ArgumentNullException();

			return source.Products(target).Average()
				- source.Average() * target.Average();
		}


		// const string CORRELATION_FATAL = "FATAL: Covariance ({0}) must be less than or equal to its denominator. √({1} * {2}) = {3}";

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

			return covariance / System.Math.Sqrt(sourceVariance * targetVariance);
		}

		public static double Correlation(double covariance, IEnumerable<double> source, IEnumerable<double> target)
		{
			if (source == null)
				throw new NullReferenceException();
			if (target == null)
				throw new ArgumentNullException();

			return Correlation(covariance, source.Variance(), target.Variance());
		}

		public static double Correlation(this IEnumerable<double> source, IEnumerable<double> target)
		{
			if (source == null)
				throw new NullReferenceException();
			if (target == null)
				throw new ArgumentNullException();

			return Correlation(source.Covariance(target), source, target);
		}
	}
}
