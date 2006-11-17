using System;

namespace MeasureDb4o
{
	class Program
	{
		static void Main(string[] args)
		{
			QueryTests qt = new QueryTests();
			qt.SimpleStringSearch();
			Console.ReadLine();
		}
	}
}
