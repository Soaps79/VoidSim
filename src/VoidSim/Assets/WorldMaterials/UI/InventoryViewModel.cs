using System.Collections.Generic;
using System.Linq;
using Assets.Placeables;
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
        private List<ProductCategory> _productsToIgnore = new List<ProductCategory>();

        private Transform _productContentHolder;
        private Transform _placeablesContentHolder;
        private Inventory _inventory;
        private readonly List<GameObject> _productEntryList = new List<GameObject>();
        private readonly List<Button> _placeableEntryList = new List<Button>();
        private PlaceablesLookup _placeablesLookup;
        private Placer _placer;

        public void BindToInventory(Inventory inventory, InventoryScriptable inventoryScriptable, PlaceablesLookup placeablesLookup)
        {
            _inventory = inventory;
            _inventory.OnProductsChanged += RefreshProducts;
            _placeablesLookup = placeablesLookup;

            InitializePlacer();
            UpdateIgnoreList(inventoryScriptable);
            BindToUI();
        }

        private void InitializePlacer()
        {
            _placer = gameObject.GetOrAddComponent<Placer>();
            _placer.Initialize(_placeablesLookup);
            _placer.OnPlacementComplete += HandlePlacementComplete;
        }

        private void HandlePlacementComplete(int id)
        {
            if(id <= 0)
                return;

            _inventory.TryRemovePlaceable(id);
            ClearPlaceableEntries();
            DrawPlaceableEntries();
        }

        private void UpdateIgnoreList(InventoryScriptable inventoryScriptable)
        {
            var products = ProductLookup.Instance.GetProducts()
                .Where(i => inventoryScriptable.ProductsToIgnore.Contains(i.Category));
            _productsToIgnore.Clear();
            _productsToIgnore.AddRange(products.Select(i => i.Category).ToList());
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
                if (_productsToIgnore.Contains(entryInfo.Product.Category))
                    continue;

                var go = Instantiate(_productEntryPrefab).gameObject;
                go.transform.SetParent(_productContentHolder.transform);
                var binder = go.gameObject.GetOrAddComponent<ProductEntryBinder>();
                binder.Bind(entryInfo.Product.Name, entryInfo.Amount);
                _productEntryList.Add(go);
            }
        }

        private void DrawPlaceableEntries()
        {
            foreach (var placeable in _inventory.Placeables)
            {
                var scriptable = _placeablesLookup.Placeables.FirstOrDefault(i => i.ProductName == placeable.Name );

                if(scriptable == null)
                    throw new UnityException("Placeable name in inventory has no lookup value");

                var button = Instantiate(_placeableEntryPrefab);
                button.transform.SetParent(_placeablesContentHolder.transform);
                var image = button.GetComponent<Image>();
                image.sprite = scriptable.IconSprite;
                button.onClick.AddListener(() => { BeginPlacement(placeable.Name, placeable.Id); });
                _placeableEntryList.Add(button);
            }
        }

        private void RefreshProducts(string productName, int amountChanged)
        {
            ClearProductEntries();
            // save incoming values here to light up entry
            DrawProductEntries();
        }

        private void ClearProductEntries()
        {
            foreach (var entry in _productEntryList)
            {
                Destroy(entry);
            }
            _productEntryList.Clear();
        }

        private void ClearPlaceableEntries()
        {
            foreach (var entry in _placeableEntryList)
            {
                entry.gameObject.SetActive(false);
                Destroy(entry);
            }
            _placeableEntryList.Clear();
        }

        private static void PositionOnCanvas(Image craftingPanel)
        {
            var canvas = GameObject.Find("InfoCanvas");
            craftingPanel.transform.SetParent(canvas.transform);
            craftingPanel.rectTransform.position = new Vector3(10, 600, 0);
        }

        private void BeginPlacement(string placeableName, int inventoryId)
        {
            _placer.BeginPlacement(placeableName, inventoryId);
        }
    }
}