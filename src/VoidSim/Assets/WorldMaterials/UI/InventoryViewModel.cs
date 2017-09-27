using System.Collections.Generic;
using System.Linq;
using Assets.Placeables;
using Assets.Placeables.Placement;
using Assets.Scripts;
using Assets.WorldMaterials.Products;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.WorldMaterials.UI
{
    public class InventoryViewModel : MonoBehaviour
    {
        [SerializeField] private Image _productEntryPrefab;
        [SerializeField] private Button _placeableEntryPrefab;
        [SerializeField] private Image _inventoryPanelPrefab;
        [SerializeField] private readonly List<ProductCategory> _productsToIgnore = new List<ProductCategory>();
	    [SerializeField] private Color _increaseColor;
	    [SerializeField] private Color _decreaseColor;
	    [SerializeField] private float _pulseTime;
	    private Color _normalColor;

		private Transform _productContentHolder;
        private Transform _placeablesContentHolder;
        private Inventory _inventory;
        private InventoryReserve _inventoryReserve;
        private readonly List<ProductEntryBinder> _productEntryList = new List<ProductEntryBinder>();
        private readonly List<Button> _placeableEntryList = new List<Button>();
        private PlaceablesLookup _placeablesLookup;
        private Placer _placer;

        public void BindToInventory(Inventory inventory, InventoryScriptable inventoryScriptable, PlaceablesLookup placeablesLookup, InventoryReserve inventoryReserve, Placer placer)
        {
            _inventory = inventory;
            _inventory.OnProductsChanged += UpdateProductEntry;
            _placeablesLookup = placeablesLookup;
            _inventoryReserve = inventoryReserve;

	        _placer = placer;
	        _placer.OnPlacementComplete += HandlePlacementComplete;

			UpdateIgnoreList(inventoryScriptable);
            BindToUI();
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
			var canvas = GameObject.Find("InfoCanvas");
            //_display = Instantiate(_displayPanelPrefab, canvas.transform, false);

            var craftingPanel = Instantiate(_inventoryPanelPrefab, canvas.transform, false);
            _productContentHolder = craftingPanel.transform.Find("content_holder/product_list");
            _placeablesContentHolder = craftingPanel.transform.Find("content_holder/placeable_list");

            gameObject.RegisterSystemPanel(craftingPanel.gameObject);

            //PositionOnCanvas(craftingPanel);
            DrawProductEntries();
            DrawPlaceableEntries();
        }

        private void DrawProductEntries()
        {
            foreach (var entryInfo in _inventory.GetProductEntries())
            {
                if (_productsToIgnore.Contains(entryInfo.Product.Category))
                    continue;

                // informational entry
                var go = Instantiate(_productEntryPrefab, _productContentHolder.transform, false).gameObject;
                var binder = go.gameObject.GetOrAddComponent<ProductEntryBinder>();
                binder.Bind(entryInfo.Product.Name, entryInfo.Amount, entryInfo.Product.ID);

                // player interface for buying and selling
                var go2 = go.transform.Find("reserve_amount").gameObject;
                var reserve = go2.GetComponent<InventoryReserveViewModel>();
                reserve.Initialize(_inventoryReserve, entryInfo.Product.ID, entryInfo.MaxAmount);
                reserve.gameObject.SetActive(false);

                _productEntryList.Add(binder);
            }
        }

		// items that can be picked up from inventory and placed in game
        private void DrawPlaceableEntries()
        {
            foreach (var placeable in _inventory.Placeables)
            {
                var scriptable = _placeablesLookup.Placeables.FirstOrDefault(i => i.ProductName == placeable.Name );

                if(scriptable == null)
                    throw new UnityException("Placeable name in inventory has no lookup value");

                var button = Instantiate(_placeableEntryPrefab, _placeablesContentHolder.transform, false);
                var image = button.GetComponent<Image>();
                image.sprite = scriptable.IconSprite;
                button.onClick.AddListener(() => { BeginPlacement(placeable.Name, placeable.Id); });
                _placeableEntryList.Add(button);
            }
        }

		private void UpdateProductEntry(int productId, int amountChanged)
        {
            var entry = _productEntryList.FirstOrDefault(i => i.ProductId == productId);
	        if (entry == null)
				return;

	        entry.SetAmount(_inventory.GetProductCurrentAmount(productId));
	        entry.PulseColorFrom(amountChanged > 0 ? _increaseColor : _decreaseColor, _pulseTime);
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

	    private void BeginPlacement(string placeableName, int inventoryId)
        {
            _placer.BeginPlacement(placeableName, inventoryId);
        }
    }
}