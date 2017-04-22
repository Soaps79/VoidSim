using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.WorldMaterials.UI
{
    public class ProductEntryBinder : MonoBehaviour
    {
        public int ProductId { get; private set; }

        [SerializeField]
        private TextMeshProUGUI _productNameField;
        [SerializeField]
        private TextMeshProUGUI _AmountField;

        public void Bind(string productName, int amount, int productId)
        {
            _productNameField.text = productName;
            _AmountField.text = amount.ToString();
            ProductId = productId;
        }

        public void SetAmount(int amount)
        {
            _AmountField.text = amount.ToString();
        }
    }
}