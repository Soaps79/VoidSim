using System;
using System.Collections.Generic;
using Messaging;
using UnityEngine;


namespace QGame
{
	public class Locator
	{
	    private static Dictionary<Type, object> _registeredObjects;

	    public static void Register<T>(object obj)
	    {
	        if(_registeredObjects.ContainsKey(typeof(T)))
                throw new UnityException(string.Format("Locator given second instance of {0}", typeof (T)));

            _registeredObjects.Add(typeof(T), obj);
	    }

	    public static T Get<T>()
	    {
	        return (T)_registeredObjects[typeof(T)];
	    }
	}
}