using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Language;

namespace WeSay.UI
{
	public partial class MultiTextControl : Widget
	{
		private IList<WritingSystem> _writingSystems;
		private List<WeSayTextBox> _textBoxes;
		//public new event EventHandler TextChanged;
	   // public new event KeyEventHandler KeyDown;

		public MultiTextControl()
		{
			this.components = new System.ComponentModel.Container();
			InitializeComponent();
			_textBoxes = new List<WeSayTextBox>();
//            this.SuspendLayout();
//            this.BackColor = System.Drawing.Color.Green;
//            this._vbox.BackColor = System.Drawing.Color.White;
//            this._vbox.Dock = DockStyle.Fill;
//            this.ResumeLayout(false);
			_vbox.Name = "vbox of anonymous multitext";
		 //   this.SetStyle(ControlStyles.Selectable, false);
		}
		public MultiTextControl(IList<WritingSystem> writingSystems, MultiText multiTextToCopyFormsFrom, string nameForTesting):this()
		{
			_vbox.Name = Name + "-vbox";
			Name = nameForTesting+"-mtc";
			_writingSystems = writingSystems;
			BuildBoxes(multiTextToCopyFormsFrom);
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
				//we don't have a binding that would keep an internal multitext up to date.
				//This seems cleaner and sufficient, at the moment.
				MultiText mt = new MultiText();
				foreach (WeSayTextBox box in this.TextBoxes )
				{
					mt.SetAlternative(box.WritingSystem.Id, box.Text);
				}
				return mt;
			}
		}

		private void BuildBoxes(MultiText multiText)
		{
			SuspendLayout();
			if (_vbox.Count > 0)
			{
				_vbox.Clear();
			}
			Height = 0;
			const int initialPanelWidth = 200;
			foreach (WritingSystem writingSystem in WritingSystems)
			{
				WeSayTextBox box = AddTextBox(initialPanelWidth, writingSystem, multiText);

				Label label = AddWritingSystemLabel(box);

				//Graphics g = CreateGraphics();
				//int descent = box.Font.FontFamily.GetCellDescent(box.Font.Style);
				//int descentPixel = (int) (box.Font.Size * descent / box.Font.FontFamily.GetEmHeight(box.Font.Style));

				this.components.Add(box);//so it will get disposed of when we are

				Panel p = new Panel();
				p.Controls.Add(box);
				p.Controls.Add(label);
				p.Size = new Size(initialPanelWidth,box.Height+0);

#if notyet
				FlagButton flagButton = MakeFlagButton(p.Size);
				p.Controls.Add(flagButton);
				this.components.Add(flagButton);//so it will get disposed of when we are
#endif
				_vbox.AddControlToBottom(p);
				Height += p.Height;
			}
			ResumeLayout(false);
		}

		private Label AddWritingSystemLabel(WeSayTextBox box)
		{
			Label label = new System.Windows.Forms.Label();
			label.Text = box.WritingSystem.Id;
			label.ForeColor = System.Drawing.Color.LightGray;


			Graphics g = CreateGraphics();
			int descent = box.Font.FontFamily.GetCellDescent(box.Font.Style);
			int descentPixel = (int) (box.Font.Size * descent / box.Font.FontFamily.GetEmHeight(box.Font.Style));

			//todo: this only takes into account the textbox descent, not the label's!
			label.Location = new Point(0, (int) (box.Bottom -
												 ( g.MeasureString(label.Text, label.Font).Height + descentPixel )) );
			return label;
		}

		private WeSayTextBox AddTextBox(int initialPanelWidth, WritingSystem writingSystem, MultiText multiText)
		{
			WeSayTextBox box = new WeSayTextBox(writingSystem);
			_textBoxes.Add(box);
			box.Name = Name.Replace("-mtc","") + "_" + writingSystem.Id; //for automated tests to find this particular guy
			box.Text = multiText[writingSystem.Id];
			box.Location = new Point(30, 0);
			const int kRightMargin = 25; // for flag button
			box.Width = (initialPanelWidth - box.Left) - kRightMargin;
			box.Anchor = AnchorStyles.Left | AnchorStyles.Right |AnchorStyles.Top;
			//  box.BorderStyle = BorderStyle.FixedSingle;
			box.TextChanged += new EventHandler(OnTextOfSomeBoxChanged);
			box.KeyDown += new KeyEventHandler(OnKeyDownInSomeBox);

			return box;
		}


		void OnKeyDownInSomeBox(object sender, KeyEventArgs e)
		{
			OnKeyDown(e);
			//if (this.KeyDown != null)
			//{
			//    KeyDown.Invoke(sender, e);
			//}
		}

		void OnTextOfSomeBoxChanged(object sender, EventArgs e)
		{
			OnTextChanged(e);
			//if (this.TextChanged != null)
			//{
			//    TextChanged.Invoke(sender, e);
			//}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IList<WritingSystem> WritingSystems
		{
			get { return _writingSystems; }
			set
			{
				_writingSystems = value;
				BuildBoxes(this.MultiText);
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
			//this was evil!
//            if (TextBoxes.Count > 0)
//            {
//                TextBoxes[0].Select();
//            }
		}



	}
}
