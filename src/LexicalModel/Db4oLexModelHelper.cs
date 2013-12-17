using System;
using System.Collections.Generic;
using Db4objects.Db4o;
using Db4objects.Db4o.Events;
using WeSay.Foundation;
using Debug=System.Diagnostics.Debug;

namespace WeSay.LexicalModel.Db4o_Specific
{
	/// <summary>
	/// All db4o-specific code for WeSayDataObjects is isolated here
	/// </summary>
	public class Db4oLexModelHelper: IDisposable
	{
		private static Db4oLexModelHelper _singleton = null;

		/// <summary>
		/// for tests
		/// </summary>
		private int _activationCount = 0;

		internal IObjectContainer _container;
		private List<Type> _doNotActivateTypes;

		private Db4oLexModelHelper(IObjectContainer container)
		{
			_container = container;
			if (container == null)
			{
				return; //for non-db tests
			}
			_doNotActivateTypes = new List<Type>();

			IEventRegistry r =
					EventRegistryFactory.ForObjectContainer(container);
			r.Activated += OnActivated;
			r.Activating += OnActivating;
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

		public List<Type> DoNotActivateTypes
		{
			get { return _doNotActivateTypes; }
			set { _doNotActivateTypes = value; }
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (_container == null)
			{
				return;
			}

			IEventRegistry r =
					EventRegistryFactory.ForObjectContainer(_container);
			r.Activated -= OnActivated;
			_container = null;
			_singleton = null;
		}

		#endregion

		/// <summary>
		/// used by test setups to avoid hiding bugs that would be found if we didn't have to initialize in
		/// order to create the test database.
		/// </summary>
		/// <param name="container"></param>
		[CLSCompliant(false)]
		public static void Deinitialize(IObjectContainer container)
		{
			Debug.Assert(_singleton._container == container);
			_singleton.Dispose();
			_singleton = null;
		}

		[CLSCompliant(false)]
		public static void Initialize(IObjectContainer container)
		{
			Debug.Assert(container != null);
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

		private void OnActivating(object sender, CancellableObjectEventArgs args)
		{
			if (_doNotActivateTypes.Contains(sender.GetType()))
			{
				args.Cancel();
			}
		}

		private void OnActivated(object sender, ObjectEventArgs args)
		{
			WeSayDataObject o = args.Object as WeSayDataObject;
			if (o == null)
			{
				return;
			}

			//activate all the children
			_container.Activate(o, int.MaxValue);
			o.FinishActivation();
			_activationCount++;
		}
	}
}