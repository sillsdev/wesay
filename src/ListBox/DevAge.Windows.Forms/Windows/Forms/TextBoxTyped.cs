using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DevAge.Windows.Forms
{
	/// <summary>
	/// A TextBox that allows to set the type of value to edit, then you can use the Value property to read and write the specific type.
	/// </summary>
	public class TextBoxTyped : System.Windows.Forms.TextBox
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Constructor
		/// </summary>
		public TextBoxTyped()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			Validator = new ComponentModel.Validator.ValidatorTypeConverter(typeof(string));
#if MINI
			Validating += new CancelEventHandler(TextBoxTyped_Validating);
#endif
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
		}
		#endregion

		/// <summary>
		/// Indica l'ultimo valore impostato valido. null se non è stato impostato nessun valore. Questo serve nel caso in cui ci sia un Validating che fallisce e viene richiesta la property Value. In questo caso si restituisce questo valore.
		/// </summary>
		private object mValue = null;
		/// <summary>
		/// The value of the TextBox, returns an instance of the specified type, or null if the Value is not valid.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Value
		{
			get{return mValue;}

			set{SetValue(value, true);}
		}

		/// <summary>
		/// Text value of the TextBox
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text
		{
			get{return base.Text;}
			set{base.Text = value;}
		}


		public void SetValue(object objectVal, bool refreshTextBox)
		{
			mIsValid = false;

			mValue = m_Validator.ObjectToValue(objectVal);
			mIsValid = true;
			OnValueChanged(EventArgs.Empty);

			if (refreshTextBox)
				ValidateTextBoxValue();
		}

		public event EventHandler ValueChanged;
		protected virtual void OnValueChanged(EventArgs e)
		{
			if (ValueChanged != null)
				ValueChanged(this, e);
		}

		private bool mIsValid = false;
		/// <summary>
		/// True if the current text box value is valid.
		/// </summary>
		public bool IsValid
		{
			get{return mIsValid;}
		}


		private bool mIsInsideValidatingText = false;

		/// <summary>
		/// Validate the content of the TextBox
		/// </summary>
		/// <returns>Returns True if the value is valid otherwise false</returns>
		public virtual bool ValidateTextBoxValue()
		{
			mIsInsideValidatingText = true;
			try
			{
				if (IsValid == false)
				{
#if !MINI
					if (m_errorProvider!=null)
						m_errorProvider.SetError(this,m_strErrorProviderMessage);
#endif
					//non valido
					return false;
				}
				else
				{
					if (m_Validator.IsStringConversionSupported())
						Text = m_Validator.ValueToString(Value);
					else
						Text = m_Validator.ValueToDisplayString(Value);

#if !MINI
					if (m_errorProvider!=null)
						m_errorProvider.SetError(this,null);
#endif
					//valido
					return true;
				}
			}
			catch(Exception)
			{
#if !MINI
				if (m_errorProvider!=null)
					m_errorProvider.SetError(this,m_strErrorProviderMessage);
#endif
				return false;
			}
			finally
			{
				mIsInsideValidatingText = false;
			}
		}

		#region Properties
#if !MINI
		private System.Windows.Forms.ErrorProvider m_errorProvider;
		/// <summary>
		/// Error provider used when a text is not valid.
		/// </summary>
		public ErrorProvider ErrorProvider
		{
			get{return m_errorProvider;}
			set{m_errorProvider = value;}
		}
#endif
		private ComponentModel.Validator.IValidator m_Validator;
		/// <summary>
		/// Type converter used for conversion
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ComponentModel.Validator.IValidator Validator
		{
			get{return m_Validator;}
			set
			{
				if (value == null)
					throw new ApplicationException("Invalid Validator, cannot be null");
				m_Validator = value;
				OnLoadingValidator();
			}
		}

		/// <summary>
		/// Reload the properties from the validator
		/// </summary>
		public virtual void OnLoadingValidator()
		{
			if (LoadingValidator != null)
				LoadingValidator(this, EventArgs.Empty);

			SetValue(m_Validator.DefaultValue, true);
			ReadOnly = !(m_Validator.IsStringConversionSupported());
		}

		/// <summary>
		///
		/// </summary>
		public event EventHandler LoadingValidator;

		private string m_strErrorProviderMessage = "Invalid value";
		/// <summary>
		/// Message used with the ErrorProvider object
		/// </summary>
		public string ErrorProviderMessage
		{
			get{return m_strErrorProviderMessage;}
			set{m_strErrorProviderMessage = value;}
		}

		private bool m_bForceFormatText = true;
		/// <summary>
		/// Indicates if after the Validating event the Text is refreshed with the new value, forcing the correct formatting.
		/// </summary>
		[DefaultValue(true)]
		public bool ForceFormatText
		{
			get{return m_bForceFormatText;}
			set{m_bForceFormatText = value;}
		}

		private bool m_bEnableEscapeKeyUndo = true;
		/// <summary>
		/// True to enable the Escape key to undo any changes. Default is true.
		/// </summary>
		[DefaultValue(true)]
		public bool EnableEscapeKeyUndo
		{
			get{return m_bEnableEscapeKeyUndo;}
			set{m_bEnableEscapeKeyUndo = value;}
		}
		private bool m_bEnableEnterKeyValidate = true;
		/// <summary>
		/// True to enable the Enter key to validate any changes. Default is true.
		/// </summary>
		[DefaultValue(true)]
		public bool EnableEnterKeyValidate
		{
			get{return m_bEnableEnterKeyValidate;}
			set{m_bEnableEnterKeyValidate = value;}
		}

		private bool m_bEnableAutoValidation = true;
		/// <summary>
		/// True to enable the validation of the textbox text when the Validating event is fired, to force always the control to be valid. Default is true.
		/// </summary>
		[DefaultValue(true)]
		public bool EnableAutoValidation
		{
			get{return m_bEnableAutoValidation;}
			set{m_bEnableAutoValidation = value;}
		}

		private char[] m_ValidCharacters = new char[0];

		/// <summary>
		/// A list of characters allowed for the textbox. Used in the OnKeyPress event. If null no check is made.
		/// If not null any others charecters is not allowed. First the function check if ValidCharacters is not null then check for InvalidCharacters.
		/// </summary>
		public char[] ValidCharacters
		{
			get{return m_ValidCharacters;}
			set{m_ValidCharacters = value;}
		}

		private char[] m_InvalidCharacters = new char[0];

		/// <summary>
		/// A list of characters not allowed for the textbox. Used in the OnKeyPress event. If null no check is made.
		/// If not null any characters in the list is not allowed. First the function check if ValidCharacters is not null then check for InvalidCharacters.
		/// </summary>
		public char[] InvalidCharacters
		{
			get{return m_InvalidCharacters;}
			set{m_InvalidCharacters = value;}
		}
		#endregion

		#region Override Events

		/// <summary>
		/// Raises the System.Windows.Forms.Control.TextChanged event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged (e);

			try
			{
				if (mIsInsideValidatingText == false)
					SetValue(Text, false);
			}
			catch(Exception)
			{
			}
		}


		/// <summary>
		/// Raises the System.Windows.Forms.Control.Validating event.
		/// </summary>
		/// <param name="e"></param>
#if !MINI
		protected override void OnValidating(CancelEventArgs e)
		{
			base.OnValidating (e);
#else
		private void TextBoxTyped_Validating(object sender, CancelEventArgs e)
		{
#endif
			if (EnableAutoValidation && ValidateTextBoxValue() == false)
				e.Cancel = true;
		}

		/// <summary>
		/// Raises the System.Windows.Forms.Control.KeyDown event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown (e);

			if (e.KeyCode == Keys.Escape && EnableEscapeKeyUndo)
			{
				ValidateTextBoxValue(); //Reset the Text with the current value
			}
		}

		/// <summary>
		/// Raises the System.Windows.Forms.Control.KeyPress event.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress (e);

			if (e.KeyChar == 13 && EnableEnterKeyValidate && Multiline == false)
			{
				ValidateTextBoxValue();
				e.Handled = true;
			}
			else if (char.IsControl(e.KeyChar) == false) //is not a non printable character like backspace, ctrl+c, ...
			{
				if (CheckValidationChars(e.KeyChar, ValidCharacters, InvalidCharacters) == false)
					e.Handled = true;
			}
		}
		#endregion

		/// <summary>
		/// Check if the key pressed is valid or invalid. Only one array can be not null, otherwise only the validChars is processed.
		/// </summary>
		/// <param name="c"></param>
		/// <param name="validChars">Array of valid characters, if null is not used</param>
		/// <param name="invalidChars">Array of invalid characters, if null is not used</param>
		/// <returns>Returns true is valid otherwise false</returns>
		public static bool CheckValidationChars(char c, char[] validChars, char[] invalidChars)
		{
			if (validChars != null && validChars.Length > 0)
			{
				for (int i = 0; i < validChars.Length; i++)
				{
					if (c == validChars[i])
						return true;
				}

				return false;
			}
			else if (invalidChars != null && invalidChars.Length > 0)
			{
				for (int i = 0; i < invalidChars.Length; i++)
				{
					if (c == invalidChars[i])
						return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Check in the specific string if all the characters are valid
		/// </summary>
		/// <param name="p_Input"></param>
		/// <param name="p_ValidCharacters"></param>
		/// <param name="p_InvalidCharacters"></param>
		/// <returns></returns>
		public static string ValidateCharactersString(string p_Input, char[] p_ValidCharacters, char[] p_InvalidCharacters)
		{
			string tmp;

			if (p_Input != null && p_ValidCharacters != null && p_ValidCharacters.Length > 0)
			{
				tmp = "";
				for (int i = 0; i < p_Input.Length; i++)
				{
					bool l_bFound = false;
					for (int j = 0; j < p_ValidCharacters.Length; j++)
					{
						if (p_ValidCharacters[j] == p_Input[i])
						{
							l_bFound = true;
							break;
						}
					}
					if (l_bFound)
						tmp += p_Input[i];
				}
			}
			else if (p_Input != null && p_InvalidCharacters != null && p_InvalidCharacters.Length > 0)
			{
				tmp = "";
				for (int i = 0; i < p_Input.Length; i++)
				{
					bool l_bFound = false;
					for (int j = 0; j < p_InvalidCharacters.Length; j++)
					{
						if (p_InvalidCharacters[j] == p_Input[i])
						{
							l_bFound = true;
							break;
						}
					}
					if (!l_bFound)
						tmp += p_Input[i];
				}
			}
			else
				tmp = p_Input;

			return tmp;
		}
	}
}
