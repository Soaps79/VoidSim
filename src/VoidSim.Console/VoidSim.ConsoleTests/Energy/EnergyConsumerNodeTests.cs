using Microsoft.VisualStudio.TestTools.UnitTesting;
using VoidSim.Console.Energy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidSim.Console.Energy.Tests
{
    [TestClass]
    public class EnergyConsumerNodeTests
    {
        [TestMethod]
        public void AddChild_UpdateTotal()
        {
            const float amount = 20;
            var childValues = new float[] {5, 10, 15};
            var consumer = new EnergyConsumerNode { AmountConsumed = amount };

            foreach (var value in childValues)
            {
                consumer.AddChild(new EnergyConsumerNode { AmountConsumed = value });
            }

            var expected = childValues.Sum() + amount;
            Assert.AreEqual(expected, consumer.TotalAmountConsumed);
        }

        [TestMethod]
        public void AmountChanged_CallbackCount_AllConsuming()
        {
            const float amount = 20;
            var childValues = new float[] { 5, 10, 15 };
            var consumer = new EnergyConsumerNode { AmountConsumed = amount };
            int callbackCount = 0;
            consumer.OnAmountConsumedChanged += (sender, args) => { callbackCount++; };

            foreach (var value in childValues)
            {
                consumer.AddChild(new EnergyConsumerNode { AmountConsumed = value });
            }

            Assert.AreEqual(childValues.Length, callbackCount);
        }

        [TestMethod]
        public void AmountChanged_CallbackCount_NonConsumer()
        {
            const float amount = 20;
            var childValues = new float[] { 5, 10, 15 };
            var consumer = new EnergyConsumerNode { AmountConsumed = amount };
            int callbackCount = 0;
            consumer.OnAmountConsumedChanged += (sender, args) => { callbackCount++; };

            foreach (var value in childValues)
            {
                consumer.AddChild(new EnergyConsumerNode { AmountConsumed = value });
            }

            // test to make sure this 0 amount does not trigger a callback
            consumer.AddChild(new EnergyConsumerNode { AmountConsumed = 0 });

            Assert.AreEqual(childValues.Length, callbackCount);
        }
    }
}