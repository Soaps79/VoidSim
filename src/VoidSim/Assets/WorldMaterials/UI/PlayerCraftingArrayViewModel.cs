using System.Collections.Generic;
using Assets.Placeables.Nodes;
using Assets.Scripts;
using Messaging;
using QGame;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.WorldMaterials.UI
{
    /// <summary>
    /// Listens for product factory created messages, and adds UI elements for the ones that come through with IsInPlayerArray true.
    /// Creates a ProductFactoryViewModel for each entry, which handles binding an individual factory to the controls.
    /// </summary>
    public class PlayerCraftingArrayViewModel : QScript, IMessageListener
    {
        [SerializeField] private Image _arrayPrefab;
        [SerializeField] private Image _factoryPrefab;
        private List<ProductFactory> _factories = new List<ProductFactory>();
        private Image _arrayPanel;
        private Image _arrayContent;

        void Start()
        {
            MessageHub.Instance.AddListener(this, ProductFactory.MessageName);
            CreatePanel();
            _arrayPanel.gameObject.SetActive(false);
            OnEveryUpdate += CheckForVisibilityToggle;
        }

        private void CheckForVisibilityToggle(float delta)
        {
            if (Input.GetButtonDown("FactoryArray"))
            {
                _arrayPanel.gameObject.SetActive(!_arrayPanel.gameObject.activeSelf);
            }
        }

        private void CreatePanel()
        {
            _arrayPanel = Instantiate(_arrayPrefab);
            var canvas = GameObject.Find("InfoCanvas");
            _arrayPanel.transform.SetParent(canvas.transform);
            _arrayPanel.rectTransform.position = new Vector3(500, 240, 0);
            _arrayContent = _arrayPanel.transform.FindChild("content_holder").GetComponent<Image>();
        }

        public void HandleMessage(string type, MessageArgs args)
        {
            if (type == ProductFactory.MessageName)
                OnNextUpdate += (d) => HandleFactoryAdd(args as ProductFactoryMessageArgs);
        }

        private void HandleFactoryAdd(ProductFactoryMessageArgs args)
        {   
            if (args == null || args.ProductFactory == null || !args.ProductFactory.IsInPlayerArray) return;

            if (!_arrayPanel.gameObject.activeSelf)
                _arrayPanel.gameObject.SetActive(true);
            
            var go = Instantiate(_factoryPrefab);
            go.transform.SetParent(_arrayContent.transform);

            var viewmodel = go.gameObject.GetOrAddComponent<ProductFactoryViewModel>();
            viewmodel.Bind(args.ProductFactory);
        }

        public string Name { get; private set; }
    }
}