using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SIL.Reporting;
using SIL.Text;
using SIL.WritingSystems;
using SIL.Windows.Forms.Keyboarding;
using WeSay.UI.audio;
using SIL.Lift;

namespace WeSay.UI.TextBoxes
{
	/// <summary>
	/// This control presents a table of input fields, with one row for each input system. For audio input systems, it presents a control for recording and playback.
	/// </summary>
	public partial class MultiTextControl: TableLayoutPanel
	{
		private IList<WritingSystemDefinition> _writingSystemsForThisField;
		private readonly List<Control> _inputBoxes;
		private bool _showAnnotationWidget;
		private IServiceProvider _serviceProvider;

		private readonly CommonEnumerations.VisibilitySetting _visibility =
			CommonEnumerations.VisibilitySetting.Visible;

		private static int _widthForWritingSystemLabels = -1;
		private static IWritingSystemRepository _allWritingSystems;
		private static Font _writingSystemLabelFont;
		public bool IsSpellCheckingEnabled { get; set; }
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

		public MultiTextControl(IWritingSystemRepository allWritingSystems, IServiceProvider serviceProvider)
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
								bool showAnnotationWidget, IWritingSystemRepository allWritingSystems,
								CommonEnumerations.VisibilitySetting visibility, bool isSpellCheckingEnabled,
								bool isMultiParagraph, IServiceProvider serviceProvider): this(allWritingSystems, serviceProvider)
		{
			Name = nameForTesting + "-mtc";
			_writingSystemsForThisField = new List<WritingSystemDefinition>();
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
			IsSpellCheckingEnabled = isSpellCheckingEnabled;
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

		public void Closing()
		{
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
#if __MonoCS__
				// The Mono runtime adds the margin to the box width to get the column width.
				// This results in a long-running loop (that somehow terminates) that keeps adding
				// 6 to the column width, resizing and laying out the table repeatedly.
				if (width > Margin.Horizontal)
					box.Width = width - Margin.Horizontal;
				else
#endif
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
					mt.SetAlternative(box.WritingSystem.LanguageTag, box.Text);
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
				var s =  text.GetExactAlternative(box.WritingSystem.LanguageTag);
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
			foreach (WritingSystemDefinition writingSystem in WritingSystemsForThisField)
			{
				RowStyles.Add(new RowStyle(SizeType.AutoSize));


				var box = AddTextBox(writingSystem, multiText);
				if(box==null)
					continue;

				Label label = AddWritingSystemLabel(((IControlThatKnowsWritingSystem) box).WritingSystem.Abbreviation);
				label.Click += subControl_Click;
				label.MouseWheel += subControl_MouseWheel;

				Controls.Add(label, 0, RowCount);
				Controls.Add(box, 1, RowCount);

				if (_showAnnotationWidget) //false for ghosts
				{
					//TODO: THIS IS TRANSITIONAL CODE... AnnotationWidget should probably become a full control (or go away)
					AnnotationWidget aw = new AnnotationWidget(multiText,
															   writingSystem.LanguageTag,
															   box.Name + "-annotationWidget");
					Control annotationControl = aw.MakeControl(new Size()); //p.Size);
					annotationControl.Click += subControl_Click;
					annotationControl.Anchor = AnchorStyles.Right;
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
			IWeSayTextBox tb = c as IWeSayTextBox;
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
						foreach (WritingSystemDefinition ws in _allWritingSystems.AllWritingSystems)
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

		private static Label AddWritingSystemLabel(string abbreviation)
		{
			var label = new Label();
			label.Text = abbreviation;
			label.ForeColor = DisplaySettings.Default.WritingSystemLabelColor;
			label.Anchor = AnchorStyles.Left;
			label.Font = _writingSystemLabelFont;

			label.AutoSize = true;
			return label;
		}

		private Control AddTextBox(WritingSystemDefinition writingSystem, MultiTextBase multiText)
		{
			Control control;
			if (writingSystem.IsVoice)
			{
				return null;
				// How did this ever work?  GatherBySemanticDomain is incompatible with audio writing systems.
				// service provider throws exception in WS 1.4.55 and 1.5.38
				if (_serviceProvider == null)
				{
					//no, better to just omit it.  throw new ConfigurationException("WeSay cannot handle yet audio in this task.");
					return null;
				}
				var ap =_serviceProvider.GetService(typeof (AudioPathProvider)) as AudioPathProvider;
				control = new WeSayAudioFieldBox(writingSystem, ap, _serviceProvider.GetService(typeof(ILogger)) as ILogger);
				control.SuspendLayout();
				((WeSayAudioFieldBox)control).PlayOnly = (_visibility == CommonEnumerations.VisibilitySetting.ReadOnly);
			}
			else
			{
				IWeSayTextBox box = null;
				if (_serviceProvider != null)
				{
					box = _serviceProvider.GetService(typeof (IWeSayTextBox)) as IWeSayTextBox;
				}
				else
				{
					// Shouldn't get to this but just in case.
					box = new WeSayTextBox();
				}
				box.Init(writingSystem, Name);

				control = (Control) box;
				control.SuspendLayout();
				box.ReadOnly = (_visibility == CommonEnumerations.VisibilitySetting.ReadOnly);
				box.Multiline = true;
				box.WordWrap = true;
				box.MultiParagraph = _isMultiParagraph;
				box.IsSpellCheckingEnabled = IsSpellCheckingEnabled;
				//box.Enabled = !box.ReadOnly;
				if (!box.ReadOnly && box is WeSayTextBox)
					KeyboardController.RegisterControl((Control)box);
			}

			_inputBoxes.Add(control);

			string text = multiText.GetExactAlternative(writingSystem.LanguageTag);
			if(_isMultiParagraph) //review... stuff was coming in with just \n, and the text box then didn't show the paragarph marks
			{
				text = text.Replace("\r\n", "\n");
				text = text.Replace("\n", "\r\n");
			}
			control.Name = Name.Replace("-mtc", "") + "_" + writingSystem.LanguageTag;
			//for automated tests to find this particular guy
			control.Text = text;

			if (control is IWeSayTextBox)
			{
				var spans = multiText.GetExactAlternativeSpans(writingSystem.LanguageTag);
				(control as IWeSayTextBox).Spans = spans;
			}

			control.TextChanged += OnTextOfSomeBoxChanged;
			control.KeyDown += OnKeyDownInSomeBox;
			control.MouseWheel += subControl_MouseWheel;

			control.ResumeLayout(false);
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
		public IList<WritingSystemDefinition> WritingSystemsForThisField
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
