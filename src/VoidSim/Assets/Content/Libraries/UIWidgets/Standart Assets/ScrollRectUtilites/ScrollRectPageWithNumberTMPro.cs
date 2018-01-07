using UnityEngine;
using UIWidgets;
using TMPro;

namespace UIWidgets.TMProSupport {
	/// <summary>
	/// Scroll rect page with number.
	/// </summary>
	public class ScrollRectPageWithNumberTMPro : ScrollRectPage {
		/// <summary>
		/// The number.
		/// </summary>
		[SerializeField]
		public TextMeshProUGUI Number;

		/// <summary>
		/// Sets the page number.
		/// </summary>
		/// <param name="page">Page.</param>
		public override void SetPage(int page)
		{
			base.SetPage(page);
			if (Number!=null)
			{
				Number.text = (page + 1).ToString();
			}
		}
	}
}