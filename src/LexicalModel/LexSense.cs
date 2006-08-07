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
		private System.Collections.Generic.List<LexExampleSentence> _exampleSentences;

		/// <summary>
		/// For INotifyPropertyChanged
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		public LexSense()
		{
			PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
			_gloss = new MultiText(PropertyChanged);
			_exampleSentences = new List<LexExampleSentence>();
		}

		void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			//if we added modified date to sense, we'd set it here.
		}

		public MultiText Gloss
		{
			get { return _gloss; }
		  //  set { _gloss = value; }
		}

		public List<LexExampleSentence> ExampleSentences
		{
			get { return _exampleSentences; }
		   // set { _exampleSentences = value; }
		}

		private void NotifyPropertyChanged(string info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}
	}
}
