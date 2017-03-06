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
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(1157, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newToolStripMenuItem.Text = "New...";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.HandleSelectNew);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.LoadSelected);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveSelected);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseSelected);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.ContentBox);
            this.panel1.Controls.Add(this.ValueBox);
            this.panel1.Controls.Add(this.AddressBox);
            this.panel1.Controls.Add(this.spreadsheetCellArray);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1157, 462);
            this.panel1.TabIndex = 4;
            // 
            // ContentBox
            // 
            this.ContentBox.Location = new System.Drawing.Point(193, 5);
            this.ContentBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ContentBox.Name = "ContentBox";
            this.ContentBox.Size = new System.Drawing.Size(957, 20);
            this.ContentBox.TabIndex = 3;
            this.ContentBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ContentBox_Keypress);
            // 
            // ValueBox
            // 
            this.ValueBox.Location = new System.Drawing.Point(81, 5);
            this.ValueBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ValueBox.Name = "ValueBox";
            this.ValueBox.ReadOnly = true;
            this.ValueBox.Size = new System.Drawing.Size(101, 20);
            this.ValueBox.TabIndex = 2;
            // 
            // AddressBox
            // 
            this.AddressBox.Location = new System.Drawing.Point(12, 5);
            this.AddressBox.Name = "AddressBox";
            this.AddressBox.ReadOnly = true;
            this.AddressBox.Size = new System.Drawing.Size(61, 20);
            this.AddressBox.TabIndex = 1;
            this.AddressBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // spreadsheetCellArray
            // 
            this.spreadsheetCellArray.AutoSize = true;
            this.spreadsheetCellArray.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.spreadsheetCellArray.Location = new System.Drawing.Point(11, 32);
            this.spreadsheetCellArray.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.spreadsheetCellArray.Name = "spreadsheetCellArray";
            this.spreadsheetCellArray.Size = new System.Drawing.Size(1147, 428);
            this.spreadsheetCellArray.TabIndex = 0;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // SpreadsheetWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1157, 486);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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

