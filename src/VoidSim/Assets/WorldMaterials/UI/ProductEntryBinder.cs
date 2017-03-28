using UnityEngine;
using UnityEngine.UI;

namespace Assets.WorldMaterials.UI
{
    public class ProductEntryBinder : MonoBehaviour
    {
        [SerializeField]
        private Text _productNameField;
        [SerializeField]
        private Text _AmountField;

        public void Bind(string productName, int amount)
        {
            _productNameField.text = productName;
            _AmountField.text = amount.ToString();
        }
    }
}