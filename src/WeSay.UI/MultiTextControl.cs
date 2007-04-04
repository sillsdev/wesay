using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Language;

namespace WeSay.UI
{
	public partial class MultiTextControl : UserControl
	{
		private IList<WritingSystem> _writingSystems;
		private List<WeSayTextBox> _textBoxes;
		//public new event EventHandler TextChanged;
	   // public new event KeyEventHandler KeyDown;
		// public event System.EventHandler SpecialKeyPress;
		private bool _showAnnotationWidget;

		public MultiTextControl()
		{
			this.components = new Container();
			InitializeComponent();
			_textBoxes = new List<WeSayTextBox>();
			_vbox.Name = "vbox of anonymous multitext";
			_vbox.Resize += new EventHandler(OnVbox_Resize);
		   // this.BackColor = System.Drawing.Color.Crimson;
			//_vbox.BackColor = System.Drawing.Color.Yellow;
			_vbox.Location = new Point(0, 0);
			_vbox.Size = this.Size;
		   _vbox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			this.Resize += new EventHandler(MultiTextControl_Resize);
		}

		void MultiTextControl_Resize(object sender, EventArgs e)
		{
//            _vbox.Size = new Size(this.Width, _vbox.Height);

		}

		void OnVbox_Resize(object sender, EventArgs e)
		{
			//todo: WHY IS THE CLASS A CONTROL? IT DISPLAYS NOTHING!
			//this.Size = new Size(this.Width, ((Control) sender).Size.Height);
			this.Height = 10 + ((Control) sender).Size.Height;
		}
		public MultiTextControl(IList<WritingSystem> writingSystems, MultiText multiTextToCopyFormsFrom, string nameForTesting, bool showAnnotationWidget):this()
		{
			Name = nameForTesting+"-mtc";
			_vbox.Name = Name + "-vbox";
			_writingSystems = writingSystems;
			_showAnnotationWidget = showAnnotationWidget;
			BuildBoxes(multiTextToCopyFormsFrom);
		}
		public MultiTextControl(IList<WritingSystem> writingSystems, MultiText text)
			: this(writingSystems, text, "Unknown", true)
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
			int initialPanelWidth = this.Width;// 200;
			foreach (WritingSystem writingSystem in WritingSystems)
			{
				WeSayTextBox box = AddTextBox(initialPanelWidth, writingSystem, multiText);

				Label label = AddWritingSystemLabel(box);
				label.Click += new EventHandler(subControl_Click);

				//Graphics g = CreateGraphics();
				//int descent = box.Font.FontFamily.GetCellDescent(box.Font.Style);
				//int descentPixel = (int) (box.Font.Size * descent / box.Font.FontFamily.GetEmHeight(box.Font.Style));

				this.components.Add(box);//so it will get disposed of when we are

				FlexibleHeightPanel p = new FlexibleHeightPanel(initialPanelWidth, 0, box);
				p.Anchor = AnchorStyles.Left | AnchorStyles.Right;
				p.Click += panel_Click;
				p.Controls.Add(label);
		   //     p.BackColor = System.Drawing.Color.LightSteelBlue;
			   // p.AutoSize = true;

				if (_showAnnotationWidget) //false for ghosts
				{
					//TODO: THIS IS TRANSITIONAL CODE... AnnotationWidget should probably become a full control (or go away)
					AnnotationWidget aw =
						new AnnotationWidget(multiText, writingSystem.Id, box.Name + "-annotationWidget");
					Control annotationControl = aw.MakeControl(p.Size);
					annotationControl.Click +=new EventHandler(subControl_Click);
					annotationControl.Anchor = AnchorStyles.Right | AnchorStyles.Top ;
					p.Controls.Add(annotationControl);
					this.components.Add(annotationControl); //so it will get disposed of when we are
				}

				_vbox.AddControlToBottom(p);
			   // Height += p.Height;
			}
		  //  Height = _vbox.Height + 100;

			ResumeLayout(false);
		}

		static void panel_Click(object sender, EventArgs e)
		{
			Control control = (Control)sender;
			foreach (Control c in control.Controls)
			{
				if (c.TabStop)
				{
					c.Focus();
					break;
				}
			}
		}

		static void subControl_Click(object sender, EventArgs e)
		{
			Control control = (Control) sender;
			foreach (Control c in control.Parent.Controls)
			{
				if(c.TabStop)
				{
					c.Focus();
					break;
				}
			}
		}

		private Label AddWritingSystemLabel(WeSayTextBox box)
		{
			Label label = new Label();
			label.Text = box.WritingSystem.Id;
			label.ForeColor = Color.LightGray;
			label.Anchor = AnchorStyles.Left | AnchorStyles.Top;
//
//
//            Graphics g = CreateGraphics();
//            int descent = box.Font.FontFamily.GetCellDescent(box.Font.Style);
//            int descentPixel = (int) (box.Font.Size * descent / box.Font.FontFamily.GetEmHeight(box.Font.Style));
//
//            //todo: this only takes into account the textbox descent, not the label's!
//            label.Location = new Point(0, (int) (box.Bottom -
//                                                 ( g.MeasureString(label.Text, label.Font).Height + descentPixel )) );

			//todo: switch to TextRenderer.Measure
			int labelAscentInPixels = (int)(label.Font.Size * label.Font.FontFamily.GetCellAscent(label.Font.Style) / label.Font.FontFamily.GetEmHeight(box.Font.Style));
			int contentAscentInPixels = (int)(box.Font.Size * box.Font.FontFamily.GetCellAscent(box.Font.Style) / label.Font.FontFamily.GetEmHeight(box.Font.Style));
			int howMuchFartherDownToPlaceLabelThanText = Math.Max(0, contentAscentInPixels - labelAscentInPixels);

			label.Location = new Point(0, (int)(box.Top + howMuchFartherDownToPlaceLabelThanText));
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
			box.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			//  box.BorderStyle = BorderStyle.FixedSingle;
			box.TextChanged += new EventHandler(OnTextOfSomeBoxChanged);
			box.KeyDown += new KeyEventHandler(OnKeyDownInSomeBox);

			return box;
		}


		void OnKeyDownInSomeBox(object sender, KeyEventArgs e)
		{
		  OnKeyDown(e);
//
////            if (Environment.OSVersion.Platform != PlatformID.Unix)
////            {
////                SetSuppressKeyPress(e, true);
////            }
//            switch (e.KeyCode)
//            {
//                case Keys.Return:
//                    e.Handled = true;
//                    if (SpecialKeyPress != null)
//                    {
//                        SpecialKeyPress.Invoke(this, e.KeyCode);
//                    }
//                    break;
//
//                default:
//                    e.Handled = false;
////                    if (Environment.OSVersion.Platform != PlatformID.Unix)
////                    {
////                        SetSuppressKeyPress(e, false);
////                    }
//                    break;
//            }
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
				BuildBoxes(MultiText);
			}
		}

		public bool ShowAnnotationWidget
		{
			get { return this._showAnnotationWidget; }
			set {
				if (this._showAnnotationWidget != value)
				{
					this._showAnnotationWidget = value;
					BuildBoxes(MultiText);
				}
			}
		}

		public void ClearAllText()
		{
			foreach (WeSayTextBox box in _textBoxes)
			{
				box.Text = "";
			}
		}
	}
}
