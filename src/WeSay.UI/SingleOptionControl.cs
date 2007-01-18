using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WeSay.Foundation;

namespace WeSay.UI
{
	public partial class SingleOptionControl : Widget
	{
		public SingleOptionControl()
		{
			InitializeComponent();
		}
		public SingleOptionControl(OptionRef optionRef)
		{
			InitializeComponent();
			BuildBoxes(optionRef);
		}
		private void BuildBoxes(OptionRef optionRef)
		{
			SuspendLayout();
//            if (_vbox.Count > 0)
//            {
//                _vbox.Clear();
//            }
			Height = 0;
			const int initialPanelWidth = 200;
			Control box = AddComboControl(initialPanelWidth, optionRef);

		  //  Label label = AddWritingSystemLabel(box);

			//Graphics g = CreateGraphics();
			//int descent = box.Font.FontFamily.GetCellDescent(box.Font.Style);
			//int descentPixel = (int) (box.Font.Size * descent / box.Font.FontFamily.GetEmHeight(box.Font.Style));

			this.components.Add(box);//so it will get disposed of when we are

			Panel p = new Panel();
			p.Controls.Add(box);
		  //  p.Controls.Add(label);
			p.Size = new Size(initialPanelWidth, box.Height + 0);

//            FlagButton flagButton = AddFlagButton(p.Size);
//            p.Controls.Add(flagButton);
//            this.components.Add(flagButton);//so it will get disposed of when we are
//            flagButton.TabStop = false;
//            _vbox.AddControlToBottom(p);
			this.Controls.Add(p);
			Height += p.Height;
			ResumeLayout(false);
		}

		private Control AddComboControl(int initialPanelWidth, OptionRef optionRef)
		{
//            WeSayTextBox box = new WeSayTextBox(writingSystem);
//            _textBoxes.Add(box);
//            box.Name = Name.Replace("-mtc", "") + "_" + writingSystem.Id; //for automated tests to find this particular guy
//            box.Text = multiText[writingSystem.Id];
//            box.Location = new Point(30, 0);
//            const int kRightMargin = 25; // for flag button
//            box.Width = (initialPanelWidth - box.Left) - kRightMargin;
//            box.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
//            //  box.BorderStyle = BorderStyle.FixedSingle;
//            box.TextChanged += new EventHandler(OnTextOfSomeBoxChanged);
//            box.KeyDown += new KeyEventHandler(OnKeyDownInSomeBox);

			return new ComboBox();
		}
	}
}
