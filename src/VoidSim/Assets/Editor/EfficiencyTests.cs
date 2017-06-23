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
		}

		[Test]
		public void Module_HandleSimpleValue()
		{
			const float affectorValue = 0.7f;
			var affector = new EfficiencyAffector(affectorValue);

			var triggered = 0;
			_module.OnValueChanged += module => triggered++;
			_module.RegisterAffector(affector);

			Assert.IsTrue(Math.Abs(affector.Efficiency - _module.CurrentAmount) < .01);
			Assert.AreEqual(1, triggered);
		}

		[Test]
		public void Affector_HandleWeightedValue()
		{
			// the double value should be offset by the half weight
			const float weight = .5f;
			const float value = 2.0f;
			var affector = new EfficiencyAffector(value, weight);

			_module.RegisterAffector(affector);
			
			Assert.IsTrue(Math.Abs(1 - _module.CurrentAmount) < .01);
		}

		[Test]
		public void Affector_HandleMultipleWeightedValues()
		{
			const float weight1 = .5f;
			const float value1 = 1.0f;
			var affector = new EfficiencyAffector(value1, weight1);

			const float weight2 = 1.0f;
			const float value2 = 0.5f;
			var affecto2 = new EfficiencyAffector(value2, weight2);

			var triggered = 0;
			_module.OnValueChanged += module => triggered++;

			_module.RegisterAffector(affector);
			_module.RegisterAffector(affecto2);

			Assert.IsTrue(Math.Abs(1 - _module.CurrentAmount) < .01);
			Assert.AreEqual(2, triggered);
		}
	}
}