using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Serialization;
using QGame;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
	public class LastIdData
	{
		public string Name;
		public int LastId;
	}

	public class LastIdManagerData
	{
		public List<LastIdData> LastIds;
	}

	public interface ILastIdManager
	{
		/// <summary>
		/// Returns the next value for the given name
		/// </summary>
		int GetNext(string idName);
		/// <summary>
		/// Resets the given Id to zero, adds an entry if not yet known
		/// </summary>
		/// <param name="idName"></param>
		void Reset(string idName);
	}

	/// <summary>
	/// Used by consumers that need to generate ID's for the objects they manage.
	/// There is no initialization needed, calling GetNext() will add the new type
	/// if it is not yet known. Values are serialized so loading the game continues the increments.
	/// </summary>
	public class LastIdManager : ILastIdManager, ISerializeData<LastIdManagerData>
	{
		private readonly Dictionary<string, int> _lastIds = new Dictionary<string, int>();

		private readonly CollectionSerializer<LastIdManagerData> _serializer
			= new CollectionSerializer<LastIdManagerData>();

		public LastIdManager()
		{
			// I *believe* sceneLoaded is being called before anything else is initialized
			// if the truth is otherwise, will need fixing
			Deserialize();
			SceneManager.sceneLoaded += HandleSceneLoad;
		}

		private void HandleSceneLoad(Scene scene, LoadSceneMode mode)
		{
			Deserialize();
		}

		private void Deserialize()
		{
			if (_serializer.HasDataFor(this, "LastIds", true))
			{
				var data = _serializer.DeserializeData();
				if (data == null && data.LastIds == null)
					return;

				_lastIds.Clear();
				data.LastIds.ForEach(i => _lastIds.Add(i.Name, i.LastId));
			}
		}

		/// <summary>
		/// Returns the next value for the given name
		/// </summary>
		public int GetNext(string idName)
		{
			if(!_lastIds.ContainsKey(idName))
				_lastIds.Add(idName, 0);

			_lastIds[idName]++;
			return _lastIds[idName];
		}

		public LastIdManagerData GetData()
		{
			return new LastIdManagerData
			{
				LastIds = _lastIds.Select(i => new LastIdData { Name = i.Key, LastId = i.Value }).ToList()
			};
		}

		public void Reset(string idName)
		{
			if (!_lastIds.ContainsKey(idName))
				_lastIds.Add(idName, 0);
			else
				_lastIds[idName] = 0;
		}
	}
}