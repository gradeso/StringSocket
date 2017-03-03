using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spreadsheet
{
	interface SpreadsheetMethods 
	{
		object GetCellContents(string name);
		void Save(TextWriter dest);
		IEnumerable<String> GetNamesOfAllNonemptyCells();
		object GetCellValue(String name);
		ISet<String> SetContentsOfCell(String name, String content);
	}
}
