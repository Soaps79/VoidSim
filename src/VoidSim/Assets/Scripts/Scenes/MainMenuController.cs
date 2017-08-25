using QGame;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Scenes
{
	public class MainMenuController : QScript
	{
		public void OnNewGame()
		{
			SceneManager.LoadScene("loading");
		}
	}
}