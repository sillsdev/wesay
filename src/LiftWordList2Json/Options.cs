// Copyright (c) 2018 SIL International
// This software is licensed under the MIT License (http://opensource.org/licenses/MIT)

using CommandLine;
using CommandLine.Text;

namespace Lift2Json
{
	class Options
	{
		[Option('i',
			"input",
			HelpText =
				"Wordlist in lift format with id and glosses in different source languages.")]
		public string InputFile { get; set; } = "Wordlist.lift";

		[Option('o',
			"output",
			HelpText = "Output file to write.")]
		public string OutputFile { get; set; } = "Wordlist.json";

		[Option('v', "verbose", HelpText = "Verbose output.")]
		public bool Verbose { get; set; } = false;

		[Option('h', null, HelpText = "Display this help screen.")]
		public bool ShowHelp { get; set; }


		public string GetUsage()
		{
			var help = new HelpText
			{
				Heading = new HeadingInfo("Lift2Json", "0.1"),
				Copyright = new CopyrightInfo("SIL International", 2018),
				AdditionalNewLineAfterOption = false,
				AddDashesToOption = true
			};
			//help.AddPreOptionsLine("<<license details here.>>");
			//help.AddPreOptionsLine("Usage: app -p Someone");
			help.AddPreOptionsLine("Lift2Json will convert a lift format wordlist to json.");
			// help.AddOptions(this);
			//var usage = new StringBuilder();
			//usage.AppendLine("LanguageData (c) 2016 SIL International");
			//usage.AppendLine("LanguageData will process Ethnologue, IANA subtag and ISO693-3 data to a single language data index file.");
			//Console.WriteLine("LastParseState: {0}", this.LastParserState?.ToString());
			/*if (this.LastParserState?.Errors.Any() == true)
			{
				Console.WriteLine ("have some cl parsing errors");
				var errors = help.RenderParsingErrorsText(this, 2); // indent with two spaces

				if (!string.IsNullOrEmpty (errors)) {
					Console.WriteLine ("error list is not empty");
					help.AddPreOptionsLine (string.Concat (Environment.NewLine, "ERROR(S):"));
					help.AddPreOptionsLine (errors);
				} else
					Console.WriteLine ("error list is empty");
			}
			*/
			return help;
		}
	}
}

