#region Copyright (c) 2003-2005, Bart De Boeck

/********************************************************************************************************************
'
' Copyright (c) 2003-2005, Bart De Boeck
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

using NUnit.Extensions.Forms;

using FormCollection=NUnit.Extensions.Forms.FormCollection;

namespace NUnit.Extensions.Forms
{
	/// <summary>
	/// Internal use only.  Finds Components according to their name property.
	/// </summary>
	/// <remarks>
	/// It is also used by the recorder application.
	/// </remarks>
	/// the recorder application.
	public class ComponentFinder : Finder
	{
		private string name;

		private FormCollection forms = null;

		/// <summary>
		/// Creates a ComponentFinder that will find Components on a specific Form according to their name.
		/// </summary>
		/// <param name="name">The name of the Component to find.</param>
		/// <param name="form">The form to search for the Component.</param>
		public ComponentFinder(string name, Form form)
		{
			this.name = name;
			if(form != null)
			{
				forms = new FormCollection();
				forms.Add(form);
			}
		}

		/// <summary>
		/// Creates a ComponentFinder that will find Components according to their name.
		/// </summary>
		/// <param name="name">The name of the Component to find.</param>
		public ComponentFinder(string name)
		{
			this.name = name;
		}

		/// <summary>
		/// Finds a Component.
		/// </summary>
		/// <exception>
		/// If there is more than one with the specified name, it will
		/// throw an AmbiguousNameException.  If the Component does not exist, it will throw
		/// a NoSuchComponentException.
		/// </exception>
		/// <returns>The Component if one is found.</returns>
		public IComponent Find()
		{
			return Find(-1);
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

		internal int Count
		{
			get
			{
				return FindComponents().Components.Count;
			}
		}

		private Container FindComponents()
		{
			Container found = new Container();
			foreach(Form form in FormCollection)
			{
				found.Add(Find(name, form));
			}
			return found;
		}

		internal IComponent Find(int index)
		{
			Container found = FindComponents();
			if(index < 0)
			{
				if(found.Components.Count == 1)
				{
					return found.Components[0];
				}
				else if (found.Components.Count == 0)
				{
					throw new NoSuchComponentException(name);
				}
				else
				{
					throw new AmbiguousNameException(name);
				}
			}
			else
			{
				if (found.Components.Count > index)
				{
					return found.Components[index];
				}
				else
				{
					throw new NoSuchComponentException(name + "[" + index + "]");
				}
			}
		}

		private IComponent Find(string name, Component Component)
		{
			Container results = new Container();
			foreach(Component c in Component.Container.Components)
			{
				results.Add(Find(name,c));
			}
				if(Matches(name, Component))
			{
				//results.Add(Component);
				return Component;
			}
			else
			{
				throw new ApplicationException("Code is not complete, yet.");
			}

			//TODO : Control c in Control.Components kan, maar waarschijnlijk kan
			//een Component geen andere Components bevatten.
			//foreach(Component c in Component.Components)
			//{
			//    results.Add(Find(name, c));
			//}

			//return results;
		}

		private bool Matches(string name, object Component)
		{
			object c = Component;
			string[] names = name.Split('.');
			for(int i = names.Length - 1; i >= 0; i--)
			{
				if(!names[i].Equals(Name(c)))
				{
					return false;
				}
				c = Parent(c);
			}
			return true;
		}
	}
}