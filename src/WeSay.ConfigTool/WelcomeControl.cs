using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Chorus.VcsDrivers.Mercurial;
using Palaso.UI.WindowsForms.i8n;
using WeSay.ConfigTool.Properties;

namespace WeSay.ConfigTool
{
	public partial class WelcomeControl: UserControl
	{
		private bool _keyNavigationInProgress=false;
		private ListViewGroup _openExistingGroup;
		private ListViewGroup _getProjectChorusGroup;
		private ListViewGroup _createProjectGroup;
		public event EventHandler NewProjectClicked;
		public event EventHandler NewProjectFromFlexClicked;
		public Action<string> OpenSpecifiedProject;
		private DateTime _lastListViewClick;
		public event EventHandler ChooseProjectClicked;

		public WelcomeControl()
		{
			Font = SystemFonts.MessageBoxFont;//use the default OS UI font
			InitializeComponent();
			listView1.Font = new Font(SystemFonts.MessageBoxFont.FontFamily, 11);//use the default OS UI font
			listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;

			AddGroups();
			AddOpenProjectChoices();
			AddOtherChoices();
		}

		private void AddOtherChoices()
		{
			var createNewProjectItem =
				new System.Windows.Forms.ListViewItem("Create new blank project");
			createNewProjectItem.Group = _createProjectGroup;
			createNewProjectItem.Tag =
				new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.createNewProject_LinkClicked);

			var createFromFLEXItem =
				new System.Windows.Forms.ListViewItem("Create new project from FLEx LIFT export");
			createFromFLEXItem.Group = _createProjectGroup;
			createFromFLEXItem.Tag =
				new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnCreateProjectFromFLEx_LinkClicked);


			var getFromUSBItem =
				new System.Windows.Forms.ListViewItem("Get From USB drive", "getFromUsb");
			getFromUSBItem.Group = _getProjectChorusGroup;
			getFromUSBItem.Tag = new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnGetFromUsb);
			getFromUSBItem.ToolTipText = "Get a project from a Chorus repository on a USB flash drive";

			var getFromInternetItem =
				new System.Windows.Forms.ListViewItem("Get from Internet");
			getFromInternetItem.Group = _getProjectChorusGroup;
			getFromInternetItem.Tag =
				new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnGetFromInternet);
			getFromInternetItem.ToolTipText =
				"Get a project from a Chorus repository which is hosted on the internet (e.g. public.languagedepot.org) and put it on this computer";

			if (!string.IsNullOrEmpty(HgRepository.GetEnvironmentReadinessMessage("en")))
			{
				getFromUSBItem.ForeColor = Color.Gray;
				getFromUSBItem.ToolTipText += "\r\n" + HgRepository.GetEnvironmentReadinessMessage("en");
				getFromInternetItem.ForeColor = Color.Gray;
				getFromInternetItem.ToolTipText += "\r\n" + HgRepository.GetEnvironmentReadinessMessage("en");
			}

			listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[]
											  {
												  createNewProjectItem,
												  createFromFLEXItem,
												  getFromUSBItem,
												  getFromInternetItem
											  });
		}

		private void AddGroups()
		{
			_openExistingGroup = new System.Windows.Forms.ListViewGroup("Open", System.Windows.Forms.HorizontalAlignment.Left);
			_createProjectGroup = new System.Windows.Forms.ListViewGroup("Create", System.Windows.Forms.HorizontalAlignment.Left);
			_getProjectChorusGroup = new System.Windows.Forms.ListViewGroup("Get Project (Chorus)", System.Windows.Forms.HorizontalAlignment.Left);

			_openExistingGroup.Header = "Open";
			_openExistingGroup.Name = "openExisting";
			_createProjectGroup.Header = "Create";
			_createProjectGroup.Name = "createProject";
			_getProjectChorusGroup.Header = "Get";
			_getProjectChorusGroup.Name = "getProjectChorus";

			this.listView1.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
																						_openExistingGroup,
																						_createProjectGroup,
																						_getProjectChorusGroup});
		}

		private void OnGetFromInternet(object sender, LinkLabelLinkClickedEventArgs e)
		{
		}

		private void OnGetFromUsb(object sender, LinkLabelLinkClickedEventArgs e)
		{
			var dlg = new Chorus.UI.Clone.GetCloneDialog(WeSay.Project.WeSayWordsProject.NewProjectDirectory);
			dlg.Model.ProjectFilter = dir => GetLooksLikeWeSayProject(dir);
			if(DialogResult.Cancel == dlg.ShowDialog())
				return;


			OpenSpecifiedProject(dlg.PathToNewProject);
		}

		private static bool GetLooksLikeWeSayProject(string directoryPath)
		{
			return Directory.GetFiles(directoryPath, "*.WeSayConfig").Length > 0;
		}

		private void AddOpenProjectChoices()
		{
			bool haveProcessedTopMostProject = false;
			int count = 0;
			foreach (string path in Settings.Default.MruConfigFilePaths.Paths)
			{
				var item = new ListViewItem(Path.GetFileNameWithoutExtension(path), "wesayProject",
																 _openExistingGroup);
				item.Tag = path;
			   // item.Font=new Font(listView1.Font,FontStyle.Bold);
				listView1.Items.Add(item);
				//item.Selected = true;
				++count;
				haveProcessedTopMostProject = true;
				if (count > 3)
					break;

			}

			var browseForProjectItem = new ListViewItem("Other...", "browse");
			browseForProjectItem.Group = _openExistingGroup;
			browseForProjectItem.Tag =
				new LinkLabelLinkClickedEventHandler(openDifferentProject_LinkClicked);
			listView1.Items.Add(browseForProjectItem);
		}

		private void openRecentProject_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (OpenSpecifiedProject != null)
			{
				OpenSpecifiedProject.Invoke(((LinkLabel) sender).Tag as string);
			}
		}

		private void openDifferentProject_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (ChooseProjectClicked != null)
			{
				ChooseProjectClicked.Invoke(this, null);
			}
		}

		private void createNewProject_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (NewProjectClicked != null)
			{
				NewProjectClicked.Invoke(this, null);
			}
		}

		private void WelcomeControl_Load(object sender, EventArgs e)
		{
//            if (flowLayoutPanel2.Controls.Count != 0)
//            {
//                flowLayoutPanel2.Controls[0].Focus();
//            }
//            else
//            {
//                openDifferentProject.Focus();
//            }
		}

		private void OnCreateProjectFromFLEx_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (NewProjectFromFlexClicked != null)
			{
				NewProjectFromFlexClicked.Invoke(this, null);
			}
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(_keyNavigationInProgress)
				return;

			_debounceListIndexChangedEvent.Enabled = true;
		}

		private void OnChooseItem()
		{
			if(listView1.SelectedItems.Count ==0)
				return;
			ListViewItem item = listView1.SelectedItems[0];
			if(item==null)
				return;
			listView1.SelectedItems.Clear();//don't leave them selected

			LinkLabelLinkClickedEventHandler handler = item.Tag as LinkLabelLinkClickedEventHandler;

			if (handler == null)
			{
				string path = item.Tag as string;
				if(path==null)
					return;
				if (OpenSpecifiedProject == null)
					return;
				OpenSpecifiedProject.Invoke(path);
				return;
			}

			handler.Invoke(this, null);
		}

		/// <summary>
		/// this lets us select items with the keyboard without acting like we clicked on them
		/// </summary>
		private void listView1_KeyDown(object sender, KeyEventArgs e)
		{
			_keyNavigationInProgress = true;
		}

		/// <summary>
		/// this lets us select items with the keyboard without acting like we clicked on them
		/// </summary>
		private void listView1_KeyUp(object sender, KeyEventArgs e)
		{
			_keyNavigationInProgress = false;
		}

		private void listView1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.Enter)
			{
				OnChooseItem();
				e.Handled = true;
			}

		}

		private void _debounceListIndexChangedEvent_Tick(object sender, EventArgs e)
		{
			_debounceListIndexChangedEvent.Enabled = false;
			OnChooseItem();
		}
	}
}