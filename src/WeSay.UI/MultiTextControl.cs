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

				//does this work?
				p.AutoSize = true;

				if (_showAnnotationWidget) //false for ghosts
				{
					//TODO: THIS IS TRANSITIONAL CODE... AnnotationWidget should probably become a full control (or go away)
					AnnotationWidget aw =
						new AnnotationWidget(multiText, writingSystem.Id, box.Name + "-annotationWidget");
					Control annotationControl = aw.MakeControl(p.Size);
					p.Controls.Add(annotationControl);
					this.components.Add(annotationControl); //so it will get disposed of when we are
				}

				_vbox.AddControlToBottom(p);
				Height += p.Height;
			}
			ResumeLayout(false);
		}

		private Label AddWritingSystemLabel(WeSayTextBox box)
		{
			Label label = new Label();
			label.Text = box.WritingSystem.Id;
			label.ForeColor = Color.LightGray;


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
