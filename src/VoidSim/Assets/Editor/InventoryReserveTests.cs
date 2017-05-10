using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Testing;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Editor
{
    public class InventoryReserveTests
    {
        private Inventory _inventory;
        private MockProductLookup _lookup;
        private InventoryScriptable _scriptable;
        private InventoryReserve _reserve;

        private const int MaxAmount = 1000;
        private const int ProductId = 1;
        private const string ProductName = "Product1";

        [SetUp]
        public void SetUp()
        {
            var go = new GameObject(string.Format("TestObject-{0}", DateTime.Now.Millisecond));
            _inventory = go.AddComponent<Inventory>();
            _lookup = go.AddComponent<MockProductLookup>();
            
            _lookup.AddProduct(new Product { Category = ProductCategory.Raw, ID = ProductId, Name = ProductName });

            _scriptable = ScriptableObject.CreateInstance<InventoryScriptable>();
            _scriptable.Products = new List<ProductEntryInfo>();
            _scriptable.Placeables = new List<string>();
            _scriptable.ProductMaxAmount = MaxAmount;
            _inventory.BindToScriptable(_scriptable, _lookup);

            _reserve = new InventoryReserve();
            _reserve.Initialize(_inventory);
            _reserve.AddReservation(ProductId, 0, false, false);
        }

        [TearDown]
        public void TearDown()
        {
            _inventory = null;
            _lookup = null;
            _scriptable = null;
            _reserve = null;
    }

        [Test]
        public void ProvideProduct_AddsProvide()
        {
            const int amount = 100;
            _reserve.SetProvide(ProductId, true);

            var status = _reserve.GetProductStatus(ProductId);
            var before = _reserve.GetProvideProducts().FirstOrDefault(i => i.ProductId == ProductId);

            _inventory.TryAddProduct(ProductId, amount);

            var after = _reserve.GetProvideProducts().FirstOrDefault(i => i.ProductId == ProductId);

            Assert.IsNotNull(status);
            Assert.IsTrue(status.ShouldProvide);
            Assert.IsFalse(status.ShouldConsume);

            Assert.IsNull(before);
            Assert.IsNotNull(after);
            Assert.AreEqual(amount, after.Amount);
        }

        [Test]
        public void ProvideProduct_TrackPartialRemoval()
        {
            const int amount = 100;
            const int removed = 40;
            _reserve.SetProvide(ProductId, true);
            _inventory.TryAddProduct(ProductId, amount);

            var before = _reserve.GetProvideProducts().FirstOrDefault(i => i.ProductId == ProductId);
            _inventory.TryRemoveProduct(ProductId, 40);
            var after = _reserve.GetProvideProducts().FirstOrDefault(i => i.ProductId == ProductId);

            Assert.IsNotNull(before);
            Assert.IsNotNull(after);
            Assert.AreEqual(amount - removed, after.Amount);
        }

        [Test]
        public void ProvideProduct_TrackFullRemoval()
        {
            const int amount = 100;
            _reserve.SetProvide(ProductId, true);
            _inventory.TryAddProduct(ProductId, amount);

            var before = _reserve.GetProvideProducts().FirstOrDefault(i => i.ProductId == ProductId);
            _inventory.TryRemoveProduct(ProductId, amount);
            var after = _reserve.GetProvideProducts().FirstOrDefault(i => i.ProductId == ProductId);

            Assert.IsNotNull(before);
            Assert.IsNull(after);
        }

        [Test]
        public void ConsumeProduct_AddsProvide()
        {
            const int amount = 100;
            _reserve.SetAmount(ProductId, amount);

            var status = _reserve.GetProductStatus(ProductId);
            var before = _reserve.GetConsumeProducts().FirstOrDefault(i => i.ProductId == ProductId);

            _reserve.SetConsume(ProductId, true);
            
            var after = _reserve.GetConsumeProducts().FirstOrDefault(i => i.ProductId == ProductId);

            Assert.IsNotNull(status);
            Assert.IsTrue(status.ShouldConsume);
            Assert.IsFalse(status.ShouldProvide);

            Assert.IsNull(before);
            Assert.IsNotNull(after);
            Assert.AreEqual(amount, after.Amount);
        }

        [Test]
        public void ConsumeProduct_TrackPartialFulfill()
        {
            const int amount = 100;
            const int fulfilled = 40;
            _reserve.SetConsume(ProductId, true);
            _reserve.SetAmount(ProductId, amount);

            var before = _reserve.GetConsumeProducts().FirstOrDefault(i => i.ProductId == ProductId);
            _inventory.TryAddProduct(ProductId, fulfilled);
            var after = _reserve.GetConsumeProducts().FirstOrDefault(i => i.ProductId == ProductId);

            Assert.IsNotNull(before);
            Assert.IsNotNull(after);
            Assert.AreEqual(amount - fulfilled, after.Amount);
        }

        [Test]
        public void ConsumeProduct_TrackFullFulfill()
        {
            const int amount = 100;
            _reserve.SetConsume(ProductId, true);
            _reserve.SetAmount(ProductId, amount);

            var before = _reserve.GetConsumeProducts().FirstOrDefault(i => i.ProductId == ProductId);
            _inventory.TryAddProduct(ProductId, amount);
            var after = _reserve.GetConsumeProducts().FirstOrDefault(i => i.ProductId == ProductId);

            Assert.IsNotNull(before);
            Assert.IsNull(after);
        }

        [Test]
        public void Hold_AffectConsume()
        {
            const int invAmount = 100;
            const int holdAmount = 40;

            _reserve.SetConsume(ProductId, true);
            _reserve.SetAmount(ProductId, invAmount);

            var before = _reserve.GetConsumeProducts().FirstOrDefault(i => i.ProductId == ProductId);
            _reserve.AdjustHold(ProductId, holdAmount);
            var during = _reserve.GetConsumeProducts().FirstOrDefault(i => i.ProductId == ProductId);
            _reserve.AdjustHold(ProductId, -holdAmount);
            var after = _reserve.GetConsumeProducts().FirstOrDefault(i => i.ProductId == ProductId);

            Assert.AreEqual(invAmount, before.Amount);
            Assert.AreEqual(invAmount - holdAmount, during.Amount);
            Assert.AreEqual(invAmount, after.Amount);
        }

        [Test]
        public void Hold_AffectProvide()
        {
            const int invAmount = 100;
            const int holdAmount = 40;

            _reserve.SetProvide(ProductId, true);
            _inventory.TryAddProduct(ProductId, invAmount);
            _reserve.SetAmount(ProductId, 0);

            var before = _reserve.GetProvideProducts().FirstOrDefault(i => i.ProductId == ProductId);
            _reserve.AdjustHold(ProductId, -holdAmount);
            var during = _reserve.GetProvideProducts().FirstOrDefault(i => i.ProductId == ProductId);
            _reserve.AdjustHold(ProductId, holdAmount);
            var after = _reserve.GetProvideProducts().FirstOrDefault(i => i.ProductId == ProductId);

            Assert.AreEqual(invAmount, before.Amount);
            Assert.AreEqual(invAmount - holdAmount, during.Amount);
            Assert.AreEqual(invAmount, after.Amount);
        }

        [Test]
        public void Hold_AffectProvideMultiple()
        {
            const int invAmount = 50;
            const int holdAmount = 20;

            _reserve.SetProvide(ProductId, true);
            _inventory.TryAddProduct(ProductId, invAmount);
            _reserve.SetAmount(ProductId, 0);

            var before = _reserve.GetProvideProducts().FirstOrDefault(i => i.ProductId == ProductId);
            _reserve.AdjustHold(ProductId, -holdAmount);
            var during = _reserve.GetProvideProducts().FirstOrDefault(i => i.ProductId == ProductId);
            _reserve.AdjustHold(ProductId, -holdAmount);
            var after = _reserve.GetProvideProducts().FirstOrDefault(i => i.ProductId == ProductId);

            Assert.AreEqual(invAmount, before.Amount);
            Assert.AreEqual(invAmount - holdAmount, during.Amount);
            Assert.AreEqual(invAmount - holdAmount - holdAmount, after.Amount);
        }
    }
}