using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QGame.Test
{
	public interface ITestService
	{
		string Name { get; set; }
	}

	public class TestService : ITestService
	{
		public string Name { get; set; }
	}

	public static class TestLocatorExtensions
	{
		public static ITestService TestService { get { return ServiceLocator.Get<ITestService>(); } }
	}

	[TestClass]
	public class LocatorTests
	{
		private ITestService _testServiceObject;

		[TestInitialize]
		public void Initialize()
		{
			_testServiceObject = new TestService();
			ServiceLocator.Register<ITestService>(_testServiceObject);
		}

		[TestCleanup]
		public void Cleanup()
		{
			ServiceLocator.Clear();
		}

		[TestMethod]
		public void TestDirectGet()
		{
			var direct = ServiceLocator.Get<ITestService>();
			Assert.AreEqual(_testServiceObject, direct);
		}

		[TestMethod]
		public void TestIndirectGet()
		{
			var indirect = TestLocatorExtensions.TestService;
			Assert.AreEqual(_testServiceObject, indirect);
		}

		[TestMethod]
		public void TestNullGet()
		{
			ServiceLocator.Clear();
			var direct = ServiceLocator.Get<ITestService>();
			Assert.IsNull(direct);
		}

		[TestMethod]
		public void TestReplaceService()
		{
			var existing = ServiceLocator.Get<ITestService>();
			var newTestService = new TestService();
			ServiceLocator.Register<ITestService>(newTestService);
			var direct = ServiceLocator.Get<ITestService>();

			// test that initial fetch was initial registered
			Assert.AreEqual(_testServiceObject, existing);
			// and that newly registered is now returned as current
			Assert.AreEqual(newTestService, direct);
		}
	}
}