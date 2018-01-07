using UnityEngine.UI;
using UIWidgets;

namespace UIWidgetsSamples.Tasks
{
	/// <summary>
	/// Task component.
	/// </summary>
	public class TaskComponent : ListViewItem, IViewData<Task>
	{
		/// <summary>
		/// Foreground graphics for coloring.
		/// </summary>
		public override Graphic[] GraphicsForeground {
			get {
				return new Graphic[] {Name, };
			}
		}

		/// <summary>
		/// Name.
		/// </summary>
		public Text Name;

		/// <summary>
		/// Progressbar.
		/// </summary>
		public Progressbar Progressbar;

		Task _item;

		/// <summary>
		/// Current task.
		/// </summary>
		public Task Item {
			get {
				return _item;
			}
			set {
				if (_item!=null)
				{
					_item.OnProgressChange -= UpdateProgressbar;
				}
				_item = value;
				if (_item!=null)
				{
					Name.text = _item.Name;
					Progressbar.Value = _item.Progress;

					_item.OnProgressChange += UpdateProgressbar;
				}
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(Task item)
		{
			Item = item;
		}

		void UpdateProgressbar()
		{
			Progressbar.Animate(_item.Progress);
		}

		/// <summary>
		/// Reset current item.
		/// </summary>
		protected override void OnDestroy()
		{
			Item = null;
		}
	}
}