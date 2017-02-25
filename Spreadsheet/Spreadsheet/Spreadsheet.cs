using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SS
{

	public class Spreadsheet : AbstractSpreadsheet
	{
		private SortedSet<Cell> cells;
		private DependencyGraph deeptree;
		private int lastStateHash;

		private string validPattern;

		//if the hash code changes the object changes.

		public Spreadsheet()
		{
			cells = new SortedSet<Cell>(new CellComparer());
			deeptree = new DependencyGraph();
			validPattern = "[a-zA-Z][0-9a-zA-Z]*";
		}

		/// <summary>
		/// represents if the ss has been changed since last save, 
		/// get does work for checking if changed 
		/// </summary>
		public override bool Changed
		{
			get
			{
				return (cells.GetHashCode() != lastStateHash || Changed);
			}

			protected set
			{
				Changed = value;
			}
		}

		//helper method for changed
		private void SetHash()
		{
			lastStateHash = cells.GetHashCode();
		}
		/// <summary>
		/// gets the contents of the cell
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object GetCellContents(string name)
		{
			throw new NotImplementedException();

		}

		protected override IEnumerable<string> GetDirectDependents(string name)
		{
			if (!checkIfValidName(name))
			{
				throw new InvalidNameException();
			}
			return deeptree.GetDependents(name).AsEnumerable();


		}


		public override IEnumerable<string> GetNamesOfAllNonemptyCells()
		{
			LinkedList<string> toReturn = new LinkedList<string>();

			foreach (Cell c in cells)
			{
				toReturn.AddLast(c.name);
			}
			return toReturn.AsEnumerable();

		}

		private LinkedList<string> getNestedDependencies(string current, LinkedList<string> allDependants, int offset)
		{

			bool changed = false;
			string last = current;
			do
			{
				foreach (string s in deeptree.GetDependents(current))
				{
					//set up for the next iterationa
					allDependants.AddLast(s);
					if (!changed) current = s;
					changed = true;

				}
			} while (changed);
			if (current == last)
			{
				return allDependants;
			}
			else
			{
				return getNestedDependencies(allDependants.ElementAt(allDependants.Count + --offset), allDependants, offset);
			}

		}
		public override ISet<string> SetCellContents(string name, double number)
		{

		}


		public override ISet<string> SetCellContents(string name, Formula formula)
		{
			if (!checkIfValidName(name))
			{
				throw new InvalidNameException();
			}

			return updateDependencies(name, formula);




		}

		public override ISet<string> SetCellContents(string name, string text)
		{
			if (!checkIfValidName(name))
			{
				throw new InvalidNameException();
			}

			return updateDependencies(name, text);

		}

		private ISet<string> updateDependencies(string name, object contents)
		{
			if (contents is string)
			{
			}
			else if (contents is double)
			{
			}
		}

		//saves the state of the spreadsheet to XML format, 
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
		private bool checkIfValidName(string name)
		{
			//check null.
			if (name == null) return false;
			//check if regex agrees.
			return (Regex.IsMatch(name, "[a-zA-Z][0-9a-zA-Z]*") && Regex.IsMatch(name, validPattern));
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
			value = "";
		}
	}
	class CellComparer : IComparer<Cell>
	{
		public int Compare(Cell x, Cell y)
		{
			return x.name.CompareTo(y.name);
		}
		
	}
	class CellDS
	{

		public SortedSet<Cell> cells
		{
			get
			{
				return cells; 
			}
			private set
			{
				cells = value;
			}
		}
		private DependencyGraph deeptree;
		

		public CellDS()
		{
			cells = new SortedSet<Cell>(new CellComparer());
			deeptree = new DependencyGraph();
		}
		/// <summary>
		/// adds a cell or replaces the contents of a cell and then recalculates dependency graph.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="contents"></param>
		public HashSet<string> setContentsOrAddCell(string name, object contents)
		{
			//if cell doesent exist create it,
			if (getCellWithName(name).Equals(new Cell()))
			{

			}
			//once we have it initialized we interpret contents

			//then we operate on contents adding to the dependency tree

			//get out nested dependencies

		}
		public Cell getCellWithName(string name)
		{
			Cell reference = new Cell(name, 0);
			foreach (Cell c in cells) {
				if (cells.Comparer.Compare(c, reference) == 0)
				{
					return c;
				}
				
			}
			return new Cell();
		}
		public DependencyGraph getDependencyGraph()
		{
			return deeptree;
		}
	}
}
	