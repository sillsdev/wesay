using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;
using WeSay.App.Properties;
using WeSay.CommonTools;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Dashboard;
using WeSay.LexicalModel;

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
			buttonItems.Add(new ThingThatGetsAButton(DashboardGroup.Gather, "Semantic Domains", "Semantic Domains Long Label", "Semantic Domains description"));
			buttonItems.Add(new ThingThatGetsAButton(DashboardGroup.Gather, "PNG Word List", "PNG Word List Long Label", "PNG Word List description"));
			buttonItems.Add(new ThingThatGetsAButton(DashboardGroup.Describe, "Nuhu Sapoo Definitions", "Nuhu Sapoo Definitions Long Label", "Nuhu Sapoo Definitions description"));
			buttonItems.Add(new ThingThatGetsAButton(DashboardGroup.Describe, "Example Sentences", "Example Sentences Long Label", "Example Sentences description"));
			buttonItems.Add(new ThingThatGetsAButton(DashboardGroup.Describe, "English Definitions", "English Definitions Long Label", "English Definitions description"));
			buttonItems.Add(new ThingThatGetsAButton(DashboardGroup.Describe, "Translate Examples To English", "Translate Examples To English Long Label", "Translate Examples To English description"));
			buttonItems.Add(new ThingThatGetsAButton(DashboardGroup.Describe, "Dictionary Browse && Edit", "Dictionary Browse && Edit Long Label", "Dictionary Browse && Edit description", ButtonStyle.IconFixedWidth, null/*CommonTools.Properties.Resources.blueDictionary*/));
			buttonItems.Add(new ThingThatGetsAButton(DashboardGroup.Refine, "Identify Base Forms", "Identify Base Forms Long Label", "Identify Base Forms description"));
			buttonItems.Add(new ThingThatGetsAButton(DashboardGroup.Refine, "Review", "Review Long Label", "Review description"));

//            buttonItems.Add(new ThingThatGetsAButton(DashboardGroup.Share, "Print", ButtonStyle.IconVariableWidth, Addin..Properties.Resources.greenPrinter));
//            buttonItems.Add(new ThingThatGetsAButton(DashboardGroup.Share, "Email", ButtonStyle.IconVariableWidth, CommonTools.Properties.Resources.greenEmail));
//            buttonItems.Add(new ThingThatGetsAButton(DashboardGroup.Share, "Synchronize", ButtonStyle.IconVariableWidth, CommonTools.Properties.Resources.greenSynchronize));

			return buttonItems;
		}
	}

	class ThingThatGetsAButton : IThingOnDashboard
	{
		private readonly DashboardGroup _group;
		private readonly string _localizedLabel;
		private readonly string _localizedLongLabel;
		private readonly string _description;
		private Font _font;
		private ButtonStyle _style;
		private Image _image;



		public ThingThatGetsAButton(DashboardGroup group, string localizedLabel, string localizedLongLabel, string description,
									ButtonStyle style, Image image)
		{
			_image = image;
			DashboardButtonStyle = style;
			_group = group;
			_localizedLabel = localizedLabel;
			_localizedLongLabel = localizedLongLabel;
			_description = description;
			Font = new Font("Arial", 10);
		}

		public ThingThatGetsAButton(DashboardGroup group, string localizedLabel, string localizedLongLabel, string description)
			: this(group, localizedLabel, localizedLongLabel, description, ButtonStyle.VariableAmount, null)
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


		#region IThingOnDashboard Members

		public DashboardGroup Group
		{
			get { return DashboardGroup.Describe; }
		}

		#endregion

		public string LocalizedLabel
		{
			get { return _localizedLabel; }
		}

		public string LocalizedLongLabel
		{
			get { return _localizedLongLabel; }
		}

		public string Description
		{
			get { return _description; }
		}

		public Font Font
		{
			get { return _font; }
			set { _font = value; }
		}

		public ButtonStyle DashboardButtonStyle
		{
			get { return _style; }
			set { _style = value; }
		}

		#region IThingOnDashboard Members

		public Image DashboardButtonImage
		{
			get { return _image;  }
		}

		#endregion
	}



}
