using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.WorldMaterials;
using Assets.WorldMaterials;
using Assets.WorldMaterials.UI;
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

/// <summary>
/// Serving as an anchor for all the other game parts as they come together.
/// Currently instantiates player's crafting parts and binds UI interactions
/// </summary>
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
    [SerializeField]
    private Inventory _inventoryPrefab;

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
        InstantiateInventory();
        BindInventoryToUI();
        InstantiateCraftingContainer();
        BindCraftingToShop();
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
        _crafter = crafter;

        // remove once crafting UI is complete
        Locator.ValueDisplay.Add("Crafting Queue", () => _crafter.CurrentQueueCount);
        Locator.ValueDisplay.Add("Current Craft", () => _crafter.CurrentCraftRemainingAsZeroToOne);
    }

    private void BindCraftingToShop()
    {
        // find the viewmodel and bind to it
        var viewmodel = _crafter.gameObject.GetComponent<PlayerCraftingViewModel>();
        viewmodel.Bind(_productLookup.GetRecipes(), _crafter, _inventory);
    }

    private void InstantiateInventory()
    {
        _inventory = GameObject.Instantiate(_inventoryPrefab);
        if(_inventory == null || _inventoryScriptable == null)
            throw new UnityException("Station inventory missing a dependency");
        _inventory.transform.SetParent(transform);
        _inventory.BindToScriptable(_inventoryScriptable, _productLookup);
    }

    private void BindInventoryToUI()
    {
        var go = (GameObject)Instantiate(Resources.Load("Prefabs/UI/inventory_viewmodel"));
        var viewmodel = go.GetOrAddComponent<InventoryViewModel>();
        viewmodel.BindToInventory(_inventory);
    }

    public StationLayer GetLayer(LayerType type)
    {
        StationLayer toReturn = null;
        _layers.TryGetValue(type, out toReturn);
        return toReturn;
    }
}
