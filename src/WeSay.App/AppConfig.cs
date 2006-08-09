using System;
using System.Collections;
using System.Collections.Generic;
using Gtk;
using WeSay.UI;


namespace WeSay.App
{
	public class AppConfig
	{

		public AppConfig(Notebook _notebook, Hashtable tabsToTools)
		{
			AddTool(new DummyTool(), _notebook,tabsToTools );
			AddTool(new WeSay.LexicalTools.EntryViewTool(), _notebook,tabsToTools );
	   }

		private void AddTool(ITool tool, Notebook _notebook, Hashtable tabsToTools)
		{
			HBox container = new HBox();
			tool.Container = container;

			int i = _notebook.AppendPage(container, new Label(tool.Label));
			tabsToTools.Add(i, tool);
		}



		//private System.Object CreateObject(string assemblyPath, string className, object[] args)
		//{
		//    // Whitespace will cause failures.
		//    assemblyPath = assemblyPath.Trim();
		//    className = className.Trim();
		//    //allow us to say "assemblyPath="%fwroot%\Src\XCo....  , at least during testing
		//    // RR: It may allow it, but it crashes, when it can't find the dll.
		//    //assemblyPath = System.Environment.ExpandEnvironmentVariables(assemblyPath);
		//    string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Substring(6);

		//    Assembly assembly = null;
		//    try
		//    {
		//        assembly = Assembly.LoadFrom(Path.Combine(baseDir, assemblyPath));
		//    }
		//    catch (Exception)
		//    {
		//        try
		//        {
		//            //Try to find without specifying the directory,
		//            //so that we find things that are in the Path environment variable
		//            //This is useful in extension situations where the extension's bin directory
		//            //is not the same as the FieldWorks binary directory (e.g. WeSay)
		//            assembly = Assembly.LoadFrom(assemblyPath);
		//        }
		//        catch (Exception error)
		//        {
		//            throw new RuntimeConfigurationException("XCore Could not find the DLL at :" + assemblyPath, error);
		//        }
		//    }


		//    Object thing = null;
		//    try
		//    {
		//        //make the object
		//        //Object thing = assembly.CreateInstance(className);
		//        thing = assembly.CreateInstance(className, false, BindingFlags.Instance | BindingFlags.Public,
		//            null, args, null, null);
		//    }
		//    catch (Exception err)
		//    {
		//        Debug.WriteLine(err.Message);
		//        string message = CouldNotCreateObjectMsg(assemblyPath, className);

		//        Exception inner = err;

		//        while (inner != null)
		//        {
		//            message += "\r\nInner exception message = " + inner.Message;
		//            inner = inner.InnerException;
		//        }
		//        throw new ConfigurationException(message);
		//    }
		//    if (thing == null)
		//    {
		//        // Bizarrely, CreateInstance is not specified to throw an exception if it can't
		//        // find the specified class. But we want one.
		//        throw new ConfigurationException(CouldNotCreateObjectMsg(assemblyPath, className));
		//    }
		//    return thing;
		//}
	}
}
