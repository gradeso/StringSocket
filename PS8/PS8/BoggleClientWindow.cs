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

            Size = new Size(666, 159);
            popUpMenu.Location = new Point(0,0);
            
            connectButton.MouseClick += registerButtonClick;
            
        }

        public event Action<string, string> registerButtonClicked;
        public event Action preapareGameWindow;
        

        public void prepareGameWindow()
        {
            Size = new Size(666, 563);
            popUpMenu.Location = new Point(698, 152);
        }

        private void registerButtonClick(object sender, MouseEventArgs e)
        {
            string name = playerNameBox.Text;
            string url = serverURL_Box.Text;
            if (name != null && url != null)
                registerButtonClicked(name,url);
        }
    }
}
