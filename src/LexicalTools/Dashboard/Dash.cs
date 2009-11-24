using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using Palaso.Reporting;
using WeSay.AddinLib;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.UI;
using Palaso.I8N;

namespace WeSay.LexicalTools.Dashboard
{
	public partial class Dash: UserControl, ITask, IFinishCacheSetup
	{
		private const double GoldRatio = 4.0;
		// arbitrary ratio we think looks the best for button sizes

		private DictionaryStatusControl _title;
		private readonly LexEntryRepository _lexEntryRepository;
		private IList<IThingOnDashboard> _thingsToMakeButtonsFor;
		private List<ButtonGroup> _buttonGroups;
		private bool _isActive;
		private readonly ICurrentWorkTask _currentWorkTaskProvider;
		private int _oldPanelWidth;
		private List<Size> _smallestPossibleButtonSizes;
		private Size _bestButtonSize;
		private bool _addedAllButtons;
		private List<List<DashboardButton>> _buttonRows;
		private readonly Padding _buttonRowMargin;
		private readonly Padding _buttonMargin;
		private int _buttonsPerRow;

		private const TextFormatFlags ToolTipFormatFlags =
			TextFormatFlags.WordBreak | TextFormatFlags.NoFullWidthCharacterBreak |
			TextFormatFlags.LeftAndRightPadding;

		public Dash(LexEntryRepository RecordListManager, ICurrentWorkTask currentWorkTaskProvider)//, UserSettingsForTask userSettings)
		{
			_buttonRows = new List<List<DashboardButton>>();
			_oldPanelWidth = 0;
			_buttonsPerRow = 0;
			_buttonRowMargin = new Padding(30, 0, 0, 15);
			_buttonMargin = new Padding(3);
			_lexEntryRepository = RecordListManager;
			_currentWorkTaskProvider = currentWorkTaskProvider;
			InitializeContextMenu();
			Initialize();
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
				ErrorReport.NotifyUserOfProblem("Could not start " + startInfo.FileName);
				return;
			}

			Application.Exit();
		}

		private void AddItemsToFlow()
		{
			_addedAllButtons = false;
			_title = new DictionaryStatusControl(_lexEntryRepository.CountAllItems());
			_title.Font = new Font(SystemFonts.DefaultFont.FontFamily, 14);
			_title.BackColor = Color.Transparent;
			_title.ShowLogo = true;
			_title.Width = _panel.Width - _title.Margin.Left - _title.Margin.Right;
			_title.TabStop = false;
			_buttonRows.Clear();
			_panel.Controls.Add(_title);

			foreach (ButtonGroup group in _buttonGroups)
			{
				AddButtonGroupToFlow(group);
			}
			_addedAllButtons = true;
		}

		private void AddButtonGroupToFlow(ButtonGroup buttonGroup)
		{
			var buttonRow = new List<DashboardButton>();
			_buttonRows.Add(buttonRow);
			bool foundAtLeastOne = false;
			foreach (IThingOnDashboard item in ThingsToMakeButtonsFor)
			{
				if (item == this)
				{
					continue;
				}
				if (item.Group == buttonGroup.Group)
				{
					buttonRow.Add(MakeButton(item, buttonGroup));
					foundAtLeastOne = true;
				}
			}
			if (foundAtLeastOne)
			{
				Label header = new Label();
				header.AutoSize = true;
				header.Text = StringCatalog.Get(buttonGroup.Group.ToString());
				header.Font = new Font("Arial", 12);
				_panel.Controls.Add(header);
				buttonRow.ForEach(b => _panel.Controls.Add(b));
				_buttonRows.Add(buttonRow);
			}
		}

		private DashboardButton MakeButton(IThingOnDashboard item, ButtonGroup group)
		{
			DashboardButton button = MakeButton(item);
			button.BackColor = Color.Transparent;
			button.Font = Font;
			button.AutoSize = false;
			button.BorderColor = group.BorderColor;
			button.DoneColor = group.DoneColor;

			button.Size = _bestButtonSize;
			button.SizeChanged += ButtonSizeChanged;
			button.Text = item.LocalizedLabel;
			button.Click += OnButtonClick;
			// if fonts or text are localized, we will need to re-measure stuff
			button.TextChanged += delegate { _smallestPossibleButtonSizes = null; };
			button.FontChanged += delegate { _smallestPossibleButtonSizes = null; };
			_toolTip.SetToolTip(button, item.LocalizedLabel + GetToolTipDescription(item));
			return button;
		}

		private void ButtonSizeChanged(object sender, EventArgs e)
		{
			// Buttons were being slightly resized when you went to another tab and came back during the
			// call to add the Dash control to the tab page.  The change appears to be from something in
			// windows forms that I couldn't figure out, so this just sets the size back to what we want.
			Control control = (Control) sender;
			if (_bestButtonSize != Size.Empty && _bestButtonSize != control.Size)
			{
				control.Size = _bestButtonSize;
			}
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
							WeSayWordsProject.Project.GetProjectInfoForAddin();
						addin.Launch(ParentForm, projectInfo);
					}
					catch (Exception error)
					{
						ErrorReport.NotifyUserOfProblem(error.Message);
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

		//This is used in place of AnchorStyle.Right because anchoring is buggy before a control is shown
		protected override void OnResize(EventArgs e)
		{
			_panel.Width = ClientRectangle.Width - _panel.Left;
			base.OnResize(e);
		}

		private void ResizeButtons()
		{
			_panel.SuspendLayout();
			int nextY = 0;
			int nextX = 0;
			bool inButtonRow = false;
			foreach (Control control in _panel.Controls)
			{
				if (control is DashboardButton)
				{
					if (!inButtonRow)  // new group of buttons
					{
						inButtonRow = true;
						nextY += _buttonRowMargin.Top + _buttonMargin.Top;
						nextX = _buttonRowMargin.Left;
					}
					if (nextX > _buttonRowMargin.Left && nextX + _buttonMargin.Horizontal +
														 _bestButtonSize.Width > _panel.ClientSize.Width)  // new row within group
					{
						nextY += _buttonMargin.Vertical + _bestButtonSize.Height;
						nextX = _buttonRowMargin.Left;
					}
					nextX += _buttonMargin.Left;
					control.Bounds = new Rectangle(new Point(nextX, nextY), _bestButtonSize);
					nextX += _buttonMargin.Right + _bestButtonSize.Width;
				}
				else
				{
					if (inButtonRow)
					{
						inButtonRow = false;
						nextY += _bestButtonSize.Height + _buttonMargin.Bottom + _buttonRowMargin.Bottom;
					}
					nextY += control.Margin.Top;
					control.Location = new Point(control.Margin.Left, nextY);
					nextY += control.Margin.Bottom + control.Height;
				}
			}
			_panel.ResumeLayout();
		}

		private IEnumerable<IEnumerable<Size>> GetAllPossibleButtonSizes()
		{
			var sizes = new List<IEnumerable<Size>>();
			foreach (var buttonRow in _buttonRows)
			{
				foreach (DashboardButton button in buttonRow)
				{
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
					_smallestPossibleButtonSizes =
						ComputeSmallestPossibleButtonSizes(GetAllPossibleButtonSizes());
				}
				return _smallestPossibleButtonSizes;
			}
		}

		internal static List<Size> ComputeSmallestPossibleButtonSizes(
			IEnumerable<IEnumerable<Size>> possibleSizesOfButtons)
		{
			List<Size> result = new List<Size>();
			Debug.Assert(possibleSizesOfButtons != null); // per contract
			foreach (IEnumerable<Size> possibleSizesFor1Button in possibleSizesOfButtons)
			{
				result = MergeButtonSizes(result, possibleSizesFor1Button);
			}

			return result;
		}

		private static List<Size> MergeButtonSizes(IEnumerable<Size> sizeList1,
												   IEnumerable<Size> sizeList2)
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

		private delegate bool EqualityComparison<T>(T x, T y);

		private static void RemoveDuplicateHeights(List<Size> result)
		{
			RemoveDuplicates(result,
							 CompareSizesByHeightThenWidth,
							 delegate(Size x, Size y) { return x.Height.Equals(y.Height); });
		}

		private static void RemoveDuplicateWidths(List<Size> result)
		{
			RemoveDuplicates(result,
							 CompareSizesByWidthThenHeight,
							 delegate(Size x, Size y) { return x.Width.Equals(y.Width); });
		}

		private static void RemoveDuplicates(List<Size> result,
											 Comparison<Size> sortComparer,
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

		private static List<Size> CombineButtonSizes(IEnumerable<Size> sizeList1,
													 IEnumerable<Size> sizeList2)
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
					result.Add(new Size(Math.Max(size1.Width, size2.Width),
										Math.Max(size1.Height, size2.Height)));
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
			Dictionary<DashboardGroup, int> buttonsPerGroup =
				new Dictionary<DashboardGroup, int>(_buttonGroups.Count);
			foreach (ButtonGroup group in _buttonGroups)
			{
				buttonsPerGroup.Add(group.Group, 0);
			}
			foreach (IThingOnDashboard item in ThingsToMakeButtonsFor)
			{
				if (item == this)
				{
					continue;
				}
				if (buttonsPerGroup.ContainsKey(item.Group))
				{
					++buttonsPerGroup[item.Group];
				}
			}
			List<int> buttonsPerGroupList = new List<int>(buttonsPerGroup.Count);
			foreach (KeyValuePair<DashboardGroup, int> pair in buttonsPerGroup)
			{
				if (pair.Value > 0)
				{
					buttonsPerGroupList.Add(pair.Value);
				}
			}
			return buttonsPerGroupList;
		}

		private void SetBestButtonSize()
		{
			Size availableSpaceForButtons = GetAvailableSpaceForButtons();
			_bestButtonSize = ComputeBestButtonSize(SmallestPossibleButtonSizes,
													availableSpaceForButtons,
													GetButtonsPerGroup());
			_buttonsPerRow = Math.Max(1,
									  availableSpaceForButtons.Width/(_bestButtonSize.Width + _buttonMargin.Horizontal));
		}

		internal static Size ComputeBestButtonSize(List<Size> smallestPossibleSizes,
												   Size availableSpaceForButtons,
												   IEnumerable<int> buttonsPerGroup)
		{
			if (smallestPossibleSizes == null || smallestPossibleSizes.Count == 0)
			{
				return Size.Empty;
			}

			List<Size> result = RemoveClippedButtonSizes(smallestPossibleSizes,
														 availableSpaceForButtons.Width);
			result = RemoveScrolledButtonSizes(result, availableSpaceForButtons, buttonsPerGroup);
			return GetBestSizeBasedOnRatio(result, GoldRatio);
		}

		private static Size GetBestSizeBasedOnRatio(IEnumerable<Size> possibleSizes,
													double targetRatio)
		{
			Debug.Assert(possibleSizes != null); // per contract
			Debug.Assert(possibleSizes.GetEnumerator().MoveNext());
			// contract: possibleSizes must contain at least one size

			Size bestSize = Size.Empty;
			double bestRatio = double.PositiveInfinity;
			foreach (Size size in possibleSizes)
			{
				double ratio = (double) size.Width / size.Height;
				if (Math.Abs(ratio - targetRatio) <= Math.Abs(bestRatio - targetRatio))
				{
					bestRatio = ratio;
					bestSize = size;
				}
			}
			Debug.Assert(bestSize != Size.Empty); // per contract
			return bestSize;
		}

		private static List<Size> RemoveScrolledButtonSizes(IEnumerable<Size> possibleSizes,
															Size availableSpaceForButtons,
															IEnumerable<int> buttonsPerGroup)
		{
			Debug.Assert(possibleSizes != null); // per contract
			Debug.Assert(possibleSizes.GetEnumerator().MoveNext());
			// contract: possibleSizes must contain at least one size

			List<Size> result = new List<Size>();
			int smallestHeight = int.MaxValue;
			foreach (Size size in possibleSizes)
			{
				int heightNeeded = CalculateHeightNeededForButtons(size,
																   availableSpaceForButtons.Width,
																   buttonsPerGroup);
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

		private static int CalculateHeightNeededForButtons(Size size,
														   int availableWidthForButtons,
														   IEnumerable<int> buttonsPerGroup)
		{
			const int widthBetweenButtons = 6; // default margin padding on controls
			const int heightBetweenRows = 6;
			int maxButtonsInRow =
				(int)
				Math.Floor((double) availableWidthForButtons /
						   (size.Width + widthBetweenButtons));
			maxButtonsInRow = Math.Max(maxButtonsInRow, 1); // always at least one button in a row
			int heightNeeded = 0;
			foreach (int buttonsInGroup in buttonsPerGroup)
			{
				int rowsNeeded = (int) Math.Ceiling((double) buttonsInGroup / maxButtonsInRow);
				heightNeeded += rowsNeeded * (size.Height + heightBetweenRows);
			}
			return heightNeeded;
		}

		private static List<Size> RemoveClippedButtonSizes(IEnumerable<Size> possibleSizes,
														   int availableWidthForButtons)
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
			Debug.Assert(workingSizes.Count > 0);
			// contract: returned list must have at least one member
			return workingSizes;
		}

		/// <summary>
		/// This method calculates the total space available for all the buttons on our form once space is removed
		/// for all the other controls.
		/// </summary>
		private Size GetAvailableSpaceForButtons()
		{
			Size sizeForButtons = new Size(_panel.ClientRectangle.Width,
										   ClientRectangle.Height - _panel.Location.Y);

			foreach (Control control in _panel.Controls)
			{
				if (control is DashboardButton)
				{
					continue;
				}
				sizeForButtons.Height -= control.Margin.Vertical + control.Height;
				sizeForButtons.Height -= control.Height;
			}
			sizeForButtons.Height -= _buttonRows.Count*_buttonRowMargin.Vertical;
			sizeForButtons.Width -= _buttonRowMargin.Horizontal;
			// If we're already scrolling, the width of the scrollbar is already figured in to _panel.ClientRectangle.Width
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

			SuspendLayout();
			if (ThingsToMakeButtonsFor == null)
			{
				ThingsToMakeButtonsFor = new List<IThingOnDashboard>();
				foreach (ITask task in WeSayWordsProject.Project.Tasks)
				{
					if (task.Available)
					{
						ThingsToMakeButtonsFor.Add(task);
					}
				}
				foreach (IWeSayAddin action in AddinSet.GetAddinsForUser())
				{
					if (action is IWeSayAddinHasSettings)
					{
						LoadAddinSettings(action as IWeSayAddinHasSettings);
					}
					ThingsToMakeButtonsFor.Add(action);
				}
			}

			AddItemsToFlow();
			ResumeLayout(true);
			_isActive = true;
		}

		//todo: this doesn't belong here... trying to rapidly fix a bug in a stable release...
		//(no settings were being loaded at all from dashboard!!!)
		private void LoadAddinSettings(IWeSayAddinHasSettings addin)
		{
			object existingSettings = addin.Settings;
			if (existingSettings == null)
			{
				return; // this class doesn't do settings
			}

			//this is not necessarily the right place for this deserialization to be happening
			string settings = AddinSet.Singleton.GetSettingsXmlForAddin(((IWeSayAddin)addin).ID);
			if (!String.IsNullOrEmpty(settings))
			{
				XmlSerializer x = new XmlSerializer(existingSettings.GetType());
				using (StringReader r = new StringReader(settings))
				{
					addin.Settings = x.Deserialize(r);
				}
			}
		}

		private void Initialize()
		{
			InitializeComponent();
			BackColor = DisplaySettings.Default.GetEndBackgroundColor(this);

			_buttonGroups = new List<ButtonGroup>();
			_buttonGroups.Add(new ButtonGroup(DashboardGroup.Gather,
											  Color.FromArgb(155, 187, 89),
											  Color.FromArgb(195, 214, 155)));
			_buttonGroups.Add(new ButtonGroup(DashboardGroup.Describe,
											  Color.FromArgb(85, 142, 213),
											  Color.FromArgb(185, 205, 229)));
			_buttonGroups.Add(new ButtonGroup(DashboardGroup.Review,
											  Color.FromArgb(250, 192, 144),
											  Color.FromArgb(252, 213, 181)));
			_buttonGroups.Add(new ButtonGroup(DashboardGroup.Share,
											  Color.FromArgb(119, 147, 60),
											  Color.White));
		}

		public void Deactivate()
		{
			if (!IsActive)
			{
				throw new InvalidOperationException(
					"Deactivate should only be called once after Activate.");
			}
			SuspendLayout(); //NB: In WS-1234, the user found this to be really slow (!,??), hence the suspend
			_toolTip.RemoveAll();
			_panel.Controls.Clear();
			ResumeLayout();
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
				return StringCatalog.Get("~Home",
										 "The label for the 'dashboard'; the task which lets you see the status of other tasks and jump to them.");
			}
		}

		public bool Available
		{
			get { return true; }
		}

		public string Description
		{
			get { return StringCatalog.Get("~Switch tasks and see current status of tasks"); }
		}



		public Control Control
		{
			get { return this; }
		}

		public string GetRemainingCountText()
		{
			throw new NotImplementedException();
		}

		public string GetReferenceCountText()
		{
			throw new NotImplementedException();
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

		public string LocalizedLongLabel
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

		private void FixPanelSize()
		{
			// fixing mono bugginess
			SuspendLayout();
			_panel.Anchor = AnchorStyles.None;
			_panel.Bounds = new Rectangle(20, 11, ClientRectangle.Width - 20, ClientRectangle.Height - 11);
			_panel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			ResumeLayout(false);
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
#if MONO
			FixPanelSize();
#endif
			base.OnLayout(e);
			Invalidate(false); // force redraw of background
			bool needReposition = false;
			if (_title != null && _panel.Width != _oldPanelWidth)
			{
				// for some reason, anchoring the title on the left and right didn't work,
				// so I have to set the size manually
				int oldTitleHeight = _title.Height;
				_title.Width = _panel.Width - _title.Margin.Left - _title.Margin.Right;
				needReposition |= _title.Height != oldTitleHeight;
			}
			Size oldBestSize = _bestButtonSize;
			int oldButtonsPerRow = _buttonsPerRow;
			SetBestButtonSize();
			if (needReposition || _bestButtonSize != oldBestSize || _buttonsPerRow != oldButtonsPerRow)
			{
				ResizeButtons();
			}
			if (_bestButtonSize == oldBestSize && _panel.Width == _oldPanelWidth)
			{
				return;
			}
			_oldPanelWidth = _panel.Width;
			bool neededScroll = _panel.Bounds.Bottom >= ClientRectangle.Height;
			_panel.Height = _panel.GetPreferredSize(new Size(_panel.Width, 0)).Height;
			// If we need a scrollbar now, and we didn't before, do another layout
			// to add the scrollbar.  This prevents some problems when resizing
			if ((!neededScroll && _panel.Bounds.Bottom >= ClientRectangle.Height) ||
				(neededScroll && _panel.Bounds.Bottom < ClientRectangle.Height))
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
			Invalidate(false); // force redraw of background
		}

		protected override void OnScroll(ScrollEventArgs se)
		{
			base.OnScroll(se);
			Invalidate(false); // force redraw of background
		}

		private void _toolTip_Draw(object sender, DrawToolTipEventArgs e)
		{
			DashboardButton button = e.AssociatedControl as DashboardButton;
			// Default behavior for non-button tooltips
			if (button == null)
			{
				e.DrawBackground();
				e.DrawBorder();
				e.DrawText();
				return;
			}
			DisplaySettings.Default.PaintBackground(e.Graphics, e.Bounds, this);
			e.DrawBorder();
			string title = button.ThingToShowOnDashboard.LocalizedLongLabel;
			Font localizedFont = StringCatalog.ModifyFontForLocalization(SystemFonts.DefaultFont);
			Font boldFont = new Font(localizedFont, FontStyle.Bold);
			int titleHeight =
				TextRenderer.MeasureText(e.Graphics,
										 title,
										 boldFont,
										 new Size(e.Bounds.Width - 6, int.MaxValue),
										 ToolTipFormatFlags).Height;
			Rectangle titleBounds = new Rectangle(e.Bounds.Left + 3,
												  e.Bounds.Top + 3,
												  e.Bounds.Width - 6,
												  e.Bounds.Top + 2 + titleHeight);
			Rectangle descriptionBounds = new Rectangle(e.Bounds.Left + 18,
														e.Bounds.Top + 3 + titleHeight,
														e.Bounds.Width - 21,
														e.Bounds.Height - 8 - titleHeight);
			TextRenderer.DrawText(e.Graphics,
								  title,
								  boldFont,
								  titleBounds,
								  Color.Black,
								  ToolTipFormatFlags);
			TextRenderer.DrawText(e.Graphics,
								  GetToolTipDescription(button.ThingToShowOnDashboard),
								  localizedFont,
								  descriptionBounds,
								  Color.Black,
								  ToolTipFormatFlags);
			localizedFont.Dispose();
			boldFont.Dispose();
		}

		private void _toolTip_Popup(object sender, PopupEventArgs e)
		{
			DashboardButton button = e.AssociatedControl as DashboardButton;
			if (button == null)
			{
				return;
			}
			string title = button.ThingToShowOnDashboard.LocalizedLongLabel;
			Graphics g = Graphics.FromHwnd(e.AssociatedWindow.Handle);
			Font localizedFont = StringCatalog.ModifyFontForLocalization(SystemFonts.DefaultFont);
			Font boldFont = new Font(localizedFont, FontStyle.Bold);
			List<Size> possibleSizes = DisplaySettings.GetPossibleTextSizes(g,
																			GetToolTipDescription(
																				button.
																					ThingToShowOnDashboard),
																			localizedFont,
																			ToolTipFormatFlags);
			Size bestSize = GetBestSizeBasedOnRatio(possibleSizes, GoldRatio);
			bestSize.Width += 15;
			bestSize.Height +=
				TextRenderer.MeasureText(g,
										 title,
										 boldFont,
										 new Size(bestSize.Width, int.MaxValue),
										 ToolTipFormatFlags).Height;
			e.ToolTipSize = new Size(bestSize.Width + 6, bestSize.Height + 8);
			g.Dispose();
			localizedFont.Dispose();
			boldFont.Dispose();
		}

		private static string GetToolTipDescription(IThingOnDashboard dashboardItem)
		{
			string toolTipString = "\n" + dashboardItem.Description;
			ITask task = dashboardItem as ITask;
			if (task != null && task.GetReferenceCount() >= 0 && task.GetRemainingCount() >= 0)
			{
				toolTipString += "\n\n" + task.GetRemainingCountText() + "\n" +
								 task.GetReferenceCountText();
			}
			return toolTipString;
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