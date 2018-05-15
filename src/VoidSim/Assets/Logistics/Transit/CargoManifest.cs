using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Serialization;
using Assets.WorldMaterials.Products;
using Assets.WorldMaterials.Trade;

namespace Assets.Logistics.Transit
{
	public class CargoManifestData
	{
		public int Id;
		public string Buyer;
		public string Seller;
		public int ProductId;
		public int ProductAmount;
		public int Currency;
		public int TradeManifestId;
	}

	public class CargoManifest : ISerializeData<CargoManifestData>
	{
		public int Id;
		public string Receiver;
		public string Shipper;
		public ProductAmount ProductAmount;
		public int Currency;
		public int TradeManifestId;
		public TradeManifest TradeManifest;

		public CargoManifest(TradeManifest manifest)
		{
			TradeManifest = manifest;
		}

		public CargoManifest(CargoManifestData data)
		{
			Id = data.Id;
			Receiver = data.Buyer;
			Shipper = data.Seller;
			ProductAmount = new ProductAmount(data.ProductId, data.ProductAmount);
			Currency = data.Currency;
			TradeManifestId = data.TradeManifestId;
		}

		public bool Close()
		{
			if (TradeManifest == null)
				return false;

			TradeManifest.CompleteAmount(ProductAmount.Amount, Currency);
			return true;
		}
		public CargoManifestData GetData()
		{
			return new CargoManifestData
			{
				Id = Id,
				Buyer = Receiver,
				Seller = Shipper,
				ProductId = ProductAmount.ProductId,
				ProductAmount = ProductAmount.Amount,
				Currency = Currency,
				TradeManifestId = TradeManifest.Id
			};
		}
	}

	public class CargoManifestBookData
	{
		public List<CargoManifestData> Manifests;
	}

	public class CargoManifestBook : ISerializeData<CargoManifestBookData>
	{
		public List<CargoManifest> ActiveManifests { get; private set; }
		public Action OnContentsUpdated;

		public List<CargoManifest> GetBuyerManifests(string clientName)
		{
			return ActiveManifests.Where(i => i.Receiver == clientName).ToList();
		}

		public List<CargoManifest> GetSellerManifests(string clientName)
		{
			return ActiveManifests.Where(i => i.Shipper == clientName).ToList();
		}

		public CargoManifestBook()
		{
			ActiveManifests = new List<CargoManifest>();
		}

		public CargoManifestBook(CargoManifestBookData data)
		{
			if(data == null || data.Manifests == null || !data.Manifests.Any())
				ActiveManifests = new List<CargoManifest>();
			else
				ActiveManifests = data.Manifests.Select(i => new CargoManifest(i)).ToList();
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


		public CargoManifestBookData GetData()
		{
			return new CargoManifestBookData
			{
				Manifests = ActiveManifests.Select(i => i.GetData()).ToList()
			};
		}
	}

}