﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace PS8
{
    public partial class BoggleClientWindow : Form, IBoggleClientView
    {

        public List<object> tileArray;

        public event Action<string, Uri> registerButtonClicked;


        public BoggleClientWindow()
        {
            InitializeComponent();

            Size = new Size(666, 195);
            popUpMenu.Location = new Point(0,0);
            
            connectButton.MouseClick += registerButtonClick;

            
        }

        public void prepareGameWindow()
        {
            Size = new Size(666, 563);
            popUpMenu.Location = new Point(698, 152);
            createArrayOfTiles();
        }

        private void registerButtonClick(object sender, MouseEventArgs j)
        {
            string name;
            Uri url;

            try
            {
                if (playerNameBox.Text != null)
                    name = playerNameBox.Text;
                else
                    throw new ArgumentNullException("name");

                if (!Uri.TryCreate(serverURL_Box.Text, UriKind.Absolute, out url) && url.Scheme == Uri.UriSchemeHttp)
                {
                    url = null;
                    throw new UriFormatException();
                }

                int gameTime = int.Parse(gameTimeBox.Text);
                
                
                registerButtonClicked(name, url);
                prepareGameWindow();
            }
            catch (UriFormatException)
            {
                //Add popup dialog 
                return;
            }
            catch (FormatException)
            {
                //Add popup dialog
                return;
            }
            catch (ArgumentNullException)
            {
                //Add popup dialog 
                return;
            }


        }
        public void createArrayOfTiles()
        {
            tileArray = new List<object>() { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6,
                textBox7, textBox8, textBox8, textBox9, textBox10, textBox11, textBox12,
                textBox12, textBox13, textBox14, textBox15, textBox16 };
        }

    }
}
