using System;
using Assets.Scripts.Testing;
using Assets.WorldMaterials;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Editor
{
    public class ProductTradingHubTests : ProductTradingHub
    {
        private GameObject _gameObject;
        private ProductTradingHub _target;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject(string.Format("TestObject-{0}", DateTime.Now.Millisecond));
            _gameObject.AddComponent<ProductTradingHub>();

            _target = _gameObject.GetComponent<ProductTradingHub>();
            Assert.IsNotNull(_target);
        }

        [TearDown]
        public void TearDown()
        {
            _gameObject = null;
            _target = null;
        }

        [Test]
        public void TryAddConsumer()
        {
            Assert.IsTrue(true);
        }

    }
}