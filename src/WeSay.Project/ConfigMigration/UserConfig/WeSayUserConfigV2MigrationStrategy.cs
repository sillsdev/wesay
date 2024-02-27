using SIL.Keyboarding;
using SIL.Migration;
using SIL.WritingSystems;
using SIL.Xml;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;

namespace WeSay.Project.ConfigMigration.UserConfig
{
	internal class WeSayUserConfigV2MigrationStrategy : MigrationStrategyBase
	{
		private LdmlInFolderWritingSystemRepository _wsRepo;

		public WeSayUserConfigV2MigrationStrategy(LdmlInFolderWritingSystemRepository wsRepo) : base(2, 3)
		{
			_wsRepo = wsRepo;
		}

		public override void Migrate(string sourceFilePath, string destinationFilePath)
		{

			// migrate local keyboard settings from WeSayUserConfig settings to new lexicon user settings file
			var configurationElem = XElement.Load(sourceFilePath);
			configurationElem.SetAttributeValue("version", 3);
			XElement keyboardsElem = configurationElem.Element("keyboards");
			if (keyboardsElem != null)
			{
				foreach (XElement keyboardElem in keyboardsElem.Elements("keyboard"))
				{
					var wsId = (string)keyboardElem.Attribute("ws");
					WritingSystemDefinition ws;
					if (_wsRepo.TryGet(wsId, out ws))
					{
						var layout = (string)keyboardElem.Attribute("layout");
						var locale = (string)keyboardElem.Attribute("locale");
						string keyboardId = string.IsNullOrEmpty(locale) ? layout : string.Format("{0}_{1}", locale, layout);
						IKeyboardDefinition keyboard;
						if (!Keyboard.Controller.TryGetKeyboard(keyboardId, out keyboard))
							keyboard = Keyboard.Controller.CreateKeyboard(keyboardId, KeyboardFormat.Unknown, Enumerable.Empty<string>());
						ws.LocalKeyboard = keyboard;
					}
				}

				_wsRepo.Save();

				// Remove keyboard element
				keyboardsElem.Remove();
			}

			var writerSettings = CanonicalXmlSettings.CreateXmlWriterSettings();
			writerSettings.NewLineOnAttributes = false;
			using (var writer = XmlWriter.Create(destinationFilePath, writerSettings))
				configurationElem.WriteTo(writer);

		}
	}
}
