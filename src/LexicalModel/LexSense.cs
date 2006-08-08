using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using WeSay.Language;

namespace WeSay.LexicalModel
{
	public class LexSense : INotifyPropertyChanged
	{
		private MultiText _gloss;
		private BindingList<LexExampleSentence> _exampleSentences;
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		///
		/// </summary>
		/// <param name="propertyChangedHandler">at this time, we are not taking note of
		/// changes at the sense level, only at the entry level.  So this is the owning
		/// lexical entry's change event.</param>
		public LexSense()
		{
			_gloss = new MultiText();
			_gloss.PropertyChanged+=new PropertyChangedEventHandler(OnChildObjectPropertyChanged);
			_exampleSentences = new BindingList<LexExampleSentence>();

			//nb: order of these two is important.  Touching adding new actually triggers ListChanged!
			_exampleSentences.AddingNew += new AddingNewEventHandler(OnExampleSentences_AddingNew);
			_exampleSentences.ListChanged += new ListChangedEventHandler(OnExampleSentences_ListChanged);

		}

		void OnChildObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			NotifyPropertyChanged(e.PropertyName);
		}

		void OnExampleSentences_ListChanged(object sender, ListChangedEventArgs e)
		{
			NotifyPropertyChanged("ExampleSentences");
		}

		void OnExampleSentences_AddingNew(object sender, AddingNewEventArgs e)
		{
			e.NewObject = new LexExampleSentence();
			((LexExampleSentence)e.NewObject).PropertyChanged += new PropertyChangedEventHandler(OnChildObjectPropertyChanged);
		}


		public MultiText Gloss
		{
			get { return _gloss; }
		}

		public BindingList<LexExampleSentence> ExampleSentences
		{
			get { return _exampleSentences; }
		}

		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
