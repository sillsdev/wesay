using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autofac;
using Palaso.IO;
using Palaso.Reporting;
using Palaso.WritingSystems;
using WeSay.ConfigTool.NewProjectCreation;
using WeSay.ConfigTool.Properties;
using WeSay.ConfigTool.Tasks;
using WeSay.Project;

namespace WeSay.ConfigTool
{
	public partial class ConfigurationWindow: Form
	{
		private WelcomeControl _welcomePage;
		private SettingsControl _projectSettingsControl;
		private WeSayWordsProject _project;
		private bool _disableBackupAndChorusStuffForTests;

		/// <summary>
		/// Used to make a note that after we've closed down nicely, the user has requested that we run WeSay
		/// </summary>
		private bool _openWeSayAfterSaving;

		/// <summary>
		/// Used to temporarily store the path while the project closes down, but we still need to know the path
		/// so we can ask WeSay to open it
		/// </summary>
		private string _pathToOpenWeSay;

		public ConfigurationWindow(string[] args)
		{
			InitializeComponent();
			//For now, Gecko boxes are only valid from the WeSay App, not the config tool
			WeSayWordsProject.GeckoOption = false;

			_helpProvider.RegisterPrimaryHelpFileMapping("wesay.helpmap");
			_helpProvider.RegisterSecondaryHelpMapping("chorus.helpmap");

			Project = null;

			//            if (this.DesignMode)
			//                return;
			//
			InstallWelcomePage();
			UpdateEnabledStates();

			if (args.Length > 0)
			{
				OpenProject(args[0].Trim(new[] {'"'}), false);
			}

			if (!DesignMode)
			{
				UpdateWindowCaption();
			}
		}

		private WeSayWordsProject Project
		{
			get { return _project; }
			set
			{
				_project = value;
				UpdateEnabledStates();
			}
		}

		// This delegate enables asynchronous calls for setting
		// the properties from another thread.
		private delegate void UpdateStuffCallback();

		private void UpdateEnabledStates()
		{
			if (toolStrip2.InvokeRequired)
			{
				UpdateStuffCallback d = UpdateEnabledStates;
				Invoke(d, new object[] {});
			}
			else
			{
				toolStrip2.Visible = (_project != null);

//                _saveACopyForFLEx54ToolStripMenuItem.Enabled = (_project != null);
//                openProjectInWeSayToolStripMenuItem.Enabled = (_project != null);
			}
		}

		private void OnChooseProject(object sender, EventArgs e)
		{
			var dlg = new OpenFileDialog();
			dlg.Title = "Open WeSay Project...";
			dlg.DefaultExt = ".WeSayConfig";
			dlg.Filter = "WeSay Configuration File (*.WeSayConfig)|*.WeSayConfig";
			dlg.Multiselect = false;
			dlg.InitialDirectory = GetInitialDirectory();
			if (DialogResult.OK != dlg.ShowDialog(this))
			{
				return;
			}
			SaveAndDisposeProject();
			OnOpenProject(dlg.FileName, false);
		}

		private static string GetInitialDirectory()
		{
			string initialDirectory = null;
			string latestProject = Settings.Default.MruConfigFilePaths.Latest;
			if (!String.IsNullOrEmpty(latestProject))
			{
				Debug.Assert(File.Exists(latestProject));
				try
				{
					initialDirectory = Path.GetDirectoryName(latestProject);
				}
				catch
				{
					//swallow

					//esa 2008-06-09 Why do we have this catch?
				}
			}

			if (string.IsNullOrEmpty(initialDirectory))
			{
				initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			}
			return initialDirectory;
		}

		public void OnOpenProject(string path, bool newClone)
		{
			if (!File.Exists(path) && !Directory.Exists(path))
			{
				ErrorReport.NotifyUserOfProblem(
						"WeSay could not find the file at {0} anymore.  Maybe it was moved or renamed?",
						path);
				return;
			}

			OpenProject(path, newClone);
		}

		private void OnCreateProject(object sender, EventArgs e)
		{
			var dlg = new NewProjectDialog(WeSay.Project.WeSayWordsProject.NewProjectDirectory);
			if (DialogResult.OK != dlg.ShowDialog())
			{
				return;
			}
			CreateAndOpenProject(dlg.PathToNewProjectDirectory, dlg.Iso639Code, dlg.LanguageName);

			PointOutOpenWeSayButton();
		}

		private void PointOutOpenWeSayButton()
		{
//            try
//            {
//                var effects = new BigMansStuff.LocusEffects.LocusEffectsProvider();
//                {
//                    effects.Initialize();
//                    effects.ShowLocusEffect(this, RectangleToScreen(this.openProjectInWeSayToolStripMenuItem.Bounds), LocusEffectsProvider.DefaultLocusEffectArrow);
//                }
//            }
//            catch (Exception e)
//            {
//            }
		}

		private void OnCreateProjectFromFLEx(object sender, EventArgs e)
		{
			var dlg = new NewProjectFromRawLiftDialog();
			if (DialogResult.OK != dlg.ShowDialog())
			{
				return;
			}

			Logger.WriteEvent("Attempting create new project from FLEx Export...");

			if (ProjectFromRawFLExLiftFilesCreator.Create(dlg.PathToNewProjectDirectory, dlg.PathToLift))
			{
				if (OpenProject(dlg.PathToNewProjectDirectory, true))
				{
					using (var info = new NewProjectInformationDialog(dlg.PathToNewProjectDirectory))
					{
						info.ShowDialog();
					}
				}
			}
			if (_project != null)
			{
				var logger = _project.Container.Resolve<ILogger>();
				logger.WriteConciseHistoricalEvent("Created New Project From FLEx Export");

			}


		}

		public void CreateAndOpenProject(string directoryPath, string languageTag, string langName)
		{
			//TODO: This method should have another argument for the language name.
			//the "wesay" part may not exist yet
			if (!Directory.GetParent(directoryPath).Exists)
			{
				Directory.GetParent(directoryPath).Create();
			}

			CreateNewProject(directoryPath);
			OpenProject(directoryPath, false);

			if (!Project.WritingSystems.Contains(languageTag))
			{
				var genericWritingSystemShippedWithWs = Project.WritingSystems.Get("qaa-x-qaa");
				genericWritingSystemShippedWithWs.Language = languageTag;
				genericWritingSystemShippedWithWs.LanguageName = langName;
				if (genericWritingSystemShippedWithWs.Language == WellKnownSubTags.Unlisted.Language)
				{
					//this is to accomodate Flex which expects to have a custom language tag
					//as the first private use subtag when the language subtag is qaa
					var langTag = TrimLanguageNameForTag(langName);
					genericWritingSystemShippedWithWs.Variant = "x-" + langTag;	// replace x-qaa
				}
				else
				{
					genericWritingSystemShippedWithWs.Variant = ""; //remove x-qaa
				}
				Project.WritingSystems.Set(genericWritingSystemShippedWithWs);
				Project.WritingSystems.Save();
			}

			 if(_project != null)
			 {
				 var logger = _project.Container.Resolve<ILogger>();
				 logger.WriteConciseHistoricalEvent("Created New Project");


				 if (ErrorReport.IsOkToInteractWithUser)
				 {
					 using (var dlg = new NewProjectInformationDialog(directoryPath))
					 {
						 dlg.ShowDialog();
					 }
				 }

			 }
		}

		/// <summary>
		/// Trim the language name to three lowercase English letters for the fake language tag.
		/// (Append 'x' as needed to ensure three English letters.)
		/// </summary>
		private static string TrimLanguageNameForTag(string languageName)
		{
			var languageTag = System.Text.RegularExpressions.Regex.Replace(languageName, @"[^a-zA-Z]", "") + @"xxx";
			return languageTag.Substring(0, 3).ToLowerInvariant();
		}

		private static void CreateNewProject(string directoryPath)
		{
			try
			{
				WeSayWordsProject.CreateEmptyProjectFiles(directoryPath);
			}
			catch (Exception e)
			{
				ErrorReport.NotifyUserOfProblem(
						"WeSay was not able to create a project there. \r\n" + e.Message);
				return;
			}
		}

		private bool AskAboutGlossMeaning(bool newClone, string newlycreatedfromFLExPath, string liftPath)
		{
			if (newClone && Project.CreatedByFLEx(liftPath) && !File.Exists(newlycreatedfromFLExPath))
			{
				DialogResult dialogResult = MessageBox.Show("This project has been created by FLEx and has just been received. Do you want to use gloss as the meaning field (it is recommended for a mew project from FLEx)?", "Created by FLEx", MessageBoxButtons.YesNo);
				if (dialogResult == DialogResult.Yes)
				{
					File.Create(newlycreatedfromFLExPath);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns>true if the project was sucessfully opend</returns>
		public bool OpenProject(string path, bool newClone)
		{
			Logger.WriteEvent("OpenProject("+path+")");
			//System.Configuration.ConfigurationManager.AppSettings["LastConfigFilePath"] = path;

			//strip off any trailing '\'
			if (path[path.Length - 1] == Path.DirectorySeparatorChar ||
				path[path.Length - 1] == Path.AltDirectorySeparatorChar)
			{
				path = path.Substring(0, path.Length - 1);
			}

			try
			{
				Project = new WeSayWordsProject();

				//just open the accompanying lift file.
				path = path.Replace(".WeSayConfig", ".lift");
				string newlycreatedfromFLExPath = Path.Combine(Path.GetFullPath(path), ".newlycreatedfromFLEx");

				if (path.Contains(".lift"))
				{
					path = Project.UpdateFileStructure(path);

					AskAboutGlossMeaning(newClone, newlycreatedfromFLExPath, path);

					if (!Project.LoadFromLiftLexiconPath(path))
					{
						Project = null;
						return false;
					}
				}
						//                else if (path.Contains(".WeSayConfig"))
						//                {
						//                    this.Project.LoadFromConfigFilePath(path);
						//                }
				else if (Directory.Exists(path))
				{
					foreach (string file in Directory.EnumerateFiles(path, "*.lift"))
					{
						AskAboutGlossMeaning(newClone, newlycreatedfromFLExPath, file);
						continue;
					}
					Project.LoadFromProjectDirectoryPath(path);
					if (_project.Container == null)
					{
						// There must not have been a .lift file in the given path.
						// This has already been reported with an error dialog box.
						_project.Dispose();
						Project = null;
						return false;
					}
				}
				else
				{
					throw new ApplicationException(path +
												   " is not named as a .lift file or .WeSayConfig file.");
				}
				if (_disableBackupAndChorusStuffForTests)
				{
					_project.BackupMaker = null;
				}
			}
			catch (ConfigurationFileTooNewException e)
			{
				Project = null;
				ErrorReport.NotifyUserOfProblem(e.Message);
				return false;
			}
			catch (Exception e)
			{
				Project = null;
				ErrorReport.NotifyUserOfProblem(e, "WeSay was not able to open that project." + e.Message);
				return false;
			}

			SetupProjectControls(BuildInnerContainerForThisProject());
			if (File.Exists(Path.Combine(_project.ProjectDirectoryPath, ".newlycreatedfromFLEx")))
			{
				_project.MakeMeaningFieldChange("definition", "gloss");
				_project.Save();
				SetupProjectControls(BuildInnerContainerForThisProject()); // reload to get meaning field change in gui
				File.Delete(Path.Combine(_project.ProjectDirectoryPath, ".newlycreatedfromFLEx"));
			}
			Settings.Default.MruConfigFilePaths.AddNewPath(Project.PathToConfigFile);
			return true;
		}

		private ILifetimeScope BuildInnerContainerForThisProject()
		{
			var scope = _project.Container.BeginLifetimeScope(containerBuilder =>
								{
									containerBuilder.RegisterType(
										typeof (TaskListView));
									containerBuilder.RegisterType(
										typeof (TaskListPresentationModel));

									containerBuilder
										.RegisterType
										<MissingInfoTaskConfigControl>()
										.InstancePerDependency();

									//      autofac's generated factory stuff wasn't working with our version of autofac, so
									//  i abandoned this
									//containerBuilder.Register<Control>().FactoryScoped();
									// containerBuilder.RegisterGeneratedFactory<ConfigTaskControlFactory>(new TypedService(typeof (Control)));

									containerBuilder.RegisterType<FieldsControl>();
									containerBuilder
										.RegisterType<WritingSystemSetup>();
									containerBuilder.RegisterType<FieldsControl>();
									containerBuilder
										.RegisterType<InterfaceLanguageControl>();
									containerBuilder.RegisterType<ActionsControl>();
									containerBuilder
										.RegisterType<BackupPlanControl>();
									//containerBuilder.Register<ChorusControl>();
									containerBuilder
										.RegisterType<OptionListControl>();
									// make the context itself available for pushing into contructors);
								});
			return scope;
		}


		private void SetupProjectControls(IComponentContext context)
		{
			UpdateWindowCaption();
			RemoveExistingControls();
			InstallProjectsControls(context);
		}

		private void UpdateWindowCaption()
		{
			string projectName = "";
			if (Project != null)
			{
				projectName = Project.Name;
			}
			Text = String.Format(
				"{0} {1}: {2}",
				"WeSay Configuration Tool",
				BasilProject.VersionString,
				projectName
			);

			_versionToolStripLabel.Text = String.Format(
				"{0} {1}",
				"WeSay Configuration Tool",
				BasilProject.VersionString
			);
		}

		private void InstallWelcomePage()
		{
			_welcomePage = new WelcomeControl();
			Controls.Add(_welcomePage);
			_welcomePage.BringToFront();
			_welcomePage.Dock = DockStyle.Fill;
			_welcomePage.NewProjectClicked += OnCreateProject;
			_welcomePage.NewProjectFromFlexClicked += OnCreateProjectFromFLEx;
			_welcomePage.OpenSpecifiedProject += OnOpenProject;
			_welcomePage.ChooseProjectClicked += OnChooseProject;
		}



		private void InstallProjectsControls(IComponentContext context)
		{
			_projectSettingsControl = new SettingsControl(context);
			Controls.Add(_projectSettingsControl);
			_projectSettingsControl.Dock = DockStyle.Fill;
			_projectSettingsControl.BringToFront();
			_projectSettingsControl.Focus();
		}

		private void RemoveExistingControls()
		{
			if (_welcomePage != null)
			{
				Controls.Remove(_welcomePage);
				_welcomePage.Dispose();
				_welcomePage = null;
			}
			if (_projectSettingsControl != null)
			{
				Controls.Remove(_projectSettingsControl);
				_projectSettingsControl.Dispose();
				_projectSettingsControl = null;
			}
		}

		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			// Fix WS-34029. If the project settings control has the focus while we close,
			// their leave control events may try and use a disposed project resource.
			// So we set the focus to the form first, causing the focussed control to lose focus.
			Focus();

			SaveAndDisposeProject();
			if(_openWeSayAfterSaving)
			{
				RunWeSay();
			}
		}

		private void SaveAndDisposeProject()
		{
			try
			{
				if (Project != null)
				{
					Project.Save();
				}

				Settings.Default.Save();
				if (_projectSettingsControl != null)
				{
					_projectSettingsControl.Dispose();
				}
				if (Project != null)
				{
					_project.Dispose();
				}
			}
			catch (Exception error)
			{
				//would make it impossible to quit. e.Cancel = true;
				ErrorReport.NotifyUserOfProblem(error.Message);
			}
			Project = null;
		}

		private void OnOpenThisProjectInWeSay(object sender, EventArgs e)
		{
			_pathToOpenWeSay = _project.PathToLiftFile;//this will get cleared as the config tool cleans up
			_openWeSayAfterSaving = true;
			Close();
		}

		private void RunWeSay()
		{
			string dir = Directory.GetParent(Application.ExecutablePath).FullName;
			var startInfo = new ProcessStartInfo(
				Path.Combine(dir, "WeSay.App.exe"),
				string.Format(" -launchedByConfigTool \"{0}\"", _pathToOpenWeSay)
			);

			Process.Start(startInfo);
		}

		private void OnExit_Click(object sender, EventArgs e)
		{
			Close();
		}

		public void DisableBackupAndChorusStuffForTests()
		{
			_disableBackupAndChorusStuffForTests = true;
		}

		private void OnAboutToolStrip_Click(object sender, EventArgs e)
		{
			string aboutPath = Path.Combine(WeSayWordsProject.ApplicationCommonDirectory, "aboutBox.htm");
			using (var dlg = new Palaso.UI.WindowsForms.SIL.SILAboutBox(aboutPath))
			{
				dlg.ShowDialog();
			}
		}

		private void OnHelpToolStrip_Click(object sender, EventArgs e)
		{
			Program.ShowHelpTopic("");
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			//if (e.KeyCode == Keys.F1) This key is now handled by the HelpProvider
		}
	}
}
