using UnityEngine;

namespace UIWidgets
{
	/// <summary>
	/// TreeGraph component.
	/// </summary>
	/// <typeparam name="T">Node type.</typeparam>
	[RequireComponent(typeof(MultipleConnector))]
	abstract public class TreeGraphComponent<T> : MonoBehaviour
	{
		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="node">Node.</param>
		abstract public void SetData(TreeNode<T> node);

		/// <summary>
		/// Called when item moved to cache, you can use it free used resources.
		/// </summary>
		public virtual void MovedToCache()
		{
			
		}
	}
}

