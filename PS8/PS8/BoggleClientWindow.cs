using System;
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

        public event Action<string, Uri> passNameAndUrl;
        public event Action<int> passGameTimeAndStart;
        public event Action cancel;
        public event Action<string> wordPlayed;

        public BoggleClientWindow()
        {
            InitializeComponent();

            Size = new Size(666, 195);
            popUpMenu.Location = new Point(0, 0);
            gameTimeBox.Visible = false;
            connectButton.MouseClick += registerButtonClick;
            playWordButton.MouseClick += playWord;

        }

        private void playWord(object sender, MouseEventArgs e)
        {
            wordPlayed(playWordButton.Text);
        }

        public void prepareGameWindow()
        {
            Size = new Size(802, 560);
            popUpMenu.Location = new Point(1470, 152);
            createArrayOfTiles();
        }

        private void registerButtonClick(object sender, MouseEventArgs j)
        {

            switch (connectButton.Text)
            {

                case "Register":

                    try
                    {
                        string name;
                        if (playerNameBox.Text != null)
                            name = playerNameBox.Text;
                        else
                            throw new ArgumentNullException("name");
                        Uri url;
                        if (!Uri.TryCreate(serverUrL_Box.Text, UriKind.Absolute, out url) && url.Scheme == Uri.UriSchemeHttp)
                        {
                            url = null;
                            throw new UriFormatException();
                        }
                        passNameAndUrl(name, url);



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
                    catch (InvalidConstraintException)
                    {

                    }
                    playerNameBox.Visible = false;
                    connectButton.Text = "Join";
                    serverUrL_Box.Visible = false;
                    gameTimeBox.Visible = true;
                    break;

                case "Join":

                    //Try 
                    int gameTime = int.Parse(gameTimeBox.Text);

                    if (!(gameTime > 5 && gameTime < 120))
                    {
                        throw new InvalidConstraintException();
                    }
                    prepareGameWindow();
                    passGameTimeAndStart(gameTime);
                    gameTimeBox.Visible = false;
                    connectButton.Text = "Cancel";


                    break;
                case "Cancel":
                    cancel();
                    break;
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