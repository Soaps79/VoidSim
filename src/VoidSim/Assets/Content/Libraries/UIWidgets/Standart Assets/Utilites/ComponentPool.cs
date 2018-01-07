using UnityEngine;
using System.Collections.Generic;

namespace UIWidgets
{
	/// <summary>
	/// Component pool.
	/// </summary>
	public class ComponentPool<T> : MonoBehaviour
		where T : MonoBehaviour
	{
		/// <summary>
		/// The cache.
		/// </summary>
		protected static Stack<T> Cache = new Stack<T>();

		/// <summary>
		/// Clones or take from cache the object original and returns the clone.
		/// </summary>
		public T Instance()
		{
			T instance;

			if (Cache.Count > 0)
			{
				instance = Cache.Pop();
				if (instance==null)
				{
					return Instance();
				}
			}
			else
			{
				instance = Instantiate(this) as T;
			}

			instance.transform.SetParent(transform.parent, false);
			instance.gameObject.SetActive(true);

			return instance;
		}

		/// <summary>
		/// Clones or take from cache the object original and returns the clone.
		/// </summary>
		/// <param name="parent">Parent.</param>
		public T Instance(Transform parent)
		{
			var instance = Instance();
			instance.transform.SetParent(parent, false);

			return instance;
		}

		/// <summary>
		/// Return current object to cache.
		/// </summary>
		public void Free()
		{
			Cache.Push(this as T);
			gameObject.SetActive(false);
		}

		/// <summary>
		/// Return current object to cache.
		/// </summary>
		/// <param name="parent">Parent.</param>
		public void Free(Transform parent)
		{
			Free();
			transform.SetParent(parent, false);
		}
	}
}