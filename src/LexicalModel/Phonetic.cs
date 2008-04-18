using System;
using System.Collections.Generic;
using System.Text;
using WeSay.Foundation;

namespace WeSay.LexicalModel
{/*
	public class Phonetic : WeSay.Foundation.WeSayDataObject
	{
		private MultiText _contents;
		private MultiText _media;

		public Phonetic(WeSayDataObject parent)
			: base(parent)
		{
			_contents = new MultiText(this);
			_media = new MultiText(this);

			WireUpEvents();
		}


		/// <summary>
		/// Used when a list of these items adds via "AddNew", where we have to have a default constructor.
		/// The parent is added in an even handler, on the parent, which is called by the list.
		/// </summary>
		public Phonetic()
			: this(null)
		{
		}

		protected override void WireUpEvents()
		{
			base.WireUpEvents();
			WireUpChild(_sentence);
			WireUpChild(_translation);
		}

		public override bool IsEmpty
		{
			get
			{
				return Contents.Empty &&
					   Media.Empty &&
					   !HasProperties;
			}
		}

		private MultiText Media
		{
			get { return _media; }
		}

		public MultiText Contents
		{
			get { return _contents; }
		}
	}*/
}
