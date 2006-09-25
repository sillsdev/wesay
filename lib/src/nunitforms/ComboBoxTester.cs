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
	/// A ControlTester for testing ComboBoxes.
	/// </summary>
	/// <remarks>
	/// Has convenience methods for Selecting items and Entering text.
	/// <para>
	/// Fully supported by the recorder application
	/// </para>
	/// </remarks>
	public class ComboBoxTester : ControlTester
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
		public ComboBoxTester(string name, Form form) : base(name, form)
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
		public ComboBoxTester(string name, string formName) : base(name, formName)
		{
		}

		/// <summary>
		/// Creates a ControlTester from the control name.
		/// </summary>
		/// <remarks>
		/// This is the best constructor.</remarks>
		/// <param name="name">The Control name.</param>
		public ComboBoxTester(string name) : base(name)
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
		public ComboBoxTester(ControlTester tester, int index) : base(tester, index)
		{
		}

		/// <summary>
		/// Allows you to find a ComboBoxTester by index where the name is not unique.
		/// </summary>
		/// <remarks>
		/// This was added to support the ability to find controls where their name is
		/// not unique.  If all of your controls are uniquely named (I recommend this) then
		/// you will not need this.
		/// </remarks>
		/// <value>The ControlTester at the specified index.</value>
		/// <param name="index">The index of the ComboBoxTester.</param>
		public new ComboBoxTester this[int index]
		{
			get
			{
				return new ComboBoxTester(this, index);
			}
		}

		/// <summary>
		/// Provides access to all of the Properties of the ComboBox.
		/// </summary>
		/// <remarks>
		/// Allows typed access to all of the properties of the underlying control.
		/// </remarks>
		/// <value>The underlying control.</value>
		public ComboBox Properties
		{
			get
			{
				return (ComboBox) Control;
			}
		}

		/// <summary>
		/// Sets the text property of the ComboBox to the specified value.
		/// </summary>
		/// <remarks>
		/// Also calls EndCurrentEdit() so that databinding will happen.
		/// </remarks>
		/// <param name="text">The specified value for the text property.</param>
		public void Enter(string text)
		{
			Properties.Text = text;
			EndCurrentEdit("Text");
		}

		/// <summary>
		/// Selects an entry in the ComboBox according to its index.
		/// </summary>
		/// <remarks>
		/// Sets the SelectedIndex property on the underlying control.
		/// </remarks>
		/// <param name="i">The index of the ComboBox entry to select.</param>
		public void Select(int i)
		{
			Properties.SelectedIndex = i;
		}

		/// <summary>
		/// Selects an entry in the ComboBox according to its string value.
		/// </summary>
		/// <remarks>
		/// Sets the Selected Index property on the underlying control after calling
		/// FindStringExact
		/// </remarks>
		/// <param name="text">The string value of the entry to select.</param>
		public void Select(string text)
		{
			int index;
			if((index = Properties.FindStringExact(text)) == -1)
			{
				throw new FormsTestAssertionException("Could not find text '" + text + "' in ComboBox '" + name + "'");
			}
			Select(index);
		}
	}
}