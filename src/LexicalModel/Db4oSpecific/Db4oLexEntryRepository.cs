using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Events;
using Palaso.Text;
using WeSay.Data;
using WeSay.Foundation;

namespace WeSay.LexicalModel.Db4oSpecific
{
	internal sealed class Db4oLexEntryRepository: Db4oRepository<LexEntry>
	{
		private static WeSayDb4oConfigurator configurator = new WeSayDb4oConfigurator();
		// we need the following code to run once, before the database is constructed
		// to set the properties of the database appropriately.
		// this is the best way we could figure out to do that.
		private class WeSayDb4oConfigurator
		{
			public WeSayDb4oConfigurator()
			{
				IConfiguration db4oConfiguration = Db4oFactory.Configure();
				db4oConfiguration.ClassActivationDepthConfigurable(true);

				db4oConfiguration.ActivationDepth(99);
				db4oConfiguration.UpdateDepth(99);

				IObjectClass objectClass;

				//avoid crash after deleting item created in a previous run
				//            objectClass = db4oConfiguration.ObjectClass(typeof(System.Collections.Generic.Dictionary<string,object>));
				//            objectClass.ObjectField("comparer").CascadeOnDelete(false);

				objectClass = db4oConfiguration.ObjectClass(typeof (LanguageForm));
				objectClass.ObjectField("_writingSystemId").Indexed(true);
				objectClass.ObjectField("_form").Indexed(true);
				objectClass.CascadeOnDelete(true);

				objectClass = db4oConfiguration.ObjectClass(typeof (LexEntry));
				objectClass.ObjectField("_modificationTime").Indexed(true);
				objectClass.ObjectField("_guid").Indexed(true);
				objectClass.ObjectField("_lexicalForm").Indexed(true);
				objectClass.ObjectField("_senses").Indexed(true);
				objectClass.CascadeOnDelete(true);

				objectClass = db4oConfiguration.ObjectClass(typeof (LexSense));
				objectClass.ObjectField("_gloss").Indexed(true);
				objectClass.ObjectField("_exampleSentences").Indexed(true);
				objectClass.CascadeOnDelete(true);

				objectClass = db4oConfiguration.ObjectClass(typeof (LexExampleSentence));
				objectClass.ObjectField("_sentence").Indexed(true);
				objectClass.ObjectField("_translation").Indexed(true);
				objectClass.CascadeOnDelete(true);

				objectClass = db4oConfiguration.ObjectClass(typeof (MultiText));
				objectClass.ObjectField("_forms").Indexed(true);
				objectClass.CascadeOnDelete(true);
			}
		}

		public Db4oLexEntryRepository(string path): base(path)
		{
			IEventRegistry r = EventRegistryFactory.ForObjectContainer(InternalDatabase);
			r.Activated += OnActivated;
		}

		private void OnActivated(object sender, ObjectEventArgs args)
		{
			WeSayDataObject o = args.Object as WeSayDataObject;
			if (o == null)
			{
				return;
			}

			//activate all the children
			InternalDatabase.Activate(o, int.MaxValue);
			this._activationCount++;
			o.FinishActivation();
		}

		/// <summary>
		/// for tests
		/// </summary>
		private int _activationCount;

		/// <summary>
		/// for tests
		/// how many times an Object has been activated
		/// </summary>
		internal int ActivationCount
		{
			get { return this._activationCount; }
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				IEventRegistry r = EventRegistryFactory.ForObjectContainer(InternalDatabase);
				r.Activated -= OnActivated;
			}
			base.Dispose(disposing);
		}
	}
}