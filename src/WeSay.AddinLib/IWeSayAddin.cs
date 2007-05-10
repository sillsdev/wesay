using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Mono.Addins;

namespace WeSay.AddinLib
{
	[TypeExtensionPoint]
	public interface IWeSayAddin
	{
		Image ButtonImage { get;}

		bool Available { get;}

		string Name
		{
			get;
		}

		string ShortDescription
		{
			get;
		}

		void Launch(string pathToTopLevelDirectory, string pathToLIFT);
	}
}
