using System.Collections.Generic;

namespace WeSay.Project
{
	//we have this, rather than relying on the standard .ent settings class, because we don't know ahead of time
	//what all the settings will be, we can't put them in code, which seems to be the way it is designed to work.
	public class UserSettingsRepository
	{
		public UserSettingsForTask FindSettingsByTaskId(string id)
		{
			var d = new UserSettingsForTask();
			d.Add("one", "1");
			return d;
		}
	}

	public class UserSettingsForTask
	{
		private Dictionary<string, string> _settings = new Dictionary<string, string>();

		public void Add(string key, string value)
		{
			_settings.Add(key,value);
		}
		public string Get(string key, string defaultValue)
		{
			string v;
			if( _settings.TryGetValue(key, out v))
				return v;
			return defaultValue;
		}
	}
}