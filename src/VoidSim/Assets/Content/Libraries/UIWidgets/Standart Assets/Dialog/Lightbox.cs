using UnityEngine;
using UnityEngine.UI;

namespace UIWidgets
{
	/// <summary>
	/// Lightbox.
	/// Display modal image.
	/// </summary>
	[RequireComponent(typeof(Image))]
	public class Lightbox : MonoBehaviour
	{
		/// <summary>
		/// The modal key.
		/// </summary>
		protected int? ModalKey;

		/// <summary>
		/// Display specified image.
		/// </summary>
		/// <param name="image">Image to display.</param>
		/// <param name="modalSprite">Modal background sprite.</param>
		/// <param name="modalColor">Modal background color.</param>
		/// <param name="canvas">Canvas.</param>
		public virtual void Show(
						Sprite image,
						Sprite modalSprite = null,
						Color? modalColor = null,
						Canvas canvas = null)
		{
			GetComponent<Image>().sprite = image;

			var parent = (canvas!=null) ? canvas.transform : Utilites.FindTopmostCanvas(gameObject.transform);
			if (parent!=null)
			{
				transform.SetParent(parent, false);
			}

			if (modalColor==null)
			{
				modalColor = new Color(0, 0, 0, 0.8f);
			}

			ModalKey = ModalHelper.Open(this, modalSprite, modalColor, Close);

			transform.SetAsLastSibling();

			gameObject.SetActive(true);
		}

		/// <summary>
		/// Close lightbox.
		/// </summary>
		public virtual void Close()
		{
			gameObject.SetActive(false);
			
			if (ModalKey!=null)
			{
				ModalHelper.Close((int)ModalKey);
			}
		}
	}
}

