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
using System.Collections.Generic;

namespace NUnit.Extensions.Forms
{
  /// <summary>
  /// This class is used to find ToolStripItems such as ToolsStripMenuItems, ToolStripButtons etc.
  /// These classes was added in .NET 2.0
  /// </summary>
  public class ToolStripItemFinder : Finder
  {
	#region private members

	/// <summary>
	/// Name of the item to finx
	/// </summary>
	///
	private string name = string.Empty;
	/// <summary>
	///  Forms we should search in.
	/// </summary>
	private FormCollection forms = null;

	#endregion

	#region constructors

	/// <summary>
	/// Constructor used to search for a ToolStripItem in a given form. If the form
	/// is null we seach in all forms in the current application.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="form"></param>
	public ToolStripItemFinder(string name, Form form)
	{
	  this.name = name;
	  if (form != null)
	  {
		forms = new FormCollection();
		forms.Add(form);
	  }
	}

	/// <summary>
	/// Constructor used to seach for ToolStripItems in all forms in current application.
	/// </summary>
	/// <param name="name"></param>
	public ToolStripItemFinder(string name)
	{
	  this.name = name;
	}

	#endregion

	#region Public functions

	/// <summary>
	/// Finds a ToolStripItemFinder.
	/// </summary>
	/// <exception>
	/// If there is more than one with the specified name, it will
	/// throw an AmbiguousNameException.  If the ToolStripItemFinder does not exist, it will throw
	/// a NoSuchControlException.
	/// </exception>
	/// <returns>The control if one is found.</returns>
	public ToolStripItem Find()
	{
	  return Find(-1);
	}

	/// <summary>
	/// Finds all ToolStripItems within either the given form or if no form is given
	/// within all forms in the current application and returns the ToolStripItem at
	/// the given index.
	/// If no control is found an NoSuchComponentException  is thrown.
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public ToolStripItem Find(int index)
	{
	  List<ToolStripItem> found = FindToolStripItems();

	  if (index < 0)
	  {
		if (found.Count == 1)
		{
		  return found[0];
		}
		else if (found.Count == 0)
		{
		  throw new NoSuchComponentException(this.name);
		}
		else
		{
		  throw new AmbiguousNameException(this.name);
		}
	  }
	  else
	  {
		if (found.Count > index)
		{
		  return found[index];
		}
		else
		{
		  throw new NoSuchControlException(this.name + "[" + index + "]");
		}
	  }
	}

	public List<ToolStripItem> Find(string name, ToolStripItemCollection collection)
	{
	  List<ToolStripItem> items = new List<ToolStripItem>();
	  foreach (ToolStripItem item in collection)
	  {
		// TODO: Replace this with Matches function ?
		if (string.Equals(name, item.Name))
		{
		  items.Add(item);
		}

		if( item is ToolStripDropDownItem)
		{
		  ToolStripDropDownItem dropDownItem = (ToolStripDropDownItem)item;
		  items.AddRange(Find(name,dropDownItem.DropDownItems));
		}
	  }
	  return items;
	}

	/// <summary>
	/// Helper function to find ToolStripItems in a ControlCollection. In .NET 2.0
	/// a MenuStrip is a control which be placed in ther controls in a form.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="collection"></param>
	/// <returns></returns>
	protected List<ToolStripItem> Find(string name, Control.ControlCollection collection)
	{
	  List<ToolStripItem> items = new List<ToolStripItem>();
	  foreach (Control c in collection)
	  {
		if (c.Controls != null)
		{
		  items.AddRange(Find(name, c.Controls));
		}

		if(c is ToolStrip)
		{
		  ToolStrip toolStrip = (ToolStrip)c;
		  items.AddRange(Find(this.name, toolStrip.Items));
		}

		if (c.ContextMenuStrip != null)
		{
		  items.AddRange(Find(this.name,c.ContextMenuStrip.Items));
		}
	  }

	  return items;
	}

	private List<ToolStripItem> FindToolStripItems()
	{
	  List<ToolStripItem> found = new List<ToolStripItem>();
	  foreach (Form form in FormCollection)
	  {
		if(form.Controls != null)
		{
		  found.AddRange(Find(this.name,form.Controls));
		}
	  }
	  return found;
	}

	private FormCollection FormCollection
	{
	  get
	  {
		if (forms == null)
		{
		  return new FormFinder().FindAll();
		}
		return forms;
	  }
	}

	/// <summary>
	/// Returns the number of ToolStipItems with the give name. If no name is given it returns all
	///
	/// </summary>
	internal int Count
	{
	  get
	  {
		return FindToolStripItems().Count;
	  }
	}

	#endregion
  }
}
