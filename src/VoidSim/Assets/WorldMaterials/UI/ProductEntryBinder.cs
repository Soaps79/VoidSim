using DG.Tweening;
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

	    private Color _baseColor;

	    public void Bind(string productName, int amount, int productId)
        {
            _productNameField.text = productName;
            _AmountField.text = amount.ToString();
	        _baseColor = _AmountField.color;

            ProductId = productId;
        }

        public void SetAmount(int amount)
        {
            _AmountField.text = amount.ToString();
        }

	    public void PulseColorFrom(Color startColor, float time)
	    {
		    _AmountField.DOKill();
		    if (startColor != null)
			    _AmountField.color = startColor;
		    _AmountField.DOColor(_baseColor, time);
	    }
    }
}