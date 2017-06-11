using Assets.Scripts.Serialization;
using Messaging;
using QGame;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI
{
	public class SerializationViewModel : QScript
	{
		private string filename = "gamesave.json";
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
			MessageHub.Instance.QueueMessage(GameMessages.PreSave, null);
			OnNextUpdate += f => SerializationHub.Instance.WriteToFile(filename);
		}

		public void LoadScene()
		{
			Load();
			MessageHub.Instance.ClearListeners();
			var id = SceneManager.GetActiveScene().buildIndex;
			SceneManager.LoadScene(id);
		}

		private void Load()
		{
			SerializationHub.Instance.LoadFromFile(filename);
		}
	}
}