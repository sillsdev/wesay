using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

using System.Windows.Forms;
using System.Drawing.Design;

namespace DevAge.Windows.Forms
{
	/// <summary>
	/// A TextBoxTypedButton that uase the UITypeEditor associated with the type.
	/// </summary>
	public class TextBoxUITypeEditor : TextBoxTypedButton, IServiceProvider, System.Windows.Forms.Design.IWindowsFormsEditorService, ITypeDescriptorContext
	{
		private System.ComponentModel.IContainer components = null;

		public TextBoxUITypeEditor()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		public override void OnLoadingValidator()
		{
			object tmp = System.ComponentModel.TypeDescriptor.GetEditor(Validator.ValueType, typeof(UITypeEditor) );
			if (tmp is UITypeEditor)
				UITypeEditor = (UITypeEditor)tmp;

			base.OnLoadingValidator ();
		}


		public override void ShowDialog()
		{
			try
			{
				OnDialogOpen(EventArgs.Empty);
				if (m_UITypeEditor != null)
				{
					UITypeEditorEditStyle l_Style = m_UITypeEditor.GetEditStyle();
					if (l_Style == UITypeEditorEditStyle.DropDown ||
						l_Style == UITypeEditorEditStyle.Modal)
					{
						object l_EditObject;
						try
						{
							l_EditObject = Value;
						}
						catch
						{
							l_EditObject = Value;
						}

						object tmp = m_UITypeEditor.EditValue(this, this,l_EditObject);
						Value = tmp;
					}
				}

				OnDialogClosed(EventArgs.Empty);
			}
			catch(Exception err)
			{
				MessageBox.Show(err.Message,"Error");
			}
		}

		private UITypeEditor m_UITypeEditor;
		public UITypeEditor UITypeEditor
		{
			get{return m_UITypeEditor;}
			set{m_UITypeEditor = value;}
		}

		#region IServiceProvider Members
		System.Object IServiceProvider.GetService ( System.Type serviceType )
		{
			//modal
			if (serviceType == typeof(System.Windows.Forms.Design.IWindowsFormsEditorService))
				return this;

			return null;
		}
		#endregion

		#region System.Windows.Forms.Design.IWindowsFormsEditorService
		private DevAge.Windows.Forms.DropDown m_dropDown = null;
		public virtual void CloseDropDown ()
		{
			if (m_dropDown != null)
			{
				m_dropDown.CloseDropDown();
			}
		}

		public virtual void DropDownControl ( System.Windows.Forms.Control control )
		{
			//NB Non bisogna fare ne close ne dispose altrimenti non si riesci più ad accedere al controllo interno (editor)
			//using(m_dropDown = new ctlDropDownCustom(this))

			if (m_dropDown == null || m_dropDown.IsDisposed || m_dropDown.IsHandleCreated == false)
				m_dropDown = new DevAge.Windows.Forms.DropDown(control, this, this.ParentForm);

			m_dropDown.DropDownFlags = DevAge.Windows.Forms.DropDownFlags.CloseOnEscape;

			m_dropDown.ShowDropDown();

			//m_dropDown.Close(); //non si può chiudere ne fare il dispose altrimenti il chiamante non riesce più ad accedere ai controlli figlio
		}

		public virtual System.Windows.Forms.DialogResult ShowDialog ( System.Windows.Forms.Form dialog )
		{
			return dialog.ShowDialog(this);
		}
		#endregion

		#region ITypeDescriptorContext Members

		void ITypeDescriptorContext.OnComponentChanged()
		{

		}

		IContainer ITypeDescriptorContext.Container
		{
			get
			{
				return base.Container;
			}
		}

		bool ITypeDescriptorContext.OnComponentChanging()
		{
			return true;
		}

		object ITypeDescriptorContext.Instance
		{
			get
			{
				return Value;
			}
		}

		PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor
		{
			get
			{
				return null;
			}
		}

		#endregion
	}
}
