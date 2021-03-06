﻿using System;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.WorldMaterials.Products;
using QGame;
using TimeUnit = Assets.Scripts.TimeUnit;
using WorldClock = Assets.Scripts.WorldClock;

namespace Assets.WorldMaterials.Trade
{
    /// <summary>
    /// Bound from a TraderRequests SO, this behavior will direct 
    /// a ProductTrader to repeatedly place requests to the trading hub.
    /// 
    /// Used to provide variance to the void's trades
    /// </summary>
    public class ProductTradeAutomater : QScript
    {
        private class ProductTradeRequest
        {
            public ProductAmount ProductAmount;
            public bool IsProviding;
	        public bool IsAdditive;
        }

        private readonly Dictionary<TimeUnit, List<ProductTradeRequest>> _requests 
            = new Dictionary<TimeUnit, List<ProductTradeRequest>>();

        private ProductTrader _trader;

        public void Initialize(ProductTrader trader, IWorldClock worldClock, TraderRequestsSO requests = null)
        {
            _trader = trader;
            InitializeRequestsTable();
            BindWorldClock(worldClock);
            BindScriptable(requests);
        }

        private void BindScriptable(TraderRequestsSO scriptable)
        {
            foreach (var request in scriptable.Requests)
            {
                _requests[request.Frequency].Add(new ProductTradeRequest
                {
                    ProductAmount = new ProductAmount
                    {
                        ProductId = ProductLookup.Instance.GetProduct(request.Product).ID,
                        Amount = request.Amount
                    },
                    IsProviding = request.isSelling
                });
            }
        }

        private void InitializeRequestsTable()
        {
            foreach (var value in Enum.GetValues(typeof (TimeUnit)))
            {
                _requests.Add((TimeUnit) value, new List<ProductTradeRequest>());
            }
        }

        private void BindWorldClock(IWorldClock worldClock)
        {
            worldClock.OnYearUp += HandleYearUp;
            worldClock.OnMonthUp += HandleMonthUp;
            worldClock.OnWeekUp += HandleWeekUp;
            worldClock.OnDayUp += HandleDayUp;
            worldClock.OnHourUp += HandleHourUp;
        }

        private void HandleRequestUpdates(List<ProductTradeRequest> requests)
        {
            foreach (var request in requests)
            {
	            if (request.IsProviding)
	            {
		            if (request.IsAdditive)
			            _trader.AddProvide(request.ProductAmount);
		            else
			            _trader.SetProvide(request.ProductAmount);
	            }
	            else
	            {
					if (request.IsAdditive)
						_trader.AddConsume(request.ProductAmount);
					else
						_trader.SetConsume(request.ProductAmount);
				}
            }

	        requests.RemoveAll(i => i.IsAdditive);
        }

	    public void AddRequest(ProductAmount productAmount, TimeUnit time, bool isProviding, bool isAdditive)
	    {
		    _requests[time].Add(new ProductTradeRequest
		    {
			    IsAdditive =  isAdditive,
				IsProviding = isProviding,
				ProductAmount = productAmount
		    });
	    }

        private void HandleHourUp(object sender, EventArgs e)
        {
            HandleRequestUpdates(_requests[TimeUnit.Hour]);
        }

        private void HandleDayUp(object sender, EventArgs e)
        {
            HandleRequestUpdates(_requests[TimeUnit.Day]);
        }

        private void HandleWeekUp(object sender, EventArgs e)
        {
            HandleRequestUpdates(_requests[TimeUnit.Week]);
        }

        private void HandleMonthUp(object sender, EventArgs e)
        {
            HandleRequestUpdates(_requests[TimeUnit.Month]);
        }

        private void HandleYearUp(object sender, EventArgs e)
        {
            HandleRequestUpdates(_requests[TimeUnit.Year]);
        }
    }
}