#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1
#else
using UnityEngine;
using TMPro;

namespace UIWidgets.TMProSupport
{
	/// <summary>
	/// FileDialog TMPro.
	/// </summary>
	public class FileDialogTMPro : FileDialog
	{
		/// <summary>
		/// Filename Input.
		/// </summary>
		[SerializeField]
		public TMP_InputField FilenameInputTMPro;

		/// <summary>
		/// Init InputProxy.
		/// </summary>
		protected override void InitFilenameInputProxy()
		{
			if (FilenameInputProxy==null)
			{
				FilenameInputProxy = new InputFieldTMProProxy(FilenameInputTMPro);
			}
		}
	}
}
#endif