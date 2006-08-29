using System.ComponentModel;
using System.Windows.Forms;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public abstract class Layouter
	{
		protected DetailList _builder;

		public Layouter(DetailList builder)
		{
			_builder = builder;
		}
		public abstract int AddWidgets(object dataObject);

		protected Control MakeBoundEntry(WeSay.Language.MultiText text, string writingSystemId)
		{
			TextBox entry = new TextBox();
			entry.Text = text[writingSystemId];

			WeSay.UI.Binding binding = new WeSay.UI.Binding(text, writingSystemId, entry);
			return entry;
		}

		protected Control MakeGhostEntry(IBindingList list, string ghostPropertyName, string writingSystemId)
		{
			TextBox entry = new TextBox();
			WeSay.UI.GhostBinding binding = new WeSay.UI.GhostBinding(list, ghostPropertyName, writingSystemId, entry);
			binding.Triggered += new GhostBinding.GhostTriggered(OnGhostBindingTriggered);
			return entry;
		}

		protected GhostBinding MakeGhostBinding(IBindingList list, string ghostPropertyName, string writingSystemId,
			TextBox entry)
		{
			WeSay.UI.GhostBinding binding = new WeSay.UI.GhostBinding(list, ghostPropertyName, writingSystemId, entry);
			binding.Triggered += new GhostBinding.GhostTriggered(OnGhostBindingTriggered);
			return binding;
		}
		protected virtual void OnGhostBindingTriggered(GhostBinding sender, object newGuy, System.EventArgs args)
		{

		}
	}
}