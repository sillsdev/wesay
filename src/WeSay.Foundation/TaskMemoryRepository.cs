using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace WeSay.Foundation
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
			if(File.Exists(Path))
			{
				File.Delete(Path);
			}
			using (StreamWriter writer = File.CreateText(Path))
			{
				x.Serialize(writer, this);
			}
		}

	}

	public interface ITaskMemory
	{
		ITaskMemory CreateNewSection(string sectionName);
		void Set(string key, string value);
		void Set(string key, int value);
		string Get(string key, string defaultValue);
		int Get(string key, int defaultValue);
		void TrackSplitContainer(SplitContainer container, string key);
	}

	[Serializable]
	public class TaskMemory : ITaskMemory
	{
		private SerializableDictionary<string, string> _pairs = new SerializableDictionary<string, string>();

		public SerializableDictionary<string, string> Pairs
		{
			get { return _pairs; }
			set { _pairs = value; }
		}

		public  void Set(string key, string value)
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

		public void Set(string key, int value)
		{
			Set(key, value.ToString());
		}

		public  string Get(string key, string defaultValue)
		{
			string v;
			if( Pairs.TryGetValue(key, out v))
				return v;
			return defaultValue;
		}

		public int Get(string key, int defaultValue)
		{
			var s = Get(key, defaultValue.ToString());
			int i;
			return int.TryParse(s, out i) ? i : defaultValue;
		}

		public void TrackSplitContainer(SplitContainer container, string key)
		{
			Debug.WriteLine(key + " was " + Get(key, -1));

			container.SplitterDistance = Get(key, container.SplitterDistance);
			container.Tag = true;
			container.SplitterMoved += (splitContainer, e) =>
										   {

											   //skip the very first call, which is bogus (comes during a PerformLayout() from the tabbedForm)
											   if ((bool)container.Tag)
											   {
//                                                   Debug.WriteLine("Skipping... " + key + "<--" +
//                                                                   ((SplitContainer)splitContainer).SplitterDistance);
												   container.Tag = false;
												   return;
											   }

//                                               Debug.WriteLine(key + "<--" +
//                                                               ((SplitContainer)splitContainer).SplitterDistance);
//
											   Set(key,
												   ((SplitContainer) splitContainer).SplitterDistance);
										   };
		}

		public ITaskMemory CreateNewSection(string sectionName)
		{
			return new TaskMemorySection(this, sectionName);
		}
	}

	/// <summary>
	/// a *very* poor-man's hierachical settings container
	/// </summary>
	class TaskMemorySection : ITaskMemory
	{
		private readonly ITaskMemory _memory;
		private readonly string _sectionName;

		public TaskMemorySection(ITaskMemory parentMemory, string sectionName)
		{
			_memory = parentMemory;
			_sectionName = sectionName;
		}

		public ITaskMemory CreateNewSection(string sectionName)
		{
			return new TaskMemorySection(this, sectionName);
		}

		public  void Set(string key, string value)
		{
			_memory.Set(RealKey(key), value);
		}

		public void Set(string key, int value)
		{
			_memory.Set(RealKey(key), value);
		}

		public  string Get(string key, string defaultValue)
		{
			return _memory.Get(RealKey(key), defaultValue);
		}

		private string RealKey(string key)
		{
			return _sectionName+"."+key;
		}

		public int Get(string key, int defaultValue)
		{
			return _memory.Get(RealKey(key), defaultValue);
		}

		public void TrackSplitContainer(SplitContainer container, string key)
		{
			_memory.TrackSplitContainer(container, key);
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