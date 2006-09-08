using System;

namespace DevAge.Windows.Forms
{
	/// <summary>
	/// Summary description for TextBoxTypedNumeric.
	/// </summary>
	public class TextBoxTypedNumeric : TextBoxTyped
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public TextBoxTypedNumeric()
		{
			Validator = new ComponentModel.Validator.ValidatorTypeConverter(typeof(double));
			Value = 0.0;
			TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			NumericCharStyle = NumericCharStyle.DecimalSeparator | NumericCharStyle.NegativeSymbol | NumericCharStyle.ExponentialSymbol | NumericCharStyle.PositiveSymbol;
		}

		private NumericCharStyle m_NumericCharStyle;

		/// <summary>
		/// Style of characters allowed. When using this property the ValidCharacters is automatically calculated.
		/// </summary>
		[System.ComponentModel.DefaultValue(NumericCharStyle.DecimalSeparator | NumericCharStyle.NegativeSymbol | NumericCharStyle.ExponentialSymbol | NumericCharStyle.PositiveSymbol)]
		public NumericCharStyle NumericCharStyle
		{
			get{return m_NumericCharStyle;}
			set{m_NumericCharStyle = value;}
		}

		protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
		{
			base.OnKeyPress (e);

			if (e.Handled == false &&
				char.IsControl(e.KeyChar) == false)
			{
				if (CheckValidationChars(e.KeyChar, CreateNumericValidChars(Validator.CultureInfo, m_NumericCharStyle), null) == false)
					e.Handled = true;
			}
		}


		/// <summary>
		/// Returns an array of valid numeric char
		/// </summary>
		/// <param name="p_Culture">If null the current culture is used</param>
		/// <param name="p_NumericCharStyle"></param>
		/// <returns></returns>
		public static char[] CreateNumericValidChars(System.Globalization.CultureInfo p_Culture, NumericCharStyle p_NumericCharStyle)
		{
			if (p_Culture == null)
				p_Culture = System.Globalization.CultureInfo.CurrentCulture;

			string l_AllowedChars = "0123456789";

			if ( (p_NumericCharStyle & NumericCharStyle.CurrencySymbol) == NumericCharStyle.CurrencySymbol)
			{
				l_AllowedChars += p_Culture.NumberFormat.CurrencySymbol;

				if ( (p_NumericCharStyle & NumericCharStyle.DecimalSeparator) == NumericCharStyle.DecimalSeparator)
					l_AllowedChars += p_Culture.NumberFormat.CurrencyDecimalSeparator;
				if ( (p_NumericCharStyle & NumericCharStyle.GroupSeparator) == NumericCharStyle.GroupSeparator)
					l_AllowedChars += p_Culture.NumberFormat.CurrencyGroupSeparator;
			}

			if ( (p_NumericCharStyle & NumericCharStyle.PercentSymbol) == NumericCharStyle.PercentSymbol)
			{
				l_AllowedChars += p_Culture.NumberFormat.PercentSymbol;

				if ( (p_NumericCharStyle & NumericCharStyle.DecimalSeparator) == NumericCharStyle.DecimalSeparator)
					l_AllowedChars += p_Culture.NumberFormat.PercentDecimalSeparator;
				if ( (p_NumericCharStyle & NumericCharStyle.GroupSeparator) == NumericCharStyle.GroupSeparator)
					l_AllowedChars += p_Culture.NumberFormat.PercentGroupSeparator;
			}

			if ( (p_NumericCharStyle & NumericCharStyle.DecimalSeparator) == NumericCharStyle.DecimalSeparator)
				l_AllowedChars += p_Culture.NumberFormat.NumberDecimalSeparator;
			if ( (p_NumericCharStyle & NumericCharStyle.GroupSeparator) == NumericCharStyle.GroupSeparator)
				l_AllowedChars += p_Culture.NumberFormat.NumberGroupSeparator;

			if ( (p_NumericCharStyle & NumericCharStyle.InfinitySymbol) == NumericCharStyle.InfinitySymbol)
			{
				l_AllowedChars += p_Culture.NumberFormat.NegativeInfinitySymbol;
				l_AllowedChars += p_Culture.NumberFormat.PositiveInfinitySymbol;
			}

			if ( (p_NumericCharStyle & NumericCharStyle.NaNSymbol) == NumericCharStyle.NaNSymbol)
				l_AllowedChars += p_Culture.NumberFormat.NaNSymbol;

			if ( (p_NumericCharStyle & NumericCharStyle.NegativeSymbol) == NumericCharStyle.NegativeSymbol)
				l_AllowedChars += p_Culture.NumberFormat.NegativeSign;

			if ( (p_NumericCharStyle & NumericCharStyle.PerMilleSymbol) == NumericCharStyle.PerMilleSymbol)
				l_AllowedChars += p_Culture.NumberFormat.PerMilleSymbol;

			if ( (p_NumericCharStyle & NumericCharStyle.PositiveSymbol) == NumericCharStyle.PositiveSymbol)
				l_AllowedChars += p_Culture.NumberFormat.PositiveSign;

			if ( (p_NumericCharStyle & NumericCharStyle.ExponentialSymbol) == NumericCharStyle.ExponentialSymbol)
			{
				l_AllowedChars += 'e';
				l_AllowedChars += 'E';
			}

			if (l_AllowedChars.Length > 0)
				return l_AllowedChars.ToCharArray();
			else
				return null;
		}
	}

	/// <summary>
	/// Numeric allowed characters. Defualt is NumericCharStyle.DecimalSeparator | NumericCharStyle.NegativeSymbol | NumericCharStyle.ExponentialSymbol | NumericCharStyle.PositiveSymbol
	/// </summary>
	[Flags]
	public enum NumericCharStyle
	{
		None = 0,
		CurrencySymbol = 1,
		PercentSymbol = 2,
		PerMilleSymbol = 4,
		DecimalSeparator = 8,
		GroupSeparator = 16,
		NegativeSymbol = 32,
		PositiveSymbol = 64,
		NaNSymbol = 128,
		InfinitySymbol = 256,
		ExponentialSymbol = 512
}
}
