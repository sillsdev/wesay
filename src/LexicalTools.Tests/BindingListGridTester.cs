using System.Windows.Forms;
using ListBox;
using NUnit.Extensions.Forms;

namespace WeSay.UI
{
	/// <summary>
	/// A ControlTester for testing BindingListGrides.
	/// </summary>
	/// <remarks>
	/// It includes helper methods for selecting items from the list
	/// and for clearing those selections.</remarks>
	public class BindingListGridTester : ControlTester
	{
		/// <summary>
		/// Creates a ControlTester from the control name and the form instance.
		/// </summary>
		/// <remarks>
		/// It is best to use the overloaded Constructor that requires just the name
		/// parameter if possible.
		/// </remarks>
		/// <param name="name">The Control name.</param>
		/// <param name="form">The Form instance.</param>
		public BindingListGridTester(string name, Form form) : base(name, form)
		{
		}

		/// <summary>
		/// Creates a ControlTester from the control name and the form name.
		/// </summary>
		/// <remarks>
		/// It is best to use the overloaded Constructor that requires just the name
		/// parameter if possible.
		/// </remarks>
		/// <param name="name">The Control name.</param>
		/// <param name="formName">The Form name..</param>
		public BindingListGridTester(string name, string formName) : base(name, formName)
		{
		}

		/// <summary>
		/// Creates a ControlTester from the control name.
		/// </summary>
		/// <remarks>
		/// This is the best constructor.</remarks>
		/// <param name="name">The Control name.</param>
		public BindingListGridTester(string name) : base(name)
		{
		}

		/// <summary>
		/// Creates a ControlTester from a ControlTester and an index where the
		/// original tester's name is not unique.
		/// </summary>
		/// <remarks>
		/// It is best to use the overloaded Constructor that requires just the name
		/// parameter if possible.
		/// </remarks>
		/// <param name="tester">The ControlTester.</param>
		/// <param name="index">The index to test.</param>
		public BindingListGridTester(ControlTester tester, int index) : base(tester, index)
		{
		}

		/// <summary>
		/// Allows you to find a BindingListGridTester by index where the name is not unique.
		/// </summary>
		/// <remarks>
		/// This was added to support the ability to find controls where their name is
		/// not unique.  If all of your controls are uniquely named (I recommend this) then
		/// you will not need this.
		/// </remarks>
		/// <value>The ControlTester at the specified index.</value>
		/// <param name="index">The index of the BindingListGridTester.</param>
		public new BindingListGridTester this[int index]
		{
			get
			{
				return new BindingListGridTester(this, index);
			}
		}

		/// <summary>
		/// Provides access to all of the Properties of the BindingListGrid.
		/// </summary>
		/// <remarks>
		/// Allows typed access to all of the properties of the underlying control.
		/// </remarks>
		/// <value>The underlying control.</value>
		public BindingListGrid Properties
		{
			get
			{
				return (BindingListGrid) Control;
			}
		}



		/// <summary>
		/// Clears the selections from the list box.
		/// </summary>
//        public void ClearSelected()
//        {
//            Properties.ClearSelected();
//        }

		/// <summary>
		/// Selects an item in the BindingListGrid according to its index.
		/// </summary>
		/// <param name="i">the index to select.</param>
		public void Select(int i)
		{
			Properties.SelectedIndex = i;
		}

		/// <summary>
		/// Selects an item in the list according to its string value.
		/// </summary>
		/// <param name="text">The item to select.</param>
		public void Select(string text)
		{
			int index = FindItemByString(text);
			if(index != -1)
			{
				Select(index);
			}
			else
			{
				throw new FormsTestAssertionException("Could not find text '" + text + "' in ComboBox '" + name + "'");
			}
		}

		/// <summary>
		/// Sets the selected property of an item at an index.
		/// </summary>
		/// <param name="index">the index to select (or clear)</param>
		/// <param name="value">true if you want to select, false to clear.</param>
//        public void SetSelected(int index, bool value)
//        {
//            Properties.SetSelected(index, value);
//        }

		/// <summary>
		/// Sets the selected property of an item with a specified string value.
		/// </summary>
		/// <param name="text">the item to select (or clear)</param>
		/// <param name="value">true if you want to select, false to clear.</param>
//        public void SetSelected(string text, bool value)
//        {
//            SetSelected(FindItemByString(text), value);
//        }

		private int FindItemByString(string text)
		{
			//TODO: Could we just use FindString or FindStringExact

			for(int i = 0; i < Properties.Rows.Count; i++)
			{
				if(Properties.GetCell(i,0).ToString() == text)
				{
					return i;
				}
			}

			return -1;
		}
	}
}