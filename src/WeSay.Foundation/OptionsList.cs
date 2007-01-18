using System;
using System.Collections.Generic;
using Exortech.NetReflector;

namespace WeSay.Foundation
{
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
			Name = name;
		}

		[ReflectorCollection("options", Required = true)]
		public List<Option> Options
		{
			get { return _options; }
			set { _options = value; }
		}

		[ReflectorProperty("name", Required = true)]
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
	}

	/// <summary>
	/// Used to refer to this option from a field
	/// </summary>
	public class OptionRef
	{
		private Guid _guid;

		public OptionRef()
		{

		}
		public OptionRef(Guid guid)
		{
			Guid = guid;
		}

		public Guid Guid
		{
			get { return _guid; }
			set { _guid = value; }
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

		private NetReflectorTypeTable MakeTypeTable()
		{
			NetReflectorTypeTable t = new NetReflectorTypeTable();
			t.Add(typeof(OptionsList));
			t.Add(typeof(Option));
			return t;
		}
	}
}
