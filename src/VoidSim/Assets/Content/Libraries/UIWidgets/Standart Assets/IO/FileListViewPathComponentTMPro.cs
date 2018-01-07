using UnityEngine;
using TMPro;
using System.IO;

namespace UIWidgets.TMProSupport
{
	/// <summary>
	/// FileListViewPath component TMPro.
	/// </summary>
	public class FileListViewPathComponentTMPro : FileListViewPathComponentBase
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		protected TextMeshProUGUI Name;

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