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
		public Spreadsheet()
		{
			InitializeComponent();
			
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
            
        }

        private void AdressBoxClick(object sender, EventArgs e)
        {
            AddressBox.Text = "hello!";
            AddressBox.Refresh();
        }
    }
}
