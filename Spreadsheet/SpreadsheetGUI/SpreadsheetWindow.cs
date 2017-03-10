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
using System.IO;

namespace SpreadsheetGUI
{
    public partial class SpreadsheetWindow : Form, ISpreadsheetView
    {
        /// <summary>
        /// Instance object that helps to decode integer cell addressing into spreadsheet cell addressing
        /// </summary>
        private StringBuilder colRefference = new StringBuilder("ABCDEFGHIJKLMNOPQRSTUZWXYZ");

        /// <summary>
        /// Stores the file path and name of the current object; is null until saved the first time.
        /// </summary>
        private string filename;

        // ************** These Events and Properties implement the interface **************************//
        public event Action<string> cellHighlighted;
        public event Action<string> loadSS;
        public event Action<string> saveSS;
        public event Action closeSS;
        public event Action<string, string> cellContentsChanged;
        public event Action windowExited;

        public string message { set { MessageBox.Show(value); } }

        public string currentValue { set { ValueBox.Text = value; } }

        public string currentContents { set { ContentBox.Text = value; } }

        public string currentName { set { AddressBox.Text = value; } }

        /// <summary>
        ///An object that is populated with names and contents of cells that need to be updated in the view. 
        /// </summary>
        /// <value>
        /// The cells to change.
        /// </value>
        public Dictionary<string, string> toUpdate { set { UpdateAll(value); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadsheetWindow"/> class.
        /// Hooks the <see cref="SpreadsheetPanel"/> 'Cell selection changed' method. 
        /// </summary>
        public SpreadsheetWindow()
        {
            InitializeComponent();
            spreadsheetCellArray.SelectionChanged += HandleSelectionChanged;
            FormClosing += WindowClosed;
        }

        /// <summary>
        /// When the cell that is selected changes, the even fired is hooked here. This method 
        /// converts the int address of the cell into the format required for Spreadsheet, gets
        /// the direct and indirect dependents of the cell that is selected, and updates all 
        /// those values as needed.
        /// </summary>
        /// <param name="ss"></param>
        private void HandleSelectionChanged(SpreadsheetPanel ss)
        {
            int row, col;
            ss.GetSelection(out col, out row);
            var val = colRefference[col] + (row + 1).ToString();
            cellHighlighted(val);
        }

        /// <summary>
        /// Takes the toUpdate dictionary and iterates through all the cells contained therein, and
        /// updates the view to reflect any changes caused by changing the value of a cell.
        /// </summary>
        /// <param name="value"></param>
        private void UpdateAll(Dictionary<string, string> value)
        {
            string temp;
            foreach (string s in value.Keys)
            {
                value.TryGetValue(s, out temp);
                int[] coords = convertNameToCoords(s);
                spreadsheetCellArray.SetValue(coords[0], coords[1], temp);
            }
        }

        /// <summary>
        /// Converts the spreadsheet style addressing back into int addressing for the SpreadsheetPanel to use.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private int[] convertNameToCoords(string s)
        {
            var toReturn = new int[2];
            int i = 0;

            while (s.Substring(0, 1) != "" + colRefference[i])
            {
                i++;
            }

            toReturn[0] = i;
            int j;
            int.TryParse(s.Substring(1), out j);
            toReturn[1] = j - 1;
            return toReturn;
        }

        /// <summary>
        /// The event that occurs when a key is pressed in the content box; if the keypress is enter, the value is added to the model.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentBox_Keypress(object sender, KeyPressEventArgs e)
        {
            int col, row;
            spreadsheetCellArray.GetSelection(out col, out row);
            var name = colRefference[col] + (row + 1).ToString();

            if (((char)Keys.Enter).Equals(e.KeyChar))
            {
                cellContentsChanged(name, ContentBox.Text);
            }

        }

        /// <summary>
        /// Handles the Load event of the spreadsheetCellArray control. Sets the focus to the content box, and intializes the address 
        /// box to display A1
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void spreadsheetCellArray_Load(object sender, EventArgs e)
        {
            spreadsheetCellArray.Select();
            AddressBox.Text = "A1";
            ContentBox.Focus();
        }

        /// <summary>
        /// Saves the current spreadsheet. Specifically it handles the Save As menu button selection; prompts the user to select the destination of the file
        /// that they are trying to save.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAsSelected(object sender, EventArgs e)
        {
            if (filename != null)
                SaveSelected(sender, e);

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Save the Current Spreadsheet";
            //saveFileDialog1.AddExtension = true;
            saveFileDialog1.Filter = "SS File(*.ss)|*.ss|All files (*.*)|*.*";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != null)
            {
                filename = saveFileDialog1.FileName;
                
                saveSS(filename);
            }
        }

        /// <summary>
        /// Saves the current Spreadsheet. Specifically, it handles the Save menu button press. If the file has already been saved and has 
        /// a filename and path, then the save continues. If a filename has not already been provided, then we call SaveAs to get one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveSelected(object sender, EventArgs e)
        {
            if (filename == null)
                SaveAsSelected(sender, e);

            saveSS(filename);
        }

        /// <summary>
        /// When the user presses the exit button (as opposed to close) this event handles that behavior.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowClosed(Object sender, FormClosingEventArgs e)
        {
            windowExited();
        }

        /// <summary>
        /// Handles the loading (or opening) of a file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadSelected(object sender, EventArgs e)
        {
            FileDialog load = new OpenFileDialog();
            load.Title = "Select the File to Load";
            load.Filter = "SS File(*.ss)|*.ss|All files (*.*)|*.*";
            load.ShowDialog();
            if (load.FileName != null)
            {
                if (Path.GetExtension(load.FileName) != ".ss")
                {
                    if(MessageBox.Show("The selected filetype is not supported!","Error Reading File",MessageBoxButtons.OK) == DialogResult.OK)
                        return;
                }
                loadSS(load.FileName);
            }
            else
                return;
        }

        /// <summary>
        /// Opens a new instance of a spreadsheet window. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleSelectNew(object sender, EventArgs e)
        {
            SpreadsheetApplicationContext.GetContext().RunNew();
        }

        /// <summary>
        /// Handles the close window button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseSelected(object sender, EventArgs e)
        {
            closeSS();
        }

        /// <summary>
        /// Closes this window.
        /// </summary>
        public void DoClose()
        {
            Close();
        }


        public void DoCloseWithSave(bool saved)
        {
            if (saved)
                if (MessageBox.Show("Do you wish to save your progress?", "Unsaved Progess - Changes will be lost!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (filename == null)
                    {
                        SaveFileDialog save = new SaveFileDialog();
                        save.Title = "Save the Current Spreadsheet";
                        save.Filter = "SS File(*.ss)|*.ss|All files (*.*)|*.*";
                        save.DefaultExt = "ss";
                        save.ShowDialog();
                        if (save.FileName != null)
                            saveSS(save.FileName);
                    }
                    else
                        saveSS(filename);

                }
                else

                    return;
            else
                return; 
        }


        private void SpreadsheetPanel_Load(object sender, EventArgs e)
        {
            AddressBox.Text = "A1";
            ContentBox.Focus();
        }

        private void HelpSelected(object sender, EventArgs e)
        {
            MessageBox.Show("To Enter A Value:\nMake sure the content box above is in focus; type a word, a number, or a formula and press enter.\n" +
                            "\nFile Menu\nNew - Opens a new instance of a blank Spreadsheet." +
                                       "\nLoad - Opens a previously saved Spreadsheet in a new window." + 
                                       "\nSave - Saves the current instance of the Spreadsheet to the desired location.","How to Operate the Spreadsheet");
        }


    }
}
