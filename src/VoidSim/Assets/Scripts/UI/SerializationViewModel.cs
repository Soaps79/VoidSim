using QGame;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI
{
	/// <summary>
	/// Handles UI display and button presses affecting serialization
	/// </summary>
	public class SerializationViewModel : QScript
	{
		[SerializeField] private string _filename;
		public bool LoadOnStart;

		void Awake()
		{
			// this behavior should be owned by something else, not this UI class
			if (LoadOnStart)
				Load();
		}

		public void Save()
		{
			OnNextUpdate += SaveGame;
		}

		private void SaveGame()
		{
			Locator.MessageHub.QueueMessage(GameMessages.PreSave, null);
			OnNextUpdate += () => Locator.Serialization.WriteToFile(_filename);
		}

		public void LoadScene()
		{
			Load();
			Locator.MessageHub.ClearListeners();
			var id = SceneManager.GetActiveScene().buildIndex;
			SceneManager.LoadScene(id);
		}

		private void Load()
		{
			Locator.Serialization.LoadFromFile(_filename);
		}

		public void ExitGame()
		{
			Application.Quit();
		}
	}
}