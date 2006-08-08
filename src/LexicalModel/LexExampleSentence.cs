using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using WeSay.Language;

namespace WeSay.LexicalModel
{
	public class LexExampleSentence : INotifyPropertyChanged
	{
		private MultiText _sentence;
		private MultiText _translation;

		public event PropertyChangedEventHandler PropertyChanged;


		/// <summary>
		///
		/// </summary>
		/// <param name="propertyChangedHandler">at this time, we are not taking note of
		/// changes at the example level, only at the entry level.  So this is the owning
		/// lexical entry's change event.</param>
		public LexExampleSentence()
		{
			_sentence = new MultiText();
			_sentence.PropertyChanged += new PropertyChangedEventHandler(OnChildObjectPropertyChanged);
			_translation = new MultiText();
			_translation.PropertyChanged += new PropertyChangedEventHandler(OnChildObjectPropertyChanged);
		}

		void OnChildObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			NotifyPropertyChanged(e.PropertyName);
		}

		public MultiText Sentence
		{
			get { return _sentence; }
			set
			{
				_sentence = value;
				NotifyPropertyChanged("ExampleSentence");
			}
		}

		public MultiText Translation
		{
			get { return _translation; }
			set
			{
				_translation = value;
				NotifyPropertyChanged("ExampleSentenceTranslation");
			}
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
