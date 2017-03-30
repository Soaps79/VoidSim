﻿using System;
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
    }

    private void TestAutomatedContainer()
    {
        var crafterGo = new GameObject();
        crafterGo.name = "automated_container";
        crafterGo.transform.SetParent(transform);
        var crafter = crafterGo.GetOrAddComponent<AutomatedContainer>();
        crafter.Initialize("Small Factory", _inventory);
        crafter.BeginCrafting("Ammunition");
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
        viewmodel.BindToInventory(_inventory);
    }

    public StationLayer GetLayer(LayerType type)
    {
        StationLayer toReturn = null;
        _layers.TryGetValue(type, out toReturn);
        return toReturn;
    }
}
