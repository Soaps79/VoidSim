using QGame;
using UnityEngine.UI;

namespace Assets.WorldMaterials.UI
{
    /// <summary>
    /// Ties user interactions to a single entry of InventoryReserve.
    /// </summary>
    public class InventoryReserveViewModel : QScript
    {
        public Toggle BuyToggle;
        public Toggle SellToggle;
        public Slider Slider;

        private InventoryReserve _reserve;
        private int _productId;

        public void Initialize(InventoryReserve reserve, int productId, int maxAmount)
        {
            _reserve = reserve;
            _productId = productId;

            var current = _reserve.GetProductStatus(_productId);
            if(current == null)
                _reserve.AddReservation(_productId, 0, false, false);

            BuyToggle.onValueChanged.AddListener(HandleBuyChanged);
            SellToggle.onValueChanged.AddListener(HandleSellChanged);

            Slider.minValue = 0;
            Slider.maxValue = maxAmount;
            Slider.onValueChanged.AddListener(HandleAmountChanged);
        }

        private void HandleBuyChanged(bool value)
        {
            var current = _reserve.GetProductStatus(_productId);
            if(value != current.ShouldCollect)
                _reserve.SetCollect(_productId, value);
        }

        private void HandleSellChanged(bool value)
        {
            var current = _reserve.GetProductStatus(_productId);
            if (value != current.ShouldRelease)
                _reserve.SetRelease(_productId, value);
        }

        private void HandleAmountChanged(float amount)
        {
            _reserve.SetAmount(_productId, (int)amount);
        }
    }
}