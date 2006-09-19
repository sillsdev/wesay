using WeSay.UI;
using System.Windows.Forms;

namespace WeSay.App
{
	public partial class TabbedForm : Form
	{
		private ITask _currentTool;
		private BasilProject _project;

		public TabbedForm(BasilProject project, ITaskBuilder taskBuilder)
		{
			InitializeComponent();

			_project = project;

			this.tabControl1.TabPages.Clear();

			this.tabControl1.SelectedIndexChanged += new System.EventHandler(tabControl1_SelectedIndexChanged);

			foreach (ITask t in taskBuilder.Tasks)
			{
				//t.Container = container;
				TabPage page = new TabPage(StringCatalog.Get(t.Label));
				page.Tag = t;
				this.tabControl1.TabPages.Add(page);
			}

			ActivateTab(this.tabControl1.SelectedTab);
		}

		void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			TabPage page = ((TabControl)sender).SelectedTab;
			ActivateTab(page);
		}

		private void ActivateTab(TabPage page)
		{
			ITask t = (ITask)page.Tag;
			if (_currentTool == t)
				return; //debounce

			if (_currentTool != null)
				_currentTool.Deactivate();
			if (t != null)
			{
				t.Activate();
				t.Control.Dock = DockStyle.Fill;
				page.Controls.Add(t.Control);
			}
			_currentTool = t;
		}

	}
}