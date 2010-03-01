// Import standard OpenOffice.org API classes. For more information on
// these classes and the OpenOffice.org API, see the OpenOffice.org
// Developers Guide at:
// http://api.openoffice.org/

importClass(Packages.com.sun.star.awt.XMessageBoxFactory);
importClass(Packages.com.sun.star.awt.XWindow);
importClass(Packages.com.sun.star.awt.XWindowPeer);
importClass(Packages.com.sun.star.awt.Rectangle);
importClass(Packages.com.sun.star.document.XDocumentEventBroadcaster);
importClass(Packages.com.sun.star.document.XDocumentEventListener);
importClass(Packages.com.sun.star.uno.UnoRuntime);
importClass(Packages.com.sun.star.text.XTextDocument);
importClass(Packages.com.sun.star.text.XText);
importClass(Packages.com.sun.star.text.XTextRange);
importClass(Packages.com.sun.star.text.XTextRange);
importClass(Packages.com.sun.star.text.XTextFrame);
importClass(Packages.com.sun.star.text.XTextContent);
importClass(Packages.com.sun.star.text.XTextViewCursorSupplier);
importClass(Packages.com.sun.star.text.XPageCursor);
importClass(Packages.com.sun.star.text.XTextCursor);
importClass(Packages.com.sun.star.text.XSimpleText);
importClass(Packages.com.sun.star.text.XTextFieldsSupplier);
importClass(Packages.com.sun.star.text.XTextFramesSupplier);
importClass(Packages.com.sun.star.text.XTextRangeCompare);
importClass(Packages.com.sun.star.text.XTextSectionsSupplier);
importClass(Packages.com.sun.star.text.XDependentTextField);
importClass(Packages.com.sun.star.text.TextContentAnchorType);
importClass(Packages.com.sun.star.text.SetVariableType);
importClass(Packages.com.sun.star.text.WrapTextMode);
importClass(Packages.com.sun.star.text.VertOrientation);
importClass(Packages.com.sun.star.text.HoriOrientation);
importClass(Packages.com.sun.star.text.RelOrientation);
importClass(Packages.com.sun.star.text.SetVariableType);
importClass(Packages.com.sun.star.style.NumberingType);
importClass(Packages.com.sun.star.style.XStyleFamiliesSupplier);
importClass(Packages.com.sun.star.table.BorderLine);
importClass(Packages.com.sun.star.frame.XModel);
importClass(Packages.com.sun.star.frame.XModel);
importClass(Packages.com.sun.star.lang.XMultiServiceFactory);
importClass(Packages.com.sun.star.uno.TypeClass);
importClass(Packages.com.sun.star.util.XRefreshable);
importClass(Packages.com.sun.star.util.XRefreshListener);
importClass(Packages.com.sun.star.beans.XPropertySet);
importClass(Packages.com.sun.star.container.XEnumerationAccess);
importClass(Packages.com.sun.star.container.XEnumeration);
importClass(Packages.com.sun.star.container.XNameAccess);
importClass(Packages.com.sun.star.container.XNamed);

// Import XScriptContext class. An instance of this class is available
// to all JavaScript scripts in the global variable "XSCRIPTCONTEXT". This
// variable can be used to access the document for which this script
// was invoked.
//
// Methods available are:
//
//   XSCRIPTCONTEXT.getDocument() returns XModel
//   XSCRIPTCONTEXT.getInvocationContext() returns XScriptInvocationContext or NULL
//   XSCRIPTCONTEXT.getDesktop() returns XDesktop
//   XSCRIPTCONTEXT.getComponentContext() returns XComponentContext
//
// For more information on using this class see the scripting
// developer guides at:
//
//   http://api.openoffice.org/docs/DevelopersGuide/ScriptingFramework/ScriptingFramework.xhtml
//

FRAME_STEM= "DictFieldFramePage";
DICT_SECTION = "Dictionary";
SET_EXPRESSION = "com.sun.star.text.FieldMaster.SetExpression";
DICT_FIRST_WORD_ON_PAGE = "DictFirstWordOnPage";
DICT_LAST_WORD_ON_PAGE = "DictLastWordOnPage";
ENTRY_STYLE = "entry";
LEXICAL_UNIT_STYLE = "lexical-unit";

oDoc = UnoRuntime.queryInterface(XModel,XSCRIPTCONTEXT.getInvocationContext());
if ( !oDoc )
  oDoc = XSCRIPTCONTEXT.getDocument();

xFrame = XSCRIPTCONTEXT.getDesktop().getCurrentFrame();
xController = null;
xWindow = null;
xWindowPeer = null;
xMessageBoxFactory = null;
if (xFrame != null)
{
	xController = xFrame.getController();
	xWindow = xFrame.getContainerWindow();
	xWindowPeer = UnoRuntime.queryInterface(XWindowPeer, xWindow);
	xToolkit = xWindowPeer.getToolkit();
	xMessageBoxFactory =  UnoRuntime.queryInterface(XMessageBoxFactory, xToolkit);
}
rect = new Rectangle();
rect.X = 200;
rect.Y = 200;
rect.Width = 600;
rect.Height = 100;

function dumpProperties(props)
{
	dump = "";
	propList = props.getPropertySetInfo().getProperties();
	for (i = 0; i < propList.length; i++)
	{
		try
		{
			dump += propList[i].Name + " " + propList[i].Type + " " + props.getPropertyValue(propList[i].Name) + "\t";
		 }
		 catch(e) { dump += e; }
	}
	xMessageBox = xMessageBoxFactory.createMessageBox(xWindowPeer, rect, "infobox", 0,
		"Properties", dump);
	xMessageBox.execute();
	return dump;
}

function updateDictHeaders(oDoc, xController)
{
	if (xController == null) return;
//need a text document view service
xViewCursorSupplier = UnoRuntime.queryInterface(XTextViewCursorSupplier, xController);
xViewCursor = xViewCursorSupplier.getViewCursor();
xViewCursor.setVisible(false);


xTextDoc = UnoRuntime.queryInterface(XTextDocument,oDoc);
xText = xTextDoc.getText();
xRangeCompare = UnoRuntime.queryInterface(XTextRangeCompare, xText);
xEnumAccess = UnoRuntime.queryInterface(XEnumerationAccess,xText);
xParaEnum = xEnumAccess.createEnumeration();
para = null;
prevPara = null;
if (xParaEnum.hasMoreElements())
{
	para = xParaEnum.nextElement();
}

docFactory = UnoRuntime.queryInterface(XMultiServiceFactory, oDoc);

//xSectionSupplier = UnoRuntime.queryInterface(XTextSectionsSupplier, oDoc);
//dictSection = xSectionSupplier.getTextSections.getByName(DICT_SECTION);

xFrameSupplier = UnoRuntime.queryInterface(XTextFramesSupplier, oDoc);
xFrameNames = xFrameSupplier.getTextFrames();

xPageCursor = UnoRuntime.queryInterface(XPageCursor, xViewCursor);
pageXTextCursor = UnoRuntime.queryInterface(XTextCursor, xViewCursor);

origPage = xPageCursor.getPage();
xPageCursor.jumpToFirstPage();
page = xPageCursor.getPage();

function getEntryWord(para)
{
	if (para == null) return "";
	word = "";
	portionEnum = UnoRuntime.queryInterface(XEnumerationAccess, para).createEnumeration();
	while (portionEnum.hasMoreElements())
	{
		portion = portionEnum.nextElement();
		portionProps =  UnoRuntime.queryInterface(XPropertySet, portion);
		textPortionType = portionProps.getPropertyValue("TextPortionType");
		if (textPortionType != "Text") continue;
		propStyle = portionProps.getPropertyValue("CharStyleName");
		if (propStyle.indexOf(LEXICAL_UNIT_STYLE) == 0)
		{
			portionRange = UnoRuntime.queryInterface(XTextRange, portion);
			word = portionRange.getString();
			break;
		}
	}
	return word;
}

function getLexicalUnitLanguage(para)
{
	portionEnum = UnoRuntime.queryInterface(XEnumerationAccess, para).createEnumeration();
	while (portionEnum.hasMoreElements())
	{
		portion = portionEnum.nextElement();
		portionProps =  UnoRuntime.queryInterface(XPropertySet, portion);
		propStyle = portionProps.getPropertyValue("CharStyleName");
		if (propStyle.indexOf(LEXICAL_UNIT_STYLE) == 0)
		{
			langOffset = propStyle.indexOf(" ") + 1;
			return propStyle.substring(langOffset, propStyle.length());
		}
	 }
	 return null;
}

// retrieve the master from the doc when it already exists
xFirstWordMasterPropSet = null;
xLastWordMasterPropSet = null;
xTextFieldsSupplier = UnoRuntime.queryInterface(XTextFieldsSupplier, oDoc);
fieldMasters = xTextFieldsSupplier.getTextFieldMasters();
masterNames = fieldMasters.getElementNames();
if (fieldMasters.hasByName(SET_EXPRESSION + "." + DICT_FIRST_WORD_ON_PAGE))
{
	fieldMaster =  fieldMasters.getByName(SET_EXPRESSION + "." + DICT_FIRST_WORD_ON_PAGE);
	xFirstWordMasterPropSet = UnoRuntime.queryInterface(
		XPropertySet, fieldMaster);
	 if (xFirstWordMasterPropSet.getPropertyValue("SubType") != SetVariableType.STRING)
	{
		xMessageBox = xMessageBoxFactory.createMessageBox(xWindowPeer, rect, "infobox", 0, DICT_FIRST_WORD_ON_PAGE + " Subtype",
		xFirstWordMasterPropSet.getPropertyValue("Name") + " " +
		xFirstWordMasterPropSet.getPropertyValue("InstanceName") +
		" Field master has wrong subtype: " + xFirstWordMasterPropSet.getPropertyValue("SubType"));
		xMessageBox.execute();
	}
}
else
{
	xFirstWordMasterPropSet = UnoRuntime.queryInterface(
		XPropertySet, docFactory.createInstance("com.sun.star.text.FieldMaster.SetExpression"));
	 // Set the name and value of the FieldMaster
	xFirstWordMasterPropSet.setPropertyValue ("SubType", SetVariableType.STRING);
	xFirstWordMasterPropSet.setPropertyValue ("Name", DICT_FIRST_WORD_ON_PAGE);
	xFirstWordMasterPropSet.setPropertyValue ("SubType", SetVariableType.STRING);
	// Setting the SubType this way seems to fail, for some reason that I don't understand.
	if (xFirstWordMasterPropSet.getPropertyValue ("SubType") != SetVariableType.STRING)
	{
		xMessageBox = xMessageBoxFactory.createMessageBox(xWindowPeer, rect, "infobox", 0, DICT_FIRST_WORD_ON_PAGE + " Setting SubType failed! Subtype=", xFirstWordMasterPropSet.getPropertyValue("SubType"));
		xMessageBox.execute();
	 }
}
if (fieldMasters.hasByName(SET_EXPRESSION + "." + DICT_LAST_WORD_ON_PAGE))
{
	fieldMaster =  fieldMasters.getByName(SET_EXPRESSION + "." + DICT_LAST_WORD_ON_PAGE);
	xLastWordMasterPropSet = UnoRuntime.queryInterface(
		XPropertySet, fieldMaster);
	if (xLastWordMasterPropSet.getPropertyValue("SubType") != SetVariableType.STRING)
	{
		xMessageBox = xMessageBoxFactory.createMessageBox(xWindowPeer, rect, "infobox", 0, DICT_LAST_WORD_ON_PAGE + " Subtype", "Field master has wrong subtype: " + xLastWordMasterPropSet.getPropertyValue("SubType"));
		xMessageBox.execute();
	}
}
else
{
	xLastWordMasterPropSet = UnoRuntime.queryInterface(
		XPropertySet, docFactory.createInstance("com.sun.star.text.FieldMaster.SetExpression"));
	 // Set the name and value of the FieldMaster
	xLastWordMasterPropSet.setPropertyValue ("SubType", SetVariableType.STRING);
	xLastWordMasterPropSet.setPropertyValue ("Name", DICT_LAST_WORD_ON_PAGE);
}

lexicalUnitLang = null;
// loop over pages
do
{
	// find first entry paragraph after page start
	paraProps = UnoRuntime.queryInterface(XPropertySet, para);
	paraTextContent = UnoRuntime.queryInterface(XTextContent, para);
	while (xRangeCompare.compareRegionStarts(paraTextContent.getAnchor(), pageXTextCursor)>0 ||
			   ! paraProps.getPropertyValue("ParaStyleName").equals(ENTRY_STYLE))
	{
		if (xParaEnum.hasMoreElements())
		{
			if (paraProps.getPropertyValue("ParaStyleName").equals(ENTRY_STYLE))
				prevPara = para;
			para = xParaEnum.nextElement();
			paraProps = UnoRuntime.queryInterface(XPropertySet, para);
			paraTextContent = UnoRuntime.queryInterface(XTextContent, para);
		}
		else break;
	}
	firstWord = getEntryWord(para);
	if (lexicalUnitLang == null)
	{
		lexicalUnitLang = getLexicalUnitLanguage(para);
	}
	// find last entry on page
	xPageCursor.jumpToEndOfPage();
	while (xRangeCompare.compareRegionStarts(paraTextContent.getAnchor(), pageXTextCursor)>0 ||
			   ! paraProps.getPropertyValue("ParaStyleName").equals(ENTRY_STYLE))
	{
		if (xParaEnum.hasMoreElements())
		{
			if (paraProps.getPropertyValue("ParaStyleName").equals(ENTRY_STYLE))
				prevPara = para;
			para = xParaEnum.nextElement();
			paraProps = UnoRuntime.queryInterface(XPropertySet, para);
			paraTextContent = UnoRuntime.queryInterface(XTextContent, para);
		}
		else
		{
			if (paraProps.getPropertyValue("ParaStyleName").equals(ENTRY_STYLE))
				prevPara = para;
			break;
		}
	}
	lastWord = getEntryWord(prevPara);
	// use a textframe anchored to each page to hold the current first and last words
	textFrame = null;
	if (xFrameNames.hasByName(FRAME_STEM+page))
	{
		textFrame = xFrameNames.getByName(FRAME_STEM+page);
		xFrameProps = UnoRuntime.queryInterface(XPropertySet, textFrame);
		framePage = xFrameProps.getPropertyValue("AnchorPageNo");
		if (framePage != page)
		{
			// it is no longer on the correct page, so remove it
			xText.removeTextContent(textFrame);
			textFrame = null;
		}
	}
	if (textFrame == null)
	{
		textFrame = docFactory.createInstance ("com.sun.star.text.TextFrame");
		line = new BorderLine();
		line.Color = 0x0F000000;
		line.InnerLineWidth = 0;
		line.OuterLineWidth = 0;
		xFrameProps = UnoRuntime.queryInterface(XPropertySet, textFrame);
		xFrameProps.setPropertyValue("BottomBorder", line);
		xFrameProps.setPropertyValue("TopBorder", line);
		xFrameProps.setPropertyValue("LeftBorder", line);
		xFrameProps.setPropertyValue("RightBorder", line);
		xFrameProps.setPropertyValue("Surround", WrapTextMode.THROUGHT);
		xFrameProps.setPropertyValue("BackTransparent", true);
		xFrameProps.setPropertyValue("Opaque", false);
		xFrameProps.setPropertyValue("VertOrientRelation", RelOrientation.FRAME);
		xFrameProps.setPropertyValue("HoriOrientRelation", RelOrientation.FRAME);
		xFrameProps.setPropertyValue("VertOrient", VertOrientation.TOP);
		xFrameProps.setPropertyValue("HoriOrient", HoriOrientation.LEFT);
		xFrameProps.setPropertyValue("AnchorType", TextContentAnchorType.AT_PAGE);
		xName = UnoRuntime.queryInterface(XNamed, textFrame);
		xName.setName(FRAME_STEM+page);
		xTextContent = UnoRuntime.queryInterface(XTextContent, textFrame);
		xText.insertTextContent(pageXTextCursor, xTextContent, false);
	}
	xTextFrame = UnoRuntime.queryInterface(XTextFrame, textFrame);
	xTextFrame.getText().setString("");
	firstWordField = docFactory.createInstance("com.sun.star.text.textfield.SetExpression");
	lastWordField = docFactory.createInstance("com.sun.star.text.textfield.SetExpression");
	firstWordProps = UnoRuntime.queryInterface(XPropertySet, firstWordField);
	lastWordProps = UnoRuntime.queryInterface(XPropertySet, lastWordField);
	firstWordProps.setPropertyValue("SubType", SetVariableType.STRING);
	lastWordProps.setPropertyValue("SubType", SetVariableType.STRING);
	firstWordProps.setPropertyValue("Content", firstWord);
	lastWordProps.setPropertyValue("Content", lastWord);
	xDependentTextField = UnoRuntime.queryInterface(XDependentTextField, firstWordField);
	xDependentTextField.attachTextFieldMaster(xFirstWordMasterPropSet);
	xDependentTextField = UnoRuntime.queryInterface(XDependentTextField, lastWordField);
	xDependentTextField.attachTextFieldMaster(xLastWordMasterPropSet);

	firstWordProps.setPropertyValue("IsVisible", false);
	lastWordProps.setPropertyValue("IsVisible", false);
	xTextFrame.getText().insertTextContent(xTextFrame.getText().getStart(), UnoRuntime.queryInterface(XTextContent,firstWordField), false);
	xTextFrame.getText().insertTextContent(xTextFrame.getText().getEnd(), UnoRuntime.queryInterface(XTextContent,lastWordField), false);
//    xTextFrame.getText().getEnd().setString(firstWord + " " + lastWord);

	prevPage = page;
	xPageCursor.jumpToNextPage();
	page = xPageCursor.getPage();
} while (prevPage < page);

// set header styles
xStyleFamiliesSupplier = UnoRuntime.queryInterface(XStyleFamiliesSupplier, oDoc);
familyNames = xStyleFamiliesSupplier.getStyleFamilies();
pageStyles = familyNames.getByName("PageStyles");
pageStyleNames = UnoRuntime.queryInterface(XNameAccess, pageStyles);
//props = pageProps.getPropertySetInfo().getProperties();

if (pageStyleNames.hasByName("Standard"))
{
	pageStyle = pageStyleNames.getByName("Standard");
	pageProps = UnoRuntime.queryInterface(XPropertySet, pageStyle);
	headerText = pageProps.getPropertyValue("HeaderText");
	xEnumAccess = UnoRuntime.queryInterface(XEnumerationAccess, headerText);
	if (xEnumAccess != null)
	{
		headerEnum = xEnumAccess.createEnumeration();
		while (headerEnum.hasMoreElements())
		{
			headerPara = headerEnum.nextElement();
			portionEnum = UnoRuntime.queryInterface(XEnumerationAccess, headerPara).createEnumeration();
			while (portionEnum.hasMoreElements())
			{
				portion = portionEnum.nextElement();
				portionProps = UnoRuntime.queryInterface(XPropertySet, portion);
				textPortionType = portionProps.getPropertyValue("TextPortionType");

				if (portionProps.getPropertyValue("TextPortionType") == "TextField")
				{
					field = portionProps.getPropertyValue("TextField");
					fieldProps = UnoRuntime.queryInterface(XPropertySet, field);
					if (fieldProps.getPropertySetInfo().hasPropertyByName("Content"))
					{
						portionProps.setPropertyValue("CharStyleName", "header-word " + lexicalUnitLang);
					}
//                dumpProperties(fieldProps);
				}
			}
		}
	}
}

// return to original position
xPageCursor.jumpToPage(origPage);
}

updateDictHeaders(oDoc, xController);

/*
names = pageStyleNames.getElementNames();
dump = "";
for (i = 0; i < names.length; i++)
{
	dump += names[i];
}
xText.getEnd().setString(dump);
*/
// the refresh listener doesn't seem to do anything at the moment!
refreshListener = new XRefreshListener () {
	refreshed : function(event)
	{
		xTextDoc = event.Source.queryInterface(XTextDocument);
		updateDictHeaders(xTextDoc, xController);
	}
};
xRefreshable = UnoRuntime.queryInterface(XRefreshable, oDoc);
xRefreshable.addRefreshListener(refreshListener);

/*
xDocEventListener = new XDocumentEventListener() {
	documentEventOccured : function(event)
	{
		updateDictHeaders(event.Source, event.ViewController);
	}
};
xDocEventBroadcaster = UnoRuntime.queryInterface(XDocumentEventBroadcaster, oDoc);
xDocEventBroadcaster.addDocumentEventListener(xDocEventListener);
*/
