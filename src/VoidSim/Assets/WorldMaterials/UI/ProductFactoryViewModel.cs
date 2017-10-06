using System.Collections.Generic;
using System.Linq;
using Assets.Controllers.GUI;
using Assets.Placeables.Nodes;
using Assets.Scripts.UI;
using Assets.Station.Efficiency;
using Assets.WorldMaterials.Products;
using QGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Assets.WorldMaterials.UI
{
	/// <summary>
	/// Binds a factory placeable node to a UI representation.
	/// </summary>
	public class ProductFactoryViewModel : QScript
	{
		private ProductFactory _factory;
		[SerializeField] private TMP_Dropdown _dropdown;
		[SerializeField] private Toggle _onOffToggle;
		[SerializeField] private Toggle _isBuyingToggle;
		[SerializeField] private SliderBinding _slider;
		[SerializeField] private TMP_Text _efficiencyAmountText;

		private readonly List<string> _recipeNames = new List<string>();
		private readonly List<Recipe> _recipes = new List<Recipe>();

		public void Bind(ProductFactory productFactory)
		{
			_factory = productFactory;
			// responsible for populating the dropdown and the on/off toggle
			BindRecipes();
			BindToggles();
			BindSlider();
			_dropdown.onValueChanged.AddListener(HandleCurrentCraftingChanged);
			productFactory.EfficiencyModule.OnValueChanged += UpdateEfficiency;
			UpdateEfficiency(productFactory.EfficiencyModule);

			InitializeEfficiencyTooltip();
		}

		private void InitializeEfficiencyTooltip()
		{
			var trigger = _efficiencyAmountText.GetComponent<BoundTooltipTrigger>();
			if (trigger != null)
			{
				trigger.OnHoverActivate += tooltipTrigger => tooltipTrigger.text =
					TooltipStringGenerator.Generate(_factory.EfficiencyModule);
			}
		}

		private void UpdateEfficiency(EfficiencyModule module)
		{
			var percent = module.CurrentAmount * 100;
			_efficiencyAmountText.text = percent.ToString("0");
		}

		private void BindSlider()
		{
			_slider.Initialize(() => _factory.CurrentCraftRemainingAsZeroToOne);
		}

		private void BindToggles()
		{
			_onOffToggle.isOn = _factory.IsCrafting;
			_onOffToggle.onValueChanged.AddListener(HandleOnOffToggle);

			_isBuyingToggle.isOn = _factory.IsBuying;
			_isBuyingToggle.onValueChanged.AddListener(HandleBuyingToggle);
		}

		private void HandleBuyingToggle(bool isOn)
		{
			_factory.SetIsBuying(isOn);
		}

		private void HandleOnOffToggle(bool isOn)
		{
			if(isOn)
				_factory.SwitchCurrentCraftingTo(_recipes[_dropdown.value]);
			else
				_factory.StopCrafting();
		}

		private void BindRecipes()
		{
			_recipes.Clear();
			_recipes.AddRange(_factory.Recipes);

			var names = _recipes.Select(i => i.DisplayName).ToList();
			_recipeNames.Clear();
			_recipeNames.AddRange(names);

			BindDropdown(names);
		}

		private void BindDropdown(List<string> names)
		{
			_dropdown.ClearOptions();
			_dropdown.AddOptions(names);

			if (_factory.IsCrafting)
			{
				// note: If it is necessary to have multiple recipes with the same name in a list,
				// this will need to be refactored
				var currentIndex = names.IndexOf(_factory.CurrentlyCrafting.DisplayName);
				_dropdown.value = currentIndex;
			}
		}

		private void HandleCurrentCraftingChanged(int index)
		{
			if (_recipeNames.Count < index + 1) return;
			var recipe = _recipes[index];
			if(_onOffToggle.isOn)
				_factory.SwitchCurrentCraftingTo(recipe);
		}
	}
}