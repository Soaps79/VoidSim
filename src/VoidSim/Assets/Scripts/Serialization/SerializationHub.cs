using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using QGame;

namespace Assets.Scripts.Serialization
{
	/// <summary>
	/// Manages the actual writing and reading of game data. Game code should not interact with this, 
	/// objects should instead instantiate a CollectionSerializer of their data type
	/// </summary>
	public class SerializationHub : SingletonBehavior<SerializationHub>
	{
		private class SerializedCollection
		{
			public string Name;
			public string Json;
			[JsonIgnore]
			public bool IsHandled;
		}

		// returns true as long as any collections that have been loaded from file have not yet been retrieved in-game
		public bool IsLoading { get; private set; }
		private string _fileExtension = ".json";

		private readonly Dictionary<string, object> _toSerialize = new Dictionary<string, object>();
		private readonly Dictionary<string, SerializedCollection> _deserialized = new Dictionary<string, SerializedCollection>();

		public void AddCollection(string collectionName, object obj)
		{
			if (_toSerialize.ContainsKey(collectionName))
				_toSerialize.Remove(collectionName);

			_toSerialize.Add(collectionName, obj);
			UberDebug.LogChannel(LogChannels.Serialization, string.Format("{0} collection added to serialization", collectionName));
		}

		// all collections are written to the specified file path.
		// Unity is limited to an old version of json.net (modified 9.0), and I can't figure out
		// a way to write a single file that's legible and deserializable using it, so we write two.
		// VoidSim.Console has a json.net v10 of this function that achieves both.
		// if more flexibility is needed with v10: 
		// https://stackoverflow.com/questions/19811301/merge-two-objects-during-serialization-using-json-net
		public void WriteToFile(string savename)
		{
			WriteToFileUsable(savename + _fileExtension);
			WriteToFileReadable(savename + "_readable" + _fileExtension);
		}

		private void WriteToFileUsable(string filename)
		{
			UberDebug.LogChannel(LogChannels.Serialization,
				!_toSerialize.Any()
					? string.Format("No data present to write to file: {0}", filename)
					: string.Format("Writing to file: {0}", filename));

			using (FileStream fs = File.Open(filename, FileMode.Create, FileAccess.Write))
			using (StreamWriter sw = new StreamWriter(fs))
			using (JsonWriter jw = new JsonTextWriter(sw))
			{
				jw.Formatting = Formatting.Indented;
				jw.WriteStartObject();
				foreach (var collection in _toSerialize)
				{
					jw.WritePropertyName(collection.Key);
					jw.WriteValue(JsonConvert.SerializeObject(collection.Value, Formatting.Indented));
				}
				jw.WriteEndObject();
			}
		}

		private void WriteToFileReadable(string filename)
		{
			UberDebug.LogChannel(LogChannels.Serialization,
				!_toSerialize.Any()
					? string.Format("No data present to write to file: {0}", filename)
					: string.Format("Writing to file: {0}", filename));

			using (FileStream fs = File.Open(filename, FileMode.Create, FileAccess.Write))
			using (StreamWriter sw = new StreamWriter(fs))
			using (JsonWriter jw = new JsonTextWriter(sw))
			{
				jw.Formatting = Formatting.Indented;
				//JsonSerializer serializer = new JsonSerializer();
				//serializer.Serialize(jw, _collections);

				// would normally JsonSerializer.Serialize but 
				jw.WriteStartObject();
				foreach (var collection in _toSerialize)
				{
					jw.WritePropertyName(collection.Key);
					jw.WriteRawValue(JsonConvert.SerializeObject(collection.Value, Formatting.Indented));
				}
				jw.WriteEndObject();

			}
		}

		// loads the file's contents into _deserialized
		public void LoadFromFile(string savename)
		{
			// load pairs of collection names and json documents
			var text = File.ReadAllText(savename  + _fileExtension);
			var table = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
			if (table == null || !table.Any())
				throw new Exception(string.Format("Unable to load data from file: {0}", savename));

			// hold them in indexed table
			_deserialized.Clear();
			foreach (var pair in table)
			{
				_deserialized.Add(pair.Key, new SerializedCollection
				{
					Name = pair.Key,
					Json = pair.Value.ToString()
				});
			}

			UberDebug.LogChannel(LogChannels.Serialization, string.Format("Data loaded from file {0}", savename));
			IsLoading = true;
		}

		// return the collection, turning IsLoading to false if all collections have been retrieved
		public string GetCollection(string collectionName)
		{
			if (_deserialized.ContainsKey(collectionName))
			{
				_deserialized[collectionName].IsHandled = true;
				var completed = _deserialized.Where(i => i.Value.IsHandled);
				var countRemaining = _deserialized.Count - completed.Count();

				if(countRemaining <= 0)
					IsLoading = false;

				UberDebug.LogChannel(LogChannels.Serialization, 
					string.Format("{0} collection fetched from hub\t {1} remaining", collectionName, countRemaining));
				return _deserialized[collectionName].Json;
			}
			return string.Empty;
		}
	}
}