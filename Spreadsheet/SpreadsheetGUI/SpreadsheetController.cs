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
		string currentCellname;

		public SpreadsheetController(ISpreadsheetView view)
		{
			this.view = view;
			model = new Spreadsheet();
			view.newSS += HandleNewSS;
			view.loadSS += HandleLoadSS;
			view.cellWithNameChagendContents += HandleCellWithNameChangedContents;
			view.cellHighlighted += HandleCellHighlighted;

			currentCellname = null;
		}

		private void HandleNewSS()
		{
			throw new NotImplementedException();
		}

		private void HandleLoadSS(string filename)
		{
			throw new NotImplementedException();
		}

		private void HandleCellWithNameChangedContents(string newContents)
		{
			throw new NotImplementedException();
		}

		private void HandleCellHighlighted(string name)
		{
			throw new NotImplementedException();
		}
	}
}
