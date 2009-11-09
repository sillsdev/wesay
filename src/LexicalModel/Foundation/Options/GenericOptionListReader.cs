using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Palaso.Text;

namespace Palaso.LexicalModel.Options
{

	public interface IOptionListReader
	{
		OptionsList LoadFromFile(string path);
	}

	public class GenericOptionListReader : IOptionListReader
	{
		public OptionsList LoadFromFile(string path)
		{
			XmlSerializer serializer = GetSerializer();

			using (XmlReader reader = XmlReader.Create(path))
			{
				OptionsList list = (OptionsList) serializer.Deserialize(reader);
				reader.Close();

				foreach (Option option in list.Options)
				{
					#if DEBUG
					Debug.Assert(option.Name.Forms != null);
					#endif
				}
				return list;
			}
		}

		protected virtual XmlSerializer GetSerializer()
		{
			return new XmlSerializer(typeof(OptionsList));
		}

	}
}
