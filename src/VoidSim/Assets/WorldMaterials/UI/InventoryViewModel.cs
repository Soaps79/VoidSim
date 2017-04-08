using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.WorldMaterials;
using Assets.Station;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.WorldMaterials.UI
{
    public class InventoryViewModel : MonoBehaviour
    {
        [SerializeField]
        private Image _productEntryPrefab;
        [SerializeField]
        private Button _placeableEntryPrefab;
        [SerializeField]
        private Image _inventoryPanelPrefab;
        [SerializeField]
        private List<int> _productsToIgnore = new List<int>();

        private Transform _productContentHolder;
        private Transform _placeablesContentHolder;
        private Inventory _inventory;
        private readonly List<GameObject> _entryList = new List<GameObject>();
        private PlaceablesLookup _placeablesLookup;

        public void BindToInventory(Inventory inventory, InventoryScriptable inventoryScriptable, PlaceablesLookup placeablesLookup)
        {
            _inventory = inventory;
            _inventory.OnProductsChanged += UpdateEntries;
            _placeablesLookup = placeablesLookup;

            UpdateIgnoreList(inventoryScriptable);
            BindToUI();
        }

        private void UpdateIgnoreList(InventoryScriptable inventoryScriptable)
        {
            var products = ProductLookup.Instance.GetProducts()
                .Where(i => inventoryScriptable.ProductsToIgnore.Contains(i.Name));
            _productsToIgnore.Clear();
            _productsToIgnore.AddRange(products.Select(i => i.ID).ToList());
        }

        private void BindToUI()
        {
            var craftingPanel = GameObject.Instantiate(_inventoryPanelPrefab);
            _productContentHolder = craftingPanel.transform.FindChild("content_holder/product_list");
            _placeablesContentHolder = craftingPanel.transform.FindChild("content_holder/placeable_list");

            PositionOnCanvas(craftingPanel);
            DrawProductEntries();
            DrawPlaceableEntries();
        }

        private void DrawProductEntries()
        {
            foreach (var entryInfo in _inventory.GetProductEntries())
            {
                if (_productsToIgnore.Contains(entryInfo.Product.ID))
                    continue;

                var go = Instantiate(_productEntryPrefab).gameObject;
                go.transform.SetParent(_productContentHolder.transform);
                var binder = go.gameObject.GetOrAddComponent<ProductEntryBinder>();
                binder.Bind(entryInfo.Product.Name, entryInfo.Amount);
                _entryList.Add(go);
            }
        }

        private void DrawPlaceableEntries()
        {
            foreach (var placeable in _inventory.Placeables)
            {
                var scriptable = _placeablesLookup.Placeables.FirstOrDefault(i => i.ProductName == placeable);

                if(scriptable == null)
                    throw new UnityException("Placeable name in inventory has no lookup value");




                var button = Instantiate(_placeableEntryPrefab);
                button.transform.SetParent(_placeablesContentHolder.transform);
                //var binder = go.gameObject.GetOrAddComponent<ProductEntryBinder>();
                //binder.Bind(entryInfo.Product.Name, entryInfo.Amount);
                var image = button.GetComponent<Image>();
                image.sprite = scriptable.IconSprite;
            }
        }

        private void UpdateEntries(string name, int amount)
        {
            ClearEntries();
            // save incoming values here to light up entry
            DrawProductEntries();
        }

        private void ClearEntries()
        {
            foreach (var entry in _entryList)
            {
                Destroy(entry);
            }
        }

        private static void PositionOnCanvas(Image craftingPanel)
        {
            var canvas = GameObject.Find("InfoCanvas");
            craftingPanel.transform.SetParent(canvas.transform);
            craftingPanel.rectTransform.position = new Vector3(10, 600, 0);
        }
    }
}