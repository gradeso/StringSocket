using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SS;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

namespace SpreadsheetGUI
{
	class SpreadsheetController 
	{
		/// <summary>
		/// The view
		/// </summary>
		ISpreadsheetView view;
		/// <summary>
		/// The model
		/// </summary>
		Spreadsheet model;

		/// <summary>
		/// Initializes a new instance of the <see cref="SpreadsheetController"/> class.
		/// </summary>
		/// <param name="view">The view.</param>
		public SpreadsheetController(ISpreadsheetView view)
		{
			this.view = view;
			model = new Spreadsheet();
			view.loadSS += HandleLoadSS;
            view.saveSS += HandleSaveSS;
            view.closeSS += HandleCloseSS;
			view.cellWithNameChagendContents += HandleCellWithNameChangedContents;
			view.cellHighlighted += HandleCellHighlighted;	
		}

        private void HandleCloseSS()
        {
            view.DoClose();
        }

        /// <summary>
        /// Handles the load ss.
        /// </summary>
        /// <param name="filename">The filename.</param>
        private void HandleLoadSS(string filename)
		{
            //Create a new Regex for the param of Spreadsheet(TextWriter dest, Regex isValid)
            Regex reg = new Regex("^.*$");
            TextReader read = File.OpenText(filename);

            //Create a new Spreadsheet using the two params
            model = new Spreadsheet(read, reg);

            //Create a new dicitonary to pass to UpdateAll 
            Dictionary<string, string> newVals = new Dictionary<string, string>();

            //For each cell in the new Spreadsheet model, we update the view to reflect the addition
            var newCells = model.GetNamesOfAllNonemptyCells();
            foreach (string name in newCells)
            {
                newVals.Add(name, model.GetCellContents(name).ToString());
            }
            view.toUpdate = newVals;
        }

        private void HandleSaveSS(string filename)
        {
            using (TextWriter write = File.CreateText(filename))
            {
                if (model.Changed)
                {
                    model.Save(write);
                }
                else
                    return;
            }
        }

		/// <summary>
		/// Handles the cell with name changed contents.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="newContents">The new contents.</param>
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
