using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;
using WeSay.Project;

namespace WeSay.Project.Tests
{
	[TestFixture]
	public class ChorusBackupMakerTests
	{
		[Test]
		public void SerialzeAndDeserialize()
		{
			ChorusBackupMaker b = new ChorusBackupMaker();
			b.PathToParentOfRepositories = @"z:\";
			StringBuilder builder = new StringBuilder();
			using (XmlWriter writer = XmlWriter.Create(builder))
			{
				b.Save(writer);
				using (XmlReader reader = XmlReader.Create(new StringReader(builder.ToString())))
				{
					ChorusBackupMaker loadedGuy = ChorusBackupMaker.LoadFromReader(reader);
					Assert.AreEqual(@"z:\", loadedGuy.PathToParentOfRepositories);
				}

			}
		}
	}

}
