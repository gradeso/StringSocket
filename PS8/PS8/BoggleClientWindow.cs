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
        private string board;
        private string nickname;
        private string player2;
        private Uri url;
        private bool pending;
        private bool gameActive = true;
        private List<object> tileArray;
        private int gameTime = -1;
        private int timerVal;
        private int score;

        public event Action<string, Uri> registerUserRequest;
        public event Action<int> joinServerRequest;
        public event Action<string> playAWord;
        public event Action CancelJoinRequest;
        

        public string Board { get { return board; } set { board = value; }}

        public bool Pending { get { return pending; } set { pending = value;  prepareGameWindow(); }}

        public string wordPlayed { set { wordPlayed = value; } } //Refresh things
        
        public bool GameActive { get { return gameActive; } set { gameActive = value; if (value == false) { preparePostGameWindow(); } } } 
        
        public int GameTime { get { return gameTime; } set { gameTime = value; } }

        public string Player2 { set { player2 = value; } }
        public int Score { get { return score; } set { score += value; ScoreLabel1.Text = value.ToString(); } }

        /// <summary>
        /// 
        /// </summary>
        public BoggleClientWindow()
        {
            InitializeComponent();

            Size = new Size(660, 295);
            popUpMenu.Location = new Point(0, 0);

            registerButton.MouseClick += registerButtonClick;
            joinButton.MouseClick += joinButtonClick;
            cancelJoinButton.MouseClick += cancelButtonClick;
            playWordButton.MouseClick += playWord;
            playWordTextBox.Click += playWord;
        }


        private void registerButtonClick(object sender, MouseEventArgs j)
        {
            try
            {
                if (playerNameBox.Text != null)
                    nickname = playerNameBox.Text;
                else
                    throw new ArgumentNullException("name");

                Uri url;
                if (!Uri.TryCreate(serverUrL_Box.Text, UriKind.Absolute, out url) && url.Scheme == Uri.UriSchemeHttp)
                {
                    url = null;
                    throw new UriFormatException();
                }
                else
                    this.url = url;
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

            playerNameBox.Enabled = false;
            serverUrL_Box.Enabled = false;
            registerButton.Enabled = false;

            registerUserRequest(nickname, url);
        }

        private void joinButtonClick(object sender, MouseEventArgs e)
        {
            int time;
            if(int.TryParse(gameTimeBox.Text, out time))
            {
                joinServerRequest(time);
            }
            else
            {
                //Handle error
            }

            
        }

        private void cancelButtonClick(object sender, MouseEventArgs e)
        {
            

        }

        private void playWord(object sender, EventArgs e)
        {
            if (playWordTextBox.Text == "Enter Words Here")
                playWordTextBox.Text = "";

            playAWord(playWordTextBox.Text);

        }

        delegate void PrepareWindowCallBack();
        delegate void PostGameCallBack();

        public void prepareGameWindow()
        {
            if(InvokeRequired)
            {
                PrepareWindowCallBack callback = new PrepareWindowCallBack(prepareGameWindow);
                Invoke(callback, new object[] { });
            }
            else
            {
                Size = new Size(802, 560);
                popUpMenu.Location = new Point(1470, 152);

                playerOneLabel.Text = "Player 1 : " + nickname;
                playerTwoLabel.Text = player2 + ": Player 2";
                 
                createArrayOfTiles();

                timerVal = gameTime;
                timerLabel.Text = timerVal + " sec";
                timer.Start();

                populateBoard();
            }
        }

        public void preparePostGameWindow()
        {
            if (InvokeRequired)
            {
                PostGameCallBack callback = new PostGameCallBack(preparePostGameWindow);
                Invoke(callback, new object[] { });
            }
            else
            {
                Size = new Size(660, 295);
                popUpMenu.Location = new Point(0, 0);
            }
        }

        private void populateBoard()
        {
            if (board != null)
            {
                int index = 0;
                foreach (dynamic box in tileArray)
                {
                    box.Text = board.Substring(index, 1);
                    index++;
                    if (index == 16)
                        break;
                }

    
            }
        }

        public void createArrayOfTiles()
        {
            tileArray = new List<object>() { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6,
                textBox7, textBox8, textBox9, textBox10, textBox11, textBox12, textBox13, textBox14, textBox15, textBox16 };
        }

        private void timerTick(object sender, EventArgs e)
        {
            if (timerVal> 0)
            {
                timerVal = timerVal - 1;
                timerLabel.Text = timerVal + " sec";
                
            }

            else
            {
                timer.Stop();
            }
                
        }

        private void enterWordKeyPressed(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == '\r')
            {

            }
        }
    }
}