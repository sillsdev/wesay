using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using Exortech.NetReflector;
using WeSay.Language;

namespace WeSay.Foundation
{
	/// <summary>
	/// Used to refer to this option from a field
	/// </summary>
	public class OptionRef : IParentable, IValueHolder<string>
	{
		// private Guid _guid;

//        [NonSerialized]
//        private Option _option;

		private string _humanReadableKey;

		/// <summary>
		/// This "backreference" is used to notify the parent of changes.
		/// IParentable gives access to this during explicit construction.
		/// </summary>
		private WeSayDataObject _parent;

		/// <summary>
		/// For INotifyPropertyChanged
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;


		public OptionRef()
		{
		}

		//        public OptionRef(Guid guid)
		//        {
		//            Guid = guid;
		//        }

		//        private Guid Guid
		//        {
		////            get { return _guid; }
		//            set
		//            {
		//                _guid = value;
		//                NotifyPropertyChanged();
		//            }
		//        }

		#region IParentable Members

		public WeSayDataObject Parent
		{
			set
			{
				_parent = value;
			}
		}

		#endregion

		private void NotifyPropertyChanged()
		{
			//tell any data binding
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs("option")); //todo
			}

			//tell our parent
			this._parent.NotifyPropertyChanged("option");//todo
		}

		public string Value
		{
			get
			{
				return this._humanReadableKey;
			}
			set
			{
				this._humanReadableKey = value;
				// this.Guid = value.Guid;
				NotifyPropertyChanged();
			}
		}
	}

	/// <summary>
	/// This is like a PossibilityList in FieldWorks, or RangeSet in Toolbox
	/// </summary>
	[ReflectorType("optionList")]
	public class OptionsList
	{
		private string _name;
		private List<Option> _options;

		public OptionsList(string name)
		{
			_name = name;
			_options = new List<Option>();
		}

		[ReflectorCollection("options", Required = true)]
		public List<Option> Options
		{
			get { return _options; }
			set { _options = value; }
		}

//        [ReflectorProperty("name", Required = true)]
//        public string Name
//        {
//            get { return _name; }
//            set { _name = value; }
//        }

		public void LoadFromFile(string pathToOptionsList)
		{
			NetReflectorReader r = new NetReflectorReader(MakeTypeTable());
			XmlReader reader = XmlReader.Create(pathToOptionsList);
			try
			{
				r.Read(reader, this);
			}
			finally
			{
				reader.Close();
			}
		}


		private NetReflectorTypeTable MakeTypeTable()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof(OptionsList));
			t.Add(typeof(Option));
			return t;
		}
	}


	[ReflectorType("option")]
	public class Option
	{
		private string _name;
		private string _abbreviation;
		private Guid _guid;

		public Option()
			:this("?","?",Guid.Empty)
		{
		}

		public Option(string name, string abbreviation, Guid guid)
		{
			_name = name;
			_abbreviation = abbreviation;
			_guid = guid;
		}

		public string Key   //todo
		{
			get { return _name; }
			set { _name = value; }
		}

		[ReflectorProperty("name", Required = true)]
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[ReflectorProperty("abbreviation", Required = false)]
		public string Abbreviation
		{
			get
			{
				if (_abbreviation == null)
				{
					return Name;
				}
				return _abbreviation;
			}
			set { _abbreviation = value; }
		}

		[ReflectorProperty("guid", Required = false)]
		public Guid Guid
		{
			get
			{
				if (_guid == null || _guid == Guid.Empty)
				{
					return Guid.NewGuid();
				}
				return _guid;
			}
			set { _guid = value; }
		}


		public override string ToString()
		{
			return _name;
		}
	}
}
