using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Events;
using Palaso.Text;
using WeSay.Data;
using WeSay.Foundation;

namespace WeSay.LexicalModel.Db4oSpecific
{
	public class Db4oLexEntryRepository: Db4oRepository<LexEntry>
	{
		public Db4oLexEntryRepository(Db4oDataSource database): base(database)
		{
			Initialize();
		}

		public Db4oLexEntryRepository(string path): base(path)
		{
			Initialize();
		}

		private void Initialize()
		{
			ConfigureDatabase();
			IEventRegistry r = EventRegistryFactory.ForObjectContainer(Database);
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
			Database.Activate(o, int.MaxValue);
			o.FinishActivation();
		}

		private static void ConfigureDatabase()
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
}