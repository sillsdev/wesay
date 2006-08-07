using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using WeSay.Language;

namespace WeSay.LexicalModel
{
	public class LexSense
	{
		private MultiText _gloss;
		private BindingList<LexExampleSentence> _exampleSentences;

		  private event PropertyChangedEventHandler _propertyChangedHandler;

		/// <summary>
		///
		/// </summary>
		/// <param name="propertyChangedHandler">at this time, we are not taking note of
		/// changes at the sense level, only at the entry level.  So this is the owning
		/// lexical entry's change event.</param>
		public LexSense(PropertyChangedEventHandler propertyChangedHandler)
		{
			_propertyChangedHandler = propertyChangedHandler;
			_gloss = new MultiText(propertyChangedHandler);
			_exampleSentences = new BindingList<LexExampleSentence>();

			//nb: order of these two is important.  Touching adding new actually triggers ListChanged!
			_exampleSentences.AddingNew += new AddingNewEventHandler(OnExampleSentences_AddingNew);
			_exampleSentences.ListChanged += new ListChangedEventHandler(OnExampleSentences_ListChanged);

		}

		void OnExampleSentences_ListChanged(object sender, ListChangedEventArgs e)
		{
			NotifyPropertyChanged("Example Sentences");
		}

		void OnExampleSentences_AddingNew(object sender, AddingNewEventArgs e)
		{
			e.NewObject = new LexExampleSentence(this._propertyChangedHandler);
		}

		//public LexSense()
		//{
		//}

		public MultiText Gloss
		{
			get { return _gloss; }
		  //  set { _gloss = value; }
		}

		public BindingList<LexExampleSentence> ExampleSentences
		{
			get { return _exampleSentences; }
		   // set { _exampleSentences = value; }
		}

		private void NotifyPropertyChanged(string info)
		{
			if (_propertyChangedHandler != null)
			{
				_propertyChangedHandler(this, new PropertyChangedEventArgs(info));
			}
		}

		///// <summary>
		///// called by owning LexEntry
		///// </summary>
		///// <param name="propertyChangedEventHandler"></param>
		//internal void WireUp(PropertyChangedEventHandler propertyChangedEventHandler)
		//{
		//    this._propertyChangedHandler = propertyChangedEventHandler;
		//}
	}
}
