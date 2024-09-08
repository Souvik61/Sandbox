using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Contains a set of linq extension methods
/// </summary>
public static class LinqExtensions
{
	/// <summary>
	/// Returns the index of the given element in the collection, or null if the element could not be found.
	/// </summary>
	public static int IndexOf<T>(this IEnumerable<T> collection, T element)
	{
		int index = 0;
		foreach (T val in collection)
		{
			if (val.Equals(element)) return index;
			index++;
		}

		return -1;
	}

	/// <summary>
	/// Returns the index of the given element in the collection, or null if the element could not be found.
	/// Takes a custom comparer function.
	/// </summary>
	public static int IndexOf<CollectionT, ElementT>(this IEnumerable<CollectionT> collection, ElementT element, 
		Func<CollectionT, ElementT, bool> comparer)
	{
		int index = 0;
		foreach (CollectionT val in collection)
		{
			if (comparer(val, element)) return index;
			index++;
		}

		return -1;
	}

	/// <summary>
	/// Returns the index of the first element to return true for the given predicate.
	/// </summary>
	public static int IndexOf<T>(this IEnumerable<T> collection, Predicate<T> predicate)
	{
		int index = 0;
		foreach (T val in collection)
		{
			if (predicate(val)) return index;
			index++;
		}

		return -1;
	}

	/// <summary>
	/// Inserts the given element into the collection at the given index.
	/// </summary>
	public static IEnumerable<T> Insert<T>(this IEnumerable<T> collection, int index, T element)
	{
		List<T> list = collection.ToList();
		list.Insert(index, element);
		return list;
	}

	/// <summary>
	/// Breaks the given collection into smaller chunks of the given size.
	/// </summary>
	/// <param name="chunkSize">The max number of entries in a single chunk.</param>
	public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> collection, int chunkSize)
	{
		return collection
			.Select((v, i) => new { i, v })
			.GroupBy(v => v.i / chunkSize)
			.Select(group => group.Select(v => v.v));
	}

	/// <summary>
	/// Repeats the collection the given number of times.
	/// </summary>
	public static IEnumerable<T> RepeatSequence<T>(this IEnumerable<T> collection, int count)
	{
		for (int i = 0; i < count; ++i)
		{
			foreach (T value in collection) yield return value;
		}
	}

	/// <summary>
	/// Returns a single random entry from the list.
	/// </summary>
	public static T Random<T>(this IEnumerable<T> collection)
	{
		int count = collection.Count();
		if (count == 0)
			return default;
		else
			return collection.ElementAt(UnityEngine.Random.Range(0, count));
	}

    /// <summary>
	/// Returns a single random entry from the list with a seed
	/// </summary>
	public static T Random<T>(this IEnumerable<T> collection, int _Seed)
    {
        int count = collection.Count();
        if (count == 0)
            return default;
        else
        {
            UnityEngine.Random.InitState(_Seed);
            return collection.ElementAt(UnityEngine.Random.Range(0, count));
        }
    }

    /// <summary>
    /// Return only values in a collection that are unique for the given property.
    /// </summary>
    public static IEnumerable<T> DistinctBy<T, TKey>
	(this IEnumerable<T> source, Func<T, TKey> keySelector)
	{
		HashSet<TKey> seenKeys = new HashSet<TKey>();
		foreach (T element in source)
		{
			if (seenKeys.Add(keySelector(element)))
			{
				yield return element;
			}
		}
	}
}
