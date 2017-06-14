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

	/// <summary>
	/// Used by objects that need to generate ID's for their managed objects.
	/// There is no initialization needed, calling GetNext() will add the new type
	/// if it is not yet known. Values are serialized so loading the game continues the increments.
	/// </summary>
	public class LastIdManager : SingletonBehavior<LastIdManager>, ISerializeData<LastIdManagerData>
	{
		private readonly Dictionary<string, int> _lastIds = new Dictionary<string, int>();

		private readonly CollectionSerializer<LastIdManagerData> _serializer
			= new CollectionSerializer<LastIdManagerData>();

		void Awake()
		{
			SceneManager.sceneLoaded += HandleSceneLoad;
		}

		private void HandleSceneLoad(Scene scene, LoadSceneMode mode)
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
	}
}