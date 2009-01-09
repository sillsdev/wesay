using System;
using WeSay.UI.audio;

namespace WeSay.LexicalTools
{
	/// <summary>
	/// this is only does one thing now, but can grow to provide other services
	/// e.g., it could give access to image catalogs when we implement those
	/// </summary>
	public class LayoutInfoProvider : IServiceProvider
	{
		private AudioPathProvider _audioPathProvider;

		public LayoutInfoProvider(AudioPathProvider audioPathProvider)
		{
			_audioPathProvider = audioPathProvider;
		}

		public object GetService(Type serviceType)
		{
			if(serviceType != typeof(AudioPathProvider))
			{
				throw new ApplicationException("This dummy service provider doesn't have that type.");
			}
			return _audioPathProvider;
		}


	}
}