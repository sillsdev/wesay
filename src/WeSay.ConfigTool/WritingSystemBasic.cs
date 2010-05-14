using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Palaso.Code;
using Palaso.Reporting;
using WeSay.LexicalModel.Foundation;
using Palaso.I8N;

namespace WeSay.ConfigTool
{
	public partial class WritingSystemBasic: UserControl
	{
		private WritingSystem _oldWritingSystemForMono;	//This is part of a workaround for Mono on 4-Aug-2009 TA
														//Mono does not returns e.Old=e.Current for PropertyChanges

		private WritingSystem _writingSystem;
		private WritingSystemCollection _writingSystemCollection;

		public event EventHandler WritingSystemIdChanged;
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
			if(e.ChangedItem.Value.ToString() == e.OldValue.ToString())
			{
				return;
			}
				Logger.WriteConciseHistoricalEvent(
					StringCatalog.Get("Modified {0} of Writing System {1}",
									  "Checkin Description in WeSay Config Tool used when you edit a writing system."),
					e.ChangedItem.PropertyDescriptor.Name, _writingSystem.Id);

				if (e.ChangedItem.PropertyDescriptor.Name == "IsAudio")
				{
					if (IsAudioChanged != null)
						IsAudioChanged.Invoke(this, null);
					return;
				}

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
				else{
					return;
				}

				Console.WriteLine("Old Id was {0}, new ID is: {1}", e.OldValue, e.ChangedItem.Value);


				if (_writingSystemCollection.ContainsKey(_writingSystem.Id))
				{
					ErrorReport.NotifyUserOfProblem(String.Format(
						"Sorry, there is already a Writing System with the ID {0}.", _writingSystem.Id));
					_writingSystem.ISO = _oldWritingSystemForMono.ISO;
					_writingSystem.Region = _oldWritingSystemForMono.Region;
					_writingSystem.Variant = _oldWritingSystemForMono.Variant;
					_writingSystem.Script = _oldWritingSystemForMono.Script;
				}
				else if (e.ChangedItem.PropertyDescriptor.Name == "ISO")
				{
					string iso = e.ChangedItem.Value as string;

					if (iso != null && iso.Contains(" "))
					{
						ErrorReport.NotifyUserOfProblem(
							"Sorry, the writingsystem Id should conform to ISO 639-3 and may not contain spaces");
						_writingSystem.ISO = e.OldValue.ToString();
					}

					if (TriedToChangeKnownLanguageId(e.OldValue.ToString(), "en", "English") ||
						TriedToChangeKnownLanguageId(e.OldValue.ToString(), "fr", "French") ||
						TriedToChangeKnownLanguageId(e.OldValue.ToString(), "id", "Indonesian") ||
						TriedToChangeKnownLanguageId(e.OldValue.ToString(), "es", "Spanish") ||
						TriedToChangeKnownLanguageId(e.OldValue.ToString(), "tpi", "Tok Pisin") ||
						TriedToChangeKnownLanguageId(e.OldValue.ToString(), "th", "Thai"))
					{
						_writingSystem.ISO = _oldWritingSystemForMono.ISO;
					}
				}
				if(_writingSystem.Id != _oldWritingSystemForMono.Id)
				{
					if (WritingSystemIdChanged != null)
					{
						WritingSystemIdChanged.Invoke(_writingSystem, e);
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
		}
	}
}