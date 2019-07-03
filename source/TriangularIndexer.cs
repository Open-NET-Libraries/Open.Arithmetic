using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Open.Arithmetic
{
	public class TriangularIndexer<T> : IReadOnlyList<T>
	{
		public readonly IList<T> Source;
		public readonly bool IsDescending;

		public TriangularIndexer(IList<T> source, bool descending = false)
		{
			Source = source ?? throw new ArgumentNullException(nameof(source));
			if (source.Count > Triangular.MaxInt32)
				throw new ArgumentException($"Source collection is too large to be indexed.  Max size: {Triangular.MaxInt32}", nameof(source));
			Contract.EndContractBlock();

			IsDescending = descending;
		}

		public T this[int index]
			=> IsDescending
			? Source[Source.Count - Triangular.Reverse(Count - index)]
			: Source[Triangular.Reverse(index)];

		public int Count
			=> Triangular.ForwardInt(Source.Count);

		public IEnumerator<T> GetEnumerator()
		{
			var source = IsDescending
				? Triangular.Disperse.Descending(Source)
				: Triangular.Disperse.Increasing(Source);

			foreach (var e in source)
				yield return e;
		}

		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();
	}

	public static class TriangularIndexer
	{
		public static TriangularIndexer<T> Increasing<T>(IList<T> source)
			=> new TriangularIndexer<T>(source);
		public static TriangularIndexer<T> Decreasing<T>(IList<T> source)
			=> new TriangularIndexer<T>(source, true);
	}
}
