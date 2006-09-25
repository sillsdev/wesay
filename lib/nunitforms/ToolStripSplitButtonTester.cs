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

// Author: Anders Lillrank

#endregion

using System;
using System.Reflection;
using System.Windows.Forms;
namespace NUnit.Extensions.Forms
{
  /// <summary>
  /// A ToolStripItem tester for testing ToolStripSplitButtons.
  /// </summary>
  public class ToolStripSplitButtonTester : ToolStripItemTester
  {
	 #region Constructors

	/// <summary>
	/// Creates a ToolStripSplitButtonTester from the name and the form instance.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="form"></param>
	public ToolStripSplitButtonTester(string name, Form form)
	  : base(name, form)
	{
	}

	/// <summary>
	/// Creates a ToolStripSplitButtonTester from the name and the form name.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="formName"></param>
	public ToolStripSplitButtonTester(string name, string formName)
	  : base(name, formName)
	{
	}

	/// <summary>
	/// Creates a ToolStripSplitButtonTester from the name.
	/// </summary>
	/// <param name="name"></param>
	public ToolStripSplitButtonTester(string name)
	  : base(name)
	{
	}

	/// <summary>
	/// Creates a ToolStripItemTester from a ToolStripItemTester and an index where the
	/// original tester's name is not unique.
	/// </summary>
	/// <remarks>
	/// It is best to use the overloaded Constructor that requires just the name
	/// parameter if possible.
	/// </remarks>
	/// <param name="tester">The ToolStripItemTester.</param>
	/// <param name="index">The index to test.</param>
	public ToolStripSplitButtonTester(ToolStripItemTester tester, int index)
	  : base(tester, index)
	{
	}
	#endregion

	/// <summary>
	/// Allows you to find a ToolStripDropDownButton by index where the name is not unique.
	/// </summary>
	/// <remarks>
	/// This was added to support the ability to find components where their name is
	/// not unique.  If all of your components are uniquely named (I recommend this) then
	/// you will not need this.
	/// </remarks>
	/// <value>The ToolStripDropDownButtonTester at the specified index.</value>
	/// <param name="index">The index of the ToolStripDropDownButtonTester.</param>
	public ToolStripSplitButtonTester this[int index]
	{
	  get
	  {
		return new ToolStripSplitButtonTester(this, index);
	  }
	}

	/// <summary>
	/// Provides access to all of the Properties of the ToolStripDropDownButton.
	/// </summary>
	/// <remarks>
	/// Allows typed access to all of the properties of the underlying control.
	/// </remarks>
	/// <value>The underlying control.</value>
	public ToolStripSplitButton Properties
	{
	  get
	  {
		return (ToolStripSplitButton)Component;
	  }
	}

	#region Public functions/operations
	/// <summary>
	/// Clickes the DropDownItem with the given index.
	/// </summary>
	/// <param name="index"></param>
	public void ClickDropDownItem(int index)
	{
	  ToolStripSplitButton button = Properties;

	  if (button.HasDropDownItems)
	  {
		ToolStripItemCollection items = button.DropDownItems;
		if (items.Count > index)
		{
		  ToolStripItem item = items[index];
		  FireEvent("DropDownItemClicked", new ToolStripItemClickedEventArgs(item));

		}
	  }
	  else
	  {
		throw new NoSuchComponentException("ToolStripDropDownButton has no DropDownItems");
	  }
	}


	/// <summary>
	/// Simulates that the "user" clicks on the button part of the
	/// ToolStripSplitButton.
	/// </summary>
	public virtual void ButtonClick()
	{
	  FireEvent("ButtonClick");
	}

	/// <summary>
	/// Simulates that the "user" double clicks on the button part of the
	/// ToolStripSplitButton.
	/// </summary>
	public virtual void ButtonDoubleClick()
	{
	  FireEvent("ButtonDoubleClick");
	}

	#endregion
  }
}
