namespace SpreadsheetGUI
{
	partial class SpreadsheetWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.panel1 = new System.Windows.Forms.Panel();
			this.ContentBox = new System.Windows.Forms.TextBox();
			this.ValueBox = new System.Windows.Forms.TextBox();
			this.AddressBox = new System.Windows.Forms.TextBox();
			this.spreadsheetCellArray = new SpreadsheetGui.SpreadsheetPanel();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.menuStrip1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(1736, 33);
			this.menuStrip1.TabIndex = 3;
			this.menuStrip1.Text = "menuStrip1";
			this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.closeToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 29);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.Size = new System.Drawing.Size(211, 30);
			this.newToolStripMenuItem.Text = "New...";
			this.newToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.HandleSelectNew);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(211, 30);
			this.openToolStripMenuItem.Text = "Open";
			this.openToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.LoadSelected);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(211, 30);
			this.saveToolStripMenuItem.Text = "Save";
			this.saveToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.SaveSelected);
			// 
			// closeToolStripMenuItem
			// 
			this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			this.closeToolStripMenuItem.Size = new System.Drawing.Size(211, 30);
			this.closeToolStripMenuItem.Text = "Close";
			this.closeToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.CloseSelected);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(61, 29);
			this.helpToolStripMenuItem.Text = "Help";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.ContentBox);
			this.panel1.Controls.Add(this.ValueBox);
			this.panel1.Controls.Add(this.AddressBox);
			this.panel1.Controls.Add(this.spreadsheetCellArray);
			this.panel1.Location = new System.Drawing.Point(0, 42);
			this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1736, 708);
			this.panel1.TabIndex = 4;
			// 
			// ContentBox
			// 
			this.ContentBox.Location = new System.Drawing.Point(229, 8);
			this.ContentBox.Name = "ContentBox";
			this.ContentBox.Size = new System.Drawing.Size(1495, 26);
			this.ContentBox.TabIndex = 3;
			this.ContentBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ContentBox_Keypress);
			// 
			// ValueBox
			// 
			this.ValueBox.Location = new System.Drawing.Point(122, 8);
			this.ValueBox.Name = "ValueBox";
			this.ValueBox.ReadOnly = true;
			this.ValueBox.Size = new System.Drawing.Size(90, 26);
			this.ValueBox.TabIndex = 2;
			// 
			// AddressBox
			// 
			this.AddressBox.Location = new System.Drawing.Point(18, 8);
			this.AddressBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.AddressBox.Name = "AddressBox";
			this.AddressBox.ReadOnly = true;
			this.AddressBox.Size = new System.Drawing.Size(90, 26);
			this.AddressBox.TabIndex = 1;
			this.AddressBox.Click += new System.EventHandler(this.AdressBoxClick);
			// 
			// spreadsheetCellArray
			// 
			this.spreadsheetCellArray.Location = new System.Drawing.Point(16, 49);
			this.spreadsheetCellArray.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
			this.spreadsheetCellArray.Name = "spreadsheetCellArray";
			this.spreadsheetCellArray.Size = new System.Drawing.Size(1720, 658);
			this.spreadsheetCellArray.TabIndex = 0;
			this.spreadsheetCellArray.Load += new System.EventHandler(this.ex);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// SpreadsheetWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1736, 748);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.menuStrip1);
			this.Name = "SpreadsheetWindow";
			this.Text = "Spreadsheet";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox AddressBox;
        private SpreadsheetGui.SpreadsheetPanel spreadsheetCellArray;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
		private System.Windows.Forms.TextBox ContentBox;
		private System.Windows.Forms.TextBox ValueBox;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
	}
}

