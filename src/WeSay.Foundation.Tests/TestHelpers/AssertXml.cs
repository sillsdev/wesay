using System;
using System.IO;
using System.Xml;
using NUnit.Framework;

namespace WeSay.Foundation.Tests.TestHelpers
{
	public class AssertXmlFile
	{
		public static void AtLeastOneMatch(string path, string xpath)
		{
			AssertXml.AtLeastOneMatch(File.ReadAllText(path), xpath);
		}

		public static void NoMatch(string path, string xpath)
		{
		   AssertXml.NoMatch(File.ReadAllText(path),xpath);
		}

		public void AtLeastOneMatchWithArgs(string path, string xpathWithArgs,
											   params object[] args)
		{
			AssertXml.AtLeastOneMatch(File.ReadAllText(path), string.Format(xpathWithArgs, args));
		}

		public void AssertNoMatchForXPathWithArgs(string path, string xpathWithArgs,
												  params object[] args)
		{
			NoMatch(File.ReadAllText(path), string.Format(xpathWithArgs, args));
		}
	}

	public class AssertXml
	{
		public static void AtLeastOneMatch(string xml, string xpath)
		{
			XmlDocument doc = GetDoc(xml);
			XmlNode node = doc.SelectSingleNode(xpath);
			if (node == null)
			{
				Console.WriteLine("Could not match " + xpath);
				Console.WriteLine();
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				settings.ConformanceLevel = ConformanceLevel.Fragment;
				XmlWriter writer = XmlWriter.Create(Console.Out, settings);
				doc.WriteContentTo(writer);
				writer.Flush();
			}
			Assert.IsNotNull(node);
		}


		public static void PrintNodeToConsole(XmlNode node)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.ConformanceLevel = ConformanceLevel.Fragment;
			XmlWriter writer = XmlWriter.Create(Console.Out, settings);
			node.WriteContentTo(writer);
			writer.Flush();
			Console.WriteLine();
		}
		public static void NoMatch(string xml, string xpath)
		{
			XmlDocument doc = GetDoc(xml);
			XmlNode node = doc.SelectSingleNode(xpath);
			if (node != null)
			{
				Console.WriteLine("Was not supposed to match " + xpath);
				Console.WriteLine();
				PrintNodeToConsole(node);
			}
			Assert.IsNull(node);
		}

		public void AtLeastOneMatchWithArgs(string xml, string xpathWithArgs,
											   params object[] args)
		{
			AtLeastOneMatch(xml, string.Format(xpathWithArgs, args));
		}

		public void AssertNoMatchForXPathWithArgs(string xml, string xpathWithArgs,
												  params object[] args)
		{
			NoMatch(xml, string.Format(xpathWithArgs, args));
		}

		private static XmlDocument GetDoc(string xml)
		{
			XmlDocument doc = new XmlDocument();
			try
			{
				doc.LoadXml(xml);
			}
			catch (Exception err)
			{
				Console.WriteLine(err.Message);
				Console.WriteLine(xml);
			}
			return doc;
		}
	}
}
