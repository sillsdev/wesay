using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Palaso.Reporting;
using Palaso.Text;
using WeSay.LexicalModel.Foundation;
using WeSay.UI.audio;
//using CommonEnumerations=Palaso.Lift.CommonEnumerations;
using Palaso.Lift;
//using CommonEnumerations=Palaso.Lift.CommonEnumerations;

namespace WeSay.UI.TextBoxes
{
	public partial class MultiTextControl: TableLayoutPanel
	{
		private IList<WritingSystem> _writingSystemsForThisField;
		private readonly List<Control> _inputBoxes;
		private bool _showAnnotationWidget;
		private IServiceProvider _serviceProvider;

		private readonly CommonEnumerations.VisibilitySetting _visibility =
			CommonEnumerations.VisibilitySetting.Visible;

		private static int _widthForWritingSystemLabels = -1;
		private static WritingSystemCollection _allWritingSystems;
		private static Font _writingSystemLabelFont;
		private readonly bool _isSpellCheckingEnabled;
		private readonly bool _isMultiParagraph;

		public MultiTextControl(): this(null, null)
		{
			//design mode only
			if (DesignMode)
			{
				AutoSize = false;
				// NONE OF THE FOLLOWING ACTUALLY WORKS... WISH IT DID
				Size = new Size(this.Width, 20);
				CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;//help it be seen
				BackColor = Color.Maroon;
			}
		}

		public MultiTextControl(WritingSystemCollection allWritingSystems, IServiceProvider serviceProvider)
		{
			if (DesignMode)
			{
				AutoSize = false;
				// NONE OF THE FOLLOWING ACTUALLY WORKS... WISH IT DID
				Size = new Size(this.Width, 20);
				CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;//help it be seen
				BackColor = Color.Maroon;
			}

			SuspendLayout();
			_allWritingSystems = allWritingSystems;
			_serviceProvider = serviceProvider;
			components = new Container();
			InitializeComponent();
			_inputBoxes = new List<Control>();
			//this.BackColor = System.Drawing.Color.Crimson;
			_writingSystemLabelFont = new Font(FontFamily.GenericSansSerif, 9);

			if (-1 == WidthForWritingSystemLabels)
			{
				//happens when this is from a hand-placed designer piece,
				//in which case we don't really care about aligning anyhow
				ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); //ws label
			}
			else
			{
				ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, WidthForWritingSystemLabels));
			}
			ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); //text
			ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); //annotation widget

			ResumeLayout(false);
		}


		public MultiTextControl(IList<string> writingSystemIds,
								MultiText multiTextToCopyFormsFrom, string nameForTesting,
								bool showAnnotationWidget, WritingSystemCollection allWritingSystems,
								CommonEnumerations.VisibilitySetting visibility, bool isSpellCheckingEnabled,
								bool isMultiParagraph, IServiceProvider serviceProvider): this(allWritingSystems, serviceProvider)
		{
			Name = nameForTesting + "-mtc";
			_writingSystemsForThisField = new List<WritingSystem>();
//            foreach (KeyValuePair<string, WritingSystem> pair in allWritingSystems)
//            {
//                if (writingSystemIds.Contains(pair.Key))
//                {
//                    _writingSystemsForThisField.Add(pair.Value);
//                }
//            }
			foreach (var id in writingSystemIds)
			{
				if (allWritingSystems.Contains(id)) //why wouldn't it?
				{
					_writingSystemsForThisField.Add(allWritingSystems.Get(id));
				}
			}
			_showAnnotationWidget = showAnnotationWidget;
			_visibility = visibility;
			_isSpellCheckingEnabled = isSpellCheckingEnabled;
			_isMultiParagraph = isMultiParagraph;
			BuildBoxes(multiTextToCopyFormsFrom);
		}

		public void FocusOnFirstWsAlternative()
		{
			if (TextBoxes.Count > 0)
			{
				TextBoxes[0].Focus();
			}
		}

		///<remarks>This can't be done during construction... we have to wait until
		///we actually have a parent to do this.</remarks>
		private void OnParentChanged(object sender, EventArgs e)
		{
			if (Parent != null && _visibility == CommonEnumerations.VisibilitySetting.ReadOnly)
			{
				BackColor = Parent.BackColor;
				foreach (Control box in _inputBoxes)
				{
					box.BackColor = Parent.BackColor;
					box.TabStop = false;
				}
			}
		}

		protected override void OnResize(EventArgs eventargs)
		{

			base.OnResize(eventargs);
			if (DesignMode)
			{
				return;
			}
			SizeBoxes();
		}

		private void SizeBoxes()
		{
			int[] widths = GetColumnWidths();
			if (widths == null || widths.Length < 2)
			{
				return;
			}
			int width = widths[1];
			foreach (Control box in _inputBoxes)
			{
				box.Width = width;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<Control> TextBoxes
		{
			get { return _inputBoxes; }
		}

		public MultiText GetMultiText()
		{
				//we don't have a binding that would keep an internal multitext up to date.
				//This seems cleaner and sufficient, at the moment.
				MultiText mt = new MultiText();
				foreach (IControlThatKnowsWritingSystem box in TextBoxes)
				{
					mt.SetAlternative(box.WritingSystem.Id, box.Text);
				}
				return mt;
		}

		/// <summary>
		/// Copy the forms into the boxes that we already have. Does not change which boxes we have!
		/// </summary>
		/// <param name="text"></param>
		public void SetMultiText(MultiText text)
		{
			foreach (IControlThatKnowsWritingSystem box in TextBoxes)
			{
				var s =  text.GetExactAlternative(box.WritingSystem.Id);
				box.Text = s ?? string.Empty;
			}
		}

		private void BuildBoxes(MultiText multiText)
		{
			SuspendLayout();

			if (Controls.Count > 0)
			{
				_inputBoxes.Clear();
				Controls.Clear();
				RowCount = 0;
				RowStyles.Clear();
			}
			Debug.Assert(RowCount == 0);
			foreach (WritingSystem writingSystem in WritingSystemsForThisField)
			{
				RowStyles.Add(new RowStyle(SizeType.AutoSize));


				var box = AddTextBox(writingSystem, multiText);
				if(box==null)
					continue;

				Label label = AddWritingSystemLabel(box, ((IControlThatKnowsWritingSystem) box).WritingSystem.Abbreviation);
				label.Click += subControl_Click;
				label.MouseWheel += subControl_MouseWheel;

				Controls.Add(label, 0, RowCount);
				Controls.Add(box, 1, RowCount);

				if (_showAnnotationWidget) //false for ghosts
				{
					//TODO: THIS IS TRANSITIONAL CODE... AnnotationWidget should probably become a full control (or go away)
					AnnotationWidget aw = new AnnotationWidget(multiText,
															   writingSystem.Id,
															   box.Name + "-annotationWidget");
					Control annotationControl = aw.MakeControl(new Size()); //p.Size);
					annotationControl.Click += subControl_Click;
					annotationControl.Anchor = AnchorStyles.Right | AnchorStyles.Top;
					Controls.Add(annotationControl, 2, RowCount);
				}
				//else
				//{
				//    SetColumnSpan(box, 2);
				//}
				RowCount++;
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

		private void subControl_Click(object sender, EventArgs e)
		{
			Control c = GetControlFromPosition(1, GetRow((Control) sender));
			FocusWithInsertionPointAtEnd(c);
		}

		private static void FocusWithInsertionPointAtEnd(Control c)
		{
			c.Focus();
			TextBox tb = c as TextBox;
			if (tb != null)
			{
				tb.Select(1000, 0); //go to end}
			}
		}

		/// <summary>
		/// We want all the texts to line up, so we have to take into account the maximum
		/// width of all the ws's that might be on the form, not just in this multitext
		/// </summary>
		protected static int WidthForWritingSystemLabels
		{
			get
			{
				if (_widthForWritingSystemLabels == -1)
				{
					Size fudgeFactor= new Size(20,0);// it wasn't coming out large enough... could be because of label padding/margin
					//null happens when this is from a hand-placed designer piece,
					//in which case we don't really care about aligning anyhow
					if (_allWritingSystems != null)
					{
						foreach (WritingSystem ws in _allWritingSystems.WritingSystemDefinitions)
						{
							Size size = TextRenderer.MeasureText(ws.Abbreviation,
																 _writingSystemLabelFont) +fudgeFactor;

							if (size.Width > _widthForWritingSystemLabels)
							{
								_widthForWritingSystemLabels = size.Width;
							}
						}
					}
				}
				return _widthForWritingSystemLabels;
			}
		}

		private static Label AddWritingSystemLabel(Control box, string abbreviation)
		{
			Label label = new Label();
			label.Text = abbreviation;
			label.ForeColor = DisplaySettings.Default.WritingSystemLabelColor;
			label.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			label.Font = _writingSystemLabelFont;

			label.AutoSize = true;

			//            Graphics g = CreateGraphics();
			//            int descent = box.Font.FontFamily.GetCellDescent(box.Font.Style);
			//            int descentPixel = (int) (box.Font.Size * descent / box.Font.FontFamily.GetEmHeight(box.Font.Style));
			//
			//            //todo: this only takes into account the textbox descent, not the label's!
			//            label.Location = new Point(0, (int) (box.Bottom -
			//                                                 ( g.MeasureString(label.Text, label.Font).Height + descentPixel )) );

			//todo: switch to TextRenderer.Measure
			int labelAscentInPixels =
				(int)
				(label.Font.Size * label.Font.FontFamily.GetCellAscent(label.Font.Style) /
				 label.Font.FontFamily.GetEmHeight(box.Font.Style));
			int contentAscentInPixels =
				(int)
				(box.Font.Size * box.Font.FontFamily.GetCellAscent(box.Font.Style) /
				 label.Font.FontFamily.GetEmHeight(box.Font.Style));
			int howMuchFartherDownToPlaceLabelThanText = Math.Max(0,
																  contentAscentInPixels -
																  labelAscentInPixels);

			label.Margin = new Padding(0, box.Top + howMuchFartherDownToPlaceLabelThanText, 0, 0);
			return label;
		}

		private Control AddTextBox(WritingSystem writingSystem, MultiTextBase multiText)
		{
			Control control;
			if (writingSystem.IsVoice)
			{
#if MONO
				return null;
#endif
				if (_serviceProvider == null)
				{
					//no, better to just omit it.  throw new ConfigurationException("WeSay cannot handle yet audio in this task.");
					return null;
				}
				var ap =_serviceProvider.GetService(typeof (AudioPathProvider)) as AudioPathProvider;
				control = new WeSayAudioFieldBox(writingSystem, ap, _serviceProvider.GetService(typeof(Palaso.Reporting.ILogger)) as ILogger);
				((WeSayAudioFieldBox)control).PlayOnly = (_visibility == CommonEnumerations.VisibilitySetting.ReadOnly);
			}
			else
			{
				var box = new WeSayTextBox(writingSystem, Name);
				control = box;
				box.ReadOnly = (_visibility == CommonEnumerations.VisibilitySetting.ReadOnly);
				box.Multiline = true;
				box.WordWrap = true;
				box.MultiParagraph = _isMultiParagraph;
				box.IsSpellCheckingEnabled = _isSpellCheckingEnabled;
				//box.Enabled = !box.ReadOnly;
			}

			_inputBoxes.Add(control);

			string text = multiText[writingSystem.Id];
			if(_isMultiParagraph) //review... stuff was coming in with just \n, and the text box then didn't show the paragarph marks
			{
				text = text.Replace("\r\n", "\n");
				text = text.Replace("\n", "\r\n");
			}
			control.Name = Name.Replace("-mtc", "") + "_" + writingSystem.Id;
			//for automated tests to find this particular guy
			control.Text = text;

			control.TextChanged += OnTextOfSomeBoxChanged;
			control.KeyDown += OnKeyDownInSomeBox;
			control.MouseWheel += subControl_MouseWheel;

			return control;
		}

		private void subControl_MouseWheel(object sender, MouseEventArgs e)
		{
			OnMouseWheel(e);
		}

		private void OnKeyDownInSomeBox(object sender, KeyEventArgs e)
		{
			OnKeyDown(e);
		}

		private void OnTextOfSomeBoxChanged(object sender, EventArgs e)
		{
			OnTextChanged(e);
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IList<WritingSystem> WritingSystemsForThisField
		{
			get { return _writingSystemsForThisField; }
			set
			{
				_writingSystemsForThisField = value;
				BuildBoxes(GetMultiText());
			}
		}

		public bool ShowAnnotationWidget
		{
			get { return _showAnnotationWidget; }
			set
			{
				if (_showAnnotationWidget != value)
				{
					_showAnnotationWidget = value;
					BuildBoxes(GetMultiText());
				}
			}
		}

		public void ClearAllText()
		{
			foreach (IControlThatKnowsWritingSystem box in _inputBoxes)
			{
				box.Text = "";
			}
		}
	}
}