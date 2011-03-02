using System;
using System.ComponentModel;
using System.Windows.Forms;
using Palaso.Reporting;
using WeSay.LexicalModel.Foundation;
using Palaso.i18n;

namespace WeSay.ConfigTool
{
	public partial class WritingSystemBasic: UserControl
	{
		struct WritingSystemInfo
		{
			public string ISO639;
			public string Region;
			public string Variant;
			public string Script;
			public string RFCTag;
		}
		private WritingSystemInfo _oldWritingSystemInfo;

		private WritingSystem _writingSystem;
		private WritingSystemCollection _writingSystemCollection;

		public event EventHandler WritingSystemIdChanged;

		/// <summary>
		/// called when the user wants to change the actual id of a ws, which has large reprocussions
		/// </summary>
		//  public event System.EventHandler IdChanging;
		public WritingSystemBasic()
		{
			InitializeComponent();
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WritingSystem WritingSystem
		{
			get { return _writingSystem; }
			set
			{
				_writingSystem = value;
				_oldWritingSystemInfo = new WritingSystemInfo();
				StoreOldWritingSystem();

				_writingSystemProperties.SelectedObject = _writingSystem;
				// _fontProperties.SelectedObjects = new object[] { _writingSystem, helper };
				Invalidate();
			}
		}

		/// <summary>
		/// for checking that ids are unique
		/// </summary>
		public WritingSystemCollection WritingSystemCollection
		{
			get { return _writingSystemCollection; }
			set { _writingSystemCollection = value; }
		}

		public ILogger Logger { get; set; }

		private static bool TriedToChangeKnownLanguageId(string oldId,
														 string officialId,
														 string language)
		{
			if (oldId == officialId)
			{
				ErrorReport.NotifyUserOfProblem(
						"Sorry, it's important to keep to international standard code for {0}, which is '{1}'.",
						language,
						officialId);
				return true;
			}
			return false;
		}

		private void ReloadOldWritingSystem()
		{
			_writingSystem.ISO = _oldWritingSystemInfo.ISO639;
			_writingSystem.Script = _oldWritingSystemInfo.Script;
			_writingSystem.Region = _oldWritingSystemInfo.Region;
			_writingSystem.Variant = _oldWritingSystemInfo.Variant;
			// Don't restore the RFCTag
		}

		private void StoreOldWritingSystem()
		{
			_oldWritingSystemInfo.ISO639 = _writingSystem.ISO;
			_oldWritingSystemInfo.Script = _writingSystem.Script;
			_oldWritingSystemInfo.Region = _writingSystem.Region;
			_oldWritingSystemInfo.Variant = _writingSystem.Variant;
			_oldWritingSystemInfo.RFCTag = _writingSystem.Id;
		}

		private void OnPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			if(e.ChangedItem.Value.ToString() == e.OldValue.ToString())
			{
				return;
			}
				Logger.WriteConciseHistoricalEvent(
					StringCatalog.Get("Modified {0} of Writing System {1}",
									  "Checkin Description in WeSay Config Tool used when you edit a writing system."),
					e.ChangedItem.PropertyDescriptor.Name, _writingSystem.Id);

				switch (e.ChangedItem.PropertyDescriptor.Name)
				{
					case "ISO":
						string iso = e.ChangedItem.Value as string;

						if (iso != null && iso.Contains(" "))
						{
							ErrorReport.NotifyUserOfProblem(
								"Sorry, the writingsystem Id should conform to ISO 639-3 and may not contain spaces");
							ReloadOldWritingSystem();
							return;
						}

						if (TriedToChangeKnownLanguageId(e.OldValue.ToString(), "en", "English") ||
							TriedToChangeKnownLanguageId(e.OldValue.ToString(), "fr", "French") ||
							TriedToChangeKnownLanguageId(e.OldValue.ToString(), "id", "Indonesian") ||
							TriedToChangeKnownLanguageId(e.OldValue.ToString(), "es", "Spanish") ||
							TriedToChangeKnownLanguageId(e.OldValue.ToString(), "tpi", "Tok Pisin") ||
							TriedToChangeKnownLanguageId(e.OldValue.ToString(), "th", "Thai"))
						{
							ReloadOldWritingSystem();
							return;
						}
						e = new PropertyValueChangedEventArgs(e.ChangedItem, _oldWritingSystemInfo.RFCTag);
						break;
					case "Region":
					case "Variant":
					case "Script":
					case "IsAudio":
						e = new PropertyValueChangedEventArgs(e.ChangedItem, _oldWritingSystemInfo.RFCTag);
						break;
					default:
						return;
				}

				if (_writingSystemCollection.ContainsKey(_writingSystem.Id))
				{
					ErrorReport.NotifyUserOfProblem(String.Format(
						"Sorry, there is already a Writing System with the ID {0}.", _writingSystem.Id));
					ReloadOldWritingSystem();
					return;
				}
				if(_writingSystem.Id != _oldWritingSystemInfo.RFCTag)
				{
					if (WritingSystemIdChanged != null)
					{
						var e2 = new PropertyValueChangedEventArgs(e.ChangedItem, _oldWritingSystemInfo.RFCTag);
						WritingSystemIdChanged.Invoke(_writingSystem, e2);
					}
				}

			//nb: don't do this ealier, since this code reverts what the user tried to change
			//(setting it earlier led to http://www.wesay.org/issues/browse/WS-15031)
			StoreOldWritingSystem();
		}
	}
}