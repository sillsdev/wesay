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

		public FieldInventory()
		{

		}

//        public FieldInventory(params Field[] fields)
//        {
//            if(fields == null)
//            {
//                throw new ArgumentNullException();
//            }
//            int i = 0;
//            foreach (Field field in fields)
//            {
//                i++;
//                if (field == null)
//                {
//                    throw new ArgumentNullException("field",
//                                                    "field argument" + i.ToString() + "is null");
//                }
//            }
//
//            _fields = fields;
//        }



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

//        public void ICollection<Field>.Add(Field item)
//        {
//            Fields.
//        }

//        void ICollection<Field>.Clear()
//        {
//            throw new NotSupportedException();
//        }

		///<summary>
		///Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> contains a specific value.
		///</summary>
		///
		///<returns>
		///true if item is found in the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.
		///</returns>
		///
		///<param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
//        public bool Contains(Field item)
//        {
//            return -1 != Array.IndexOf<Field>(_fields, item);
//        }

		public bool Contains(string fieldName)
		{
			return Exists(
									   delegate(Field field)
									   {
										   return field.FieldName == fieldName;
									   });
		}

		///<summary>
		///Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
		///</summary>
		///
		///<param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
		///<param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		///<exception cref="T:System.ArgumentOutOfRangeException">arrayIndex is less than 0.</exception>
		///<exception cref="T:System.ArgumentNullException">array is null.</exception>
		///<exception cref="T:System.ArgumentException">array is multidimensional.-or-arrayIndex is equal to or greater than the length of array.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"></see> is greater than the available space from arrayIndex to the end of the destination array.-or-Type T cannot be cast automatically to the type of the destination array.</exception>
//        public void CopyTo(Field[] array, int arrayIndex)
//        {
//            _fields.CopyTo(array, arrayIndex);
//        }

//        bool ICollection<Field>.Remove(Field item)
//        {
//            throw new NotSupportedException();
//        }

		///<summary>
		///Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		///</summary>
		///
		///<returns>
		///The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		///</returns>
		///
//        public int Count
//        {
//            get
//            {
//                return _fields.Length;
//            }
//        }

		///<summary>
		///Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.
		///</summary>
		///
		///<returns>
		///true
		///</returns>
		///
//        public bool IsReadOnly
//        {
//            get
//            {
//                return true;
//            }
//        }

		///<summary>
		///Returns an enumerator that iterates through the collection.
		///</summary>
		///
		///<returns>
		///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
		///</returns>
//        ///<filterpriority>1</filterpriority>
//        IEnumerator<Field> IEnumerable<Field>.GetEnumerator()
//        {
//            foreach (Field field in _fields)
//            {
//                yield return field;
//            }
//        }

		///<summary>
		///Returns an enumerator that iterates through a collection.
		///</summary>
		///
		///<returns>
		///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
		///</returns>
		///<filterpriority>2</filterpriority>
//        public IEnumerator GetEnumerator()
//        {
//            return _fields.GetEnumerator();
//        }
//
//
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

		public void Write(XmlWriter writer)
		{
			try
			{
				writer.WriteStartDocument();
			   NetReflector.Write(writer, this);
			}
			finally
			{
				writer.Close();
			}
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
