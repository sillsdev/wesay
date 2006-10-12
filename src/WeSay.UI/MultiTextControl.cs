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
		private IList<WritingSystem> _writingSystems;
		private MultiText _multiText;
		private List<WeSayTextBox> _textBoxes;
	   public new event EventHandler TextChanged;
		public new event KeyEventHandler KeyDown;

		public MultiTextControl()
		{
			this.components = new System.ComponentModel.Container();
			InitializeComponent();
			_textBoxes = new List<WeSayTextBox>();
			_multiText = new WeSay.Language.MultiText();
//            this.SuspendLayout();
//            this.BackColor = System.Drawing.Color.Green;
//            this._vbox.BackColor = System.Drawing.Color.White;
//            this._vbox.Dock = DockStyle.Fill;
//            this.ResumeLayout(false);
			_vbox.Name = "vbox of anonymous multitext";
		 //   this.SetStyle(ControlStyles.Selectable, false);
		}
		public MultiTextControl(IList<WritingSystem> writingSystems, MultiText text, string nameForTesting):this()
		{
			_vbox.Name = this.Name + "-vbox";
			this.Name = nameForTesting+"-mtc";
			_writingSystems = writingSystems;
			MultiText = text;
		}
		public MultiTextControl(IList<WritingSystem> writingSystems, MultiText text)
			: this(writingSystems, text, "Unknown")
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
			this.SuspendLayout();
			if (_vbox.Count > 0)
			{
				_vbox.Clear();
			}
			this.Height = 0;
			foreach (WritingSystem writingSystem in WritingSystems)
			{
				string writingSystemId =writingSystem.Id;
				const int initialPanelWidth = 200;
				WeSayTextBox box = new WeSayTextBox(writingSystem);
				_textBoxes.Add(box);
				this.components.Add(box);//so it will get disposed of when we are
				box.Name = this.Name.Replace("-mtc","") + "_" + writingSystemId; //for automated tests to find this particular guy
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
			this.ResumeLayout(false);

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
		public IList<WritingSystem> WritingSystems
		{
			get { return _writingSystems; }
			set
			{
				_writingSystems = value;
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
