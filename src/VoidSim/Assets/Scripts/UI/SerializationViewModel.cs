using Assets.Scripts.Serialization;
using Messaging;
using QGame;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI
{
	public class SerializationViewModel : QScript
	{
		private string filename = "gamesave.json";

		public void Save()
		{
			MessageHub.Instance.QueueMessage(GameMessages.PreSave, null);
			StopWatch.AddNode("savetime", .1f, true).OnTick += () => SerializationHub.Instance.WriteToFile(filename);
		}

		public void Load()
		{
			SerializationHub.Instance.LoadFromFile(filename);
			MessageHub.Instance.ClearListeners();
			var id = SceneManager.GetActiveScene().buildIndex;
			SceneManager.LoadScene(id);
		}
	}
}