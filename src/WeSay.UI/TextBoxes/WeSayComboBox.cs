using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SIL.WritingSystems;

namespace WeSay.UI.TextBoxes
{
	public interface IWeSayComboBox
	{

		Size GetPreferredSize(Size proposedSize);

		[Browsable(false)]
		string Text { set; get; }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		WritingSystemDefinition WritingSystem { get; set; }

		bool Sorted { get; set; }
		AutoCompleteMode AutoCompleteMode { get; set; }
		AutoCompleteSource AutoCompleteSource { get; set; }
		int MaxDropDownItems { get; set; }
		Object SelectedItem { get; }
		int SelectedIndex { get; set; }
		ComboBoxStyle DropDownStyle { get; set; }
		int Length { get; }
		int Height { get; set; }
		Font Font { get; set; }
		Color BackColor { get; set; }
		Point Location { get; set; }
		Padding Margin { get; set; }
		AnchorStyles Anchor { get; set; }
		string Name { get; set; }
		int TabIndex { get; set; }
		Size Size { get; set; }
		DrawMode DrawMode { get; set; }
		FlatStyle FlatStyle { get; set; }
		String SelectedText { get; }
		Object GetItem(int i);
		void Clear();
		void AddItem(Object item);
		void ListCompleted();

		event EventHandler SelectedValueChanged;
		event DrawItemEventHandler DrawItem;
		event MeasureItemEventHandler MeasureItem;
	}

	public class WeSayComboBox : ComboBox, IWeSayComboBox
	{
		private WritingSystemDefinition _writingSystem;
		public new event EventHandler<DrawItemEventArgs> DrawItem;

		public WritingSystemDefinition WritingSystem
		{
			get
			{
				if (_writingSystem == null)
				{
					throw new InvalidOperationException(
						"Input system must be initialized prior to use.");
				}
				return _writingSystem;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				_writingSystem = value;
			}
		}

		public void AddItem(Object item)
		{
			this.Items.Add(item);
		}

		public void Clear()
		{
			this.Items.Clear();
		}

		public int Length
		{
			get
			{
				return this.Items.Count;
			}
		}

		public void ListCompleted()
		{
			return;
		}

		public Object GetItem(int i)
		{
			return this.Items[i];
		}

	}
}
