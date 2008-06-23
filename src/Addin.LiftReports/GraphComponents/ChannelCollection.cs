#region copyright info

//
// Written by Anup. V (anupshubha@yahoo.com)
// Copyright (c) 2006.
//
// This code may be used in compiled form in any way you desire. This
// file may be redistributed by any means PROVIDING it is not sold for
// for profit without the authors written consent, and providing that
// this notice and the authors name is included. If the source code in
// this file is used in any commercial application then acknowledgement
// must be made to the author of this file (in whatever form you wish).
//
// This file is provided "as is" with no expressed or implied warranty.
//
// Please use and enjoy. Please let me know of any bugs/mods/improvements
// that you have found/implemented and I will fix/incorporate them into
// this file.

#endregion copyright info

using System;
using System.Collections;

namespace Addin.LiftReports.GraphComponents
{
	/// <summary>
	/// This class provides a mechanism for the user to treat the channels of
	/// the plotter and CRO graphs as a single collection.
	/// </summary>
	public class ChannelCollection: CollectionBase
	{
		/// <summary>
		/// Gets or sets the Channel object at the particular index
		/// </summary>
		public Channel this[int index]
		{
			get { return (Channel) List[index]; }
			set { List[index] = value; }
		}

		/// <summary>
		/// Adds a new Channel into the plotter
		/// </summary>
		public int Add(Channel value)
		{
			if (value == null)
			{
				throw new ArgumentNullException();
			}

			return (List.Add(value));
		}

		/// <summary>
		/// Returns the index of a specified channel in the plotter
		/// </summary>
		public int IndexOf(Channel value)
		{
			return (List.IndexOf(value));
		}

		/// <summary>
		/// Inserts a channel in the index specified in the plotter
		/// </summary>
		public void Insert(int index, Channel value)
		{
			if (value == null || index >= List.Count)
			{
				return;
			}

			List.Insert(index, value);
		}

		/// <summary>
		/// Removes the specified channel from the plotter
		/// </summary>
		public void Remove(Channel value)
		{
			if (value == null)
			{
				return;
			}

			if (! List.Contains(value))
			{
				throw new ArgumentException();
			}

			for (int i = 0;i < List.Count;i ++)
			{
				if (((Channel) List[i]).YAxisName == value.YAxisName)
				{
					List.RemoveAt(i);
					break;
				}
			}
		}

		/// <summary>
		/// Checks whether a channel is contained in the plotter
		/// </summary>
		/// <param name="value">Channel to check for</param>
		/// <returns>True if it is in the stacked bar graph. False otherwise</returns>
		public bool Contains(Channel value)
		{
			// If value is not of type Channel, this will return false.
			return (List.Contains(value));
		}

		protected override void OnInsert(int index, Object value)
		{
			// Insert additional code to be run only when inserting values.
		}

		protected override void OnRemove(int index, Object value)
		{
			// Insert additional code to be run only when removing values.
		}

		protected override void OnSet(int index, Object oldValue, Object newValue)
		{
			// Insert additional code to be run only when setting values.
		}

		protected override void OnValidate(Object value)
		{
			if (value.GetType() != Type.GetType("GraphComponents.Channel"))
			{
				throw new ArgumentException();
			}
		}

		public static void CopyTo(ChannelCollection collection, Int32 index)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}

			if (index < 0 || index >= collection.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}

			// TODO: Implement this later
			throw new InvalidOperationException();
		}
	}
}