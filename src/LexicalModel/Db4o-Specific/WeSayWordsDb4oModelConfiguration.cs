using WeSay.Data;
using WeSay.Foundation;
using WeSay.Language;

namespace WeSay.LexicalModel.Db4o_Specific
{
	public class WeSayWordsDb4oModelConfiguration : IDb4oModelConfiguration
	{
		public  void Configure()
		{
			Db4objects.Db4o.Config.IConfiguration db4oConfiguration = Db4objects.Db4o.Db4oFactory.Configure();
			db4oConfiguration.ClassActivationDepthConfigurable(true);

			db4oConfiguration.ActivationDepth(99);
			db4oConfiguration.UpdateDepth(99);

			Db4objects.Db4o.Config.IObjectClass objectClass;

			//avoid crash after deleting item created in a previous run
//            objectClass = db4oConfiguration.ObjectClass(typeof(System.Collections.Generic.Dictionary<string,object>));
//            objectClass.ObjectField("comparer").CascadeOnDelete(false);

			objectClass = db4oConfiguration.ObjectClass(typeof(LanguageForm));
			objectClass.ObjectField("_writingSystemId").Indexed(true);
			objectClass.ObjectField("_form").Indexed(true);
			objectClass.CascadeOnDelete(true);

			objectClass = db4oConfiguration.ObjectClass(typeof(LexEntry));
			objectClass.ObjectField("_modificationTime").Indexed(true);
			objectClass.ObjectField("_guid").Indexed(true);
			objectClass.ObjectField("_lexicalForm").Indexed(true);
			objectClass.ObjectField("_senses").Indexed(true);
			objectClass.CascadeOnDelete(true);

			objectClass = db4oConfiguration.ObjectClass(typeof(LexSense));
			objectClass.ObjectField("_gloss").Indexed(true);
			objectClass.ObjectField("_exampleSentences").Indexed(true);
			objectClass.CascadeOnDelete(true);

			objectClass = db4oConfiguration.ObjectClass(typeof(LexExampleSentence));
			objectClass.ObjectField("_sentence").Indexed(true);
			objectClass.ObjectField("_translation").Indexed(true);
			objectClass.CascadeOnDelete(true);

			objectClass = db4oConfiguration.ObjectClass(typeof(MultiText));
			objectClass.ObjectField("_forms").Indexed(true);
			objectClass.CascadeOnDelete(true);

		}
	}
}
