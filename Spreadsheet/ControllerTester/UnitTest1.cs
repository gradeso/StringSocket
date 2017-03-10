using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetGUI;

namespace ControllerTester
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            SpreadsheetWindowStub stub = new SpreadsheetWindowStub();
            SpreadsheetController controller = new SpreadsheetController(stub);
            stub.FireCellContentsChanged();
            stub.FireCellContentsChanged_InvalidName();
            stub.FireCellHighlighted();
            stub.FireCellHighlighted_InvalidName();
            stub.FireCloseSS();
            stub.FireSaveSS();
            stub.FireLoadSS();
            stub.DoClose();
        }
    }
}
