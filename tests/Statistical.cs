using Microsoft.VisualStudio.TestTools.UnitTesting;
using Open.Numeric.Precision;
using System;
using System.Linq;

namespace Open.Arithmetic.Tests
{
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
			Assert.IsTrue(
				expected.IsPreciseEqual(VarianceSample.Variance(), true));
			expected = VarianceSample.Select(s => Math.Pow(s - mean, 2)).Sum() / (count - 1);
			Assert.IsTrue(
				expected.IsPreciseEqual(VarianceSample.Variance(true), true));
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
	}
}
