using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.i8n;

namespace WeSay.Project
{
   public class CheckinDescriptionBuilder: Palaso.Reporting.ILogger
	{
		private Dictionary<string, int> _actions;


		public CheckinDescriptionBuilder()
		{
			Clear();
		}

	   /// <summary>
	   /// We don't store the same event over and over, just the count, which makes for a more concise description
	   /// </summary>
	   /// <param name="description"></param>
		void AddEvent(string description)
		{
			if (_actions.ContainsKey(description))
			{
				_actions[description]++;
			}
			else
			{
				_actions.Add(description, 1);
			}
		}

		public string GetDescription()
		{
#if ON_The_Shelf          string appName = Application.ProductName;

			var builder = new StringBuilder();

			bool first = true;
			foreach (var pair in _actions)
			{
				if (!first)
				{
					builder.Append(", ");
				}
				first = false;
				builder.Append(pair.Key);
				if(pair.Value > 1)
				{
					builder.Append(string.Format("({0})", pair.Value));
				}
			}

			var x = builder.ToString();
			if(string.IsNullOrEmpty(x))
			{
				return StringCatalog.Get(appName+": no logged activity");
			}
			return appName+": " + x;
#endif
			return "[appName] auto";
		}

		public void Clear()
		{
			_actions = new Dictionary<string, int>();
		}

	   public void WriteConciseHistoricalEvent(string message, params object[] args)
	   {
		   AddEvent(string.Format(message,args));
	   }
	}
}