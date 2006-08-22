using System.ComponentModel;
using Gtk;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public abstract class Layouter
	{
		protected TableBuilder _builder;

	   public Layouter(TableBuilder builder)
		{
			_builder = builder;
		}
		public abstract int AddWidgets(object dataObject);

		protected Entry MakeBoundEntry(WeSay.Language.MultiText text, string writingSystemId)
		{
			Gtk.Entry entry = new Gtk.Entry(text[writingSystemId]);
			WeSay.UI.Binding binding = new WeSay.UI.Binding(text, writingSystemId, entry);
			return entry;
		}

		protected Entry MakeGhostEntry(IBindingList list, string ghostPropertyName, string writingSystemId)
		{
			Gtk.Entry entry = new Gtk.Entry();
			WeSay.UI.GhostBinding binding = new WeSay.UI.GhostBinding(list, ghostPropertyName, writingSystemId, entry);
			return entry;
		}
	}
}