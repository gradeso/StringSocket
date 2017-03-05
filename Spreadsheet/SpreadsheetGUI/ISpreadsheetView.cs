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
		Dictionary<string, string> toUpdate { set; }

		event Action<string, string> cellWithNameChagendContents;  //pass name and new contents
		event Action<string> cellHighlighted;               //pass name
		event Action<string> loadSS;						//pass filepath
		event Action saveSS;
		event Action closeSS;
	}
}
