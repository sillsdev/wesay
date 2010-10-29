using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Palaso.Code;
using Palaso.Reporting;
using WeSay.LexicalModel.Foundation;
using Palaso.i18n;

namespace WeSay.ConfigTool
{
	public partial class WritingSystemBasic: UserControl
	{
		private WritingSystem _oldWritingSystemValues;

		private WritingSystem _writingSystem;
		private WritingSystemCollection _writingSystemCollection;

		public delegate void WritingSystemIdChanged(WritingSystem newWritingSystemValue, WritingSystem oldWritingSystemValues);
		public event WritingSystemIdChanged WritingSystemIdChangedEvent;
		public event EventHandler IsAudioChanged;

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
				_oldWritingSystemValues = new WritingSystem
				{
					ISO = _writingSystem.ISO,
					Region = _writingSystem.Region,
					Variant = _writingSystem.Variant,
					Script = _writingSystem.Script,
					IsAudio = _writingSystem.IsAudio
				};
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


		private void OnPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			string propertyName = e.ChangedItem.PropertyDescriptor.Name;
			bool requiresAction = (propertyName == "ISO") ||
								  (propertyName == "Region") ||
								  (propertyName == "Variant") ||
								  (propertyName == "Script") ||
								  (propertyName == "IsAudio");
			if (!requiresAction) { return; }


			Logger.WriteConciseHistoricalEvent(
				StringCatalog.Get("Modified {0} of Writing System {1}",
								  "Checkin Description in WeSay Config Tool used when you edit a writing system."),
				propertyName, _writingSystem.Id);

			IdChanged();
		}

		public void IdChanged()
		{
			if(_writingSystem.Id == _oldWritingSystemValues.Id)
			{
				return;
			}

			Console.WriteLine("Old Id was {0}, new ID is: {1}", _oldWritingSystemValues.Id, _writingSystem.Id);

			if (_writingSystemCollection.ContainsKey(_writingSystem.Id))
			{
				ErrorReport.NotifyUserOfProblem(String.Format(
					"Sorry, there is already a Writing System with the ID {0}.", _writingSystem.Id));
				RevertWritingSystemToOldValues();
			}
			else if (_writingSystem.ISO != null && _writingSystem.ISO.Contains(" "))
			{
				ErrorReport.NotifyUserOfProblem(
					"Sorry, the writingsystem Id should conform to ISO 639-3 and may not contain spaces");
				RevertWritingSystemToOldValues();
			}
			else if (TriedToChangeKnownLanguageId(_oldWritingSystemValues.Id, "en", "English") ||
				TriedToChangeKnownLanguageId(_oldWritingSystemValues.Id, "fr", "French") ||
				TriedToChangeKnownLanguageId(_oldWritingSystemValues.Id, "id", "Indonesian") ||
				TriedToChangeKnownLanguageId(_oldWritingSystemValues.Id, "es", "Spanish") ||
				TriedToChangeKnownLanguageId(_oldWritingSystemValues.Id, "tpi", "Tok Pisin") ||
				TriedToChangeKnownLanguageId(_oldWritingSystemValues.Id, "th", "Thai"))
			{
				RevertWritingSystemToOldValues();
			}
			else if(_writingSystem.Id != _oldWritingSystemValues.Id)
			{
				if (WritingSystemIdChangedEvent != null)
				{
					WritingSystemIdChangedEvent.Invoke(_writingSystem, _oldWritingSystemValues);
				}
			}

			if (_writingSystem.IsAudio != _oldWritingSystemValues.IsAudio)
			{
				if (IsAudioChanged != null)
				{
					IsAudioChanged.Invoke(this, null);
				}
			}

			//nb: don't do this ealier, since some of this code revers what the user tried to change
			//(setting it earlier let to http://www.wesay.org/issues/browse/WS-15031)
			_oldWritingSystemValues = new WritingSystem
			{
				ISO = _writingSystem.ISO,
				Region = _writingSystem.Region,
				Variant = _writingSystem.Variant,
				Script = _writingSystem.Script
			};
		}

		private void RevertWritingSystemToOldValues()
		{
			_writingSystem.IsAudio = false;
			_writingSystem.ISO = _oldWritingSystemValues.ISO;
			_writingSystem.Region = _oldWritingSystemValues.Region;
			_writingSystem.Variant = _oldWritingSystemValues.Variant;
			_writingSystem.Script = _oldWritingSystemValues.Script;
		}
	}
}