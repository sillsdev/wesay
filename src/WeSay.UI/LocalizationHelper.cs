using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Foundation;

namespace WeSay.UI
{
	[Designer(typeof (LocalizationHelperDesigner))]
	[ToolboxItem(true)]
	[ProvideProperty("ParentFo", typeof (Form))]
	public partial class LocalizationHelper : Component, ISupportInitialize, IExtenderProvider
	{
		private bool _alreadyChanging;
		private Control _parent;

		public LocalizationHelper()
		{
			InitializeComponent();
		}

		public LocalizationHelper(IContainer container)
		{
			if (container != null)
			{
				container.Add(this);
			}

			InitializeComponent();
		}

		public Control Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}

		private void OnFontChanged(object sender, EventArgs e)
		{
			if (_alreadyChanging)
			{
				return;
			}
			Control control = (Control) sender;
			_alreadyChanging = true;
			if (!(control is WeSay.UI.Buttons.RegionButton))//making a big font on these things that don't have text was causing them to grow
			{
				control.Font = StringCatalog.ModifyFontForLocalization(control.Font);
			}
			_alreadyChanging = false;
		}

		private void OnTextChanged(object sender, EventArgs e)
		{
			if (_alreadyChanging)
			{
				return;
			}
			Control control = (Control)sender;

			if (control.Text.Contains("{0}"))
			{
				return; //they're going to have to format it anyways, so we can't fix it automatically
			}

			_alreadyChanging = true;
			if (!String.IsNullOrEmpty(control.Text)) //don't try to translation, for example, buttons with no label
			{
				control.Text = StringCatalog.Get(control.Text);
			}
			_alreadyChanging = false;
		}

		void OnControlAdded(object sender, ControlEventArgs e)
		{
			WireToConrol(e.Control);
			WireToChildren(e.Control);
		}

		#region ISupportInitialize Members

		///<summary>
		///Signals the object that initialization is starting.
		///</summary>
		///
		public void BeginInit() {}

		///<summary>
		///Signals the object that initialization is complete.
		///</summary>
		///
		public void EndInit()
		{
			WireToChildren(Parent);
		}

		private void WireToChildren(Control control)
		{
			control.ControlAdded += new ControlEventHandler(OnControlAdded);
			//Debug.WriteLine("Wiring to children of " + control.Name);
			foreach (Control child in control.Controls)
			{
				WireToConrol(child);
				WireToChildren(child);
			}
		}

		private void WireToConrol(Control control)
		{
			if (control is Label || control is IButtonControl)
			{
				// Debug.WriteLine("Wiring to " + control.Name);
				control.TextChanged += new EventHandler(OnTextChanged);
				control.FontChanged += new EventHandler(OnFontChanged);

				OnTextChanged(control, null);
				OnFontChanged(control, null);
			}
		}

		#endregion

		#region IExtenderProvider Members

		///<summary>
		///Specifies whether this object can provide its extender properties to the specified object.
		///</summary>
		///
		///<returns>
		///true if this object can provide extender properties to the specified object; otherwise, false.
		///</returns>
		///
		///<param name="extendee">The <see cref="T:System.Object"></see> to receive the extender properties. </param>
		public bool CanExtend(object extendee)
		{
			return (extendee is UserControl);
		}

		#endregion
	}

	/// <summary>
	///   Designer object used to set the Parent property.
	/// </summary>
	internal class LocalizationHelperDesigner : ComponentDesigner
	{
		///   <summary>
		///   Sets the Parent property to "this" -
		///   the Form/UserControl where the component is being dropped.
		///   </summary>
		[Obsolete()]
		public override void OnSetComponentDefaults()
		{
			LocalizationHelper rp = (LocalizationHelper) Component;
			rp.Parent = (Control) Component.Site.Container.Components[0];
		}
	}
}