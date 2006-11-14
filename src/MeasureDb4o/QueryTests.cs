using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using com.db4o;
using System.Diagnostics;
using Debug=System.Diagnostics.Debug;

namespace MeasureDb4o
{
	class QueryTests
	{

		public void SimpleStringSearch()
		{
			ObjectContainer db = MakeFlatStringDatabase(true);
			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
			QueryStats stats = new QueryStats(db, "SimpleStringSearch");
			IList<LanguageForm> matches = db.Query<LanguageForm>(delegate(LanguageForm f)
								  {
									  return f._form == "99";
								  });
			Debug.Assert(1 == matches.Count);
			stats.PrintReport();
			db.Dispose();
		}

		private ObjectContainer MakeFlatStringDatabase(bool doIndex)
		{
			string path = Path.GetTempFileName();
			com.db4o.config.Configuration db4oConfiguration = com.db4o.Db4o.Configure();
			if (doIndex)
			{
				com.db4o.config.ObjectClass objectClass = db4oConfiguration.ObjectClass(typeof(LanguageForm));
				objectClass.ObjectField("_form").Indexed(true);
			}

			com.db4o.diagnostic.DiagnosticToConsole listener = new com.db4o.diagnostic.DiagnosticToConsole();
			db4oConfiguration.Diagnostic().AddListener(listener);

			com.db4o.ObjectContainer db = com.db4o.Db4oFactory.OpenFile(path);
			((com.db4o.YapStream)db).GetNativeQueryHandler().QueryOptimizationFailure += new com.db4o.inside.query.QueryOptimizationFailureHandler(OnQueryOptimizationFailure);

			for (int i = 0; i < 100; i++)
			{
				LanguageForm f = new LanguageForm("en", i.ToString());
				db.Set(f);
			}
			db.Commit();
			return db;
		}


		void OnQueryOptimizationFailure(object sender, com.db4o.inside.query.QueryOptimizationFailureEventArgs args)
		{
			Debug.WriteLine("Query not Optimized:");
			Debug.WriteLine(args.Reason);
		}
	}
}
