using System.Linq;
using Assets.WorldMaterials.Products;
using UIWidgets;
using UnityEngine;
#pragma warning disable 649

namespace Assets.WorldMaterials.UI
{
    public class InventoryGridTile
    {
        public string Name;
        public Color Color;
        public int Quantity;
        public bool IsEmpty;
    }

    public class InventoryGridViewModel : TileViewCustom<InventoryGridItemViewModel, InventoryGridTile>
    {
        [SerializeField] private int TileAmount;
        private ProductInventory _inventory;

        public void BindToInventory(ProductInventory inventory)
        {
            _inventory = inventory;
            _inventory.OnProductsChanged += UpdateGrid;
            UpdateGrid(0, 0);
        }

        private void UpdateGrid(int arg1, int arg2)
        {
            if (TileAmount <= 0)
                throw new UnityException("InventoryGrid tried to initialize with no cell count");

            var products = _inventory.GetProductEntries().Where(i => i.Product.Category != ProductCategory.Core);
            var observable = new ObservableList<InventoryGridTile>();
            foreach (var product in products)
            {
                var remaining = product.Amount;
                while (remaining >= TileAmount)
                {
                    observable.Add(new InventoryGridTile
                    {
                        Name = product.Product.Name,
                        Color = product.Product.Color,
                        Quantity = product.Amount
                    });
                    remaining -= TileAmount;
                }        
            }

            if (_inventory.MaxGlobalAmount > 0)
            {
                var totalCells = _inventory.MaxGlobalAmount > 0 ? _inventory.MaxGlobalAmount / TileAmount : 0;
                var freeCells = totalCells - observable.Count;
                if (freeCells > 0)
                {
                    for (var i = 0; i < freeCells; i++)
                    {
                        observable.Add(new InventoryGridTile {IsEmpty = true});
                    }
                }
            }

            DataSource = observable;
        }
    }
}