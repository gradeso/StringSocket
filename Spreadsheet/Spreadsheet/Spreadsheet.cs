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
		private CellDS cds;
		private string validPattern;
		

		//dont forget to add the other constructors.



		public Spreadsheet()
		{
			cds = new CellDS();
			validPattern = "[a-zA-Z][0-9a-zA-Z]*";
			
		}

		/// <summary>
		/// represents if the ss has been changed since last save, 
		
		/// </summary>
		public override bool Changed
		{
			get
			{
				return cds.unsavedChanges;
			}

			protected set
			{
				Changed = value;
			}
		}

		/// <summary>
		/// gets the contents of the cell
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object GetCellContents(string name)
		{
			Cell toReturn = new Cell();
			cds.getCellWithName(name, ref toReturn);
			return toReturn.contents;

		}

		protected override IEnumerable<string> GetDirectDependents(string name)
		{
			
			if (!checkIfValidNameAndNormalize(ref name))
			{
				throw new InvalidNameException();
			}
			return cds.getDependencyGraph().GetDependents(name).AsEnumerable();


		}


		public override IEnumerable<string> GetNamesOfAllNonemptyCells()
		{
			return Solver.SummonNames(cds);
		}

		
		public override ISet<string> SetCellContents(string name, double number)
		{
			throw new NotImplementedException();
		}


		public override ISet<string> SetCellContents(string name, Formula formula)
		{
			if (!checkIfValidName(name))
			{
				throw new InvalidNameException();
			}
			
			




		}

		public override ISet<string> SetCellContents(string name, string text)
		{
			if (!checkIfValidName(name))
			{
				throw new InvalidNameException();
			}

			return updateDependencies(name, text);

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
		private bool checkIfValidNameAndNormalize(ref string name)
		{
			//check null.
			if (name == null) return false;
			name = Solver.Normalize(name);

			//check if regex agrees.
			return (Regex.IsMatch(name, "[a-zA-Z][0-9a-zA-Z]*") && Regex.IsMatch(name, validPattern));
		}
	}
	struct Cell
	{
		private const string unevaluatedFlag = "";

		public string name;
		public object contents;
		public object value;
		public Cell(string namein, object contentsin)
		{
			name = namein;
			contents = contentsin;
			value = unevaluatedFlag;
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
		internal bool unsavedChanges;

		public CellDS()
		{
			cells = new SortedSet<Cell>(new CellComparer());
			deeptree = new DependencyGraph();
			unsavedChanges = false;
		}
		/// <summary>
		/// adds a cell or replaces the contents of a cell and then recalculates dependency graph.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="newContents"></param>
		public HashSet<string> setContentsOrAddCell(string name, object newContents)
		{
			Cell cellOfInterest = new Cell(); 

			
			//if cell doesent exist create it,
			if (!getCellWithName(name, ref cellOfInterest))
			{
				
				//if were here we need to add the cell.
				cellOfInterest.name = name;
				cellOfInterest.contents = newContents;
			}

			else if (newContents == cellOfInterest.contents)
			{
				return new HashSet<string>();
			}
			//if PC gets here we have made a change.
			unsavedChanges = true;

			//once we have it initialized we interpret contents
			
			cellOfInterest.contents = newContents;
			
			

			
			//then we operate on contents adding/removing to/from the dependency tree

			//return the nested dependencies in a set



		}
		public bool getCellWithName(string name, ref Cell output)
		{
			Cell reference = new Cell(name, 0);
			foreach (Cell c in cells) {
				if (cells.Comparer.Compare(c, reference) == 0)
				{
					output = c;
					return true;
				}
				
			}
			output = new Cell();
			return false;
		}
		public DependencyGraph getDependencyGraph()
		{
			return deeptree;
		}
		//solves the formula and returns the value it evaluated to.
		public object solve(Cell firstCell)
		{
			//access the cell for maipulation
			var cellOfInterest = new Cell();
		
			getCellWithName(name, ref cellOfInterest);
			//
			if (cellOfInterest.value is string)
				{
					return cellOfInterest.value;

				}

			if (cellOfInterest.value is double)
				{
					return cellOfInterest.value;

				}

			if (cellOfInterest.value is Formula) {

					var luf = cells.ToLookup(cel => SolveFormula(cel.value), cel => cel.name);
					foreach (IGrouping<> in luf[])
					{

					}
					return ((Formula)cellOfInterest).contents.Evaluate(luf);
				}
			else {

			}
		}
		
		private double recursiveSolve()
		{
		}
		///helps the get cell value method
		public object returnValueOfFormula(Cell cellOfInterest)
		{
			return cellOfInterest.value;
		}

		
	}
	static class Solver
	{



		private static Normalizer defaultNormailzer = (s => s.ToUpper());


		public static IEnumerable<string> SummonNames(CellDS cds)
		{
			return (cds.cells).Select(c => c.name);
		}
		public static string Normalize(string toBeNormalized)
		{
			return Normalize(toBeNormalized, defaultNormailzer);
		}
		public static string Normalize(string toBeNormalized, Normalizer n)
		{
			return n(toBeNormalized);
		}

		public static ISet<string> NormalizeAndRetrieveVariables(object contents)
		{
			ISet<string> whatThisDependsOn;
			if (contents is string)
			{
				whatThisDependsOn = new HashSet<string>();

			}
			else if (contents is double)
			{
				whatThisDependsOn = new HashSet<string>();

			}
			else if (contents is Formula)
			{
				whatThisDependsOn = ((Formula)contents).GetVariables();

			}
			else
			{
				whatThisDependsOn = new HashSet<string>();

			}
			return whatThisDependsOn;
		}


	}
}
	