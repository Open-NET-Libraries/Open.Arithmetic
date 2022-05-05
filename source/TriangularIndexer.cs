using System;
using System.Collections;
using System.Collections.Generic;

namespace Open.Arithmetic;

internal abstract class TriangularIndexerBase<T, TSource> : IReadOnlyList<T>
	where TSource : IEnumerable<T>
{
	public readonly TSource Source;
	protected readonly Func<TSource, int> Counter;

	protected TriangularIndexerBase(TSource source, Func<TSource, int> counter)
	{
		Source = source ?? throw new ArgumentNullException(nameof(source));
		Counter = counter ?? throw new ArgumentNullException(nameof(counter));
		var count = counter(source);
		if (count > Triangular.MaxInt32)
			throw new ArgumentException($"Source collection is too large to be indexed. {count} > {Triangular.MaxInt32} (max size)", nameof(source));
	}

	protected int AssertValidCount()
	{
		var count = Counter(Source);
		return count > Triangular.MaxInt32
			? throw new NotSupportedException($"Source collection has growns too large to be indexed. {count} > {Triangular.MaxInt32} (max size)")
			: count;
	}

	public abstract T this[int index] { get; }

	private int _sourceCount = -1;
	private int _count = -1;
	public int Count => GetCounts().Triangular;

	public (int Source, int Triangular) GetCounts()
	{
		var c = AssertValidCount();
		var count = _count;
		var source = _sourceCount;
		if (count == -1 || source != c)
		{
			count = _count = Triangular.ForwardInt(c);
			source = _sourceCount = c;
		}
		return (source, count);
	}

	protected abstract int GetSourceIndex(int index);

	public abstract IEnumerator<T> GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal abstract class IncreasingTriangularIndexer<T, TSource> : TriangularIndexerBase<T, TSource>
	where TSource : IEnumerable<T>
{
	protected IncreasingTriangularIndexer(TSource source, Func<TSource, int> counter)
		: base(source, counter)	{ }

	public override IEnumerator<T> GetEnumerator()
		=> Triangular.Disperse.Increasing(Source).GetEnumerator();

	protected override int GetSourceIndex(int index)
		=> Triangular.Reverse(index);
}

internal abstract class DecreasingTriangularIndexer<T, TSource> : TriangularIndexerBase<T, TSource>
	where TSource : IEnumerable<T>
{
	protected DecreasingTriangularIndexer(TSource source, Func<TSource, int> counter)
		: base(source, counter) { }

	public override IEnumerator<T> GetEnumerator()
		=> Triangular.Disperse.Decreasing(Source, Count).GetEnumerator();

	protected override int GetSourceIndex(int index)
	{
		var (source, triangular) = GetCounts();
		return source - Triangular.Reverse(triangular - index);
	}
}

internal class IncreasingTriangularIndexer<T> : IncreasingTriangularIndexer<T, IList<T>>
{
	public IncreasingTriangularIndexer(IList<T> source)
		: base(source, list => list.Count) { }

	public override T this[int index]
		=> Source[GetSourceIndex(index)];
}

internal class IncreasingReadOnlyTriangularIndexer<T> : IncreasingTriangularIndexer<T, IReadOnlyList<T>>
{
	public IncreasingReadOnlyTriangularIndexer(IReadOnlyList<T> source)
		: base(source, list => list.Count) { }

	public override T this[int index]
		=> Source[GetSourceIndex(index)];
}

internal class DecreasingTriangularIndexer<T> : DecreasingTriangularIndexer<T, IList<T>>
{
	public DecreasingTriangularIndexer(IList<T> source)
		: base(source, list => list.Count) { }

	public override T this[int index]
		=> Source[GetSourceIndex(index)];
}

internal class DecreasingReadOnlyTriangularIndexer<T> : DecreasingTriangularIndexer<T, IReadOnlyList<T>>
{
	public DecreasingReadOnlyTriangularIndexer(IReadOnlyList<T> source)
		: base(source, list => list.Count) { }

	public override T this[int index]
		=> Source[GetSourceIndex(index)];
}

public static class TriangularIndexer
{
	public static IReadOnlyList<T> Increasing<T>(IList<T> source)
		=> new IncreasingTriangularIndexer<T>(source);
	public static IReadOnlyList<T> Decreasing<T>(IList<T> source)
		=> new DecreasingTriangularIndexer<T>(source);
	public static IReadOnlyList<T> Increasing<T>(IReadOnlyList<T> source)
		=> new IncreasingReadOnlyTriangularIndexer<T>(source);
	public static IReadOnlyList<T> Decreasing<T>(IReadOnlyList<T> source)
		=> new DecreasingReadOnlyTriangularIndexer<T>(source);
}
