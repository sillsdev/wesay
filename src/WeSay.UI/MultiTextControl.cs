using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WeSay.Language;

namespace WeSay.UI
{
	public partial class MultiTextControl : UserControl
	{
		private IList<String> _writingSystemIds;
		private MultiText _multiText;
		private List<WeSayTextBox> _textBoxes;
	   public new event EventHandler TextChanged;
		public new event KeyEventHandler KeyDown;

		public MultiTextControl()
		{
			_textBoxes = new List<WeSayTextBox>();
			_multiText = new WeSay.Language.MultiText();
		   InitializeComponent();
			this.BackColor = System.Drawing.Color.Green;
			this._vbox.BackColor = System.Drawing.Color.White;
			this._vbox.Dock = DockStyle.Fill;
		 //   this.SetStyle(ControlStyles.Selectable, false);
		}
		public MultiTextControl(IList<String> writingSystemIds, MultiText text, string nameForTesting):this()
		{
			this.Name = nameForTesting;
			_writingSystemIds = writingSystemIds;
			MultiText = text;
		}
		public MultiTextControl(IList<String> writingSystemIds, MultiText text):this(writingSystemIds,text,"Unknown")
		{
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<WeSayTextBox> TextBoxes
		{
			get
			{
				return _textBoxes;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public MultiText MultiText
		{
			get
			{
				return _multiText;
			}
			set
			{
				//REVIEW: Should this be *copying* the multitext?
				_multiText = value;
				BuildBoxes();
			}
		}

		private void BuildBoxes()
		{
			_vbox.Clear();
			this.Height = 0;
			foreach (string writingSystemId in WritingSystemIds)
			{
				const int initialPanelWidth = 200;
				WeSayTextBox box = new WeSayTextBox(BasilProject.Project.WritingSystems[writingSystemId]);
				_textBoxes.Add(box);
				box.Name = this.Name + "_" + writingSystemId; //for automated tests to find this particular guy
				box.Text = _multiText[writingSystemId];
				box.Location = new Point(30, 0);
				box.Width = initialPanelWidth - box.Left;
				box.Anchor = AnchorStyles.Left | AnchorStyles.Right |AnchorStyles.Top;
				//  box.BorderStyle = BorderStyle.FixedSingle;
				box.TextChanged += new EventHandler(OnTextOfSomeBoxChanged);
				box.KeyDown += new KeyEventHandler(OnKeyDownInSomeBox);

				Label label = new System.Windows.Forms.Label();
				label.Text = writingSystemId;
				label.ForeColor = System.Drawing.Color.LightGray;

				Graphics g = this.CreateGraphics();
				int descent = box.Font.FontFamily.GetCellDescent(box.Font.Style);
				int descentPixel = (int) (box.Font.Size * descent / box.Font.FontFamily.GetEmHeight(box.Font.Style));

				//todo: this only takes into account the textbox descent, not the label's!
				label.Location = new Point(0, (int) (box.Bottom -
													 ( g.MeasureString(label.Text, label.Font).Height + descentPixel )) );

				Panel p = new Panel();
				p.Controls.Add(box);
				p.Controls.Add(label);
				p.Size = new Size(initialPanelWidth,box.Height+0);

				_vbox.AddControlToBottom(p);
				this.Height += p.Height;
			}
		}

		void OnKeyDownInSomeBox(object sender, KeyEventArgs e)
		{
			if (this.KeyDown != null)
			{
				KeyDown.Invoke(sender, e);
			}
		}

		void OnTextOfSomeBoxChanged(object sender, EventArgs e)
		{
			if (this.TextChanged != null)
			{
				TextChanged.Invoke(sender, e);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IList<string> WritingSystemIds
		{
			get { return _writingSystemIds; }
			set
			{
				_writingSystemIds = value;
				BuildBoxes();
			}
		}

		public void ClearAllText()
		{
			foreach (WeSayTextBox box in _textBoxes)
			{
				box.Text = "";
			}
		}

		private void MultiTextControl_Enter(object sender, EventArgs e)
		{
			if (this.TextBoxes.Count > 0)
			{
				this.TextBoxes[0].Select();
			}
		}

	}
}
