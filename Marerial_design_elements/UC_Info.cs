﻿using System;
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
    public partial class UC_Info : UserControl
    {
        public UC_Info()
        {
            InitializeComponent();
        }

        public string InfoText
        {
            get { return infoText.Text; }
            set { infoText.Text = value; }
        }
    }
}
