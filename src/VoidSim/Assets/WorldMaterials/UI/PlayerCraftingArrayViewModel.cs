using System.Collections.Generic;
using System.Linq;
using Assets.Controllers.GUI;
using Assets.Placeables.Nodes;
using Assets.Scripts;
using Assets.Station;
using Messaging;
using QGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.WorldMaterials.UI
{
    /// <summary>
    /// Listens for product factory created messages, and adds UI elements for the ones that come through with IsInPlayerArray true.
    /// Creates a ProductFactoryViewModel for each entry, which handles binding an individual factory to the controls.
    /// </summary>
    public class PlayerCraftingArrayViewModel : QScript
    {
        [SerializeField] private Image _arrayPrefab;
        [SerializeField] private Image _factoryPrefab;
        private Image _arrayPanel;
        private Image _arrayContent;
        private bool _hasAFactory;
	    private FactoryControl _control;
		private readonly List<ProductFactoryViewModel> _children = new List<ProductFactoryViewModel>();

	    void Start()
        {
            CreatePanel();
            _arrayPanel.gameObject.SetActive(false);
            gameObject.RegisterSystemPanel(_arrayPanel.gameObject);
        }

	    public void Bind(FactoryControl control)
	    {
		    _control = control;
		    _control.OnFactoryListUpdated += RefreshFactories;
	    }

	    private void CreatePanel()
        {
            var canvas = GameObject.Find("InfoCanvas");
            _arrayPanel = Instantiate(_arrayPrefab, canvas.transform, false);
            _arrayContent = _arrayPanel.transform.FindChild("content_holder").GetComponent<Image>();
        }

   //     private void HandleFactoryAdd(ProductFactoryMessageArgs args)
   //     {   
   //         if (args == null || args.ProductFactory == null || !args.ProductFactory.IsInPlayerArray) return;

   //         var go = Instantiate(_factoryPrefab, _arrayContent.transform, false);
   //         var viewmodel = go.gameObject.GetOrAddComponent<ProductFactoryViewModel>();
   //         viewmodel.Bind(args.ProductFactory);
			

			//// Open the UI panel if this is the first factory, tis brittle
   //         if (!_hasAFactory)
   //         {
	  //          SignalFirstFactoryPlaced();
   //         }
   //     }

	    private void RefreshFactories()
	    {
		    _children.ForEach(i => Destroy(i.gameObject));
			_children.Clear();

		    foreach (var factory in _control.Factories)
		    {
			    if(!factory.IsInPlayerArray)
					continue;

			    var go = Instantiate(_factoryPrefab, _arrayContent.transform, false);
			    var viewmodel = go.gameObject.GetOrAddComponent<ProductFactoryViewModel>();
			    viewmodel.Bind(factory);
				_children.Add(viewmodel);
			}

			if(!_hasAFactory && _children.Any())
				SignalFirstFactoryPlaced();
	    }

		// will open the UI panel if this is the first factory placed
		private void SignalFirstFactoryPlaced()
	    {
		    var go = GameObject.Find("binder_player_crafting_array_viewmodel");
			if(go == null)
				return;
			var binder = go.GetComponent<ToggleButtonPressBinder>();
		    binder.Toggle.isOn = true;
		    _hasAFactory = true;
	    }
    }
}