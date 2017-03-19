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

        private void registerButtonClick(object sender, MouseEventArgs e)
        {
            
        }
    }
}
