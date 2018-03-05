using Assets.Placeables.Nodes;
using UIWidgets;
using UnityEngine;

namespace Assets.Placeables.UI
{
    /// <summary>
    /// Represents the pop container placeable node.
    /// </summary>
    public class PopContainerViewModel : TileView<OccupantViewModel, Occupancy>
    {
        private PopContainer _container;
        private int _lastReserveCount;
        
        public void Initialize(PopContainer popContainer)
        {
            DataSource = popContainer.CurrentOccupancy.ToObservableList();
            _container = popContainer;
            // currently will not handle container resizing
            UpdateContainer();
        }

        private void UpdateContainer()
        {
            DataSource = _container.CurrentOccupancy.ToObservableList();
            var rows = DataSource.Count / Layout.GridConstraintCount;
            if (rows < 1) rows = 1;
            var rect = GetComponent<RectTransform>();
            var width = Layout.GridConstraintCount * (itemWidth + Layout.Spacing.x) + Layout.GetMarginLeft() + Layout.GetMarginRight();
            var height = rows * (itemHeight + Layout.Spacing.y) + Layout.GetMarginTop() + Layout.GetMarginBottom();
            rect.sizeDelta = new Vector2(width, height);
        }
    }
}