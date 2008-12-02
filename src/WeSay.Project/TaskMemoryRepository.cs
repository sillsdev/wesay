using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace WeSay.Project
{
	/// <summary>
	/// Why not use Settings?  Chiefly, because these settins are not
	/// just per user, but per project.  Putting them in this obvious
	/// place also makes it easier to remove a file if it's triggering
	/// some bug.
	/// </summary>
	[Serializable]
	public class TaskMemoryRepository : IDisposable
	{
		public const string FileExtensionWithDot = ".wesayUserMemory";
		private SerializableDictionary<string, TaskMemory> _memories;

		[System.Xml.Serialization.XmlIgnore]
		public string Path { get; set; }

		public SerializableDictionary<string, TaskMemory> Memories
		{
			get { return _memories; }
			set { _memories = value; }
		}

		public TaskMemoryRepository()
		{
			Memories = new SerializableDictionary<string, TaskMemory>();
		}

		public static TaskMemoryRepository CreateOrLoadTaskMemoryRepository(string projectName, string settingsDirectory)
		{
			var path = System.IO.Path.Combine(settingsDirectory, projectName+FileExtensionWithDot);
			TaskMemoryRepository repo=null;
			if(File.Exists(path))
			{
				try
				{

					var x = new XmlSerializer(typeof (TaskMemoryRepository));
					using (FileStream stream = File.OpenRead(path))
					{
						repo = x.Deserialize(stream) as TaskMemoryRepository;
					}
				}
				catch(Exception error)
				{
					Palaso.Reporting.Logger.WriteEvent("Error trying to read task memory: "+error.Message);
					//now just make a new one, this is the kind of data we can through away
				}
			}

			if(repo==null)
			{
				repo = new TaskMemoryRepository();
			}
			repo.Path = path;
			return repo;
		}

		public TaskMemory FindOrCreateSettingsByTaskId(string id)
		{
			TaskMemory memory;
			if(!Memories.TryGetValue(id, out memory))
			{
				memory = new TaskMemory();
				Memories.Add(id, memory);
			}
			return memory;
		}

		public void Dispose()
		{
			var x = new XmlSerializer(typeof(TaskMemoryRepository));
			using (FileStream stream = File.OpenWrite(Path))
			{
				 x.Serialize(stream, this);
			}
		}


	}

	[Serializable]
	public class TaskMemory
	{
		private SerializableDictionary<string, string> _pairs = new SerializableDictionary<string, string>();

		public SerializableDictionary<string, string> Pairs
		{
			get { return _pairs; }
			set { _pairs = value; }
		}

		public void Set(string key, string value)
		{
			if(Pairs.ContainsKey(key))
			{
				Pairs[key] = value;
			}
			else
			{
				Pairs.Add(key, value);
			}
		}
		public string Get(string key, string defaultValue)
		{
			string v;
			if( Pairs.TryGetValue(key, out v))
				return v;
			return defaultValue;
		}
	}

	/// <summary>
	/// from Paul Welter's blog
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	[XmlRoot("dictionary")]
	public class SerializableDictionary<TKey, TValue>
		: Dictionary<TKey, TValue>, IXmlSerializable
	{
		#region IXmlSerializable Members

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}


		public void ReadXml(System.Xml.XmlReader reader)
		{
			XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));

			XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));


			bool wasEmpty = reader.IsEmptyElement;

			reader.Read();


			if (wasEmpty)

				return;


			while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				reader.ReadStartElement("item");


				reader.ReadStartElement("key");

				TKey key = (TKey)keySerializer.Deserialize(reader);

				reader.ReadEndElement();


				reader.ReadStartElement("value");

				TValue value = (TValue)valueSerializer.Deserialize(reader);

				reader.ReadEndElement();


				this.Add(key, value);


				reader.ReadEndElement();

				reader.MoveToContent();
			}

			reader.ReadEndElement();
		}


		public void WriteXml(System.Xml.XmlWriter writer)
		{
			XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));

			XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));


			foreach (TKey key in this.Keys)
			{
				writer.WriteStartElement("item");


				writer.WriteStartElement("key");

				keySerializer.Serialize(writer, key);

				writer.WriteEndElement();


				writer.WriteStartElement("value");

				TValue value = this[key];

				valueSerializer.Serialize(writer, value);

				writer.WriteEndElement();


				writer.WriteEndElement();
			}
		}

		#endregion
	}
}