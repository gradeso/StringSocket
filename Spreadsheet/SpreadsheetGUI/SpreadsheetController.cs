using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SS;
namespace SpreadsheetGUI
{
	class SpreadsheetController 
	{
		ISpreadsheetView view;
		Spreadsheet model;

		public SpreadsheetController(ISpreadsheetView view)
		{
			this.view = view;
			model = new Spreadsheet();
			view.newSS += HandleNewSS;
			view.loadSS += HandleLoadSS;
			view.cellWithNameChagendContents += HandleCellWithNameChangedContents;
			view.cellHighlighted += HandleCellHighlighted;
			
		}

		private void HandleNewSS()
		{
			throw new NotImplementedException();
		}

		private void HandleLoadSS(string filename)
		{
			throw new NotImplementedException();
		}

		private void HandleCellWithNameChangedContents(string name, string newContents)
		{

			var previousContents = model.GetCellContents(name);
			var updateDict = new Dictionary<string, string>();
			try
			{
				 var cellsChanged = model.SetContentsOfCell(name, newContents);
				foreach (string s in cellsChanged)
				{
					updateDict.Add(s, model.GetCellValue(s).ToString());
				}
				view.toUpdate = updateDict;
			}
			catch (Exception e)
			{
				view.message = e.Message;
				model.SetContentsOfCell(name, previousContents.ToString());
			}
			
		}

		/// <summary>
		/// Handles the cell highlighted and set the variables for the view to display
		/// </summary>
		/// <param name="name">The name.</param>
		/// 
		private void HandleCellHighlighted(string name)
		{
			view.currentName = name;
			try
			{
				
				
				view.currentContents = model.GetCellContents(name).ToString();
				view.currentValue = model.GetCellValue(name) is FormulaError ? "Evaluation Error" : model.GetCellValue(name).ToString();
			}
			catch (InvalidNameException)
			{

				view.currentContents = "";
				view.currentValue = "";
			}
		}
	}
	
}
