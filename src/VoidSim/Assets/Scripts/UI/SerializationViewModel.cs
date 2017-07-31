using Assets.Scripts.Serialization;
using Messaging;
using QGame;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI
{
	public class SerializationViewModel : QScript
	{
		[SerializeField] private string _filename;
		public bool LoadOnStart;

		void Awake()
		{
			if (LoadOnStart)
				Load();
		}

		public void Save()
		{
			OnNextUpdate += SaveGame;
		}

		private void SaveGame(float obj)
		{
			Locator.MessageHub.QueueMessage(GameMessages.PreSave, null);
			OnNextUpdate += f => Locator.Serialization.WriteToFile(_filename);
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