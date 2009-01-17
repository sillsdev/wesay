using System;
using System.Collections.Generic;
using System.Text;
using WeSay.Project;

namespace WeSay.LexicalTools.AddPictures
{
	public class AddPicturesConfig : TaskConfigurationBase, ITaskConfiguration
	{
		private string _indexFileName;

		public AddPicturesConfig(string xml) : base(xml)
		{

			IndexFileName = GetStringFromConfigNode("indexFileName");
		}

		public string IndexFileName
		{
			get { return _indexFileName; }
			set { _indexFileName = value;}
		}

		protected override IEnumerable<KeyValuePair<string, string>> ValuesToSave
		{
			get { throw new System.NotImplementedException(); }
		}

		public string Label
		{
			get { return "Add Pictures"; }
		}

		public string LongLabel
		{
			get { return Label; }
		}

		public string Description
		{
			get { return Label; }
		}

		public string RemainingCountText
		{
			get { return ""; }
		}

		public string ReferenceCountText
		{
			get { return ""; }
		}

		public bool IsPinned
		{
			get { return false; }
		}
	}
}
