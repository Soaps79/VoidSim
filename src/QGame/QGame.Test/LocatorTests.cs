using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QGame.Test
{
	public interface ITest
	{
		string Name { get; set; }
	}

	public class Test : ITest
	{
		public string Name { get; set; }
	}

	public static class TestLocatorExtensions
	{
		public static ITest Test { get { return ServiceLocator.Get<ITest>(); } }
	}

	[TestClass]
	public class LocatorTests
	{
		private ITest _testObject;

		[TestInitialize]
		public void Initialize()
		{
			_testObject = new Test();
			ServiceLocator.Register<ITest>(_testObject);
		}

		[TestCleanup]
		public void Cleanup()
		{
			ServiceLocator.Clear();
		}

		[TestMethod]
		public void TestDirectGet()
		{
			var direct = ServiceLocator.Get<ITest>();
			Assert.AreEqual(_testObject, direct);
		}

		[TestMethod]
		public void TestIndirectGet()
		{
			var indirect = TestLocatorExtensions.Test;
			Assert.AreEqual(_testObject, indirect);
		}
	}
}