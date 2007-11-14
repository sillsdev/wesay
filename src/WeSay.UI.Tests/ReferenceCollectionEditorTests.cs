using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NUnit.Framework;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.UI;
using WeSay.UI.AutoCompleteTextBox;
using NUnit.Extensions.Forms;

namespace WeSay.UI.Tests
{
	[TestFixture]
	public class ReferenceCollectionEditorTests :  IReceivePropertyChangeNotifications
	{
		private WritingSystem _ws = new WritingSystem("test", new Font("Arial", 30));
		private ReferenceCollectionEditor<Option, string, OptionRef> _control;
		private Control _focussedControl;
		private Form _window;
		private OptionsList _sourceChoices;
		private OptionRefCollection _chosenItems;
		private string _lastPropertyChanged;
		private Control _somethingElseToFocusOn;


		[SetUp]
		public void Setup()
		{
			_sourceChoices = new OptionsList();
			AddSourceChoice( "one", "1");
			AddSourceChoice( "two", "2");
			AddSourceChoice( "three", "3");

			_chosenItems = new OptionRefCollection(this);

			List<WritingSystem> lw = new List<WritingSystem>();
			lw.Add(_ws);
			_control = new ReferenceCollectionEditor<Option, string, OptionRef>(_chosenItems.Members, lw);

			_control.Name = "refcontrol";
			_control.SourceChoices = _sourceChoices.Options;
			_control.CollectionBeingEdited = _chosenItems.Members;

			OptionDisplayAdaptor adaptor = new OptionDisplayAdaptor(_sourceChoices,_ws.Id);
			_control.ItemDisplayStringAdaptor = adaptor;
			_control.GetKeyValueFromValue = adaptor.GetOptionFromKey;
			_control.GetValueFromKeyValue = adaptor.GetKeyFromOption;
			_control.GetKeyValueFromKey_Container = adaptor.GetOptionFromOptionRef;
			_control.UpdateKeyContainerFromKeyValue = adaptor.UpdateKeyContainerFromKeyValue;

		}

		[TearDown]
		public void TearDown()
		{
			_window.Close();
			_window.Dispose();
			_window = null;
		}

		[Test]
		public void ShowWithNothingChosenYet()
		{
			ActuallyShowOnScreen();
			Assert.AreEqual(1, Boxes.Count);
		}

		[Test]
		public void ShowWithEmptyChoiceList()
		{
			_sourceChoices.Options.Clear();
			ActuallyShowOnScreen();
			Assert.AreEqual(1, Boxes.Count);
		}


		[Test]
		public void DisplaysCorrectLabel()
		{
			_chosenItems.Add("3");
			ActuallyShowOnScreen();
			Assert.AreEqual("three", GetTextBox(0).Text);
		}

		private Control GetTextBox(int index)
		{
			return Boxes[0].Controls["_textBox"];
		}

		[Test]
		public void ShowWithTwoChosenItems()
		{
			_chosenItems.Add("3");
			_chosenItems.Add("1");
			ActuallyShowOnScreen();
			Assert.AreEqual(3, Boxes.Count);
		}

		IList<AutoCompleteWithCreationBox<Option, string>> Boxes
		{
			get
			{
				IList<AutoCompleteWithCreationBox<Option, string>> boxes = new List<AutoCompleteWithCreationBox<Option, string>>();
				Control panel = _control.Controls[0];
				foreach (Control child in panel.Controls)
				{
					AutoCompleteWithCreationBox<Option, string> b = child as AutoCompleteWithCreationBox<Option, string>;
					if(b!=null)
					{
						boxes.Add(b);
					}
				}
				return boxes;
			}
		}
		[Test]
		public void ShowWithChoiceThatIsNotInList()
		{
			_chosenItems.Add("29");
			ActuallyShowOnScreen();
			Assert.AreEqual(2, Boxes.Count);
			Assert.AreEqual("29", Boxes[0].Text);
			Assert.IsTrue(Boxes[0].HasProblems);
		}


		[Test]
		public void DoesNotShowAddNewButtonWithClosedList()
		{
			 _chosenItems.Add("29");
			ActuallyShowOnScreen();
			GetTextBox(0).Focus();
			Assert.IsTrue(GetTextBox(0).Focused);
			Assert.IsFalse(Boxes[0].AddNewButton.Visible);
		 }

		[Test]
		public void DoesShowAddNewButtonWithClosedOpen()
		{
			_chosenItems.Add("29");
			_control.CreateNewTargetItem += new EventHandler<CreateNewArgs>(OnCreateNewTargetItem);
			ActuallyShowOnScreen();
			GetTextBox(0).Focus();
			Assert.IsTrue(GetTextBox(0).Focused);
			Assert.IsTrue(Boxes[0].AddNewButton.Visible);
		}

		void OnCreateNewTargetItem(object sender, CreateNewArgs e)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		[Test]
		public void AddItem()
		{
			ActuallyShowOnScreen();
			 Assert.AreEqual(1, Boxes.Count);
		   Assert.AreEqual("", GetTextBox(0).Text);
			GetTextBox(0).Text = "two";
			_somethingElseToFocusOn.Focus();
			Assert.AreEqual(1, _chosenItems.Count);
			Assert.AreEqual("2", _chosenItems.KeyAtIndex(0));
			Assert.AreEqual(2, Boxes.Count);
		}

		[Test]
		public void RemoveMiddleItem()
		{
			_chosenItems.Add("1");
			_chosenItems.Add("2");
			_chosenItems.Add("3");
			ActuallyShowOnScreen();
			Assert.AreEqual(4, Boxes.Count);
			GetTextBox(1).Text = "";
			_somethingElseToFocusOn.Focus();
			Assert.AreEqual(3, Boxes.Count);
		}

		[Test]
		public void RemoveLastItem()
		{
			_chosenItems.Add("3");
			ActuallyShowOnScreen();
			Assert.AreEqual(2, Boxes.Count);
			Assert.AreEqual(1, _chosenItems.Count);
			GetTextBox(0).Text = "";
			_somethingElseToFocusOn.Focus();
			Assert.AreEqual(1, Boxes.Count);
			Assert.AreEqual(0, _chosenItems.Count);
		}

		[Test]
		public void RemoveLastItemThenAdd()
		{
			RemoveLastItem();
			AddItem();
		}

		[Test]
		public void AddThenRemove()
		{
			AddItem();
			Assert.AreEqual(2, Boxes.Count);
			GetTextBox(0).Text = "";
			_somethingElseToFocusOn.Focus();
			Assert.AreEqual(1, Boxes.Count);
		}

		[Test]
		public void ChangeItemBetweenRealChoices()
		{
			_chosenItems.Add("3");
			ActuallyShowOnScreen();
			Assert.AreEqual(2, Boxes.Count);
			Assert.AreEqual("3", _chosenItems.KeyAtIndex(0));
			GetTextBox(0).Text = "two";
			Assert.AreEqual(1, _chosenItems.Count);
			Assert.AreEqual("2", _chosenItems.KeyAtIndex(0));
			Assert.AreEqual(2, Boxes.Count);
		}


		//------------------------------------------------------------
		private void AddSourceChoice(string label, string key)
		{
			MultiText name = new MultiText();
			name[_ws.Id] = label;
			_sourceChoices.Options.Add(new Option(key, name));
		}


		private void ActuallyShowOnScreen()
		{
			if (_window == null)
			{
				_window = new Form();
				_somethingElseToFocusOn = new Button();
				_window.Controls.Add(_somethingElseToFocusOn);

				_control.Dock = DockStyle.Fill;
				_window.Controls.Add(_control);

				_window.Show();
			}
		}

//        /// <summary>
//        /// FormToObectFinderDelegate
//        /// </summary>
//        /// <param name="form"></param>
//        /// <returns></returns>
//        public object GetOptionFromForm(string form)
//        {
//            OptionDisplayAdaptor da = new OptionDisplayAdaptor(_sourceChoices, _ws.Id);
//            foreach (object item in _sourceChoices.Options)
//            {
//                if (da.GetDisplayLabel(item) == form)
//                {
//                    return item;
//                }
//            }
//            return null;
//        }

		#region IReceivePropertyChangeNotifications Members

		public void NotifyPropertyChanged(string property)
		{
			_lastPropertyChanged = property;
		}

		#endregion
	}
}
