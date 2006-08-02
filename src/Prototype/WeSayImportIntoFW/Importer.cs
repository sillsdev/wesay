using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Diagnostics;
using SIL.FieldWorks.FDO;
using SIL.FieldWorks.FDO.Cellar;
using SIL.FieldWorks.FDO.Cellar.Generated;
using SIL.FieldWorks.FDO.Ling;
using SIL.FieldWorks.FDO.Ling.Generated;

namespace WeSay.FieldWorks
{
	public class Importer
	{
		protected FdoCache _cache;
		protected MoMorphTypeCollection _flexMorphTypes;

		public Importer(FdoCache cache)
		{
			_cache = cache;
			_flexMorphTypes = new MoMorphTypeCollection(_cache);
		}

		public void ImportWeSayFile(string path)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(path);

			foreach (XmlNode node in doc.SelectNodes("lexicon/entry"))
			{
				ImportOneEntry(node);
			}
		}


		/// <summary>
		/// Import a single entry selected out of a Lift file. For test use.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="xpath"></param>
		/// <returns>either the entry which was created were merged into</returns>
		public SIL.FieldWorks.FDO.Ling.LexEntry ImportOneWeSayEntry(string path, string xpath)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(path);
			return ImportOneEntry(doc.SelectSingleNode(xpath));
		}

		private SIL.FieldWorks.FDO.Ling.LexEntry ImportOneEntry(XmlNode node)
		{
			SIL.FieldWorks.FDO.Ling.LexEntry flexEntry;
			WeSay.LexicalModel.LexicalEntry weSayEntry = WeSay.LexicalModel.LiftImporter.LoadFromWeSayXml(node);

			Guid guid = weSayEntry.Guid;
			int hvo = _cache.GetIdFromGuid(guid);
			if (hvo > 0)
			{
				flexEntry = LexEntry.CreateFromDBObject(_cache, hvo);
			   // if (((LexSense)flexEntry.SensesOS[0]).Gloss.AnalysisDefaultWritingSystem != weSayEntry.Gloss)
				MergeAnalysisString(((LexSense)flexEntry.SensesOS[0]).Gloss,weSayEntry.Gloss);
				MergeVernacularString(flexEntry.LexemeFormOA.Form,weSayEntry.LexicalForm);
				MergeVernacularString(((LexExampleSentence)((LexSense)flexEntry.SensesOS[0]).ExamplesOS[0]).Example,weSayEntry.Example);
			}
			else
			{
				flexEntry = MakeFwEntryFromWeSayEntry(weSayEntry);
			}
			return flexEntry;
		}


		private void MergeAnalysisString(MultiUnicodeAccessor fwString, string wsString)
		{
			if(wsString.Length >0)
				fwString.AnalysisDefaultWritingSystem = wsString;

			//MultiStringAccessor a = new MultiStringAccessor(_cache, 0, 0, null);
			//existing.LexemeFormOA.Form.MergeAlternatives();
		}

		private void MergeVernacularString (MultiUnicodeAccessor fwString, string wsString)
		{
			if (wsString.Length > 0)
				fwString.VernacularDefaultWritingSystem = wsString;

		}

		private void MergeVernacularString (MultiStringAccessor fwString, string wsString)
		{
			if (wsString.Length > 0)
				fwString.VernacularDefaultWritingSystem.Text = wsString;
		}

		private SIL.FieldWorks.FDO.Ling.LexEntry MakeFwEntryFromWeSayEntry(WeSay.LexicalModel.LexicalEntry weSayEntry)
		{
			//MoStemMsa msa = new MoStemMsa();
			//// I wouldn't even *pretend* to understand this weirdness. Is 'dummy' a technical term?
			//DummyGenericMSA dmsa = DummyGenericMSA.Create(msa);
			//MoMorphType mmt = _flexMorphTypes.Item(MoMorphType.kmtStem);
			//LexEntry entry = LexEntry.CreateEntry(_cache, EntryType.ketMajorEntry, mmt, weSayEntry.LexicalForm, null, weSayEntry.Gloss, dmsa);
			LexEntry entry = new LexEntry();
			_cache.LanguageProject.LexicalDatabaseOA.EntriesOC.Add(entry);
			//(_cache, EntryType.ketMajorEntry, mmt, weSayEntry.LexicalForm, null, weSayEntry.Gloss, dmsa);

			entry.Guid = weSayEntry.Guid;

			entry.LexemeFormOA = new MoStemAllomorph();
			entry.LexemeFormOA.Form.VernacularDefaultWritingSystem
					= weSayEntry.LexicalForm;
		   //LexSense.CreateSense(entry, dmsa, weSayEntry.Gloss);

			MakeSense(weSayEntry, entry);

			return entry;
		}

		private static void MakeSense(WeSay.LexicalModel.LexicalEntry weSayEntry, LexEntry entry)
		{
			LexSense sense = new LexSense();
			entry.SensesOS.Append(sense);
			sense.Gloss.AnalysisDefaultWritingSystem = weSayEntry.Gloss;

			if (weSayEntry.Example != null && weSayEntry.Example.Length >0)
			{
				LexExampleSentence example = new LexExampleSentence();
				sense.ExamplesOS.Append(example);
				 example.Example.VernacularDefaultWritingSystem.Text = weSayEntry.Example;
		  }

		}
/*
		/// <summary>
		///
		/// </summary>
		/// <param name="ld"></param>
		/// <param name="cf"></param>
		/// <param name="defn"></param>
		/// <param name="hvoDomain"></param>
		/// <returns></returns>
		protected LexEntry MakeLexEntry(LexicalDatabase ld, string cf, string defn, int hvoDomain)
		{
			LexEntry le = new LexEntry();
			ld.EntriesOC.Add(le);
			le.CitationForm.VernacularDefaultWritingSystem = cf;
			LexSense ls = new LexSense();
			le.SensesOS.Append(ls);
			ls.Definition.AnalysisDefaultWritingSystem.Text = defn;
			if (hvoDomain != 0)
				ls.SemanticDomainsRC.Add(hvoDomain);
			MoMorphoSyntaxAnalysis msa = new MoStemMsa();
			le.MorphoSyntaxAnalysesOC.Add(msa);
			ls.MorphoSyntaxAnalysisRA = msa;
			return le;
		}

		private void TryToMerge(LexEntry entry, Guid guid)
		{
			Debug.Assert(entry.Guid != guid, "Don't assign the guid yourself.");

			int hvo = _cache.GetIdFromGuid(guid);
			if (hvo > 0)
			{
				LexEntry existing = LexEntry.CreateFromDBObject(_cache, hvo);
				existing.MergeObject(entry, true);
			}
			else
			{
				entry.Guid = guid; // let the new guy live and keep this guid
			}
		}
 * */
	}
}
