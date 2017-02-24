using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS
{

	public class Spreadsheet : AbstractSpreadsheet
	{
		private Dictionary<string, Cell> cells;
		private DependencyGraph deeptree;
		private int lastStateHash;
		//if the hash code changes the object changes.

		public Spreadsheet()
		{
			cells = new Dictionary<string, Cell>();
			deeptree = new DependencyGraph();
		}

		public override bool Changed
		{
			get
			{
				throw new NotImplementedException();
			}

			protected set
			{
				throw new NotImplementedException();
			}
		}

		//helper method for changed
		private void SetHash()
		{
			lastStateHash = this.GetHashCode();
		}
		/// <summary>
		/// gets the contents of the cell
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object GetCellContents(string name)
		{
			Cell temp;
			if (!cells.TryGetValue(name, out temp))
			{
				throw new InvalidNameException();
			}
			return temp.contents;

		}

		protected override IEnumerable<string> GetDirectDependents(string name)
		{
			if (name == null)
			{
				throw new InvalidNameException();

			}
			Cell temp;
			if (!cells.TryGetValue(name, out temp))
			{
				throw new InvalidNameException();
			}
			return deeptree.GetDependents(name);


		}
		private void calculate()
		{

		}
		public override IEnumerable<string> GetNamesOfAllNonemptyCells()
		{
			return cells.Keys.AsEnumerable();
		}
		public override ISet<string> SetCellContents(string name, double number)
		{

			Cell temp;
			if (!cells.TryGetValue(name, out temp))
			{
				throw new InvalidNameException();
			}
			temp.contents = number;
			var templist = new HashSet<string>();

			foreach (string s in GetNamesOfAllNonemptyCells())
			{
				if (temp.contents.ToString().Contains(s))
				{
					templist.Add(s);
				}
			}

			deeptree.ReplaceDependents(name, templist);
			templist.Add(name);

			return (ISet<string>)RecursiveGetDependencies(name, templist);

		}

		private HashSet<string> RecursiveGetDependencies(string name, HashSet<string> allDependants)
		{
			var toReturn = new HashSet<string>(allDependants);
			foreach (string s in allDependants)
			{
				foreach (string t in deeptree.GetDependents(s))
				{
					toReturn.Add(t);
				}
			}
			if (toReturn.Count == 0)
			{
				return toReturn;
			}
			else
			{
				return RecursiveGetDependencies(name, allDependants);
			}

		}
		public override ISet<string> SetCellContents(string name, Formula formula)
		{
			Cell temp;
			if (!cells.TryGetValue(name, out temp))
			{
				throw new InvalidNameException();
			}
			temp.contents = formula;
			var templist = new HashSet<string>();

			foreach (string s in GetNamesOfAllNonemptyCells())
			{
				if (temp.contents.ToString().Contains(s))
				{
					templist.Add(s);
				}
			}

			deeptree.ReplaceDependents(name, templist);
			templist.Add(name);

			return (ISet<string>)RecursiveGetDependencies(name, templist);

		}

		public override ISet<string> SetCellContents(string name, string text)
		{
			Cell temp;
			if (!cells.TryGetValue(name, out temp))
			{
				throw new InvalidNameException();
			}
			temp.contents = text;
			var templist = new HashSet<string>();

			foreach (string s in GetNamesOfAllNonemptyCells())
			{
				if (temp.contents.ToString().Contains(s))
				{
					templist.Add(s);
				}
			}

			deeptree.ReplaceDependents(name, templist);
			templist.Add(name);

			return (ISet<string>)RecursiveGetDependencies(name, templist);

		}

		public override void Save(TextWriter dest)
		{
			throw new NotImplementedException();
		}

		public override object GetCellValue(string name)
		{
			throw new NotImplementedException();
		}

		public override System.Collections.Generic.ISet<string> SetContentsOfCell(string name, string content)
		{
			throw new NotImplementedException();
		}
	}
	struct Cell
	{
		public string name;
		public object contents;
		public object value;
		public Cell(string namein, object contentsin)
		{
			name = namein;
			contents = contentsin;
			if (contentsin is string || contentsin is double)
			{
				value = contentsin;
			}
			else if (contentsin is Formula)
			{
				value = "";
			}
			else
			{
				value = new FormulaError();

			}
		}
	}
}