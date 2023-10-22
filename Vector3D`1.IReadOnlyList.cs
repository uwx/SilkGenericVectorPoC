using System.Collections;

namespace GenericVector;

public readonly partial struct Vector3D<T> : IReadOnlyList<T>
{
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        yield return X;
        yield return Y;
        yield return Z;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<T>)this).GetEnumerator();
    }

    int IReadOnlyCollection<T>.Count => Count;
    
    /// <summary>Copies the contents of the vector into the given array, starting from index.</summary>
    /// <exception cref="ArgumentNullException">If array is null.</exception>
    /// <exception cref="RankException">If array is multidimensional.</exception>
    /// <exception cref="ArgumentOutOfRangeException">If index is greater than end of the array or index is less than zero.</exception>
    /// <exception cref="ArgumentException">If number of elements in source vector is greater than those available in destination array.</exception>
    public void CopyTo(T[]? array, int index)
    {
        ArgumentNullException.ThrowIfNull(array, nameof(array));

        if ((index < 0) || (index >= array.Length))
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Specified argument was out of the range of valid values.");
        }
        
        ArgumentOutOfRangeException.ThrowIfLessThan((array.Length - index), 3, nameof(array));
        
        Components.CopyTo(array.AsSpan(index));
    }

}