using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuzzleWin
{
    public partial class Form2 : Form
    {
        string FN;
        public Form2()
        {
            InitializeComponent();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1(FN);
            this.Hide();
            form1.Show();
        }
        private void btnLoad_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            FN = openFileDialog1.FileName;
        }
    }
}
