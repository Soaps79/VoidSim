using QGame;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Scenes
{
	public class LoadingController : QScript
	{
		private StopWatchNode _stopwatchNode;

		void Start()
		{
			_stopwatchNode = StopWatch.AddNode("loading_bar", 5, true);
			_stopwatchNode.OnTick += () => SceneManager.LoadScene("scene1");
		}
	}
}