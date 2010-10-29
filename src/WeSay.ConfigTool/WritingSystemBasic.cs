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
		private WritingSystem _oldWritingSystemForMono;	//This is part of a workaround for Mono on 4-Aug-2009 TA
														//Mono does not returns e.Old=e.Current for PropertyChanges

		private WritingSystem _writingSystem;
		private WritingSystemCollection _writingSystemCollection;

		public delegate void WritingSystemIdChanged(WritingSystem ws, string propertyName, string oldValue);
		public event WritingSystemIdChanged WritingSystemIdChangedEvent;
		public event EventHandler IsAudioChanged;

		//        public class PropertyChangingEventArgs : PropertyChangedEventArgs
		//        {
		//            public bool Cancel = false;
		//
		//            public PropertyChangingEventArgs(string propertyName)
		//                : base(propertyName)
		//            {
		//            }
		//        }

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
				_oldWritingSystemForMono = new WritingSystem
				{
					ISO = _writingSystem.ISO,
					Region = _writingSystem.Region,
					Variant = _writingSystem.Variant,
					Script = _writingSystem.Script
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
			e = GetNewEventArgsBecauseOfMonoBug(e);
			somethingelse(e.ChangedItem.PropertyDescriptor.Name, e.ChangedItem.Value.ToString(), e.OldValue.ToString());
		}

		private void somethingelse(string propertyName, string newValue, string oldValue)
		{
			if(newValue == oldValue)
			{
				return;
			}

			bool requiresAction = (propertyName == "ISO")      ||
						  (propertyName == "Region")   ||
						  (propertyName == "Variant")  ||
						  (propertyName == "Script")   ||
						  (propertyName == "IsAudio");

				Logger.WriteConciseHistoricalEvent(
					StringCatalog.Get("Modified {0} of Writing System {1}",
									  "Checkin Description in WeSay Config Tool used when you edit a writing system."),
					propertyName, _writingSystem.Id);

				if (!requiresAction){return; }

				Console.WriteLine("Old Id was {0}, new ID is: {1}", oldValue, newValue);


				if (_writingSystemCollection.ContainsKey(_writingSystem.Id))
				{
					ErrorReport.NotifyUserOfProblem(String.Format(
						"Sorry, there is already a Writing System with the ID {0}.", _writingSystem.Id));
					_writingSystem.ISO = _oldWritingSystemForMono.ISO;
					_writingSystem.Region = _oldWritingSystemForMono.Region;
					_writingSystem.Variant = _oldWritingSystemForMono.Variant;
					_writingSystem.Script = _oldWritingSystemForMono.Script;
				}
				else if (propertyName == "ISO")
				{
					string iso = newValue;

					if (iso != null && iso.Contains(" "))
					{
						ErrorReport.NotifyUserOfProblem(
							"Sorry, the writingsystem Id should conform to ISO 639-3 and may not contain spaces");
						_writingSystem.ISO = oldValue.ToString();
					}

					if (TriedToChangeKnownLanguageId(oldValue.ToString(), "en", "English") ||
						TriedToChangeKnownLanguageId(oldValue.ToString(), "fr", "French") ||
						TriedToChangeKnownLanguageId(oldValue.ToString(), "id", "Indonesian") ||
						TriedToChangeKnownLanguageId(oldValue.ToString(), "es", "Spanish") ||
						TriedToChangeKnownLanguageId(oldValue.ToString(), "tpi", "Tok Pisin") ||
						TriedToChangeKnownLanguageId(oldValue.ToString(), "th", "Thai"))
					{
						_writingSystem.ISO = _oldWritingSystemForMono.ISO;
					}
				}
				if(_writingSystem.Id != _oldWritingSystemForMono.Id)
				{
					if (WritingSystemIdChangedEvent != null)
					{
						WritingSystemIdChangedEvent.Invoke(_writingSystem, propertyName, oldValue);
					}
				}

			//nb: don't do this ealier, since some of this code revers what the user tried to change
			//(setting it earlier let to http://www.wesay.org/issues/browse/WS-15031)
			_oldWritingSystemForMono = new WritingSystem
			{
				ISO = _writingSystem.ISO,
				Region = _writingSystem.Region,
				Variant = _writingSystem.Variant,
				Script = _writingSystem.Script
			};

			if (propertyName == "IsAudio")
			{
				if (IsAudioChanged != null)
				{
					IsAudioChanged.Invoke(this, null);
				}
				if (WritingSystemIdChangedEvent != null)
				{
					WritingSystemIdChangedEvent.Invoke(_writingSystem, "ISO", oldValue);
					WritingSystemIdChangedEvent.Invoke(_writingSystem, "Script", oldValue);
					WritingSystemIdChangedEvent.Invoke(_writingSystem, "Region", oldValue);
					WritingSystemIdChangedEvent.Invoke(_writingSystem, "Variant", oldValue);
				}
			}
		}

		private PropertyValueChangedEventArgs GetNewEventArgsBecauseOfMonoBug(PropertyValueChangedEventArgs e)
		{
			if (e.ChangedItem.PropertyDescriptor.Name == "ISO")
			{
				e = new PropertyValueChangedEventArgs(e.ChangedItem, _oldWritingSystemForMono.ISO);
			}
			else if (e.ChangedItem.PropertyDescriptor.Name == "Region")
			{
				e = new PropertyValueChangedEventArgs(e.ChangedItem, _oldWritingSystemForMono.Region);
			}
			else if (e.ChangedItem.PropertyDescriptor.Name == "Variant")
			{
				e = new PropertyValueChangedEventArgs(e.ChangedItem, _oldWritingSystemForMono.Variant);
			}
			else if (e.ChangedItem.PropertyDescriptor.Name == "Script")
			{
				e = new PropertyValueChangedEventArgs(e.ChangedItem, _oldWritingSystemForMono.Script);
			}
			else if (e.ChangedItem.PropertyDescriptor.Name == "IsAudio")
			{
				e = new PropertyValueChangedEventArgs(e.ChangedItem, _oldWritingSystemForMono.IsAudio);
			}
			return e;
		}
	}
}