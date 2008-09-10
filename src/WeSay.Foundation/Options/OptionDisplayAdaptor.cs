using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WeSay.Foundation.Options
{
	public class OptionDisplayAdaptor: IChoiceSystemAdaptor<Option, string, OptionRef>
	{
		private readonly OptionsList _allOptions;
		private readonly string _preferredWritingSystemId;
		private readonly IDisplayStringAdaptor _toolTipAdaptor;

		public OptionDisplayAdaptor(OptionsList allOptions, string preferredWritingSystemId)
		{
			_allOptions = allOptions;
			_preferredWritingSystemId = preferredWritingSystemId;
			_toolTipAdaptor = null;
		}

		#region IDisplayStringAdaptor Members

		public string GetDisplayLabel(object item)
		{
			if (item == null)
			{
				return string.Empty;
			}

			Option option = item as Option; // _allOptions.GetOptionFromKey((string)item);

			if (option == null)
			{
				return (string) item; // no matching object, just show the key
			}

			return option.Name.GetBestAlternative(_preferredWritingSystemId);
		}

		public string GetToolTip(object item)
		{
			Option option = (Option) item;
			return option.Description.GetBestAlternative(_preferredWritingSystemId);
		}

		public string GetToolTipTitle(object item)
		{
			Option option = (Option) item;
			return option.Abbreviation.GetBestAlternative(_preferredWritingSystemId);
		}

		#endregion

		//other delegates

		public string GetKeyFromOption(Option t)
		{
			return t.Key;
		}

		/// <summary>
		/// GetValueFromKeyValueDelegate
		/// </summary>
		/// <returns></returns>
		public Option GetOptionFromKey(string s) //review: is this the key?
		{
			Option result = _allOptions.Options.Find(delegate(Option opt) { return opt.Key == s; });

			// string name = " no match";
			//                if (result != null)
			//                    name = result.Name.GetFirstAlternative();
			//Debug.WriteLine("GetOptionFromKey(" + s + ") = " + name);
			return result;
		}

		public Option GetOptionFromOptionRef(OptionRef oref)
		{
			return GetOptionFromKey(oref.Key);
		}

		public void UpdateKeyContainerFromKeyValue(Option kv, OptionRef oRef)
		{
			oRef.Key = kv.Key;
		}

		public IDisplayStringAdaptor ToolTipAdaptor
		{
			get
			{
				if (_toolTipAdaptor == null)
				{
					//_toolTipAdaptor = new
				}
				return _toolTipAdaptor;
			}
		}

		public string GetValueFromKeyValue(Option kv)
		{
			return GetKeyFromOption(kv);
		}

		public Option GetKeyValueFromKey_Container(OptionRef kc)
		{
			return GetOptionFromOptionRef(kc);
		}

		#region IChoiceSystemAdaptor<Option,string,OptionRef> Members

		public Option GetKeyValueFromValue(string t)
		{
			return GetOptionFromKey(t);
		}

		public Option GetValueFromForm(string form)
		{
			foreach (Option item in _allOptions.Options)
			{
				if (String.Compare(GetDisplayLabel(item), form, true) == 0)
				{
					return item;
				}
			}
			return null;
		}

		public object GetValueFromFormNonGeneric(string form)
		{
			return GetValueFromForm(form);
		}

		public IEnumerable GetItemsToOffer(string text,
										   IEnumerable items,
										   IDisplayStringAdaptor adaptor)
		{
			List<Option> show = new List<Option>();
#if !DEBUG
			try
			{
#endif
			//enhance: make that text safe for regex. for now, we just swallow the exception
			string pattern = @"(^|[^\w])(" + text + @")[^\w]";
			Regex MatchWholeWordNoCase = new Regex(pattern,
												   RegexOptions.IgnoreCase | RegexOptions.Compiled);

			foreach (Option option in _allOptions.Options)
			{
				//todo: make this prefferd script(s) savvy
				if (option.Name.GetFirstAlternative().StartsWith(text,
																 StringComparison.
																		 CurrentCultureIgnoreCase))
				{
					show.Add(option);
				}
			}
			foreach (Option option in _allOptions.Options)
			{
				//todo: make this prefferd script(s) savvy
				if (option.Abbreviation.GetFirstAlternative().StartsWith(text,
																		 StringComparison.
																				 CurrentCultureIgnoreCase))
				{
					if (!show.Contains(option))
					{
						show.Add(option);
					}
				}
			}
			//  if(show.Count == 0)
			{
				foreach (Option option in _allOptions.Options)
				{
					//todo: make this prefferd script(s) savvy
					if (MatchWholeWordNoCase.IsMatch(option.Description.GetFirstAlternative()))
					{
						if (!show.Contains(option))
						{
							show.Add(option);
						}
					}
				}
			}
#if !DEBUG
			}
			catch (Exception e)
			{
				//not worth crashing over some regex problem
			}
#endif
			return show;
		}

		#endregion
	}
}