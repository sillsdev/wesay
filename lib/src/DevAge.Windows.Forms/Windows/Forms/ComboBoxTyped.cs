using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace DevAge.Windows.Forms
{
	/// <summary>
	/// Control to simulate a ComboBox, because the one provided with the Framework doesn't support vertical sizing different from the size of the font.
	/// </summary>
	public class ComboBoxTyped : EditableControlBase
	{
		private DevAge.Windows.Forms.DropDownButton btDown;
		private DevAge.Windows.Forms.TextBoxTyped txtBox;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Constructor
		/// </summary>
		public ComboBoxTyped()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			btDown.BackColor = Color.FromKnownColor(KnownColor.Control);
			txtBox.LoadingValidator += new EventHandler(txtBox_LoadingValidator);
			txtBox.ValueChanged += new EventHandler(txtBox_ValueChanged);

			SetContentAndButtonLocation(txtBox, btDown);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btDown = new DevAge.Windows.Forms.DropDownButton();
			this.txtBox = new DevAge.Windows.Forms.TextBoxTyped();
			this.SuspendLayout();
			//
			// btDown
			//
			this.btDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btDown.BackColor = System.Drawing.Color.Transparent;
			this.btDown.Location = new System.Drawing.Point(140, 2);
			this.btDown.Name = "btDown";
			this.btDown.Size = new System.Drawing.Size(18, 16);
			this.btDown.TabIndex = 1;
			this.btDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btDown_MouseDown);
			//
			// txtBox
			//
			this.txtBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
				| System.Windows.Forms.AnchorStyles.Left)
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtBox.AutoSize = false;
			this.txtBox.BackColor = System.Drawing.Color.White;
			this.txtBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtBox.ErrorProvider = null;
			this.txtBox.ErrorProviderMessage = "Invalid value";
			this.txtBox.ForceFormatText = true;
			this.txtBox.HideSelection = false;
			this.txtBox.Location = new System.Drawing.Point(2, 2);
			this.txtBox.Name = "txtBox";
			this.txtBox.Size = new System.Drawing.Size(138, 16);
			this.txtBox.TabIndex = 0;
			this.txtBox.WordWrap = false;
			this.txtBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBox_KeyDown);
			this.txtBox.TextChanged += new System.EventHandler(this.txtBox_TextChanged);
			//
			// ComboBoxTyped
			//
			this.Controls.Add(this.txtBox);
			this.Controls.Add(this.btDown);
			this.Name = "ComboBoxTyped";
			this.Size = new System.Drawing.Size(160, 20);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Required
		/// </summary>
		[Browsable(false),DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ComponentModel.Validator.IValidator Validator
		{
			get{return txtBox.Validator;}
			set{txtBox.Validator = value;}
		}

		/// <summary>
		/// Reload the properties from the validator
		/// </summary>
		public virtual void OnLoadingValidator()
		{
			if (LoadingValidator != null)
				LoadingValidator(this, EventArgs.Empty);
		}

		/// <summary>
		///
		/// </summary>
		public event EventHandler LoadingValidator;

		/// <summary>
		/// True to set the textbox readonly otherwise false.
		/// </summary>
		public bool ReadOnlyTextBox
		{
			get{return txtBox.ReadOnly;}
			set{txtBox.ReadOnly = value;}
		}

		private int m_iSelectedItem = -1;
		/// <summary>
		/// Selected Index of the Items array. -1 if no value is selected or if the value is not in the Items list.
		/// </summary>
		[Browsable(false),DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectedIndex
		{
			get{return m_iSelectedItem;}
			set{m_iSelectedItem = value;OnSelectedIndexChanged();}
		}

		/// <summary>
		/// Populate and show the listbox
		/// </summary>
		public virtual void ShowListBox()
		{
			if (m_DropDown == null || m_DropDown.IsDisposed || m_DropDown.IsHandleCreated == false)
			{
				m_ListBox = new ListBox();
				m_ListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
				m_ListBox.SelectedIndexChanged += new EventHandler(ListBox_SelectedChange);
				m_ListBox.Click += new EventHandler(ListBox_Click);

				m_DropDown = new DropDown(m_ListBox, this, this.ParentForm);
				m_DropDown.DropDownClosed += new EventHandler(m_DropDown_DropDownClosed);
				m_DropDown.DropDownOpen += new EventHandler(m_DropDown_DropDownOpen);
			}

			m_DropDown.ShowDropDown();
		}

		private DropDown m_DropDown = null;
		private ListBox m_ListBox = null;

		/// <summary>
		/// Fired when the SelectedIndex property change
		/// </summary>
		protected virtual void OnSelectedIndexChanged()
		{
			if (Validator.StandardValues != null &&
				m_iSelectedItem != -1 &&
				m_iSelectedItem < Validator.StandardValues.Count)
			{
				try
				{
					m_bEditTxtBoxByCode = true;//per disabilitare l'evento txtBoxChange

					txtBox.Value = Validator.StandardValueAtIndex(m_iSelectedItem);

					SelectAllTextBox();
				}
				finally
				{
					m_bEditTxtBoxByCode = false;
				}
			}
		}

		/// <summary>
		/// Returns the string valud at the specified index using the editor. If index is not valid return Validator.NullDisplayString.
		/// </summary>
		/// <param name="p_Index"></param>
		/// <returns></returns>
		protected virtual string GetStringValueAtIndex(int p_Index)
		{
			if (Validator.StandardValues != null &&
				p_Index >= 0 &&
				p_Index < Validator.StandardValues.Count)
			{
				if (Validator.IsStringConversionSupported())
					return Validator.ValueToString(Validator.StandardValueAtIndex(p_Index));
				else
					return Validator.ValueToDisplayString(Validator.StandardValueAtIndex(p_Index));
			}
			else
				return Validator.NullDisplayString;
		}

		private bool m_bEditTxtBoxByCode = false;

		private void ListBox_SelectedChange(object sender, EventArgs e)
		{
			SelectedIndex = ((ListBox)sender).SelectedIndex;
		}

		/// <summary>
		/// Gets or sets the current value of the editor.
		/// </summary>
		[Browsable(false),DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Value
		{
			get
			{
				return txtBox.Value;
			}
			set
			{
				txtBox.Value = value;

				//provo a cercare il valore nell'elenco di valori attualmente nella lista in modo da poterlo selezionare
				if (Validator.StandardValues != null)
					m_iSelectedItem = Validator.StandardValuesIndexOf(value);
				else
					m_iSelectedItem = -1;
			}
		}

		private void btDown_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			txtBox.Focus();
			ShowListBox();
		}

		/// <summary>
		/// Select all the text of the textbox
		/// </summary>
		public void SelectAllTextBox()
		{
			txtBox.SelectAll();
		}

		private void txtBox_TextChanged(object sender, System.EventArgs e)
		{
			if (m_bEditTxtBoxByCode==false)
			{
				m_iSelectedItem = -1;
			}
		}

		private void txtBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
			{
				int l_SelectedIndex = m_iSelectedItem;
				if (e.KeyCode == Keys.Down)
					l_SelectedIndex++;
				else if (e.KeyCode == Keys.Up)
					l_SelectedIndex--;

				//controllo che sia valido
				if (l_SelectedIndex >= 0 && Validator.StandardValues != null && l_SelectedIndex < Validator.StandardValues.Count)
				{
					SelectedIndex = l_SelectedIndex;
				}
			}
		}

		/// <summary>
		/// The button in the right of the editor
		/// </summary>
		public DevAge.Windows.Forms.DropDownButton Button
		{
			get{return btDown;}
		}

		public DevAge.Windows.Forms.TextBoxTyped TextBox
		{
			get{return txtBox;}
		}

		private void ListBox_Click(object sender, EventArgs e)
		{
			//TODO da riverificare
			//.NET BUG
			//aggiunto questo pezzo di codice per risolvere un baco presente quando veniva inserita questa combo in un form con BorderStyle impostato su ToolWindow
			// che non faceva scatenare più l'evento SelectedIndexChange dopo questo evento di Click.
			// In questo modo chiamo io manualmente l'evento.
			ListBox_SelectedChange(sender, EventArgs.Empty);

			m_DropDown.CloseDropDown();
		}

		/// <summary>
		/// Fired when showing the drop down
		/// </summary>
		public event EventHandler DropDownOpen;

		/// <summary>
		/// Fired when showing the drop down
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnDropDownOpen(EventArgs e)
		{
			m_ListBox.Items.Clear();
			if (Validator.StandardValues != null)
			{
				foreach (object o in Validator.StandardValues)
				{
					m_ListBox.Items.Add(Validator.ValueToDisplayString(o));
				}
			}

			if (m_iSelectedItem >= 0 && m_iSelectedItem < m_ListBox.Items.Count)
			{
				m_ListBox.SelectedIndex = m_iSelectedItem;
			}

			if (DropDownOpen!=null)
				DropDownOpen(this,e);
		}

		/// <summary>
		/// Fired when closing the dropdown
		/// </summary>
		public event EventHandler DropDownClosed;

		/// <summary>
		/// Fired when closing the dropdown
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnDropDownClosed(EventArgs e)
		{
			if (DropDownClosed!=null)
				DropDownClosed(this,e);
		}

		#region Properties
		/// <summary>
		/// Indicates if after the Validating event the Text is refreshed with the new value, forcing the correct formatting.
		/// </summary>
		public bool ForceFormatText
		{
			get{return txtBox.ForceFormatText;}
			set{txtBox.ForceFormatText = value;}
		}


		/// <summary>
		/// True to enable the Escape key to undo any changes. Default is true.
		/// </summary>
		public bool EnableEscapeKeyUndo
		{
			get{return txtBox.EnableEscapeKeyUndo;}
			set{txtBox.EnableEscapeKeyUndo = value;}
		}

		/// <summary>
		/// True to enable the Enter key to validate any changes. Default is true.
		/// </summary>
		public bool EnableEnterKeyValidate
		{
			get{return txtBox.EnableEnterKeyValidate;}
			set{txtBox.EnableEnterKeyValidate = value;}
		}

		/// <summary>
		/// True to enable the validation of the textbox text when the Validating event is fired, to force always the control to be valid. Default is true.
		/// </summary>
		public bool EnableAutoValidation
		{
			get{return txtBox.EnableAutoValidation;}
			set{txtBox.EnableAutoValidation = value;}
		}
		#endregion

		private void txtBox_LoadingValidator(object sender, EventArgs e)
		{
			OnLoadingValidator();
		}

		private void m_DropDown_DropDownClosed(object sender, EventArgs e)
		{
			OnDropDownClosed(e);
		}

		private void m_DropDown_DropDownOpen(object sender, EventArgs e)
		{
			OnDropDownOpen(e);
		}

		public event EventHandler ValueChanged;

		protected virtual void OnValueChanged(EventArgs e)
		{
			if (ValueChanged != null)
				ValueChanged(this, e);
		}

		private void txtBox_ValueChanged(object sender, EventArgs e)
		{
			OnValueChanged(e);
		}

		protected override void OnBorderStyleChanged(EventArgs e)
		{
			base.OnBorderStyleChanged (e);
			SetContentAndButtonLocation(txtBox, btDown);
		}

	}
}
