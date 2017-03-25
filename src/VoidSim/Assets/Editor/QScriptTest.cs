using System;
using Assets.Scripts.Testing;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Editor
{
    public class QScriptTest
    {
        private GameObject _gameObject;
        private ConcreteScript _target;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject(String.Format("TestObject-{0}", DateTime.Now.Millisecond));
            _gameObject.AddComponent<ConcreteScript>();

            _target = _gameObject.GetComponent(typeof(ConcreteScript)) as ConcreteScript;
            Assert.IsNotNull(_target);
        }

        [TearDown]
        public void TearDown()
        {
            _gameObject = null;
            _target = null;
        }

        [Test]
        public void QScript_IsAliveOnCreation()
        {
            Assert.IsTrue(_target.IsAlive);
        }

        [Test]
        public void QScript_OnDeath_EventsFired()
        {
            var aliveChangedFired = false;
            var onDeathFired = false;

            _target.AliveChanged += delegate { aliveChangedFired = true; };

            Assert.IsFalse(aliveChangedFired);
            Assert.IsFalse(onDeathFired);

            _target.IsAlive = false;

            Assert.IsTrue(aliveChangedFired);
            Assert.IsTrue(onDeathFired);
        }

        [Test]
        public void QScript_AliveChanged_EventsFired()
        {
            var aliveChangedCount = 0;

            _target.AliveChanged += delegate { aliveChangedCount++; };
            
            _target.IsAlive = false;
            _target.IsAlive = true;

            Assert.AreEqual(2, aliveChangedCount);
        }

        [Test]
        public void QScript_OnUpdate_EventsFired()
        {
            var onNextUpdateCount = 0;
            var onEveryUpdateCount = 0;

            _target.OnNextUpdate += delegate { onNextUpdateCount++; };
            _target.OnEveryUpdate += delegate { onEveryUpdateCount++; };
            
            //_target..Update();
            //Assert.AreEqual(1, onNextUpdateCount);
            //Assert.AreEqual(1, onEveryUpdateCount);
            //Assert.AreEqual(1, _target.OnUpdateCount);
        }

        [Test]
        public void QScript_OnNextUpdateIsCalledOnce()
        {
            var onNextUpdateCount = 0;
            var onEveryUpdateCount = 0;

            _target.OnNextUpdate += delegate { onNextUpdateCount++; };
            _target.OnEveryUpdate += delegate { onEveryUpdateCount++; };

            //_target.Update(1);
            //_target.Update(1);
            //Assert.AreEqual(1, onNextUpdateCount);
            //Assert.AreEqual(2, onEveryUpdateCount);
            //Assert.AreEqual(2, onEveryUpdateCount);
        }

        [Test]
        public void QScript_EnabledChanged_EventFired()
        {
            var enabledChangedCount = 0;

            _target.EnableChanged += delegate { enabledChangedCount++; };

            _target.IsEnabled = !_target.IsEnabled;
            Assert.AreEqual(1, enabledChangedCount);
        }

        [Test]
        public void QScript_ClearAllDelegates()
        {
            var onNextUpdateCount = 0;
            var onEveryUpdateCount = 0;
            var enabledChangedCount = 0;
            var aliveChangedCount = 0;

            _target.EnableChanged += delegate { enabledChangedCount++; };
            _target.OnNextUpdate += delegate { onNextUpdateCount++; };
            _target.OnEveryUpdate += delegate { onEveryUpdateCount++; };
            _target.AliveChanged += delegate { aliveChangedCount++; };
            //_target.OnDeath += delegate { onDeathCount++; };

            _target.ClearAllDelegates();

            _target.IsAlive = false;
            _target.IsAlive = true;
            //_target.Update();
            //_target.Update();
            _target.IsEnabled = false;
            _target.IsEnabled = true;

            Assert.AreEqual(0, onNextUpdateCount);
            Assert.AreEqual(0, onEveryUpdateCount);
            Assert.AreEqual(0, enabledChangedCount);
            Assert.AreEqual(0, aliveChangedCount);
            Assert.AreEqual(2, _target.OnUpdateCount);
            //Assert.AreEqual(0, onDeathCount, "OnDeath was invoked after ClearAllDelegates was called.");

        }
    }
}
