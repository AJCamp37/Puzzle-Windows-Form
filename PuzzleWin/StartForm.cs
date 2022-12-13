using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuzzleWin
{
    public partial class StartForm : Form
    {
        string FN = "";
        string FT = "";
        public StartForm()
        {
            InitializeComponent();
        }
        private void StartForm_Load(object sender, EventArgs e)
        {
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            if(FN != null)
            {
                
                PuzzleForm form1 = new PuzzleForm(FN, FT);
                this.Hide();
                form1.Show();
            }
        }
       
        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            FN = openFileDialog1.FileName;
            if (FN != "")
               btnStart.Enabled = true;
            else
                btnStart.Enabled = false;

            FT = "IMG";
        }
        
        private void btnLoadSave_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
            FN = openFileDialog2.FileName;
            if (FN != "")
                btnStart.Enabled = true;
            else
                btnStart.Enabled = false;

            FT = "SF";
        }
    }
}
