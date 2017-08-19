using System;
using Assets.Station.Efficiency;
using NUnit.Framework;

namespace Assets.Editor
{
	public class EfficiencyTests
	{
		private readonly EfficiencyModule _module = new EfficiencyModule();

		[SetUp]
		public void SetUp()
		{
			
		}

		[TearDown]
		public void TearDown()
		{
			_module.Clear();
			_module.MinimumAmount = 0.0f;
		}

		[Test]
		public void Module_HandleSimpleValue()
		{
			const float affectorValue = 0.7f;
			var affector = new EfficiencyAffector("1", affectorValue);

			_module.RegisterAffector(affector);

			Assert.IsTrue(Math.Abs(affector.Efficiency - _module.CurrentAmount) < .01);
		}

		[Test]
		public void Module_HandleMinimumAmount()
		{
			const float affectorValue = 0.7f;
			const float minimumAmount = 0.8f;
			var affector = new EfficiencyAffector("1", affectorValue);

			_module.MinimumAmount = minimumAmount;
			_module.RegisterAffector(affector);

			Assert.IsTrue(Math.Abs(minimumAmount - _module.CurrentAmount) < .01);
		}

		[Test]
		public void Module_EventsTriggered()
		{
			const float value1 = 1.2f;
			var affector = new EfficiencyAffector("1", value1);

			const float value2 = .8f;
			var affector2 = new EfficiencyAffector("2", value2);

			var triggered = 0;
			_module.OnValueChanged += module => triggered++;
			_module.RegisterAffector(affector);
			_module.RegisterAffector(affector2);
			
			Assert.AreEqual(2, triggered);
		}

		[Test]
		public void Module_SingleNegativeValue_BringsDownResult()
		{
			const float value1 = 1.0f;
			var affector = new EfficiencyAffector("1", value1);

			const float value2 = .8f;
			var affector2 = new EfficiencyAffector("2", value2);

			_module.RegisterAffector(affector);
			_module.RegisterAffector(affector2);

			Assert.IsTrue(Math.Abs(value2 - _module.CurrentAmount) < .01);
		}

		[Test]
		public void Module_MultipleNegativeValues_LowestSelected()
		{
			const float value1 = .2f;
			var affector = new EfficiencyAffector("1", value1);

			const float value2 = .8f;
			var affecto2 = new EfficiencyAffector("2", value2);

			_module.RegisterAffector(affector);
			_module.RegisterAffector(affecto2);

			Assert.IsTrue(Math.Abs(value1 - _module.CurrentAmount) < .01);
		}

		[Test]
		public void Module_FullEfficiency()
		{
			const float value1 = 1.0f;
			var affector = new EfficiencyAffector("1", value1);

			const float value2 = 1.0f;
			var affecto2 = new EfficiencyAffector("2", value2);

			_module.RegisterAffector(affector);
			_module.RegisterAffector(affecto2);

			Assert.IsTrue(Math.Abs(1 - _module.CurrentAmount) < .01);
		}

		[Test]
		public void Module_StackingBonuses()
		{
			const float value1 = .25f;
			var affector = new EfficiencyAffector("1", 1 + value1);

			const float value2 = .5f;
			var affecto2 = new EfficiencyAffector("2", 1 + value2);

			_module.RegisterAffector(affector);
			_module.RegisterAffector(affecto2);

			var expected = value1 + value2;
			var actualBonus = _module.CurrentAmount - 1;
			Assert.IsTrue(Math.Abs(expected - actualBonus) < .01);
		}
	}
}