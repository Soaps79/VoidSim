using Assets.Controllers.GUI;
using DG.Tweening;
using QGame;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Scenes
{
	public class LoadingController : QScript
	{
		private StopWatchNode _stopwatchNode;
		[SerializeField] private Slider _slider;
		[SerializeField] private TMP_Text _text;
		private SliderBinding _binding;
		public float LoadTime;

		void Start()
		{
			var node = StopWatch.AddNode("loading_bar", LoadTime, true);
			node.OnTick += HandleLoadComplete;
			_binding = _slider.gameObject.AddComponent<SliderBinding>();
			_binding.Initialize(() => node.RemainingLifetimeAsZeroToOne);
		}

		private void HandleLoadComplete()
		{
			// yes this is weird, needs to be refactored for a smooth ending
			Destroy(_binding);
			_slider.value = 1;
			_text.gameObject.SetActive(true);

			OnEveryUpdate += CheckForClick;
		}

		private void CheckForClick(float obj)
		{
			if (!Input.GetMouseButtonUp(0))
				return;

			SceneManager.LoadScene("scene1");
		}
	}
}