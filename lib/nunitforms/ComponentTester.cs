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
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using NUnit.Extensions.Forms;

namespace NUnit.Extensions.Forms
{
	/// <summary>
	/// <para>
	/// A ComponentTester for any type of Component.  It is the base class for all
	/// ComponentTesters in the API.  It can also serve as a generic tester for all
	/// Components with no specifically implemented support.
	/// </para>
	/// <para>
	/// This ComponentTester looks for Forms and Components based on their name. So, it is sufficient to initialize
	/// a Form or Component which has a known name and passing this name to this ComponentTester during intialization. In
	/// the following code
	/// <code>
	/// new LabelTestForm().Show();
	/// ComponentTester label = new ComponentTester("myLabel");
	/// </code>
	/// the initialization of <c>LabelTestForm</c> sets its name as <c>myLabel</c>. <c>ComponentTester</c> looks for
	/// initialized Forms and Components based on their names. Passing <c>myLabel</c> during construction to <c>ComponentTester</c>
	/// allows <c>ComponentTester</c> to look for Forms and Components with the name <c>myLabel</c>. This happens in <c>GetComponentFinder()</c>.
	/// </para>
	/// <para>
	/// The following names are used by build in NUnitForm types :
	/// <list type="bullet">
	/// <li>LabelTestForm : myLabel</li>
	/// <li>AppForm : statusBar1 - speedBar - lblSpeed - gutter</li>
	/// </list>
	/// </para>
	/// <para>
	/// This <c>ComponentTester</c> encapsulates the <c>Component</c> under test. For instance, a click event is simulated
	/// by calling the <c>OnClick</c> method on the encapsulated <c>Component</c>.
	/// </para>
	/// </summary>
	/// <remarks>
	/// If you want to make your own ComponentTester for a custom or unsupported
	/// Component, you should implement a version of each of the four constructors.
	/// I plan to separate out (and generate) this code once we get partial class
	/// support in c#.
	/// You should also implement a Property named Properties that returns the
	/// underlying Component.
	/// You should hide the indexer (new) and implement one that returns the
	/// appropriate type.
	/// The ButtonTester class is a good place to look for an example (or cut and
	/// paste starting point) if you are making your own tester.</remarks>
	public class ComponentTester : IEnumerable
	{
		private Form form;

		private string formName;

		/// <summary>
		/// The name of the underlying Component.
		/// </summary>
		protected string name;

		private int index = -1;

		/// <summary>
		/// Creates a ComponentTester that will test Components with the specified name
		/// on a form with the specified name.
		/// </summary>
		/// <remarks>
		/// If the name is unique, you can operate on the tester directly, otherwise
		/// you should use the indexer or Enumerator properties to access each separate
		/// Component.</remarks>
		/// <param name="name">The name of the Component to test.</param>
		/// <param name="formName">The name of the form to test.</param>
		public ComponentTester(string name, string formName)
		{
			this.formName = formName;
			this.name = name;
		}

		/// <summary>
		/// Should call this method after editing something in order to trigger any
		/// databinding done with the Databindings collection.  (ie text box to a data
		/// set)
		/// </summary>
		public void EndCurrentEdit(string propertyName)
		{
			//TODO Waarschijnlijk niet nodig voor Component
			//if(Component.DataBindings[propertyName] != null)
			//{
			//    Component.DataBindings[propertyName].BindingManagerBase.EndCurrentEdit();
			//}
		}

		/// <summary>
		/// Creates a ComponentTester that will test Components with the specified name
		/// on the specified form.
		/// </summary>
		/// <remarks>
		/// If the name is unique, you can operate on the tester directly, otherwise
		/// you should use the indexer or Enumerator properties to access each separate
		/// Component.</remarks>
		/// <param name="name">The name of the Component to test.</param>
		/// <param name="form">The form to test.</param>
		public ComponentTester(string name, Form form)
		{
			this.form = form;
			this.name = name;
		}

		/// <summary>
		/// Creates a ComponentTester that will test Components with the specified name.
		/// </summary>
		/// <remarks>
		/// If the name is unique, you can operate on the tester directly, otherwise
		/// you should use the indexer or Enumerator properties to access each separate
		/// Component.</remarks>
		/// <param name="name">The name of the Component to test.</param>
		public ComponentTester(string name)
		{
			this.name = name;
		}

		/// <summary>
		/// Allows you to find a ComponentTester by index where the name is not unique.
		/// </summary>
		/// <remarks>
		/// When a Component is not uniquely identified by its name property, you can
		/// access it according to an index.  This should only be used when you have
		/// dynamic Components and it is inconvenient to set the Name property uniquely.
		///
		/// This was added to support the ability to find Components where their name is
		/// not unique.  If all of your Components are uniquely named (I recommend this) then
		/// you will not need this.
		/// </remarks>
		/// <value>The ComponentTester at the specified index.</value>
		/// <param name="index">The index of the ComponentTester.</param>
		public ComponentTester this[int index]
		{
			get
			{
				return new ComponentTester(this, index);
			}
		}

		/// <summary>
		/// Returns the number of Components associated with this tester.
		/// </summary>
		public int Count
		{
			get
			{
				return GetComponentFinder().Count;
			}
		}

		/// <summary>
		/// Returns uniquely qualified ComponentTesters for each of the Components
		/// associated with this tester as an IEnumerator.  This allows use of a
		/// foreach loop.
		/// </summary>
		/// <returns>IEnumerator of ComponentTesters (typed correctly)</returns>
		public IEnumerator GetEnumerator()
		{
			ArrayList list = new ArrayList();
			int count = Count;
			Type type = GetType();
			for(int i = 0; i < count; i++)
			{
				list.Add(Activator.CreateInstance(type, new object[] {this, i}));
			}
			return list.GetEnumerator();
		}

		/// <summary>
		/// Convenience method "Clicks" on the Component being tested if it is visible.
		/// </summary>
		/// <exception>
		/// ComponentNotVisibleException is thrown if the Component is not Visible.
		/// </exception>
		public virtual void Click()
		{
			//TODO Deze methode is waarschijnlijk niet nodig voor een Component.
			throw new ApplicationException("Code is not complete, yet.");
			//if(Component.Visible)
			//{
			//    FireEvent("Click");
			//}
			//else
			//{
			//    throw new ComponentNotVisibleException(name);
			//}
		}

		/// <summary>
		/// Convenience method retrieves the Text property of the tested Component.
		/// </summary>
		public virtual string Text
		{
			get
			{
				return Component.Site.Name;
			}
		}

		public ComponentTester(ComponentTester tester, int index)
		{
			if(index < 0)
			{
				throw new Exception("Should not have index < 0");
			}
			this.index = index;
			form = tester.form;
			formName = tester.formName;
			name = tester.name;
		}

		#region EventFiring

		/// <summary>
		/// Simulates firing of an event by the Component being tested.
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
			minfo.Invoke(Component, new object[] {Activator.CreateInstance(parameterType, args)});
		}

		/// <summary>
		/// Simulates firing of an event by the Component being tested.
		/// </summary>
		/// <param name="eventName">The name of the event to fire.</param>
		/// <param name="arg">The EventArgs object to pass as a parameter on the event.</param>
		public void FireEvent(string eventName, EventArgs arg)
		{
			MethodInfo minfo =
					Component.GetType().GetMethod("On" + eventName,
												BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			minfo.Invoke(Component, new object[] {arg});
		}

		#endregion

		#region Properties

		/// <summary>
		/// Convenience accessor / mutator for any nonsupported property on a Component
		/// to test.
		/// </summary>
		/// <example>
		/// ComponentTester t = new ComponentTester("t");
		/// t["Text"] = "a";
		/// AssertEqual("a", t["Text"]);
		/// </example>
		///
		public object this[string name]
		{
			get
			{
				return GetPropertyInfo(name).GetValue(Component, null);
			}
			set
			{
				GetPropertyInfo(name).SetValue(Component, value, null);
				EndCurrentEdit(name);
			}
		}

		private PropertyInfo GetPropertyInfo(string propertyName)
		{
			return
					Component.GetType().GetProperty(propertyName,
												  BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Convenience method invoker for any nonsupported method on a Component to test
		/// </summary>
		/// <param name="methodName">the name of the method to invoke</param>
		/// <param name="args">the arguments to pass into the method</param>
		public object Invoke(string methodName, params object[] args)
		{
			Type[] types = new Type[args.Length];
			for(int i = 0; i < types.Length; i++)
			{
				types[i] = args[i].GetType();
			}
			MethodInfo minfo =
					Component.GetType().GetMethod(methodName,
												BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
												null, types, null);
			return minfo.Invoke(Component, args);
		}

		#endregion

		//#region Mouse

		///// <summary>
		/////   Returns a <see cref="NUnit.Extensions.Forms.MouseComponentler"/> that
		/////   can be used with the <see cref="Component">Component under test</see>.
		/////
		/////   It would be better to use the MouseComponentler on the base test class so
		/////   that you don't have to worry about disposing it after your test.  I think
		/////   this may be marked obsolete soon.
		///// </summary>
		///// <returns>
		/////   A <see cref="NUnit.Extensions.Forms.MouseComponentler"/>.
		///// </returns>
		///// <remarks>
		/////   <b>MouseComponentler</b> returns a new instance of a <see cref="NUnit.Extensions.Forms.MouseComponentler"/>
		/////   that can be used with the <see cref="Component">Component under test</see>.
		/////   All <see cref="NUnit.Extensions.Forms.MouseComponentler.Position">mouse positions</see> are relative
		/////   the Component.
		/////   <para>
		/////   The returned <b>MouseComponentler</b> must be <see cref="NUnit.Extensions.Forms.MouseComponentler.Dispose">disposed</see>
		/////   to restore the mouse settings prior to the testing; which can be accomplished with the <c>using</c>
		/////   statement.
		/////   </para>
		///// </remarks>
		///// <example>
		///// <code>
		///// TextBoxTester textBox = new TextBoxTester("myTextBox");
		///// using (MouseComponentler mouse = textBox.MouseComponentler())
		///// {
		/////   mouse.Position = new PointF(1,1);
		/////   mouse.Drag(30,1);
		///// }
		///// </code>
		///// </example>
		//public MouseComponentler MouseComponentler()
		//{
		//    return new MouseComponentler(this);
		//}

		//#endregion

		/// <summary>
		/// The underlying Component for this tester.
		/// </summary>
		protected internal IComponent Component
		{
			get
			{
				return GetComponentFinder().Find(index);
			}
		}

		private ComponentFinder GetComponentFinder()
		{
			if(form != null)
			{
				//may have dynamically added Components.  I am not saving this.
				return new ComponentFinder(name, form);
			}
			else if(formName != null)
			{
				return new ComponentFinder(name, new FormFinder().Find(formName));
			}
			else
			{
				return new ComponentFinder(name);
			}
		}
	}
}