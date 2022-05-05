using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Open.Arithmetic;

public static class MathExtensions
{
	static void ValidateIntPower(int power)
	{
		if (power < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(power), power,
				"In order to maintain the interger math, power cannot be negative.");
		}
	}

	/// <summary>
	/// Raise a number to the power of another.  Uses integer math only.
	/// </summary>
	/// <param name="base">The value to multiply.</param>
	/// <param name="power">The number of times to multiply by.</param>
	/// <returns>The value to the power.</returns>
	public static int PowerOf(this int @base, int power)
	{
		ValidateIntPower(power);

		var result = 1;
		while (power != 0)
		{
			if ((power & 1) == 1)
				result *= @base;
			power >>= 1;
			@base *= @base;
		}

		return result;
	}

	/// <summary>
	/// Raise a number to the power of another.  Uses integer math only.
	/// </summary>
	/// <param name="base">The value to multiply.</param>
	/// <param name="power">The number of times to multiply by.</param>
	/// <returns>The value to the power.</returns>
	public static ulong PowerOf(this ulong @base, int power)
	{
		ulong result = 1;
		while (power != 0)
		{
			if ((power & 1) == 1)
				result *= @base;
			power >>= 1;
			@base *= @base;
		}

		return result;
	}

	/// <summary>
	/// Raise a number to the power of another.  Uses integer math only.
	/// </summary>
	/// <param name="base">The value to multiply.</param>
	/// <param name="power">The number of times to multiply by.</param>
	/// <returns>The value to the power.</returns>
	public static long PowerOf(this long @base, int power)
	{
		ValidateIntPower(power);

		long result = 1;
		while (power != 0)
		{
			if ((power & 1) == 1)
				result *= @base;
			power >>= 1;
			@base *= @base;
		}

		return result;
	}

	/// <summary>
	/// Converts a set of booleans (0|1) to a 32 bit integer.
	/// </summary>
	/// <param name="source">A array of 32 booleans or less.</param>
	/// <returns>32 bit integer</returns>
	public static int AsInteger(this bool[] source)
	{
		if (source is null) throw new NullReferenceException();
		if (source.Length > 32) throw new ArgumentOutOfRangeException(nameof(source), source, "Array cannot be greater than 32 bits.");
		Contract.EndContractBlock();

		var result = 0;

		for (var i = 0; i < source.Length; i++)
		{
			if (source[i])
				result += 2.PowerOf(i);
		}

		return result;
	}

	/// <summary>
	/// Converts a set of booleans (0|1) to a 64 bit integer.
	/// </summary>
	/// <param name="source">A array of 64 booleans or less.</param>
	/// <returns>64 bit integer</returns>
	public static long AsLong(this bool[] source)
	{
		if (source is null) throw new NullReferenceException();
		if (source.Length > 64) throw new ArgumentOutOfRangeException(nameof(source), source, "Array cannot be greater than 64 bits.");
		Contract.EndContractBlock();

		long result = 0;

		for (var i = 0; i < source.Length; i++)
		{
			if (source[i])
				result += 2L.PowerOf(i);
		}

		return result;
	}

	/// <summary>
	/// Multiplies a set of numbers together.
	/// </summary>
	/// <param name="source">The source enumerable.</param>
	/// <param name="defaultValueIfNoElements">The value to return if the sequence contains no elements.</param>
	/// <returns>The resultant product.</returns>
	public static double Product(this IEnumerable<double> source,
		double defaultValueIfNoElements = double.NaN)
	{
		if (source is null)
			throw new NullReferenceException();
		Contract.EndContractBlock();

		var any = false;
		var result = 1d;
		foreach (var s in source)
		{
			if (double.IsNaN(s)) return double.NaN;
			any = true;
			result *= s;
		}

		return any ? result : defaultValueIfNoElements;
	}

	/// <summary>
	/// Sums all the values of the sequence and divides by the count of the elements. (The average of the sequence.)
	/// </summary>
	/// <param name="source">The source enumerable.</param>
	/// <param name="defaultValueIfNoElements">The value to return if the sequence contains no elements.</param>
	/// <returns>The resultant average of the sequence.</returns>
	public static double AverageDouble(this IEnumerable<double> source,
		double defaultValueIfNoElements = double.NaN)
	{
		if (source is null)
			throw new NullReferenceException();
		Contract.EndContractBlock();

		double sum = 0;
		var count = 0;
		foreach (var s in source)
		{
			if (double.IsNaN(s))
				return double.NaN;
			sum += s;
			count++;
		}

		return count == 0
			? defaultValueIfNoElements
			: sum / count;
	}

	/// <summary>
	/// Takes the first element in the sequence and divides it by the following elements.
	/// </summary>
	/// <param name="source">The source enumerable.</param>
	/// <returns>The resultant quotiient.</returns>
	public static double Quotient(this IEnumerable<double> source)
	{
		if (source is null)
			throw new NullReferenceException();
		Contract.EndContractBlock();

		var index = 0;
		var result = double.NaN;
		foreach (var s in source)
		{
			if (double.IsNaN(s)) return double.NaN;
			switch (index)
			{
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				case 0 when s == 0:
					return 0;
				case 0:
					result = s;
					break;
				default:
					// ReSharper disable once CompareOfFloatsByEqualityOperator
					if (s == 0) return double.NaN;
					result /= s;
					break;
			}

			index++;
		}

		return result;
	}

	/// <summary>
	/// Takes the numerator and divides it by the divisors.
	/// </summary>
	/// <param name="divisors">The source enumerable.</param>
	/// <param name="numerator">The value to divide.</param>
	/// <returns>The resultant quotient.</returns>
	public static double QuotientOf(this IEnumerable<double> divisors, double numerator)
	{
		if (divisors is null)
			throw new NullReferenceException();
		Contract.EndContractBlock();

		// ReSharper disable once CompareOfFloatsByEqualityOperator
		if (double.IsNaN(numerator) || double.IsInfinity(numerator) || numerator == 0)
			return numerator;

		var any = false;
		var result = numerator;
		foreach (var s in divisors)
		{
			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (s == 0 || double.IsNaN(s)) return double.NaN;
			result /= s;
			any = true;
		}

		return any ? result : double.NaN;
	}
}
