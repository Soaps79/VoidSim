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
        private ProductInventory _productInventory;
        private MockProductLookup _lookup;
        private InventoryScriptable _scriptable;

        private const int MaxAmount = 1000;
        private const int ProductId = 1;
        private const string ProductName = "Product1";

        [SetUp]
        public void SetUp()
        {
            var go = new GameObject(string.Format("TestObject-{0}", DateTime.Now.Millisecond));
            _productInventory = new ProductInventory();
            _lookup = go.AddComponent<MockProductLookup>();

            _lookup.AddProduct(new Product {Category = ProductCategory.Raw, ID = ProductId, Name = ProductName});

            _scriptable = ScriptableObject.CreateInstance<InventoryScriptable>();
            _scriptable.Products = new List<ProductEntryInfo>();
            _scriptable.Placeables = new List<string>();
            _scriptable.ProductMaxAmount = MaxAmount;
        }

        [TearDown]
        public void TearDown()
        {
            _productInventory = null;
        }

        [Test]
        public void AddProduct_Simple()
        {
            const int amount = 10;

            _productInventory.Initialize(_scriptable, _lookup);

            var initial = _productInventory.GetProductCurrentAmount(ProductId);
            _productInventory.SetProductMaxAmount(1, 1000);
            var remainder = _productInventory.TryAddProduct(ProductId, amount);

            var current = _productInventory.GetProductCurrentAmount(ProductId);

            Assert.AreEqual(0, initial);
            Assert.AreEqual(0, remainder);
            Assert.AreEqual(amount, current);
        }

        [Test]
        public void MaxAmount_IsHonored()
        {
            const int difference = 100;

            _productInventory.Initialize(_scriptable, _lookup);

            var remainder = _productInventory.TryAddProduct(ProductId, MaxAmount + difference);
            var current = _productInventory.GetProductCurrentAmount(ProductId);

            Assert.AreEqual(difference, remainder);
            Assert.AreEqual(MaxAmount, current);
        }

        [Test]
        public void MaxAmount_CanBeSet()
        {
            const int max = 40;
            const int amount = 100;

            _productInventory.Initialize(_scriptable, _lookup);
            _productInventory.SetProductMaxAmount(1, max);

            var remainder = _productInventory.TryAddProduct(ProductId, amount);
            var current = _productInventory.GetProductCurrentAmount(ProductId);
            var currentMax = _productInventory.GetProductMaxAmount(ProductId);

            Assert.AreEqual(amount - max, remainder);
            Assert.AreEqual(max, current);
            Assert.AreEqual(max, currentMax);
        }

        [Test]
        public void RemoveProduct_FullAvailable()
        {
            const int amount = 100;

            _scriptable.Products.Add(new ProductEntryInfo {ProductName = ProductName, Amount = amount});
            _productInventory.Initialize(_scriptable, _lookup);

            var removed = _productInventory.TryRemoveProduct(ProductId, amount);
            var current = _productInventory.GetProductCurrentAmount(ProductId);

            Assert.AreEqual(amount, removed);
            Assert.AreEqual(0, current);
        }

        [Test]
        public void RemoveProduct_PartialAvailable()
        {
            const int available = 100;
            const int amount = 1000;

            _scriptable.Products.Add(new ProductEntryInfo {ProductName = ProductName, Amount = available});
            _productInventory.Initialize(_scriptable, _lookup);

            var removed = _productInventory.TryRemoveProduct(ProductId, amount);
            var current = _productInventory.GetProductCurrentAmount(ProductId);

            Assert.AreEqual(available, removed);
            Assert.AreEqual(0, current);
        }

        [Test]
        public void Callback_AddProduct()
        {
            const int amount = 100;

            var callbackId = 0;
            var callbackAmount = 0;

            _productInventory.Initialize(_scriptable, _lookup);
            _productInventory.OnProductsChanged += (i, a) =>
            {
                callbackId = i;
                callbackAmount = a;
            };

            _productInventory.TryAddProduct(ProductId, amount);

            Assert.AreEqual(ProductId, callbackId);
            Assert.AreEqual(amount, callbackAmount);
        }

        [Test]
        public void Callback_RemoveProduct()
        {
            const int amount = 100;

            var callbackId = 0;
            var callbackAmount = 0;

            _scriptable.Products.Add(new ProductEntryInfo {ProductName = ProductName, Amount = amount});
            _productInventory.Initialize(_scriptable, _lookup);
            _productInventory.OnProductsChanged += (i, a) =>
            {
                callbackId = i;
                callbackAmount = a;
            };

            _productInventory.TryRemoveProduct(ProductId, amount);

            Assert.AreEqual(ProductId, callbackId);
            Assert.AreEqual(-amount, callbackAmount);
        }

        [Test]
        public void Callback_Remove_ParentInventoryCallbackIsCalled()
        {
            const int amount = 100;
            var callbackBaseHappened = false;

            var go = new GameObject();
            var inventory = go.AddComponent<Inventory>();
            _scriptable.Products.Add(new ProductEntryInfo {ProductName = ProductName, Amount = amount});
            inventory.Initialize(_scriptable, _lookup);
            var productInventory = inventory.Products;

            inventory.OnInventoryChanged += () => callbackBaseHappened = true;
            productInventory.TryRemoveProduct(ProductId, amount);

            Assert.IsTrue(callbackBaseHappened);
        }

        [Test]
        public void Callback_Add_ParentInventoryCallbackIsCalled()
        {
            const int amount = 100;
            var callbackBaseHappened = false;

            var go = new GameObject();
            var inventory = go.AddComponent<Inventory>();
            _scriptable.Products.Add(new ProductEntryInfo {ProductName = ProductName, Amount = amount});
            inventory.Initialize(_scriptable, _lookup);
            var productInventory = inventory.Products;

            inventory.OnInventoryChanged += () => callbackBaseHappened = true;
            productInventory.TryAddProduct(ProductId, amount);

            Assert.IsTrue(callbackBaseHappened);
        }
    }
}