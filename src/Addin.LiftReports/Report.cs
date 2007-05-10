using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace LiftReports
{
	public partial class Report : UserControl
	{
		private string _pathToLIFT;
		private const int kSizeOfYAxisLabels = 92;

		public Report()
		{
			InitializeComponent();
		}

		public string PathToLift
		{
			set
			{
				_pathToLIFT = value;
			}
		}

		private void _overviewChart_Load(object sender, EventArgs e)
		{
			_overviewChart.GraphControl.GraphMarginLeft = kSizeOfYAxisLabels;
			_overviewChart.GraphControl.BarOrientation = GraphComponents.Orientation.Horizontal;
			_overviewChart.Title = "# of entries containing these items.";
			_overviewChart.PathToXmlDocument = _pathToLIFT;
			_overviewChart.AddBar("Note", "//entry/note");
			 _overviewChart.AddBar("Relations", "//entry/relation");
		   _overviewChart.AddBar("Pronunciation", "//entry/pronunciation");
			_overviewChart.AddBar("Sense (multiple)", "//entry[count(sense)>1]");
			_overviewChart.AddBar("Sense", "//entry/sense");
			_overviewChart.AddBar("Citation Form", "//entry/citation");
			_overviewChart.AddBar("All Entries", "//entry");

		}

		private void _senseChart_Load(object sender, EventArgs e)
		{
			_senseChart.GraphControl.GraphMarginLeft = kSizeOfYAxisLabels;
			_senseChart.GraphControl.BarOrientation = GraphComponents.Orientation.Horizontal;
			_senseChart.Title = "# of senses containing these items";
			_senseChart.PathToXmlDocument = _pathToLIFT;
			 _senseChart.AddBar("Example", "//example");
			_senseChart.AddBar("Definition", "//definition");
			_senseChart.AddBar("POS", "//grammatical-info");
			_senseChart.AddBar("Gloss (multiple)", "//sense[count(gloss)>1]");
		   _senseChart.AddBar("Gloss", "//sense[count(gloss)]");
			_senseChart.AddBar("All Senses", "//sense");
		}


		private void Report_Load(object sender, EventArgs e)
		{
			if (this.DesignMode)
				return;

			XmlDocument doc = new XmlDocument();
			doc.Load(_pathToLIFT);

			_countLabel.Text = String.Format("This file contains {0} entries.", GetCount(doc, "//entry"));
		}

		private int GetCount(XmlDocument doc, string xpath)
		{
			XmlNodeList l = doc.SelectNodes(xpath);
			if (l == null)
				return 0;
			else
				return l.Count;
		}
	}
}
