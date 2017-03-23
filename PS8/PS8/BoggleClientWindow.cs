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
        public BoggleClientWindow()
        {
            InitializeComponent();

            Size = new Size(666, 195);
            popUpMenu.Location = new Point(0,0);
            
            connectButton.MouseClick += registerButtonClick;
        }

        public event Action<string, Uri> registerButtonClicked;

        public void prepareGameWindow()
        {
            Size = new Size(666, 563);
            popUpMenu.Location = new Point(698, 152);
        }

        private void registerButtonClick(object sender, MouseEventArgs j)
        {
            bool validInfo = false;
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
                return;
            }
            catch (FormatException)
            {
                return;
            }
            catch (ArgumentNullException)
            {
                return;
            }
        }
    }
}
