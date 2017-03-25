using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.WorldMaterials;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Facilitates binding the crafting system to the UI.
/// Handles the game view of recipes, a craftin queue, and progress of current craft.
/// </summary>
public class CraftingViewModel : MonoBehaviour
{
    [SerializeField]
    private Button _buttonPrefab;
    [SerializeField]
    private Image _craftingPanelPrefab;

    private Image _recipesViewContext;
    private Image _queueViewContext;
    
    private List<Recipe> _recipes;
    private CraftingContainer _crafter;

    /// <summary>
    /// Typically called by the owner of the CraftingContainer
    /// </summary>
    public void Bind(List<Recipe> recipes, CraftingContainer crafter)
    {
        _crafter = crafter;
        _recipes = recipes;
        BindToUI();
    }

    private void BindToUI()
    {
        // Instantiate the UI prefab
        var craftingPanel = GameObject.Instantiate(_craftingPanelPrefab);

        // get refs to recipes and queue views
        var scrollview = craftingPanel.transform.Find("content_holder/recipes_scroll_view");
        _recipesViewContext = scrollview.transform.Find("view_port/content").GetComponent<Image>();
        // _queueViewContext = ??

        if (_buttonPrefab == null || _craftingPanelPrefab == null 
            || _recipesViewContext == null ) // || _queueViewContext == null)
            throw new UnityException("Crafting UI ref missing");

        // create buttons for each recipe
        BindRecipes();

        // set the canvas and position. extract position eventually
        PositionOnCanvas(craftingPanel);
    }

    private static void PositionOnCanvas(Image craftingPanel)
    {
        craftingPanel.rectTransform.position = new Vector3(45, 420, 0);
        var canvas = GameObject.Find("InfoCanvas");
        craftingPanel.transform.SetParent(canvas.transform);
    }

    private void BindRecipes()
    {
        foreach (var recipe in _recipes)
        {
            var button = CreateButton(recipe);
            button.onClick.AddListener(() => { _crafter.QueueCrafting(recipe); });
        }
    }

    private Button CreateButton(Recipe recipe)
    {
        var button = GameObject.Instantiate(_buttonPrefab).GetComponent<Button>();
        var text = button.GetComponentInChildren<Text>();
        text.text = GenerateText(recipe);
        button.gameObject.transform.SetParent(_recipesViewContext.transform);
        return button;
    }

    private string GenerateText(Recipe recipe)
    {
        return recipe.ResultProduct;
        //return recipe.Ingredients.Aggregate(string.Format("{0}\t\t", recipe.ResultProduct), (content, ing)
        //    => content + string.Format("{0} {1} ", ing.Quantity, ing.ProductName));
    }
}
