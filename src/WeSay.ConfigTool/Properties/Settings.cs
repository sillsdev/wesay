using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using SIL.Reporting;

namespace WeSay.ConfigTool.Properties
{
	internal sealed partial class Settings
	{
		private void Initialize()
		{
			if (MruConfigFilePaths == null)
			{
				MruConfigFilePaths = new MruProjects();
			}
			if (Reporting == null)
			{
				Reporting = new ReportingSettings();
			}
		}

		public Settings()
		{
			Initialize();
		}

		public Settings(string settingsKey): base(settingsKey)
		{
			Initialize();
		}

		public Settings(IComponent owner): base(owner)
		{
			Initialize();
		}

		public Settings(IComponent owner, string settingsKey): base(owner, settingsKey)
		{
			Initialize();
		}

		public override void Upgrade()
		{
			string lastConfigFilePath = null;
			try
			{
				lastConfigFilePath = (string) GetPreviousVersion("LastConfigFilePath");
			}
			catch
			{
				// mono does not support GetPreviousVersion as of 23-Sep-2008
				// it doesn't matter if this fails
			}

			base.Upgrade(); // bring forward our MruConfigFilePaths and any other properties
			if (MruConfigFilePaths == null)
			{
				MruConfigFilePaths = new MruProjects();
			}
			if (Reporting == null)
			{
				Reporting = new ReportingSettings();
			}
			if (!string.IsNullOrEmpty(lastConfigFilePath))
			{
				MruConfigFilePaths.AddNewPath(lastConfigFilePath);
			}
		}

		// need to have this declaration or else we cannot upgrade by using GetPreviousVersion (it doesn't know whether
		// it is user scoped or application scoped)
		[UserScopedSetting]
		[DebuggerNonUserCode]
		[DefaultSettingValue("")]
		[Obsolete("Please use MruConfigFilePaths instead")]
		[NoSettingsVersionUpgrade]
		public string LastConfigFilePath
		{
			get
			{
				throw new NotSupportedException(
						"LastConfigFilePath is obsolete please use MruConfigFilePaths instead");
			}
			set
			{
				throw new NotSupportedException(
						"LastConfigFilePath is obsolete please use MruConfigFilePaths instead");
			}
		}
	}
}