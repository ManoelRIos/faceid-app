using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Marerial_design_elements
{
    public partial class DadosForm : Form
    {

        private static FaceRecogniton formLogin = new FaceRecogniton();

        public DadosForm()
        {
            InitializeComponent();
            
        }

        private void moveImageBox(object sender)
        {
            Guna2Button b = (Guna2Button)sender;
            
        }

        private void btnNivel3_CheckedChanged(object sender, EventArgs e)
        {
            moveImageBox(sender);
        }
    }
}
