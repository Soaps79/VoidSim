using UnityEngine;
using UIWidgets;
using UnityEngine.Serialization;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Test Notify.
	/// </summary>
	public class TestNotify : MonoBehaviour
	{
		/// <summary>
		/// Notification template.
		/// Gameobject in Hierarchy window, parent gameobject should have Layout component (recommended EasyLayout)
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("notifyPrefab")]
		protected Notify NotifyTemplate;

		/// <summary>
		/// Show notify.
		/// </summary>
		public void ShowNotify()
		{
			NotifyTemplate.Clone().Show("Achievement unlocked. Hide after 3 seconds.", customHideDelay: 3f);
		}
	}
}