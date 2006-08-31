using System.ComponentModel;
using System.Windows.Forms;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	/// <summary>
	/// A Layouter is responsible for filling a detailed list with the contents
	/// of a single data object (e.g. LexSense, LexExample), etc.
	/// There are will normally be a single subclass per class of data,
	/// and each of these layout erstwhile call a different layouter for each
	/// child object (e.g. LexEntryLayouter would employ a SenseLayouter to display senses).
	/// </summary>
	public abstract class Layouter
	{
		/// <summary>
		/// The DetailList we are filling.
		/// </summary>
		protected DetailList _detailList;

		public Layouter(DetailList builder)
		{
			_detailList = builder;
		}

		/// <summary>
		/// actually add the widget's that are needed to the detailed list
		/// </summary>
		/// <param name="dataObject"></param>
		/// <returns></returns>
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

		protected virtual void OnGhostBindingTriggered(GhostBinding sender, object newDataTarget, System.EventArgs args)
		{

		}
	}
}