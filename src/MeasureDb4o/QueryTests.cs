using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using com.db4o;
using System.Diagnostics;
using com.db4o.config;
using com.db4o.query;
using Debug=System.Diagnostics.Debug;

namespace MeasureDb4o
{
	class QueryTests
	{
		private string _path;

		public QueryTests()
		{
			PrimeTheQueryPump();
		}

		public void SimpleStringSearch()
		{
//            _path = @"c:\test.yap";
//            ObjectContainer x = OpenDb();
//            DoSodaQuery(x, "96");
//            DoNativeQuery(x, "99");
//            DoNativeQuery(x, "88");
//            x.Dispose();

			MakeDatabase();

			ObjectContainer db = OpenDb();
			FindFormNQ(db, "99");
			FindFormNQ(db, "88");
			db.Dispose();

			db = OpenDb();
			FindFormSODA(db, "96");
			FindFormNQ(db, "87");
			db.Dispose();

			db = OpenDb();
			FindEntryNoArraySODA(db, "87");
			db.Dispose();

			db = OpenDb();
			FindEntryNoArrayNQ(db, "87");
			db.Dispose();

		   db = OpenDb();
		   FindEntryWithArrayNQ(db, "83");
			db.Dispose();

			File.Delete(_path);
	   }

	   private static void FindEntryWithArrayNQ(ObjectContainer db, string searchString)
	   {
		   QueryStats stats = new QueryStats(db, "FindEntry-Array-NQ");
		   stats.StartManually();

		   IList<Entry> matches = db.Query<Entry>(
			   delegate(Entry e)
			   {
				   return e._name._forms[0]._form == searchString;
			   });
		   stats.FinishManually();
		   Debug.Assert(1 == matches.Count);
		   Debug.Assert(matches[0]._name._singleForm._form == searchString);
		   stats.PrintReport();
	   }

		private static void FindEntryNoArrayNQ(ObjectContainer db, string searchString)
		{
			QueryStats stats = new QueryStats(db, "FindEntry-NoArray-NQ");
			stats.StartManually();

			IList<Entry> matches = db.Query<Entry>(
				delegate(Entry e)
				{
					return e._name._singleForm._form == searchString;
				});
			stats.FinishManually();
			Debug.Assert(1 == matches.Count);
			Debug.Assert(matches[0]._name._singleForm._form == searchString);
			stats.PrintReport();
		}

		private static void FindEntryNoArraySODA(ObjectContainer db, string searchString)
		{
			QueryStats stats = new QueryStats(db, "FindEntry-NoArray-SODA");
			Query q = db.Query();
			q.Constrain(typeof(Entry));
			q.Descend("_name").Descend("_singleForm").Descend("_form").Constrain(searchString).Equal();

			ObjectSet results = q.Execute();

			Debug.Assert(results.Count>0);
			Debug.Assert(1 == results.Count);
			Debug.Assert(((Entry)results[0])._name._singleForm._form == searchString);
			stats.PrintReport();
		}

		private static void FindFormNQ(ObjectContainer db, string searchString)
		{
			QueryStats stats = new QueryStats(db, "FindForm-NQ");
			stats.StartManually();

			IList<LanguageForm> matches = db.Query<LanguageForm>(
				delegate(LanguageForm f)
				 {
					 return f._form == searchString;
				 });
			stats.FinishManually();
			Debug.Assert(matches.Count>0);
			Debug.Assert(matches[0]._form == searchString);
			stats.PrintReport();
		}

		private static void FindFormSODA(ObjectContainer db, string searchString)
		{
			QueryStats stats = new QueryStats(db, "FindForm-SODA");

			Query q = db.Query();
			q.Constrain(typeof(LanguageForm));
			q.Descend("_form").Constrain(searchString).Equal();
			ObjectSet results = q.Execute();

			Debug.Assert(results.Count>0);
			Debug.Assert(((LanguageForm) results[0])._form == searchString);
			stats.PrintReport();
		}

		/// <summary>
		/// Cause Db4o to do one-time overhead stuff, chiefly loading
		/// the dll for evaluating native queries.
		/// </summary>
		public void PrimeTheQueryPump()
		{
			Console.WriteLine("PrimeTheQueryPump...");
			string path = Path.GetTempFileName();
			com.db4o.ObjectContainer db = com.db4o.Db4oFactory.OpenFile(path);
			db.Set(new LanguageForm("en", "hello"));
			db.Commit();
			//SODA doesn't prime it: db.Query().Execute();
			db.Query<LanguageForm>(
				   delegate(LanguageForm x)
				   {
					   return true;
				   });

			db.Close();
			db.Dispose();
			File.Delete(path);
		}

		private void MakeDatabase()
		{
			Console.WriteLine("Creating Sample Db...");
			_path = Path.GetTempFileName();
			Configure(true);

			com.db4o.ObjectContainer db = com.db4o.Db4oFactory.OpenFile(_path);
			((com.db4o.YapStream)db).GetNativeQueryHandler().QueryOptimizationFailure += new com.db4o.inside.query.QueryOptimizationFailureHandler(OnQueryOptimizationFailure);


			for (int i = 0; i < 10000; i++)
			{
				Entry e = new Entry();
				e._name = new MultiText();
				e._name._singleForm = new LanguageForm("en", i.ToString());
				e._name._forms[0] = new LanguageForm("en", i.ToString());
				db.Set(e);
			}

			//http://tracker.db4o.com/jira/browse/COR-279 makes me worried about indexes not
			//being used until the db is reopened, maybe?

			db.Commit();
			db.Close();
			db.Dispose();
		}

		private void Configure(bool doIndex)
		{
			com.db4o.diagnostic.DiagnosticToConsole listener = new com.db4o.diagnostic.DiagnosticToConsole();

			com.db4o.config.Configuration db4oConfiguration = com.db4o.Db4o.Configure();
			//db4oConfiguration.Diagnostic().AddListener(listener);

			if (doIndex)
			{
				com.db4o.config.ObjectClass lf = db4oConfiguration.ObjectClass(typeof(LanguageForm));
				lf.ObjectField("_form").Indexed(true);

				//these next two don't seem to help, & don't turn off the warning message
				com.db4o.config.ObjectClass mt = db4oConfiguration.ObjectClass(typeof(MultiText));
				mt.ObjectField("_singleForm").Indexed(true);
				com.db4o.config.ObjectClass e = db4oConfiguration.ObjectClass(typeof(Entry));
				e.ObjectField("_name").Indexed(true);
			}
		}

		public ObjectContainer OpenDb()
		{
			Console.WriteLine("Opening Sample Db...");
			Configure(true);
			return com.db4o.Db4oFactory.OpenFile(_path);
		}


		void OnQueryOptimizationFailure(object sender, com.db4o.inside.query.QueryOptimizationFailureEventArgs args)
		{
			Debug.WriteLine("Query not Optimized:");
			Debug.WriteLine(args.Reason);
		}
	}
}
