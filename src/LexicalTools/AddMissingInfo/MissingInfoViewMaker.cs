using System;
using System.Collections.Generic;
using Palaso.i18n;
using Palaso.Lift;
using WeSay.LexicalModel;
using WeSay.Project;
using Palaso.DictionaryServices.Model;

namespace WeSay.LexicalTools.AddMissingInfo
{
	public class MissingInfoViewMaker
	{
		public static ViewTemplate CreateViewTemplate(MissingInfoConfiguration config, ViewTemplate baseTemplate)
		{
			var template = CreateViewTemplateFromListOfFields(baseTemplate, config.FieldsToShowCommaSeparated, config.FieldsToShowReadOnly);

			//see WS-1120 (martin_diprose@sil.org) Add option to limit "add meanings" task to the ones that have a semantic domain
			//for now, let's just turn off all ghosts in these fill-in tasks
			template.DoWantGhosts = false;

			MarkReadOnlyFields(template, config.FieldsToShowReadOnly);

			//hack until we overhaul how Tasks are setup:
			var isBaseFormFillingTask = config.FieldsToShowCommaSeparated.Contains(LexEntry.WellKnownProperties.BaseForm);
			if (isBaseFormFillingTask)
			{
				Field flagField = new Field();
				flagField.DisplayName = StringCatalog.Get("~&This word has no Base Form",
														  "The user will click this to say that this word has no baseform.  E.g. Kindness has Kind as a baseform, but Kind has no other word as a baseform.");
				flagField.DataTypeName = "Flag";
				flagField.ClassName = "LexEntry";
				flagField.FieldName = "flag-skip-" + config.MissingInfoFieldName;
				flagField.Enabled = true;
				template.Add(flagField);
			}
			return template;
		}

		private static ViewTemplate CreateViewTemplateFromListOfFields(IEnumerable<Field> fieldList,
																  string fieldsToShow,
																	string fieldsToShowReadOnly)
		{
			string[] fields = SplitUpFieldNames(fieldsToShow+","+fieldsToShowReadOnly);
			ViewTemplate viewTemplate = new ViewTemplate();
			foreach (Field field in fieldList)
			{
				if (Array.IndexOf(fields, field.FieldName) >= 0)
				{
					if (field.Enabled == false)
					//make sure specified fields are shown (greg's ws-356)
					{
						Field enabledField = new Field(field);
						enabledField.Visibility = CommonEnumerations.VisibilitySetting.Visible;
						enabledField.Enabled = true;
						viewTemplate.Add(enabledField);
					}
					else
					{
						if (field.Visibility != CommonEnumerations.VisibilitySetting.Visible)
						//make sure specified fields are visible (not in 'rare mode)
						{
							Field visibleField = new Field(field);
							visibleField.Visibility = CommonEnumerations.VisibilitySetting.Visible;
							viewTemplate.Add(visibleField);
						}
						else
						{
							viewTemplate.Add(field);
						}
					}
				}
			}
			return viewTemplate;
		}



		private static void MarkReadOnlyFields(ViewTemplate template, string fieldsToShowReadOnly)
		{
			string[] readOnlyFields = SplitUpFieldNames(fieldsToShowReadOnly);

			for (int i = 0; i < template.Count; i++)
			{
				Field field = template[i];
				foreach (string s in readOnlyFields)
				{
					if (s == field.FieldName)
					{
						Field readOnlyVersion = new Field(field);
						readOnlyVersion.Visibility = CommonEnumerations.VisibilitySetting.ReadOnly;
						template.Remove(field);
						template.Insert(i, readOnlyVersion);
					}
				}
			}
		}

		private static string[] SplitUpFieldNames(string fieldsToShow)
		{
			return fieldsToShow.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
		}

	}
}
