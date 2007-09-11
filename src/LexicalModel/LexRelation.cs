using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using WeSay.Foundation;

namespace WeSay.LexicalModel
{
	public class LexRelationType
	{
		public enum Multiplicities
		{
			One,
			Many
		}

		public enum TargetTypes
		{
			Entry,
			Sense
		}

		private string _id;
		private Multiplicities _multiplicity;
		private TargetTypes _targetType;

		public LexRelationType(string id, Multiplicities multiplicity, TargetTypes targetType)
		{
			_id = id;
			_targetType = targetType;
			_multiplicity = multiplicity;
		}

		public string ID
		{
			get { return _id; }
		}

		public Multiplicities Multiplicity
		{
			get { return _multiplicity; }
		}

		public TargetTypes TargetType
		{
			get { return _targetType; }
		}
	}

	public class LexRelation : IParentable, IReferenceContainer, IValueHolder<LexEntry>, IEmptinessCleanup
	{
		//private LexRelationType _type;
		private string _fieldId;
		//private WeSayDataObject _target;
		private string _targetId;
		private WeSayDataObject _parent;
//
//        public LexRelation()
//        {
//        }

		public LexRelation(string fieldId, string targetId, WeSayDataObject parent)
		{
			_fieldId = fieldId;
			_targetId = targetId;
			_parent = parent;
		}

		/// <summary>
		/// Set to string.emtpy to clear the relation
		/// </summary>
		public string TargetId
		{
			get { return _targetId; }
			set { _targetId = value; }
		}

//        public LexRelationType Type
//        {
//            get
//            {
//                return _type;
//            }
//            set
//            {
//                _type = value;
//            }
//        }

		#region IParentable Members

		public WeSayDataObject Parent
		{
			set { _parent = value; }
		}

		public string FieldId
		{
			get { return _fieldId; }
			set { _fieldId = value; }
		}

		#region IReferenceContainer Members

		public object Target
		{
			get
			{
				return Lexicon.FindFirstLexEntryMatchingId(_targetId);
				// return Lexicon.TheLexicon.FindEntryFromId(_targetId);
			}
			set
			{
				if (value == Target)
				{
					return;
				}

				if (value == null)
				{
					_targetId = string.Empty;
				}
				else
				{
					LexEntry entry = value as LexEntry;
					Debug.Assert(entry != null);
					_targetId = entry.GetOrCreateId(true);
				}
				NotifyPropertyChanged();
			}
		}

		#endregion

		#endregion

		private void NotifyPropertyChanged()
		{
			//tell any data binding. These would update the display of this data.
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs("relation"));
			}

			//tell our parent, which then handles getting us saved eventually
			if (_parent != null)
			{
				_parent.NotifyPropertyChanged("relation");
			}
		}

		#region IValueHolder<LexEntry> Members

		/// <summary>
		///  IValueHolder<LexEntry>.Value
		/// </summary>
		public LexEntry Value
		{
			get { return Target as LexEntry; }
			set { Target = value; }
		}

		#region IEmptinessStatus Members

		public bool IsEmpty
		{
			get { return string.IsNullOrEmpty(_targetId); }
		}

		#region IEmptinessCleanup Members

		public void RemoveEmptyStuff()
		{
			//nothing to do...
		}

		#endregion

		#endregion

		#endregion

		#region INotifyPropertyChanged Members

		///<summary>
		///Occurs when a property value changes.
		///</summary>
		///
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}

	public class LexRelationCollection : IParentable, IEmptinessCleanup
	{
		private WeSayDataObject _parent;
		private List<LexRelation> _relations = new List<LexRelation>();

		#region IParentable Members

		public WeSayDataObject Parent
		{
			set { _parent = value; }
			get { return _parent; }
		}

		#endregion

		public List<LexRelation> Relations
		{
			get { return _relations; }
			set { _relations = value; }
		}

		#region IEmptinessStatus Members

		public bool IsEmpty
		{
			get
			{
				foreach (LexRelation relation in _relations)
				{
					if (!relation.IsEmpty)
					{
						return false;
					}
				}
				return true;
			}
		}

		#region IEmptinessCleanup Members

		public void RemoveEmptyStuff()
		{
			//we do this in two passes because you can't remove items from a collection you are iterating over
			List<LexRelation> condemed = new List<LexRelation>();
			foreach (LexRelation relation in _relations)
			{
				if (relation.IsEmpty)
				{
					condemed.Add(relation);
				}
			}

			foreach (LexRelation relation in condemed)
			{
				_relations.Remove(relation);
			}
		}

		#endregion

		#endregion
	}
}