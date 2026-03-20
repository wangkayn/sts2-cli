// Replacements for compiler-generated readonly collection wrappers.
// The decompiled versions reference InlineArray fields that don't compile.

using System.Collections;
using System.Runtime.CompilerServices;

[CompilerGenerated]
internal sealed class _003C_003Ez__ReadOnlyArray<T> : IReadOnlyList<T>, IList<T>
{
    private readonly T[] _items;

    public _003C_003Ez__ReadOnlyArray(T[] items) => _items = items;

    public int Count => _items.Length;
    public bool IsReadOnly => true;

    public T this[int index]
    {
        get => _items[index];
        set => throw new NotSupportedException();
    }

    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_items).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    public int IndexOf(T item) => Array.IndexOf(_items, item);
    public void Insert(int index, T item) => throw new NotSupportedException();
    public void RemoveAt(int index) => throw new NotSupportedException();
    public void Add(T item) => throw new NotSupportedException();
    public void Clear() => throw new NotSupportedException();
    public bool Contains(T item) => Array.IndexOf(_items, item) >= 0;
    public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
    public bool Remove(T item) => throw new NotSupportedException();
}

[CompilerGenerated]
internal sealed class _003C_003Ez__ReadOnlySingleElementList<T> : IReadOnlyList<T>, IList<T>
{
    private readonly T _item;

    public _003C_003Ez__ReadOnlySingleElementList(T item) => _item = item;

    public int Count => 1;
    public bool IsReadOnly => true;

    public T this[int index]
    {
        get => index == 0 ? _item : throw new IndexOutOfRangeException();
        set => throw new NotSupportedException();
    }

    public IEnumerator<T> GetEnumerator()
    {
        yield return _item;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public int IndexOf(T item) => EqualityComparer<T>.Default.Equals(_item, item) ? 0 : -1;
    public void Insert(int index, T item) => throw new NotSupportedException();
    public void RemoveAt(int index) => throw new NotSupportedException();
    public void Add(T item) => throw new NotSupportedException();
    public void Clear() => throw new NotSupportedException();
    public bool Contains(T item) => EqualityComparer<T>.Default.Equals(_item, item);
    public void CopyTo(T[] array, int arrayIndex) => array[arrayIndex] = _item;
    public bool Remove(T item) => throw new NotSupportedException();
}

[CompilerGenerated]
internal sealed class _003C_003Ez__ReadOnlyList<T> : IReadOnlyList<T>, IList<T>
{
    private readonly List<T> _items;

    public _003C_003Ez__ReadOnlyList(List<T> items) => _items = items;

    public int Count => _items.Count;
    public bool IsReadOnly => true;

    public T this[int index]
    {
        get => _items[index];
        set => throw new NotSupportedException();
    }

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    public int IndexOf(T item) => _items.IndexOf(item);
    public void Insert(int index, T item) => throw new NotSupportedException();
    public void RemoveAt(int index) => throw new NotSupportedException();
    public void Add(T item) => throw new NotSupportedException();
    public void Clear() => throw new NotSupportedException();
    public bool Contains(T item) => _items.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
    public bool Remove(T item) => throw new NotSupportedException();
}
