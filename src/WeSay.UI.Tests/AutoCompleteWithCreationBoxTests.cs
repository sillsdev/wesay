using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NUnit.Framework;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.UI;
using WeSay.UI.AutoCompleteTextBox;


namespace WeSay.UI.Tests
{
	[TestFixture]
	public class AutoCompleteWithCreationBoxTests
	{
		private AutoCompleteWithCreationBox<Option, string> _control;
		private Form _window;
		private OptionsList _sourceChoices;
		private List<string> _choiceKeys;
		private Control _somethingElseToFocusOn;
//        private bool _createNewClickedFired;
//        private bool _valueChangedFired;

		/// <summary>
		/// Key concept: this is the data (as would be in the database) that we are editing
		/// </summary>
		private OptionRef _dataBeingEditted;
		private WritingSystem _ws;
		private OptionDisplayAdaptor _displayAdaptor;


		[SetUp]
		public void Setup()
		{
			_ws = new WritingSystem("xx", new Font("Arial", 12));
//            _createNewClickedFired=false;
//            _valueChangedFired = false;
			_sourceChoices = new OptionsList();
			_choiceKeys = new List<string>();
			AddSourceChoice("one", "1");
			AddSourceChoice("two", "2");
			AddSourceChoice("three", "3");

			_control = new AutoCompleteWithCreationBox<Option,string>();
			_control.Name = "autobox";
			_control.Box.Items = _sourceChoices.Options;

		   //leave for individual tests _control.CreateNewClicked += new EventHandler<CreateNewArgs>(_control_CreateNewClicked);
			_displayAdaptor = new OptionDisplayAdaptor(_sourceChoices,_ws.Id);
			_control.Box.ItemDisplayStringAdaptor = _displayAdaptor;
			_control.Box.WritingSystem = _ws;
			_control.GetKeyValueFromValue = _displayAdaptor.GetOptionFromKey;
			_control.GetValueFromKeyValue = _displayAdaptor.GetKeyFromOption;
			_control.ValueChanged += new EventHandler(_control_ValueChanged);

			_dataBeingEditted = new OptionRef();
		}


		void _control_CreateNewClicked(object sender, CreateNewArgs e)
		{
			//_createNewClickedFired=true;
		}

		void _control_ValueChanged(object sender, EventArgs e)
		{
		   // _valueChangedFired = true;
		}


		[TearDown]
		public void TearDown()
		{
			if (_window != null)
			{
				_window.Close();
				_window.Dispose();
				_window = null;
			}
		}

		[Test]
		public void ShowWithNothingChosenYet()
		{
			_control.Box.SelectedItem = null;
			BindAndShow();
		}



		[Test]
		public void DisplaysCorrectLabel()
		{
			SetKeyAndShow("3");
			Assert.AreEqual("three", _control.Box.Text);
		}

		[Test]
		public void ShowWithChoiceThatIsNotInList()
		{
			SetKeyAndShow("29");
			Assert.AreEqual("29", _control.Box.Text);
			Assert.IsTrue(_control.HasProblems);
		}


		[Test]
		public void DoesNotShowAddNewButtonWithClosedList()
		{
			SetKeyAndShow("29");
			_control.Focus();
			Assert.IsTrue(_control.ContainsFocus);
			Assert.IsFalse(_control.AddNewButton.Visible);
		}

		[Test]
		public void DoesShowAddNewButtonWithOpenList()
		{
			_control.CreateNewClicked += new EventHandler<CreateNewArgs>(_control_CreateNewClicked);
			SetKeyAndShow("29");
			_control.Focus();
			Assert.IsTrue(_control.ContainsFocus);
			Assert.IsTrue(_control.AddNewButton.Visible);
		}

		[Test]
		public void SetPreviouslyEmptyItem()
		{
			SetKeyAndShow(String.Empty);
			Assert.AreEqual("", _control.Box.Text);
			_control.Box.Focus();
			_control.Box.Paste("two");
			_somethingElseToFocusOn.Focus();
			Assert.AreEqual("2", _dataBeingEditted.Key);
		}

		[Test]
		public void ClearItem()
		{
			SetKeyAndShow("3");
			Assert.AreEqual("3", _dataBeingEditted.Key);
			_control.Box.Text = "";
			_somethingElseToFocusOn.Focus();
			Assert.IsTrue(String.IsNullOrEmpty(_dataBeingEditted.Key));
		}


		[Test]
		public void ChangeItemBetweenRealChoices()
		{
			SetKeyAndShow("3");
			Assert.AreEqual("3", _dataBeingEditted.Key);
			_control.Box.Text = "two";
			Assert.AreEqual("2", _dataBeingEditted.Key);
		}




		//------------------------------------------------------------
		private void AddSourceChoice(string label, string key)
		{
			MultiText name = new MultiText();
			name[_ws.Id] = label;
			_sourceChoices.Options.Add(new Option(key, name));
			_choiceKeys.Add(key);
		}


		private void SetKeyAndShow(string key)
		{
			_dataBeingEditted.Key = key;
			BindAndShow();
		}

		/// <summary>
		/// Needed for focus-related tests
		/// </summary>
		private void BindAndShow()
		{
			if (_window == null)
			{
				_control.Box.SelectedItem = _displayAdaptor.GetOptionFromKey(_dataBeingEditted.Key);
				if(_control.Box.SelectedItem == null)
				{
				 _control.Box.Text = _dataBeingEditted.Key;//will show with red background
				}
				//just creating it is enough; the events bindings it creates keeps it alive
				SimpleBinding<string> dummy = new SimpleBinding<string>(_dataBeingEditted, _control);
				_window = new Form();
				_somethingElseToFocusOn = new Button();
				_window.Controls.Add(_somethingElseToFocusOn);

			   // _control.Dock = DockStyle.Fill;
				_window.Controls.Add(_control);

				_window.Show();
			}
		}


	}
}
