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
            public bool ShouldConsume;
            public bool ShouldProvide;
        }

        [SerializeField] private Inventory _inventory;
        private Dictionary<int, int> _holdProducts = new Dictionary<int, int>();
        private readonly List<InventoryReserveEntry> _reserveEntries = new List<InventoryReserveEntry>();
        private readonly List<ProductAmount> _toConsume = new List<ProductAmount>();
        private readonly List<ProductAmount> _toProvide = new List<ProductAmount>();


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
            UpdateConsume();
            UpdateProvide();
        }

        private void UpdateConsume()
        {
            _toConsume.Clear();

            foreach (var productAmount in _reserveEntries.Where(i => i.ShouldConsume))
            {
                var current = _inventory.GetProductCurrentAmount(productAmount.ProductId);
                var amount = productAmount.Amount;
                if (_holdProducts.ContainsKey(productAmount.ProductId))
                    amount -= _holdProducts[productAmount.ProductId];
                if (current >= amount) continue;

                var difference = amount - current;
                _toConsume.Add(new ProductAmount { ProductId = productAmount.ProductId, Amount = difference });
            }
        }

        private void UpdateProvide()
        {
            _toProvide.Clear();

            foreach (var productAmount in _reserveEntries.Where(i => i.ShouldProvide))
            {
                var current = _inventory.GetProductCurrentAmount(productAmount.ProductId);
                var amount = productAmount.Amount;
                if (_holdProducts.ContainsKey(productAmount.ProductId))
                    amount -= _holdProducts[productAmount.ProductId];
                if (current <= amount) continue;

                var difference = current - amount;
                _toProvide.Add(new ProductAmount { ProductId = productAmount.ProductId, Amount = difference });
            }
        }

        public List<ProductAmount> GetProvideProducts()
        {
            return _toProvide;
        }

        public List<ProductAmount> GetConsumeProducts()
        {
            return _toConsume;
        }

        public InventoryReserveEntry GetProductStatus(int productId)
        {
            return _reserveEntries.FirstOrDefault(i => i.ProductId == productId);
        }

        public void SetConsume(int productId, bool shouldConsume)
        {
            var collect = _reserveEntries.FirstOrDefault(i => i.ProductId == productId);
            if (collect == null) return;

            collect.ShouldConsume = shouldConsume;
            UpdateReserve();
        }

        public void SetProvide(int productId, bool shouldProvide)
        {
            var release = _reserveEntries.FirstOrDefault(i => i.ProductId == productId);
            if (release == null) return;

            release.ShouldProvide = shouldProvide;
            UpdateReserve();
        }

        public void AddReservation(int productId, int amount, bool shouldConsume, bool shouldProvide)
        {
            _reserveEntries.Add(new InventoryReserveEntry
            {
                ProductId = productId,
                Amount = amount,
                ShouldConsume = shouldConsume,
                ShouldProvide = shouldProvide
            });
            UpdateReserve();
        }

        public void SetAmount(int productId, int amount)
        {
            var release = _reserveEntries.FirstOrDefault(i => i.ProductId == productId);
            if (release == null || release.Amount == amount) return;

            release.Amount = amount;
            UpdateReserve();
        }

        public void AdjustHold(int productId, int amount)
        {
            if (!_holdProducts.ContainsKey(productId))
                _holdProducts.Add(productId, 0);

            _holdProducts[productId] += amount;
            UpdateReserve();
        }
    }
}