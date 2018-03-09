using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Palaso.Data;
using Palaso.DictionaryServices.Model;
using Palaso.Progress;
using WeSay.Project;
using WeSay.Project.Tests;
using Palaso.Text;

namespace Lift2Json
{
	class Program
	{
		static int Main(string[] args)
		{
			var options = new Options();
			var isValid = CommandLine.Parser.Default.ParseArgumentsStrict(args, options);
			if (isValid)
			{
				if (options.ShowHelp)
				{
					Console.WriteLine(options.GetUsage());
					return 0;
				}
				if (!File.Exists(options.InputFile))
				{
					Console.WriteLine("Input lift wordlist {0} does not exist", options.InputFile);
					return 1;
				}

				if ((options.OutputFile != "Wordlist.json") && File.Exists(options.OutputFile))
				{
					Console.WriteLine("The file {0} already exists.", options.OutputFile);
					return 1;
				}
				if (options.Verbose)
				{
					Console.WriteLine("Input file: {0}", options.InputFile);
					Console.WriteLine("Output file: {0}", options.OutputFile);
				}

			}
			else
			{
				// Display the default usage information
				Console.WriteLine("command line parsing failed");
				Console.WriteLine(options.GetUsage());
				return 1;
			}

			List<LexEntry> _words;
			_words = new List<LexEntry>();

			using (ProjectDirectorySetupForTesting p = new ProjectDirectorySetupForTesting("<entry id='foo1'><lexical-unit><form lang='qaa-x-qaa'><text>fooOne</text></form></lexical-unit></entry>"))
			{
				WeSayWordsProject project = p.CreateLoadedProject();

				using (var reader = new Palaso.DictionaryServices.Lift.LiftReader(new NullProgressState(),
					WeSayWordsProject.Project.GetSemanticDomainsList(),
					WeSayWordsProject.Project.GetIdsOfSingleOptionFields()))
				using (var m = new MemoryDataMapper<LexEntry>())
				{
					reader.Read(options.InputFile, m);
					_words.AddRange(from RepositoryId repositoryId in m.GetAllItems() select m.GetItem(repositoryId));
				}
				foreach (var word in _words)
				{
					foreach (var sense in word.Senses)
					{
						// copy all definition forms to gloss then delete definition form
						foreach (var form in sense.Definition.Forms)
						{
							sense.Gloss.SetAlternative(form.WritingSystemId, form.Form);
							sense.Definition.SetAlternative(form.WritingSystemId, null);
						}
					}
				}

				using (StreamWriter file = new StreamWriter(options.OutputFile))
				{
					using (JsonWriter writer = new JsonTextWriter(file))
					{
						writer.WriteStartArray();
						foreach (LexEntry word in _words)
						{
							writer.WriteStartObject();

							writer.WritePropertyName("lexicalid");
							LanguageForm idform = word.LexicalForm.Find("en");
							string lexicalid = (idform == null ? word.LexicalForm.GetFirstAlternative() : idform.Form);
							writer.WriteValue(lexicalid);

							foreach (var sense in word.Senses)
							{
								foreach (var form in sense.Gloss.Forms)
								{
									writer.WritePropertyName(form.WritingSystemId);
									writer.WriteValue(form.Form);
								}
							}

							writer.WriteEndObject();
						}
						writer.WriteEndArray();
					}
				}
			}
			return 0;
		}
	}
}
