using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using WeSay.Language;

namespace WeSay.LexicalModel
{
	public class LexExampleSentence
	{
		private MultiText _sentence;
		private MultiText _translation;

		private event PropertyChangedEventHandler _propertyChangedHandler;


		/// <summary>
		///
		/// </summary>
		/// <param name="propertyChangedHandler">at this time, we are not taking note of
		/// changes at the example level, only at the entry level.  So this is the owning
		/// lexical entry's change event.</param>
		public LexExampleSentence(PropertyChangedEventHandler propertyChangedHandler)
		{
			_propertyChangedHandler = propertyChangedHandler;
			_sentence = new MultiText(_propertyChangedHandler);
			_translation = new MultiText(_propertyChangedHandler);
		}

		public MultiText Sentence
		{
			get { return _sentence; }
			set
			{
				_sentence = value;
				NotifyPropertyChanged("example sentence");
			}
		}

		public MultiText Translation
		{
			get { return _translation; }
			set
			{
				_translation = value;
				NotifyPropertyChanged("translation of example sentence");
			}
		}

		private void NotifyPropertyChanged(string info)
		{
			if (_propertyChangedHandler != null)
			{
				_propertyChangedHandler(this, new PropertyChangedEventArgs(info));
			}
		}
	}
}
