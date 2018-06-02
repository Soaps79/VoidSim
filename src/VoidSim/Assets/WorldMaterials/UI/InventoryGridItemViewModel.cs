using UIWidgets;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.WorldMaterials.UI
{
    public class InventoryGridItemViewModel : ListViewItem, IViewData<InventoryGridTile>
    {
        public Image ColorBlock;
        public Sprite EmptyTile;
        public Sprite FullTile;

        public void SetData(InventoryGridTile item)
        {
            if (item.IsEmpty)
            {
                ColorBlock.sprite = EmptyTile;
            }
            else
            {
                ColorBlock.sprite = FullTile;
                ColorBlock.color = item.Color;
            }
        }
    }
}