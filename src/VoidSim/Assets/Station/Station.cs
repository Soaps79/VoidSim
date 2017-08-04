using System.Collections.Generic;
using System.Linq;
using Assets.Placeables;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Assets.Scripts.WorldMaterials;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.Trade;
using Assets.WorldMaterials.UI;
using Messaging;
using QGame;
using UnityEngine;
using Zenject;
using TimeUnit = Assets.Scripts.TimeUnit;
using WorldClock = Assets.Scripts.WorldClock;

namespace Assets.Station
{
    public enum LayerType
    {
        Top, Middle, Bottom, Core
    }

    /// <summary>
    /// Serving as an anchor for all the other game parts as they come together.
    /// Currently instantiates player's crafting parts and binds UI interactions
    /// </summary>
    public class Station : QScript, IMessageListener
    {
        public const string ClientName = "Station";

        [Inject] private WorldClock _worldClock;
        [Inject] private ProductLookup _productLookup;

        [SerializeField] private InventoryScriptable _inventoryScriptable;
        [SerializeField] private PlaceablesLookup _placeablesLookup;
        [SerializeField] private TraderRequestsSO _voidTradeRequests;
	    [SerializeField] private CargoControl _cargoControlPrefab;

        private CraftingContainer _crafter;
        private Inventory _inventory;

	    private readonly CollectionSerializer<InventoryData> _inventorySerializer
		    = new CollectionSerializer<InventoryData>();

	    private const string _invCollectionName = "StationInventory";
		private InventoryReserve _inventoryReserve;

        // _initialLayers for editor, converted to _layers
        [SerializeField]
        private List<StationLayer> _initialLayers;
        private readonly Dictionary<LayerType, StationLayer> _layers = new Dictionary<LayerType, StationLayer>();
	    private PopulationControl _populationControl;

	    void Start()
        {
			Locator.MessageHub.AddListener(this, GameMessages.PreSave);

            MapLayers();
            InstantiateInventory();
	        InstantiatePopulationControl();
            InstantiateTrader();
            BindInventoryToUI();
            InstantiateCraftingContainer();
            BindCraftingToShop();
            BindFactoryControl();

            InstantiateCargoControl();

            TestPowerGrid();
            RegisterSupplyMonitors();

            InitializeLayers();
        }

        private void MapLayers()
        {
            foreach (var layer in _initialLayers)
            {
                _layers.Add(layer.LayerType, layer);
            }
        }

        // fulfill layer dependencies
        private void InitializeLayers()
        {
            foreach (var layer in _layers)
            {
                layer.Value.Initialize(this, _inventory);
            }
        }

        // hook energy and pop up to UI display
        private void RegisterSupplyMonitors()
        {
            var product = _productLookup.GetProduct("Energy");
            if (product == null)
                throw new UnityException("Energy product not found in lookup");

            Locator.MessageHub.QueueMessage(ProductSupplyMonitor.CreatedMessageType,
                new ProductSupplyMonitorCreatedMessageArgs
                {
                    SupplyMonitor = new ProductSupplyMonitor(new ProductSupplyMonitor.Data
                    {
                        Product = product,
                        Inventory = _inventory,
                        SupplyUpdatefrequency = TimeUnit.Hour,
                        ChangeUpdateFrequency = TimeUnit.Day,
                        Mode = ProductSupplyDisplayMode.Difference
                    })
                });

            product = _productLookup.GetProduct("Population");
            if (product == null)
                throw new UnityException("Population product not found in lookup");

            Locator.MessageHub.QueueMessage(ProductSupplyMonitor.CreatedMessageType,
                new ProductSupplyMonitorCreatedMessageArgs
                {
                    SupplyMonitor = new ProductSupplyMonitor(new ProductSupplyMonitor.Data
                    {
                        Product = product,
                        Inventory = _inventory,
                        SupplyUpdatefrequency = TimeUnit.Hour,
                        ChangeUpdateFrequency = TimeUnit.Day,
                        Mode = ProductSupplyDisplayMode.OutOfMax,
                        DisplayName = "Pop"
                    })
                });

            product = _productLookup.GetProduct("Credits");
            if (product == null)
                throw new UnityException("Credits product not found in lookup");

            Locator.MessageHub.QueueMessage(ProductSupplyMonitor.CreatedMessageType,
                new ProductSupplyMonitorCreatedMessageArgs
                {
                    SupplyMonitor = new ProductSupplyMonitor(new ProductSupplyMonitor.Data
                    {
                        Product = product,
                        Inventory = _inventory,
                        SupplyUpdatefrequency = TimeUnit.Hour,
                        ChangeUpdateFrequency = TimeUnit.Day,
                        Mode = ProductSupplyDisplayMode.SupplyOnly,
                        DisplayName = "Pop"
                    })
                });
        }

        // instantiate a PowerGrid
        private void TestPowerGrid()
        {
            var go = new GameObject();
            go.name = "power_grid";
            go.transform.SetParent(_layers[LayerType.Core].transform);
            var grid = go.GetOrAddComponent<PowerGrid>();
            grid.Initialize(_inventory);
        }

        private void InstantiatePopulationControl()
        {
            var go = new GameObject();
            go.name = "population_control";
            go.transform.SetParent(_layers[LayerType.Core].transform);
            var pop = go.GetOrAddComponent<PopulationControl>();
            pop.Initialize(_inventory, 60);
	        _populationControl = pop;
        }

        // Crafting items here are temporary. They will eventually work entirely through Placeables
        private void InstantiateCraftingContainer()
        {
            // standard unity instantiation
            var crafterGo = new GameObject();
            crafterGo.name = "crafting_container";
            crafterGo.transform.SetParent(_layers[LayerType.Core].transform);
            var crafter = crafterGo.GetOrAddComponent<CraftingContainer>();

            // until single instances are figured out
            crafter.WorldClock = _worldClock;

            // bind to container
            crafter.Info = _productLookup.GetContainers().FirstOrDefault();
            _crafter = crafter;
        }

        // expose build menu to player
        private void BindCraftingToShop()
        {
            var go = (GameObject)Instantiate(Resources.Load("Views/player_crafting_viewmodel"));
            go.transform.SetParent(_layers[LayerType.Core].transform);
            go.name = "player_crafting_viewmodel";
            var viewmodel = go.GetOrAddComponent<PlayerCraftingViewModel>();
            viewmodel.Bind(_productLookup.GetRecipes(), _crafter, _inventory);
        }

        private void BindFactoryControl()
        {
            var go = new GameObject();
	        go.transform.SetParent(_layers[LayerType.Core].transform);
            go.name = "factory_control";
	        var control = go.AddComponent<FactoryControl>();
			control.Initialize(_inventory, _productLookup, _inventoryReserve);
		}

		private void InstantiateCargoControl()
        {
	        var cargo = Instantiate(_cargoControlPrefab);
            cargo.transform.SetParent(_layers[LayerType.Core].transform);
            cargo.name = "cargo_control";
            cargo.Initialize(_inventory, _inventoryReserve, ClientName);
        }

        // centralized inventory for the station
        private void InstantiateInventory()
        {
            var go = new GameObject();
            go.transform.SetParent(_layers[LayerType.Core].transform);
            go.name = "inventory";
            _inventory = go.GetOrAddComponent<Inventory>();
            if (_inventory == null || _inventoryScriptable == null)
                throw new UnityException("Station inventory missing a dependency");

			// check to see if game is loading, if not, use presets from scripable object
	        if (_inventorySerializer.HasDataFor(_inventory, "StationInventory"))
		        _inventory.Initialize(_inventorySerializer.DeserializeData(), _productLookup, true);
	        else
		        _inventory.Initialize(_inventoryScriptable, _productLookup, true);

	        var product = _productLookup.GetProduct("Credits");
            _inventory.SetProductMaxAmount(product.ID, 1000000);

            product = _productLookup.GetProduct("Energy");
            _inventory.SetProductMaxAmount(product.ID, 1000000);

            _inventoryReserve = new InventoryReserve();
            _inventoryReserve.Initialize(_inventory);
        }

        private void InstantiateTrader()
        {
            var go = new GameObject();
	        go.name = "station_trader";
            go.transform.SetParent(_layers[LayerType.Core].transform);
            var trader = go.AddComponent<StationTrader>();
            trader.ClientName = ClientName;
            trader.Initialize(_inventory, _inventoryReserve);
        }

        // convert editor-friendly objects to more usable dictionary
        private void BindInventoryToUI()
        {
            var go = (GameObject)Instantiate(Resources.Load("Views/inventory_viewmodel"));
            go.transform.SetParent(_layers[LayerType.Core].transform);
            go.name = "inventory_viewmodel";
            var viewmodel = go.GetOrAddComponent<InventoryViewModel>();
            viewmodel.BindToInventory(_inventory, _inventoryScriptable, _placeablesLookup, _inventoryReserve);
        }

	    public void HandleMessage(string type, MessageArgs args)
	    {
	    }

	    public string Name { get { return "Station"; } }
    }
}