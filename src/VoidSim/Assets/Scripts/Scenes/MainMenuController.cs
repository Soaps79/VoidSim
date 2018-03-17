using Assets.Scripts.Initialization;
using QGame;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Scenes
{
	public class MainMenuController : QScript
	{
		public CanvasGroup MenuToLoad;

		void Start()
		{
			ServiceInitializer.Initialize();
			OnEveryUpdate += CheckForMenuLoadButtonPress;
		}

		private void CheckForMenuLoadButtonPress()
		{
			if (Input.GetButtonDown("Confirm"))
			{
				MenuToLoad.gameObject.SetActive(true);
				OnEveryUpdate -= CheckForMenuLoadButtonPress;
			}
		}

		public void OnNewGame()
		{
			TransitionToLoadingScreen();
		}

		private static void TransitionToLoadingScreen()
		{
			SceneManager.LoadScene("loading");
		}

		public void OnContinue()
		{
			Locator.Serialization.LoadFromFile("gamesave");
			TransitionToLoadingScreen();
		}

		public void OnExit()
		{
			Application.Quit();
		}
	}
}