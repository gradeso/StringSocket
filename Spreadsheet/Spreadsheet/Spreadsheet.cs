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
			
			if (name == null || !Regex.IsMatch(name, validPattern))
			{
				throw new InvalidNameException();
			}

			
			cells
			
		}

		private LinkedList<string> getNestedDependencies(string current, LinkedList<string> allDependants)
		{
		
			bool changed = false;
			string last = current;
			do
			{
				foreach (string s in deeptree.GetDependents(current))
				{
					//set up for the next iterationa
					allDependants.AddLast(s);
					if(!changed) current = s;
					changed = true;
					
				}
			} while (changed);
			if (current == last)
			{
				return allDependants;
			}
			else
			{
				return getNestedDependencies(allDependants.ElementAt(allDependants.Count - 2), allDependants);
			}
				
			
			
			

		}
		public override ISet<string> SetCellContents(string name, Formula formula)
		{
			

		}

		public override ISet<string> SetCellContents(string name, string text)
		{
			

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
}