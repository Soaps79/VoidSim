using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UIWidgets;

namespace UIWidgetsSamples
{
	/// <summary>
	/// Test ListView.
	/// </summary>
	[RequireComponent(typeof(Button))]
	public class TestListView : MonoBehaviour
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("listView")]
		public ListView ListView;

		/// <summary>
		/// Test button.
		/// </summary>
		[HideInInspector]
		protected Button Button;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Button = GetComponent<Button>();
			if (Button!=null)
			{
				Button.onClick.AddListener(Click);
			}
		}

		/// <summary>
		/// Items.
		/// </summary>
		protected ObservableList<string> items;

		/// <summary>
		/// Clicks count.
		/// </summary>
		protected int click = 0;

		/// <summary>
		/// Handle click event.
		/// </summary>
		protected virtual void Click()
		{
			if (click==0)
			{
				items = ListView.DataSource;

				items.Add("Added from script 0");
				items.Add("Added from script 1");
				items.Add("Added from script 2");

				items.Remove("Caster");

				click += 1;
				return ;
			}
			if (click==1)
			{
				items.Clear();
				
				click += 1;
				return ;
			}
			if (click==2)
			{
				items.Add("test");
				
				click += 1;
				return ;
			}
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (Button!=null)
			{
				Button.onClick.RemoveListener(Click);
			}
		}
	}
}