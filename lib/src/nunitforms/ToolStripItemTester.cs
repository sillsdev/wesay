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
  public class ToolStripItemTester
  {
	#region Private/Protected members

	private Form form;

	private string formName;

	protected string name;

	private int index = -1;

	#endregion

	#region Constructors

	public ToolStripItemTester(string name, Form form)
	{
	  this.form = form;
	  this.name = name;
	}

	public ToolStripItemTester(string name, string formName)
	{
	  this.formName = formName;
	  this.name = name;
	}

	public ToolStripItemTester(string name)
	{
	  this.name = name;
	}

	public ToolStripItemTester(ToolStripItemTester tester, int index)
	{
	  if (index < 0)
	  {
		throw new Exception("Should not have index < 0");
	  }
	  this.index = index;
	  form = tester.form;
	  formName = tester.formName;
	  name = tester.name;
	}
	#endregion

	#region Accessors
	protected ToolStripItem Component
	{
	  get
	  {
		if (form != null)
		{
		  //may have dynamically added controls.  I am not saving this.
		  return new ToolStripItemFinder(name, form).Find();
		}
		else if (formName != null)
		{
		  return new ToolStripItemFinder(name, new FormFinder().Find(formName)).Find();
		}
		else
		{
		  return new ToolStripItemFinder(name).Find();
		}
	  }
	}

	#endregion

	/// <summary>
	/// Clicks the MenuItem (activates it)
	/// </summary>
	public virtual void Click()
	{
	  FireEvent("Click");
	}

	public virtual void DoubleClick()
	{

	  FireEvent("DoubleClick");
	}

	/// <summary>
	/// Gets the text of this MenuItem.
	/// </summary>
	public string Text
	{
	  get
	  {
		return Component.Text;
	  }
	}

	#region EventFiring

	protected void FireEvent(string eventName)
	{
	  MethodInfo minfo = Component.GetType().GetMethod("On" + eventName,
										   BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
	  ParameterInfo[] param = minfo.GetParameters();
	  Type parameterType = param[0].ParameterType;
	  minfo.Invoke(Component, new Object[] { Activator.CreateInstance(parameterType) });
	}

	/// <summary>
	/// Simulates firing of an event by the control being tested.
	/// </summary>
	/// <param name="eventName">The name of the event to fire.</param>
	/// <param name="args">The optional arguments required to construct the EventArgs for the specified event.</param>
	public void FireEvent(string eventName, params object[] args)
	{
	  MethodInfo minfo =
			  Component.GetType().GetMethod("On" + eventName,
										  BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
	  ParameterInfo[] param = minfo.GetParameters();
	  Type parameterType = param[0].ParameterType;
	  minfo.Invoke(Component, new object[] { Activator.CreateInstance(parameterType, args) });
	}

	/// <summary>
	/// Simulates firing of an event by the control being tested.
	/// </summary>
	/// <param name="eventName">The name of the event to fire.</param>
	/// <param name="arg">The EventArgs object to pass as a parameter on the event.</param>
	public void FireEvent(string eventName, EventArgs arg)
	{
	  MethodInfo minfo =
			  Component.GetType().GetMethod("On" + eventName,
										  BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
	  minfo.Invoke(Component, new object[] { arg });
	}

	#endregion

  }
}
