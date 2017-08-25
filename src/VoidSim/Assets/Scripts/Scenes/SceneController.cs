using QGame;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Scenes
{
	public class SceneController : QScript
	{
		public void OnNewGame()
		{
			SceneManager.LoadScene("scene1");
		}
	}
}