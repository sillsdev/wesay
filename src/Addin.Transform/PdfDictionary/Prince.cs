using System;
using System.Diagnostics;
using System.IO;
using System.Text;

public interface IPrince
{
	void SetEncryptInfo(int keyBits, string userPassword, string ownerPassword, bool disallowPrint, bool disallowModify, bool disallowCopy, bool disallowAnnotate);
	void AddStyleSheet(string cssPath);
	void ClearStyleSheets();
	void AddScript(string jsPath);
	void ClearScripts();
	void AddFileAttachment(string filePath);
	void ClearFileAttachments();
	void SetLicenseFile(string file);
	void SetLicenseKey(string key);
	void SetInputType(string inputType);
	void SetHTML(bool html);
	void SetJavaScript(bool js);
	void SetHttpUser(string user);
	void SetHttpPassword(string password);
	void SetHttpProxy(string proxy);
	void SetInsecure(bool insecure);
	void SetLog(string logFile);
	void SetBaseURL(string baseurl);
	void SetFileRoot(string fileroot);
	void SetXInclude(bool xinclude);
	void SetEmbedFonts(bool embed);
	void SetSubsetFonts(bool embedSubset);
	void SetCompress(bool compress);
	void SetEncrypt(bool encrypt);
	bool Convert(string xmlPath);
	bool Convert(string xmlPath, string pdfPath);
	bool Convert(string xmlPath, Stream pdfOutput);
	bool Convert(Stream xmlInput, string pdfPath);
	bool Convert(Stream xmlInput, Stream pdfOutput);
	bool ConvertMemoryStream(MemoryStream xmlInput, Stream pdfOutput);
	bool ConvertString(string xmlInput, Stream pdfOutput);
	bool ConvertMultiple(string[] xmlPaths, string pdfPath);

}

public class PrinceFilter : System.IO.Stream
{

	private IPrince prince;
	private Stream oldFilter;

	private MemoryStream memStream;
	public PrinceFilter(IPrince prince, Stream oldFilter)
	{
		this.prince = prince;
		this.oldFilter = oldFilter;
		this.memStream = new MemoryStream();
	}

	public override bool CanSeek {
		get { return false; }
	}

	public override bool CanWrite {
		get { return true; }
	}

	public override bool CanRead {
		get { return false; }
	}

	public override long Position {
		get { return 0; }
			// do nothing
		set { }
	}

	public override long Length {
		get { return 0; }
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		return 0;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return 0;
	}

	public override void SetLength(long value)
	{
		// do nothing
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		memStream.Write(buffer, offset, count);
	}

	public override void Flush()
	{
		// FIXME?
	}

	public override void Close()
	{
		prince.ConvertMemoryStream(memStream, oldFilter);
		oldFilter.Close();
	}

}

public class Prince : IPrince
{

	private string mPrincePath;
	private string mStyleSheets;
	private string mJavaScripts;
	private string mFileAttachments;
	private string mLicenseFile;
	private string mLicenseKey;
	private string mInputType;
	private bool mJavaScript;
	private string mHttpUser;
	private string mHttpPassword;
	private string mHttpProxy;
	private bool mInsecure;
	private string mLog;
	private string mBaseURL;
	private string mFileRoot;
	private bool mXInclude;
	private bool mEmbedFonts;
	private bool mSubsetFonts;
	private bool mCompress;
	private bool mEncrypt;
	private string mEncryptInfo;
	public Prince()
	{
		this.mPrincePath = "";
		this.mStyleSheets = "";
		this.mJavaScripts = "";
		this.mFileAttachments = "";
		this.mLicenseFile = "";
		this.mLicenseKey = "";
		this.mInputType = "auto";
		this.mJavaScript = false;
		this.mHttpUser = "";
		this.mHttpPassword = "";
		this.mHttpProxy = "";
		this.mInsecure = false;
		this.mLog = "";
		this.mBaseURL = "";
		this.mFileRoot = "";
		this.mXInclude = true;
		this.mEmbedFonts = true;
		this.mSubsetFonts = true;
		this.mCompress = true;
		this.mEncrypt = false;
		this.mEncryptInfo = "";
	}
	public Prince(string princePath)
	{
		this.mPrincePath = princePath;
		this.mStyleSheets = "";
		this.mJavaScripts = "";
		this.mFileAttachments = "";
		this.mLicenseFile = "";
		this.mLicenseKey = "";
		this.mInputType = "auto";
		this.mJavaScript = false;
		this.mHttpUser = "";
		this.mHttpPassword = "";
		this.mHttpProxy = "";
		this.mInsecure = false;
		this.mLog = "";
		this.mBaseURL = "";
		this.mFileRoot = "";
		this.mXInclude = true;
		this.mEmbedFonts = true;
		this.mSubsetFonts = true;
		this.mCompress = true;
		this.mEncrypt = false;
		this.mEncryptInfo = "";
	}
	public void SetLicenseFile(string file)
	{
		mLicenseFile = file;
	}
	public void SetLicenseKey(string key)
	{
		mLicenseKey = key;
	}
	public void SetHTML(bool html)
	{
		if (html) {
			mInputType = "html";
		} else {
			mInputType = "xml";
		}
	}
	public void SetInputType(string inputType)
	{
		mInputType = inputType;
	}
	public void SetJavaScript(bool js)
	{
		mJavaScript = js;
	}
	public void SetHttpUser(string user)
	{
		mHttpUser = user;
	}
	public void SetHttpPassword(string password)
	{
		mHttpPassword = password;
	}
	public void SetHttpProxy(string proxy)
	{
		mHttpProxy = proxy;
	}
	public void SetInsecure(bool insecure)
	{
		mInsecure = insecure;
	}
	public void SetLog(string logFile)
	{
		mLog = logFile;
	}
	public void SetBaseURL(string baseurl)
	{
		mBaseURL = baseurl;
	}
	public void SetFileRoot(string fileroot)
	{
		mFileRoot = fileroot;
	}
	public void SetXInclude(bool xinclude)
	{
		mXInclude = xinclude;
	}
	public void SetEmbedFonts(bool embed)
	{
		mEmbedFonts = embed;
	}
	public void SetSubsetFonts(bool subset)
	{
		mSubsetFonts = subset;
	}
	public void SetCompress(bool compress)
	{
		mCompress = compress;
	}
	public void SetEncrypt(bool encrypt)
	{
		mEncrypt = encrypt;
	}

	public void SetEncryptInfo(int keyBits, string userPassword, string ownerPassword, bool disallowPrint, bool disallowModify, bool disallowCopy, bool disallowAnnotate)
	{
		mEncrypt = true;

		if ((keyBits != 40) & (keyBits != 128)) {
			mEncryptInfo = "";
			throw new ApplicationException("Invalid value for keyBits: must be 40 or 128");
		} else {
			mEncryptInfo = "--encrypt " + " --key-bits " + keyBits + " --user-password=" + "\"" + cmdline_arg_escape_2(cmdline_arg_escape_1(userPassword)) + "\"" + " --owner-password=" + "\"" + cmdline_arg_escape_2(cmdline_arg_escape_1(ownerPassword)) + "\"" + " ";

			if (disallowPrint) {
				mEncryptInfo = mEncryptInfo + "--disallow-print ";
			}

			if (disallowModify) {
				mEncryptInfo = mEncryptInfo + "--disallow-modify ";
			}

			if (disallowCopy) {
				mEncryptInfo = mEncryptInfo + "--disallow-copy ";
			}

			if (disallowAnnotate) {
				mEncryptInfo = mEncryptInfo + "--disallow-annotate ";
			}
		}
	}
	public void AddStyleSheet(string cssPath)
	{
		mStyleSheets = mStyleSheets + "-s " + "\"" + cssPath + "\"" + " ";
	}
	public void ClearStyleSheets()
	{
		mStyleSheets = "";
	}
	public void AddScript(string jsPath)
	{
		mJavaScripts = mJavaScripts + "--script " + "\"" + jsPath + "\"" + " ";
	}
	public void ClearScripts()
	{
		mJavaScripts = "";
	}
	public void AddFileAttachment(string filePath)
	{
		mFileAttachments = mFileAttachments + "--attach=\"" + "\"" + filePath + "\"" + " ";
	}
	public void ClearFileAttachments()
	{
		mFileAttachments = "";
	}
	private string getArgs()
	{
		string args = null;

		args = "--server " + mStyleSheets + mJavaScripts + mFileAttachments;

		if (mEncrypt) {
			args = args + mEncryptInfo;
		}


		if (mInputType == "auto") {
		} else {
			args = args + "-i " + "\"" + mInputType + "\"" + " ";
		}

		if (mJavaScript) {
			args = args + "--javascript ";
		}

		if (!string.IsNullOrEmpty(mHttpUser)) {
			args = args + "--http-user=\"" + cmdline_arg_escape_2(cmdline_arg_escape_1(mHttpUser)) + "\" ";
		}

		if (!string.IsNullOrEmpty(mHttpPassword)) {
			args = args + "--http-password=\"" + cmdline_arg_escape_2(cmdline_arg_escape_1(mHttpPassword)) + "\" ";
		}

		if (!string.IsNullOrEmpty(mHttpProxy)) {
			args = args + "--http-proxy=\"" + mHttpProxy + "\" ";
		}

		if (mInsecure) {
			args = args + "--insecure ";
		}

		if (!string.IsNullOrEmpty(mLog)) {
			args = args + "--log=\"" + mLog + "\" ";
		}

		if (!string.IsNullOrEmpty(mBaseURL)) {
			args = args + "--baseurl=\"" + mBaseURL + "\" ";
		}

		if (!string.IsNullOrEmpty(mFileRoot)) {
			args = args + "--fileroot=\"" + mFileRoot + "\" ";
		}

		if (!string.IsNullOrEmpty(mLicenseFile)) {
			args = args + "--license-file=\"" + mLicenseFile + "\" ";
		}

		if (!string.IsNullOrEmpty(mLicenseKey)) {
			args = args + "--license-key=\"" + mLicenseKey + "\" ";
		}

		if (!mXInclude) {
			args = args + "--no-xinclude ";
		}

		if (!mEmbedFonts) {
			args = args + "--no-embed-fonts ";
		}

		if (!mSubsetFonts) {
			args = args + "--no-subset-fonts ";
		}

		if (!mCompress) {
			args = args + "--no-compress ";
		}

		return args;
	}
	public bool Convert(string xmlPath)
	{
		string args = null;

		args = getArgs() + "\"" + xmlPath + "\"";

		return Convert1(args);
	}
	public bool Convert(string xmlPath, string pdfPath)
	{
		string args = null;

		args = getArgs() + "\"" + xmlPath + "\"" + " -o " + "\"" + pdfPath + "\"";

		return Convert1(args);
	}
	public bool ConvertMultiple(string[] xmlPaths, string pdfPath)
	{
		string args = null;
		string doc = null;
		string docPaths = null;

		docPaths = "";
		foreach (string doc_loopVariable in xmlPaths) {
			doc = doc_loopVariable;
			docPaths = docPaths + "\"" + doc + "\"" + " ";
		}

		args = getArgs() + docPaths + " -o " + "\"" + pdfPath + "\"";

		return Convert1(args);
	}
	public bool Convert(string xmlPath, Stream pdfOutput)
	{

		byte[] buf = new byte[4097];
		int bytesRead = 0;
		Process prs = new Process();
		string args = null;

		if (!pdfOutput.CanWrite) {
			throw new ApplicationException("The pdfOutput stream is not writable");
		} else {
			args = getArgs() + "--silent " + "\"" + xmlPath + "\" -o -";
			prs = StartPrince(args);

			prs.StandardInput.Close();

			bytesRead = prs.StandardOutput.BaseStream.Read(buf, 0, 4096);
			while (bytesRead != 0) {
				pdfOutput.Write(buf, 0, bytesRead);
				bytesRead = prs.StandardOutput.BaseStream.Read(buf, 0, 4096);
			}
			prs.StandardOutput.Close();

			if (ReadMessages(prs) == "success") {
				return true;
			} else {
				return false;
			}
		}
	}
	public bool Convert(Stream xmlInput, string pdfPath)
	{

		byte[] buf = new byte[4097];
		int bytesRead = 0;
		Process prs = new Process();
		string args = null;

		if (!xmlInput.CanRead) {
			throw new ApplicationException("The xmlInput stream is not readable");
		} else {
			args = getArgs() + "--silent - -o \"" + pdfPath + "\"";
			prs = StartPrince(args);

			bytesRead = xmlInput.Read(buf, 0, 4096);
			while (bytesRead != 0) {
				prs.StandardInput.BaseStream.Write(buf, 0, bytesRead);
				bytesRead = xmlInput.Read(buf, 0, 4096);
			}
			prs.StandardInput.Close();

			prs.StandardOutput.Close();

			if (ReadMessages(prs) == "success") {
				return true;
			} else {
				return false;
			}
		}
	}
	public bool Convert(Stream xmlInput, Stream pdfOutput)
	{

		byte[] buf = new byte[4097];
		int bytesRead = 0;
		Process prs = new Process();
		string args = null;

		if (!xmlInput.CanRead) {
			throw new ApplicationException("The xmlInput stream is not readable");
		} else if (!pdfOutput.CanWrite) {
			throw new ApplicationException("The pdfOutput stream is not writable");
		} else {
			args = getArgs() + "--silent -";
			prs = StartPrince(args);

			bytesRead = xmlInput.Read(buf, 0, 4096);
			while (bytesRead != 0) {
				prs.StandardInput.BaseStream.Write(buf, 0, bytesRead);
				bytesRead = xmlInput.Read(buf, 0, 4096);
			}
			prs.StandardInput.Close();

			bytesRead = prs.StandardOutput.BaseStream.Read(buf, 0, 4096);
			while (bytesRead != 0) {
				pdfOutput.Write(buf, 0, bytesRead);
				bytesRead = prs.StandardOutput.BaseStream.Read(buf, 0, 4096);
			}
			prs.StandardOutput.Close();

			if (ReadMessages(prs) == "success") {
				return true;
			} else {
				return false;
			}
		}
	}
	public bool ConvertMemoryStream(MemoryStream xmlInput, Stream pdfOutput)
	{

		byte[] buf = new byte[4097];
		int bytesRead = 0;
		Process prs = new Process();
		string args = null;

		if (!pdfOutput.CanWrite) {
			throw new ApplicationException("The pdfOutput stream is not writable");
		} else {
			args = getArgs() + "--silent -";
			prs = StartPrince(args);

			xmlInput.WriteTo(prs.StandardInput.BaseStream);
			prs.StandardInput.Close();

			bytesRead = prs.StandardOutput.BaseStream.Read(buf, 0, 4096);
			while (bytesRead != 0) {
				pdfOutput.Write(buf, 0, bytesRead);
				bytesRead = prs.StandardOutput.BaseStream.Read(buf, 0, 4096);
			}
			prs.StandardOutput.Close();

			if (ReadMessages(prs) == "success") {
				return true;
			} else {
				return false;
			}
		}
	}
	public bool ConvertString(string xmlInput, Stream pdfOutput)
	{

		byte[] buf = new byte[4097];
		int bytesRead = 0;
		Process prs = new Process();
		string args = null;

		if (!pdfOutput.CanWrite) {
			throw new ApplicationException("The pdfOutput stream is not writable");
		} else {
			args = getArgs() + "--silent -";
			prs = StartPrince(args);

			UTF8Encoding enc = new UTF8Encoding();
			byte[] stringBytes = enc.GetBytes(xmlInput);
			prs.StandardInput.BaseStream.Write(stringBytes, 0, stringBytes.Length);
			prs.StandardInput.Close();

			bytesRead = prs.StandardOutput.BaseStream.Read(buf, 0, 4096);
			while (bytesRead != 0) {
				pdfOutput.Write(buf, 0, bytesRead);
				bytesRead = prs.StandardOutput.BaseStream.Read(buf, 0, 4096);
			}
			prs.StandardOutput.Close();

			if (ReadMessages(prs) == "success") {
				return true;
			} else {
				return false;
			}
		}
	}

	private bool Convert1(string args)
	{
		Process pr = StartPrince(args);

		if ((pr != null)) {
			if (ReadMessages(pr) == "success") {
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}

	}
	private Process StartPrince(string args)
	{
		int ERROR_FILE_NOT_FOUND = 2;
		int ERROR_PATH_NOT_FOUND = 3;
		int ERROR_ACCESS_DENIED = 5;

		Process pr = new Process();

		pr.StartInfo.FileName = mPrincePath;
		pr.StartInfo.Arguments = args;
		pr.StartInfo.UseShellExecute = false;
		pr.StartInfo.CreateNoWindow = true;
		pr.StartInfo.RedirectStandardInput = true;
		pr.StartInfo.RedirectStandardOutput = true;
		pr.StartInfo.RedirectStandardError = true;

		try {
			pr.Start();

			if (!pr.HasExited) {
				return pr;
			} else {
				throw new ApplicationException("Error starting Prince: " + mPrincePath);
			}

		} catch (System.ComponentModel.Win32Exception ex) {
			string msg = null;
			msg = ex.Message;
			if (ex.NativeErrorCode == ERROR_FILE_NOT_FOUND) {
				msg = msg + " -- Please verify that Prince.exe is in the directory";
			} else if (ex.NativeErrorCode == ERROR_ACCESS_DENIED) {
				msg = msg + " -- Please check system permission to run Prince.";
			} else if (ex.NativeErrorCode == ERROR_PATH_NOT_FOUND) {
				msg = msg + " -- Please check Prince path.";
			} else {
				// just use ex.Message
			}

			throw new ApplicationException(msg);
		}
	}
	private string ReadMessages(Process prs)
	{
		StreamReader stdErrFromPr = prs.StandardError;
		string line = null;
		string result = null;

		line = "";
		result = "";
		line = stdErrFromPr.ReadLine();
		while ((line != null)) {
			if (line.Substring(0, 4) == "fin|") {
				result = line.Substring(4, (line.Length - 4));
			}
			line = stdErrFromPr.ReadLine();
		}
		stdErrFromPr.Close();
		return result;
	}
	private string cmdline_arg_escape_1(string arg)
	{
		int pos = 0;
		int numSlashes = 0;
		string rightSubstring = null;
		string leftSubstring = null;
		string middleSubstring = null;


		if (arg.Length == 0) {
			//return empty string
			return arg;

		} else {
			//chr(34) is character double quote ( " ), chr(92) is character backslash ( \ )

			for (pos = (arg.Length - 1); pos >= 0; pos += -1) {
				if (arg[pos] == '\"') {
					//if there is a double quote in the arg string
					//find number of backslashes preceding the double quote ( " )
					numSlashes = 0;
					while (((pos - 1 - numSlashes) >= 0)) {
						if (arg[pos - 1 - numSlashes] == '\\') {
							numSlashes += 1;
						} else {
							break; // TODO: might not be correct. Was : Exit Do
						}
					}

					rightSubstring = arg.Substring(pos + 1);
					leftSubstring = arg.Substring(0, (pos - numSlashes));

					middleSubstring = "\\";
					for (int i = 1; i <= numSlashes; i += 1) {
						middleSubstring = middleSubstring + "\\" + "\\";
					}

					middleSubstring = middleSubstring + "\"";

					return cmdline_arg_escape_1(leftSubstring) + middleSubstring + rightSubstring;

				}
			}

			//no double quote found, return string itself
			return arg;

		}
	}
	private string cmdline_arg_escape_2(string arg)
	{
		int pos = 0;
		int numEndingSlashes = 0;

		numEndingSlashes = 0;
		for (pos = (arg.Length - 1); pos >= 0; pos += -1) {
			if (arg[pos] == '\\') {
				numEndingSlashes += 1;
			} else {
				break; // TODO: might not be correct. Was : Exit For
			}
		}

		for (int i = 1; i <= numEndingSlashes; i += 1) {
			arg = arg + '\\';
		}

		return arg;

	}
}
