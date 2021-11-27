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

        private static DadosForm dados = new DadosForm();

        public DadosForm()
        {
            InitializeComponent();
            
        }

        private void moveImageBox(object sender)
        {
            Guna2Button b = (Guna2Button)sender;
            imgSlide.Location = new Point(b.Location.X + 108 , b.Location.Y - 34 );
            imgSlide.SendToBack();
            
        }

        private void btnNivel3_CheckedChanged(object sender, EventArgs e)
        {
            moveImageBox(sender);
        }

        public void nivel1()
        {
            dados.Show();
            btnNivel3.Hide();
            btnNivel2.Hide();
        }

        public void nivel2()
        {
            dados.Show();
            btnNivel3.Hide();
            btnNivel1.Hide();
        }

        public void nivel3()
        {
            dados.Show();
            btnNivel1.Hide();
            btnNivel2.Hide();
        }
    }
}
