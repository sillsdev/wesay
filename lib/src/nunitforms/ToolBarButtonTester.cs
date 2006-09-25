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

using System.Windows.Forms;

namespace NUnit.Extensions.Forms
{
	public class ToolBarButtonTester
	{
		private ToolBarTester bar;

		private ToolBarButton button;


		public ToolBarButtonTester(ToolBarButton button, ToolBarTester bar)
		{
			this.bar = bar;
			this.button = button;
		}

		#region Properties

		/// <summary>
		/// The toolbar that this button refers too
		/// </summary>
		public ToolBar Bar
		{
			get
			{
				return bar.Properties;
			}
		}

		/// <summary>
		/// The button that this tester encapsulates
		/// </summary>
		public ToolBarButton Button
		{
			get
			{
				return button;
			}
		}

		/// <summary>
		/// Helper method to get the dropdown menu from a button
		/// </summary>
		public Menu DropDownMenu
		{
			get
			{
				FormsAssert.IsTrue(button.Style == ToolBarButtonStyle.DropDownButton);
				return button.DropDownMenu;
			}
		}

		public bool Pushed
		{
			set
			{
				Button.Pushed = value;
			}
			get
			{
				return Button.Pushed;
			}
		}

		public bool PartialPushed
		{
			set
			{
				Button.PartialPush = value;
			}
			get
			{
				return Button.PartialPush;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Click a pushbutton
		/// </summary>
		public void Click()
		{
			FormsAssert.IsTrue((button.Style == ToolBarButtonStyle.PushButton) ||
							   (button.Style == ToolBarButtonStyle.ToggleButton));
			ToolBarButtonClickEventArgs buttonArg = new ToolBarButtonClickEventArgs(Button);
			bar.FireEvent("ButtonClick", buttonArg);
		}

		/// <summary>
		/// Click an item in a dropdown menu
		/// </summary>
		/// <param name="menuText">The name of the menu item to click</param>
		public void ClickDropDownMenuItem(string menuText)
		{
			FormsAssert.IsTrue(button.Style == ToolBarButtonStyle.DropDownButton);
			foreach(MenuItem item in button.DropDownMenu.MenuItems)
			{
				if(item.Text == menuText)
				{
					item.PerformClick();
					return;
				}
			}
		}

		public void Push()
		{
			FormsAssert.IsTrue(button.Style == ToolBarButtonStyle.ToggleButton);
			Pushed = !Pushed;
		}

		public void PartialPush()
		{
			FormsAssert.IsTrue(button.Style == ToolBarButtonStyle.ToggleButton);
			PartialPushed = !PartialPushed;
		}

		#endregion
	}
}