using System;
using com.db4o;
using com.db4o.events;
using com.db4o.foundation;
using Debug = System.Diagnostics.Debug;

namespace MeasureDb4o
{
	class QueryStats
	{
		private readonly string _label;
		private EventRegistry _registry = null;
		private int _activationCount;
		private StopWatch _watch = new StopWatch();

		public void PrintReport()
		{
		  // string s = String.Format(_label + " " + this.ExecutionTime / 1000.0 + " seconds " + _activationCount + " activations");
		   string s = String.Format(_label + " " + this.ExecutionTime + " ms " + _activationCount + " activations");
			 Debug.WriteLine(s);
			 Console.WriteLine(s);
		}

		/// <summary>
		/// How long the last query took to execute, in miliseconds
		/// </summary>
		public long ExecutionTime
		{
			get
			{
				return _watch.Elapsed();
			}
		}

		/// <summary>
		/// only here because the events don't actually fire in db4o 5.7!
		/// </summary>
		public void StartManually()
		{
			_watch.Start();
		}

		public void FinishManually()
		{
			_watch.Stop ();
		}

		/// <summary>
		/// How many objects were activated so far.
		/// </summary>
		public int ActivationCount
		{
			get
			{
				return _activationCount;
			}
		}

		void OnActivated(object sender, ObjectEventArgs args)
		{
			++_activationCount;
		}

		void OnQueryFinished(object sender, QueryEventArgs args)
		{
			_watch.Stop();
		}

		void OnQueryStarted(object sender, QueryEventArgs args)
		{
			_activationCount = 0;
			_watch.Start();
		}

		/// <summary>
		/// Starts gathering query statistics for the specified container.
		/// </summary>
		/// <param name="container"></param>
		public QueryStats(ObjectContainer container, string label)
		{
			_label = label;
			if (_registry != null)
			{
				throw new ArgumentException("Already connected to an ObjectContainer");
			}
			_registry = EventRegistryFactory.ForObjectContainer(container);
			_registry.QueryStarted += new QueryEventHandler(OnQueryStarted);
			_registry.QueryFinished += new QueryEventHandler(OnQueryFinished);
			_registry.Activated += new ObjectEventHandler(OnActivated);
		}

		/// <summary>
		/// Disconnects from the current container.
		/// </summary>
		public void Disconnect()
		{
			if (null != _registry)
			{
				_registry.QueryStarted -= new QueryEventHandler(OnQueryStarted);
				_registry.QueryFinished -= new QueryEventHandler(OnQueryFinished);
				_registry.Activated -= new ObjectEventHandler(OnActivated);
				_registry = null;
			}
		}
	}

}
