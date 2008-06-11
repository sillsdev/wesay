using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.i8n;
using WeSay.AddinLib;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Dashboard;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.CommonTools
{

	public partial class Dash : UserControl, ITask, IFinishCacheSetup
	{
		private readonly LexEntryRepository _recordListManager;
		private int _standardButtonWidth;
		private IList<IThingOnDashboard> _thingsToMakeButtonsFor;
		private List<ButtonGroup> _buttonGroups;
		private bool _isActive=false;
		private readonly ICurrentWorkTask _currentWorkTaskProvider;

		public Dash(LexEntryRepository RecordListManager, ICurrentWorkTask currentWorkTaskProvider)
		{
			_recordListManager = RecordListManager;
			_currentWorkTaskProvider = currentWorkTaskProvider;
		}

		private int DetermineStandardButtonWidth()
		{
			int maxRequestedWidth = 30;
			foreach (IThingOnDashboard item in ThingsToMakeButtonsFor)
			{
				int w = 100;// item.WidthToDisplayFullSizeLabel;
				if(w > maxRequestedWidth)
					maxRequestedWidth = w;
			}
			return maxRequestedWidth;
		}

		private void Fill()
		{
			DictionaryStatusControl title = new DictionaryStatusControl(_recordListManager.GetListOfType<LexEntry>());
			title.Font = new Font("Arial", 14);
			title.BackColor = this.BackColor;
			title.ShowLogo = true;
			_flow.Controls.Add(title);

			_standardButtonWidth = DetermineStandardButtonWidth();
			_standardButtonWidth += 30;//for space between text and button

			foreach (ButtonGroup group in _buttonGroups)
			{
				if (!group.MakeButtonsSameSize)
				{
					AddButtonGroup(group, 0);
				}
				else
				{
					AddButtonGroup(group, _standardButtonWidth);
				}
			}
		 }

//        private IEnumerable<string> GetGroups()
//        {
//            List<string> foundGroups = new List<string>();
//            foreach (IThingOnDashboard item in _thingsToMakeButtonsFor)
//            {
//                if (!foundGroups.Contains(item.GroupName))
//                {
//                    foundGroups.Add(item.GroupName);
//                    yield return item.GroupName;
//                }
//            }
//        }

		private void AddButtonGroup(ButtonGroup buttonGroup, int buttonWidth)
		{
			FlowLayoutPanel buttonFlow = new FlowLayoutPanel();
			buttonFlow.AutoSize = true;
			buttonFlow.FlowDirection = FlowDirection.LeftToRight;
		   // buttonGroup.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			buttonFlow.Margin =  new Padding(30,0,0,15);
			bool foundAtLeastOne = false;
			foreach (IThingOnDashboard item in ThingsToMakeButtonsFor)
			{
				if (item == this)
					continue;
				if (item.Group == buttonGroup.Group)
				{
					buttonFlow.Controls.Add(MakeButton(item, buttonWidth, buttonGroup));
					foundAtLeastOne = true;
				}
			}
			if (foundAtLeastOne)
			{
				Label header = new Label();
				header.Text = StringCatalog.Get(buttonGroup.Group.ToString());
				header.Font = new Font("Arial", 12);
				_flow.Controls.Add(header);
				_flow.Controls.Add(buttonFlow);
			}
		}

		private Control MakeButton(IThingOnDashboard item, int buttonWidth, ButtonGroup group)
		{
			DashboardButton button = MakeButton(item);
			button.BackColor = this.BackColor;
			button.Font = this.Font;
			button.AutoSize = false;
			button.BorderColor = group.BorderColor;
			button.DoneColor = group.DoneColor;
			button.TodoColor = group.TodoColor;
//            if (buttonWidth == 0)
//                buttonWidth = item.WidthToDisplayFullSizeLabel;

			button.Size = new Size(buttonWidth, 40);
			button.Text = item.LocalizedLabel;
			button.Click += new EventHandler(OnButtonClick);
			return button;
		}

		void OnButtonClick(object sender, EventArgs e)
		{
			DashboardButton b = (DashboardButton)sender;
			ITask task = b.ThingToShowOnDashboard as ITask;
			if (task != null && _currentWorkTaskProvider != null)
			{
				_currentWorkTaskProvider.ActiveTask = task;
			}
			else
			{
				IWeSayAddin addin = b.ThingToShowOnDashboard as IWeSayAddin;
				if (addin != null)
				{
					Cursor.Current = Cursors.WaitCursor;

					try
					{
						ProjectInfo projectInfo = WeSay.Project.WeSayWordsProject.Project.GetProjectInfoForAddin();
						addin.Launch(this.ParentForm, projectInfo);
					}
					catch (Exception error)
					{
						Palaso.Reporting.ErrorReport.ReportNonFatalMessage(error.Message);
					}

					Cursor.Current = Cursors.Default;

				}
			}
		}


		public DashboardButton MakeButton(IThingOnDashboard item)
		{
			switch (item.DashboardButtonStyle)
			{
				case ButtonStyle.FixedAmount:
					return new DashboardButton(item);
				case ButtonStyle.VariableAmount:
					return new DashboardButton(item);
				case ButtonStyle.IconFixedWidth:
					return new DashboardButtonWithIcon(item);
				case ButtonStyle.IconVariableWidth:
					return new DashboardButtonWithIcon(item);
				default:
					return new DashboardButton(item);
			}
		}


		#region ITask Members

		const int CountNotRelevant = -1;
		public int ReferenceCount
		{
			get { return CountNotRelevant; }
		}

		public bool IsPinned
		{
			get { return true; }
		}

		public int Count
		{
			get { return CountNotRelevant; }
		}

		public int ExactCount
		{
			get { return CountNotRelevant; }
		}

		public void Activate()
		{
			if (IsActive)
			{
				throw new InvalidOperationException("Activate should not be called when object is active.");
			}

			Initialize();
			SuspendLayout();
			this.Dock = DockStyle.Fill;
			this.AutoScroll = true;
			if (ThingsToMakeButtonsFor == null)
			{
				ThingsToMakeButtonsFor = new List<IThingOnDashboard>();
				foreach (ITask task in Project.WeSayWordsProject.Project.Tasks)
				{
					ThingsToMakeButtonsFor.Add(task);
				}
				foreach (IThingOnDashboard action in AddinSet.GetAddinsForUser())
				{
					ThingsToMakeButtonsFor.Add(action);
				}
			}


			Fill();
			ResumeLayout(true);
			_isActive = true;
		}

		private void Initialize()
		{
			InitializeComponent();
			this.BackColor = _flow.BackColor;

			//_flow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

			_buttonGroups = new List<ButtonGroup>();
			_buttonGroups.Add(new ButtonGroup(DashboardGroup.Gather, true,
											  Color.FromArgb(155, 187, 89),
											  Color.FromArgb(195, 214, 155),
											  Color.FromArgb(235, 241, 222)));
			_buttonGroups.Add(new ButtonGroup(DashboardGroup.Describe, true, Color.FromArgb(85, 142, 213),
											  Color.FromArgb(185, 205, 229),
											  Color.White));
			_buttonGroups.Add(new ButtonGroup(DashboardGroup.Refine, true, Color.FromArgb(250, 192, 144),
											  Color.FromArgb(252, 213, 181),
											  Color.White));
			_buttonGroups.Add(new ButtonGroup(DashboardGroup.Share, true, Color.FromArgb(119, 147, 60),
											  Color.White,
											  Color.White));

			LocalizationHelper helper = new LocalizationHelper(null);
			helper.Parent = this;
			helper.EndInit();
		}

		public void Deactivate()
		{
			if (!IsActive)
			{
				throw new InvalidOperationException("Deactivate should only be called once after Activate.");
			}
			Controls.Clear();
			_isActive = false;
		}

		public void GoToUrl(string url)
		{
		}

		public bool IsActive
		{
			get { return _isActive; }
		}

		public string Label
		{
			get
			{
				return
						StringCatalog.Get("~Home",
										  "The label for the 'dashboard'; the task which lets you see the status of other tasks and jump to them.");
			}
		}

		public string Description
		{
			get { return StringCatalog.Get("~Switch tasks and see current status of tasks"); }
		}

		public bool MustBeActivatedDuringPreCache
		{
			get { return false; }
		}

		public void RegisterWithCache(ViewTemplate viewTemplate)
		{

		}

		public Control Control
		{
			get { return this; }
		}

		#region IThingOnDashboard Members

		public DashboardGroup Group
		{
			get { return DashboardGroup.DontShow; }
		}

		public string LocalizedLabel
		{
			get { throw new NotImplementedException(); }
		}

		public ButtonStyle DashboardButtonStyle
		{
			get { throw new NotImplementedException(); }
		}

		public Image DashboardButtonImage
		{
			get { throw new NotImplementedException(); }
		}

		public IList<IThingOnDashboard> ThingsToMakeButtonsFor
		{
			get { return _thingsToMakeButtonsFor; }
			set { _thingsToMakeButtonsFor = value; }
		}

		#endregion

		#endregion

		#region IFinishCacheSetup Members

		public void FinishCacheSetup()
		{
			Activate();
			Deactivate();
		}

		#endregion
	}





	internal class ButtonGroup
	{
		private readonly DashboardGroup _group;
		private readonly bool _makeButtonsSameSize;
		private Color _doneColor;
		private Color _borderColor;
		private Color _todoColor;

		public ButtonGroup(DashboardGroup group, bool makeButtonsSameSize, Color borderColor, Color doneColor, Color todoColor)
		{
			_group = group;
			_makeButtonsSameSize = makeButtonsSameSize;
			_borderColor = borderColor;
			_doneColor = doneColor;
			_todoColor = todoColor;
		}

		public bool MakeButtonsSameSize
		{
			get { return _makeButtonsSameSize; }
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

		public DashboardGroup Group
		{
			get { return _group; }
		}
	}
}
