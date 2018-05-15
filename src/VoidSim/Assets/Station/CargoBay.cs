using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Logistics;
using Assets.Logistics.Transit;
using Assets.Scripts;
using Assets.Station.UI;
using Assets.WorldMaterials;
using Assets.WorldMaterials.Products;
using QGame;
using UnityEngine;

namespace Assets.Station
{
	public class CargoBay : QScript
	{
		public string Name { get; private set; }
		public int AmountPerTick;
		public int SecondsPerTick;

		private ShipBerth _berth;
		private TrafficShip _ship;
		private CargoManifestBook _manifestBook;
		private ProductInventory _inventory;
		private InventoryReserve _reserve;
		private int _creditsProductID;

		private bool _isUnloadingIn;
		private bool _isUnloadingOut;

		private const string _stopwatchName = "tick_unload";


		private readonly Queue<CargoManifest> _manifestsIn = new Queue<CargoManifest>();
		private readonly Queue<CargoManifest> _manifestsOut = new Queue<CargoManifest>();

		private ProductAmount _productIn;
		private ProductAmount _productOut;
		private TransactionText _textPrefab;

	    public Action<CargoManifest> OnCargoManifestComplete;

		public void Initialize(ShipBerth berth, ProductInventory inventory, InventoryReserve reserve, TransactionText textPrefab)
		{
			_berth = berth;
			_inventory = inventory;
			_reserve = reserve;
			_textPrefab = textPrefab;

			transform.position = berth.transform.position;

			AmountPerTick = 5;
			SecondsPerTick = 1;
			_creditsProductID = ProductValueLookup.CreditsProductId;

			InitializeBerth();
			InitializeStopwatch();
		}

		// take on berth's name, hook into docking actions
		private void InitializeBerth()
		{
			Name = _berth.Name;
			name = _berth.Name;
			_berth.OnShipDock += HandleShipDock;
		}

		// stopwatch is used to regulate ticking of goods movements
		private void InitializeStopwatch()
		{
			var node = StopWatch.AddNode(_stopwatchName, SecondsPerTick);
			node.OnTick += TickUnloads;
			node.Pause();
		}

		// grab references to the ship and begin transferring goods
		private void HandleShipDock(TrafficShip ship)
		{
			_ship = ship;
			_manifestBook = ship.ManifestBook;

			if(_ship == null || _manifestBook == null)
				throw new UnityException(string.Format("CargoBay {0} received bad ship.", Name));

			BeginTransfer();
		}

		// current functionality:
		// ticks each manifest into inventory until it is empty,
		// transfers currency at the end of each manifest,
		// removes manifest from book, and checks for a new one
		private void TickUnloads()
		{
		    if (_isUnloadingIn)
			{
				if (AmountPerTick > _productIn.Amount)
				{
					_reserve.AdjustHold(_productIn.ProductId, -_productIn.Amount);
					_inventory.TryAddProduct(_productIn.ProductId, _productIn.Amount);
					_productIn.Amount = 0;
				}
				else
				{
					_reserve.AdjustHold(_productIn.ProductId, -AmountPerTick);
					_inventory.TryAddProduct(_productIn.ProductId, AmountPerTick);
					_productIn.Amount -= AmountPerTick;
				}

				if (_productIn.Amount <= 0)
				{
					var manifest = _manifestsIn.Dequeue();
					_inventory.TryRemoveProduct(_creditsProductID, manifest.Currency);
					_manifestBook.Close(manifest.Id);
					CreateCompletionText(manifest.Currency, true);
					CheckNextIncoming();
                    if(OnCargoManifestComplete != null)
                        OnCargoManifestComplete(manifest);
				}
			}

			if (_isUnloadingOut)
			{
				if (AmountPerTick > _productOut.Amount)
				{
					_reserve.AdjustHold(_productOut.ProductId, _productOut.Amount);
					_inventory.TryRemoveProduct(_productOut.ProductId, _productOut.Amount);
					_productOut.Amount = 0;
				}
				else
				{
					_reserve.AdjustHold(_productOut.ProductId, AmountPerTick);
					_inventory.TryRemoveProduct(_productOut.ProductId, AmountPerTick);
					_productOut.Amount -= AmountPerTick;
				}

				if (_productOut.Amount <= 0)
				{
					var manifest = _manifestsOut.Dequeue();
					_inventory.TryAddProduct(_creditsProductID, manifest.Currency);
					_manifestBook.Close(manifest.Id);
					CreateCompletionText(manifest.Currency, false);
					CheckNextOutgoing();
				    if (OnCargoManifestComplete != null)
				        OnCargoManifestComplete(manifest);
                }
			}

			if (!_isUnloadingIn && !_isUnloadingOut)
				CompleteUnload();
		}

		private void CreateCompletionText(int amount, bool wasBought)
		{
            var text = Instantiate(_textPrefab);
			text.Initialize(
                Locator.CanvasManager.GetCanvas(CanvasType.GameText).transform, 
                amount, wasBought, transform.position);
		}

		// manifests are complete, tell the berth
		private void CompleteUnload()
		{
			StopWatch[_stopwatchName].Pause();
			_berth.CompleteServicing();
			_ship = null;
			_manifestBook = null;
		}
		
		private void BeginTransfer()
		{
			var incoming = _manifestBook.GetBuyerManifests(Station.ClientName);
			var outgoing = _manifestBook.GetSellerManifests(Station.ClientName);

			incoming.ForEach(i => _manifestsIn.Enqueue(i));
			outgoing.ForEach(i => _manifestsOut.Enqueue(i));

			CheckNextIncoming();
			CheckNextOutgoing();

			if (_isUnloadingIn || _isUnloadingOut)
				_berth.State = BerthState.Transfer;

			StopWatch[_stopwatchName].Reset();
		}

		private void CheckNextIncoming()
		{
			if (_manifestsIn.Any())
			{
				_productIn = new ProductAmount(
					_manifestsIn.Peek().ProductAmount.ProductId,
					_manifestsIn.Peek().ProductAmount.Amount);
				_isUnloadingIn = true;
			}
			else
			{
				_productIn = null;
				_isUnloadingIn = false;
			}
		}

		private void CheckNextOutgoing()
		{
			if (_manifestsOut.Any())
			{
				_productOut = new ProductAmount(
					_manifestsOut.Peek().ProductAmount.ProductId,
					_manifestsOut.Peek().ProductAmount.Amount);
				_isUnloadingOut = true;
			}
			else
			{
				_productOut = null;
				_isUnloadingOut = false;
			}
		}
	}
}