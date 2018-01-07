﻿using UnityEngine;
using UIWidgets;

namespace UIWidgetsSamples.Tasks
{
	/// <summary>
	/// Task Picker.
	/// </summary>
	public class TaskPicker : Picker<Task,TaskPicker>
	{
		/// <summary>
		/// TaskView.
		/// </summary>
		[SerializeField]
		public TaskView TaskView;

		/// <summary>
		/// Set initial value and add callbacj.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public override void BeforeOpen(Task defaultValue)
		{
			// set default value
			TaskView.SelectedIndex = TaskView.DataSource.IndexOf(defaultValue);

			// add callback
			TaskView.OnSelectObject.AddListener(ListViewCallback);
		}

		/// <summary>
		/// Callback when value selected.
		/// </summary>
		/// <param name="index">Selected item index.</param>
		void ListViewCallback(int index)
		{
			// apply selected value and close picker
			Selected(TaskView.DataSource[index]);
		}

		/// <summary>
		/// Remove listener.
		/// </summary>
		public override void BeforeClose()
		{
			// remove callback
			TaskView.OnSelectObject.RemoveListener(ListViewCallback);
		}
	}
}