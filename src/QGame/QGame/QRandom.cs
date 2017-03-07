using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

namespace QGame
{
	public class QRandom
	{
		public int NumberToLoad { get; private set; }
		private Queue<float> _floatList;

		public QRandom(int numberToLoad)
		{
			NumberToLoad = numberToLoad;
		}

		public void LoadValues()
		{
			_floatList = new Queue<float>(NumberToLoad);
			for (int i = 0; i < NumberToLoad; i++)
			{
				_floatList.Enqueue(Random.value);
			}
		}

		public float GetNextValues()
		{
			var f = _floatList.Dequeue();
			_floatList.Enqueue(f);
			return f;
		}
	}


}