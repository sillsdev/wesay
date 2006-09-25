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

using System.Collections;
using System.Windows.Forms;

namespace NUnit.Extensions.Forms
{
	/// <summary>
	/// Internal use only.  Represents a collection of controls.
	/// </summary>
	/// <remarks>
	/// NUnitForms users should not have a need for this class.  When C# supports
	/// generics, this should be replaced.
	/// </remarks>
	public class ControlCollection
	{
		private ArrayList list = new ArrayList();

		/// <summary>
		/// Add a Control to the collection.
		/// </summary>
		/// <remarks>
		/// Will not add a duplicate control.  In this way, the collection acts like a Set.
		/// </remarks>
		/// <param name="control">The control to add.</param>
		public void Add(Control control)
		{
			if(!list.Contains(control))
			{
				list.Add(control);
			}
		}

		/// <summary>
		/// Add one ControlCollection to another.  Combines them into one collection.
		/// </summary>
		/// <param name="collection">The collection to merge with this one.</param>
		public void Add(ControlCollection collection)
		{
			foreach(Control control in collection)
			{
				Add(control);
			}
		}

		/// <summary>
		/// Returns the number of Controls in this ControlCollection.
		/// </summary>
		/// <remarks>
		/// Delegates to the Count property of the underlying collection.
		/// </remarks>
		/// <value>
		/// How many controls are in the collection.
		/// </value>
		public int Count
		{
			get
			{
				return list.Count;
			}
		}

		/// <summary>
		/// Returns an IEnumerator of the Controls in this collection.
		/// </summary>
		/// <returns>An enumerator of controls.</returns>
		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}

		/// <summary>
		/// Returns a Control from this collection according to its index.
		/// </summary>
		/// <remarks>
		/// delegates to the Item indexer of an underlying collection.
		/// </remarks>
		/// <value>
		/// The control at the index.
		/// </value>
		/// <param name="i">the index</param>
		public Control this[int i]
		{
			get
			{
				return (Control) list[i];
			}
		}
	}
}