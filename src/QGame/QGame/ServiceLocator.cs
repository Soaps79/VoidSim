using System;
using System.Collections.Generic;
using UnityEngine;


namespace QGame
{
	public class ServiceLocator
	{
		private static readonly Dictionary<Type, object> _registeredObjects = new Dictionary<Type, object>();

		/// <summary>
		/// Will register new service, or replace existing instance with parameter
		/// </summary>
		public static void Register<T>(object obj) where T: class 
		{
			if (!_registeredObjects.ContainsKey(typeof(T)))
			{
				_registeredObjects.Add(typeof(T), obj);
			}
			else
			{
				_registeredObjects[typeof(T)] = obj;
				// would really like to find a way to log this, Unity calls do not work in this assembly
				// Debug.Log($"Locator given replacement instance for {typeof(T).Name}");
			}
		}

		public static T Get<T>() where T : class
		{
			if(_registeredObjects.ContainsKey(typeof(T)))
				return (T)_registeredObjects[typeof(T)];

			return null;
		}

		public static void Clear()
		{
			_registeredObjects.Clear();
		}
	}
}