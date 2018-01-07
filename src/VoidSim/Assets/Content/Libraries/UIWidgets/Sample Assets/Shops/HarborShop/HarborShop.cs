﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UIWidgets;
using UnityEngine.Serialization;

namespace UIWidgetsSamples.Shops
{
	/// <summary>
	/// Harbor shop.
	/// </summary>
	public class HarborShop : MonoBehaviour
	{
		Trader Harbor;
		
		/// <summary>
		/// Trade ListView.
		/// </summary>
		[SerializeField]
		protected HarborListView TradeView;

		/// <summary>
		/// Text component to display total value.
		/// </summary>
		[SerializeField]
		protected Text TradeTotal;

		/// <summary>
		/// Buy button.
		/// </summary>
		[SerializeField]
		protected Button BuyButton;
		
		Trader Player;

		/// <summary>
		/// Text component to display player money.
		/// </summary>
		[SerializeField]
		protected Text PlayerMoney;

		/// <summary>
		/// Notification template.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("notify")]
		protected Notify NotifyTemplate;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Harbor = new Trader(false);
			Player = new Trader(false);
			
			Init();
			
			BuyButton.onClick.AddListener(Trade);

			Harbor.OnItemsChange += UpdateTraderItems;
			Player.OnItemsChange += UpdateTraderItems;

			Player.OnMoneyChange += UpdatePlayerMoneyInfo;
			
			UpdateTraderItems();
			
			UpdatePlayerMoneyInfo();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public void Init()
		{
			Harbor.Money = -1;
			Harbor.PriceFactor = 1;
			Harbor.Inventory.Clear();
			var shop_items = new List<Item>(){
				new Item("Wood", 100),
				new Item("Wheat", 30),
				new Item("Fruits", 0),
				new Item("Sugar", 20),
				new Item("Metal", 40),
				new Item("Cotton", 0),
				new Item("Silver", 25),
				new Item("Gold", 55),
				new Item("Cocoa", 10),
				new Item("Coffee", 7),
				new Item("Tobacco", 20),
			};
			Harbor.Inventory.AddRange(shop_items);

			Player.Money = 5000;
			Player.PriceFactor = 0.5f;
			Player.Inventory.Clear();
			var player_items = new List<Item>(){
				new Item("Wood", 50),
				new Item("Cocoa", 100),
				new Item("Metal", 20),
				new Item("Sugar", 10),
			};
			Player.Inventory.AddRange(player_items);
		}

		/// <summary>
		/// Updates the total.
		/// </summary>
		public void UpdateTotal()
		{
			var order = new HarborOrder(TradeView.DataSource);
			TradeTotal.text = order.Total().ToString();
		}

		static ObservableList<HarborOrderLine> CreateOrderLines(Trader harbor, Trader player)
		{
			return harbor.Inventory.Convert(item => {
				var playerItem = player.Inventory.Find(x => x.Name==item.Name);
				return new HarborOrderLine(
					item,
					Prices.GetPrice(item, harbor.PriceFactor),
					Prices.GetPrice(item, player.PriceFactor),
					item.Count,
					(playerItem==null) ? 0 : playerItem.Count
				);
			});
		}
		
		void UpdateTraderItems()
		{
			TradeView.DataSource = CreateOrderLines(Harbor, Player);
		}

		void UpdatePlayerMoneyInfo()
		{
			PlayerMoney.text = Player.Money.ToString();
		}
		
		void Trade()
		{
			var order = new HarborOrder(TradeView.DataSource);
			
			if (Player.CanBuy(order))
			{
				Harbor.Sell(order);
				Player.Buy(order);

				//UpdateTotal();
			}
			else
			{
				var message = string.Format("Not enough money to buy items. Available: {0}; Required: {1}", Player.Money, order.Total());
				NotifyTemplate.Clone().Show(message, customHideDelay: 3f, sequenceType: NotifySequence.First, clearSequence: true);
			}
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (BuyButton!=null)
			{
				BuyButton.onClick.RemoveListener(Trade);
			}

			if (Harbor!=null)
			{
				Harbor.OnItemsChange -= UpdateTraderItems;
			}
			
			if (Player!=null)
			{
				Player.OnItemsChange -= UpdateTraderItems;
				Player.OnMoneyChange -= UpdatePlayerMoneyInfo;
			}
		}
		
	}
}