using System.Collections.Generic;
using System.Linq;
using Assets.Logistics;
using Assets.Placeables.Nodes;
using Assets.Scripts;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Population;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.Trade;
using Messaging;
using QGame;
using UnityEngine;
using Random = UnityEngine.Random;
using TimeLength = Assets.Scripts.TimeLength;

namespace Assets.Station
{
	/// <summary>
	/// Bird's eye view of population; including housing, employment, and pops themselves
	/// </summary>
	public class PopulationControl : QScript, IPopulationHost, ITraderDriver, IMessageListener
	{
		public string Name { get { return "PopulationControl"; } }
		[SerializeField] private int _totalCapacity;
		[SerializeField] private int _initialCapacity;
		[SerializeField] private int _currentCount;
		[SerializeField] private int _currentUnemployed;
		private readonly List<PopHousing> _housing = new List<PopHousing>();
		private readonly List<PopEmployer> _employers = new List<PopEmployer>();

		// Population is currently stored in the Station's inventory as a Product
		// Allows it to be handled by trade, cargo, etc
		private const string POPULATION_PRODUCT_NAME = "Population";
		private Inventory _inventory;
		private int _populationProductId;

		// When there is room for more population, 
		// it is requested through this Trader which is hooked into the trade system
		private ProductTrader _trader;
		private int _inboundPopulation;

		public class EmploymentUpdateParams
		{
			public TimeLength EmploymentUpdateTimeLength;
			public int EmploymentUpdateCount;
			public float BaseEmployChance;
		}

		// basic employment model
		[SerializeField] private TimeLength _employmentUpdateTimeLength;
		[SerializeField] private int _employmentUpdateCount;
		private string _stopwatchNodeName = "employment";
		private float _baseEmployChance;
		
		public void Initialize(Inventory inventory, EmploymentUpdateParams updateParams, int initialCapacity = 0)
		{
			var pop = ProductLookup.Instance.GetProduct(POPULATION_PRODUCT_NAME);
			_populationProductId = pop.ID;

			_initialCapacity = initialCapacity;
			_inventory = inventory;
			_inventory.OnProductsChanged += HandleInventoryProductChanged;
			_currentCount = _inventory.GetProductCurrentAmount(_populationProductId);
			_currentUnemployed = _currentCount;
			
			CurrentQualityOfLife = 10;

			if (_initialCapacity > 0)
				_inventory.SetProductMaxAmount(_populationProductId, _initialCapacity);

			Locator.MessageHub.AddListener(this, PopHousing.MessageName);
			Locator.MessageHub.AddListener(this, PopEmployer.MessageName);

			// remove when housing serialization is in place
			Locator.LastId.Reset("pop_housing");

			InitializeProductTrader();
			InitializeEmploymentUpdate(updateParams);
		}

		// register with stopwatch to regularly check for updates
		private void InitializeEmploymentUpdate(EmploymentUpdateParams updateParams)
		{
			_employmentUpdateTimeLength = updateParams.EmploymentUpdateTimeLength;
			_employmentUpdateCount = updateParams.EmploymentUpdateCount;
			_baseEmployChance = updateParams.BaseEmployChance;

			var time = Locator.WorldClock.GetSeconds(_employmentUpdateTimeLength);
			var node = StopWatch.AddNode(_stopwatchNodeName, time);
			node.OnTick += HandleEmploymentUpdate;
		}

		private void HandleEmploymentUpdate()
		{
			if (_currentUnemployed <= 0 || !_employers.Any(i => i.HasRoom))
				return;

			// making a queue as basic distribution
			var seeking = _employers.Where(i => i.HasRoom).OrderBy(i => i.EmployeeDesirability).ToList();
			var employers = new Queue<PopEmployer>();
			seeking.ForEach(i => employers.Enqueue(i));

			// for each possible employee
			for (int i = 0; i < _employmentUpdateCount; i++)
			{
				if (_currentUnemployed < 0 || !employers.Any())
					break;

				// see if they want the job
				var roll = Random.value;
				if (roll > _baseEmployChance)
					continue;

				// pop the employer, give him the worker, add to back if he still has room
				var employer = employers.Dequeue();
				employer.AddEmployee(1);
				_currentUnemployed -= 1;
				if(employer.HasRoom)
					employers.Enqueue(employer);
			}
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
			_currentUnemployed += amount;
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

		public int TotalCapacity
		{
			get { return _totalCapacity; }
		}

		// subscribed to messages for housing and employment being placed
		public void HandleMessage(string type, MessageArgs args)
		{
			switch (type)
			{
				case PopHousing.MessageName:
					HandleHousingAdd(args as PopHousingMessageArgs);
					break;
				case PopEmployer.MessageName:
					HandleEmployerAdd(args as PopEmployerMessageArgs);
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

			args.PopHousing.name = "pop_housing_" + Locator.LastId.GetNext("pop_housing");

			_housing.Add(args.PopHousing);
			UpdateCapacity();
			UpdateTradeRequest();
		}

		private void UpdateCapacity()
		{
			_totalCapacity = _initialCapacity + _housing.Sum(i => i.Capacity);
			_inventory.SetProductMaxAmount(_populationProductId, _totalCapacity);
		}

		private void HandleEmployerAdd(PopEmployerMessageArgs args)
		{
			if (args == null || args.PopEmployer == null)
			{
				Debug.Log("PopulationControl given bad employer message args.");
				return;
			}

			var employer = args.PopEmployer;
			_employers.Add(employer);

			// real basic implementation of placing employees, place half the max amount
			if (_currentUnemployed > 0)
			{
				var countToEmploy = employer.MaxEmployeeCount / 2;
				if (_currentUnemployed > countToEmploy)
				{
					employer.AddEmployee(countToEmploy);
					_currentUnemployed -= countToEmploy;
				}
				else
				{
					employer.CurrentEmployeeCount = _currentUnemployed;
					_currentUnemployed = 0;
				}
			}
		}


		public float CurrentQualityOfLife { get; private set; }
		public bool PopulationWillMigrateTo(IPopulationHost otherHost)
		{
			return false;
		}

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
	}
}