using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeSay.TestUtilities
{
	public class OptionListFileContent
	{
		public static string GetOptionListWithWritingSystems(string id1, string id2)
		{
			#region filecontent
			return String.Format(
			@"<?xml version='1.0' encoding='utf-8'?>
<optionsList xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
  <option>
	<key>one</key>
	<name>
	  <form lang='{0}'>eins</form>
	  <form lang='{1}'>one</form>
	</name>
	<abbreviation>
	  <form lang='{0}'>eins</form>
	  <form lang='{1}'>one</form>
	</abbreviation>
	<description />
  </option>
</optionsList>", id1, id2).Replace("'", "\"");
			#endregion
		}
	}
}
