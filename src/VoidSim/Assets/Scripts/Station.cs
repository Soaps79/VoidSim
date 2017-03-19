using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.WorldMaterials;
using QGame;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public enum LayerType
{
    Top, Middle, Bottom
}

[Serializable]
public class StationLayer
{
    public LayerType Type;
    public int BuildingGridCapacity;
}

public class Station : QScript
{
    [Inject]
    private WorldClock _worldClock;
    [Inject]
    private ProductLookup _productLookup;

    [SerializeField]
    private StationLayer[] _initialLayers;
    [SerializeField]
    private GameObject _craftingPrefab;
    [SerializeField]
    private Inventory _inventory;

    private readonly Dictionary<LayerType, StationLayer> _layers =  new Dictionary<LayerType, StationLayer>();
    private CraftingContainer _crafter;
    

    void Awake()
    {
        if (_initialLayers != null)
        {
            foreach (var initialLayer in _initialLayers)
            {
                _layers.Add(initialLayer.Type, initialLayer);
            }
        }
    }

    // Use this for initialization
    void Start ()
    {
        CreateCraftingContainer();
        BindToUI();
        BindToInventory();
    }

    private void CreateCraftingContainer()
    {
        // testing container basic functionality
        var crafterGo = GameObject.Instantiate(_craftingPrefab);
        crafterGo.transform.SetParent(transform);
        var crafter = crafterGo.GetOrAddComponent<CraftingContainer>();
        crafter.Info = _productLookup.GetContainers().FirstOrDefault();
        // Move this to DI or Locator
        crafter.WorldClock = _worldClock;
        crafter.OnCraftingComplete += OnCraftingComplete;
        _crafter = crafter;

        Locator.ValueDisplay.Add("Crafting Queue", () => _crafter.CurrentQueueCount);
        Locator.ValueDisplay.Add("Current Craft", () => _crafter.CurrentCraftRemainingAsZeroToOne);
    }

    private void BindToUI()
    {
        // find the viewmodel and bind to it
        var viewmodel = _crafter.gameObject.GetComponent<CraftingViewModel>();
        viewmodel.Bind(_productLookup.GetRecipes(), _crafter);
    }

    private void BindToInventory()
    {
        
    }

    private void OnCraftingComplete(Recipe recipe)
    {
        Debug.Log(string.Format("{0}: Craft Complete", recipe.ResultProduct));
    }

    public StationLayer GetLayer(LayerType type)
    {
        StationLayer toReturn = null;
        _layers.TryGetValue(type, out toReturn);
        return toReturn;
    }
}
