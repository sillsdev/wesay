using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Exortech.NetReflector;
using WeSay.Language;

namespace WeSay.LexicalModel
{
	[ReflectorType("fieldInventory")]
	public class FieldInventory : List<Field>
	{
		private string _id="Default Field Inventory";

		public FieldInventory()
		{

		}

		/// <summary>
		/// For serialization only
		/// </summary>
		[ReflectorCollection("fields", Required = true)]
		public List<Field> Fields
		{
			get
			{
				return this;
			}
			set
			{
				this.Clear();
				foreach (Field  f in value)
				{
					if (f == null)
					{
						throw new ArgumentNullException("field",
														"one of the fields is null");
					}
					this.Add(f);
				}
			}
		}

		[ReflectorProperty("id", Required = false)]
		public string Id
		{
			get { return _id; }
			set { _id = value; }
		}

		///<summary>
		///Gets the field with the specified name.
		///</summary>
		///
		///<returns>
		///The field with the given field name.
		///</returns>
		///
		///<param name="index">The field name of the field to get.</param>
		///<exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.Generic.IList`1"></see>.</exception>
		public Field this[string fieldName]
		{
			get
			{
				Field field;
				if(!TryGetField(fieldName, out field))
				{
					throw new ArgumentOutOfRangeException();
				}
				return field;
			}
		}

		public bool TryGetField(string fieldName, out Field field)
		{
			if(fieldName == null)
			{
				throw new ArgumentNullException();
			}
			field = Find(
						   delegate(Field f)
						   {
							   return f.FieldName == fieldName;
						   });

			if (field == default(Field))
			{
				return false;
			}
			return true;
		}

		public Field GetField(string fieldName)
		{
			Field field = null;
			TryGetField(fieldName, out field);
			return field;
		}


		public bool Contains(string fieldName)
		{
			Field field;
			if (!TryGetField(fieldName, out field))
			{
				return false;
			}
			return field.Visibility == Field.VisibilitySetting.Visible;
		}

		/// <summary>
		/// used in the admin to make sure what we write out is based on the latest master inventory
		/// </summary>
		/// <param name="masterInventory"></param>
		/// <param name="usersInventory"></param>
		public static void ModifyMasterFromUser(FieldInventory masterInventory, FieldInventory usersInventory)
		{
			foreach (Field masterField in masterInventory)
			{
				Field userField = usersInventory.GetField(masterField.FieldName);
				if (userField != null)
				{
					Field.ModifyMasterFromUser(masterField, userField);
				}
			}
		}



		public static FieldInventory MakeMasterInventory(WritingSystemCollection writingSystems)
		{
			FieldInventory masterInventory = new FieldInventory();
			masterInventory.Add(MakeField(Field.FieldNames.EntryLexicalForm.ToString(), "Word", true,writingSystems));
			masterInventory.Add(MakeField(Field.FieldNames.SenseGloss.ToString(), "Gloss", true,writingSystems));
			masterInventory.Add(MakeField(Field.FieldNames.ExampleSentence.ToString(), "Example Sentence", true,writingSystems));
			masterInventory.Add(MakeField(Field.FieldNames.ExampleTranslation.ToString(), "Translation", false,writingSystems));
			return masterInventory;
		}

		private static Field MakeField(string name, string displayName, bool defaultVisible, WritingSystemCollection writingSystems)
		{
			Field field = new Field();
			field.FieldName = name;
			field.DisplayName = displayName;
			if (defaultVisible)
			{
				field.Visibility = Field.VisibilitySetting.Visible;
			}
			else
			{
				field.Visibility = Field.VisibilitySetting.Invisible;
			}

			foreach (string id in writingSystems.Keys)
			{
				field.WritingSystemIds.Add(id);
			}
			return field;
		}

		#region persistence

		public void Load(string path)
		{
			NetReflectorReader r = new NetReflectorReader(MakeTypeTable());
			XmlReader reader = XmlReader.Create(path);
			try
			{
				r.Read(reader, this);
			}
			finally
			{
				reader.Close();
			}
		}

		public void LoadFromString(string xml)
		{
			NetReflectorReader r = new NetReflectorReader(MakeTypeTable());
			XmlReader reader = XmlReader.Create(new System.IO.StringReader(xml));
			try
			{
				r.Read(reader, this);
			}
			finally
			{
				reader.Close();
			}
		}

		/// <summary>
		/// Does not "start" the xml doc, no close the writer
		/// </summary>
		/// <param name="writer"></param>
		public void Write(XmlWriter writer)
		{
			 NetReflector.Write(writer, this);
		}

		private NetReflectorTypeTable MakeTypeTable()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof(FieldInventory ));
			t.Add(typeof(Field));
		 //   t.Add(typeof(Field.WritingSystemId));
			return t;
		}

		#endregion
	 }

}
