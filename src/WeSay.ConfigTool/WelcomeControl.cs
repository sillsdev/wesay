using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Chorus.UI.Clone;
using Palaso.Extensions;
using Palaso.i18n;
using Palaso.IO;
using WeSay.ConfigTool.Properties;
using System.Collections.Generic;

namespace WeSay.ConfigTool
{
	public partial class WelcomeControl: UserControl
	{
		public event EventHandler NewProjectClicked;
		public event EventHandler NewProjectFromFlexClicked;
		public Action<string> OpenSpecifiedProject;
		public event EventHandler ChooseProjectClicked;

		public WelcomeControl()
		{
			Font = SystemFonts.MessageBoxFont;//use the default OS UI font
			InitializeComponent();
		  }

		private void LoadButtons()
		{
			flowLayoutPanel1.Controls.Clear();
			var createAndGetGroup = new TableLayoutPanel();
			createAndGetGroup.AutoSize = true;
			AddCreateChoices(createAndGetGroup);
			AddGetChoices(createAndGetGroup);

			var openChoices = new TableLayoutPanel();
			openChoices.AutoSize = true;
			AddSection("Open", openChoices);
			AddOpenProjectChoices(openChoices);
			flowLayoutPanel1.Controls.AddRange(new Control[] { createAndGetGroup, openChoices });
		}

		private void AddSection(string sectionName, TableLayoutPanel panel)
		{
			 panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			panel.RowCount++;
			var label = new Label();
			label.Font = new Font(StringCatalog.LabelFont.FontFamily, _templateLabel.Font.Size, _templateLabel.Font.Style);
			label.ForeColor = _templateLabel.ForeColor;
			label.Text = sectionName;
			label.Margin = new Padding(0, 20, 0, 0);
			panel.Controls.Add(label);
		}

		private void AddFileChoice(string path, TableLayoutPanel panel)
		{
			var button = AddChoice(Path.GetFileNameWithoutExtension(path), path, "wesayProject", true, openRecentProject_LinkClicked, panel);
			button.Tag = path;
		}


		private Button AddChoice(string localizedLabel, string localizedTooltip, string imageKey, bool enabled,
   EventHandler clickHandler, TableLayoutPanel panel)
		{
			panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			panel.RowCount++;
			var button = new Button();
			button.Anchor = AnchorStyles.Top | AnchorStyles.Left;

			button.Width = _templateButton.Width;//review
			button.Font = new Font(StringCatalog.LabelFont.FontFamily, _templateButton.Font.Size, _templateButton.Font.Style);

			button.ImageAlign = ContentAlignment.MiddleLeft;
			button.ImageKey = imageKey;
			button.ImageList = _imageList;
			button.Click += clickHandler;
			button.Text = "  "+localizedLabel;

			button.FlatAppearance.BorderSize = _templateButton.FlatAppearance.BorderSize;
			button.FlatStyle = _templateButton.FlatStyle;
			button.ImageAlign = _templateButton.ImageAlign;
			button.TextImageRelation = _templateButton.TextImageRelation ;
			button.UseVisualStyleBackColor = _templateButton.UseVisualStyleBackColor;
			button.Enabled = enabled;

			toolTip1.SetToolTip(button, localizedTooltip);
			panel.Controls.Add(button);
			return button;
		}

		private void AddCreateChoices(TableLayoutPanel panel)
		{
			AddSection("Create", panel);
			AddChoice("Create new blank project", string.Empty, "newProject", true, createNewProject_LinkClicked, panel);
			AddChoice("Create new project from a set of plain LIFT files","Use this after using SOLID to convert Toolbox Files to LIFT", "solid", true, OnCreateProjectFromPlainLift_LinkClicked, panel);
		}

		private void AddGetChoices(TableLayoutPanel panel)
		{
			AddSection("Get", panel);
			//nb: we want these always enabled, so that we can give a message explaining about hg if needed

			var usbButton = AddChoice("Get from USB drive", "Get a project from a Chorus repository on a USB flash drive, created by either WeSay or FLEx (using LiftBridge)", "getFromUsb", true, OnGetFromUsb, panel);
			var internetButton = AddChoice("Get from Internet", "Get a project from a Chorus repository which is hosted on the internet (e.g. public.languagedepot.org) and put it on this computer",
				"getFromInternet", true, OnGetFromInternet, panel);
			var hubButton = AddChoice("Get from ChorusHub", "Get a project from a Chorus repository managed by a ChorusHub server on the local network.", "getFromChorusHub", true, OnGetFromChorusHub, panel);
			if (!string.IsNullOrEmpty(Chorus.VcsDrivers.Mercurial.HgRepository.GetEnvironmentReadinessMessage("en")))
			{
				usbButton.ForeColor = Color.Gray;
				internetButton.ForeColor = Color.Gray;
				hubButton.ForeColor = Color.Gray;
			}
		}

		private void OnGetFromInternet(object sender, EventArgs e)
		{
//            if (!Chorus.UI.Misc.ReadinessDialog.ChorusIsReady)
//            {
//                using (var dlg = new Chorus.UI.Misc.ReadinessDialog())
//                {
//                    dlg.ShowDialog();
//                    return;
//                }
//            }
			if (!Directory.Exists(Project.WeSayWordsProject.NewProjectDirectory))
			{
				//e.g. mydocuments/wesay
				Directory.CreateDirectory(Project.WeSayWordsProject.NewProjectDirectory);
			}
			using (var dlg = new Chorus.UI.Clone.GetCloneFromInternetDialog(Project.WeSayWordsProject.NewProjectDirectory))
			{
				if (DialogResult.Cancel == dlg.ShowDialog())
					return;
				OpenSpecifiedProject(dlg.PathToNewlyClonedFolder);
				if (Project.WeSayWordsProject.ProjectExists)
					Project.WeSayWordsProject.Project.SetupUserForChorus();
			}
		}

		private void OnGetFromUsb(object sender, EventArgs e)
		{
//            if(!Chorus.UI.Misc.ReadinessDialog.ChorusIsReady)
//            {
//                using (var dlg = new Chorus.UI.Misc.ReadinessDialog())
//                {
//                    dlg.ShowDialog();
//                    return;
//                }
//            }
			if (!Directory.Exists(Project.WeSayWordsProject.NewProjectDirectory))
			{
				//e.g. mydocuments/wesay
				Directory.CreateDirectory(Project.WeSayWordsProject.NewProjectDirectory);
			}
			using (var dlg = new Chorus.UI.Clone.GetCloneFromUsbDialog(Project.WeSayWordsProject.NewProjectDirectory))
			{
				dlg.Model.ProjectFilter = dir => GetLooksLikeLiftChorusFolder(dir);
				if (DialogResult.Cancel == dlg.ShowDialog())
					return;
				OpenSpecifiedProject(dlg.PathToNewlyClonedFolder);
			}
		}

		private void OnGetFromChorusHub(object sender, EventArgs e)
		{
			if (!Directory.Exists(Project.WeSayWordsProject.NewProjectDirectory))
			{
				//e.g. mydocuments/wesay
				Directory.CreateDirectory(Project.WeSayWordsProject.NewProjectDirectory);
			}
			var existingProjectNames = new HashSet<string>(from dir in Directory.GetDirectories(Project.WeSayWordsProject.NewProjectDirectory) select Path.GetFileName(dir));
			Dictionary<string,string> existingRepositories;
			try
			{
				existingRepositories = GetSharedProjectModel.ExtantRepoIdentifiers(Project.WeSayWordsProject.NewProjectDirectory, "");
			}
			catch
			{
				existingRepositories = new Dictionary<string, string>();
			}
			var getCloneFromChorusHubModel = new GetCloneFromChorusHubModel(Project.WeSayWordsProject.NewProjectDirectory)
			{
				ProjectFilter = "*.lift",
				ExistingProjects = existingProjectNames,
				ExistingRepositoryIdentifiers = existingRepositories
			};
			using (var dlg = new Chorus.UI.Clone.GetCloneFromChorusHubDialog(getCloneFromChorusHubModel))
			{
				if (DialogResult.Cancel == dlg.ShowDialog())
					return;
				OpenSpecifiedProject(dlg.PathToNewlyClonedFolder);
				if (Project.WeSayWordsProject.ProjectExists)
					Project.WeSayWordsProject.Project.SetupUserForChorus();
			}
		}

		private static bool GetLooksLikeLiftChorusFolder(string directoryPath)
		{
			var hgDataFolder = directoryPath.CombineForPath(".hg", "store", "data");
			return Directory.Exists(hgDataFolder)
				&& Directory.GetFiles(hgDataFolder, "*.lift.i").Length > 0;
		}

		private void AddOpenProjectChoices(TableLayoutPanel panel)
		{
			int count = 0;
			foreach (string path in Settings.Default.MruConfigFilePaths.Paths)
			{
				AddFileChoice(path, panel);
				++count;
				if (count > 2)
					break;

			}
			AddChoice("Browse for other projects...", string.Empty, "browse", true, openDifferentProject_LinkClicked, panel);
		}

		private void openRecentProject_LinkClicked(object sender, EventArgs e)
		{
			if (OpenSpecifiedProject != null)
			{
				OpenSpecifiedProject.Invoke(((Button) sender).Tag as string);
			}
		}

		private void openDifferentProject_LinkClicked(object sender, EventArgs e)
		{
			if (ChooseProjectClicked != null)
			{
				ChooseProjectClicked.Invoke(this, null);
			}
		}

		private void createNewProject_LinkClicked(object sender, EventArgs e)
		{
			if (NewProjectClicked != null)
			{
				NewProjectClicked.Invoke(this, null);
			}
		}

		private void WelcomeControl_Load(object sender, EventArgs e)
		{
			LoadButtons();
		}

		private void OnCreateProjectFromPlainLift_LinkClicked(object sender, EventArgs e)
		{
			if (NewProjectFromFlexClicked != null)
			{
				NewProjectFromFlexClicked.Invoke(this, null);
			}
		}

	}
}