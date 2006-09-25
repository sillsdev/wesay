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
	/// Internal use only.  This class finds MenuItems according to a name.
	/// </summary>
	/// <remarks>
	/// It is also used by the recorder application to determine names of Controls.
	/// </remarks>
	public class MenuItemFinder : Finder
	{
		private string name;

		private FormCollection forms;

		/// <summary>
		/// Creates a MenuItemFinder that will find MenuItems according to their name
		/// and qualified by a form instance.
		/// </summary>
		/// <param name="name">The MenuItem name.</param>
		/// <param name="form">The form instance.</param>
		public MenuItemFinder(string name, Form form)
		{
			this.name = name;
			if(form != null)
			{
				forms = new FormCollection();
				forms.Add(form);
			}
		}

		/// <summary>
		/// Creates a MenuItemFinder that will find a MenuItem according to its name.
		/// </summary>
		/// <param name="name"></param>
		public MenuItemFinder(string name)
		{
			this.name = name;
		}

		private FormCollection FormCollection
		{
			get
			{
				if(forms == null)
				{
					return new FormFinder().FindAll();
				}
				return forms;
			}
		}

		/// <summary>
		/// Finds the MenuItem.
		/// </summary>
		/// <returns>the MenuItem found by this MenuItemFinder.</returns>
		/// <exception cref="NoSuchControlException">
		/// Thrown if this MenuItem is not found.</exception>
		/// <exception cref="AmbiguousNameException">
		/// Thrown if multiple MenuItems are found with the same name.</exception>
		public MenuItem Find()
		{
			MenuItemCollection found = new MenuItemCollection();
			foreach(Form form in FormCollection)
			{
				if(form.Menu != null)
				{
					found.Add(Find(name, form.Menu, form));
				}
				found.Add(Find(name, form));
			}

			if(found.Count == 1)
			{
				return found[0];
			}
			else if(found.Count == 0)
			{
				throw new NoSuchControlException(name);
			}
			else
			{
				throw new AmbiguousNameException(name);
			}
		}

		private MenuItemCollection Find(string name, Control control)
		{
			MenuItemCollection results = new MenuItemCollection();

			foreach(Control c in control.Controls)
			{
				results.Add(Find(name, c));
				if(c.ContextMenu != null)
				{
					results.Add(Find(name, c.ContextMenu, c));
				}
			}

			return results;
		}

		private MenuItemCollection Find(string name, Menu menu, Control sourceControl)
		{
			MenuItemCollection results = new MenuItemCollection();

			if(Matches(name, menu, sourceControl))
			{
				results.Add((MenuItem) menu);
			}
			foreach(MenuItem m in menu.MenuItems)
			{
				results.Add(Find(name, m, sourceControl));
			}

			return results;
		}

		private bool Matches(string name, Menu menu, Control sourceControl)
		{
			if(!(menu is MenuItem))
			{
				return false;
			}

			object m = menu;
			string[] names = name.Split('.');
			for(int i = names.Length - 1; i >= 0; i--)
			{
				//for a menu item, uses text instead of name because
				//there is no name property which is confusing.
				if(!names[i].Equals(Name(m)))
				{
					return false;
				}
				m = Parent(m);
				if(m == null)
				{
					m = sourceControl;
				}
			}
			return true;
		}
	}
}