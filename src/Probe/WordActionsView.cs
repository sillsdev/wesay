using System;
using System.Collections.Generic;
using System.Text;
using Gtk;
using Glade;
using WeSay.Core;

namespace WeSay.UI
{
	class WordActionsView
	{
		protected LexiconModel _model;

#pragma warning disable 649
		[Widget]
		protected Gtk.VBox _rootVBox;
#pragma warning restore 649


		public WordActionsView(Container container, LexiconModel model)
		{
			_model = model;

			Glade.XML gxml = new Glade.XML("probe.glade", "_actionsViewHolder", null);
			gxml.Autoconnect(this);

			_rootVBox.Reparent(container);
		}


	}
}
