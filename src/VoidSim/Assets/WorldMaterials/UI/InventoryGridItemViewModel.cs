using UIWidgets;
using UnityEngine.UI;

namespace Assets.WorldMaterials.UI
{
    public class InventoryGridItemViewModel : ListViewItem, IViewData<InventoryGridTile>
    {
        public Image ColorBlock;

        public void SetData(InventoryGridTile item)
        {
            ColorBlock.color = item.Color;
        }
    }
}