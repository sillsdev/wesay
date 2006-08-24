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
				TabPage page = new TabPage(t.Label);
				page.Tag = t;
				t.Control.Dock = DockStyle.Fill;

				page.Controls.Add(t.Control);
				this.tabControl1.TabPages.Add(page);
			}
		}

		void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ITask t = (ITask)((TabControl)sender).SelectedTab.Tag;
			if (_currentTool == t)
				return; //debounce

			if (_currentTool != null)
				_currentTool.Deactivate();
			if (t != null)
				t.Activate();
			_currentTool = t;
		}
	}
}