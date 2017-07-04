using System;
using UnityEngine;

namespace Assets.Controllers.GUI
{
	public class ClosePanelButton : MonoBehaviour
	{
		public Action OnClose;

		public void ClosePanel()
		{
			gameObject.SetActive(false);
			if (OnClose != null)
				OnClose();
		}
	}
}
