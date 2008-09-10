using System;
using System.Collections.Generic;
using System.Text;
using WeSay.Data;
using WeSay.Foundation.Options;

namespace WeSay.LexicalModel
{
	internal class SemanticDomainQuery :IQuery<LexEntry>
	{
		public bool Matches(LexEntry item)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<string> GetDisplayStrings(LexEntry item)
		{
			List<string> keys = new List<string>();
			foreach (LexSense sense in item.Senses)
			{
				OptionRefCollection semanticDomains = sense.GetProperty<OptionRefCollection>(_semanticDomainFieldName);

				if (semanticDomains != null)
				{
					foreach (string s in semanticDomains.Keys)
					{
						if (!keys.Contains(s))
						{
							keys.Add(s);
						}
					}
				}
			}
			return keys;
		}

		public List<RecordToken> GetDisplayStringsForAllMatching()
		{
			throw new NotImplementedException();
		}
	}
}
