using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;
using WeSay.App.Properties;
using WeSay.CommonTools;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.App.Tests
{
	[TestFixture]
	public class DashBoardTests
	{
		[SetUp]
		public void Setup()
		{
			Form window = new Form();
			window.Size = new Size(800, 600);
			InMemoryRecordListManager manager = new InMemoryRecordListManager();
			IRecordList<LexEntry> entries = manager.GetListOfType<LexEntry>();
			entries.AddNew();

			Dash dash = new Dash(manager, null);
			dash.ThingsToMakeButtonsFor= GetButtonItems();
			dash.Dock = DockStyle.Fill;
			window.Controls.Add(dash);
			window.BackColor = dash.BackColor;
			dash.Activate();
			Application.Run(window);
		}
		[Test, Ignore("not really a test")]
		public void Run()
		{
		}

		private List<IThingOnDashboard> GetButtonItems()
		{
			List<IThingOnDashboard> buttonItems = new List<IThingOnDashboard>();
			buttonItems.Add(new ThingThatGetsAButton("Gather", "Semantic Domains"));
			buttonItems.Add(new ThingThatGetsAButton("Gather", "PNG Word List"));
			buttonItems.Add(new ThingThatGetsAButton("Describe", "Nuhu Sapoo Definitions"));
			buttonItems.Add(new ThingThatGetsAButton("Describe", "Example Sentences"));
			buttonItems.Add(new ThingThatGetsAButton("Describe", "English Definitions"));
			buttonItems.Add(new ThingThatGetsAButton("Describe", "Translate Examples To English"));
			buttonItems.Add(new ThingThatGetsAButton("Describe", "Dictionary Browse && Edit", ButtonStyle.IconFixedWidth, CommonTools.Properties.Resources.blueDictionary));
			buttonItems.Add(new ThingThatGetsAButton("Refine", "Identify Base Forms"));
			buttonItems.Add(new ThingThatGetsAButton("Refine", "Review"));

			buttonItems.Add(new ThingThatGetsAButton("Share", "Print", ButtonStyle.IconVariableWidth, CommonTools.Properties.Resources.greenPrinter));
			buttonItems.Add(new ThingThatGetsAButton("Share", "Email", ButtonStyle.IconVariableWidth, CommonTools.Properties.Resources.greenEmail));
			buttonItems.Add(new ThingThatGetsAButton("Share", "Synchronize", ButtonStyle.IconVariableWidth, CommonTools.Properties.Resources.greenSynchronize));

			return buttonItems;
		}
	}

	class ThingThatGetsAButton : IThingOnDashboard
	{
		private readonly string _groupName;
		private readonly string _localizedLabel;
		private Font _font;
		private ButtonStyle _style;
		private Image _image;



		public ThingThatGetsAButton(string groupName, string localizedLabel, ButtonStyle style, Image image)
		{
			_image = image;
			Style = style;
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

		public ButtonStyle Style
		{
			get { return _style; }
			set { _style = value; }
		}

		#region IThingOnDashboard Members

		public Image Image
		{
			get { return _image;  }
		}

		#endregion
	}



}
