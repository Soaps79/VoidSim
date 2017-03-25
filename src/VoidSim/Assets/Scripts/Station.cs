using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.WorldMaterials;
using Assets.WorldMaterials;
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
    private GameObject _craftingPrefab;
    [SerializeField]
    private InventoryScriptable _inventoryScriptable;

    private readonly Dictionary<LayerType, StationLayer> _layers =  new Dictionary<LayerType, StationLayer>();
    private CraftingContainer _crafter;
    private Inventory _inventory;

    // placeholder
    [SerializeField]
    private StationLayer[] _initialLayers;


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

    void Start ()
    {
        InstantiateCraftingContainer();
        BindCraftingToUI();
        InstantiateInventory();
        BindInventoryToUI();
    }

    private void InstantiateCraftingContainer()
    {
        // standard unity instantiation
        var crafterGo = GameObject.Instantiate(_craftingPrefab);
        crafterGo.transform.SetParent(transform);
        var crafter = crafterGo.GetOrAddComponent<CraftingContainer>();

        // until single instances are figured out
        crafter.WorldClock = _worldClock;

        // bind to container
        crafter.Info = _productLookup.GetContainers().FirstOrDefault();
        crafter.OnCraftingComplete += OnCraftingComplete;
        _crafter = crafter;

        // remove once crafting UI is complete
        Locator.ValueDisplay.Add("Crafting Queue", () => _crafter.CurrentQueueCount);
        Locator.ValueDisplay.Add("Current Craft", () => _crafter.CurrentCraftRemainingAsZeroToOne);
    }

    private void BindCraftingToUI()
    {
        // find the viewmodel and bind to it
        var viewmodel = _crafter.gameObject.GetComponent<CraftingViewModel>();
        viewmodel.Bind(_productLookup.GetRecipes(), _crafter);
    }

    private void InstantiateInventory()
    {
        // standard unity instantiation
        var crafterGo = GameObject.Instantiate(_craftingPrefab);
        crafterGo.transform.SetParent(transform);
        var crafter = crafterGo.GetOrAddComponent<Inventory>();
    }

    private void BindInventoryToUI()
    {
        
    }

    private void OnCraftingComplete(Recipe recipe)
    {
        Debug.Log(string.Format("Craft Complete: {0}", recipe.ResultProduct));
    }

    public StationLayer GetLayer(LayerType type)
    {
        StationLayer toReturn = null;
        _layers.TryGetValue(type, out toReturn);
        return toReturn;
    }
}
