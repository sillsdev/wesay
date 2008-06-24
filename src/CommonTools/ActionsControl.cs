using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Mono.Addins;
using Palaso.UI.WindowsForms.i8n;
using WeSay.AddinLib;
using WeSay.Foundation;
using WeSay.Foundation.Dashboard;
using WeSay.Project;

namespace WeSay.CommonTools
{
    public partial class ActionsControl : UserControl, ITask
    {
        private bool _isActive;
        private bool _wasLoaded=false;

        public ActionsControl()
        {
            InitializeComponent();
       }

        #region ITask

        public bool MustBeActivatedDuringPreCache
        {
            get{ return false;}
        }

        #region ITask Members

        public void RegisterWithCache(ViewTemplate viewTemplate)
        {
            
        }

        #endregion

        public void Activate()
        {
            //get everything into the LIFT file
            if (WeSayWordsProject.Project.LiftUpdateService != null) // can be null when SampleDataProcessor runs
            {
                WeSayWordsProject.Project.LiftUpdateService.DoLiftUpdateNow(true);
            }
            if (!_wasLoaded)
            {
                _wasLoaded = true;
                LoadAddins();
            }
            _isActive = true;

        }

        private void LoadAddins()
        {
            _addinsList.SuspendLayout();
            _addinsList.Controls.Clear();
            _addinsList.RowStyles.Clear();
            try
            {
                foreach (IWeSayAddin addin in AddinSet.GetAddinsForUser())
                {
                    AddAddin(addin);
                }

                //            AddAddin(new ComingSomedayAddin("Send My Work to Sangkran", "Send email containing all your WeSay work to your advisor.",
                //                 WeSay.CommonTools.Properties.Resources.emailAction));
            }
            catch (Exception error)
            {
                Palaso.Reporting.ErrorReport.ReportNonFatalMessage(
                    "WeSay encountered an error while looking for Addins (e.g., Actions).  The error was: {0}",
                    error.Message);
            }
            _addinsList.ResumeLayout();
        }

      

        private void AddAddin(IWeSayAddin addin)
        {
            ActionItemControl control = new ActionItemControl(addin,false, null);
            control.TabIndex = _addinsList.RowCount;
            control.Launch += OnLaunchAction;

            _addinsList.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _addinsList.Controls.Add(control);
        }

        private static void OnLaunchAction(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                ((IWeSayAddin)sender).Launch(null, WeSay.Project.WeSayWordsProject.Project.GetProjectInfoForAddin());
            }
            catch (Exception error)
            {
                Palaso.Reporting.ErrorReport.ReportNonFatalMessage(error.Message);
            }

            Cursor.Current = Cursors.Default;
        }

        public void Deactivate()
        {
            if(!IsActive)
            {
                throw new InvalidOperationException("Deactivate should only be called once after Activate.");
            }
           // this._vbox.Clear();
            _isActive = false;
        }

        #region ITask Members

        public void GoToUrl(string url)
        {
            
        }

        #endregion

        public bool IsActive
        {
            get { return this._isActive; }
        }

        public string Label
        {
            get { return StringCatalog.Get("~Actions"); }
        }

        public Control Control
        {
            get { return this; }
        }

        public bool IsPinned
        {
            get
            {
                return true;
            }
        }

        public int GetRemainingCount()
        {
            return CountNotRelevant;
        }

        public int ExactCount
        {
            get
            {
                return CountNotRelevant;
            }
        }

        private const int CountNotRelevant = -1;

        /// <summary>
        /// Not relevant for this task
        /// </summary>
        public int GetReferenceCount()
        {
            return CountNotRelevant;
        }

        public bool AreCountsRelevant()
        {
            return false;
        }

        public string GetRemainingCountText()
        {
            return string.Empty;
        }

        public string GetReferenceCountText()
        {
            return string.Empty;
        }

        public string Description
        {
            get
            {
                return StringCatalog.Get("~Backup, print, etc.", "The description of the Actions task.");
            }
        }

        #region IThingOnDashboard Members



        public DashboardGroup Group
        {
            get { return DashboardGroup.DontShow; }
        }

        public string LocalizedLabel
        {
            get { return StringCatalog.Get(Label); }
        }

        public string LocalizedLongLabel
        {
            get { return LocalizedLabel; }
        }

        public ButtonStyle DashboardButtonStyle
        {
            get { return ButtonStyle.VariableAmount; }
        }

        public Image DashboardButtonImage
        {
            get {return null; }
        }

        #endregion

        #endregion

    }
}