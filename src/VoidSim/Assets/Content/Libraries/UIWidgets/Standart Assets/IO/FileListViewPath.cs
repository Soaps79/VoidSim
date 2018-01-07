﻿using UnityEngine;
using System.Collections.Generic;

namespace UIWidgets
{
	/// <summary>
	/// FileListViewPath.
	/// </summary>
	public class FileListViewPath : MonoBehaviour
	{
		/// <summary>
		/// FileView.
		/// </summary>
		[HideInInspector]
		public FileListView FileView;

		/// <summary>
		/// Current path.
		/// </summary>
		protected string path;

		/// <summary>
		/// Current path.
		/// </summary>
		public string Path {
			get {
				return path;
			}
			set {
				SetPath(value);
			}
		}

		/// <summary>
		/// FileListViewPathComponent template.
		/// </summary>
		[SerializeField]
		public FileListViewPathComponentBase Template;

		/// <summary>
		/// Used components.
		/// </summary>
		[HideInInspector]
		protected List<FileListViewPathComponentBase> Components = new List<FileListViewPathComponentBase>();

		/// <summary>
		/// Start this instance.
		/// </summary>
		public virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			Template.gameObject.SetActive(false);
		}

		/// <summary>
		/// Directories list from current to root.
		/// </summary>
		protected List<string> CurrentDirectories = new List<string>();

		/// <summary>
		/// Set path.
		/// </summary>
		/// <param name="newPath">New path.</param>
		protected virtual void SetPath(string newPath)
		{
			path = newPath;

			CurrentDirectories.Clear();
			do
			{
				CurrentDirectories.Add(newPath);
				newPath = System.IO.Path.GetDirectoryName(newPath);
			}
			while (!string.IsNullOrEmpty(newPath));
			CurrentDirectories.Reverse();

			for (int i = Components.Count-1; i >= CurrentDirectories.Count; i--)
			{
				var c = Components[i];
				Components.RemoveAt(i);
				c.Owner = null;
				c.Free();
			}
			for (int i = Components.Count; i < CurrentDirectories.Count; i++)
			{
				Components.Add(Template.Instance());
			}
			CurrentDirectories.ForEach((x, i) => {
				Components[i].Owner = this;
				Components[i].SetPath(x);
			});
		}

		/// <summary>
		/// Open directory.
		/// </summary>
		/// <param name="directory">Directory.</param>
		public virtual void Open(string directory)
		{
			var index = CurrentDirectories.IndexOf(directory);
			var select_directory = index==(CurrentDirectories.Count-1) ? string.Empty : CurrentDirectories[index+1];
			FileView.CurrentDirectory = directory;
			if (!string.IsNullOrEmpty(select_directory))
			{
				FileView.Select(FileView.DataSource.FindIndex(x => x.FullName==select_directory));
			}
		}

		/// <summary>
		/// Destroy created components.
		/// </summary>
		protected virtual void OnDestroy()
		{
			Components.ForEach(x => {
				x.Owner = null;
				x.Free();
			});
			Components.Clear();
		}
	}
}