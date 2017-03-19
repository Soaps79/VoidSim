using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.WorldMaterials;
using Newtonsoft.Json;
using QGame;
using UnityEditor;
using UnityEngine;

public class ProductEditorViewModel : QScript
{
	// to populate dropdowns in editor
	public static string[] ProductNames;
	public static string[] ContainerNames;

	public Product[] Products;
	public CraftingContainerInfo[] ContainersInfo;
	public Recipe[] Recipes;

	public ProductEditorViewModel()
	{
		BindProductNames();
		BindContainerNames();
	}

	private void BindProductNames()
	{
		ProductNames = Products != null && Products.Any() 
			? Products.Select(i => i.Name).ToArray()
			: new string[1] {"Empty"};
	}

	private void BindContainerNames()
	{
		ContainerNames = ContainersInfo != null && ContainersInfo.Any()
			? ContainersInfo.Select(i => i.Name).ToArray()
			: new string[1] { "Empty" };
	}

	public void SerializeAll()
	{
		if (Products == null || !Products.Any())
			return;

		// Move this elsewhere if it becomes a thing
		for (int i = 0; i < Products.Length; i++)
		{
			Products[i].ID = i + 1;
		}

		var collection = SerializeProducts();
		SaveCollectionToFile("products", collection);
		BindProductNames();

		collection = SerializeContainers();
		SaveCollectionToFile("containers", collection);
		BindContainerNames();

		collection = SerializeRecipes();
		SaveCollectionToFile("recipes", collection);
	}

	public string SerializeProducts()
	{
		
		return JsonConvert.SerializeObject(Products, Formatting.Indented);
	}

	public string SerializeContainers()
	{
		return JsonConvert.SerializeObject(ContainersInfo, Formatting.Indented);
	}

	public string SerializeRecipes()
	{
		return JsonConvert.SerializeObject(Recipes, Formatting.Indented);
	}

	private void SaveCollectionToFile(string name, string json)
	{
		using (FileStream fs = File.Open(string.Format("Assets/Resources/{0}.json", name), FileMode.Create, FileAccess.Write))
		using (StreamWriter sw = new StreamWriter(fs))
		{
			sw.WriteLine(json);
		}
	}

	
}
