﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

namespace UIWidgets
{
	/// <summary>
	/// FileView.
	/// </summary>
	public class FileListView : TileViewCustomSize<FileListViewComponentBase,FileSystemEntry>
	{
		/// <summary>
		/// Current directory.
		/// </summary>
		[SerializeField]
		protected string currentDirectory;

		/// <summary>
		/// Current directory.
		/// </summary>
		public string CurrentDirectory {
			get {
				return currentDirectory;
			}
			set {
				SetCurrentDirectory(value);
			}
		}

		/// <summary>
		/// Directory patterns.
		/// </summary>
		[SerializeField]
		protected string directoryPatterns = string.Empty;

		/// <summary>
		/// Gets or sets the directory patterns, semicolon used as separator.
		/// Directory will be displayed if it's match one of the pattern.
		/// Wildcards:
		/// * - Zero or more characters in that position.
		/// ? - Zero or one character in that position.
		/// Warning: if directory match two or more patterns it will be displayed two or more times.
		/// </summary>
		/// <value>The directory patterns.</value>
		public string DirectoryPatterns {
			get {
				return directoryPatterns;
			}
			set {
				directoryPatterns = value;
			}
		}

		/// <summary>
		/// File patterns.
		/// </summary>
		[SerializeField]
		protected string filePatterns = string.Empty;

		/// <summary>
		/// Gets or sets the file patterns, semicolon used as separator between patterns.
		/// File will be displayed if it's match one of the pattern.
		/// Wildcards:
		/// * - Zero or more characters in that position.
		/// ? - Zero or one character in that position.
		/// Warning: if file match two or more patterns it will be displayed two or more times.
		/// </summary>
		/// <value>The files patterns.</value>
		public string FilePatterns {
			get {
				return filePatterns;
			}
			set {
				filePatterns = value;
			}
		}

		/// <summary>
		/// Button Up.
		/// Open parent directory.
		/// </summary>
		[SerializeField]
		protected Button ButtonUp;

		/// <summary>
		/// FileListViewPath.
		/// Display path.
		/// </summary>
		[SerializeField]
		protected FileListViewPath PathView;

		/// <summary>
		/// DrivesListView.
		/// </summary>
		[SerializeField]
		protected DrivesListView DrivesListView;

		/// <summary>
		/// Display IO errors.
		/// </summary>
		[SerializeField]
		public IOExceptionsView ExceptionsView;

		/// <summary>
		/// Can display file system entry?
		/// </summary>
		public Func<FileSystemEntry,bool> CanDisplayEntry = DisplayAll;

		/// <summary>
		/// Default comparison.
		/// </summary>
		protected Comparison<FileSystemEntry> ComparisonDefault = (x, y) => {
			if (x.IsFile==y.IsFile)
			{
				return x.DisplayName.CompareTo(y.DisplayName);
			}
			return x.IsFile.CompareTo(y.IsFile);
		};

		bool isInited = false;

		/// <summary>
		/// Init and add listeners.
		/// </summary>
		public override void Init()
		{
			if (isInited)
			{
				return ;
			}
			isInited = true;

			base.Init();

			DataSource.Comparison = ComparisonDefault;

			SetCurrentDirectory(currentDirectory);

			if (ButtonUp!=null)
			{
				ButtonUp.onClick.AddListener(Up);
			}

			if (DrivesListView!=null)
			{
				DrivesListView.OnSelectObject.AddListener(ChangeDrive);
				DrivesListView.FileListView = this;
				DrivesListView.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected override void OnDestroy()
		{
			if (ButtonUp!=null)
			{
				ButtonUp.onClick.RemoveListener(Up);
			}
			if (DrivesListView!=null)
			{
				DrivesListView.OnSelectObject.RemoveListener(ChangeDrive);
			}

			base.OnDestroy();
		}

		/// <summary>
		/// Callback when drive changed.
		/// </summary>
		/// <param name="index">Drive index.</param>
		protected virtual void ChangeDrive(int index)
		{
			CurrentDirectory = DrivesListView.DataSource[index].FullName;
			DrivesListView.Close();
		}

		/// <summary>
		/// Open parent directory.
		/// </summary>
		public virtual void Up()
		{
			var current = CurrentDirectory;
			var directory = Path.GetDirectoryName(current);
			if (!string.IsNullOrEmpty(directory))
			{
				CurrentDirectory = directory;
				Select(DataSource.FindIndex(x => x.FullName==current));
			}
		}

		/// <summary>
		/// Set current directory.
		/// </summary>
		/// <param name="directory">New directory.</param>
		protected virtual void SetCurrentDirectory(string directory)
		{
			currentDirectory = Path.GetFullPath((string.IsNullOrEmpty(directory)) ? Application.persistentDataPath : directory);

			if (ButtonUp!=null)
			{
				ButtonUp.gameObject.SetActive(!string.IsNullOrEmpty(Path.GetDirectoryName(CurrentDirectory)));
			}

			if (PathView!=null)
			{
				PathView.FileView = this;
				PathView.Path = currentDirectory;
			}

			DataSource.BeginUpdate();
			DataSource.Clear();

			try
			{
				ExceptionsView.Execute(GetFiles);
			}
			finally
			{
				DataSource.EndUpdate();
			}
		}

		/// <summary>
		/// Get files.
		/// </summary>
		protected virtual void GetFiles()
		{
			if (!string.IsNullOrEmpty(directoryPatterns))
			{
				directoryPatterns.Split(';').ForEach(pattern => {
					var dirs = Directory.GetDirectories(currentDirectory, pattern);
					dirs.ForEach(AddDirectory);
				});
			}
			else
			{
				var dirs = Directory.GetDirectories(currentDirectory);
				dirs.ForEach(AddDirectory);
			}

			if (!string.IsNullOrEmpty(filePatterns))
			{
				filePatterns.Split(';').ForEach(pattern => {
					var files = Directory.GetFiles(currentDirectory, pattern);
					files.ForEach(AddFiles);
				});
			}
			else
			{
				var files = Directory.GetFiles(currentDirectory);
				files.ForEach(AddFiles);
			}
		}

		/// <summary>
		/// Add directory to DataSource.
		/// </summary>
		/// <param name="directory">Directory.</param>
		protected virtual void AddDirectory(string directory)
		{
			var item = new FileSystemEntry(directory, Path.GetFileName(directory), false);
			if (CanDisplayEntry(item))
			{
				DataSource.Add(item);
			}
		}

		/// <summary>
		/// Add files DataSource.
		/// </summary>
		/// <param name="file">File.</param>
		protected virtual void AddFiles(string file)
		{
			var item = new FileSystemEntry(file, Path.GetFileName(file), true);
			if (CanDisplayEntry(item))
			{
				DataSource.Add(item);
			}
		}

		#region Display
		/// <summary>
		/// Display all file system entry.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true.</returns>
		public static bool DisplayAll(FileSystemEntry item)
		{
			return true;
		}

		/// <summary>
		/// Display only directories.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if item is directory; otherwise, false.</returns>
		public static bool DisplayOnlyDirectories(FileSystemEntry item)
		{
			return item.IsDirectory;
		}

		/// <summary>
		/// Display only files.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if item is file; otherwise, false.</returns>
		public static bool DisplayOnlyFiles(FileSystemEntry item)
		{
			return item.IsFile;
		}
		#endregion
	}
}
