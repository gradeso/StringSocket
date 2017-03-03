using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
	public partial class Spreadsheet : Form
	{
        StringBuilder s = new System.Text.StringBuilder("ABCDEFGHIJKLMNOPQRSTUVWXYZ");

		public Spreadsheet()
		{
			InitializeComponent();
            spreadsheetCellArray.SelectionChanged += displaySelection;
        }

        private void displaySelection(SpreadsheetGui.SpreadsheetPanel ss)
        {
            int row, col;
            string value;
            ss.GetSelection(out col, out row);
            AddressBox.Text = s[col] + (row + 1).ToString();

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

        private void spreadsheetCellArray_Load(object sender, EventArgs e)
        {
            spreadsheetCellArray.Select();
            spreadsheetCellArray.SetValue(1, 1, "Swar!");
        }

        private void ClickOnCells(object sender, EventArgs e)
        {
            spreadsheetCellArray.Select();
            spreadsheetCellArray.SetValue(1, 1, "Click On Cells");
            spreadsheetCellArray.Refresh();
            spreadsheetCellArray.Update();
        }
    }
}
