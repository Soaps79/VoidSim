using UnityEngine;
using System.Collections;
using System.Linq;
using UIWidgets;
using UnityEngine.Serialization;

namespace UIWidgets.Tests
{
	/// <summary>
	/// Smoke test.
	/// </summary>
	public class SmokeTest : MonoBehaviour
	{
		/// <summary>
		/// Accordion.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("accordion")]
		protected Accordion Accordion;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			#if UNITY_STANDALONE
			if (System.Environment.GetCommandLineArgs().Contains("-smoke-test"))
			{
				StartCoroutine(SimpleTest());
			}
			#endif
		}

		/// <summary>
		/// Simple test.
		/// </summary>
		/// <returns>Coroutine.</returns>
		protected virtual IEnumerator SimpleTest()
		{
			yield return new WaitForSeconds(5f);
			
			var items = Accordion.DataSource;
			if (!Accordion.DataSource[0].Open || !Accordion.DataSource[0].ContentObject.activeSelf)
			{
				throw new UnityException("Overview is not active!");
			}

			foreach (var item in items)
			{
				if (item.ToggleObject.name=="Exit")
				{
					continue ;
				}
				Accordion.ToggleItem(item);
				yield return new WaitForSeconds(5f);
			}

			Application.Quit();
		}
	}
}