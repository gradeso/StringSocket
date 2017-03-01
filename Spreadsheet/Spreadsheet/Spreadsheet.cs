using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace SS
{

	/// <summary>
	/// this class represents an implementation of a spreadsheet.
	/// </summary>
	/// <seealso cref="SS.AbstractSpreadsheet" />
	public class Spreadsheet : AbstractSpreadsheet
	{
		/// <summary>
		/// The central data structure for the spreadsheet
		/// </summary>
		private CellDS cds;
		/// <summary>
		/// is valid string for regex purposes.
		/// </summary>
		private string IsValid;


		//dont forget to add the other constructors.



		/// <summary>
		/// Initializes a new instance of the <see cref="Spreadsheet"/> class.
		/// </summary>
		public Spreadsheet()
		{
			cds = new CellDS();
			IsValid = "[A-Za-z]([A-Za-z][1-9]|[1-9][0-9]|[1-9]$)[0-9]*";

		}

		/// <summary>
		/// Creates an empty Spreadsheet whose IsValid regular expression accepts every string.
		/// </summary>
		/// <param name="isValid">The is valid.</param>
		public Spreadsheet(Regex isValid)
		{
			cds = new CellDS();
			this.IsValid = isValid.ToString();

		}


		/// Creates a Spreadsheet that is a duplicate of the spreadsheet saved in source.
		///
		/// See the AbstractSpreadsheet.Save method and Spreadsheet.xsd for the file format 
		/// specification.  
		///
		/// If there's a problem reading source, throws an IOException.
		///
		/// Else if the contents of source are not consistent with the schema in Spreadsheet.xsd, 
		/// throws a SpreadsheetReadException.  
		///
		/// Else if the IsValid string contained in source is not a valid C# regular expression, throws
		/// a SpreadsheetReadException.  (If the exception is not thrown, this regex is referred to
		/// below as oldIsValid.)
		///
		/// Else if there is a duplicate cell name in the source, throws a SpreadsheetReadException.
		/// (Two cell names are duplicates if they are identical after being converted to upper case.)
		///
		/// Else if there is an invalid cell name or an invalid formula in the source, throws a 
		/// SpreadsheetReadException.  (Use oldIsValid in place of IsValid in the definition of 
		/// cell name validity.)
		///
		/// Else if there is an invalid cell name or an invalid formula in the source, throws a
		/// SpreadsheetVersionException.  (Use newIsValid in place of IsValid in the definition of
		/// cell name validity.)
		///
		/// Else if there's a formula that causes a circular dependency, throws a SpreadsheetReadException. 
		///
		/// Else, create a Spreadsheet that is a duplicate of the one encoded in source except that
		/// the new Spreadsheet's IsValid regular expression should be newIsValid.
		public Spreadsheet(TextReader source, Regex newIsValid)
		{
			cds = new CellDS();
			
			LinkedList<string> names = new LinkedList<string>();
			LinkedList<string> contentsList = new LinkedList<string>();
			using (var xmr = XmlReader.Create(source))
				{
				
					while (xmr.Read())
					{
						if (xmr.IsStartElement())
						{
							
							switch (xmr.Name)
							{
							case "spreadsheet":
								IsValid = xmr["IsValid"];
								if(IsValid == null) throw new SpreadsheetReadException("is valid didnt read properly");
								break;

								case "cell":
							
								var temp = xmr["name"];
								if (names.Contains(temp)) throw new SpreadsheetReadException("duplicite cell names");
								try
								{
									if (!Regex.IsMatch(temp, IsValid)) throw new SpreadsheetReadException("source file unreadable");
								}
								catch(ArgumentException e)
								{
									throw new SpreadsheetReadException( e.Message);
								}
								names.AddLast(temp);
								if (temp == null) throw new SpreadsheetReadException("a name didnt read properly");

								//deal with the contents
								string form = xmr["contents"];
								if (form == null) throw new SpreadsheetReadException("contents didnt read properly");

								if (form.Substring(0, 1).Equals("="))
								{
									try
									{
										new Formula(form.Substring(1), (s => s), checkIfValidName);
									}
									catch (FormulaFormatException e)
									{
										throw new SpreadsheetReadException("source file unreadable, bad formula");
									}
								}
								contentsList.AddLast(form);
									break;
								default:
									throw new IOException();
							
							}
						}
					}
				}
			IsValid = newIsValid.ToString();
			int i = 0;
			try
			{

				foreach (string s in names)
				{
					SetContentsOfCell(s, contentsList.ElementAt(i));
					i++;
				}
			}
			catch (Exception)
			{
				throw new SpreadsheetVersionException("bad regex on new file");

			}
			if(names.Count != contentsList.Count) throw new SpreadsheetVersionException("uneven data matches");



		}
		/// <summary>
		/// represents if the ss has been changed since last save,
		/// PS I know this is bad practice if you could comment
		/// on how i should handle protection levels when i need 2way communication with a field and a dependent class.
		/// </summary>
		public override bool Changed
		{
			get
			{
				return cds.unsavedChanges;
			}

			protected set
			{
				cds.unsavedChanges = value;
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
			using (var sw = new StringWriter())
			{
				using (var xw = XmlWriter.Create(sw))
				{
					xw.WriteStartDocument();
					xw.WriteStartElement("spreadsheet");
					
					xw.WriteAttributeString("IsValid", IsValid.ToString());
					foreach(Cell c in cds.getCells())
					{
						xw.WriteStartElement("cell");
						xw.WriteAttributeString("name",c.name);
						//gotta <3 lambdas!
						xw.WriteAttributeString("contents", 
							(c.contents is string ? (string)c.contents : 
							(c.contents is double ? c.contents.ToString() :
							(c.contents is Formula ? "=" + c.contents.ToString() :
							pretendToReturnStringButActuallyThrowAnException()))));
						xw.WriteEndElement();
					}

					xw.WriteEndElement();
					xw.WriteEndDocument();

				}
				dest.Write(sw.ToString());
			}
			Changed = false;
		}
		/// <summary>
		/// Pretends to return string but actually throw an exception.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.IO.IOException"></exception>
		private string pretendToReturnStringButActuallyThrowAnException() {
			throw new IOException();
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
				return toReturn.value;
			}
		
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
			
			if (content == null) throw new ArgumentNullException();
			
			if(!checkIfValidName(name) )throw new InvalidNameException();


			double newContentsDub;
			if (double.TryParse(content, out newContentsDub))
			{
				
				return SetCellContents(name, newContentsDub);
			}
			if (String.IsNullOrWhiteSpace(content)) return SetCellContents(name, content);
			if (content.Substring(0, 1).Equals("=")){
				return SetCellContents(name, 
								new Formula(content.Substring(1), (s => s.ToUpper()), (n => Regex.IsMatch(n, IsValid))));
			}
			return SetCellContents(name, content);
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

			var solved = new HashSet<Cell>(new CellComparer());
			//after we fix the dependency graph we get the set ready with the method that was so kindly provided.
			IEnumerable<string> cellsToRecalculate;
			try
			{
				cellsToRecalculate = GetCellsToRecalculate(name);
				
			}
			catch (CircularException e)
			{
				cds.setContentsOrAddCell(name, previousContents);
				throw e;
			}

			return cds.recalculate(cellsToRecalculate);
			//and return for later use
			 

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

		/// <summary>
		/// Checks the name of if valid.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		private bool checkIfValidName(string name) {
			if (name == null) return false;
			return (Regex.IsMatch(name, "[A-Za-z]([A-Za-z][1-9]|[1-9][0-9]|[1-9]$)[0-9]*") && Regex.IsMatch(name, IsValid));
		}


		/// <summary>
		/// Checks if valid name and normalizes the string paramether.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		private bool checkIfValidNameAndNormalize(ref string name)
		{
			//check null.
			if (name == null) return false;
			name = Solver.Normalize(name);

			//check if regex agrees.
			return (Regex.IsMatch(name, "[A-Za-z]([A-Za-z][1-9]|[1-9][0-9]|[1-9]$)[0-9]*") && Regex.IsMatch(name, IsValid));
		}
	}
	/// <summary>
	/// The cell class that represents a cell in the spreadsheet.
	/// </summary>
	struct Cell
	{
		

		/// <summary>
		/// The name
		/// </summary>
		public string name;
		/// <summary>
		/// The contents
		/// </summary>
		public object contents;
		/// <summary>
		/// The value
		/// </summary>
		public object value;


		/// <summary>
		/// Initializes a new instance of the <see cref="Cell"/> struct.
		/// </summary>
		/// <param name="namein">The namein.</param>
		/// <param name="contentsin">The contentsin.</param>
		public Cell(string namein, object contentsin)
		{
			name = namein;
			contents = contentsin;
			value = "";
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Cell"/> struct.
		/// </summary>
		/// <param name="c">The c.</param>
		public Cell(Cell c)
		{
			name = c.name;
			contents = c.contents;
			value = c.value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Cell"/> struct.
		/// </summary>
		/// <param name="s">The s.</param>
		public Cell(string s) 
		{
			name = s;
			contents = null;
			value = null;
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
		private HashSet<Cell> solved;
		public CellDS()
		{
			solved = new HashSet<Cell>(new CellComparer());

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

		/// <summary>
		/// Gets the name of the cell with.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="output">The output.</param>
		/// <returns></returns>
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

		/// <summary>
		/// Replaces the cell.
		/// </summary>
		/// <param name="c">The c.</param>
		public void replaceCell(Cell c)
		{
			cells.Remove(c);
			cells.Add(c);
		}
		/// <summary>
		/// Gets the dependency graph.
		/// </summary>
		/// <returns></returns>
		public DependencyGraph getDependencyGraph()
		{
			return deeptree;
		}

		///helps the get cell value method
		 object returnValueOfFormula(Cell cellOfInterest)
		{
			return cellOfInterest.value;
		}
		

		/// <summary>
		/// Gets the cells.
		/// </summary>
		/// <returns></returns>
		internal HashSet<Cell> getCells()
		{
			return cells;
		}

		/// <summary>
		/// Generates the lookup used by recalculate. takes the solved cells as a parameter
		/// returns a function for all mappings name to value found so far with.
		/// </summary>
		/// <param name="cellsSolved">The cells solved.</param>
		/// <returns></returns>
		private Lookup generateLookup(HashSet<Cell> cellsSolved)
		{
			var lookup = cellsSolved.ToLookup(c => c.name, c => (double)c.value);
			return (s => lookup[s].First());
		}


		/// <summary>
		/// Recalculates the specified cells to recalculate.
		/// </summary>
		/// <param name="cellsToRecalculate">The cells to recalculate.</param>
		internal ISet<string> recalculate(IEnumerable<string> cellsToRecalculate)
		{
			var toReturn = new HashSet<string>();
			foreach (string s in cellsToRecalculate)
			{
				toReturn.Add(s);
				solved.Remove(new Cell(s));
			}

			
			//iterate through the cellstoRecalculate, 
			foreach (string s in cellsToRecalculate)
			{
				Cell cel;
				getCellWithName(s, out cel); //will return true, error checking done else where.

				//if contents of a cell is a double, add name to set of solved values update value then continue
				if (cel.contents is double)
				{
					cel.value = cel.contents;
					solved.Add(cel);
				}
				//if contents is a formula
				//we then pass the lookup with [s] as parameter to evaluate contents of s and save it to the value
				//add the current cell name to a list of solved values

				else if (cel.contents is Formula)
				{
					try
					{
						var luf = generateLookup(solved);
						//as we iterate we turn solved into a lookup that maps Lookup to name
						cel.value = ((Formula)cel.contents).Evaluate(luf);
						solved.Add(cel);
					}
					catch (FormulaEvaluationException e)
					{
						cel.value = new FormulaError(e.Message);

					}
				}
				else if (cel.contents is string)
				{
					cel.value = cel.contents;
				}
				replaceCell(cel);
				
			}
			return toReturn;
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
	