using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Palaso.Reporting;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.Project;
using System.Linq;

namespace WeSay.LexicalTools.AddPictures
{
	public class AddPicturesTask : TaskBase
	{
		private readonly AddPicturesConfig _config;
		private readonly IFileLocator _fileLocator;
		private Control _view;
		private Dictionary<string, List<string>> _index;

		public AddPicturesTask( AddPicturesConfig config,
									LexEntryRepository lexEntryRepository,
								 TaskMemoryRepository taskMemoryRepository,
								IFileLocator fileLocator)
			: base(config, lexEntryRepository, taskMemoryRepository)
		{
			_config = config;
			_fileLocator = fileLocator;
			_index = new Dictionary<string, List<string>>();
		}

		public override void Activate()
		{
			LoadIndex();
			base.Activate();
		}




		private void LoadIndex()
		{
			string path = _fileLocator.LocateFile(_config.IndexFileName, "picture index");
			if(string.IsNullOrEmpty(path) || !File.Exists(path))
			{
				throw new ConfigurationException("Could not load picture index file.");
			}

		   using(var f =File.OpenText(path))
		   {
			   while(!f.EndOfStream)
			   {
				   var line = f.ReadLine();
				   var parts=line.Split(new char[] {'\t'});
				   Debug.Assert(parts.Length == 2);
				   if (parts.Length != 2)
					   continue;
				   var fileName = parts[0];
				   var keyString = parts[1].Trim(new char[] {' ', '"'});//some have quotes, some don't
				   var keys = keyString.SplitTrimmed(',');
				   foreach (var key in keys)
				   {
					  _index.GetOrCreate(key).Add(fileName);
				   }
			   }
		   }
		}

		public IList<string> GetMatchingPictures(string keywords)
		{
			return GetMatchingPictures(keywords.SplitTrimmed(' '));
		}

		private IList<string> GetMatchingPictures(IEnumerable<string> keywords)
		{
			List<string> pictures = new List<string>();
			foreach (var key in keywords)
			{
				List<string> picturesForThisKey = new List<string>();

				if(_index.TryGetValue(key, out picturesForThisKey))
				{
					pictures.AddRange(picturesForThisKey);
				}
			}
			return  new List<string>(pictures.Distinct());
		}

		public override Control Control
		{
			get
			{
				if (_view == null)
				{
					_view = new AddPicturesControl(this);
				}
				return _view;
			}
		}

		protected override int ComputeCount(bool returnResultEvenIfExpensive)
		{
			return 0;
		}

		protected override int ComputeReferenceCount()
		{
			return 0;
		}
	}
}
