namespace PS8
{
    partial class BoggleClientWindow
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.helpButton = new System.Windows.Forms.Button();
            this.popUpMenu = new System.Windows.Forms.Panel();
            this.connectButton = new System.Windows.Forms.Button();
            this.serverURL_Box = new System.Windows.Forms.TextBox();
            this.playerNameBox = new System.Windows.Forms.TextBox();
            this.playerTwoBox = new System.Windows.Forms.Label();
            this.playerOneBox = new System.Windows.Forms.Label();
            this.textBox17 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ScoreLabel1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.textBox16 = new System.Windows.Forms.TextBox();
            this.textBox15 = new System.Windows.Forms.TextBox();
            this.textBox14 = new System.Windows.Forms.TextBox();
            this.textBox13 = new System.Windows.Forms.TextBox();
            this.textBox12 = new System.Windows.Forms.TextBox();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox18 = new System.Windows.Forms.TextBox();
            this.gameTimeBox = new System.Windows.Forms.TextBox();
            this.textBox19 = new System.Windows.Forms.TextBox();
            this.ScoreLabel2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.popUpMenu.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel1.Controls.Add(this.popUpMenu);
            this.panel1.Controls.Add(this.ScoreLabel2);
            this.panel1.Controls.Add(this.textBox19);
            this.panel1.Controls.Add(this.helpButton);
            this.panel1.Controls.Add(this.playerTwoBox);
            this.panel1.Controls.Add(this.playerOneBox);
            this.panel1.Controls.Add(this.textBox17);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.ScoreLabel1);
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Controls.Add(this.textBox18);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1179, 794);
            this.panel1.TabIndex = 0;
            // 
            // helpButton
            // 
            this.helpButton.BackColor = System.Drawing.Color.DimGray;
            this.helpButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.helpButton.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.helpButton.Location = new System.Drawing.Point(355, 739);
            this.helpButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(468, 46);
            this.helpButton.TabIndex = 5;
            this.helpButton.Text = "Help";
            this.helpButton.UseVisualStyleBackColor = false;
            // 
            // popUpMenu
            // 
            this.popUpMenu.Controls.Add(this.gameTimeBox);
            this.popUpMenu.Controls.Add(this.connectButton);
            this.popUpMenu.Controls.Add(this.serverURL_Box);
            this.popUpMenu.Controls.Add(this.playerNameBox);
            this.popUpMenu.Location = new System.Drawing.Point(1464, 230);
            this.popUpMenu.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.popUpMenu.Name = "popUpMenu";
            this.popUpMenu.Size = new System.Drawing.Size(940, 246);
            this.popUpMenu.TabIndex = 13;
            // 
            // connectButton
            // 
            this.connectButton.BackColor = System.Drawing.Color.DimGray;
            this.connectButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.connectButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.connectButton.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connectButton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.connectButton.Location = new System.Drawing.Point(10, 185);
            this.connectButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(280, 46);
            this.connectButton.TabIndex = 3;
            this.connectButton.Text = "Register";
            this.connectButton.UseVisualStyleBackColor = false;
            // 
            // serverURL_Box
            // 
            this.serverURL_Box.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serverURL_Box.Location = new System.Drawing.Point(10, 75);
            this.serverURL_Box.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.serverURL_Box.Multiline = true;
            this.serverURL_Box.Name = "serverURL_Box";
            this.serverURL_Box.Size = new System.Drawing.Size(940, 44);
            this.serverURL_Box.TabIndex = 2;
            this.serverURL_Box.Text = "http://cs3500-boggle-s17.azurewebsites.net/BoggleService.svc/";
            // 
            // playerNameBox
            // 
            this.playerNameBox.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playerNameBox.Location = new System.Drawing.Point(10, 15);
            this.playerNameBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.playerNameBox.Multiline = true;
            this.playerNameBox.Name = "playerNameBox";
            this.playerNameBox.Size = new System.Drawing.Size(940, 44);
            this.playerNameBox.TabIndex = 1;
            this.playerNameBox.Text = "wes";
            // 
            // playerTwoBox
            // 
            this.playerTwoBox.AutoSize = true;
            this.playerTwoBox.Font = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playerTwoBox.Location = new System.Drawing.Point(833, 23);
            this.playerTwoBox.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.playerTwoBox.Name = "playerTwoBox";
            this.playerTwoBox.Size = new System.Drawing.Size(175, 43);
            this.playerTwoBox.TabIndex = 12;
            this.playerTwoBox.Text = "Player 2: ";
            this.playerTwoBox.Visible = false;
            // 
            // playerOneBox
            // 
            this.playerOneBox.AutoSize = true;
            this.playerOneBox.Font = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playerOneBox.Location = new System.Drawing.Point(13, 23);
            this.playerOneBox.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.playerOneBox.Name = "playerOneBox";
            this.playerOneBox.Size = new System.Drawing.Size(175, 43);
            this.playerOneBox.TabIndex = 11;
            this.playerOneBox.Text = "Player 1: ";
            this.playerOneBox.Visible = false;
            // 
            // textBox17
            // 
            this.textBox17.Font = new System.Drawing.Font("Century Gothic", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox17.Location = new System.Drawing.Point(355, 571);
            this.textBox17.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new System.Drawing.Size(468, 61);
            this.textBox17.TabIndex = 9;
            this.textBox17.Text = "Enter Words Here";
            this.textBox17.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(496, 643);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(179, 89);
            this.label2.TabIndex = 8;
            this.label2.Text = "0:00";
            this.label2.Visible = false;
            // 
            // ScoreLabel1
            // 
            this.ScoreLabel1.AutoSize = true;
            this.ScoreLabel1.Font = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScoreLabel1.Location = new System.Drawing.Point(13, 91);
            this.ScoreLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ScoreLabel1.Name = "ScoreLabel1";
            this.ScoreLabel1.Size = new System.Drawing.Size(157, 43);
            this.ScoreLabel1.TabIndex = 6;
            this.ScoreLabel1.Text = "Score: 0";
            this.ScoreLabel1.Visible = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.textBox16, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBox15, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBox14, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBox13, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBox12, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBox11, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBox10, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBox9, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBox8, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBox7, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBox6, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBox5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBox4, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBox3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBox2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBox1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(353, 79);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(470, 482);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // textBox16
            // 
            this.textBox16.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textBox16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox16.Font = new System.Drawing.Font("Century Gothic", 33.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox16.Location = new System.Drawing.Point(351, 360);
            this.textBox16.Margin = new System.Windows.Forms.Padding(0);
            this.textBox16.Multiline = true;
            this.textBox16.Name = "textBox16";
            this.textBox16.ReadOnly = true;
            this.textBox16.Size = new System.Drawing.Size(119, 122);
            this.textBox16.TabIndex = 15;
            this.textBox16.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox15
            // 
            this.textBox15.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textBox15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox15.Font = new System.Drawing.Font("Century Gothic", 33.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox15.Location = new System.Drawing.Point(234, 360);
            this.textBox15.Margin = new System.Windows.Forms.Padding(0);
            this.textBox15.Multiline = true;
            this.textBox15.Name = "textBox15";
            this.textBox15.ReadOnly = true;
            this.textBox15.Size = new System.Drawing.Size(117, 122);
            this.textBox15.TabIndex = 14;
            this.textBox15.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox14
            // 
            this.textBox14.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textBox14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox14.Font = new System.Drawing.Font("Century Gothic", 33.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox14.Location = new System.Drawing.Point(117, 360);
            this.textBox14.Margin = new System.Windows.Forms.Padding(0);
            this.textBox14.Multiline = true;
            this.textBox14.Name = "textBox14";
            this.textBox14.ReadOnly = true;
            this.textBox14.Size = new System.Drawing.Size(117, 122);
            this.textBox14.TabIndex = 13;
            this.textBox14.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox13
            // 
            this.textBox13.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textBox13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox13.Font = new System.Drawing.Font("Century Gothic", 33.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox13.Location = new System.Drawing.Point(0, 360);
            this.textBox13.Margin = new System.Windows.Forms.Padding(0);
            this.textBox13.Multiline = true;
            this.textBox13.Name = "textBox13";
            this.textBox13.ReadOnly = true;
            this.textBox13.Size = new System.Drawing.Size(117, 122);
            this.textBox13.TabIndex = 12;
            this.textBox13.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox12
            // 
            this.textBox12.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textBox12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox12.Font = new System.Drawing.Font("Century Gothic", 33.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox12.Location = new System.Drawing.Point(351, 240);
            this.textBox12.Margin = new System.Windows.Forms.Padding(0);
            this.textBox12.Multiline = true;
            this.textBox12.Name = "textBox12";
            this.textBox12.ReadOnly = true;
            this.textBox12.Size = new System.Drawing.Size(119, 120);
            this.textBox12.TabIndex = 11;
            this.textBox12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox11
            // 
            this.textBox11.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textBox11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox11.Font = new System.Drawing.Font("Century Gothic", 33.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox11.Location = new System.Drawing.Point(234, 240);
            this.textBox11.Margin = new System.Windows.Forms.Padding(0);
            this.textBox11.Multiline = true;
            this.textBox11.Name = "textBox11";
            this.textBox11.ReadOnly = true;
            this.textBox11.Size = new System.Drawing.Size(117, 120);
            this.textBox11.TabIndex = 10;
            this.textBox11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox10
            // 
            this.textBox10.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox10.Font = new System.Drawing.Font("Century Gothic", 33.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox10.Location = new System.Drawing.Point(117, 240);
            this.textBox10.Margin = new System.Windows.Forms.Padding(0);
            this.textBox10.Multiline = true;
            this.textBox10.Name = "textBox10";
            this.textBox10.ReadOnly = true;
            this.textBox10.Size = new System.Drawing.Size(117, 120);
            this.textBox10.TabIndex = 9;
            this.textBox10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox9
            // 
            this.textBox9.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textBox9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox9.Font = new System.Drawing.Font("Century Gothic", 33.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox9.Location = new System.Drawing.Point(0, 240);
            this.textBox9.Margin = new System.Windows.Forms.Padding(0);
            this.textBox9.Multiline = true;
            this.textBox9.Name = "textBox9";
            this.textBox9.ReadOnly = true;
            this.textBox9.Size = new System.Drawing.Size(117, 120);
            this.textBox9.TabIndex = 8;
            this.textBox9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox8
            // 
            this.textBox8.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textBox8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox8.Font = new System.Drawing.Font("Century Gothic", 33.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox8.Location = new System.Drawing.Point(351, 120);
            this.textBox8.Margin = new System.Windows.Forms.Padding(0);
            this.textBox8.Multiline = true;
            this.textBox8.Name = "textBox8";
            this.textBox8.ReadOnly = true;
            this.textBox8.Size = new System.Drawing.Size(119, 120);
            this.textBox8.TabIndex = 7;
            this.textBox8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox7
            // 
            this.textBox7.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox7.Font = new System.Drawing.Font("Century Gothic", 33.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox7.Location = new System.Drawing.Point(234, 120);
            this.textBox7.Margin = new System.Windows.Forms.Padding(0);
            this.textBox7.Multiline = true;
            this.textBox7.Name = "textBox7";
            this.textBox7.ReadOnly = true;
            this.textBox7.Size = new System.Drawing.Size(117, 120);
            this.textBox7.TabIndex = 6;
            this.textBox7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox6
            // 
            this.textBox6.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textBox6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox6.Font = new System.Drawing.Font("Century Gothic", 33.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox6.Location = new System.Drawing.Point(117, 120);
            this.textBox6.Margin = new System.Windows.Forms.Padding(0);
            this.textBox6.Multiline = true;
            this.textBox6.Name = "textBox6";
            this.textBox6.ReadOnly = true;
            this.textBox6.Size = new System.Drawing.Size(117, 120);
            this.textBox6.TabIndex = 5;
            this.textBox6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox5
            // 
            this.textBox5.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox5.Font = new System.Drawing.Font("Century Gothic", 33.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox5.Location = new System.Drawing.Point(0, 120);
            this.textBox5.Margin = new System.Windows.Forms.Padding(0);
            this.textBox5.Multiline = true;
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            this.textBox5.Size = new System.Drawing.Size(117, 120);
            this.textBox5.TabIndex = 4;
            this.textBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox4
            // 
            this.textBox4.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox4.Font = new System.Drawing.Font("Century Gothic", 33.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox4.Location = new System.Drawing.Point(351, 0);
            this.textBox4.Margin = new System.Windows.Forms.Padding(0);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(119, 120);
            this.textBox4.TabIndex = 3;
            this.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox3.Font = new System.Drawing.Font("Century Gothic", 33.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(234, 0);
            this.textBox3.Margin = new System.Windows.Forms.Padding(0);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(117, 120);
            this.textBox3.TabIndex = 2;
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.Font = new System.Drawing.Font("Century Gothic", 33.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(117, 0);
            this.textBox2.Margin = new System.Windows.Forms.Padding(0);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(117, 120);
            this.textBox2.TabIndex = 1;
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Font = new System.Drawing.Font("Century Gothic", 33.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Margin = new System.Windows.Forms.Padding(0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(117, 120);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox18
            // 
            this.textBox18.Location = new System.Drawing.Point(831, 154);
            this.textBox18.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox18.Multiline = true;
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new System.Drawing.Size(334, 512);
            this.textBox18.TabIndex = 10;
            // 
            // gameTimeBox
            // 
            this.gameTimeBox.AcceptsReturn = true;
            this.gameTimeBox.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gameTimeBox.Location = new System.Drawing.Point(11, 135);
            this.gameTimeBox.Name = "gameTimeBox";
            this.gameTimeBox.Size = new System.Drawing.Size(305, 37);
            this.gameTimeBox.TabIndex = 4;
            this.gameTimeBox.Text = " Enter Game Time (sec)";
            // 
            // textBox19
            // 
            this.textBox19.Location = new System.Drawing.Point(11, 154);
            this.textBox19.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox19.Multiline = true;
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new System.Drawing.Size(334, 512);
            this.textBox19.TabIndex = 14;
            // 
            // ScoreLabel2
            // 
            this.ScoreLabel2.AutoSize = true;
            this.ScoreLabel2.Font = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScoreLabel2.Location = new System.Drawing.Point(833, 91);
            this.ScoreLabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ScoreLabel2.Name = "ScoreLabel2";
            this.ScoreLabel2.Size = new System.Drawing.Size(157, 43);
            this.ScoreLabel2.TabIndex = 15;
            this.ScoreLabel2.Text = "Score: 0";
            this.ScoreLabel2.Visible = false;
            // 
            // BoggleClientWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1179, 794);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "BoggleClientWindow";
            this.Text = "BOGGLE";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.popUpMenu.ResumeLayout(false);
            this.popUpMenu.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox16;
        private System.Windows.Forms.TextBox textBox15;
        private System.Windows.Forms.TextBox textBox14;
        private System.Windows.Forms.TextBox textBox13;
        private System.Windows.Forms.TextBox textBox12;
        private System.Windows.Forms.TextBox textBox11;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.TextBox serverURL_Box;
        private System.Windows.Forms.TextBox playerNameBox;
        private System.Windows.Forms.Label ScoreLabel1;
        private System.Windows.Forms.Button helpButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox18;
        private System.Windows.Forms.TextBox textBox17;
        private System.Windows.Forms.Label playerTwoBox;
        private System.Windows.Forms.Label playerOneBox;
        private System.Windows.Forms.Panel popUpMenu;
        private System.Windows.Forms.TextBox gameTimeBox;
        private System.Windows.Forms.Label ScoreLabel2;
        private System.Windows.Forms.TextBox textBox19;
    }
}

