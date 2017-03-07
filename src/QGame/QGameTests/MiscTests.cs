using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QGame;

namespace QGameTests
{
	[TestClass]
	public class MiscTests
	{
		[TestMethod]
		public void KeyValueDisplay_BasicFunctionality()
		{
			var kvd = new KeyValueDisplay();

			kvd.Add("TestHeight", () => (object)TestHeight);
			kvd.Add("TestHeight2", () => (object)TestHeight);

			var s = kvd.CurrentDisplayString();
			
			if (!string.IsNullOrEmpty(s))
			{
				int i = 1;
				i++;
			}

		}
		 
		public int TestSpeed()
		{
			return 5;
		}

		public float TestHeight
		{
			get { return 4.78f; }
		}
	}
}