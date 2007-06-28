using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeSay.Language;

namespace WeSay.UI
{
	[Designer(typeof(LocalizationHelperDesigner))]
	[ToolboxItem(true)]
	[ProvideProperty("ParentFo", typeof(Form))]

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
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

		void OnFontChanged(object sender, EventArgs e)
		{
			if (_alreadyChanging)
			{
				return;
			}
			Control control = sender as Control;
			_alreadyChanging = true;
			control.Font = StringCatalog.ModifyFontForLocalization(control.Font);
			_alreadyChanging = false;
		}

		void OnTextChanged(object sender, EventArgs e)
		{
			if (_alreadyChanging)
			{
				return;
			}
			Control control = sender as Control;

			if(control.Text.Contains("{0}"))
			{
				return;//they're going to have to format it anyways, so we can't fix it automatically
			}

			_alreadyChanging = true;
			if (!String.IsNullOrEmpty(control.Text))//don't try to translation, for example, buttons with no label
			{
				control.Text = StringCatalog.Get(control.Text);
			}
			_alreadyChanging = false;
		}

		#region ISupportInitialize Members

		///<summary>
		///Signals the object that initialization is starting.
		///</summary>
		///
		public void BeginInit()
		{
		}

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
			//Debug.WriteLine("Wiring to children of " + control.Name);
			foreach (Control child in control.Controls)
			{
				if (child is Label || child is Button)
				{
					// Debug.WriteLine("Wiring to " + child.Name);
					child.TextChanged += new EventHandler(OnTextChanged);
					child.FontChanged += new EventHandler(OnFontChanged);

					OnTextChanged(child, null);
					OnFontChanged(child, null);
				}
				WireToChildren(child);
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
		public override void OnSetComponentDefaults()
		{
			LocalizationHelper rp = (LocalizationHelper)Component;
			rp.Parent = (Control)Component.Site.Container.Components[0];
		}
	}	}
