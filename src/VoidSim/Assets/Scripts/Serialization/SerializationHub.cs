using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using QGame;

namespace Assets.Scripts.Serialization
{
	public class SerializationHub : SingletonBehavior<SerializationHub>
	{
		private class SerializedCollection
		{
			public string Name;
			public string Json;
			[JsonIgnore]
			public bool IsHandled;
		}

		public bool IsLoading { get; private set; }

		private readonly Dictionary<string, object> _toSerialize = new Dictionary<string, object>();
		private readonly Dictionary<string, SerializedCollection> _deserialized = new Dictionary<string, SerializedCollection>();

		public void AddCollection(string collectionName, object obj)
		{
			if (_toSerialize.ContainsKey(collectionName))
				_toSerialize.Remove(collectionName);

			_toSerialize.Add(collectionName, obj);
			UberDebug.LogChannel(LogChannels.Serialization, string.Format("{0} collection added to serialization", collectionName));
		}

		// all collections are written to the specified file path
		// due to Unity being limited to an old version of json.net, we can't have 
		// a single file that's actually readable, so we write two
		// VoidSim.Console has a json.net version of this function that does both
		// if more flexibility is needed: 
		// https://stackoverflow.com/questions/19811301/merge-two-objects-during-serialization-using-json-net
		public void WriteToFile(string filename)
		{
			WriteToFileUsable(filename);
			WriteToFileReadable(filename + "_readable");
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

		public void WriteToFileReadable(string filename)
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
		public void LoadFromFile(string filename)
		{
			// load pairs of collection names and json documents
			var text = File.ReadAllText(filename);
			var table = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
			if (table == null || !table.Any())
				throw new Exception(string.Format("Unable to load data from file: {0}", filename));

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

			UberDebug.LogChannel(LogChannels.Serialization, string.Format("Data loaded from file {0}", filename));
			IsLoading = true;
		}

		// return the collection, turning IsLoading to false if all collections have been retrieved
		public string GetCollection(string collectionName)
		{
			if (_deserialized.ContainsKey(collectionName))
			{
				_deserialized[collectionName].IsHandled = true;
				if (_deserialized.All(i => i.Value.IsHandled))
					IsLoading = false;

				UberDebug.LogChannel(LogChannels.Serialization, string.Format("{0} collection fetched from hub", collectionName));
				return _deserialized[collectionName].Json;
			}
			return string.Empty;
		}
	}
}