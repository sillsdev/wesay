using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.i8n;
using WeSay.AddinLib;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.UI;

namespace WeSay.CommonTools
{
	public partial class Dash : UserControl, ITask, IFinishCacheSetup
	{
		private DictionaryStatusControl _title;
		private readonly LexEntryRepository _lexEntryRepository;
		private IList<IThingOnDashboard> _thingsToMakeButtonsFor;
		private List<ButtonGroup> _buttonGroups;
		private bool _isActive = false;
		private readonly ICurrentWorkTask _currentWorkTaskProvider;
		private int _oldFlowWidth;
		private List<Size> _smallestPossibleButtonSizes;
		private Size _bestButtonSize;
		private bool _addedAllButtons = false;

		public Dash(LexEntryRepository RecordListManager, ICurrentWorkTask currentWorkTaskProvider)
		{
			_oldFlowWidth = 0;
			_lexEntryRepository = RecordListManager;
			_currentWorkTaskProvider = currentWorkTaskProvider;
			InitializeContextMenu();
		}

		private void InitializeContextMenu()
		{
			ContextMenu = new ContextMenu();
			ContextMenu.MenuItems.Add("Configure this project...", OnRunConfigureTool);
			ContextMenu.MenuItems.Add("Use projector-friendly colors", OnToggleColorScheme);
			ContextMenu.MenuItems[1].Checked = DisplaySettings.Default.UsingProjectorScheme;
		}

		private void OnToggleColorScheme(object sender, EventArgs e)
		{
			DisplaySettings.Default.ToggleColorScheme();
			ContextMenu.MenuItems[1].Checked = DisplaySettings.Default.UsingProjectorScheme;
			Invalidate(true);
		}

		private static void OnRunConfigureTool(object sender, EventArgs e)
		{
			string dir = Directory.GetParent(Application.ExecutablePath).FullName;
			ProcessStartInfo startInfo =
					new ProcessStartInfo(Path.Combine(dir, "WeSay Configuration Tool.exe"),
										 string.Format("\"{0}\"",
													   WeSayWordsProject.Project.PathToConfigFile));
			try
			{
				Process.Start(startInfo);
			}
			catch
			{
				ErrorReport.ReportNonFatalMessage("Could not start " + startInfo.FileName);
				return;
			}

			Application.Exit();
		}

		private void AddItemsToFlow()
		{
			_title = new DictionaryStatusControl(_lexEntryRepository.CountAllItems());
			_title.Font = new Font("Arial", 14);
			_title.BackColor = Color.Transparent;
			_title.ShowLogo = true;
			_title.Width = _flow.Width - _title.Margin.Left - _title.Margin.Right;
			_flow.Controls.Add(_title);

			foreach (ButtonGroup group in _buttonGroups)
			{
				AddButtonGroupToFlow(group);
			}
			_addedAllButtons = true;
		}

		private void AddButtonGroupToFlow(ButtonGroup buttonGroup)
		{
			FlowLayoutPanel buttonFlow = new FlowLayoutPanel();
			buttonFlow.AutoSize = true;
			buttonFlow.FlowDirection = FlowDirection.LeftToRight;
			buttonFlow.Margin = new Padding(30, 0, 0, 15);
			buttonFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			buttonFlow.WrapContents = true;
			bool foundAtLeastOne = false;
			foreach (IThingOnDashboard item in ThingsToMakeButtonsFor)
			{
				if (item == this)
				{
					continue;
				}
				if (item.Group == buttonGroup.Group)
				{
					buttonFlow.Controls.Add(MakeButton(item, buttonGroup));
					foundAtLeastOne = true;
				}
			}
			if (foundAtLeastOne)
			{
				Label header = new Label();
				header.AutoSize = true;
				header.Text = StringCatalog.Get(buttonGroup.Group.ToString());
				header.Font = new Font("Arial", 12);
				_flow.Controls.Add(header);
				_flow.Controls.Add(buttonFlow);
			}
		}

		private Control MakeButton(IThingOnDashboard item, ButtonGroup group)
		{
			DashboardButton button = MakeButton(item);
			button.BackColor = Color.Transparent;
			button.Font = Font;
			button.AutoSize = false;
			button.BorderColor = group.BorderColor;
			button.DoneColor = group.DoneColor;

			button.Size = _bestButtonSize;
			button.Text = item.LocalizedLabel;
			button.Click += OnButtonClick;
			return button;
		}

		private void OnButtonClick(object sender, EventArgs e)
		{
			DashboardButton b = (DashboardButton) sender;
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
						ProjectInfo projectInfo =
								WeSayWordsProject.Project.GetProjectInfoForAddin(_lexEntryRepository);
						addin.Launch(ParentForm, projectInfo);
					}
					catch (Exception error)
					{
						ErrorReport.ReportNonFatalMessage(error.Message);
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

		private void ResizeFlows()
		{
			_flow.SuspendLayout();
			foreach (Control control in _flow.Controls)
			{
				FlowLayoutPanel buttonGroup = control as FlowLayoutPanel;
				if (buttonGroup == null)
				{
					continue;
				}
				buttonGroup.MaximumSize =
						new Size(_flow.Width - buttonGroup.Margin.Left - buttonGroup.Margin.Right, 0);
			}
			_flow.Height = _flow.GetPreferredSize(new Size(_flow.Width, 0)).Height;
			_flow.ResumeLayout();
		}

		private void ResizeButtons()
		{
			_flow.SuspendLayout();
			foreach (Control buttonGroup in _flow.Controls)
			{
				foreach (Control control in buttonGroup.Controls)
				{
					DashboardButton button = control as DashboardButton;
					if (button == null)
					{
						continue;
					}
					button.Size = _bestButtonSize;
				}
			}
			_flow.ResumeLayout();
		}

		private IEnumerable<IEnumerable<Size>> GetAllPossibleButtonSizes()
		{
			List<IEnumerable<Size>> sizes = new List<IEnumerable<Size>>();
			foreach (Control buttonGroup in _flow.Controls)
			{
				foreach (Control buttonControl in buttonGroup.Controls)
				{
					DashboardButton button = buttonControl as DashboardButton;
					if (button == null)
					{
						continue;
					}
					sizes.Add(button.GetPossibleButtonSizes());
				}
			}
			return sizes;
		}

		private static int CompareSizesByHeightThenWidth(Size x, Size y)
		{
			int retval = x.Height.CompareTo(y.Height);
			if (retval == 0)
			{
				retval = x.Width.CompareTo(y.Width);
			}
			return retval;
		}

		private static int CompareSizesByWidthThenHeight(Size x, Size y)
		{
			int retval = x.Width.CompareTo(y.Width);
			if (retval == 0)
			{
				retval = x.Height.CompareTo(y.Height);
			}
			return retval;
		}

		private List<Size> SmallestPossibleButtonSizes
		{
			get
			{
				if (_smallestPossibleButtonSizes == null && _addedAllButtons)
				{
					_smallestPossibleButtonSizes = ComputeSmallestPossibleButtonSizes(GetAllPossibleButtonSizes());
				}
				return _smallestPossibleButtonSizes;
			}
		}

		internal static List<Size> ComputeSmallestPossibleButtonSizes(
			IEnumerable<IEnumerable<Size>> possibleSizesOfButtons)
		{
			List<Size> result = new List<Size>();
			Debug.Assert(possibleSizesOfButtons != null);   // per contract
			foreach (IEnumerable<Size> possibleSizesFor1Button in possibleSizesOfButtons)
			{
				result = MergeButtonSizes(result, possibleSizesFor1Button);
			}

			return result;
		}

		private static List<Size> MergeButtonSizes(IEnumerable<Size> sizeList1, IEnumerable<Size> sizeList2)
		{
			List<Size> result;
			if (sizeList1 == null && sizeList2 == null)
			{
				return null;
			}
			else if (sizeList1 == null || !sizeList1.GetEnumerator().MoveNext())
			{
				result = new List<Size>(sizeList2);
			}
			else if (sizeList2 == null || !sizeList2.GetEnumerator().MoveNext())
			{
				result = new List<Size>(sizeList1);
			}
			else
			{
				result = CombineButtonSizes(sizeList1, sizeList2);
			}

			RemoveDuplicateHeights(result);
			RemoveDuplicateWidths(result);
			return result;
		}

		private static void RemoveDuplicateHeights(List<Size> result)
		{
			RemoveDuplicates(result, CompareSizesByHeightThenWidth,
							 delegate(Size x, Size y) { return x.Height.Equals(y.Height); });
		}

		private static void RemoveDuplicateWidths(List<Size> result)
		{
			RemoveDuplicates(result, CompareSizesByWidthThenHeight,
							 delegate(Size x, Size y) { return x.Width.Equals(y.Width); });
		}

		private static void RemoveDuplicates(List<Size> result, Comparison<Size> sortComparer,
											 EqualityComparison<Size> equalComparer)
		{
			Size prevSize = new Size(int.MaxValue, int.MaxValue);
			result.Sort(sortComparer);
			int i = 0;
			while (i < result.Count)
			{
				if (equalComparer(result[i], prevSize))
				{
					result.RemoveAt(i);
				}
				else
				{
					prevSize = result[i];
					++i;
				}
			}
		}

		private static List<Size> CombineButtonSizes(IEnumerable<Size> sizeList1, IEnumerable<Size> sizeList2)
		{
			Debug.Assert(sizeList1 != null);
			Debug.Assert(sizeList2 != null);
			Debug.Assert(sizeList1.GetEnumerator().MoveNext());
				// per contract: both lists must contain at least one value
			Debug.Assert(sizeList2.GetEnumerator().MoveNext());
			List<Size> result = new List<Size>();

			// merge possibleSizesFor1Button and result into workingSizes
			foreach (Size size1 in sizeList1)
			{
				foreach (Size size2 in sizeList2)
				{
					result.Add(
						new Size(Math.Max(size1.Width, size2.Width), Math.Max(size1.Height, size2.Height)));
				}
			}
			Debug.Assert(result.Count != 0); // per contract
			return result;
		}

		private List<int> GetButtonsPerGroup()
		{
			if (_buttonGroups == null || ThingsToMakeButtonsFor == null)
			{
				return new List<int>();
			}
			Dictionary<DashboardGroup, int> buttonsPerGroup = new Dictionary<DashboardGroup, int>(_buttonGroups.Count);
			foreach (ButtonGroup group in _buttonGroups)
			{
				buttonsPerGroup.Add(group.Group, 0);
			}
			foreach (IThingOnDashboard item in ThingsToMakeButtonsFor)
			{
				if (item == this)
					continue;
				if (buttonsPerGroup.ContainsKey(item.Group))
				{
					++buttonsPerGroup[item.Group];
				}
			}
			List<int> buttonsPerGroupList = new List<int>(buttonsPerGroup.Count);
			foreach (KeyValuePair<DashboardGroup, int> pair in buttonsPerGroup)
			{
				if (pair.Value > 0)
					buttonsPerGroupList.Add(pair.Value);
			}
			return buttonsPerGroupList;
		}

		private Size GetBestButtonSize()
		{
			return
				ComputeBestButtonSize(SmallestPossibleButtonSizes, GetAvailableSpaceForButtons(), GetButtonsPerGroup());
		}

		internal static Size ComputeBestButtonSize(List<Size> smallestPossibleSizes, Size availableSpaceForButtons, IEnumerable<int> buttonsPerGroup)
		{
			if (smallestPossibleSizes == null || smallestPossibleSizes.Count == 0)
			{
				return Size.Empty;
			}

			List<Size> result = RemoveClippedButtonSizes(smallestPossibleSizes, availableSpaceForButtons.Width);
			result = RemoveScrolledButtonSizes(result, availableSpaceForButtons, buttonsPerGroup);
			return GetBestSizeBasedOnRatio(result);
		}

		private static Size GetBestSizeBasedOnRatio(IEnumerable<Size> possibleSizes)
		{
			Debug.Assert(possibleSizes != null); // per contract
			Debug.Assert(possibleSizes.GetEnumerator().MoveNext());
				// contract: possibleSizes must contain at least one size

			Size bestSize = Size.Empty;
			double bestRatio = double.PositiveInfinity;
			const double goldRatio = 4.0; // arbitrary ratio we think looks the best
			foreach (Size size in possibleSizes)
			{
				double ratio = (double) size.Width/size.Height;
				if (Math.Abs(ratio - goldRatio) <= Math.Abs(bestRatio - goldRatio))
				{
					bestRatio = ratio;
					bestSize = size;
				}
			}
			Debug.Assert(bestSize != Size.Empty); // per contract
			return bestSize;
		}

		private static List<Size> RemoveScrolledButtonSizes(IEnumerable<Size> possibleSizes, Size availableSpaceForButtons, IEnumerable<int> buttonsPerGroup)
		{
			Debug.Assert(possibleSizes != null); // per contract
			Debug.Assert(possibleSizes.GetEnumerator().MoveNext());
				// contract: possibleSizes must contain at least one size

			List<Size> result = new List<Size>();
			int smallestHeight = int.MaxValue;
			foreach (Size size in possibleSizes)
			{
				int heightNeeded = CalculateHeightNeededForButtons(size, availableSpaceForButtons.Width, buttonsPerGroup);
				// consider all heights that don't cause scroll as the same
				heightNeeded = Math.Max(heightNeeded, availableSpaceForButtons.Height);
				if (heightNeeded < smallestHeight)
				{
					// found one that causes less scroll, so clear out old values
					result.Clear();
					smallestHeight = heightNeeded;
				}
				if (heightNeeded == smallestHeight)
				{
					result.Add(size);
				}
			}

			Debug.Assert(result.Count > 0); // contract: returned list must have at least one member
			return result;
		}

		private static int CalculateHeightNeededForButtons(Size size, int availableWidthForButtons, IEnumerable<int> buttonsPerGroup)
		{
			const int widthBetweenButtons = 6; // default margin padding on controls
			const int heightBetweenRows = 6;
			int maxButtonsInRow = (int) Math.Floor((double) availableWidthForButtons/(size.Width + widthBetweenButtons));
			maxButtonsInRow = Math.Max(maxButtonsInRow, 1); // always at least one button in a row
			int heightNeeded = 0;
			foreach (int buttonsInGroup in buttonsPerGroup)
			{
				int rowsNeeded = (int) Math.Ceiling((double) buttonsInGroup/maxButtonsInRow);
				heightNeeded += rowsNeeded*(size.Height + heightBetweenRows);
			}
			return heightNeeded;
		}

		private static List<Size> RemoveClippedButtonSizes(IEnumerable<Size> possibleSizes, int availableWidthForButtons)
		{
			Debug.Assert(possibleSizes != null); // per contract
			List<Size> sortedSizes = new List<Size>(possibleSizes);
			List<Size> workingSizes = new List<Size>(sortedSizes.Count);
			Debug.Assert(sortedSizes.Count > 0); // per contract

			sortedSizes.Sort(CompareSizesByWidthThenHeight);
			// if all sizes are clipped, return least clipped
			if (sortedSizes[0].Width > availableWidthForButtons)
			{
				workingSizes.Add(sortedSizes[0]);
			}
			else
			{
				// find sizes that are not clipped
				foreach (Size size in sortedSizes)
				{
					if (size.Width <= availableWidthForButtons)
					{
						workingSizes.Add(size);
					}
					else
					{
						break;
					}
				}
			}
			Debug.Assert(workingSizes.Count > 0); // contract: returned list must have at least one member
			return workingSizes;
		}

		/// <summary>
		/// This method calculates the total space available for all the buttons on our form once space is removed
		/// for all the other controls.
		/// </summary>
		private Size GetAvailableSpaceForButtons()
		{
			Size sizeForButtons = new Size(_flow.ClientRectangle.Width, ClientRectangle.Height - _flow.Location.Y);

			foreach (Control control in _flow.Controls)
			{
				sizeForButtons.Height -= control.Margin.Top + control.Margin.Bottom;
				FlowLayoutPanel flow = control as FlowLayoutPanel;
				if (flow != null)
				{
					sizeForButtons.Width = _flow.ClientRectangle.Width - flow.Margin.Left - flow.Margin.Right;
				}
				else
				{
					sizeForButtons.Height -= control.Height;
				}
			}
			// If we're already scrolling, the width of the scrollbar is already figured in to _flow.ClientRectangle.Width
			// Otherwise, pretend like we will need a scrollbar to may sizing work better when we do
			sizeForButtons.Width -= (VScroll ? 0 : SystemInformation.VerticalScrollBarWidth);
			return sizeForButtons;
		}

		#region ITask Members

		private const int CountNotRelevant = -1;

		public int GetReferenceCount()
		{
			return CountNotRelevant;
		}

		public bool IsPinned
		{
			get { return true; }
		}

		public int GetRemainingCount()
		{
			return CountNotRelevant;
		}

		public bool AreCountsRelevant()
		{
			return false;
		}

		public int ExactCount
		{
			get { return CountNotRelevant; }
		}

		public void Activate()
		{
			if (IsActive)
			{
				throw new InvalidOperationException(
						"Activate should not be called when object is active.");
			}

			Initialize();
			SuspendLayout();
			if (ThingsToMakeButtonsFor == null)
			{
				ThingsToMakeButtonsFor = new List<IThingOnDashboard>();
				foreach (ITask task in WeSayWordsProject.Project.Tasks)
				{
					ThingsToMakeButtonsFor.Add(task);
				}
				foreach (IWeSayAddin action in AddinSet.GetAddinsForUser())
				{
					ThingsToMakeButtonsFor.Add(action);
				}
			}


			AddItemsToFlow();
			ResumeLayout(true);
			_isActive = true;
		}

		private void Initialize()
		{
			InitializeComponent();
			BackColor = DisplaySettings.Default.GetEndBackgroundColor(this);

			_buttonGroups = new List<ButtonGroup>();
			_buttonGroups.Add(
					new ButtonGroup(DashboardGroup.Gather,
									Color.FromArgb(155, 187, 89),
									Color.FromArgb(195, 214, 155)));
			_buttonGroups.Add(
					new ButtonGroup(DashboardGroup.Describe,
									Color.FromArgb(85, 142, 213),
									Color.FromArgb(185, 205, 229)));
			_buttonGroups.Add(
					new ButtonGroup(DashboardGroup.Refine,
									Color.FromArgb(250, 192, 144),
									Color.FromArgb(252, 213, 181)));
			_buttonGroups.Add(
					new ButtonGroup(DashboardGroup.Share,
									Color.FromArgb(119, 147, 60),
									Color.White));

			LocalizationHelper helper = new LocalizationHelper(null);
			helper.Parent = this;
			helper.EndInit();
		}

		public void Deactivate()
		{
			if (!IsActive)
			{
				throw new InvalidOperationException(
						"Deactivate should only be called once after Activate.");
			}
			Controls.Clear();
			_isActive = false;
		}

		public void GoToUrl(string url) {}

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

		protected override void OnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
			Invalidate(false); // force redraw of background
			Size oldBestSize = _bestButtonSize;
			_bestButtonSize = GetBestButtonSize();
			if (_title != null && _flow.Width != _oldFlowWidth)
			{
				// for some reason, anchoring the title on the left and right didn't work,
				// so I have to set the size manually
				_title.Width = _flow.Width - _title.Margin.Left - _title.Margin.Right;
			}
			if (_bestButtonSize != oldBestSize)
			{
				ResizeButtons();
			}
			if (_bestButtonSize == oldBestSize && _flow.Width == _oldFlowWidth)
			{
				return;
			}
			_oldFlowWidth = _flow.Width;
			bool neededScroll = _flow.Bounds.Bottom >= ClientRectangle.Height;
			ResizeFlows();
			// If we need a scrollbar now, and we didn't before, do another layout
			// to add the scrollbar.  This prevents some problems when resizing
			if ((!neededScroll && _flow.Bounds.Bottom >= ClientRectangle.Height)
				|| (neededScroll && _flow.Bounds.Bottom < ClientRectangle.Height))
			{
				base.OnLayout(e);
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			DisplaySettings.Default.PaintBackground(this, e);
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			Invalidate(true); // force redraw of background
		}

		protected override void OnScroll(ScrollEventArgs se)
		{
			base.OnScroll(se);
			Invalidate(false); // force redraw of background
		}
	}

	internal class ButtonGroup
	{
		private readonly DashboardGroup _group;
		private readonly Color _doneColor;
		private readonly Color _borderColor;

		public ButtonGroup(DashboardGroup group, Color borderColor, Color doneColor)
		{
			_group = group;
			_borderColor = borderColor;
			_doneColor = doneColor;
		}

		public Color DoneColor
		{
			get { return _doneColor; }
		}

		public Color BorderColor
		{
			get { return _borderColor; }
		}

		public DashboardGroup Group
		{
			get { return _group; }
		}
	}
}
