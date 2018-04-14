using System.Collections.Generic;
using System.Linq;
using Assets.Controllers.GUI;
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
	    [SerializeField] private Toggle _removePrefab;
        [SerializeField] private Image _inventoryPanelPrefab;
        [SerializeField] private readonly List<ProductCategory> _productsToIgnore = new List<ProductCategory>();
	    [SerializeField] private Color _increaseColor;
	    [SerializeField] private Color _decreaseColor;
	    [SerializeField] private float _pulseTime;
	    [SerializeField] private Sprite _removeSprite;
	    private Color _normalColor;

		private Transform _productContentHolder;
        private Transform _placeablesContentHolder;
        private StationInventory _stationInventory;
        private InventoryReserve _inventoryReserve;
        private readonly List<ProductEntryBinder> _productEntryList = new List<ProductEntryBinder>();
        private readonly List<Button> _placeableEntryList = new List<Button>();
        private PlaceablesLookup _placeablesLookup;
        private UserPlacement _userPlacement;

        public void BindToInventory(StationInventory stationInventory, InventoryScriptable inventoryScriptable, PlaceablesLookup placeablesLookup, InventoryReserve inventoryReserve, UserPlacement userPlacement)
        {
            _stationInventory = stationInventory;
            _stationInventory.Products.OnProductsChanged += UpdateProductEntry;
            _placeablesLookup = placeablesLookup;
            _inventoryReserve = inventoryReserve;

	        _userPlacement = userPlacement;
	        _userPlacement.OnPlacementComplete += HandlePlacementComplete;

			UpdateIgnoreList(inventoryScriptable);
            BindToUI();
        }

        private void HandlePlacementComplete(int id)
        {
            if(id <= 0)
                return;

            _stationInventory.TryRemovePlaceable(id);
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
			var canvas = Locator.CanvasManager.GetCanvas(CanvasType.MediumUpdate);
            //_display = Instantiate(_displayPanelPrefab, canvas.transform, false);

            var craftingPanel = Instantiate(_inventoryPanelPrefab, canvas.transform, false);
            _productContentHolder = craftingPanel.transform.Find("content_holder/product_list");
            _placeablesContentHolder = craftingPanel.transform.Find("content_holder/placeable_list");

            gameObject.RegisterSystemPanel(craftingPanel.gameObject);

			DrawRemoveToggle();
			DrawProductEntries();
            DrawPlaceableEntries();
        }

        private void DrawProductEntries()
        {
            foreach (var entryInfo in _stationInventory.Products.GetProductEntries())
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
	        foreach (var placeable in _stationInventory.Placeables)
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

		// adds remove button to inventory, only added once
	    private void DrawRemoveToggle()
	    {
		    var toggle = Instantiate(_removePrefab, _placeablesContentHolder.transform, false);
		    var binder = toggle.gameObject.AddComponent<ToggleButtonPressBinder>();
			binder.Bind(toggle, "Cancel", true);
		    toggle.onValueChanged.AddListener(_userPlacement.HandleRemoveToggle);
	    }

	    private void UpdateProductEntry(int productId, int amountChanged)
        {
            var entry = _productEntryList.FirstOrDefault(i => i.ProductId == productId);
	        if (entry == null)
				return;

	        entry.SetAmount(_stationInventory.Products.GetProductCurrentAmount(productId));
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
            _userPlacement.BeginPlacement(placeableName, inventoryId);
        }
    }
}