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
    public partial class LoadingSignUp : Form
    {
        public LoadingSignUp()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            progressSignUp.Increment(1);
            if(progressSignUp.Value == 100)
            {
                timer1.Enabled = false;
                this.Close();

            }
        }

    }
}
