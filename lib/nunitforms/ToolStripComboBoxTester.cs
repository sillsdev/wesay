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
  /// <summary>
  /// A Component tester for testing ToolStripComboBoxes.
  /// </summary>
  public class ToolStripComboBoxTester : ToolStripItemTester
  {

	#region Constructors

	/// <summary>
	/// Constructs a ToolStripComboBox from the name of the ToolStripComboBox
	/// to test contained in a given form instance. If there are more than one
	/// with the same name an AmbiguousNameException will be thrown.
	/// </summary>
	public ToolStripComboBoxTester(string name, Form form) : base(name,form)
	{
	}
	/// <summary>
	/// Constructs a ToolStripComboBox from the name of the ToolStripComboBox
	/// to test contained in a form with the given form name. If there are more than one with the same name
	/// an AmbiguousNameException will be thrown.
	/// </summary>
	public ToolStripComboBoxTester(string name, string formName)
	  : base(name,formName)
	{
	}

	/// <summary>
	/// Constructs a ToolStripComboBox from the name of the ToolStripComboBox
	/// to test. If there are more than one with the same name
	/// an AmbiguousNameException will be thrown.
	/// </summary>
	/// <param name="name"></param>
	public ToolStripComboBoxTester(string name)
	  : base(name)
	{
	}

	/// <summary>
	/// Creates a ToolStripComboBoxTester from a ToolStripComboBoxTester and an index where the
	/// original tester's name is not unique.
	/// </summary>
	/// <remarks>
	/// It is best to use the overloaded Constructor that requires just the name
	/// parameter if possible.
	/// </remarks>
	/// <param name="tester">The ToolStripComboBoxTester.</param>
	/// <param name="index">The index to test.</param>
	public ToolStripComboBoxTester(ToolStripComboBoxTester tester, int index)
	  : base(tester, index)
	{
	}
	#endregion

	/// <summary>
	/// Provides access to all of the Properties of the CombBox.
	/// </summary>
	/// <remarks>
	/// Allows typed access to all of the properties of the underlying control.
	/// </remarks>
	/// <value>The underlying control.</value>
	public ToolStripComboBox Properties
	{
	  get
	  {
		return (ToolStripComboBox)Component;
	  }
	}

	public void Select(int index)
	{
	  this.Properties.SelectedIndex = index;
	}

	/// <summary>
	/// Sets the text property of the ToolStripComboBox to the specified value.
	/// </summary>
	/// <remarks>
	/// TODO: Also calls EndCurrentEdit() so that databinding will happen.
	/// </remarks>
	/// <param name="text">The specified value for the text property.</param>
	public void Enter(string text)
	{
	  Properties.Text = text;
	  // EndCurrentEdit("Text");
	}

	/// <summary>
	/// Selects an entry in the ToolStripComboBox according to its string value.
	/// </summary>
	/// <remarks>
	/// Sets the Selected Index property on the underlying control after calling
	/// FindStringExact
	/// </remarks>
	/// <param name="text">The string value of the entry to select.</param>
	public void Select(string text)
	{
	  int index;
	  if ((index = Properties.FindStringExact(text)) == -1)
	  {
		throw new FormsTestAssertionException("Could not find text '" + text + "' in ToolStripComboBox '" + name + "'");
	  }
	  Select(index);
	}
  }
}
