using System;
using System.Collections.Generic;
using System.Text;
using Gtk;
using Glade;
using WeSay.Core;

namespace WeSay.UI
{
	class ViewHandler
	{
		protected LexiconModel _model;
		public static Gtk.Notebook s_tabcontrol;

	public LexiconModel Model
	{
		set { _model = value; }
	}
  }
 }
