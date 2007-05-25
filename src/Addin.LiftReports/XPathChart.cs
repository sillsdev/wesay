using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using GraphComponents;

namespace Addin.LiftReports
{
	public partial class XPathChart : UserControl
	{
		private string _pathToXmlDocument;
		private XmlDocument _doc;

		public XPathChart()
		{
			InitializeComponent();
			_graph.ValueFormat = "{0:#######}";
			_graph.GraduationsX = 0;
		}

		public StackedBarGraph GraphControl
		{
			get
			{
				return _graph;
			}
		}

		public string PathToXmlDocument
		{
			get
			{
				return _pathToXmlDocument;
			}
			set
			{
				_pathToXmlDocument = value;
				if (String.IsNullOrEmpty(_pathToXmlDocument))
				{
				   _doc = null;
				   return;
				}
				_doc = new XmlDocument();
				_doc.Load(value);
			}
		}

		public string Title
		{
			set
			{
				_label.Text = value;
			}
		}

		/// <summary>
		/// Used from html templates to make an inline image
		/// </summary>
		/// <param name="orientation"></param>
		/// <returns></returns>
		public string MakeBitmapFile(string orientation)
		{
			this._graph.BarOrientation = (GraphComponents.Orientation)Enum.Parse(typeof(GraphComponents.Orientation), orientation);

			using (Graphics g = this.CreateGraphics())
			{
				using (Bitmap bm = new Bitmap(this.Width, this.Height, g))
				{
					ReverseBarOrder();
					this.DrawToBitmap(bm, new Rectangle(0, 0, this.Width, this.Height));
					this._graph.Bars.Clear();
					string path =Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".png");
					//System.Drawing.Imaging.
					//System.Drawing.Bitmap b= new png
					bm.Save(path, System.Drawing.Imaging.ImageFormat.Png);
					return "file://"+path;
				}
			}
		}

		/// <summary>
		/// The lib we are using draws the bars in reverse order, so this is handy
		/// </summary>
		private void ReverseBarOrder()
		{
			ArrayList l = new ArrayList();
			l.AddRange(_graph.Bars);
			_graph.Bars.Clear();
			l.Reverse();

			foreach (Bar bar in l)
			{
				_graph.Bars.Add(bar);
			}
		}

		public void AddBar(string label, string xpath)
		{
			if (_doc == null)
				return;

			int i = GetCount(xpath);
			if (i == 0)
			{
				return;
			}
			Bar bar = new Bar(label, i);
			_graph.Bars.Add(bar);
			if (i > _graph.MaximumValue)
			{
				_graph.MaximumValue = i;
			}

			int labelWidth = 15+ TextRenderer.MeasureText(label, _graph.Font).Width;
			if (labelWidth > _graph.GraphMarginLeft)
			{
				_graph.GraphMarginLeft = labelWidth;
			}
		}

		private int GetCount(string xpath)
		{
			XmlNodeList l= _doc.SelectNodes(xpath);
			if(l==null)
				return 0;
			else
				return l.Count;
		}

		private void XPathChart_Load(object sender, EventArgs e)
		{


		}

		private void XPathChart_Resize(object sender, EventArgs e)
		{
			_graph.Invalidate();
		}

		public override void Refresh()
		{
	 //doesn't work       this._label.Left = _graph.GraphMarginLeft;
			base.Refresh();
		}
	}
}
