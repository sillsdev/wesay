using System;
using System.Collections.Generic;
using System.Text;
using Gtk;
using WeSay.UI;

namespace WeSay.App
{
	public class DummyTool : WeSay.UI.ITool
	{
		private HBox _container;

		public HBox Container
		{
			get { return _container; }
			set { _container = value; }
		}

		public DummyTool()
		{
		}

		public void Activate()
		{
			TextBuffer buffer = new TextBuffer(null);
			buffer.Text = "Now is the time for all good men to come to the aid of the party.";

			_container.PackStart(CreateText(buffer));
			_container.ShowAll();
		}

		public void Deactivate()
		{
			_container.Remove(_container.Children[0]);
		}

		public string Label
		{
			get {return "dummy";}
		}

		private ScrolledWindow CreateText(TextBuffer buffer)
		{
			ScrolledWindow scrolledWindow = new ScrolledWindow();
			scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
			scrolledWindow.ShadowType = ShadowType.In;

			TextView textView = new TextView(buffer);
			textView.Editable = false;
			textView.CursorVisible = false;

			scrolledWindow.Add(textView);


			// Make it a bit nicer for text
			textView.WrapMode = Gtk.WrapMode.Word;
			textView.PixelsAboveLines = 2;
			textView.PixelsBelowLines = 2;


			return scrolledWindow;
		}
	}
}
