using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.WorldMaterials.UI
{
    public class InventoryViewModel : MonoBehaviour
    {
        [SerializeField]
        private Image _productEntryPrefab;
        [SerializeField]
        private Image _inventoryPanelPrefab;

        private Transform _contentHolder;
        private Inventory _inventory;
        private readonly List<GameObject> _entryList = new List<GameObject>();

        public void BindToInventory(Inventory inventory)
        {
            _inventory = inventory;
            _inventory.OnProductsChanged += UpdateEntries;

            BindToUI();
        }

        private void BindToUI()
        {
            var craftingPanel = GameObject.Instantiate(_inventoryPanelPrefab);
            var list = craftingPanel.transform.FindChild("content_holder/product_list");
            _contentHolder = list;

            PositionOnCanvas(craftingPanel);
            DrawEntries();
        }

        private void DrawEntries()
        {
            foreach (var entryInfo in _inventory.GetProductEntries())
            {
                var go = Instantiate(_productEntryPrefab).gameObject;
                go.transform.SetParent(_contentHolder.transform);
                var binder = go.gameObject.GetOrAddComponent<ProductEntryBinder>();
                binder.Bind(entryInfo.Product.Name, entryInfo.Amount);
                _entryList.Add(go);
            }
        }

        private void UpdateEntries(string name, int amount)
        {
            ClearEntries();
            // save incoming values to light up entry
            DrawEntries();
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
            craftingPanel.rectTransform.position = new Vector3(10, 500, 0);
        }
    }
}