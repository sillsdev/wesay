using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.LexicalTools.Properties;
using WeSay.Project;


/* todo
 *
 * handle:
 *     <filter class="WeSay.LexicalTools.MissingItemFilter" assembly="LexicalTools">
		<viewTemplate ref="Default View Template" />
		<field>POS</field>
	  </filter>
 *
 * Deal with parameters that are just strings... do we adjust the paramters to match the
 * config file names?  Stoop to positionalParameters again? What would be nice is attributes on the parameters that we match.
*/

namespace WeSay.LexicalTools
{
	public class DictionaryTask: TaskBase
	{
		private DictionaryControl _dictionaryControl;
		private readonly ViewTemplate _viewTemplate;
		private UserSettingsForTask _userSettings;
		private static readonly string kTaskLabel = "~Dictionary";
		private static readonly string kTaskLongLabel = "~Dictionary Browse && Edit";

		public DictionaryTask(LexEntryRepository lexEntryRepository, ViewTemplate viewTemplate)
				: base(
						kTaskLabel,
						kTaskLongLabel,
						string.Empty,
						string.Empty,
						string.Empty,
						true,
						lexEntryRepository)
		{
#if JustForCodeScanner
			StringCatalog.Get(kTaskLabel,
							  "The label for the task that lets you see all entries, search for entries, and edit various fields.  We don't like the English name very much, so feel free to call this something very different in the language you are translating to.");
			StringCatalog.Get(kTaskLongLabel,
							  "The long label for the task that lets you see all entries, search for entries, and edit various fields.  We don't like the English name very much, so feel free to call this something very different in the language you are translating to.");
#endif
			if (viewTemplate == null)
			{
				throw new ArgumentNullException("viewTemplate");
			}
			_viewTemplate = viewTemplate;
		  //  _userSettings = userSettings;
		}

		//this wants to be in the constructor, but it's waiting until we get rid of this bizarre picocontainer usage that makes it impossible to add arbitrary stuff without modifying everyone's config file
		public UserSettingsForTask UserSettings
		{
			set
			{
				_userSettings = value;
			}
		}

		public override void Activate()
		{
			try
			{
				base.Activate();
				_dictionaryControl = new DictionaryControl(LexEntryRepository, ViewTemplate);
//   Debug.Assert(_userSettings.Get("one", "0") == "1");

				if(_userSettings !=null && _userSettings.Get("lastUrl", null)!=null)
					_dictionaryControl.GoToEntry(_userSettings.Get("lastUrl", null));
			}
			catch (ConfigurationException)
			{
				IsActive = false;
				throw;
			}
		}

		public override void Deactivate()
		{
			base.Deactivate();
			_dictionaryControl.Dispose();
			_dictionaryControl = null;
		}

		public override void GoToUrl(string url)
		{
			_dictionaryControl.GoToEntry(GetEntryFromUrl(url));
		}

		private static string GetEntryFromUrl(string url)
		{
			return url;
		}

		/// <summary>
		/// The entry detail control associated with this task
		/// </summary>
		/// <remarks>Non null only when task is activated</remarks>
		public override Control Control
		{
			get { return _dictionaryControl; }
		}

		public override string Description
		{
			get
			{
				return
						String.Format(
								StringCatalog.Get("~See all {0} {1} words.",
												  "The description of the 'Dictionary' task.  In place of the {0} will be the number of words in the dictionary.  In place of the {1} will be the name of the project."),
								ComputeCount(true),
								BasilProject.Project.Name);
			}
		}

		public ViewTemplate ViewTemplate
		{
			get { return _viewTemplate; }
		}

		protected override int ComputeCount(bool returnResultEvenIfExpensive)
		{
			return LexEntryRepository.CountAllItems();
		}

		protected override int ComputeReferenceCount()
		{
			return CountNotRelevant;
		}

		public override ButtonStyle DashboardButtonStyle
		{
			get { return ButtonStyle.IconFixedWidth; }
		}

		public override Image DashboardButtonImage
		{
			get { return Resources.blueDictionary; }
		}
	}
}