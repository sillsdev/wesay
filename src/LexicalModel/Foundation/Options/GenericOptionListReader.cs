using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Palaso.Lift.Options;
using Palaso.Text;
using WeSay.LexicalModel.Foundation.Options;

namespace Palaso.Lift.Options // TODO Send this out to Palaso.Lift
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
