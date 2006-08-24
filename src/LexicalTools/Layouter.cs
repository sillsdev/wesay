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

			//WeSay.UI.Binding binding = new WeSay.UI.Binding(text, writingSystemId, entry);
			return entry;
		}

		protected Control MakeGhostEntry(IBindingList list, string ghostPropertyName, string writingSystemId)
		{
			TextBox entry = new TextBox();
			//WeSay.UI.GhostBinding binding = new WeSay.UI.GhostBinding(list, ghostPropertyName, writingSystemId, entry);
			//binding.Triggered += new GhostBinding.GhostTriggered(binding_Triggered);
			return entry;
		}

		protected virtual void binding_Triggered(object sender, System.EventArgs args)
		{

		}
	}
}