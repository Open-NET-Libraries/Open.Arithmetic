using Microsoft.VisualStudio.TestTools.UnitTesting;
using Open.Numeric.Precision;
using System;
using System.Linq;

namespace Open.Arithmetic.Tests;

[TestClass]
public class Statistical
{
	private readonly double[] VarianceSample =
	{
		5, 7, 4.2, 12, 45, 2
	};

	[TestMethod]
	public void Variance()
	{
		// https://www.wikihow.com/Calculate-Variance
		var count = VarianceSample.Length;
		var sum = VarianceSample.Sum();
		var mean = sum / count;
		var expected = VarianceSample.Select(s => Math.Pow(s - mean, 2)).Sum() / (count - 0);
		var actual = VarianceSample.Variance();
		Assert.IsTrue(expected.IsNearEqual(actual, 10));
		expected = VarianceSample.Select(s => Math.Pow(s - mean, 2)).Sum() / (count - 1);
		actual = VarianceSample.Variance(true);
		Assert.IsTrue(expected.IsNearEqual(actual, 10));
	}

	private readonly double[] CovarianceSampleX =
{
		5,20,40,80,100
	};
	private readonly double[] CovarianceSampleY =
	{
		10,24,33,54,10
	};

	[TestMethod]
	public void Covariance()
	{
		Assert.AreEqual(1502, (int)(CovarianceSampleX.Covariance(CovarianceSampleY) * 10));
		Assert.AreEqual(187.75, CovarianceSampleX.Covariance(CovarianceSampleY, true));
	}

	[TestMethod]
	public void Correlation()
	{
		{
			var same = Enumerable.Range(1, 10).Select(Convert.ToDouble);
			Assert.AreEqual(1, same.Correlation(same));
		}

		{
			var same = Enumerable.Repeat(1, 10).Select(Convert.ToDouble);
			Assert.AreEqual(1, same.Correlation(same));
		}
	}

	[TestMethod]
	public void LargeSampleSizes_DoNotOverflow()
	{
		// The n·n term overflows Int32 for any n above 46,340; at n = 2^20 it wrapped to exactly
		// zero, so variance became infinite and correlation NaN. Alternating 0/1 values make every
		// intermediate sum an exact power of two, so the expected values are exact, not approximate:
		// population variance 0.25, self-correlation 1.
		var values = new double[1 << 20];
		for (var i = 0; i < values.Length; i++)
			values[i] = i % 2;

		Assert.AreEqual(0.25, values.Variance());
		Assert.AreEqual(1, values.Correlation(values));

		// Sample (n − 1) denominators take the n·(n − 1) path.
		var sampleVariance = values.Variance(true);
		Assert.IsTrue(sampleVariance.IsNearEqual(0.25, 6));

		// The smallest overflowing count: 46,341² exceeds Int32.MaxValue.
		var threshold = new double[46_341];
		for (var i = 0; i < threshold.Length; i++)
			threshold[i] = i % 2;

		Assert.IsTrue(threshold.Variance().IsNearEqual(0.25, 6));
		Assert.AreEqual(1, threshold.Correlation(threshold));
	}
}
