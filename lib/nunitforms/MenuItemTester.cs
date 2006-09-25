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

using System;
using System.Reflection;
using System.Windows.Forms;

namespace NUnit.Extensions.Forms
{
	/// <summary>
	/// A ControlTester for MenuItems.
	/// </summary>
	/// <remarks>
	/// It does not extend ControlTester because MenuItems are not Controls.  (sadly)</remarks>
	public class MenuItemTester
	{
		private Form form;

		private string formName;

		protected string name;

		protected MenuItem MenuItem
		{
			get
			{
				if(form != null)
				{
					//may have dynamically added controls.  I am not saving this.
					return new MenuItemFinder(name, form).Find();
				}
				else if(formName != null)
				{
					return new MenuItemFinder(name, new FormFinder().Find(formName)).Find();
				}
				else
				{
					return new MenuItemFinder(name).Find();
				}
			}
		}

		public MenuItemTester(string name, Form form)
		{
			this.form = form;
			this.name = name;
		}

		public MenuItemTester(string name, string formName)
		{
			this.formName = formName;
			this.name = name;
		}

		public MenuItemTester(string name)
		{
			this.name = name;
		}

		/// <summary>
		/// Clicks the MenuItem (activates it)
		/// </summary>
		public virtual void Click()
		{
			FireEvent("Click");
		}

		/// <summary>
		/// Pops up a menu.
		/// </summary>
		public virtual void Popup()
		{
			FireEvent("Popup");
		}

		/// <summary>
		/// Gets the text of this MenuItem.
		/// </summary>
		public string Text
		{
			get
			{
				return MenuItem.Text;
			}
		}

		/// <summary>
		/// Allows you to access any properties of this MenuItem.
		/// </summary>
		public MenuItem Properties
		{
			get
			{
				return MenuItem;
			}
		}

		#region EventFiring

		protected void FireEvent(string eventName)
		{
			MethodInfo minfo =
					MenuItem.GetType().GetMethod("On" + eventName,
												 BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			ParameterInfo[] param = minfo.GetParameters();
			Type parameterType = param[0].ParameterType;
			minfo.Invoke(MenuItem, new Object[] {Activator.CreateInstance(parameterType)});
		}

		#endregion
	}
}