using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C_Sharp_Onscreenkeys
{
    public partial class Form_Config : Form
    {
        
        public Form_Config()
        {
            InitializeComponent();
        }
        

        private void bt_FT_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog(panel1);            
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //Properties.Settings.Default.Nome = textBox1.Text;
            Properties.Settings.Default.Save();
        }
    }
}
