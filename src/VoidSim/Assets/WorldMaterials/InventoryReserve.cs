using System.Collections.Generic;
using System.Linq;
using Assets.WorldMaterials.Products;
using UnityEngine;

namespace Assets.WorldMaterials
{
    /// <summary>
    /// Attaches to an Inventory and has a list of product minimums to keep.
    /// Can withdraw and return product counts above the minimum.
    /// TODO: tighten up interface once usage is better known
    /// </summary>
    public class InventoryReserve
    {
        public class InventoryReserveEntry
        {
            public int ProductId;
            public int Amount;
            public bool ShouldCollect;
            public bool ShouldRelease;
        }

        [SerializeField] private Inventory _inventory;
        private List<InventoryReserveEntry> _reserveEntries = new List<InventoryReserveEntry>();
        private readonly List<ProductAmount> _toCollect = new List<ProductAmount>();
        private readonly List<ProductAmount> _toRelease = new List<ProductAmount>();


        public void Initialize(Inventory inventory)
        {
            _inventory = inventory;
            UpdateReserve();

            _inventory.OnInventoryChanged += UpdateReserve;
        }

        // Updating of reserve lists happens in these three functions
        // could be refactored into one loop
        private void UpdateReserve()
        {
            UpdateCollect();
            UpdateRelease();
        }

        private void UpdateCollect()
        {
            _toCollect.Clear();

            foreach (var productAmount in _reserveEntries.Where(i => i.ShouldCollect))
            {
                var current = _inventory.GetProductCurrentAmount(productAmount.ProductId);
                if (current >= productAmount.Amount) continue;

                var difference = productAmount.Amount - current;
                _toCollect.Add(new ProductAmount { ProductId = productAmount.ProductId, Amount = difference });
            }
        }

        private void UpdateRelease()
        {
            _toRelease.Clear();

            foreach (var productAmount in _reserveEntries.Where(i => i.ShouldRelease))
            {
                var current = _inventory.GetProductCurrentAmount(productAmount.ProductId);
                if (current <= productAmount.Amount) continue;

                var difference = current - productAmount.Amount;
                _toRelease.Add(new ProductAmount { ProductId = productAmount.ProductId, Amount = difference });
            }
        }

        public List<ProductAmount> GetProvideProducts()
        {
            return _toRelease;
        }

        public List<ProductAmount> GetConsumeProducts()
        {
            return _toCollect;
        }

        public InventoryReserveEntry GetProductStatus(int productId)
        {
            return _reserveEntries.FirstOrDefault(i => i.ProductId == productId);
        }

        public void SetCollect(int productId, bool shouldCollect)
        {
            var collect = _reserveEntries.FirstOrDefault(i => i.ProductId == productId);
            if (collect == null) return;

            collect.ShouldCollect = shouldCollect;
            UpdateReserve();
        }

        public void SetRelease(int productId, bool shouldRelease)
        {
            var release = _reserveEntries.FirstOrDefault(i => i.ProductId == productId);
            if (release == null) return;

            release.ShouldRelease = shouldRelease;
            UpdateReserve();
        }

        public void AddReservation(int productId, int amount, bool shouldCollect, bool shouldRelease)
        {
            _reserveEntries.Add(new InventoryReserveEntry
            {
                ProductId = productId, Amount = amount, ShouldCollect = shouldCollect, ShouldRelease = shouldRelease
            });
        }

        public void SetAmount(int productId, int amount)
        {
            var release = _reserveEntries.FirstOrDefault(i => i.ProductId == productId);
            if (release == null || release.Amount == amount) return;

            release.Amount = amount;
            UpdateReserve();
        }
    }
}