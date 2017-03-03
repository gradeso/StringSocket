using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpreadsheetGui;

namespace SpreadsheetGUI
{
	public partial class SpreadsheetWindow : Form, ISpreadsheetView
	{
        StringBuilder s = new System.Text.StringBuilder("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
		private SpreadsheetPanel currentPannel;

		public event Action<string> cellWithNameChagendContents;
		public event Action<string> cellHighlighted;
		public event Action ContentBoxUsed;
		public event Action newSS;
		public event Action<string> loadSS;
		public event Action saveSS;

		public string message
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


		/// <summary>
		///any time this value changes we update all cells that changed.
		/// </summary>
		/// <value>
		/// The cells2 change.
		/// </value>
		public ISet<string> cells2Change
		{
			set
			{
				cells2Change = value;
				updateAll();
			}
		}

		public SpreadsheetWindow()
		{
			InitializeComponent();
            spreadsheetCellArray.SelectionChanged += displaySelection;
        }

        private void displaySelection(SpreadsheetPanel ss)
        {
            int row, col;
            
            ss.GetSelection(out col, out row);
            var val = s[col] + (row + 1).ToString();
			ss.SetValue(col, row, AddressBox.Text);
			currentPannel = ss;
			cellHighlighted(val);
        }

		/// <summary>
		/// Updates all the vlaues of the cell,
		/// called after the contents box has changed the value of calls2change
		/// </summary>
		private void updateAll()
		{
			
		}
		
        private void spreadsheetPanel1_Load(object sender, EventArgs e)
		{
			
		}

		private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{

		}

		private void ClickEvent(object sender, MouseEventArgs e)
		{
			int x, y;
			spreadsheetCellArray.GetSelection(out x, out y);
			AddressBox.Text = x.ToString();
			AddressBox.Update();
			spreadsheetCellArray.SetValue(1,1,"Hello");
            spreadsheetCellArray.Update();
		}

        private void ClickOnCells(object sender, MouseEventArgs e)
        {
            spreadsheetCellArray.Select();
            spreadsheetCellArray.SetValue(1, 1, "Click On Cells");
        }

        private void AdressBoxClick(object sender, EventArgs e)
        {
            AddressBox.Text = "hello!";
            AddressBox.Refresh();
        }

		/// <summary>
		/// Handles the Load event of the spreadsheetCellArray control. Load and New will access this method.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		private void spreadsheetCellArray_Load(object sender, EventArgs e)
        {
            spreadsheetCellArray.Select();
        }

        private void ClickOnCells(object sender, EventArgs e)
        {
            spreadsheetCellArray.Select();
            spreadsheetCellArray.SetValue(1, 1, "Click On Cells");
            spreadsheetCellArray.Refresh();
            spreadsheetCellArray.Update();
        }

		private void ContentBox_Keypress(object sender, KeyPressEventArgs e)
		{

			if (((char)Keys.Enter).Equals(e.KeyChar))
			{
				
				Console.WriteLine("Works");
			}
		}
	}
}
