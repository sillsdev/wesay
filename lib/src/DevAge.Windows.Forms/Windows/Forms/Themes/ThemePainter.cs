using System;

namespace DevAge.Windows.Forms
{
	/// <summary>
	/// Theme painter class. Use the CurrentProvider static property to specific the used provider. The default value is ThemeProviderBase.
	/// </summary>
	public sealed class ThemePainter
	{
		private ThemePainter()
		{
		}

		/// <summary>
		/// Static property used to specific the provider used. Can be any class that implements IThemeProvider interface.
		/// For example you can use ThemeProviderBase for the standard drawing provider or ThemeProviderXP to enable XP theme.
		/// </summary>
		public static IThemeProvider CurrentProvider = new ThemeProviderBase();
	}
}
