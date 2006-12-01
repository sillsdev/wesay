using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using WeSay.Language;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class LiftImportTestsFlexVer1Pt1 : LiftImportTestsBase
	{
		protected override LiftImporter CreateImporter()
		{
			return new LiftImporterFlexVer1Pt1(_entries);
		}

		[Test]
		public void LoadKalabaFromFLEx1pt1()
		{
			IList<LexEntry> entries = new List<LexEntry>();
			string path = Path.Combine(WeSay.Project.WeSayWordsProject.Project.ApplicationTestDirectory, "kalabaFlex1pt1.lift");
			LiftImporter importer= LiftImporter.ReadFile(entries, path, null);
			Assert.IsInstanceOfType(typeof (LiftImporterFlexVer1Pt1), importer);
			Assert.AreEqual(61, entries.Count);
		}


		/// <summary>
		/// this version used ws instead of lang
		/// </summary>
		[Test]
		public void MultiTextWithWsAtrributes()
		{
			this._doc.LoadXml("<wrap><form ws=\"blue\">ocean</form><form ws=\"red\">sunset</form></wrap>");
			MultiText text = new MultiText();
			_importer.ReadMultiText(this._doc.SelectSingleNode("wrap"), text);
			Assert.AreEqual("ocean", text["blue"]);
			Assert.AreEqual("sunset", text["red"]);
		}

		[Test]
		public void MissingMultiText()
		{
			_doc.LoadXml("<wrap></wrap>");
			MultiText text = new MultiText();
			_importer.ReadMultiText(_doc.SelectSingleNode("wrap"), text);
			Assert.AreEqual(0, text.Count);
		}

		/// <summary>
		/// when I wrote the flex exporter, lift did not yet implement semantic domain
		/// </summary>
		[Test, Ignore("Not yet implemented in WeSay")]
		public void SemanticDomainTraitIsBroughtInCorrectly()
		{
			_doc.LoadXml("<trait range=\"semantic-domain\" value=\"6.5.1.1\"/>");
			//TODO   _importer.ReadTrait(_doc.SelectSingleNode("wrap"));
		}

		/// <summary>
		/// when I wrote the flex exporter, lift did not yet implement part of speech
		/// </summary>
		[Test, Ignore("Not yet implemented in WeSay")]
		public void GrammiWithTextLabel()
		{
			_doc.LoadXml("<sense><grammi type=\"conc\"/></sense>");
			//TODO   _importer.ReadSense(_doc.SelectSingleNode("sense"));
		}

		/// <summary>
		/// when I wrote the flex exporter, lift did not yet implement part of speech
		/// </summary>
		[Test, Ignore("Not yet implemented in WeSay")]
		public void GrammiWithEmptyLabel()
		{
			_doc.LoadXml("<sense><grammi type=\"\"/></sense>");
			//TODO   _importer.ReadSense(_doc.SelectSingleNode("sense"));
		}

		/// <summary>
		/// when I wrote the flex exporter, lift did not yet implement importing dates
		/// </summary>
		[Test, Ignore("Not yet implemented in WeSay")]
		public void DateCreatedIsBroughtInCorrectly()
		{
			_doc.LoadXml("<dateCreated val=\"2003-08-07T08:42:42+07:00\"/>");
			//TODO   _importer.ReadDateCreated(_doc.SelectSingleNode("dateCreated"));
		}

		/// <summary>
		/// when I wrote the flex exporter, lift did not yet implement importing dates
		/// </summary>
		[Test, Ignore("Not yet implemented in WeSay")]
		public void DateModifiedIsBroughtInCorrectly()
		{
			_doc.LoadXml("<dateModified val=\"2003-07-09T04:51:43+07:00\"/>");
			//TODO   _importer.ReadDateModified(_doc.SelectSingleNode("dateModified"));
		}

		/// <summary>
		/// when I wrote the flex exporter, lift did not yet implement importing Definition
		/// </summary>
		[Test, Ignore("Definition is not yet implemented in WeSay")]
		public void Definition()
		{
			_doc.LoadXml("<sense><def><form ws=\"Eng\">female feline</form></def></sense>");
			LexSense s = _importer.ReadSense(_doc.SelectSingleNode("sense"));
			// Assert.AreEqual(1, s.
		}


	}
}