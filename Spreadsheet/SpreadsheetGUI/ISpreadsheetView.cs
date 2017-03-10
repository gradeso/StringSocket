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

        event Action<string> cellHighlighted;
        event Action<string> loadSS;
        event Action<string> saveSS;
        event Action closeSS;
        event Action<string, string> cellContentsChanged;
        event Action windowExited;

        void DoClose();
        void DoCloseWithSave(bool saved);
    }
}
