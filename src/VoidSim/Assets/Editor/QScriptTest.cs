﻿using System;
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
        public void QScript_OnUpdate_EventsFired()
        {
            var onNextUpdateCount = 0;
            var onEveryUpdateCount = 0;

            _target.OnNextUpdate += delegate { onNextUpdateCount++; };
            _target.OnEveryUpdate += delegate { onEveryUpdateCount++; };

            _target.MockUpdate(1);
            Assert.AreEqual(1, onNextUpdateCount);
            Assert.AreEqual(1, onEveryUpdateCount);
        }

        [Test]
        public void QScript_OnNextUpdateIsCalledOnce()
        {
            var onNextUpdateCount = 0;
            var onEveryUpdateCount = 0;

            _target.OnNextUpdate += delegate { onNextUpdateCount++; };
            _target.OnEveryUpdate += delegate { onEveryUpdateCount++; };

            _target.MockUpdate(1);
            _target.MockUpdate(1);
            Assert.AreEqual(1, onNextUpdateCount);
            Assert.AreEqual(2, onEveryUpdateCount);
            Assert.AreEqual(2, onEveryUpdateCount);
        }

        [Test]
        public void QScript_ClearAllDelegates()
        {
            var onNextUpdateCount = 0;
            var onEveryUpdateCount = 0;

            _target.OnNextUpdate += delegate { onNextUpdateCount++; };
            _target.OnEveryUpdate += delegate { onEveryUpdateCount++; };
            
            _target.ClearCallbacks();

            _target.MockUpdate(1);

            Assert.AreEqual(0, onNextUpdateCount);
            Assert.AreEqual(0, onEveryUpdateCount);
        }
    }
}
