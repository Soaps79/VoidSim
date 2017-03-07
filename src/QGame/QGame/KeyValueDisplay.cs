using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace QGame
{
	public interface IKeyValueDisplay
	{
		string CurrentDisplayString();
		void Add(string name, ObjectCallback value);
		void Remove(string name);
	}

	internal class NullKeyValueDisplay : IKeyValueDisplay
	{
		public string CurrentDisplayString()
		{
			string s = "NullKeyValueDisplay Accessed";
			Debug.Log(s);
			return s;
		}

		public void Add(string name, ObjectCallback value)
		{
			Debug.Log("NullKeyValueDisplay Accessed");
		}

		public void Remove(string name)
		{
			Debug.Log("NullKeyValueDisplay Accessed");
		}
	}

	public class KeyValueDisplay : IKeyValueDisplay
	{
		class KVDNode
		{
			public string Name;
			public ObjectCallback Value;

			public KVDNode(string name, ObjectCallback value)
			{
				this.Name = name;
				this.Value = value;
			}
		}

		List<KVDNode> kvpList = new List<KVDNode>();

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

		//public override void DebugDraw(GameTime gameTime)
		//{
		//	// draw rectangle
		//	// set Label starting Y
		//	backgroundPanel.Draw(gameTime);

		//	foreach (KVDNode k in kvpList)
		//	{
		//		string s = k.Name + ": " + k.Value().ToString();
		//		k.Label.Text = s;

		//		k.Label.Draw(gameTime);

		//		Vector2 bgSize = backgroundPanel.Transform.Size;
		//		Vector2 size = k.Label.Font.MeasureString(s);

		//		if (bgSize.X <= size.X)
		//		{
		//			bgSize.X = size.X + 15;
		//		}
		//		else
		//		{
		//			float f = MathHelper.Distance(bgSize.X, size.X);
		//			if (f < 15)
		//			{
		//				bgSize.X = bgSize.X + 15;
		//			}
		//		}

		//		backgroundPanel.Transform.Size = bgSize;
		//	}
		//}

		//private void ResetPositions()
		//{
		//	for (int i = 0; i < kvpList.Count; i++)
		//	{
		//		int y = labelHeight * i;

		//		Vector2 tmp = kvpList[i].Label.Transform.Position;
		//		tmp.Y = this.Transform.Position.Y + y ;

		//		kvpList[i].Label.Transform.Position = tmp;

		//		tmp = backgroundPanel.Transform.Size;
		//		tmp.Y = kvpList[i].Label.Transform.Position.Y + (labelHeight + 4);

		//		backgroundPanel.Transform.Size = tmp;
		//	}
		//}

		public void Add(string name, ObjectCallback value)
		{
			//KVDNode node = kvpList.Find(
			//    delegate(KVDNode k)
			//    {
			//        return k.Name == name;
			//    }
			//);

			KVDNode node = kvpList.Find(k => k.Name == name);

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
		/// Removes a value from display
		/// </summary>
		/// <param name="name"></param>
		public void Remove(string name)
		{
			//KVDNode node = kvpList.Find(
			//    delegate (KVDNode k)
			//    {
			//        return k.Name == name;
			//    }
			//);

			var node = kvpList.Find(k => k.Name == name);

			if (node != null)
			{
				kvpList.Remove(node);
			}

		}
		#endregion
	}
}