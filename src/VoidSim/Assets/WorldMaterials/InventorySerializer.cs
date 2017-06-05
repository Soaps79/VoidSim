using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Serialization;
using Assets.WorldMaterials.Products;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.WorldMaterials
{
	[Serializable]
	public class InventoryData
	{
		public List<InventoryProductEntryData> Products;
		public List<InventoryPlaceableData> Placeables;
		public int DefaultProductCapacity;
	}

	[Serializable]
	public class InventoryProductEntryData
	{
		public string ProductName;
		public int Amount;
		public int MaxAmount;
	}

	[Serializable]
	public class InventoryPlaceableData
	{
		public int Id;
		public string PlaceableName;
	}

	// first pass at serialization, trying to find patterns
	public class InventorySerializer : Serializer<Inventory, InventoryData>
	{
		public InventoryData ConvertToSerializable(Inventory inventory)
		{
			// convert products and placeables to their data types
			var products = inventory.GetProductEntries().Select(i => new InventoryProductEntryData
			{
				ProductName = i.Product.Name,
				Amount = i.Amount,
				MaxAmount = i.MaxAmount
			}).ToList();
			
			var placeables = inventory.Placeables.Select(i => new InventoryPlaceableData
			{
				Id = i.Id,
				PlaceableName = i.Name
			}).ToList();

			var data = new InventoryData
			{
				Products = products,
				Placeables = placeables
			};

			return data;
		}

		public InventoryData ConvertFromSerialized(string serialized)
		{
			if(string.IsNullOrEmpty(serialized))
				throw new UnityException("Deserializer given bad data string");

			var data = JsonConvert.DeserializeObject<InventoryData>(serialized);
			return data;
		}
	}
}