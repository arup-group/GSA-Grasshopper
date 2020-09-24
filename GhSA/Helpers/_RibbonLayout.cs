using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace GhSA.Components.Ribbon
{
	public class CategoryName
	{
		public static string name()
		{
			return "GSA";
		}
	}

	internal class SubCategoryName
	{
		public static string cat0()
		{
			return new string(' ', 10) + "Model";
		}

		public static string cat1()
		{
			return new string(' ', 9) + "Properties";
		}

		public static string cat2()
		{
			return new string(' ', 8) + "Geometry";
		}

		public static string cat3()
		{
			return new string(' ', 7) + "Loads";
		}

		public static string cat4()
		{
			return new string(' ', 6) + "Analyse";
		}

		public static string cat5()
		{
			return new string(' ', 5) + "Results";
		}

		public static string cat6()
		{
			return "";
		}

		public static string cat7()
		{
			return "";
		}

		public static string cat8()
		{
			return "test";
		}

		public static string cat9()
		{
			return "Parameters";
		}
	}
}
