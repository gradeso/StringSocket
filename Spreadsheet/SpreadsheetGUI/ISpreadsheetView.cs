using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetGUI
{
	interface ISpreadsheetView
	{
		string message { set; }
		string currentValue { set; }
		string currentContents { set; }
		string currentName { set; }
		ISet<string> cells2Change { set; }

		event Action<string> cellWithNameChagendContents;  //pass new contents
		event Action<string> cellHighlighted;				//pass name


		event Action newSS;
		event Action<string> loadSS;						//pass filepath
		event Action saveSS;
	}
}
