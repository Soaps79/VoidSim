using System;
using System.Collections.Generic;
using System.Linq;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.Trade;

namespace Assets.Logistics
{
	public class CargoManifest
	{
		public int Id;
		public string Buyer;
		public string Seller;
		public ProductAmount ProductAmount;
		public int Currency;
		private TradeInfo _tradeInfo;

		public CargoManifest(TradeInfo info)
		{
			_tradeInfo = info;
		}

		public void Close()
		{
			_tradeInfo.CompleteAmount(ProductAmount.Amount, Currency);
		}
	}

	public class CargoManifestData
	{
		
	}

	public class CargoManifestBook
	{
		public List<CargoManifest> ActiveManifests { get; private set; }
		public Action OnContentsUpdated;

		public List<CargoManifest> GetBuyerManifests(string clientName)
		{
			return ActiveManifests.Where(i => i.Buyer == clientName).ToList();
		}

		public List<CargoManifest> GetSellerManifests(string clientName)
		{
			return ActiveManifests.Where(i => i.Seller == clientName).ToList();
		}

		public CargoManifestBook()
		{
			ActiveManifests = new List<CargoManifest>();
		}

		public void Add(CargoManifest manifest)
		{
			if (manifest != null)
				ActiveManifests.Add(manifest);
			CheckCallback();
		}

		public void Close(int id)
		{
			var manifest = ActiveManifests.FirstOrDefault(i => i.Id == id);
			if (manifest == null)
				return;

			ActiveManifests.Remove(manifest);
			manifest.Close();
			CheckCallback();
		}

		private void CheckCallback()
		{
			if (OnContentsUpdated != null)
				OnContentsUpdated();
		}

	}

}