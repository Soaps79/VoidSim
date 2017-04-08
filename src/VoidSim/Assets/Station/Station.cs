using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.WorldMaterials;
using Assets.Station;
using Assets.WorldMaterials;
using Assets.WorldMaterials.UI;
using Messaging;
using QGame;
using UnityEngine;
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
        InstantiateInventory();
        BindInventoryToUI();
        InstantiateCraftingContainer();
        BindCraftingToShop();
        TestAutomatedContainer();
        TestPowerGrid();
        RegisterEnergySupplyMonitor();
    }

    private void RegisterEnergySupplyMonitor()
    {
        var product = _productLookup.GetProduct("Energy");
        if (product == null)
            throw new UnityException("Energy product not found in lookup");

        MessageHub.Instance.QueueMessage(ProductSupplyMonitor.CreatedMessageType,
            new ProductSupplyMonitorCreatedMessageArgs
            {
                SupplyMonitor = new ProductSupplyMonitor(product, _inventory, TimeUnit.Hour, TimeUnit.Day)
            });
    }

    private void TestPowerGrid()
    {
        // bind to station's inventory
        var go = new GameObject();
        go.name = "power_grid";
        go.transform.SetParent(transform);
        var grid = go.GetOrAddComponent<PowerGrid>();
        grid.Initialize(_inventory);

        // fire fake message to test consumption
        MessageHub.Instance.QueueMessage(
            PlaceableMessages.PlaceablePlacedMessageName, 
            new PlaceablePlacedArgs { ObjectPlaced = new Placeable(3) });
    }

    private void TestAutomatedContainer()
    {
        var go = new GameObject();
        go.name = "automated_container";
        go.transform.SetParent(transform);
        var crafter = go.GetOrAddComponent<AutomatedContainer>();
        crafter.Initialize("Power Plant", _inventory);
        crafter.BeginCrafting("Energy");
    }

    private void InstantiateCraftingContainer()
    {
        // standard unity instantiation
        var crafterGo = new GameObject();
        crafterGo.name = "crafting_container";
        crafterGo.transform.SetParent(transform);
        var crafter = crafterGo.GetOrAddComponent<CraftingContainer>();

        // until single instances are figured out
        crafter.WorldClock = _worldClock;

        // bind to container
        crafter.Info = _productLookup.GetContainers().FirstOrDefault();
        _crafter = crafter;

        // remove once crafting UI is complete
        KeyValueDisplay.Instance.Add("Crafting Queue", () => _crafter.CurrentQueueCount);
        KeyValueDisplay.Instance.Add("Current Craft", () => _crafter.CurrentCraftRemainingAsZeroToOne);
    }

    private void BindCraftingToShop()
    {
        // find the viewmodel and bind to it
        var go = (GameObject) Instantiate(Resources.Load("Views/player_crafting_viewmodel"));
        go.transform.parent = transform;
        go.name = "player_crafting_viewmodel";
        var viewmodel = go.GetOrAddComponent<PlayerCraftingViewModel>();
        viewmodel.Bind(_productLookup.GetRecipes(), _crafter, _inventory);
    }

    private void InstantiateInventory()
    {
        var go = new GameObject();
        go.transform.parent = transform;
        go.name = "inventory";
        _inventory = go.GetOrAddComponent<Inventory>();
        if(_inventory == null || _inventoryScriptable == null)
            throw new UnityException("Station inventory missing a dependency");
        _inventory.transform.SetParent(transform);
        _inventory.BindToScriptable(_inventoryScriptable, _productLookup);
    }

    private void BindInventoryToUI()
    {
        var go = (GameObject)Instantiate(Resources.Load("Views/inventory_viewmodel"));
        go.transform.parent = transform;
        go.name = "inventory_viewmodel";
        var viewmodel = go.GetOrAddComponent<InventoryViewModel>();
        viewmodel.BindToInventory(_inventory, _inventoryScriptable);
    }

    public StationLayer GetLayer(LayerType type)
    {
        StationLayer toReturn = null;
        _layers.TryGetValue(type, out toReturn);
        return toReturn;
    }
}
