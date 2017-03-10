using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetGUI;

namespace ControllerTester
{
    class SpreadsheetWindowStub : ISpreadsheetView
    {
        public bool DoClose_Called { get; private set; }
        public bool DoCloseWithSaved_Called { get; private set; }
        string filename = @"C:\Users\Wes BEAST\Documents\Visual Studio 2015\Projects\sceneKids\testOfSS";


        // Methods to fire events
        public void FireCellContentsChanged()
        {
            cellContentsChanged("A1","content");
        }

        public void FireCellContentsChanged_InvalidName()
        {
            try { cellContentsChanged("1", "content"); }
            catch (Exception) { }
        }

        public void FireCellHighlighted()
        {
            cellHighlighted("A1");
        }

        public void FireCellHighlighted_InvalidName()
        {
            try { cellHighlighted("A1"); }
            catch (Exception) { }
        }

        public void FireCloseSS()
        {
            closeSS();
        }

        public void FireLoadSS()
        {
            loadSS("C:\\Users\\Wes BEAST\\Documents\\Visual Studio 2015\\Projects\\sceneKids\\Spreadsheet\\SampleSavedSpreadsheet.ss");
        }

        public void FireSaveSS()
        {
            saveSS("C:\\Users\\Wes BEAST\\Documents\\Visual Studio 2015\\Projects\\sceneKids\\Spreadsheet\\SampleSavedSpreadsheet1.ss");
        }

        // The following properties implement the interface ********************
        public string currentContents { set; get; }

        public string currentName { set; get; }

        public string currentValue { set; get; }

        public string message { set; get; }

        public Dictionary<string, string> toUpdate { set; get; }

        // The following events implement the interface ************************
        public event Action<string, string> cellContentsChanged;
        public event Action<string> cellHighlighted;
        public event Action closeSS;
        public event Action<string> loadSS;
        public event Action<string> saveSS;
        public event Action windowExited;

        public void DoClose()
        {

            DoClose_Called = true;
        }

        public void DoCloseWithSave(bool saved)
        {
            DoCloseWithSaved_Called = true;
        }
    }
}
