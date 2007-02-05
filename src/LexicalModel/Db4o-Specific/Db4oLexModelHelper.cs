using System;
using System.Collections.Generic;
using Db4objects.Db4o;
using Db4objects.Db4o.Events;
using WeSay.Foundation;

namespace WeSay.LexicalModel.Db4o_Specific
{
	/// <summary>
	/// All db4o-specific code for WeSayDataObjects is isolated here
	/// </summary>
	public class Db4oLexModelHelper :IDisposable
	{
		static private Db4oLexModelHelper _singleton = null;
		internal IObjectContainer _container;

		/// <summary>
		/// for tests
		/// </summary>
		private int _activationCount=0;

		[CLSCompliant(false)]
		public static void Initialize(Db4objects.Db4o.IObjectContainer container)
		{
			System.Diagnostics.Debug.Assert(container != null);
			if (_singleton != null && container == _singleton._container)
			{
				return;
			}

			//exists, but some test is opening a different container
			if (_singleton != null)
			{
				_singleton.Dispose();
				_singleton = null;
			}

			if (_singleton == null)
			{
				_singleton = new Db4oLexModelHelper(container);
			}
		}

		public static void InitializeForNonDbTests()
		{
			_singleton = new Db4oLexModelHelper(null);
		}

		/// <summary>
		/// how many times a WeSayDataObject has been activated
		/// </summary>
		public int ActivationCount
		{
			get { return _activationCount; }
		}

		public static Db4oLexModelHelper Singleton
		{
			get { return _singleton; }
		}

		private List<Type> _doNotActivateTypes;

		public List<Type> DoNotActivateTypes
		{
			get { return this._doNotActivateTypes; }
			set { this._doNotActivateTypes = value; }
		}

		private Db4oLexModelHelper(Db4objects.Db4o.IObjectContainer container)
		{
			_container = container;
			if (container == null)
			{
				return; //for non-db tests
			}
			_doNotActivateTypes = new List<Type>();

			IEventRegistry r = EventRegistryFactory.ForObjectContainer(container);
			r.Activated += new ObjectEventHandler(OnActivated);
			r.Activating += new CancellableObjectEventHandler(OnActivating);
		}

		void OnActivating(object sender, CancellableObjectEventArgs args)
		{
			if (_doNotActivateTypes.Contains(sender.GetType()))
			{
				args.Cancel();
			}
		}

		public void Dispose()
		{
			if (_container == null)
			{
				return;
			}

			IEventRegistry r = EventRegistryFactory.ForObjectContainer(_container);
			r.Activated -= new ObjectEventHandler(OnActivated);
			_container = null;
			_singleton = null;
		}


		void OnActivated(object sender, ObjectEventArgs args)
		{
			WeSayDataObject o = args.Object as WeSayDataObject;
			if (o == null)
				return;

			//activate all the children
			_container.Activate(o, int.MaxValue);
			o.FinishActivation();
			_activationCount++;
		}
	}
}
