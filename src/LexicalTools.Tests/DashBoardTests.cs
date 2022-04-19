using NUnit.Framework;
using SIL.TestUtilities;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.LexicalTools.Dashboard;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class DashBoardTests
	{
		private LexEntryRepository _lexEntryRepository;
		private TemporaryFolder _tempFolder;

		[SetUp]
		public void Setup()
		{
			_tempFolder = new TemporaryFolder(GetType().Name);
			_lexEntryRepository = new LexEntryRepository(_tempFolder.GetPathForNewTempFile(false));

			Form window = new Form();
			window.Size = new Size(800, 600);

			_lexEntryRepository.CreateItem();

			Dash dash = new Dash(_lexEntryRepository, null);//, new UserSettingsForTask());
			dash.ThingsToMakeButtonsFor = GetButtonItems();
			dash.Dock = DockStyle.Fill;
			window.Controls.Add(dash);
			window.BackColor = dash.BackColor;
			dash.Activate();
			Application.Run(window);
		}

		[TearDown]
		public void Teardown()
		{
			_lexEntryRepository.Dispose();
			_tempFolder.Dispose();
		}

		[Test]
		[Ignore("not really a test")]
		public void Run() { }

		private static List<IThingOnDashboard> GetButtonItems()
		{
			List<IThingOnDashboard> buttonItems = new List<IThingOnDashboard>();
			buttonItems.Add(new ThingThatGetsAButton("Semantic Domains",
													 "Semantic Domains Long Label",
													 "Semantic Domains description"));
			buttonItems.Add(new ThingThatGetsAButton("PNG Word List",
													 "PNG Word List Long Label",
													 "PNG Word List description"));
			buttonItems.Add(new ThingThatGetsAButton("Nuhu Sapoo Definitions",
													 "Nuhu Sapoo Definitions Long Label",
													 "Nuhu Sapoo Definitions description"));
			buttonItems.Add(new ThingThatGetsAButton("Example Sentences",
													 "Example Sentences Long Label",
													 "Example Sentences description"));
			buttonItems.Add(new ThingThatGetsAButton("English Definitions",
													 "English Definitions Long Label",
													 "English Definitions description"));
			buttonItems.Add(new ThingThatGetsAButton("Translate Examples To English",
													 "Translate Examples To English Long Label",
													 "Translate Examples To English description"));
			buttonItems.Add(new ThingThatGetsAButton("Dictionary Browse && Edit",
													 "Dictionary Browse && Edit Long Label",
													 "Dictionary Browse && Edit description",
													 ButtonStyle.IconFixedWidth,
													 null
								/*CommonTools.Properties.Resources.blueDictionary*/));
			buttonItems.Add(new ThingThatGetsAButton("Identify Base Forms",
													 "Identify Base Forms Long Label",
													 "Identify Base Forms description"));
			buttonItems.Add(new ThingThatGetsAButton("Review",
													 "Review Long Label",
													 "Review description"));

			//            buttonItems.Add(new ThingThatGetsAButton(DashboardGroup.Share, "Print", ButtonStyle.IconVariableWidth, Addin..Properties.Resources.greenPrinter));
			//            buttonItems.Add(new ThingThatGetsAButton(DashboardGroup.Share, "Email", ButtonStyle.IconVariableWidth, CommonTools.Properties.Resources.greenEmail));
			//            buttonItems.Add(new ThingThatGetsAButton(DashboardGroup.Share, "Synchronize", ButtonStyle.IconVariableWidth, CommonTools.Properties.Resources.greenSynchronize));

			return buttonItems;
		}
	}

	internal class ThingThatGetsAButton : IThingOnDashboard
	{
		public ThingThatGetsAButton(string localizedLabel,
									string localizedLongLabel,
									string description,
									ButtonStyle style,
									Image image)
		{
			DashboardButtonImage = image;
			DashboardButtonStyle = style;
			LocalizedLabel = localizedLabel;
			LocalizedLongLabel = localizedLongLabel;
			Description = description;
			Font = new Font("Arial", 10);
		}

		public ThingThatGetsAButton(string localizedLabel,
									string localizedLongLabel,
									string description)
			: this(localizedLabel,
				localizedLongLabel,
				description,
				ButtonStyle.VariableAmount,
				null)
		{ }

		#region IThingOnDashboard Members

		public DashboardGroup Group => DashboardGroup.Describe;

		#endregion

		public string LocalizedLabel { get; }

		public string LocalizedLongLabel { get; }

		public string Description { get; }

		public Font Font { get; set; }

		public ButtonStyle DashboardButtonStyle { get; set; }

		#region IThingOnDashboard Members

		public Image DashboardButtonImage { get; }

		#endregion
	}
}
