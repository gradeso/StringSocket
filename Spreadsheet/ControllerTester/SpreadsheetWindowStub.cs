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
        public string currentContents
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public string currentName
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public string currentValue
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public string message
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public Dictionary<string, string> toUpdate
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public event Action<string> cellHighlighted;
        public event Action<string, string> cellWithNameChagendContents;
        public event Action closeSS;
        public event Action<string> loadSS;
        public event Action<string> saveSS;

        public void DoClose()
        {
            throw new NotImplementedException();
        }
    }
}
