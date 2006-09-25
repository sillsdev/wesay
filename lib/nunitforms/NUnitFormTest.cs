#region Copyright (c) 2003-2005, Luke T. Maxon

/********************************************************************************************************************
'
' Copyright (c) 2003-2005, Luke T. Maxon
' All rights reserved.
'
' Redistribution and use in source and binary forms, with or without modification, are permitted provided
' that the following conditions are met:
'
' * Redistributions of source code must retain the above copyright notice, this list of conditions and the
' 	following disclaimer.
'
' * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and
' 	the following disclaimer in the documentation and/or other materials provided with the distribution.
'
' * Neither the name of the author nor the names of its contributors may be used to endorse or
' 	promote products derived from this software without specific prior written permission.
'
' THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
' WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
' PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
' ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
' LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
' INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
' OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
' IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
'
'*******************************************************************************************************************/

#endregion

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using NUnit.Framework;

//using NUnitForms.ScreenCapture;

namespace NUnit.Extensions.Forms
{
  /// <summary>
  /// One of three base classes for your NUnitForms tests.  This one can be
  /// used by people who do not need or want "built-in" Assert functionality.
  ///
  /// This is the recommended base class for all unit tests that use NUnitForms.
  /// </summary>
  /// <remarks>
  /// You should probably extend this class to create all of your test fixtures.  The benefit is that
  /// this class implements setup and teardown methods that clean up after your test.  Any forms that
  /// are created and displayed during your test are cleaned up by the tear down method.  This base
  /// class also provides easy access to keyboard and mouse controllers.  It has a method that allows
  /// you to set up a handler for modal dialog boxes.  It allows your tests to run on a separate
  /// (usually hidden) desktop so that they are faster and do not interfere with your normal desktop
  /// activity.  If you want custom setup and teardown behavior, you should override the virtual
  /// Setup and TearDown methods.  Do not use the setup and teardown attributes in your child class.
  /// </remarks>
  public class NUnitFormTest
  {
	  protected bool verified = false;

	  private Thread guiThread;

	  private Form formOnThread;

	private static readonly FieldInfo isUserInteractive =
			typeof(SystemInformation).GetField("isUserInteractive", BindingFlags.Static | BindingFlags.NonPublic);

	private Form currentForm = null;

	private MouseController mouse = null;

	private KeyboardController keyboard = null;

	private Desktop testDesktop;

	/// <summary>
	/// This property controls whether the separate hidden desktop is displayed for the duration of
	/// this test.  You will need to override and return true from this property if your test makes
	/// use of the keyboard or mouse controllers.  (The hidden desktop cannot accept user input.)  For
	/// tests that do not use the keyboard and mouse controller (most should not) you don't need to do
	/// anything with this.  The default behavior is fine.
	/// </summary>
	public virtual bool DisplayHidden
	{
	  get
	  {
		return false;
	  }
	}

	/// <summary>
	/// This property controls whether a separate desktop is used at all.  I highly recommend that you
	/// leave this as returning true.  Tests on the separate desktop are faster and safer.  (There is
	/// no danger of keyboard or mouse input going to your own separate running applications.)  However
	/// I have heard report of operating systems or environments where the separate desktop does not work.
	/// In that case there are 2 options.  You can override this method from your test class to return false.
	/// Or you can set an environment variable called "UseHiddenDesktop" and set that to "false"  Either will
	/// cause the tests to run on your original, standard desktop.
	/// </summary>
	/// <remarks>
	/// <list type="bullet">
	/// <li>This method now defaults to <c>false</c>. When the problems with the separate desktop are solved, this
	/// method will again return <c>true</c>.</li>
	/// <li>An <c>else</c> branch to deal with <c>UseHiddenDesktop</c> is <c>TRUE</c>.</li>
	/// </list>
	/// </remarks>
	public virtual bool UseHidden
	{
	  get
	  {
		string useHiddenDesktop = Environment.GetEnvironmentVariable("UseHiddenDesktop");
		if (useHiddenDesktop != null && useHiddenDesktop.ToUpper().Equals("FALSE"))
		{
		  return false;
		}
		else if (useHiddenDesktop != null && useHiddenDesktop.ToUpper().Equals("TRUE"))
		{
		  return true;
		}

		return false;
	  }
	}

	/// <summary>
	/// It is used only in combination with the FormType property.
	/// If you override the FormType property, then CurrentForm will be initialized (on setup) to
	/// an instance of the form whose type you specify.  None of the testers require a reference
	/// to the active form anymore, so this should not be necessary.
	/// </summary>
	[Obsolete()]
	public Form CurrentForm
	{
	  get
	  {
		return currentForm;
	  }
	  set
	  {
		currentForm = value;
	  }
	}

	/// <summary>
	/// Returns a reference to the current MouseController for doing Mouse tests.  I recommend
	/// this only when you are writing your own custom controls and need to respond to actual
	/// mouse input to test them properly.  In most other cases there is a better way to test
	/// the form's logic.
	/// </summary>
	public MouseController Mouse
	{
	  get
	  {
		return mouse;
	  }
	}

	/// <summary>
	/// Returns a reference to the current KeyboardController for doing Keyboard tests.  I recommend
	/// this only when you are writing your own custom controls and need to respond to actual
	/// keyboard input to test them properly.  In most other cases there is a better way to test
	/// for the form's logic.
	/// </summary>
	public KeyboardController Keyboard
	{
	  get
	  {
		return keyboard;
	  }
	}

	/// <summary>
	/// This is the base classes setup method.  It will be called by NUnit before each test.
	/// You should not have anything to do with it.
	/// </summary>
	[SetUp]
	public void init()
	{
	  verified = false;
	  if (!SystemInformation.UserInteractive)
	  {
		isUserInteractive.SetValue(null, true);
	  }

	  if (UseHidden)
	  {
		testDesktop = new Desktop("NUnitForms Test Desktop", DisplayHidden);
	  }

	  modal = new ModalFormTester();

	  BaseSetup();

	  mouse = new MouseController();
	  keyboard = new KeyboardController();

	  if (CurrentForm != null)
	  {
		currentForm.Show();
	  }
	  Setup();
	}

	private ModalFormTester modal;

	/// <summary>
	/// This method is needed because the way the FileDialogs working are strange.
	/// It seems that both open/save dialogs initial title is "Open". The handler
	/// </summary>
	/// <param name="modalHandler"></param>
	protected void ExpectFileDialog(string modalHandler)
	{
	  ExpectModal("Open", modalHandler);
	}

	protected void ExpectFileDialog(string modalHandler, bool expected)
	{
	  ExpectModal("Open", modalHandler, expected);
	}

	protected void ExpectFileDialog(ModalFormActivated handler)
	{
	  modal.ExpectModal("Open", handler, true);
	}

	protected void ExpectFileDialog(ModalFormActivated handler, bool expected)
	{
	  modal.ExpectModal("Open", handler, true);
	}

	/// <summary>
	/// One of four overloaded methods to set up a modal dialog handler.  If you expect a modal
	/// dialog to appear and can handle it during the test, use this method to set up the handler.
	/// </summary>
	/// <param name="name">The caption on the dialog you expect.</param>
	/// <param name="handler">The method to call when that dialog appears.</param>
	protected void ExpectModal(string name, ModalFormActivated handler)
	{
	  modal.ExpectModal(name, handler, true);
	}

	/// <summary>
	/// One of four overloaded methods to set up a modal dialog handler.  If you expect a modal
	/// dialog to appear and can handle it during the test, use this method to set up the handler.
	/// Because "expected" is usually (always) true if you are calling this, I don't expect it will
	/// be used externally.
	/// </summary>
	/// <param name="name">The caption on the dialog you expect.</param>
	/// <param name="handler">The method to call when that dialog appears.</param>
	/// <param name="expected">A boolean to indicate whether you expect this modal dialog to appear.</param>
	protected void ExpectModal(string name, ModalFormActivated handler, bool expected)
	{
	  modal.ExpectModal(name, handler, expected);
	}

	/// <summary>
	/// One of four overloaded methods to set up a modal dialog handler.  If you expect a modal
	/// dialog to appear and can handle it during the test, use this method to set up the handler.
	/// Because "expected" is usually (always) true if you are calling this, I don't expect it will
	/// be used externally.
	/// </summary>
	/// <param name="name">The caption on the dialog you expect.</param>
	/// <param name="handlerName">The name of the method to call when that dialog appears.</param>
	/// <param name="expected">A boolean to indicate whether you expect this modal dialog to appear.</param>
	protected void ExpectModal(string name, string handlerName, bool expected)
	{
	  ExpectModal(name,
				  (ModalFormActivated)Delegate.CreateDelegate(typeof(ModalFormActivated), this, handlerName),
				  expected);
	}

	/// <summary>
	/// One of four overloaded methods to set up a modal dialog handler.  If you are not sure which
	/// to use, use this one.  If you expect a modal dialog to appear and can handle it during the
	/// test, use this method to set up the handler. Because "expected" is usually (always) true
	/// if you are calling this, I don't expect it will be used externally.
	/// </summary>
	/// <param name="name">The caption on the dialog you expect.</param>
	/// <param name="handlerName">The name of the method to call when that dialog appears.</param>
	protected void ExpectModal(string name, string handlerName)
	{
	  ExpectModal(name, handlerName, true);
	}



	/// <summary>
	/// In your test class, you used to have the choice to override this
	/// method, or implement the FormType property.  Now neither is necessary.  It is still
	/// here for compatibility with tests written to use the CurrentForm property.
	/// </summary>
	[Obsolete()]
	public virtual void BaseSetup()
	{
	  CurrentForm = ActivateForm();
	}

	/// <summary>
	/// Override this Setup method if you have custom behavior to execute before each test
	/// in your fixture.
	/// </summary>
	public virtual void Setup()
	{
	}

	/// <summary>
	/// This method is called before each test in order
	/// to set the CurrentForm property (also obsolete)  You can override this method as an
	/// alternative to setting the FormType property if you want to test the old way.
	/// </summary>
	/// <returns></returns>
	[Obsolete()]
	public virtual Form ActivateForm()
	{
	  if (FormType != null)
	  {
		return (Form)Activator.CreateInstance(FormType);
	  }
	  return null;
	}

	/// <summary>
	/// Compare the screen capture of a control with a stored screen capture of this control.
	/// </summary>
	/// <param name="filePath">
	/// The path to the screen capture of a control. This file path has the following format :
	/// <c>nameForm_number.png</c>.
	/// </param>
	protected virtual void CompareControlCapture(string filePath)
	{
//      string formName;
//      formName = Path.GetFileName(filePath);
//      formName = formName.Substring(0, formName.LastIndexOf('_'));
//      Form formUnderTest = new FormFinder().Find(formName);
//      Assert.AreEqual(formName, formUnderTest.Name);
//      Bitmap expectedCapture = new Bitmap(filePath);
//      ScreenCapture screenCapture = new ScreenCapture();
//      screenCapture.Capture(formUnderTest, @"NUnitFormsCapture\").Save(@"g:\temp\cap.png");
//      Assert.IsTrue(AreBinaryEqual(filePath,@"g:\temp\cap.png"));
	}

	  /// <summary>
	  /// A binary compare of two files.
	  /// </summary>
	  /// <param name="filePathOne">
	  /// The path of the first file.
	  /// </param>
	  /// <param name="filePathTwo">
	  /// The path of the second file.
	  /// </param>
	  /// <returns>
	  ///
	  /// </returns>
	  protected bool AreBinaryEqual(string filePathOne, string filePathTwo)
	  {
		  bool equal = true;
		  BinaryReader fileOne = null;
		  BinaryReader fileTwo = null;
		  if (File.Exists(filePathOne) && File.Exists(filePathTwo))
		  {
			  fileOne = new BinaryReader(File.OpenRead(filePathOne));
			  fileTwo = new BinaryReader(File.OpenRead(filePathTwo));
			  while (fileOne.PeekChar() != -1 && fileTwo.PeekChar() != -1 && equal)
			  {
				  equal = fileOne.ReadByte().Equals(fileTwo.ReadByte());
			  }
			  return equal & (fileOne.PeekChar() == fileTwo.PeekChar());
		  }
		  else
		  {
			  return false;
		  }

	  }

	protected virtual void CompareCapture(Form form, string filePath)
	{
		formOnThread = form;
		guiThread = new Thread(new ThreadStart(this.RunForm));
		guiThread.Start();
		Thread.Sleep(2000);
		CompareControlCapture(filePath);
	}

	  protected void RunForm()
	  {
		  Application.Run(formOnThread);
	  }


	/// <summary>
	/// This property specifies the type of form to instantiate
	/// before each test.
	/// </summary>
	[Obsolete()]
	public virtual Type FormType
	{
	  get
	  {
		return null;
	  }
	}



	/// <summary>
	/// This method is called by NUnit after each test runs.  If you have custom
	/// behavior to run after each test, then override the TearDown method and do
	/// it there.  That method is called at the beginning of this one.
	/// You should not need to do anything with it.  Do not call it.
	/// If you do call it, call it as the last thing you do in your test.
	/// </summary>
	[TearDown]
	public void Verify()
	{
	  TearDown();

	  if (!verified)
	  {
		verified = true;
		FormCollection allForms = new FormFinder().FindAll();

		foreach (Form form in allForms)
		{
		  if (!KeepAlive(form))
		  {
			form.Dispose();
			form.Hide();
		  } //else branch not tested
		}

		bool modalVerify = modal.Verify();

		modal.Dispose();

		if (UseHidden)
		{
		  testDesktop.Dispose();
		}

		mouse.Dispose();
		keyboard.Dispose();

		if (!modalVerify)
		{
		  throw new FormsTestAssertionException("unexpected/expected modal was invoked/not invoked");
		}
	  }
	}

	internal bool KeepAlive(Form form)
	{
	  return form is IKeepAlive && (form as IKeepAlive).KeepAlive;
	}

	/// <summary>
	/// This method is called after each test.  Put code here to clean up anything
	/// you need to between tests.  NUnitForms cleans up most everything you need
	/// related to the framework (closes extra windows, etc..) but you might need
	/// custom behavior beyond this.  Put it here.
	/// </summary>
	public virtual void TearDown()
	{
	}
  }

  public interface IKeepAlive
  {
	bool KeepAlive
	{
	  get;
	}
  }
}