using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace GhSA.Components.Ribbon
{
	/// <summary>
	/// Class containing the ribbon tab display name
	/// 
	/// Call this class from all components in plugin for naming consistency
	/// </summary>
	public class CategoryName
	{
		public static string name()
		{
			return "GSA";
		}
	}

	/// <summary>
	/// Class containing ribbon category names
	/// 
	/// Call this class from all components to pick which category component should sit in
	/// 
	/// Sorting of categories in the ribbon is controlled with a number of spaces in front of the name
	/// to avoid naming each category with a number in front. Spaces will automatically be removed when displayed
	/// </summary>
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
