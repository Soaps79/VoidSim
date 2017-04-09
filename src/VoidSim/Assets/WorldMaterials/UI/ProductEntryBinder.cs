using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.WorldMaterials.UI
{
    public class ProductEntryBinder : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _productNameField;
        [SerializeField]
        private TextMeshProUGUI _AmountField;

        public void Bind(string productName, int amount)
        {
            _productNameField.text = productName;
            _AmountField.text = amount.ToString();
        }
    }
}