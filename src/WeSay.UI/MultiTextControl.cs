using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Language;

namespace WeSay.UI
{
	public partial class MultiTextControl : TableLayoutPanel
	{
		private IList<WritingSystem> _writingSystems;
		private List<WeSayTextBox> _textBoxes;
		private bool _showAnnotationWidget;

		public MultiTextControl()
		{
			this.components = new Container();
			InitializeComponent();
			_textBoxes = new List<WeSayTextBox>();
		   // this.BackColor = System.Drawing.Color.Crimson;
			ColumnCount = 3;
			ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
			ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
		}


		public MultiTextControl(IList<WritingSystem> writingSystems, MultiText multiTextToCopyFormsFrom, string nameForTesting, bool showAnnotationWidget):this()
		{
			Name = nameForTesting+"-mtc";
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
				foreach (WeSayTextBox box in TextBoxes )
				{
					mt.SetAlternative(box.WritingSystem.Id, box.Text);
				}
				return mt;
			}
		}

		private void BuildBoxes(MultiText multiText)
		{
			SuspendLayout();

			if (Controls.Count > 0)
			{
				Controls.Clear();
				RowCount = 0;
				RowStyles.Clear();
			}

			foreach (WritingSystem writingSystem in WritingSystems)
			{
				RowCount++;
				RowStyles.Add(new RowStyle(SizeType.AutoSize));

				WeSayTextBox box = AddTextBox(writingSystem, multiText);

				Label label = AddWritingSystemLabel(box);
				label.Click += new EventHandler(subControl_Click);

				Controls.Add(label, 0, RowCount);
				Controls.Add(box, 1, RowCount);
				this.components.Add(box);//so it will get disposed of when we are

				if (_showAnnotationWidget) //false for ghosts
				{
					//TODO: THIS IS TRANSITIONAL CODE... AnnotationWidget should probably become a full control (or go away)
					AnnotationWidget aw =
						new AnnotationWidget(multiText, writingSystem.Id, box.Name + "-annotationWidget");
					Control annotationControl = aw.MakeControl(new Size());//p.Size);
					annotationControl.Click += new EventHandler(subControl_Click);
					annotationControl.Anchor = AnchorStyles.Right | AnchorStyles.Top;
					Controls.Add(annotationControl, 2, RowCount);

					this.components.Add(annotationControl); //so it will get disposed of when we are
				}
				//else
				//{
				//    SetColumnSpan(box, 2);
				//}
			}

			ResumeLayout(false);
		}


		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			foreach (Control c in Controls)
			{
				if (c.TabStop)
				{
					FocusWithInsertionPointAtEnd(c);
					break;
				}
			}
		}

		void subControl_Click(object sender, EventArgs e)
		{
			Control c = GetControlFromPosition(1, GetRow((Control)sender));
			FocusWithInsertionPointAtEnd(c);
		}

		private static void FocusWithInsertionPointAtEnd(Control c) {
			c.Focus();
			TextBox tb = c as TextBox;
			if(tb != null)
			{
				tb.Select(1000, 0);//go to end}
			}
		}

		static private Label AddWritingSystemLabel(WeSayTextBox box)
		{
			Label label = new Label();
			label.Text = box.WritingSystem.Id;
			label.ForeColor = Color.LightGray;
			label.Anchor = AnchorStyles.Left | AnchorStyles.Top;

			label.AutoSize = true;

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

			label.Margin = new Padding(0, box.Top + howMuchFartherDownToPlaceLabelThanText,0,0);
			return label;
		}

		private WeSayTextBox AddTextBox(WritingSystem writingSystem, MultiText multiText)
		{
			WeSayTextBox box = new WeSayTextBox(writingSystem, this.Name);
			_textBoxes.Add(box);
			box.Name = Name.Replace("-mtc","") + "_" + writingSystem.Id; //for automated tests to find this particular guy
			box.Text = multiText[writingSystem.Id];

			box.Dock = DockStyle.Fill;
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
