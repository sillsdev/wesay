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
using System.Windows.Forms;
namespace NUnit.Extensions.Forms
{
  public class ToolStripTextBoxTester : ToolStripItemTester
  {
	#region Constructors

	public ToolStripTextBoxTester(string name, Form form)
	  : base(name, form)
	{
	}

	public ToolStripTextBoxTester(string name, string formName)
	  : base(name, formName)
	{
	}

	public ToolStripTextBoxTester(string name)
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
	public ToolStripTextBoxTester(ToolStripItemTester tester, int index)
	  : base(tester, index)
	{
	}
	#endregion

	/// <summary>
	/// Allows you to find a ToolStripTextBoxTester by index where the name is not unique.
	/// </summary>
	/// <remarks>
	/// This was added to support the ability to find controls where their name is
	/// not unique.  If all of your controls are uniquely named (I recommend this) then
	/// you will not need this.
	/// </remarks>
	/// <value>The ToolStripTextBoxTester at the specified index.</value>
	/// <param name="index">The index of the TextBoxTester.</param>
	public ToolStripTextBoxTester this[int index]
	{
	  get
	  {
		return new ToolStripTextBoxTester(this, index);
	  }
	}

	/// <summary>
	/// Provides access to all of the Properties of the ToolStripTextBox.
	/// </summary>
	/// <remarks>
	/// Allows typed access to all of the properties of the underlying control.
	/// </remarks>
	/// <value>The underlying control.</value>
	public ToolStripTextBox Properties
	{
	  get
	  {
		return (ToolStripTextBox)Component;
	  }
	}

	/// <summary>
	/// This method allows you to enter text into the text box.
	/// TODO: This may not work with databindings. I don't know if it's possible
	/// to bind a ToolStripTextBox to a datagrid or other data sources.
	/// </summary>
	/// <param name="text">The text to enter into the text box.</param>
	public void Enter(string text)
	{
	  FireEvent("Enter");
	  Properties.Text = text;
	  FireEvent("Leave");
	}
  }
}
