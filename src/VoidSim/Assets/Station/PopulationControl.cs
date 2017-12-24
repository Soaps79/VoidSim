using System.Collections.Generic;
using System.Linq;
using Assets.Logistics;
using Assets.Placeables.Nodes;
using Assets.Scripts;
using Assets.Scripts.Serialization;
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
		public EmployerControlData EmployerControlData;
	    public List<PersonData> Population;
	}

	/// <summary>
	/// Bird's eye view of population; including housing, employment, and pops themselves
	/// </summary>
	public class PopulationControl : QScript, IPopulationHost, ISerializeData<PopulationData>, ITraderDriver, IMessageListener
	{
		private const string _placeableNameSuffix = "pop_housing_";
		public string Name { get { return "PopulationControl"; } }
		[SerializeField] private int _totalCapacity;
		[SerializeField] private int _baseCapacity;
		[SerializeField] private int _currentCount;
	    [SerializeField] private int _countNamesToLoad;
		private readonly List<PopHousing> _housing = new List<PopHousing>();

	    // Population is currently stored in the Station's inventory as a Product
		// Allows it to be handled by trade, cargo, etc
		private Inventory _inventory;
		private int _populationProductId;
		
		// When there is room for more population, 
		// it is requested through this Trader which is hooked into the trade system
		private ProductTrader _trader;
		private int _inboundPopulation;

		private PopulationSO _scriptable;
		private bool _ignoreNeeds;

		public MoodManager MoodManager { get; private set; }
		private EmployerControl _employerControl;

		// serialization
		private readonly CollectionSerializer<PopulationData> _serializer
			= new CollectionSerializer<PopulationData>();
		private PopulationData _deserialized;
		private const string _collectionName = "PopulationControl";

        private readonly PersonGenerator _personGenerator = new PersonGenerator();
        private readonly List<Person> _allPopulation = new List<Person>();
        private readonly List<Person> _needsResidentHousing = new List<Person>();


	    public void Initialize(Inventory inventory, PopulationSO scriptable)
		{
		    _scriptable = scriptable;
			_inventory = inventory;
			_inventory.OnProductsChanged += HandleInventoryProductChanged;

			_populationProductId = ProductIdLookup.Population;
			_currentCount = _inventory.GetProductCurrentAmount(_populationProductId);
			_baseCapacity = scriptable.BaseCapacity;

			// still temporary values until system is worked out
			CurrentQualityOfLife = 10;

			// establish sub-objects
			MoodManager = new MoodManager(scriptable.MoodParams, inventory);
			_employerControl = gameObject.AddComponent<EmployerControl>();
			_employerControl.Initialize(scriptable, MoodManager.EfficiencyModule, _currentCount);
		    _personGenerator.Initialize(scriptable.GenerationParams);

            // load or set defaults
            if (_serializer.HasDataFor(this, _collectionName))
				LoadFromFile();
			else
				LoadFromScriptable();
			_inventory.SetProductMaxAmount(_populationProductId, scriptable.BaseCapacity);

			Locator.MessageHub.AddListener(this, PopHousing.MessageName);

			InitializeProductTrader();
		}

		private void LoadFromScriptable()
		{
			_inventory.TryAddProduct(_populationProductId, _scriptable.InitialCount);
		    var people = _personGenerator.GeneratePeople(_scriptable.InitialCount);
            people.ForEach(i => i.IsResident = true);
            _needsResidentHousing.AddRange(people);
		    _allPopulation.AddRange(people);
        }

		private void LoadFromFile()
		{
			_deserialized = _serializer.DeserializeData();
			_currentCount = _deserialized.CurrentCount;
			_inboundPopulation = _deserialized.InboundPopulation;
			_employerControl.Deserialize(_deserialized.EmployerControlData);
            _allPopulation.AddRange(_deserialized.Population.Select(i => new Person(i)));
			if(_inventory.GetProductCurrentAmount(ProductIdLookup.Population) != _currentCount)
				throw new UnityException("PopControl data not matching station inventory");
		}

		private void InitializeProductTrader()
		{
			_trader = gameObject.AddComponent<ProductTrader>();
			_trader.Initialize(this, Station.ClientName);
			UpdateTradeRequest();
		}

		// Hooked into _inventory's update event
		private void HandleInventoryProductChanged(int productId, int amount)
		{
			if (productId != _populationProductId)
				return;

			// currently the only way pop can rise is through being delivered, acknowledge the fulfilled trade
			if (amount > 0)
				_inboundPopulation -= amount;

			_currentCount = _inventory.GetProductCurrentAmount(_populationProductId);
			_employerControl.CurrentUnemployed += amount;
		}

		// checks to see if inventory has room for more pop (discounting those already in transit)
		private void UpdateTradeRequest()
		{
			var remaining = _inventory.GetProductRemainingSpace(_populationProductId);
			remaining -= _inboundPopulation;
			_trader.SetConsume(new ProductAmount
			{
				ProductId = _populationProductId,
				Amount = remaining > 0  ? remaining : 0
			});
		}

		public void HandleMessage(string type, MessageArgs args)
		{
			switch (type)
			{
				case PopHousing.MessageName:
					HandleHousingAdd(args as PopHousingMessageArgs);
					break;
			}
		}

		private void HandleHousingAdd(PopHousingMessageArgs args)
		{
			if (args == null || args.PopHousing == null)
			{
				Debug.Log("PopulationControl given bad housing message args.");
				return;
			}

			// name it since popcontrol owns all pop housings
			args.PopHousing.name = _placeableNameSuffix + Locator.LastId.GetNext("pop_housing");
			_housing.Add(args.PopHousing);
			args.PopHousing.OnRemove += HandleRemove;

		    TryMovePeopleIntoHomes(args.PopHousing);
			// update capacity, put out a request for inhabitants
			UpdateCapacity();
			UpdateTradeRequest();
		}

	    private void TryMovePeopleIntoHomes(PopHousing popHousing)
	    {
            // this is primitive, will work until people are choosy about their housing
	        if (!_needsResidentHousing.Any())
	            return;

	        var movingIn = new List<Person>();
	        if (_needsResidentHousing.Count > popHousing.Capacity)
	            movingIn.AddRange(_needsResidentHousing.Take(popHousing.Capacity));
            else
                movingIn.AddRange(_needsResidentHousing);

	        var holder = popHousing.GetComponent<PopHolder>();
            holder.TakePeople(movingIn);
            movingIn.ForEach(i => _needsResidentHousing.Remove(i));
	    }

	    private void HandleRemove(PopHousing obj)
		{
			if (_housing.Remove(obj))
			{
				UpdateCapacity();
			}
		}

		private void UpdateCapacity()
		{
			_totalCapacity = _baseCapacity + _housing.Sum(i => i.Capacity);
			_inventory.SetProductMaxAmount(_populationProductId, _totalCapacity);
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

			_inboundPopulation += provided.Amount;
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

		public void HandleConsumeSuccess(TradeManifest manifest) { }

		public PopulationData GetData()
		{
			var data = new PopulationData
			{
				CurrentCount = _currentCount,
				Capacity = _totalCapacity,
				InboundPopulation = _inboundPopulation,
				EmployerControlData = _employerControl.GetData(),
                Population = _allPopulation.Select(i => i.GetData()).ToList()
			};
			return data;
		}
	}
}