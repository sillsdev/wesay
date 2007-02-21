using System.Configuration;

namespace WeSay.App.Properties {


	// This class allows you to handle specific events on the settings class:
	//  The SettingChanging event is raised before a setting's value is changed.
	//  The PropertyChanged event is raised after a setting's value is changed.
	//  The SettingsLoaded event is raised after the setting values are loaded.
	//  The SettingsSaving event is raised before the setting values are saved.

	//problems with user.config: http://blogs.msdn.com/rprabhu/articles/433979.aspx
	//registry sample: http://www.sellsbrothers.com/writing/default.aspx?content=dotnet2customsettingsprovider.htm
	[SettingsProvider(typeof(Microsoft.Samples.Windows.Forms.RegistrySettingsProvider.RegistrySettingsProvider ))]
	internal sealed partial class Settings
	{

		public Settings() {
			// // To add event handlers for saving and changing settings, uncomment the lines below:
			//
			// this.SettingChanging += this.SettingChangingEventHandler;
			//
			// this.SettingsSaving += this.SettingsSavingEventHandler;
			//
			PropertyChanged += PropertyChangedEventHandler;
		}

		private void PropertyChangedEventHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Save();
		}

		//private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
		//    // Add code to handle the SettingChangingEvent event here.
		//}

		//private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
		//    // Add code to handle the SettingsSaving event here.
		//}
	}
}
