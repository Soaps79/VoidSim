﻿using UIWidgets;

namespace UIWidgetsSamples.Tasks
{
	/// <summary>
	/// TaskView.
	/// </summary>
	public class TaskView : ListViewCustom<TaskComponent,Task>
	{
		/// <summary>
		/// Tasks comparison.
		/// </summary>
		public static System.Comparison<Task> ItemsComparison = (x, y) => x.Name.CompareTo(y.Name);

		bool isTaskViewInited = false;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isTaskViewInited)
			{
				return ;
			}
			isTaskViewInited = true;

			base.Init();

			DataSource.Comparison = ItemsComparison;
		}
	}
}