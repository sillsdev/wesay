#region Copyright (c) 2003-2005, Luke T. Maxon

/********************************************************************************************************************
'
' Copyright (c) 2003-2005, Luke T. Maxon
' All rights reserved.
'
' Redistribution and use in source and binary forms, with or without modification, are permitted provided
' that the following conditions are met:
'
' * Redistributions of source code must retain the above copyright notice, this list of conditions and the
' 	following disclaimer.
'
' * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and
' 	the following disclaimer in the documentation and/or other materials provided with the distribution.
'
' * Neither the name of the author nor the names of its contributors may be used to endorse or
' 	promote products derived from this software without specific prior written permission.
'
' THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
' WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
' PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
' ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
' LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
' INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
' OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
' IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
'
'*******************************************************************************************************************/

#endregion

using System.Windows.Forms;

namespace NUnit.Extensions.Forms
{
	/// <summary>
	/// A ControlTester for testing ListBoxes.
	/// </summary>
	/// <remarks>
	/// It includes helper methods for selecting items from the list
	/// and for clearing those selections.</remarks>
	public class ListBoxTester : ControlTester
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
		public ListBoxTester(string name, Form form) : base(name, form)
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
		public ListBoxTester(string name, string formName) : base(name, formName)
		{
		}

		/// <summary>
		/// Creates a ControlTester from the control name.
		/// </summary>
		/// <remarks>
		/// This is the best constructor.</remarks>
		/// <param name="name">The Control name.</param>
		public ListBoxTester(string name) : base(name)
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
		public ListBoxTester(ControlTester tester, int index) : base(tester, index)
		{
		}

		/// <summary>
		/// Allows you to find a ListBoxTester by index where the name is not unique.
		/// </summary>
		/// <remarks>
		/// This was added to support the ability to find controls where their name is
		/// not unique.  If all of your controls are uniquely named (I recommend this) then
		/// you will not need this.
		/// </remarks>
		/// <value>The ControlTester at the specified index.</value>
		/// <param name="index">The index of the ListBoxTester.</param>
		public new ListBoxTester this[int index]
		{
			get
			{
				return new ListBoxTester(this, index);
			}
		}

		/// <summary>
		/// Provides access to all of the Properties of the ListBox.
		/// </summary>
		/// <remarks>
		/// Allows typed access to all of the properties of the underlying control.
		/// </remarks>
		/// <value>The underlying control.</value>
		public ListBox Properties
		{
			get
			{
				return (ListBox) Control;
			}
		}

		/// <summary>
		/// Clears the selections from the list box.
		/// </summary>
		public void ClearSelected()
		{
			Properties.ClearSelected();
		}

		/// <summary>
		/// Selects an item in the ListBox according to its index.
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
		public void SetSelected(int index, bool value)
		{
			Properties.SetSelected(index, value);
		}

		/// <summary>
		/// Sets the selected property of an item with a specified string value.
		/// </summary>
		/// <param name="text">the item to select (or clear)</param>
		/// <param name="value">true if you want to select, false to clear.</param>
		public void SetSelected(string text, bool value)
		{
			SetSelected(FindItemByString(text), value);
		}

		private int FindItemByString(string text)
		{
			//TODO: Could we just use FindString or FindStringExact

			for(int i = 0; i < Properties.Items.Count; i++)
			{
				if(Properties.Items[i].ToString() == text)
				{
					return i;
				}
			}

			return -1;
		}
	}
}