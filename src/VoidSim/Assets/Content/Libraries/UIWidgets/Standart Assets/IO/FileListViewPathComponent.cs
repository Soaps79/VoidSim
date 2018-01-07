using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace UIWidgets
{
	/// <summary>
	/// FileListViewPathComponent.
	/// </summary>
	public class FileListViewPathComponent : FileListViewPathComponentBase
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		protected Text Name;

		/// <summary>
		/// Set path.
		/// </summary>
		/// <param name="path">Path.</param>
		public override void SetPath(string path)
		{
			FullName = path;
			var dir = Path.GetFileName(path);
			Name.text = !string.IsNullOrEmpty(dir) ? dir : path;
		}
	}
}