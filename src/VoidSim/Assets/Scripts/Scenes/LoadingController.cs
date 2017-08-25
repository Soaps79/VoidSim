using Assets.Controllers.GUI;
using QGame;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Scenes
{
	public class LoadingController : QScript
	{
		private StopWatchNode _stopwatchNode;
		[SerializeField] private Slider _slider;

		void Start()
		{
			_stopwatchNode = StopWatch.AddNode("loading_bar", 5);
			_stopwatchNode.OnTick += () =>
			{
				_stopwatchNode.Pause();
				SceneManager.LoadScene("scene1");
			};

			var binding = _slider.gameObject.AddComponent<SliderBinding>();
			binding.Initialize( () => _stopwatchNode.RemainingLifetimeAsZeroToOne );
		}
	}
}