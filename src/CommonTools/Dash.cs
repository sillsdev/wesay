using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.i8n;
using WeSay.CommonTools.Properties;
using WeSay.Data;
using WeSay.LexicalModel;

namespace WeSay.CommonTools
{

	public partial class Dash : UserControl
	{
		private readonly IRecordListManager _recordListManager;
		private int _standardButtonWidth;
		private List<ThingThatGetsAButton> _buttonItems;
		private List<ButtonGroup> _groups;

		public Dash(IRecordListManager RecordListManager)
		{
			_recordListManager = RecordListManager;
			InitializeComponent();
			//_flow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

			_groups = new List<ButtonGroup>();
			_groups.Add(new ButtonGroup("Gather", true,
										Color.FromArgb(155, 187, 89),
										Color.FromArgb(195, 214, 155),
										Color.FromArgb(235, 241, 222)));
			_groups.Add(new ButtonGroup("Describe", true, Color.FromArgb(85, 142, 213),
				Color.FromArgb(185,205,229),
				Color.White));
			_groups.Add(new ButtonGroup("Refine", true, Color.FromArgb(250,192,144),
							Color.FromArgb(252,213,181),
				Color.White));
			_groups.Add(new ButtonGroup("Share", true, Color.FromArgb(119,147,60),
							Color.White,
				 Color.White));
		}

		private int DetermineStandardButtonWidth(List<ThingThatGetsAButton> items)
		{
			int maxRequestedWidth = 30;
			foreach (ThingThatGetsAButton item in items)
			{
				int w = item.WidthToDisplayFullSizeLabel;
				if(w > maxRequestedWidth)
					maxRequestedWidth = w;
			}
			return maxRequestedWidth;
		}

		private void Dash_Load(object sender, EventArgs e)
		{
			DictionaryStatusControl title = new DictionaryStatusControl(_recordListManager.GetListOfType<LexEntry>());
			title.Font = new Font("Arial", 14);
			_flow.Controls.Add(title);

			GetButtonItems();
			_standardButtonWidth = DetermineStandardButtonWidth(_buttonItems);
			_standardButtonWidth += 30;//for space between text and button

			foreach (ButtonGroup group in _groups)
			{
				if (!group.MakeButtonsSameSize)
				{
					_flow.Controls.Add(MakeButtonGroup(group, 0));
				}
				else
				{
					_flow.Controls.Add(MakeButtonGroup(group, _standardButtonWidth));
				}
			}
		 }

		private IEnumerable<string> GetGroups()
		{
			List<string> foundGroups = new List<string>();
			foreach (ThingThatGetsAButton item in _buttonItems)
			{
				if (!foundGroups.Contains(item.GroupName))
				{
					foundGroups.Add(item.GroupName);
					yield return item.GroupName;
				}
			}
		}

		private FlowLayoutPanel MakeButtonGroup(ButtonGroup group, int buttonWidth)
		{
			Label header = new Label();
			header.Text = StringCatalog.Get(group.Name);
			header.Font = new Font("Arial", 13);
			_flow.Controls.Add(header);

			FlowLayoutPanel buttonGroup = new FlowLayoutPanel();
			buttonGroup.AutoSize = true;
			buttonGroup.FlowDirection = FlowDirection.LeftToRight;
		   // buttonGroup.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			buttonGroup.Margin =  new Padding(30,0,0,15);

			foreach (ThingThatGetsAButton item in _buttonItems)
			{
				if (item.GroupName == group.Name)
				{
					buttonGroup.Controls.Add(MakeButton(item, buttonWidth, group));
				}
			}
			return buttonGroup;
		}

		private Control MakeButton(ThingThatGetsAButton item, int buttonWidth, ButtonGroup group)
		{
			DashboardButton button = item.MakeButton();

			button.Font = item.Font;
			button.AutoSize = false;
			button.BorderColor = group.BorderColor;
			button.DoneColor = group.DoneColor;
			button.TodoColor = group.TodoColor;
			if (buttonWidth == 0)
				buttonWidth = item.WidthToDisplayFullSizeLabel;

			button.Size = new Size(buttonWidth, 50);
			button.Text = item.LocalizedLabel;
			return button;
		}


		private void GetButtonItems()
		{
			_buttonItems = new List<ThingThatGetsAButton>();
			_buttonItems.Add(new ThingThatGetsAButton("Gather", "Semantic Domains"));
			_buttonItems.Add(new ThingThatGetsAButton("Gather", "PNG Word List"));
			_buttonItems.Add(new ThingThatGetsAButton("Describe", "Nuhu Sapoo Definitions"));
			_buttonItems.Add(new ThingThatGetsAButton("Describe", "Example Sentences"));
			_buttonItems.Add(new ThingThatGetsAButton("Describe", "English Definitions"));
			_buttonItems.Add(new ThingThatGetsAButton("Describe", "Translate Examples To English"));
			_buttonItems.Add(new ThingThatGetsAButton("Describe", "Dictionary Browse && Edit", ThingThatGetsAButton.ButtonStyle.IconFixedWidth, Resources.blueDictionary));
			_buttonItems.Add(new ThingThatGetsAButton("Refine", "Identify Base Forms"));
			_buttonItems.Add(new ThingThatGetsAButton("Refine", "Review"));

			_buttonItems.Add(new ThingThatGetsAButton("Share", "Print",ThingThatGetsAButton.ButtonStyle.IconVariableWidth, Resources.greenPrinter));
			_buttonItems.Add(new ThingThatGetsAButton("Share", "Email", ThingThatGetsAButton.ButtonStyle.IconVariableWidth, Resources.greenEmail));
			_buttonItems.Add(new ThingThatGetsAButton("Share", "Synchronize", ThingThatGetsAButton.ButtonStyle.IconVariableWidth, Resources.greenSynchronize));
		}
	}



	class ThingThatGetsAButton
	{
		private readonly string _groupName;
		private readonly string _localizedLabel;
		private Font _font;
		private ButtonStyle _style;
		private Image _image;

		public enum ButtonStyle
		{
			FixedAmount,
			VariableAmount,
			IconFixedWidth,
			IconVariableWidth
		} ;

		public ThingThatGetsAButton(string groupName, string localizedLabel, ButtonStyle style, Image image)
		{
			_image = image;
			_style = style;
			_groupName = groupName;
			_localizedLabel = localizedLabel;
			Font = new Font("Arial", 10);
		}

		public ThingThatGetsAButton(string groupName, string localizedLabel)
			: this(groupName, localizedLabel, ButtonStyle.VariableAmount, null)
		{
		}

		   //todo: this belongs on the button, which knows better what it has planned
		public int WidthToDisplayFullSizeLabel
		{
			get
			{
				return TextRenderer.MeasureText(LocalizedLabel, Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.LeftAndRightPadding).Width;
			}
		}

		public string GroupName
		{
			get { return _groupName; }
		}

		public string LocalizedLabel
		{
			get { return _localizedLabel; }
		}

		public Font Font
		{
			get { return _font; }
			set { _font = value; }
		}

		public DashboardButton MakeButton()
		{
			switch (_style)
			{
				case ButtonStyle.FixedAmount:
					return new DashboardButton();
					break;
				case ButtonStyle.VariableAmount:
					return new DashboardButton();
					break;
				case ButtonStyle.IconFixedWidth:
					return new DashboardButtonWithIcon(_image, false);
					break;
				case ButtonStyle.IconVariableWidth:
					return new DashboardButtonWithIcon(_image, true);
					break;
				default:
					return new DashboardButton();
					break;
			}
		}
	}


	internal class ButtonGroup
	{
		private readonly string _name;
		private readonly bool _makeButtonsSameSize;
		private readonly Color _color;
		private Color _doneColor;
		private Color _borderColor;
		private Color _todoColor;

		public ButtonGroup(string name, bool makeButtonsSameSize, Color borderColor, Color doneColor, Color todoColor)
		{
			_name = name;
			_makeButtonsSameSize = makeButtonsSameSize;
			_borderColor = borderColor;
			_doneColor = doneColor;
			_todoColor = todoColor;
		}

		public string Name
		{
			get { return _name; }
		}

		public bool MakeButtonsSameSize
		{
			get { return _makeButtonsSameSize; }
		}

		public Color Color
		{
			get { return _color; }
		}

		public Color DoneColor
		{
			get { return _doneColor; }
		}

		public Color BorderColor
		{
			get { return _borderColor; }
		}

		public Color TodoColor
		{
			get { return _todoColor; }
		}
	}
}
