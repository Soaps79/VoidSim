using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Serialization;
using Assets.WorldMaterials.Products;

namespace Assets.Logistics.Transit
{
	public class CargoManifestData
	{
		public string Buyer;
		public string Seller;
		public int ProductId;
		public int ProductAmount;
		public int Currency;
	}

	public class CargoManifest : ISerializeData<CargoManifestData>
	{
		public string Receiver;
		public string Shipper;
		public ProductAmount ProductAmount;
		public int Currency;

		public CargoManifest() { }

		public CargoManifest(CargoManifestData data)
		{
			Receiver = data.Buyer;
			Shipper = data.Seller;
			ProductAmount = new ProductAmount(data.ProductId, data.ProductAmount);
			Currency = data.Currency;
		}

		public CargoManifestData GetData()
		{
			return new CargoManifestData
			{
				Buyer = Receiver,
				Seller = Shipper,
				ProductId = ProductAmount.ProductId,
				ProductAmount = ProductAmount.Amount,
				Currency = Currency
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
		public Action<CargoManifest> OnManifestAdded;

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

		public void Complete(CargoManifest manifest)
		{
			if (!ActiveManifests.Contains(manifest))
				return;
			
			ActiveManifests.Remove(manifest);
			CheckCallback();
		}

		private void CheckCallback()
		{
			OnContentsUpdated?.Invoke();
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