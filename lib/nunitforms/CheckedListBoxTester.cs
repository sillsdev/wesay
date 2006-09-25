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

//Contributed by: Ian Cooper

using System;
using System.Collections;
using System.Windows.Forms;

namespace NUnit.Extensions.Forms
{
	/// <summary>
	/// A ControlTester for testing CheckedListBoxes.
	/// </summary>
	public class CheckedListBoxTester : ControlTester
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
		public CheckedListBoxTester(string name, Form form) : base(name, form)
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
		public CheckedListBoxTester(string name, string formName) : base(name, formName)
		{
		}

		/// <summary>
		/// Creates a ControlTester from the control name.
		/// </summary>
		/// <remarks>
		/// This is the best constructor.</remarks>
		/// <param name="name">The Control name.</param>
		public CheckedListBoxTester(string name) : base(name)
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
		public CheckedListBoxTester(ControlTester tester, int index) : base(tester, index)
		{
		}

		/// <summary>
		/// Allows you to find a CheckBoxTester by index where the name is not unique.
		/// </summary>
		/// <remarks>
		/// This was added to support the ability to find controls where their name is
		/// not unique.  If all of your controls are uniquely named (I recommend this) then
		/// you will not need this.
		/// </remarks>
		/// <value>The ControlTester at the specified index.</value>
		/// <param name="index">The index of the CheckBoxTester.</param>
		public new CheckedListBoxTester this[int index]
		{
			get
			{
				return new CheckedListBoxTester(this, index);
			}
		}

		/// <summary>
		/// Checks the row that matches item in the list
		/// </summary>
		/// <param name="item">The list item to check</param>
		public void CheckItem(string item)
		{
			SetItemChecked(FindListItem(item), true);
		}

		/// <summary>
		/// Check a range of items
		/// </summary>
		/// <param name="items"></param>
		public void CheckItems(string[] items)
		{
			foreach(string item in items)
			{
				CheckItem(item);
			}
		}

		/// <summary>
		/// Check a specific item in a list
		/// </summary>
		/// <param name="index">The index of the item to check</param>
		public void SetItemChecked(int index, bool selected)
		{
			Properties.SetItemChecked(index, selected);
		}

		/// <summary>
		/// Clears the row that matches item in the list
		/// </summary>
		/// <param name="item">The list item to check</param>
		public void ClearItem(string item)
		{
			SetItemChecked(FindListItem(item), false);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="selectedItems"></param>
		public void CheckSelectedItems(ArrayList matchList)
		{
			foreach(object item in CheckedItems)
			{
				FormsAssert.IsTrue(matchList.Contains(item));
			}
		}

		/// <summary>
		/// Provides access to all of the Properties of the ListBox.
		/// </summary>
		/// <remarks>
		/// Allows typed access to all of the properties of the underlying control.
		/// </remarks>
		/// <value>The underlying control.</value>
		public CheckedListBox Properties
		{
			get
			{
				return (CheckedListBox) Control;
			}
		}

		/// <summary>
		/// Provides access to the list of items
		/// </summary>
		/// <returns>A CheckedListBox.ObjectCollection of all the items in the list</returns>
		public CheckedListBox.ObjectCollection Items
		{
			get
			{
				return Properties.Items;
			}
		}

		/// <summary>
		/// Provides access to the list of checked items
		/// </summary>
		public CheckedListBox.CheckedItemCollection CheckedItems
		{
			get
			{
				return Properties.CheckedItems;
			}
		}


		private int FindListItem(string item)
		{
			int index = Properties.FindStringExact(item);

			if(index == -1)
			{
				throw new IndexOutOfRangeException(string.Format("{0} not in list", item));
			}

			return index;
		}
	}
}