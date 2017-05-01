using System;
using System.Collections.Generic;
using Assets.Scripts.Testing;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using UnityEngine;
using NUnit.Framework;

namespace Assets.Editor
{
    public class InventoryTests : MonoBehaviour
    {
        private Inventory _inventory;
        private MockProductLookup _lookup;
        private InventoryScriptable _scriptable;

        private const int MaxAmount = 1000;
        private const int ProductId = 1;
        private const string ProductName = "Product1";

        [SetUp]
        public void SetUp()
        {
            var go = new GameObject(string.Format("TestObject-{0}", DateTime.Now.Millisecond));
            _inventory = go.AddComponent<Inventory>();
            _lookup = go.AddComponent<MockProductLookup>();

            _lookup.AddProduct(new Product { Category = ProductCategory.Raw, ID = ProductId, Name = ProductName});

            _scriptable = ScriptableObject.CreateInstance<InventoryScriptable>();
            _scriptable.Products = new List<ProductEntryInfo>();
            _scriptable.Placeables = new List<string>();
            _scriptable.ProductMaxAmount = MaxAmount;
        }

        [TearDown]
        public void TearDown()
        {
            _inventory = null;
        }

        [Test]
        public void AddProduct_Simple()
        {
            const int amount = 10;

            _inventory.BindToScriptable(_scriptable, _lookup);

            var initial = _inventory.GetProductCurrentAmount(ProductId);
            _inventory.SetProductMaxAmount(1, 1000);
            var remainder = _inventory.TryAddProduct(ProductId, amount);

            var current = _inventory.GetProductCurrentAmount(ProductId);

            Assert.AreEqual(0, initial);
            Assert.AreEqual(0, remainder);
            Assert.AreEqual(amount, current);
        }

        [Test]
        public void MaxAmount_IsHonored()
        {
            const int difference = 100;

            _inventory.BindToScriptable(_scriptable, _lookup);

            var remainder = _inventory.TryAddProduct(ProductId, MaxAmount + difference);
            var current = _inventory.GetProductCurrentAmount(ProductId);

            Assert.AreEqual(difference, remainder);
            Assert.AreEqual(MaxAmount, current);
        }

        [Test]
        public void MaxAmount_CanBeSet()
        {
            const int max = 40;
            const int amount = 100;

            _inventory.BindToScriptable(_scriptable, _lookup);
            _inventory.SetProductMaxAmount(1, max);

            var remainder = _inventory.TryAddProduct(ProductId, amount);
            var current = _inventory.GetProductCurrentAmount(ProductId);
            var currentMax = _inventory.GetProductMaxAmount(ProductId);

            Assert.AreEqual(amount - max, remainder);
            Assert.AreEqual(max, current);
            Assert.AreEqual(max, currentMax);
        }

        [Test]
        public void RemoveProduct_FullAvailable()
        {
            const int amount = 100;

            _scriptable.Products.Add(new ProductEntryInfo { ProductName = ProductName, Amount = amount });
            _inventory.BindToScriptable(_scriptable, _lookup);
            
            var removed = _inventory.TryRemoveProduct(ProductId, amount);
            var current = _inventory.GetProductCurrentAmount(ProductId);

            Assert.AreEqual(amount, removed);
            Assert.AreEqual(0, current);
        }

        [Test]
        public void RemoveProduct_PartialAvailable()
        {
            const int available = 100;
            const int amount = 1000;

            _scriptable.Products.Add(new ProductEntryInfo { ProductName = ProductName, Amount = available });
            _inventory.BindToScriptable(_scriptable, _lookup);

            var removed = _inventory.TryRemoveProduct(ProductId, amount);
            var current = _inventory.GetProductCurrentAmount(ProductId);

            Assert.AreEqual(available, removed);
            Assert.AreEqual(0, current);
        }
    }
}