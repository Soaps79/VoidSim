using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace QGame
{
	public interface IKeyValueDisplay
	{
		string CurrentDisplayString();
		void Add(string name, Func<object> value);
		void Remove(string name);
	}

	internal class NullKeyValueDisplay : IKeyValueDisplay
	{
		public string CurrentDisplayString()
		{
			var s = "NullKeyValueDisplay Accessed";
			Debug.Log(s);
			return s;
		}

		public void Add(string name, Func<object> value)
		{
			Debug.Log("NullKeyValueDisplay Accessed");
		}

		public void Remove(string name)
		{
			Debug.Log("NullKeyValueDisplay Accessed");
		}
	}

	public class KeyValueDisplay : SingletonBehavior<KeyValueDisplay>, IKeyValueDisplay
	{
		class KVDNode
		{
			public string Name;
			public Func<object> Value;

			public KVDNode(string name, Func<object> value)
			{
				this.Name = name;
				this.Value = value;
			}
		}

	    readonly List<KVDNode> kvpList = new List<KVDNode>();

		#region Public Interface

		public string CurrentDisplayString()
		{
			string s = "";

			foreach (var node in kvpList)
			{
				s += node.Name + ": " + node.Value() + "\n";	
			}
			
			return s;
		}

	    public void Add(string name, Func<object> value)
		{
			var node = kvpList.Find(k => k.Name == name);

			if (node == null)
			{
				node = new KVDNode(name, value);
				kvpList.Add(node);
			}
			else
			{
				node.Value = value;
			}
		}

		/// <summary>
		/// Removes a value, by name, from display
		/// </summary>
		public void Remove(string name)
		{
			var node = kvpList.Find(k => k.Name == name);

			if (node != null)
			{
				kvpList.Remove(node);
			}

		}
		#endregion
	}
}