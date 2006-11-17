using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using com.db4o;
using com.db4o.query;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class Db4oQueryPerformance
	{

		class Entry
		{
			public MultiText name;
		}

		class MultiText
		{
			public LanguageForm _singleForm;
			public LanguageForm[] _forms;
			public MultiText()
			{
				_forms = new LanguageForm[2];
			}
		}

		class LanguageForm
		{
			public string _writingSystemId;
			public string _form;

			public LanguageForm(string writingSystemId, string form)
			{
				_writingSystemId = writingSystemId;
				_form = form;
			}
		}

		protected com.db4o.ObjectContainer _db;
		protected string _filePath;


		[SetUp]
		public void SetUp()
		{
			_filePath = Path.GetTempFileName();
			com.db4o.config.Configuration db4oConfiguration = com.db4o.Db4o.Configure();
			com.db4o.config.ObjectClass objectClass = db4oConfiguration.ObjectClass(typeof(WeSay.Data.Tests.Db4oQueryPerformance.LanguageForm));
		   // objectClass.ObjectField("_writingSystemId").Indexed(true);
		   // objectClass.ObjectField("_form").Indexed(true);

			_db = com.db4o.Db4oFactory.OpenFile(_filePath);
			((com.db4o.YapStream)_db).GetNativeQueryHandler().QueryOptimizationFailure += new com.db4o.inside.query.QueryOptimizationFailureHandler(OnQueryOptimizationFailure);

			for (int i = 0; i < 10000; i++)
			{
				Entry e = new Entry();
				e.name = new MultiText();
				e.name._forms[0] = new LanguageForm("en", "en-"+i.ToString());
				//e.name._forms[1] = new LanguageForm("fr", "fr-"+i.ToString());
				e.name._singleForm = new LanguageForm("en", i.ToString());
				_db.Set(e);
			}

//            _db.Commit();
//            _db.Dispose();
//            _db = com.db4o.Db4oFactory.OpenFile(_filePath);
		}

		[TearDown]
		public void TearDown()
		{
			this._db.Dispose();
			File.Delete(_filePath);
		}

		[Test]
		public void FindWithNativeUsingArray()
		{
			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

			stopwatch.Start();
			IList<Entry> matches = _db.Query<Entry>(delegate(Entry e)
								  {
									  return e.name._forms[0]._form == "en-99";
								  });
			stopwatch.Stop();
			Assert.AreEqual(1, matches.Count);
			Console.WriteLine("FindWithNativeUsingArray " + stopwatch.ElapsedMilliseconds/1000.0 + " seconds");
		}

		[Test]
		public void FindWithNativeNoArray()
		{
			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

			stopwatch.Start();
			IList<Entry> matches = _db.Query<Entry>(delegate(Entry e)
								  {
									  return e.name._singleForm._form == "99";
								  });
			stopwatch.Stop();
			Assert.AreEqual(1, matches.Count);
			Console.WriteLine("FindWithNativeNoArray " +stopwatch.ElapsedMilliseconds / 1000.0 + " seconds");
		}


		[Test]
		public void FindRawForm()
		{
			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

			stopwatch.Start();
			IList<LanguageForm> matches = _db.Query<LanguageForm>(delegate(LanguageForm f)
								  {
									  return f._form == "99";
								  });
			stopwatch.Stop();
			Assert.AreEqual(1, matches.Count);
			Console.WriteLine("FindRawForm " + stopwatch.ElapsedMilliseconds / 1000.0 + " seconds");
		}


		[Test]
		public void SimpleStringSearch()
		{
			ObjectContainer db = MakeFlatStringDatabase(true);
			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

			stopwatch.Start();
			IList<LanguageForm> matches = db.Query<LanguageForm>(delegate(LanguageForm f)
								  {
									  return f._form  == "99";
								  });
			stopwatch.Stop();
			Assert.AreEqual(1, matches.Count);
			Console.WriteLine("SimpleStringSearch " + stopwatch.ElapsedMilliseconds / 1000.0 + " seconds");

			db.Dispose();
		}

		private ObjectContainer MakeFlatStringDatabase(bool doIndex)
		{
			string path = Path.GetTempFileName();
			com.db4o.config.Configuration db4oConfiguration = com.db4o.Db4o.Configure();
			if (doIndex)
			{
				com.db4o.config.ObjectClass objectClass = db4oConfiguration.ObjectClass(typeof(WeSay.Data.Tests.Db4oQueryPerformance.LanguageForm));
				objectClass.ObjectField("_form").Indexed(true);
			}

			com.db4o.diagnostic.DiagnosticToConsole listener = new com.db4o.diagnostic.DiagnosticToConsole();
			db4oConfiguration.Diagnostic().AddListener(listener);

			com.db4o.ObjectContainer db = com.db4o.Db4oFactory.OpenFile(path);
			((com.db4o.YapStream)db).GetNativeQueryHandler().QueryOptimizationFailure += new com.db4o.inside.query.QueryOptimizationFailureHandler(OnQueryOptimizationFailure);

			for (int i = 0; i < 10000; i++)
			{
				LanguageForm f = new LanguageForm("en", i.ToString());
				db.Set(f);
			}
			db.Commit();
			return db;
		}


		void OnQueryOptimizationFailure(object sender, com.db4o.inside.query.QueryOptimizationFailureEventArgs args)
		{
			Console.WriteLine("Query not Optimized:");
			Console.WriteLine(args.Reason);
		}


		/* all times from running only the one test in isolation, in test runner without debugging on
		 * with index on _form
		 * FindWithNativeUsingArray 2.765 seconds
		   FindWithNativeNoArray 1.046 seconds
		 * FindRawForm 0.55 seconds
		 *
		 *
		 * no index
		 * FindWithNativeUsingArray 2.765 seconds
			FindWithNativeNoArray 1.051 seconds
		 *  FindRawForm 0.864 seconds
		 */
	}
}
