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
            registerUsernameButton.MouseClick += registerButtonClick;
            
        }

        public event Action<string, string> registerButtonClicked;

        private void registerButtonClick(object sender, MouseEventArgs e)
        {
            string name = playerNameBox.Text;
            string url = serverURL_Box.Text;
            if (name != null && url != null)
                registerButtonClicked(name,url);
        }
    }
}
