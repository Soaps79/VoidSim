using System;
using System.Collections.Generic;
using UnityEngine;


namespace QGame
{
	public class ServiceLocator
	{
		private static readonly Dictionary<Type, object> _registeredObjects = new Dictionary<Type, object>();

		public static void Register<T>(object obj)
		{
			if(_registeredObjects.ContainsKey(typeof(T)))
				throw new InvalidOperationException(string.Format("ServiceLocator given second instance of {0}", typeof (T)));

			_registeredObjects.Add(typeof(T), obj);
		}

		public static T Get<T>()
		{
			return (T)_registeredObjects[typeof(T)];
		}

		public static void Clear()
		{
			_registeredObjects.Clear();
		}
	}
}