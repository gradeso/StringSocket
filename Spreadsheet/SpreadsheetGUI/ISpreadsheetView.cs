using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetGUI
{
	public interface ISpreadsheetView
	{
		string message { set; }
		string currentValue { set; }
		string currentContents { set; }
		string currentName { set; }
		Dictionary<string, string> toUpdate { set; }

		event Action<string, string> cellContentsChanged;  //pass name and new contents
		event Action<string> cellHighlighted;               //pass name
		event Action<string> loadSS;                        //pass filepath
        event Action<string> saveSS;
        event Action closeSS;

        void DoClose();
    }
}
