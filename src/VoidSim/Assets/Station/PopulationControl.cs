using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Logistics;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Assets.Station.Population;
using Assets.Station.UI;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Population;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.Trade;
using Messaging;
using QGame;
using UnityEngine;

namespace Assets.Station
{
	public class PopulationData
	{
		public int CurrentCount;
		public int Capacity;
		public int InboundPopulation;
	    public List<PersonData> Population;
	}

	/// <summary>
	/// Bird's eye view of population; including housing, employment, and pops themselves
	/// </summary>
	public class PopulationControl : QScript, IPopulationHost, ISerializeData<PopulationData>, ITraderDriver, IMessageListener
	{
		public string Name { get { return "PopulationControl"; } }
		[SerializeField] private int _totalCapacity;
		[SerializeField] private int _baseCapacity;
		[SerializeField] private int _currentCount;
	    [SerializeField] private int _countNamesToLoad;

	    // Population is currently stored in the Station's inventory as a Product
		// Allows it to be handled by trade, cargo, etc
		private WorldMaterials.StationInventory _stationInventory;
		private int _populationProductId;
		
		// When there is room for more population, 
		// it is requested through this Trader which is hooked into the trade system
		private ProductTrader _trader;
		private int _inboundPopulation;

		private PopulationSO _scriptable;
		private bool _ignoreNeeds;

		public MoodManager MoodManager { get; private set; }

		// serialization
		private readonly CollectionSerializer<PopulationData> _serializer
			= new CollectionSerializer<PopulationData>();
		private PopulationData _deserialized;
		private const string _collectionName = "PopulationControl";

        private readonly PersonGenerator _personGenerator = new PersonGenerator();
	    private readonly List<IPopMonitor> _peopleHandlers = new List<IPopMonitor>();

        public readonly List<Person> AllPopulation = new List<Person>();

	    [SerializeField] private PopulationListViewModel _listViewModelPrefab;
        private PopulationListViewModel _listViewModel;

	    //public Action<List<Person>, bool> OnPopulationUpdated;

	    public void Initialize(WorldMaterials.StationInventory stationInventory, PopulationSO scriptable)
		{
		    _scriptable = scriptable;
			_stationInventory = stationInventory;
			_stationInventory.Products.OnProductsChanged += HandleInventoryProductChanged;
            OnNextUpdate += () => _stationInventory.Products.OnProductMaxAmountChanged += HandleInventoryMaxAmountChanged;

			_populationProductId = ProductIdLookup.Population;
			_currentCount = _stationInventory.Products.GetProductCurrentAmount(_populationProductId);
			_baseCapacity = scriptable.BaseCapacity;

			// still temporary values until system is worked out
			CurrentQualityOfLife = 10;

			// establish sub-objects
			MoodManager = new MoodManager(scriptable.MoodParams, stationInventory);
		    _personGenerator.Initialize(scriptable.GenerationParams);

		    InitializeEmployment();
            InitializeMover();
            InitializeHouser();

		    // load or set defaults
            if (_serializer.HasDataFor(this, _collectionName))
				LoadFromFile();
			else
				LoadFromScriptable();
			_stationInventory.Products.SetProductMaxAmount(_populationProductId, scriptable.BaseCapacity);

			InitializeProductTrader();

            InitializeListView();
		}

	    private void InitializeListView()
	    {
	        if (_listViewModelPrefab == null)
	            return;
	        var canvas = Locator.CanvasManager.GetCanvas(CanvasType.MediumUpdate);
            _listViewModel = Instantiate(_listViewModelPrefab, canvas.transform, false);
	        _listViewModel.Initialize();
            _listViewModel.UpdateList(AllPopulation);
	        _listViewModel.gameObject.SetActive(false);
	    }

        private void InitializeEmployment()
	    {
	        var employer = GetComponent<PopEmployerMonitor>();
            employer.Initialize(this, _scriptable, MoodManager.EfficiencyModule);
            _peopleHandlers.Add(employer);
	    }

	    private void InitializeMover()
	    {
	        var mover = GetComponent<PeopleMover>();
	        mover.Initialize(this);
	        _peopleHandlers.Add(mover);
	    }

	    private void InitializeHouser()
	    {
	        var houser = GetComponent<PopHomeMonitor>();
	        houser.Initialize(this, _stationInventory, _scriptable);
	        _peopleHandlers.Add(houser);
	    }

	    private void LoadFromScriptable()
		{
			_stationInventory.Products.TryAddProduct(_populationProductId, _scriptable.InitialCount);
		    var people = _personGenerator.GeneratePeople(_scriptable.InitialCount);
            // move this into scriptable? maybe a percentage?
		    people.ForEach(i => i.IsResident = true);
            AddPopulation(people);
        }

	    private void AddPopulation(List<Person> people)
	    {
	        AllPopulation.AddRange(people);
            _peopleHandlers.ForEach(i => i.HandlePopulationUpdate(people, true));
            if(_listViewModel != null)
                _listViewModel.UpdateList(AllPopulation);
        }

	    private void LoadFromFile()
		{
			_deserialized = _serializer.DeserializeData();
			_currentCount = _deserialized.CurrentCount;
			_inboundPopulation = _deserialized.InboundPopulation;

		    var people = _personGenerator.DeserializePopulation(_deserialized.Population);

            if(_stationInventory.Products.GetProductCurrentAmount(ProductIdLookup.Population) != people.Count())
				throw new UnityException("PopControl data not matching station inventory");
	        AllPopulation.AddRange(people);
		    _peopleHandlers.ForEach(i => i.HandleDeserialization(people));
        }

	    private void InitializeProductTrader()
		{
			_trader = gameObject.AddComponent<ProductTrader>();
		    _trader.OnResume += HandleNewManifest;
			_trader.Initialize(this, Station.ClientName);
			UpdateTradeRequest();
		}

        // for handling PopHomeMonitor adding/removing housing
	    private void HandleInventoryMaxAmountChanged(int productId, int amount)
	    {
            if(productId == _populationProductId)
                UpdateTradeRequest();
	    }

        // Hooked into _inventory's update event
        private void HandleInventoryProductChanged(int productId, int amount)
		{
			if (productId != _populationProductId)
				return;

			_currentCount = _stationInventory.Products.GetProductCurrentAmount(_populationProductId);
		}

		// checks to see if inventory has room for more pop (discounting those already in transit)
		private void UpdateTradeRequest()
		{
			var remaining = _stationInventory.Products.GetProductRemainingSpace(_populationProductId);
			remaining -= _inboundPopulation;
			_trader.SetConsume(new ProductAmount
			{
				ProductId = _populationProductId,
				Amount = remaining > 0  ? remaining : 0
			});
		}

		public void HandleMessage(string type, MessageArgs args)
		{
		
		}

		// will cause mood to be ignored, always returning 100%
		public bool IgnoreNeeds
		{
			get { return _ignoreNeeds; }
			set
			{
				if (_ignoreNeeds == value)
					return;

				MoodManager.SetIgnoreNeeds(value);
				_ignoreNeeds = value;
			}
		}

		public float CurrentQualityOfLife { get; private set; }

		public bool PopulationWillMigrateTo(IPopulationHost otherHost)
		{
			return false;
		}

		// since there are people en-route, remove them from current trade request
		public bool WillConsumeFrom(ProductTrader provider, ProductAmount provided)
		{
			if(provided.ProductId != _populationProductId)
				throw new UnityException("PopulationControl offered trade that was not population");

			return true;
		}

		public bool WillProvideTo(ProductTrader consumer, ProductAmount provided)
		{
			if (provided.ProductId != _populationProductId)
				throw new UnityException("PopulationControl offered trade that was not population");

			return true;
		}

		// request cargo passage for pop
		public void HandleProvideSuccess(TradeManifest manifest)
		{
			Locator.MessageHub.QueueMessage(LogisticsMessages.CargoRequested, new CargoRequestedMessageArgs
			{
				Manifest = new CargoManifest(manifest)
				{
					Seller = Station.ClientName,
					Buyer = manifest.Consumer,
					Currency = 0,
					ProductAmount = new ProductAmount { ProductId = _populationProductId, Amount = manifest.AmountTotal }
				}
			});
		}

	    public void HandleConsumeSuccess(TradeManifest manifest)
	    {
	        _inboundPopulation += manifest.AmountTotal;
            HandleNewManifest(manifest);
	    }

	    private void HandleNewManifest(TradeManifest manifest)
	    {
	        manifest.OnAmountCompleted += HandleTradeComplete;
	    }

	    private void HandleTradeComplete(TradeManifest tradeManifest)
	    {
	        if (tradeManifest.Status == TradeStatus.Complete && tradeManifest.Consumer == _trader.ClientName)
	        {
	            _inboundPopulation -= tradeManifest.AmountTotal;
                UpdateTradeRequest();
            }
        }

	    public PopulationData GetData()
		{
			var data = new PopulationData
			{
				CurrentCount = _currentCount,
				Capacity = _totalCapacity,
				InboundPopulation = _inboundPopulation,
                Population = AllPopulation.Select(i => i.GetData()).ToList()
			};
			return data;
		}

	    public void HandleManifestComplete(CargoManifest manifest)
	    {
	        if (manifest.ProductAmount.ProductId != _populationProductId)
	            return;

	        var people = _personGenerator.GeneratePeople(manifest.ProductAmount.Amount);
            people.ForEach(i => i.IsResident = true);
            AddPopulation(people);
	    }
	}
}