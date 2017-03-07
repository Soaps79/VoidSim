using System.Collections.Generic;
using Behaviors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QGame.Test.Behaviors
{
    [TestClass]
    public class BehaviorTests
    {
        private class LivingConcrete : ILiving
        {
            public event VoidILivingCallback AliveChanged;

            private bool _isAlive;

            public bool IsAlive
            {
                get { return _isAlive; }
                set
                {
                    if (_isAlive != value)
                    {
                        _isAlive = value;
                        if (AliveChanged != null)
                            AliveChanged(this);
                    }
                    
                    
                }
            }
        }

        [TestMethod]
        public void Behavior_KillTimerTest()
        {
            var behavior = new WaitBehavior(4);
            behavior.IsAlive = true;


            int i = 0;
            while (behavior.IsAlive)
            {
                behavior.Update(0.5f);
                i++;
            }

            Assert.IsTrue(i == 9);
        }

        [TestMethod]
        public void BehaviorHolder_UpdateBasicTest()
        {
            var holder = new BehaviorHolder(new LivingConcrete());

            int count = 0;

            var beh = new WaitBehavior(2);
            holder.Attach(beh);

            beh = new WaitBehavior(3);
            holder.Attach(beh);

            while (holder.HasAliveBehaviors)
            {
                holder.Update(1);
                count++;
            }

            // Behaviors are granted one last update after they are killed
            Assert.AreEqual(4, count);
        }

        [TestMethod]
        public void LivingConcrete_AliveChanged_IsFired()
        {
            var living = new LivingConcrete();
            var eventFired = false;
            living.AliveChanged += delegate { eventFired = true; };

            Assert.IsFalse(eventFired);
            living.IsAlive = !living.IsAlive;
            Assert.IsTrue(eventFired);

        }

        [TestMethod]
        public void BehaviorPack_AttachChildrenOnFirstUpdate()
        {
            var holder = new BehaviorHolder(new LivingConcrete());

            var pack = new BehaviorPack(new List<Behavior>()
                                            {
                                                new WaitBehavior(1),
                                                new WaitBehavior(1),
                                                new WaitBehavior(1),
                                            });
            holder.Attach(pack);
            holder.Update(0.001f);
            Assert.AreEqual(3, holder.AliveCount);
        }
    }
}