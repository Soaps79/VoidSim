﻿using System.Collections.Generic;
using System.Collections;
using System;

namespace UIWidgets
{
	/// <summary>
	/// Linked HashSet.
	/// </summary>
	public sealed class LinkedHashSet<T> : ICollection<T>
	{
		readonly LinkedList<T> list;

		readonly Dictionary<T,LinkedListNode<T>> dict;

		/// <summary>
		/// Initializes a new instance of the LinkedHashSet&lt;T&gt;.
		/// </summary>
		public LinkedHashSet()
		{
			list = new LinkedList<T>();
			dict = new Dictionary<T, LinkedListNode<T>>();
		}

		/// <summary>
		/// Initializes a new instance of the LinkedHashSet&lt;T&gt; class.
		/// </summary>
		/// <param name="source">Source.</param>
		public LinkedHashSet(IEnumerable<T> source)
		{
			list = new LinkedList<T>();
			dict = new Dictionary<T, LinkedListNode<T>>();
			source.ForEach(Add);
		}

		/// <summary>
		/// Get items.
		/// </summary>
		public List<T> Items()
		{
			return new List<T>(list);
		}

		/// <summary>
		/// Get first item.
		/// </summary>
		public T First()
		{
			return list.First.Value;
		}

		/// <summary>
		/// Get last item.
		/// </summary>
		public T Last()
		{
			return list.Last.Value;
		}

		/// <summary>
		/// Removes all elements that match the conditions defined by the specified predicate from a LinkedHashSet&lt;T&gt; collection.
		/// </summary>
		/// <returns>The number of elements that were removed from the LinkedHashSet&lt;T&gt; collection.</returns>
		/// <param name="match">The Predicate&lt;T&gt; delegate that defines the conditions of the elements to remove.</param>
		public int RemoveWhere(Predicate<T> match)
		{
			var result = 0;

			var keys = new List<T>(dict.Keys);
			foreach (var item in keys)
			{
				if (match(item))
				{
					Remove(item);
					result += 1;
				}
			}

			return result;
		}

		#region ICollection implementation
		/// <summary>
		/// Adds an object to the LinkedHashSet&lt;T&gt;.
		/// </summary>
		/// <param name="item">The object to add to the LinkedHashSet&lt;T&gt;.</param>
		public void Add(T item)
		{
			if (!dict.ContainsKey(item))
			{
				list.AddLast(item);
				dict.Add(item, list.Last);
			}
		}

		/// <summary>
		/// Removes all items from the LinkedHashSet&lt;T&gt;.
		/// </summary>
		public void Clear()
		{
			dict.Clear();
			list.Clear();
		}

		/// <summary>
		/// Determines whether the LinkedHashSet&lt;T&gt; contains a specific value.
		/// </summary>
		/// <returns>true if item is found in the LinkedHashSet&lt;T&gt;; otherwise, false.</returns>
		/// <param name="item">The object to locate in the LinkedHashSet&lt;T&gt;.</param>
		public bool Contains(T item)
		{
			return dict.ContainsKey(item);
		}

		/// <summary>
		/// Copies the elements of the LinkedHashSet&lt;T&gt; to an Array, starting at a particular Array index.
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the elements copied from LinkedHashSet&lt;T&gt;. The Array must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the LinkedHashSet&lt;T&gt;.
		/// </summary>
		/// <returns><c>true</c> if the element is successfully found and removed; otherwise, <c>false</c>. This method returns false if item is not found in the LinkedHashSet&lt;T&gt; object.</returns>
		/// <param name="item">The object to remove from the LinkedHashSet&lt;T&gt;.</param>
		public bool Remove(T item)
		{
			if (dict.ContainsKey(item))
			{
				list.Remove(dict[item]);
				dict.Remove(item);

				return true;
			}

			return false;
		}

		/// <summary>
		/// Gets the number of elements contained in the LinkedHashSet&lt;T&gt;.
		/// </summary>
		/// <value>The number of elements contained in the LinkedHashSet&lt;T&gt;.</value>
		public int Count {
			get {
				return dict.Count;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the LinkedHashSet&lt;T&gt; is read only.
		/// </summary>
		/// <value><c>true</c> if the LinkedHashSet&lt;T&gt; is read only; otherwise, <c>false</c>.</value>
		public bool IsReadOnly {
			get {
				return false;
			}
		}
		#endregion

		#region IEnumerable implementation
		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return list.GetEnumerator();
		}
		#endregion

		#region IEnumerable implementation
		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>An enumerator that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return (list as IEnumerable).GetEnumerator();
		}
		#endregion
	}
}

