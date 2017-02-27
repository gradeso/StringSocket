using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SS
{

	/// <summary>
	/// this class represents an implementation of a spreadsheet.
	/// </summary>
	/// <seealso cref="SS.AbstractSpreadsheet" />
	public class Spreadsheet : AbstractSpreadsheet
	{
		private CellDS cds;
		private string validPattern;


		//dont forget to add the other constructors.



		/// <summary>
		/// Initializes a new instance of the <see cref="Spreadsheet"/> class.
		/// </summary>
		public Spreadsheet()
		{
			cds = new CellDS();
			validPattern = "[A-Za-z]([A-Za-z][1-9]|[1-9][0-9]|[1-9]$)[0-9]*";

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Spreadsheet"/> class.
		/// </summary>
		/// <param name="isValid">The is valid.</param>
		public Spreadsheet(Regex isValid)
		{
			cds = new CellDS();
			validPattern = isValid.ToString();

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
		/// Writes the contents of this spreadsheet to dest using an XML format.
		/// The XML elements should be structured as follows:
		/// <spreadsheet IsValid="IsValid regex goes here"><cell name="cell name goes here" contents="cell contents go here"></cell><cell name="cell name goes here" contents="cell contents go here"></cell><cell name="cell name goes here" contents="cell contents go here"></cell></spreadsheet>
		/// The value of the IsValid attribute should be IsValid.ToString()
		/// There should be one cell element for each non-empty cell in the spreadsheet.
		/// If the cell contains a string, the string (without surrounding double quotes) should be written as the contents.
		/// If the cell contains a double d, d.ToString() should be written as the contents.
		/// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
		/// If there are any problems writing to dest, the method should throw an IOException.
		/// </summary>
		/// <param name="dest"></param>
		/// <exception cref="System.NotImplementedException"></exception>
		public override void Save(TextWriter dest)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// If name is null or invalid, throws an InvalidNameException.
		/// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
		/// value should be either a string, a double, or a FormulaError.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public override object GetCellValue(string name)
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// If content is null, throws an ArgumentNullException.
		///
		/// Otherwise, if name is null or invalid, throws an InvalidNameException.
		///
		/// Otherwise, if content parses as a double, the contents of the named
		/// cell becomes that double.
		///
		/// Otherwise, if content begins with the character '=', an attempt is made
		/// to parse the remainder of content into a Formula f using the Formula
		/// constructor with s => s.ToUpper() as the normalizer and a validator that
		/// checks that s is a valid cell name as defined in the AbstractSpreadsheet
		/// class comment.  There are then three possibilities:
		///
		///   (1) If the remainder of content cannot be parsed into a Formula, a
		///       Formulas.FormulaFormatException is thrown.
		///
		///   (2) Otherwise, if changing the contents of the named cell to be f
		///       would cause a circular dependency, a CircularException is thrown.
		///
		///   (3) Otherwise, the contents of the named cell becomes f.
		///
		/// Otherwise, the contents of the named cell becomes content.
		///
		/// If an exception is not thrown, the method returns a set consisting of
		/// name plus the names of all other cells whose value depends, directly
		/// or indirectly, on the named cell.
		///
		/// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
		/// set {A1, B1, C1} is returned.
		/// </summary>
		public override ISet<string> SetContentsOfCell(string name, string content)
		{
			throw new NotImplementedException();
		}


		/// <summary>
		/// gets the contents of the cell
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public override object GetCellContents(string name)
		{
			Cell toReturn;
			if (!checkIfValidNameAndNormalize(ref name))
			{
				throw new InvalidNameException();
			}
			if (!cds.getCellWithName(name, out toReturn))
			{
				return "";
			}
			else
			{
				return toReturn.contents;
			}
		}

		/// <summary>
		/// If name is null, throws an ArgumentNullException.
		/// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
		/// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
		/// values depend directly on the value of the named cell.  In other words, returns
		/// an enumeration, without duplicates, of the names of all cells that contain
		/// formulas containing name.
		/// For example, suppose that
		/// A1 contains 3
		/// B1 contains the formula A1 * A1
		/// C1 contains the formula B1 + A1
		/// D1 contains the formula B1 - C1
		/// The direct dependents of A1 are B1 and C1
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <exception cref="InvalidNameException"></exception>
		protected override IEnumerable<string> GetDirectDependents(string name)
		{

			return cds.getDependencyGraph().GetDependents(name).AsEnumerable();
		}


		/// <summary>
		/// Enumerates the names of all the non-empty cells in the spreadsheet.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<string> GetNamesOfAllNonemptyCells()
		{
			return Solver.SummonNames(cds);
		}


		/// <summary>
		/// heper method for all three SetCellContents.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="contents"></param>
		/// <returns></returns>
		private ISet<string> SetCellContentsMaster(string name, object contents)
		{
			if (contents == null) { throw new ArgumentNullException(); }
			//error check and normalize
			if (!checkIfValidNameAndNormalize(ref name))
			{
				throw new InvalidNameException();
			}

			//set contents, update deeptree(DG).
			object previousContents = cds.setContentsOrAddCell(name, contents);

			//after we fix the dependency graph we get the set ready with the method that was so kindly provided.
			HashSet<string> toReturn = new HashSet<string>();
			try
			{
				foreach (string s in GetCellsToRecalculate(name))
				{
					toReturn.Add(s);
				}
			}
			catch (CircularException e)
			{
				cds.setContentsOrAddCell(name, previousContents);
				throw e;
			}
			//and return for later use
			return toReturn;

		}

		/// <summary>
		/// Requires that all of the variables in formula are valid cell names.
		/// If name is null or invalid, throws an InvalidNameException.
		/// Otherwise, if changing the contents of the named cell to be the formula would cause a
		/// circular dependency, throws a CircularException.
		/// Otherwise, the contents of the named cell becomes formula.  The method returns a
		/// Set consisting of name plus the names of all other cells whose value depends,
		/// directly or indirectly, on the named cell.
		/// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
		/// set {A1, B1, C1} is returned.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="formula"></param>
		/// <returns></returns>
		public override ISet<string> SetCellContents(string name, Formula formula)
		{
			return SetCellContentsMaster(name, formula);

		}

		/// <summary>
		/// If name is null or invalid, throws an InvalidNameException.
		/// Otherwise, the contents of the named cell becomes number.  The method returns a
		/// set consisting of name plus the names of all other cells whose value depends,
		/// directly or indirectly, on the named cell.
		/// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
		/// set {A1, B1, C1} is returned.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="number"></param>
		/// <returns></returns>
		public override ISet<string> SetCellContents(string name, double number)
		{
			return SetCellContentsMaster(name, number);
		}

		/// <summary>
		/// If text is null, throws an ArgumentNullException.
		/// Otherwise, if name is null or invalid, throws an InvalidNameException.
		/// Otherwise, the contents of the named cell becomes text.  The method returns a
		/// set consisting of name plus the names of all other cells whose value depends,
		/// directly or indirectly, on the named cell.
		/// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
		/// set {A1, B1, C1} is returned.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		public override ISet<string> SetCellContents(string name, string text)
		{
			return SetCellContentsMaster(name, text);
		}


		private bool checkIfValidNameAndNormalize(ref string name)
		{
			//check null.
			if (name == null) return false;
			name = Solver.Normalize(name);

			//check if regex agrees.
			return (Regex.IsMatch(name, "[A-Za-z]([A-Za-z][1-9]|[1-9][0-9]|[1-9]$)[0-9]*") && Regex.IsMatch(name, validPattern));
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
		public Cell(Cell c)
		{
			name = c.name;
			contents = c.contents;
			value = c.value;
		}
	}

	/// <summary>
	/// comparator used for the setup of the set.
	/// </summary>
	class CellComparer : IEqualityComparer<Cell>
	{
		public bool Equals(Cell x, Cell y)
		{
			return x.name == y.name;
		}

		public int GetHashCode(Cell obj)
		{
			return obj.name.GetHashCode();
		}
	}
	/// <summary>
	/// The data structure to hold the guts of the spreadsheet
	/// works by tying a dependency graph with a Hash Set.
	/// </summary>
	class CellDS
	{
		//the location of the cells
		private HashSet<Cell> cells;

		private DependencyGraph deeptree;
		internal bool unsavedChanges;

		public CellDS()
		{
			cells = new HashSet<Cell>(new CellComparer());
			deeptree = new DependencyGraph();
			unsavedChanges = false;
		}
		/// <summary>
		/// adds a cell or replaces the contents of a cell and then recalculates dependency graph.
		/// retrns the previous cell in case of a circular exception fatrher down the line.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="newContents"></param>
		public object setContentsOrAddCell(string name, object newContents)
		{
			Cell cellOfInterest = new Cell();

			object previousContents;
			//if cell doesent exist create it,
			if (!getCellWithName(name, out cellOfInterest))
			{

				//if were here we need to add the cell.
				cellOfInterest.name = name;
				cellOfInterest.contents = newContents;
				cells.Add(cellOfInterest);
				previousContents = "";
			}

			else if (newContents == cellOfInterest.contents)
			{
				return cellOfInterest.contents;
			}
			else
			{
				previousContents = cellOfInterest.contents;
			}


			//if PC gets here we will soon have made a change.
			unsavedChanges = true;

			//once we have it initialized
			// we operate on contents adding/removing to/from the dependency tree

			deeptree.ReplaceDependees(name, Solver.RetrieveVariablesFromContents(newContents));

			//finally update contents
			cellOfInterest.contents = newContents;
			replaceCell(cellOfInterest);
			return previousContents;


		}

		public bool getCellWithName(string name, out Cell output)
		{
			foreach (Cell c in cells)
			{
				if (c.name == name)
				{
					output = c;
					return true;
				}

			}
			output = new Cell();
			return false;
		}
		public void replaceCell(Cell c)
		{
			cells.Remove(c);
			cells.Add(c);
		}
		public DependencyGraph getDependencyGraph()
		{
			return deeptree;
		}

		///helps the get cell value method
		public object returnValueOfFormula(Cell cellOfInterest)
		{
			return cellOfInterest.value;
		}

		internal HashSet<Cell> getCells()
		{
			return cells;
		}
	}
	static class Solver
	{



		private static Normalizer defaultNormailzer = (s => s.ToUpper());


		public static IEnumerable<string> SummonNames(CellDS cds)
		{
			//this overly complicated statement returns all nonempty cell names
			return (cds.getCells()).Select(c => c.contents is string ? ((string)c.contents == "" ? "" : c.name) : c.name).Except(new HashSet<string>() { "" });
		}
		public static string Normalize(string toBeNormalized)
		{
			return Normalize(toBeNormalized, defaultNormailzer);
		}
		public static string Normalize(string toBeNormalized, Normalizer n)
		{
			return n(toBeNormalized);
		}

		public static ISet<string> RetrieveVariablesFromContents(object contents)
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
	