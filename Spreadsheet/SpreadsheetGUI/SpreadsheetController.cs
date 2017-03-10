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
	public class SpreadsheetController 
	{
		/// <summary>
		/// The view
		/// </summary>
		private ISpreadsheetView spreadsheetView;
		/// <summary>
		/// The model
		/// </summary>
		private Spreadsheet model;

		/// <summary>
		/// Initializes a new instance of the <see cref="SpreadsheetController"/> class. This 
        /// class is the control of the Spreadsheet GUI implementation, it handles the interface
        /// between the model and the view.
		/// </summary>
		/// <param name="view">The view.</param>
		public SpreadsheetController(ISpreadsheetView view)
		{
			this.spreadsheetView = view;
			model = new Spreadsheet();
			view.loadSS += HandleLoadSS;
            view.saveSS += HandleSaveSS;
            view.closeSS += HandleCloseSS;
			view.cellContentsChanged += HandleCellWithNameChangedContents;
			view.cellHighlighted += HandleCellHighlighted;
            view.windowExited += HandleWindowExit;
                	
		}

        /// <summary>
        /// Initilizes a new instance of the <see cref="SpreadsheetController"/> with
        /// a filename parameter. This constructor is to be used for load operations.
        /// </summary>
        /// <param name="view">The window being displayed</param>
        /// <param name="filename">The filepath of the object being loaded</param>
        public SpreadsheetController(ISpreadsheetView view, string filename)
        {
            this.spreadsheetView = view;
            view.loadSS += HandleLoadSS;
            view.saveSS += HandleSaveSS;
            view.closeSS += HandleCloseSS;
            view.cellContentsChanged += HandleCellWithNameChangedContents;
            view.cellHighlighted += HandleCellHighlighted;
            view.windowExited += HandleWindowExit;

            model = null;
            DoLoad(filename);

        }

        /// <summary>
        /// When the exit button is pressed, this hook calls the DoCloseWithSave method, which handles the 
        /// state of the spreadsheet.
        /// </summary>
        private void HandleWindowExit()
        {
            spreadsheetView.DoCloseWithSave(model.Changed);
        }

        private void HandleCloseSS()
        {
            if (!model.Changed)
                spreadsheetView.DoClose();
            else
            {
                try { spreadsheetView.DoCloseWithSave(model.Changed); }
                catch (Exception) { return; }
            }
        }

        /// <summary>
        /// Handles the load ss.
        /// </summary>
        /// <param name="filename">The filename.</param>
        private void HandleLoadSS(string filename)
        {
            
            try { SpreadsheetApplicationContext.GetContext().RunNew(filename); }
            catch (SpreadsheetReadException)
            { return; }
            catch (Exception e) { return; }

        }

        /// <summary>
        /// Deals with the second SpreadsheetController Constructor that passes a file path
        /// so that the file to load can be accessed and read here.
        /// </summary>
        /// <param name="filename"></param>
        private void DoLoad(string filename)
        {
            //Create a new Regex for the param of Spreadsheet(TextWriter dest, Regex isValid)
            Regex reg = new Regex("^.*$");

            TextReader read = File.OpenText(filename);

            //Create a new Spreadsheet model using the two params
            model = new Spreadsheet(read, reg);

            //Create a new dicitonary to pass to UpdateAll 
            Dictionary<string, string> newVals = new Dictionary<string, string>();

            //For each cell in the new Spreadsheet model, we update the view to reflect the addition
            var newCells = model.GetNamesOfAllNonemptyCells();
            foreach (string name in newCells)
            {
                newVals.Add(name, model.GetCellValue(name).ToString());
            }


            //Send all nonempty cells to be updated in view
            spreadsheetView.toUpdate = newVals;

        }

        private void HandleSaveSS(string filename)
        {
            try
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
            catch (Exception)
            {

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
				spreadsheetView.toUpdate = updateDict;
			}
			catch (Exception e)
			{
				spreadsheetView.message = e.Message;
				model.SetContentsOfCell(name, previousContents.ToString());
			}

		}

		/// <summary>
		/// Handles the cell highlighted and set the variables for the view to display
		/// </summary>
		/// <param name="name">The name.</param>
		private void HandleCellHighlighted(string name)
		{
			spreadsheetView.currentName = name;
			try
			{
				spreadsheetView.currentContents = model.GetCellContents(name).ToString();
				spreadsheetView.currentValue = model.GetCellValue(name) is FormulaError ? "Evaluation Error" : model.GetCellValue(name).ToString();
			}
			catch (InvalidNameException)
			{
				spreadsheetView.currentContents = "";
				spreadsheetView.currentValue = "";
			}
		}
	}
	
}
