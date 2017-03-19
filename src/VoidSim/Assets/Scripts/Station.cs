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
    private Image _craftingViewContext;
    [SerializeField]
    private Button _buttonPrefab;

    [SerializeField]
    private StationLayer[] _initialLayers;
    [SerializeField]
    private GameObject _craftingPrefab;

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
        if(_craftingViewContext == null || _buttonPrefab == null)
            throw new UnityException("Crafting UI ref missing");

        foreach (var recipe in _productLookup.GetRecipes())
        {
            var button = GameObject.Instantiate(_buttonPrefab).GetComponent<Button>();
            var text = button.GetComponentInChildren<Text>();
            text.text = recipe.Ingredients.Aggregate(string.Format("{0}\t\t", recipe.ResultProduct), (content, ing)
                => content + string.Format("{0} {1} ", ing.Quantity, ing.ProductName));
            button.transform.parent = _craftingViewContext.transform;
            button.onClick.AddListener( () =>
            {
                _crafter.QueueCrafting(recipe);
            });
        }
    }

    private void OnCraftingComplete(Recipe recipe)
    {
        Debug.Log(string.Format("{0}: Craft Complete", recipe.ResultProduct));
    }

    public void CraftSomething()
    {
        _crafter.QueueCrafting(_productLookup.GetRecipes().FirstOrDefault());
    }

    public StationLayer GetLayer(LayerType type)
    {
        StationLayer toReturn = null;
        _layers.TryGetValue(type, out toReturn);
        return toReturn;
    }
}
